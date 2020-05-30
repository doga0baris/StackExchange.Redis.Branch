using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    /// <summary>
    /// Redis Filter. It filters entity before any group or sort operation. A branch can contains multiple filters. Filter doesn't effect redis key like groups or sort.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisFilter<T> : IRedisFilter<T> where T : RedisEntity, new()
    {
        private Expression<Func<T, bool>> _filterExpression { get; set; }


        public RedisFilter(Expression<Func<T, bool>> filterExpression)
        {
            _filterExpression = filterExpression;
        }

        public bool Invoke(T entity)
        {
            var f = _filterExpression.Compile();
            return f.Invoke(entity);
        }
    }
}
