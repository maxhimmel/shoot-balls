using Cysharp.Threading.Tasks;

namespace ShootBalls.Gameplay.Audio
{
	public class AudioBankLoader
	{
		private readonly Settings _settings;
		private readonly IAudioController _audioController;

		public AudioBankLoader( Settings settings,
			IAudioController audioController )
		{
			_settings = settings;
			_audioController = audioController;
		}

		public async UniTask LoadBanks()
		{
			await UniTask.WhenAll( _settings.LoadBanks.Select( 
				bank => _audioController.LoadBank( bank ) 
			) );
		}

		public async UniTask UnloadBanks()
		{
			await UniTask.WhenAll( _settings.LoadBanks.Select(
				bank => _audioController.UnloadBank( bank )
			) );
		}

		[System.Serializable]
		public class Settings
		{
			public string[] LoadBanks;
		}
	}
}