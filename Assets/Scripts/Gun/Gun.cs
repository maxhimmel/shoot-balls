using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Weapons
{
	public class Gun : IFixedTickable
	{
		public event System.Action<Gun, IAmmoHandler> Emptied;

		public bool IsFiring => _isFiringRequested;
		public AmmoData AmmoData => _ammoHandler != null ? _ammoHandler.AmmoData : AmmoData.Full;

		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly Projectile.Factory _factory;
		private readonly ShotSpot _shotSpot;
		private readonly IFireSpread _fireSpread;
		private readonly IAmmoHandler _ammoHandler;
		private readonly IFireSafety[] _fireSafeties;
		private readonly IProjectileFiredProcessor[] _projectileFiredProcessors;
		private readonly IFireStartProcessor[] _fireStartProcessors;
		private readonly IFireEndProcessor[] _fireEndProcessors;
		private readonly IPreFireProcessor[] _preFireProcessors;
		private readonly IGunTickable[] _tickables;

		private IPawn _owner;
		private bool _isFiringRequested;
		private bool _isEmptied;

		public Gun( Settings settings,
			SignalBus signalBus,
			Projectile.Factory factory,
			ShotSpot shotSpot,
			IFireSpread fireSpread,

			[InjectOptional] IAmmoHandler ammoHandler,
			[InjectOptional] IFireSafety[] safeties,
			[InjectOptional] IProjectileFiredProcessor[] projectileFiredProcessors,
			[InjectOptional] IFireStartProcessor[] fireStartProcessors,
			[InjectOptional] IFireEndProcessor[] fireEndProcessors,
			[InjectOptional] IPreFireProcessor[] preFireProcessors,
			[InjectOptional] IGunTickable[] tickables )
		{
			_settings = settings;
			_signalBus = signalBus;
			_factory = factory;
			_shotSpot = shotSpot;
			_fireSpread = fireSpread;
			_ammoHandler = ammoHandler;
			_fireSafeties = safeties ?? new IFireSafety[0];
			_projectileFiredProcessors = projectileFiredProcessors ?? new IProjectileFiredProcessor[0];
			_fireStartProcessors = fireStartProcessors ?? new IFireStartProcessor[0];
			_fireEndProcessors = fireEndProcessors ?? new IFireEndProcessor[0];
			_preFireProcessors = preFireProcessors ?? new IPreFireProcessor[0];
			_tickables = tickables ?? new IGunTickable[0];

			if ( ammoHandler != null )
			{
				ammoHandler.Emptied += () => _isEmptied = true;
			}
		}

		public void SetOwner( IPawn owner )
		{
			_owner = owner;
		}

		public void StartFiring()
		{
			_isFiringRequested = true;
		}

		public void StopFiring()
		{
			_isFiringRequested = false;
		}

		public void FixedTick()
		{
			if ( _isFiringRequested && CanFire() )
			{
				ProcessFireStarting();
				{
					HandleFiring();
				}
				ProcessFireEnding();
			}

			ProcessTickables();
			HandleEmptiedNotification();
		}

		private bool CanFire()
		{
			foreach ( var safety in _fireSafeties )
			{
				if ( !safety.CanFire() )
				{
					return false;
				}
			}
			return true;
		}

		private void ProcessFireStarting()
		{
			foreach ( var process in _fireStartProcessors )
			{
				process.FireStarting();
			}
		}

		private void HandleFiring()
		{
			_settings.ProjectileSettings.Owner = _owner;

			int spreadCount = 0;
			Vector2 avgShotOrigin = Vector2.zero;
			Vector3 avgShotDirection = Vector3.zero;

			foreach ( var shotSpot in _fireSpread.GetSpread( _shotSpot ) )
			{
				var processedShotSpot = PreProcessShotSpot( shotSpot );

				var newProjectile = Fire( processedShotSpot );
				NotifyProjectileFired( newProjectile );

				++spreadCount;
				avgShotOrigin += processedShotSpot.Position;
				avgShotDirection += processedShotSpot.Rotation * Vector2.up;
			}

			_signalBus.FireId( "Attacked", new FxSignal() {
				Position = avgShotOrigin / spreadCount,
				Direction = avgShotDirection / spreadCount,
				Parent = _shotSpot.Transform
			} );
		}

		private IOrientation PreProcessShotSpot( IOrientation shotSpot )
		{
			foreach ( var processor in _preFireProcessors )
			{
				processor.PreFire( ref shotSpot );
			}

			return shotSpot;
		}

		private Projectile Fire( IOrientation orientation )
		{
			Vector2 direction = orientation.Rotation * Vector2.up;

			Projectile newProjectile = _factory.Create( _settings.ProjectileSettings );
			newProjectile.Body.HolisticMove( orientation.Position, direction.ToLookRotation() );

			Vector2 projectileImpulse = direction * _settings.ProjectileSpeed;
			newProjectile.Launch( projectileImpulse );

			return newProjectile;
		}

		private void NotifyProjectileFired( Projectile firedProjectile )
		{
			foreach ( var processor in _projectileFiredProcessors )
			{
				processor.Notify( firedProjectile );
			}
		}

		private void ProcessFireEnding()
		{
			foreach ( var processor in _fireEndProcessors )
			{
				processor.FireEnding();
			}
		}

		private void ProcessTickables()
		{
			if ( _tickables != null )
			{
				foreach ( var tickable in _tickables )
				{
					tickable.FixedTick();
				}
			}
		}

		private void HandleEmptiedNotification()
		{
			if ( _isEmptied )
			{
				_isEmptied = false;
				Emptied?.Invoke( this, _ammoHandler );
			}
		}

		public void Reload()
		{
			_ammoHandler?.Reload();
		}

		[System.Serializable]
		public class Settings
		{
			[BoxGroup( "Gameplay" )]
			public Transform ShotSpot;

			[BoxGroup( "Gameplay/Projectile", ShowLabel = false )]
			public string ProjectilePoolId;
			[BoxGroup( "Gameplay/Projectile", ShowLabel = false )]
			public GameObject ProjectilePrefab;
			[BoxGroup( "Gameplay/Projectile", ShowLabel = false ), HideLabel]
			public Projectile.Settings ProjectileSettings;
			[BoxGroup( "Gameplay/Projectile", ShowLabel = false )]
			public float ProjectileSpeed;

			[BoxGroup( "Gameplay/Required" )]
			[HideReferenceObjectPicker]
			[SerializeReference] public IFireSpread.ISettings FireSpread;

			[FoldoutGroup( "Gameplay/Optional" ), OnInspectorGUI]
			[InfoBox( "Right-click a module foldout to change its type.", InfoMessageType.None )]

			[FoldoutGroup( "Gameplay/Optional" )]
			[HideReferenceObjectPicker, ListDrawerSettings( ListElementLabelName = "GetModuleLabel" )]
			[SerializeReference] public IGunModule[] Modules;
		}
	}
}
