using System;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Audio
{
    public class MusicPlayer : IInitializable,
		ITickable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly IAudioController _audioController;
		private readonly AudioEventReference[] _trackEvents;

		private float _nextPlayTime;
		private int _nextTrackIndex;
		private IEventInstance _currentEvent;

		public MusicPlayer( Settings settings,
			IAudioController audioController )
		{
			_settings = settings;
			_audioController = audioController;

			int trackCount = settings.Tracks.Length;
			_trackEvents = new AudioEventReference[trackCount];
			Array.Copy( settings.Tracks, _trackEvents, trackCount );
		}

		public void Initialize()
		{
			_trackEvents.FisherYatesShuffle();
			_nextPlayTime = Time.timeSinceLevelLoad + _settings.NextTrackDelay;
		}

		public void Dispose()
		{
			if ( _currentEvent != null )
			{
				_currentEvent.Stop();
			}
		}

		public void Tick()
		{
			if ( CanPlayNextTrack() )
			{
				string musicKey = GetNextTrack();
				_currentEvent = _audioController.PlayOneShot( musicKey, Vector2.zero );
			}
		}

		private bool CanPlayNextTrack()
		{
			if ( !Application.isFocused && !Application.runInBackground )
			{
				return false;
			}

			if ( _nextPlayTime > Time.timeSinceLevelLoad )
			{
				return false;
			}

			return _currentEvent == null || !_currentEvent.IsPlaying;
		}

		private string GetNextTrack()
		{
			if ( _nextTrackIndex >= _trackEvents.Length )
			{
				_nextTrackIndex = 0;
				_trackEvents.FisherYatesShuffle();
			}

			var eventRef = _trackEvents[_nextTrackIndex++];
			return eventRef.EventName;
		}

		[System.Serializable]
		public class Settings
		{
			public float NextTrackDelay;
			public AudioEventReference[] Tracks;
		}
	}
}
