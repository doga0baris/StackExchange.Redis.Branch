using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Sort. It sorts entities by a function. Redis Key is function name. Sort is the last element of the branch and at most one sort can be in a branch.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisSortByFunction<T> : IRedisSort<T> where T : RedisEntity, new()
    {
        private string _functionName { get; set; }
        private Expression<Func<T, double>> _sortFunction { get; set; }

        private BranchRedisKey _redisKey { get; set; }

        public RedisSortByFunction(string functionName, Expression<Func<T, double>> sortFunction)
        {
            _functionName = functionName;
            _sortFunction = sortFunction;
        }

        public BranchRedisKey GetKey()
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Sort, _functionName);
            }
            return _redisKey;
        }

        public double GetScore(T entity)
        {
            var f = _sortFunction.Compile();
            return f.Invoke(entity);
        }
    }
}
