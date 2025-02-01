using FunctionalProgramming;
using Npgsql;
using Server.Application.ConfigurationApp;
using Server.Application.Interfaces;
using Server.Domain.Messages;
using static FunctionalProgramming.ResultExtensions;

namespace Server.Application.Services
{
	/// <inheritdoc cref="ILoadDataService{TEntity}"/>
	public sealed class LoadDataService<TEntity> : IObserver<AppSetting>, ILoadDataService<TEntity> where TEntity : class
	{
		private readonly ILogService _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly string _connectionString;

		/// <inheritdoc cref="LoadDataService{TEntity}"/>
		/// <param name="logger"></param>
		/// <param name="unitOfWork"></param>
		/// <param name="configService"></param>
		public LoadDataService(ILogService logger, IUnitOfWork unitOfWork, IConfigService configService)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			configService.Subscribe(this);
			_connectionString = configService.GetConfiguration().ConnectionString;
		}
		public IEnumerable<TEntity> GetAllData()
		{
			var repository = _unitOfWork.GetRepository<TEntity>();
			_logger.Write(new LogMessage(""));
			return repository == null ? throw new ArgumentNullException() 
				: repository.GetAll();
		}

		public Task ListenNotification(string channelName, CancellationToken cancellationToken = default) =>
			TryCatchAsync(
					async () =>
					{

						await using var connection = new NpgsqlConnection(_connectionString);
						await connection.OpenAsync(cancellationToken);

						await using (var cmd = new NpgsqlCommand($"LISTEN {channelName}", connection))
							await cmd.ExecuteNonQueryAsync(cancellationToken);

						connection.Notification += (_, args) =>
							_logger.Write(new LogMessage($"{args.Payload} from {args.Channel} channel", nameof(LoadDataService<TEntity>), args.Payload));

						while (!cancellationToken.IsCancellationRequested)
							await connection.WaitAsync(cancellationToken);

					}, cancellationToken)
				.MatchErrorAsync(ex => _logger.Write(new LogError(ex, nameof(LoadDataService<TEntity>), ex.Message)),
					token: cancellationToken);

		#region IObserver
		public void OnCompleted()
		{
			throw new NotImplementedException();
		}

		public void OnError(Exception error)
		{
			throw new NotImplementedException();
		}

		public void OnNext(AppSetting value)
		{
			throw new NotImplementedException();
		}
		#endregion

	}
}
