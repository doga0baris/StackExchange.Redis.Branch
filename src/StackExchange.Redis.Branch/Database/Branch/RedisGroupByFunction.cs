using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    /// <summary>
    /// Redis Group. It groups entities by a function. Redis Key is function name.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisGroupByFunction<T> : IRedisGroup<T> where T : RedisEntity, new()
    {
        private Expression<Func<T, string>> _groupFunction { get; set; }

        private BranchRedisKey _redisKey { get; set; }

        private string _functionName { get; set; }

        public RedisGroupByFunction(string functionName, Expression<Func<T, string>> groupFunction)
        {
            _groupFunction = groupFunction;
            _functionName = functionName;
        }

        public RedisGroupByFunction(Expression<Func<T, string>> groupFunction)
        {
            _groupFunction = groupFunction;
        }

        private string GetFunctionGroupKey(T entity)
        {
            var f = _groupFunction.Compile();
            return f.Invoke(entity);
        }

        public BranchRedisKey GetKey(T entity)
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, GetFunctionGroupKey(entity));
            }
            return _redisKey;
        }

        public BranchRedisKey GetKey()
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _functionName);
            }
            return _redisKey;
        }

        public BranchRedisKey GetKey(string funtionReturnValue)
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, funtionReturnValue);
            }
            return _redisKey;
        }
    }
}
