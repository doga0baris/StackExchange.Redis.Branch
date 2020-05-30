using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    /// <summary>
    /// Redis Sort. It sorts entities by a property. Redis Key is property name. Sort is the last element of the branch and at most one sort can be in a branch.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisSortByProperty<T> : IRedisSort<T> where T : RedisEntity, new()
    {
        private string _propertyName { get; set; }
        private BranchRedisKey _redisKey { get; set; }

        public RedisSortByProperty(string propertyName)
        {
            bool propertyFound = false;
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.Name == propertyName)
                {
                    propertyFound = true;
                    break;
                }
            }

            if (!propertyFound)
            {
                throw new ArgumentException($"{propertyName} is not memeber of {typeof(T).Name}.");
            }

            _propertyName = propertyName;
        }

        public BranchRedisKey GetKey()
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Sort, _propertyName);
            }
            return _redisKey;
        }

        public double GetScore(T entity)
        {
            object propertyValue = entity.GetType().GetProperty(_propertyName).GetValue(entity);
            double score;
            if (double.TryParse(propertyValue.ToString(), out score))
            {
                return score;
            }
            throw new ArgumentException($"{_propertyName} can not parse to double.", _propertyName);
        }
    }
}
