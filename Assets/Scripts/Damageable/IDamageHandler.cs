namespace ShootBalls.Gameplay.Pawn
{
	public interface IDamageHandler
    {
		/// <returns>True if this handler successfully processed the damage.</returns>
		bool Handle( IPawn owner, IDamageData data );
	}

	public abstract class DamageHandler<TData> : IDamageHandler
		where TData : IDamageData
	{
		public bool Handle( IPawn owner, IDamageData data )
		{
			return Handle( owner, (TData)data );
		}

		protected abstract bool Handle( IPawn owner, TData data );
	}

	public abstract class DamageHandler<TOwner, TData> : IDamageHandler
		where TOwner : IPawn
		where TData : IDamageData
	{
		public bool Handle( IPawn owner, IDamageData data )
		{
			return Handle( (TOwner)owner, (TData)data );
		}

		protected abstract bool Handle( TOwner owner, TData data );
	}
}
