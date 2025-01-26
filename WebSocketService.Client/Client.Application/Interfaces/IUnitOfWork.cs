using Microsoft.EntityFrameworkCore;

namespace Client.Application.Interfaces
{
	public interface IUnitOfWork<out TContext> : IUnitOfWork
		where TContext : DbContext
	{
		/// <summary>
		/// Получить контекст базы данных
		/// </summary>
		/// <returns><typeparamref name="TContext"/></returns>
		TContext DbContext { get; }

		/// <summary>
		/// Сохраняет все изменения, внесенные в этот контекст, в базе данных с использованием распределенной транзакции.
		/// </summary>
		/// <param name="unitOfWorks">Необязательный массив <see cref="IUnitOfWork{TAgent}"/></param>
		/// <returns><see cref="Task{TResult}"/>, представляющий асинхронную операцию сохранения. Результат задачи содержит количество сущностей состояния, записанных в базу данных.</returns>
		Task<int> SaveChangesAsync(params IUnitOfWork[] unitOfWorks);

	}

	public interface IUnitOfWork : IDisposable
	{
		/// <summary>
		/// Получить репозиторий <typeparamref name="TEntity"/>.
		/// </summary>
		/// <param name="hasCustomRepository"><c>True</c>, если предоставляется пользовательский репозиторий</param>
		/// <typeparam name="TEntity">Тип сущности</typeparam>
		/// <returns>Экземпляр типа, наследуемого от интерфейса <see cref="IRepository{TEntity}"/></returns>
		IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class;

		/// <summary>
		/// Сохранить все изменения, внесенные в этот контекст, в базе данных
		/// </summary>
		/// <returns>Количество записей состояния, записанных в базу данных</returns>
		int SaveChanges();

		/// <summary>
		/// Асинхронно сохраняет все изменения, внесенные в эту единицу работы, в базе данных
		/// </summary>
		/// <returns><see cref="Task{TResult}"/>, представляющий асинхронную операцию сохранения. Результат задачи содержит количество сущностей состояния, записанных в базу данных</returns>
		Task<int> SaveChangesAsync();
		
	}
}
