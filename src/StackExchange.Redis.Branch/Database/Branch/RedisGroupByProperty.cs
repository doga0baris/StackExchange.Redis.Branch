using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    /// <summary>
    /// Redis Group. It groups entities by a property. Redis Key is property name.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisGroupByProperty<T> : IRedisGroup<T> where T : RedisEntity, new()
    {
        private string _propertyName { get; set; }
        private BranchRedisKey _redisKey { get; set; }

        public RedisGroupByProperty(string propertyName)
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
                throw new ArgumentException($"{propertyName} is not member of {typeof(T).Name}.");
            }

            _propertyName = propertyName;
        }

        public BranchRedisKey GetKey(T entity)
        {
            object propertyValue = entity.GetType().GetProperty(_propertyName).GetValue(entity);

            if (_redisKey == default)
            {
                if (_propertyName == "Id")
                {
                    _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, propertyValue.ToString());
                }
                else
                {
                    _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _propertyName, propertyValue.ToString());
                }
            }
            _redisKey.SetValue(propertyValue.ToString());
            return _redisKey;
        }

        public BranchRedisKey GetKey()
        {
            if (_redisKey == default)
            {
                if (_propertyName == "Id")
                {
                    _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, "{propertyValue}");
                }
                else
                {
                    _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _propertyName, "{propertyValue}");
                }
            }
            return _redisKey;
        }

        public BranchRedisKey GetKey(string propertyValue)
        {
            if (_redisKey == default)
            {
                if (_propertyName == "Id")
                {
                    _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, propertyValue);
                }
                else
                {
                    _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Group, _propertyName, propertyValue);
                }
            }
            _redisKey.SetValue(propertyValue);
            return _redisKey;
        }
    }
}
