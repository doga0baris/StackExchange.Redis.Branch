using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    public interface IRedisFilter<T> where T : RedisEntity, new()
    {
        bool Invoke(T entity);
    }
}
