using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    public interface IRedisSort<T> where T : RedisEntity, new()
    {
        public double GetScore(T entity);
        public BranchRedisKey GetKey();
    }
}
