using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Repository
{
    public interface IRedisGroup<T> where T : RedisEntity, new()
    {
        public BranchRedisKey GetKey();
        public BranchRedisKey GetKey(T entity);
        public BranchRedisKey GetKey(string propertyValue);
    }
}
