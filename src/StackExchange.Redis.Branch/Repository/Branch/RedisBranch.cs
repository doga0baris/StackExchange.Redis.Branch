using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Repository
{
    /// <summary>
    /// Redis Branch. Branch contains filters, groups and a sort.
    /// </summary>
    /// <typeparam name="T">Redis Entity</typeparam>
    public class RedisBranch<T> : IRedisBranch<T> where T : RedisEntity, new()
    {
        private List<IRedisFilter<T>> _filters { get; set; }
        private List<IRedisGroup<T>> _groups { get; set; }
        private IRedisSort<T> _sort { get; set; }
        private string _branchId { get; set; }
        private Type _entityType { get; set; }

        public RedisBranch()
        {
            _groups = new List<IRedisGroup<T>>();
            _filters = new List<IRedisFilter<T>>();
        }

        public string GetBranchId()
        {
            return _branchId;
        }

        public void SetBranchId(string branchId)
        {
            _branchId = branchId;
        }

        public IRedisBranch<T> GroupBy()
        {
            _groups.Add(new RedisGroupByProperty<T>("Id"));
            return this;
        }

        public IRedisBranch<T> GroupBy(string propName)
        {
            _groups.Add(new RedisGroupByProperty<T>(propName));
            return this;
        }

        public IRedisBranch<T> GroupBy(string functionName, Expression<Func<T, string>> groupFunction)
        {
            _groups.Add(new RedisGroupByFunction<T>(functionName, groupFunction));
            return this;
        }

        public void SortBy()
        {
            _sort = new RedisSortByProperty<T>("Id");
        }

        public void SortBy(string propName)
        {
            _sort = new RedisSortByProperty<T>(propName);
        }

        public void SortBy(string functionName, Expression<Func<T, double>> sortFunction)
        {
            _sort = new RedisSortByFunction<T>(functionName, sortFunction);
        }

        public string GetBranchKey(T entity)
        {
            string branchKey = entity.GetType().Name;
            foreach (IRedisGroup<T> group in _groups)
            {
                branchKey = $"{branchKey}:{group.GetKey(entity)}";
            }

            if (_sort != default)
            {
                branchKey = $"{branchKey}:{_sort.GetKey()}";
            }

            return branchKey;
        }

        public string GetBranchKey(string[] values)
        {
            string branchKey = _entityType.Name;

            if (values.Length != GetGroups().Count(i => i.GetType() == typeof(RedisGroupByProperty<T>)))
            {
                throw new IndexOutOfRangeException($"Branch group parameters more than group parameters. Branch group parameters: {GetGroups().Count(i => i.GetType() == typeof(RedisGroupByProperty<T>))}, Group parameters: {values.Length}");
            }

            var valueIndex = 0;
            foreach (var group in GetGroups())
            {
                if (group is RedisGroupByFunction<T>)
                {
                    branchKey = $"{branchKey}:{group.GetKey()}";
                }
                else if (group is RedisGroupByProperty<T>)
                {
                    branchKey = $"{branchKey}:{group.GetKey(values[valueIndex])}";
                    valueIndex++;
                }
            }

            var sort = GetSort();
            if (sort != default)
            {
                branchKey += $":{sort.GetKey()}";
            }

            return branchKey;
        }

        public string GetBranchKey()
        {
            string branchKey = _entityType.Name;
            foreach (IRedisGroup<T> group in _groups)
            {
                branchKey = $"{branchKey}:{group.GetKey()}";
            }

            if (_sort != default)
            {
                branchKey = $"{branchKey}:{_sort.GetKey()}";
            }

            return branchKey;
        }

        public void SetEntityType(Type entityType)
        {
            _entityType = entityType;
        }

        public Type GetEntityType()
        {
            return _entityType;
        }

        public List<IRedisGroup<T>> GetGroups()
        {
            return _groups;
        }

        public IRedisSort<T> GetSort()
        {
            return _sort;
        }

        public List<IRedisFilter<T>> GetFilters()
        {
            return _filters;
        }

        public IRedisBranch<T> FilterBy(Expression<Func<T, bool>> filterFunction)
        {
            _filters.Add(new RedisFilter<T>(filterFunction));
            return this;
        }

        public bool ApplyFilters(T entity)
        {
            bool isFilterPass = true;
            foreach (IRedisFilter<T> filter in _filters)
            {
                isFilterPass = filter.Invoke(entity);
                if (!isFilterPass)
                {
                    break;
                }
            }
            return isFilterPass;
        }
    }
}
