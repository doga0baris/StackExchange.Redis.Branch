using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Branch.Entity;
using StackExchange.Redis.Branch.Repository;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StackExchange.Redis.Branch
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds redis exchange connection multiplexer to DI as Singleton. Adds redis repositories to DI as Singleton.
        /// </summary>
        /// <param name="services">IServiceCollection instance.</param>
        /// <param name="redisConnectionString">Redis connection string.</param>
        /// <param name="assembly">Assembly which contains redis repositories.</param>
        /// <returns></returns>
        public static IServiceCollection AddRedisBranch(this IServiceCollection services, string redisConnctionConfiguration, params Assembly[] assemblies)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (redisConnctionConfiguration == null) throw new ArgumentNullException(nameof(redisConnctionConfiguration));
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));

            services.AddSingleton(sp =>
            {
                return ConnectionMultiplexer.Connect(redisConnctionConfiguration);
            });

            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract &&
                        (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(RedisRepositoryBase<>)
                        || type.BaseType.BaseType != default && type.BaseType.BaseType.IsGenericType && type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(RedisRepositoryBase<>)
                        ))
                    {
                        Type entityType = type.BaseType.GetGenericArguments()[0];

                        var iRepositoryType = typeof(IRedisRepository<>);
                        var iRepository = iRepositoryType.MakeGenericType(entityType);

                        var serviceDescriptor = new ServiceDescriptor(iRepository, type, ServiceLifetime.Singleton);
                        services.Add(serviceDescriptor);
                    }
                    else if (type.BaseType == typeof(RedisEntity))
                    {
                        var serviceDescriptor = new ServiceDescriptor(typeof(IRedisEntity), type, ServiceLifetime.Transient);
                        services.Add(serviceDescriptor);
                    }
                }
            }

            return services;
        }
    }

}
