using Client.Application.Interfaces;
using Client.DataApplicationLayer.Messages.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Client.DataApplicationLayer.UnitOfWork
{
	/// <summary>
	/// Реализация по умолчанию <see cref="IUnitOfWork"/> и <see cref="IUnitOfWork{TContext}"/>
	/// </summary>
	/// <typeparam name="TContext">Тип контекста базы данных</typeparam>
	public sealed class UnitOfWork<TContext> : IRepositoryFactory, IUnitOfWork<TContext>
		where TContext : DbContext
	{
		#region fields

		private bool _disposed;
		private Dictionary<Type, object>? _repositories;

		#endregion

		///<inheritdoc cref="UnitOfWork{TContext}"/>
		/// <param name="context"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public UnitOfWork(TContext context)
		{
			DbContext = context ?? throw new ArgumentNullException(nameof(context));
			LastSaveChangesResult = new SaveChangesResult();
		}

		#region properties

		/// <summary>
		/// Контекст базы данных
		/// </summary>
		/// <returns><typeparamref name="TContext"/></returns>
		public TContext DbContext { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Начать транзакцию 
		/// </summary>
		/// <returns></returns>
		public Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false)
		{
			var transaction = DbContext.Database.CurrentTransaction;
			if (transaction == null)
				return DbContext.Database.BeginTransactionAsync();

			return useIfExists ? Task.FromResult(transaction) : DbContext.Database.BeginTransactionAsync();
		}

		/// <summary>
		/// Начать транзакцию 
		/// </summary>
		/// <returns></returns>
		public IDbContextTransaction BeginTransaction(bool useIfExists = false)
		{
			var transaction = DbContext.Database.CurrentTransaction;
			if (transaction == null)
				return DbContext.Database.BeginTransaction();

			return useIfExists ? transaction : DbContext.Database.BeginTransaction();
		}

		/// <summary>
		/// Откатить транзакцию 
		/// </summary>
		/// <returns></returns>
		public void RollbackTransaction()
		{
			var transaction = DbContext.Database.CurrentTransaction;
			if (transaction is null) return;
			DbContext.Database.RollbackTransaction();
		}

		/// <summary>
		/// Закончить транзакцию 
		/// </summary>
		/// <returns></returns>
		public void CommitTransaction()
		{
			var transaction = DbContext.Database.CurrentTransaction;
			if (transaction is null) return;
			DbContext.SaveChanges();
			DbContext.Database.CommitTransaction();
		}

		/// <summary>
		/// Автоматическое обнаружение изменений
		/// </summary>
		/// <param name="value"></param>
		public void SetAutoDetectChanges(bool value) => DbContext.ChangeTracker.AutoDetectChangesEnabled = value;

		public SaveChangesResult LastSaveChangesResult { get; }

		#endregion

		/// <summary>
		/// Получить репозиторий сущностей <typeparamref name="TEntity"/>.
		/// </summary>
		/// <param name="hasCustomRepository"><c>True</c>, если предоставляется пользовательский репозиторий</param>
		/// <typeparam name="TEntity">Тип сущности</typeparam>
		/// <exception cref="ArgumentNullException"></exception>
		/// <returns><see cref="IRepository{TEntity}"/></returns>
		public IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = false) where TEntity : class
		{
			_repositories ??= new Dictionary<Type, object>();

			if (hasCustomRepository)
				return DbContext.GetService<IRepository<TEntity>>();
			
			var type = typeof(TEntity);
			if (_repositories is not null && !_repositories.ContainsKey(type))
				_repositories[type] = new Repository<TEntity>(DbContext);

			if (_repositories is null) throw new ArgumentNullException();
			return (IRepository<TEntity>)_repositories[type];
		}

		/// <summary>
		/// Сохранить изменения, сделанные в контексте базы данных
		/// </summary>
		/// <returns>Количестыо внесенных изменений</returns>
		public int SaveChanges()
		{
			try
			{
				return DbContext.SaveChanges();
			}
			catch (Exception exception)
			{
				LastSaveChangesResult.Exception = exception;
				return 0;
			}
		}

		/// <summary>
		/// Асинхронно сохранить изменения, сделанные в контексте базы данных
		/// </summary>
		/// <returns><see cref="Task{TResult}"/> представляет асинхронное сохранение изменений. Результат содержит количество внесенных изменений</returns>
		public async Task<int> SaveChangesAsync()
		{
			try
			{
				return await DbContext.SaveChangesAsync();
			}
			catch (Exception exception)
			{
				LastSaveChangesResult.Exception = exception;
				return 0;
			}
		}

		/// <summary>
		/// Сохраняет все изменения, внесенные в этот контекст, в базе данных с использованием распределенной транзакции
		/// </summary>
		/// <param name="unitOfWorks">Необязательный массив <see cref="T:IUnitOfWork"/>.</param>
		/// <returns>Задача <see cref="Task{TResult}"/>, представляющая асинхронную операцию сохранения. Результат задачи содержит количество сущностей состояния, записанных в базу данных.</returns>
		public async Task<int> SaveChangesAsync(params IUnitOfWork[] unitOfWorks)
		{
			var count = 0;
			foreach (var unitOfWork in unitOfWorks)
				count += await unitOfWork.SaveChangesAsync();

			count += await SaveChangesAsync();
			return count;
		}

		/// <summary>
		/// Выполняет задачи, определенные приложением, связанные с освобождением, высвобождением или сбросом неуправляемых ресурсов
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			// ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Выполняет задачи, определенные приложением, связанные с освобождением, высвобождением или сбросом неуправляемых ресурсов
		/// </summary>
		/// <param name="disposing">Указывает, выполняется ли освобождение управляемых ресурсов</param>
		private void Dispose(bool disposing)
		{
			if (!_disposed)
				if (disposing)
				{
					_repositories?.Clear();
					DbContext.Dispose();
				}
			
			_disposed = true;
		}
	}
}
