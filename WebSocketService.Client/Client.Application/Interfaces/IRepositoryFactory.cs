namespace Client.Application.Interfaces
{
	/// <summary>
	/// Фабрика для <see cref="IRepository{TEntity}"/>
	/// </summary>
	public interface IRepositoryFactory
	{
		/// <summary>
		/// Получает указанный репозиторий для <typeparamref name="TEntity"/>.
		/// </summary>
		/// <param name="hasCustomRepository"><c>True</c>, если предоставляется пользовательский репозиторий</param>
		/// <typeparam name="TEntity">Тип сущности.</typeparam>
		/// <returns>Экземпляр типа, унаследованного от интерфейса <see cref="IRepository{TEntity}"/></returns>
		IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;
	}
}
