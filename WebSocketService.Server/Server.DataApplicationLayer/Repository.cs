using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Server.Application.Interfaces;
using System.Linq.Expressions;
using System.Reflection;

namespace Server.DataApplicationLayer
{
	/// <summary>
	/// Представление типизируемого репозитория, реализующего интерфейс the <see cref="IRepository{TEntity}"/>.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		private readonly DbContext _dbContext;
		private readonly DbSet<TEntity> _dbSet;

		/// <inheritdoc cref="Repository{TEntity}"/>
		/// <param name="dbContext">Контекст базы данных</param>
		public Repository(DbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_dbSet = _dbContext.Set<TEntity>();
		}
		#region implementation of IRepository

		#region GetAll
		/// <inheritdoc/>
		public IQueryable<TEntity> GetAll() => _dbSet;

		/// <inheritdoc/>
		public IQueryable<TResult> GetAll<TResult>(
			Expression<Func<TEntity, TResult>> selector,
			Expression<Func<TEntity, bool>>? predicate = null)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);

			return query.Select(selector);
		}

		/// <inheritdoc/>
		public IQueryable<TEntity> GetAll<TResult>(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);

			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();
			
			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();
			
			return orderBy is not null
				? orderBy(query)
				: query;
		}

		/// <inheritdoc/>
		public async Task<IList<TEntity>> GetAllAsync()
			=> await _dbSet.ToListAsync();

		/// <inheritdoc/>
		public async Task<IList<TEntity>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);

			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();

			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();
			
			if (orderBy is not null)
				return await orderBy(query).ToListAsync();

			return await query.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IList<TResult>> GetAllAsync<TResult>(
			Expression<Func<TEntity, TResult>> selector,
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);
			
			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();
			
			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();
			
			return orderBy is not null
				? await orderBy(query).Select(selector).ToListAsync()
				: await query.Select(selector).ToListAsync();
		}
		#endregion

		#region GetList
		/// <inheritdoc/>
		public IList<TEntity> GetList(
			Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);
			
			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();

			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();
			
			return orderBy is not null
				? orderBy(query).ToList()
				: query.ToList();
		}

		/// <inheritdoc/>
		public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false,
			CancellationToken cancellationToken = default)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);
			
			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();
			
			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();

			return orderBy is not null
				? orderBy(query).ToListAsync(cancellationToken)
				: query.ToListAsync(cancellationToken);
		}
		#endregion

		#region GetFirstOrDefault
		/// <inheritdoc/>
		public TEntity? GetFirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);

			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();

			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();

			return orderBy is not null
				? orderBy(query).FirstOrDefault()
				: query.FirstOrDefault();
		}

		/// <inheritdoc/>
		public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
			bool ignoreQueryFilters = false,
			bool ignoreAutoIncludes = false)
		{
			IQueryable<TEntity> query = _dbSet;

			if (predicate is not null)
				query = query.Where(predicate);

			if (ignoreQueryFilters)
				query = query.IgnoreQueryFilters();

			if (ignoreAutoIncludes)
				query = query.IgnoreAutoIncludes();

			return orderBy is not null
				? await orderBy(query).FirstOrDefaultAsync()
				: await query.FirstOrDefaultAsync();
		}
		#endregion

		#region Count

		/// <inheritdoc/>
		public int Count(Expression<Func<TEntity, bool>>? predicate = null) =>
			predicate is null
				? _dbSet.Count()
				: _dbSet.Count(predicate);

		/// <inheritdoc/>
		public async Task<int> CountAsync(
			Expression<Func<TEntity, bool>>? predicate = null,
			CancellationToken cancellationToken = default) =>
			predicate is null
				? await _dbSet.CountAsync(cancellationToken)
				: await _dbSet.CountAsync(predicate, cancellationToken);

		/// <inheritdoc/>
		public long LongCount(Expression<Func<TEntity, bool>>? predicate = null) =>
			predicate is null
				? _dbSet.LongCount()
				: _dbSet.LongCount(predicate);

		/// <inheritdoc/>
		public async Task<long> LongCountAsync(
			Expression<Func<TEntity, bool>>? predicate = null,
			CancellationToken cancellationToken = default) =>
			predicate is null
				? await _dbSet.LongCountAsync(cancellationToken)
				: await _dbSet.LongCountAsync(predicate, cancellationToken);

		#endregion

		#region Exists

		/// <inheritdoc/>
		public bool Exists(Expression<Func<TEntity, bool>>? predicate = null) =>
			predicate is null
				? _dbSet.Any()
				: _dbSet.Any(predicate);

		/// <inheritdoc/>
		public async Task<bool> ExistsAsync(
			Expression<Func<TEntity, bool>>? selector = null,
			CancellationToken cancellationToken = default) =>
			selector is null
				? await _dbSet.AnyAsync(cancellationToken)
				: await _dbSet.AnyAsync(selector, cancellationToken);

		#endregion

		#region Insert

		/// <inheritdoc/>
		public TEntity Insert(TEntity entity) => _dbSet.Add(entity).Entity;

		/// <inheritdoc/>
		public void Insert(params TEntity[] entities) => _dbSet.AddRange(entities);

		/// <inheritdoc/>
		public void Insert(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

		/// <inheritdoc/>
		public ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) =>
			_dbSet.AddAsync(entity, cancellationToken);

		/// <inheritdoc/>
		public Task InsertAsync(params TEntity[] entities) => _dbSet.AddRangeAsync(entities);

		/// <inheritdoc/>
		public Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => _dbSet.AddRangeAsync(entities, cancellationToken);

		#endregion

		#region Update

		/// <inheritdoc/>
		public void Update(TEntity entity) => _dbSet.Update(entity);

		/// <inheritdoc/>
		public void Update(params TEntity[] entities) => _dbSet.UpdateRange(entities);

		/// <inheritdoc/>
		public void Update(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

		/// <inheritdoc/>
		public int ExecuteUpdate(Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate)
			=> _dbSet.ExecuteUpdate(predicate);

		/// <inheritdoc/>
		public Task<int> ExecuteUpdateAsync(
			Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> predicate,
			CancellationToken cancellationToken)
			=> _dbSet.ExecuteUpdateAsync(predicate, cancellationToken);

		#endregion

		#region Delete

		/// <inheritdoc/>
		public void Delete(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

		/// <inheritdoc/>
		public void Delete(params TEntity[] entities) => _dbSet.RemoveRange(entities);

		/// <inheritdoc/>
		public void Delete(TEntity entity) => _dbSet.Remove(entity);

		/// <inheritdoc/>
		public void Delete(object id)
		{
			var typeInfo = typeof(TEntity).GetTypeInfo();
			var key = _dbContext.Model.FindEntityType(typeInfo)?.FindPrimaryKey()?.Properties.FirstOrDefault();
			if (key is null) return;
			
			var property = typeInfo.GetProperty(key.Name);
			if (property != null)
			{
				var entity = Activator.CreateInstance<TEntity>();
				property.SetValue(entity, id);
				_dbContext.Entry(entity).State = EntityState.Deleted;
			}
			else
			{
				var entity = _dbSet.Find(id);
				if (entity != null)
					Delete(entity);
			}
		}

		/// <inheritdoc/>
		public int ExecuteDelete() => _dbSet.ExecuteDelete();

		/// <inheritdoc/>
		public Task<int> ExecuteDeleteAsync(CancellationToken cancellationToken = default) => _dbSet.ExecuteDeleteAsync(cancellationToken);
		
		#endregion

		#endregion
	}
}
