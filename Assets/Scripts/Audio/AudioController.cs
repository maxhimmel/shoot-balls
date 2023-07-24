using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using Zenject;

namespace ShootBalls.Gameplay.Audio
{
	public class AudioController : MonoBehaviour,
		IAudioController
    {
		[BoxGroup]
		[SerializeField] private AudioMixer _masterMixer;
		[BoxGroup]
		[SerializeField] private AudioSource[] _sourcePrefabs;

		[Space]
        [SerializeField] private AudioBank[] _banks;

        private readonly Dictionary<string, BankEvent> _events = new Dictionary<string, BankEvent>();
		private readonly Dictionary<string, ObjectPool<AudioSource>> _sourcesByMixer = new Dictionary<string, ObjectPool<AudioSource>>();
		private readonly List<AudioSource> _playingSources = new List<AudioSource>();

		private AudioVolumeModel _volumeModel;

		[Inject]
		public void Construct( AudioVolumeModel volumeModel )
		{
			_volumeModel = volumeModel;
		}

		private void Awake()
		{
			// Init event lookup ...
			foreach ( var bank in _banks )
			{
				foreach ( var data in bank.Events )
				{
					string key = bank.ExportKey( data );
					_events.Add( key, data );
				}
			}

			// Init audio source lookup ...
			foreach ( var source in _sourcePrefabs )
			{
				_sourcesByMixer.Add(
					source.outputAudioMixerGroup.name, new ObjectPool<AudioSource>(
						createFunc:			() => Instantiate( source, transform ),
						actionOnGet:		source => source.gameObject.SetActive( true ),
						actionOnRelease:	source => source.gameObject.SetActive( false ),
						actionOnDestroy:	source => Destroy( source.gameObject ),
						maxSize:			100
				) );
			}

			_volumeModel.MixerVolumeChanged += OnMixerVolumeChanged;
		}

		public UniTask LoadBank( string category )
		{
			var bank = Array.Find( _banks, b => b.Category == category );
			if ( bank == null )
			{
				throw new KeyNotFoundException( category );
			}

			foreach ( var data in bank.Events )
			{
				data.Clip.LoadAudioData();
			}

			return UniTask.CompletedTask;
		}

		public UniTask UnloadBank( string category )
		{
			var bank = Array.Find( _banks, b => b.Category == category );
			if ( bank == null )
			{
				throw new KeyNotFoundException( category );
			}

			foreach ( var data in bank.Events )
			{
				data.Clip.UnloadAudioData();
			}

			return UniTask.CompletedTask;
		}

		public float GetVolume( string category )
		{
			_masterMixer.GetFloat( GetVolumeKey( category ), out var volume );
			return Mathf.Pow( 10, volume / 20f );
		}

		public void SetVolume( string category, float volume )
		{
			volume = Mathf.Clamp( volume, 0.001f, 1 );
			float convertedVolume = Mathf.Log10( volume ) * 20;
			bool isSuccess = _masterMixer.SetFloat( GetVolumeKey( category ), convertedVolume );

			if ( !isSuccess )
			{
				throw new NotImplementedException( $"'{_masterMixer.name}/{category}' does not expose a 'Volume' parameter." );
			}
		}

		private string GetVolumeKey( string category )
		{
			return $"{category}/Volume";
		}

		public IEventInstance PlayOneShot( string key, Vector2 position )
		{
			IEventInstance instance = CreateInstance( key );
			instance.SetOrientation( new Orientation( position ) );
			instance.Play();

			return instance;
		}

		public IEventInstance CreateInstance( string key )
		{
			if ( !_events.TryGetValue( key, out var data ) )
			{
				throw new KeyNotFoundException( key );
			}

			var mixerKey = key.Split( '/' )[0];
			var sourcePool = _sourcesByMixer[mixerKey];
			var source = sourcePool.Get();

			// 3D ...
			source.spatialBlend = data.Is3d ? 1 : 0;
			source.minDistance = data.DistanceRange.x;
			source.maxDistance = data.DistanceRange.y;

			// General ...
			source.volume = data.Volume;
			source.pitch = data.PitchRange.Random();
			source.clip = data.Clip;

			_playingSources.Add( source );
			return new EventInstance( source );
		}

		private void Update()
		{
			for ( int idx = _playingSources.Count - 1; idx >= 0; --idx )
			{
				var source = _playingSources[idx];
				if ( !source.isPlaying )
				{
					string mixerKey = source.outputAudioMixerGroup.name;
					var sourcePool = _sourcesByMixer[mixerKey];

					sourcePool.Release( source );
					_playingSources.RemoveAt( idx );
				}
			}
		}

		private void OnDestroy()
		{
			foreach ( var kvp in _sourcesByMixer )
			{
				kvp.Value.Dispose();
			}

			_volumeModel.MixerVolumeChanged -= OnMixerVolumeChanged;
		}

		private void OnMixerVolumeChanged( string mixerId, float volume )
		{
			SetVolume( mixerId, volume );
		}


		/*---*/


		private class EventInstance : IEventInstance
		{
			public bool IsPlaying => _source.isPlaying;

			private readonly AudioSource _source;

			public EventInstance( AudioSource source )
			{
				_source = source;
			}

			public void Play()
			{
				if ( _source.clip.loadState != AudioDataLoadState.Loaded )
				{
					Debug.LogWarning( $"Playing '<b>{_source.clip.name}</b>' clip before loading. This may cause lag.\n" +
						$"Try using '<b>{nameof( IAudioController )}.{nameof( LoadBank )}</b>' prior to this call.", _source.clip );
				}

				_source.Play();
			}

			public void Stop()
			{
				if ( _source != null )
				{
					_source.Stop();
				}
			}

			public void Pause()
			{
				if ( _source != null )
				{
					_source.Pause();
				}
			}

			public void Resume()
			{
				if ( _source != null )
				{
					_source.UnPause();
				}
			}

			public void SetOrientation( IOrientation orientation )
			{
				if ( _source != null )
				{
					_source.transform.SetPositionAndRotation(
						orientation.Position,
						orientation.Rotation
					);
					//_source.transform.SetParent( orientation.Parent );
				}
			}
		}
	}
}
