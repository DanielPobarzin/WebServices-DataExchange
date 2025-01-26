using Client.Application.Interfaces;
using Client.DataApplicationLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Client.DataApplicationLayer.Extensions;

/// <summary>
/// Методы расширения для настройки служб, связанных с единицей работы, в <see cref="IServiceCollection"/>.
/// </summary>
public static class UnitOfWorkCollectionExtensions
{
	/// <summary>
	/// Регистрирует единицу работы с заданным контекстом как службу в <see cref="IServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext">Тип контекста базы данных.</typeparam>
	/// <param name="services">Коллекция сервисов <see cref="IServiceCollection"/>, в которую нужно добавить службы.</param>
	/// <returns>Одна и та же коллекция сервисов, позволяющая объединить несколько вызовов.</returns>
	public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
		where TContext : DbContext
	{
		services.AddScoped<IRepositoryFactory, UnitOfWork<TContext>>();
		services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
		services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();

		return services;
	}

	/// <summary>
	/// Регистрирует единицу работы с заданным контекстом как службу в <see cref="IServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext1">Тип контекста базы данных.</typeparam>
	/// <typeparam name="TContext2">Тип контекста базы данных.</typeparam>
	/// <param name="services">Коллекция сервисов <see cref="IServiceCollection"/>, в которую нужно добавить службы.</param>
	/// <returns>Одна и та же коллекция сервисов, позволяющая объединить несколько вызовов.</returns>
	public static IServiceCollection AddUnitOfWork<TContext1, TContext2>(this IServiceCollection services)
		where TContext1 : DbContext
		where TContext2 : DbContext
	{
		services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
		services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();

		return services;
	}

	/// <summary>
	/// Регистрирует единицу работы с заданным контекстом как службу в <see cref="IServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TContext1">Тип контекста базы данных.</typeparam>
	/// <typeparam name="TContext2">Тип контекста базы данных.</typeparam>
	/// <typeparam name="TContext3">Тип контекста базы данных.</typeparam>
	/// <param name="services">Коллекция сервисов <see cref="IServiceCollection"/>, в которую нужно добавить службы.</param>
	/// <returns>Одна и та же коллекция сервисов, позволяющая объединить несколько вызовов.</returns>
	public static IServiceCollection AddUnitOfWork<TContext1, TContext2, TContext3>(
		this IServiceCollection services)
		where TContext1 : DbContext
		where TContext2 : DbContext
		where TContext3 : DbContext
	{
		services.AddScoped<IUnitOfWork<TContext1>, UnitOfWork<TContext1>>();
		services.AddScoped<IUnitOfWork<TContext2>, UnitOfWork<TContext2>>();
		services.AddScoped<IUnitOfWork<TContext3>, UnitOfWork<TContext3>>();

		return services;
	}

	/// <summary>
	/// Регистрация пользовательского репозитория как сервиса в <see cref="IServiceCollection"/>.
	/// </summary>
	/// <typeparam name="TEntity">Тип сущности.</typeparam>
	/// <typeparam name="TRepository">Тип пользовательского репозитория.</typeparam>
	/// <param name="services">Коллекция сервисов <see cref="IServiceCollection"/>, в которую нужно добавить службы.</param>
	/// <returns>Одна и та же коллекция сервисов, позволяющая объединить несколько вызовов.</returns>
	public static IServiceCollection AddCustomRepository<TEntity, TRepository>(this IServiceCollection services)
		where TEntity : class
		where TRepository : class, IRepository<TEntity>
	{
		services.AddScoped<IRepository<TEntity>, TRepository>();

		return services;
	}
}