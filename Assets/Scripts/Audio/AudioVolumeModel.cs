using System.Collections.Generic;

namespace ShootBalls.Gameplay.Audio
{
	public class AudioVolumeModel
	{
		public delegate void OnMixerVolumeChanged( string mixerId, float volume );
		public event OnMixerVolumeChanged MixerVolumeChanged;

		private readonly Dictionary<string, float> _volumeByMixer = new Dictionary<string, float>();

		public void SetVolume( string mixerId, float volume )
		{
			_volumeByMixer[mixerId] = volume;
			MixerVolumeChanged?.Invoke( mixerId, volume );
		}

		public float GetVolume( string mixerId )
		{
			return _volumeByMixer[mixerId];
		}
	}
}