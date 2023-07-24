using System.Collections.Generic;

namespace ShootBalls.Gameplay.Audio
{
	public class AudioVolumeModel
	{
		public delegate void OnMixerVolumeChanged( string mixerId, float volume );
		public event OnMixerVolumeChanged MixerVolumeChanged;

		private readonly Dictionary<string, float> _volumeByMixer;
	}
}