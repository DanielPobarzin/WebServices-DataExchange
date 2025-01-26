namespace Client.Domain.Library
{
	/// <summary>
	/// Сущность
	/// </summary>
	/// <typeparam name="TUniqueId"></typeparam>
	public abstract class Entity<TUniqueId> : IEquatable<Entity<TUniqueId>>
	{
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		public abstract TUniqueId UniqueId { get; }

		/// <summary>
		/// Сравние с объектом
		/// </summary>
		/// <returns></returns>
		public override bool Equals(object? obj)
		{
			if (obj is Entity<TUniqueId> other)
			{
				return Equals(other);
			}
			return false;
		}

		/// <summary>
		/// Сравние с другой сущностью <see cref="Entity{TUniqueId}"/>
		/// </summary>
		/// <returns></returns>
		public bool Equals(Entity<TUniqueId>? other) => 
			other is not null && UniqueId!.Equals(other.UniqueId);
		
		/// <summary>
		/// Получить хэш-код сущности 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() =>
			UniqueId!.GetHashCode();

		/// <summary>
		/// Оператор сравнения на равенство
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Entity<TUniqueId>? left, Entity<TUniqueId>? right) => 
			Equals(left, right);
		
		/// <summary>
		/// Оператор сравнения на неравенство
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(Entity<TUniqueId>? left, Entity<TUniqueId>? right) =>
			!(left == right);
	}
}
