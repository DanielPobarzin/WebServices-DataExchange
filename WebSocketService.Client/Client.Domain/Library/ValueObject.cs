namespace Client.Domain.Library
{
	/// <summary>
	/// Объект-Значение
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ValueObject<T> where T : ValueObject<T>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerable<object?> GetEqualityAttributes();

		/// <summary>
		/// Определяет, равен ли указанный объект текущему экземпляру
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object? obj) =>
			Equals(obj as T);

		/// <summary>
		/// Определяет, равен ли указанный экземпляр <typeparamref name="T"/> текущему экземпляру
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		private bool Equals(T? other) =>
			other is not null && GetEqualityAttributes().SequenceEqual(other.GetEqualityAttributes());

		/// <summary>
		/// Получить хэш-код объекта
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() =>
			GetEqualityAttributes().Aggregate(17,
				(current, equalityAttribute) => current * 31 + (equalityAttribute?.GetHashCode() ?? 0));

		/// <summary>
		/// Оператор равенства
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(ValueObject<T> a, ValueObject<T> b) => Equals(a, b);

		/// <summary>
		///  Оператор неравенства
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(ValueObject<T> a, ValueObject<T> b) => !(a == b);
		
	}
}
