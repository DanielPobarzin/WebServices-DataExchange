using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Server.Application.Interfaces
{
	/// <summary>
	/// Репозиторий
	/// </summary>
	/// <typeparam name="TEntity">Тип сущности в репозитории</typeparam>
	public interface IRepository<TEntity> where TEntity : class
	{
		#region GetList

		/// <summary>
		/// Получает <see cref="IList{T}"/> на основе условия и делегата для сортировки
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на соответствие условию</param>
		/// <param name="orderBy">Функция для сортировки элементов</param>
		/// <param name="ignoreQueryFilters">Игнорировать фильтры запросов</param>
		/// <param name="ignoreAutoIncludes">Игнорировать автоматические включения</param>
		/// <returns>Экземпляр <see cref="IList{TEntity}"/>, который содержит элементы, удовлетворяющие условию, указанному в <paramref name="predicate"/>
		/// </returns>
		IList<TEntity> GetList(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false);

		/// <summary>
		/// Получает <see cref="IList{T}"/> на основе условия и делегата для сортировки
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на соответствие условию</param>
		/// <param name="orderBy">Функция для сортировки элементов</param>
		/// <param name="ignoreQueryFilters">Игнорировать фильтры запросов</param>
		/// <param name="ignoreAutoIncludes">Игнорировать автоматические включения</param>
		/// <param name="cancellationToken">
		///     Токен отмены <see cref='CancellationToken' /> для наблюдения во время ожидания завершения задачи
		/// </param>
		/// <returns>Экземпляр <see cref="IList{TEntity}"/>, который содержит элементы, удовлетворяющие условию, указанному в <paramref name="predicate"/></returns>
		Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false,
			CancellationToken cancellationToken = default);

		#endregion

		#region GetFirstOrDefault

		/// <summary>
		/// Получает первый элемент или значение по умолчанию на основе условия, делегата orderBy.
		/// Этот метод по умолчанию выполняет запрос только для чтения 
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на соответствие условию</param>
		/// <param name="orderBy">Функция для упорядочивания элементов</param>
		/// <param name="ignoreQueryFilters">Игнорировать фильтры запроса</param>
		/// <param name="ignoreAutoIncludes">Игнорировать автоматические включения</param>
		/// <returns>Объект <see cref="IList{TEntity}"/>, содержащий элементы, удовлетворяющие условию <paramref name="predicate"/>
		/// </returns>
		TEntity? GetFirstOrDefault(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false);

		/// <summary>
		/// Получает первый элемент или значение по умолчанию на основе условия, делегата orderBy.
		/// Этот метод по умолчанию выполняет запрос только для чтения 
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на соответствие условию</param>
		/// <param name="orderBy">Функция для упорядочивания элементов</param>
		/// <param name="ignoreQueryFilters">Игнорировать фильтры запроса</param>
		/// <param name="ignoreAutoIncludes">Игнорировать автоматические включения</param>
		/// <returns>Объект <see cref="IList{TEntity}"/>, содержащий элементы, удовлетворяющие условию <paramref name="predicate"/>
		/// </returns>
		Task<TEntity?> GetFirstOrDefaultAsync(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false);

		#endregion

		#region GetAll

		/// <summary>
		/// Получить все сущности
		/// </summary>
		/// <returns><see cref="IQueryable{TEntity}"/></returns>
		IQueryable<TEntity> GetAll();

		/// <summary>
		/// Получить все сущности
		/// </summary>
		/// <param name="selector">Селектор</param>
		/// <param name="predicate">Функция для проверки каждого элемента на выполнение условия</param>
		/// <returns>The <see cref="IQueryable{TEntity}"/>.</returns>
		IQueryable<TResult> GetAll<TResult>(
			Expression<Func<TEntity, TResult>> selector,
			Expression<Func<TEntity, bool>>? predicate = null);

		/// <summary>
		/// Получить все сущности
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на выполнение условия</param>
		/// <param name="orderBy">Функция для упорядочивания элементов</param>
		/// <param name="ignoreQueryFilters">Игнорировать фильтры запроса.</param>
		/// <param name="ignoreAutoIncludes">Игнорировать автоматические включения.</param>
		/// <returns>Объект <see cref="IQueryable{TEntity}"/>, содержащий элементы, удовлетворяющие условию, указанному в <paramref name="predicate"/>.</returns>
		IQueryable<TEntity> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false);

		/// <summary>
		/// Получить все сущности
		/// </summary>
		/// <returns><see cref="IList{TEntity}"/></returns>
		Task<IList<TEntity>> GetAllAsync();


		/// <summary>
		/// Получить все сущности
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на выполнение условия</param>
		/// <param name="orderBy">Функция для упорядочивания элементов</param>
		/// <param name="ignoreQueryFilters">Игнорировать фильтры запроса.</param>
		/// <param name="ignoreAutoIncludes">Игнорировать автоматические включения.</param>
		/// <returns>Объект <see cref="IList{TEntity}"/>, содержащий элементы, удовлетворяющие условию, указанному в <paramref name="predicate"/>.</returns>
		Task<IList<TEntity>> GetAllAsync<TResult>(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false);

		/// <summary>
		/// Получить все сущности
		/// </summary>
		/// <param name="predicate">Функция для проверки каждого элемента на выполнение условия</param>
		/// <param name="selector">Селектор</param>
		/// <param name="orderBy">Функция для упорядочивания элементов</param>
		/// <param name="ignoreQueryFilters">Ignore query filters</param>
		/// <param name="ignoreAutoIncludes">Ignore automatic includes</param>
		/// <returns>Объект <see cref="IList{TEntity}"/>, содержащий элементы, удовлетворяющие условию, указанному в <paramref name="predicate"/>.</returns>
		Task<IList<TResult>> GetAllAsync<TResult>(
			Expression<Func<TEntity, TResult>> selector,
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false);

		#endregion

		#region Insert

		/// <summary>
		/// Вставить новую сущность синхронно
		/// </summary>
		/// <param name="entity">Элемент для вставки</param>
		TEntity Insert(TEntity entity);

		/// <summary>
		/// Вставить диапазон сущностей синхронно
		/// </summary>
		/// <param name="entities">Сущности для вставки.</param>
		void Insert(params TEntity[] entities);

		/// <summary>
		/// Вставить диапазон сущностей синхронно
		/// </summary>
		/// <param name="entities">Сущности для вставки</param>
		void Insert(IEnumerable<TEntity> entities);

		/// <summary>
		/// Вставить новую сущность асинхронно
		/// </summary>
		/// <param name="entity">Сущность для вставки</param>
		/// <param name="cancellationToken">Токен отмены <see cref="CancellationToken"/>, который можно использовать для отслеживания завершения задачи</param>
		ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Вставить диапазон сущностей асинхронно
		/// </summary>
		/// <param name="entities">Сущности для вставки</param>
		Task InsertAsync(params TEntity[] entities);

		/// <summary>
		/// Вставить диапазон сущностей асинхронно
		/// </summary>
		/// <param name="entities">Сущности для вставки</param>
		/// <param name="cancellationToken">Токен отмены <see cref="CancellationToken"/>, который можно использовать для отслеживания завершения задачи</param>
		Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

		#endregion

		#region Update

		/// <summary>
		/// Обновить сущность
		/// </summary>
		/// <param name="entity"></param>
		void Update(TEntity entity);

		/// <summary>
		/// Обновить сущности
		/// </summary>
		/// <param name="entities"></param>
		void Update(params TEntity[] entities);

		/// <summary>
		/// Обновить сущности
		/// </summary>
		/// <param name="entities">The entities</param>
		void Update(IEnumerable<TEntity> entities);

		/// <summary>
		///     Обновляет все строки базы данных для экземпляров сущностей, которые соответствуют LINQ-запросу из базы данных.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Эта операция выполняется немедленно в базе данных, а не откладывается до вызова
		///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" />
		///     </para>
		/// </remarks>
		/// <param name="predicate">Условие</param>
		/// <returns>Общее количество строк, обновленных в базе данных.</returns>
		int ExecuteUpdate(Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate);

		/// <summary>
		///     Асинхронно обновляет все строки базы данных для экземпляров сущностей, которые соответствуют LINQ-запросу из базы данных.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Эта операция выполняется немедленно в базе данных, а не откладывается до вызова
		///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" />
		///     </para>
		/// </remarks>
		/// <param name="predicate">Условие</param>
		/// <param name="cancellationToken">Токен отмены <see cref="CancellationToken"/>, который можно использовать для отслеживания завершения задачи</param>
		/// <returns>Общее количество строк, обновленных в базе данных.</returns>
		public Task<int> ExecuteUpdateAsync(
			Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate,
			CancellationToken cancellationToken);

		#endregion

		#region Delete

		/// <summary>
		/// Удалить сущность по уникальному идентификатору
		/// </summary>
		/// <param name="uniqueUId"></param>
		void Delete(object uniqueUId);

		/// <summary>
		/// Удалить сущность
		/// </summary>
		/// <param name="entity"></param>
		void Delete(TEntity entity);

		/// <summary>
		/// Удалить сущности
		/// </summary>
		/// <param name="entities"></param>
		void Delete(params TEntity[] entities);

		/// <summary>
		/// Удалить сущности
		/// </summary>
		/// <param name="entities"></param>
		void Delete(IEnumerable<TEntity> entities);

		/// <summary>
		///     Удалить все экземпляры сущностей в источнике, которые соответствуют LINQ-запросу
		/// </summary>
		/// <remarks>
		///     <para>
		///         Эта операция выполняется немедленно в базе данных, а не откладывается до вызова
		///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" />
		///     </para>
		/// </remarks>
		/// <returns>Колличество удаленных экземпляров сущностей</returns>
		int ExecuteDelete();

		/// <summary>
		///     Асинхронно удалить все экземпляры сущностей в источнике, которые соответствуют LINQ-запросу
		/// </summary>
		/// <remarks>
		///     <para>
		///         Эта операция выполняется немедленно в базе данных, а не откладывается до вызова
		///         <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges" />
		///     </para>
		/// </remarks>
		/// <param name="cancellationToken">Токен отмены <see cref="CancellationToken"/>, который можно использовать для отслеживания завершения задачи</param>
		/// <returns>Колличество удаленных экземпляров сущностей</returns>
		Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default);

		#endregion

		#region Count

		/// <summary>
		/// Получить количество сущностей, соответсвующих условию
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		int Count(Expression<Func<TEntity, bool>>? predicate = null);

		/// <summary>
		/// Асинхронно получить количество сущностей, соответсвующих условию
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Получить общее количество на основе условия
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		long LongCount(Expression<Func<TEntity, bool>>? predicate = null);

		/// <summary>
		/// Асинхронно получить общее количество на основе условия
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		Task<long> LongCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

		#endregion

		#region Exists

		/// <summary>
		/// Проверить наличие сущности по условию
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		bool Exists(Expression<Func<TEntity, bool>>? predicate = null);

		/// <summary>
		/// Асинхронно роверить наличие сущности по условию
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null, CancellationToken cancellationToken = default);

		#endregion

	}
}
