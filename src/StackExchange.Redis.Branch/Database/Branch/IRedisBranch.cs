using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace StackExchange.Redis.Branch.Database
{
    public interface IRedisBranch<T> where T : RedisEntity, new()
    {
        public string GetBranchId();
        public void SetBranchId(string branchId);
        public void SetEntityType(Type entityType);
        public Type GetEntityType();
        public List<IRedisGroup<T>> GetGroups();
        public List<IRedisFilter<T>> GetFilters();
        public IRedisSort<T> GetSort();
        public IRedisBranch<T> FilterBy(Expression<Func<T, bool>> filterFunction);
        public IRedisBranch<T> GroupBy();
        public IRedisBranch<T> GroupBy(string propName);
        public IRedisBranch<T> GroupBy(string functionName, Expression<Func<T, string>> groupFunction);
        public void SortBy();
        public void SortBy(string propName);
        public void SortBy(string functionName, Expression<Func<T, double>> sortFunction);
        public string GetBranchKey();
        public string GetBranchKey(T entity);
        public string GetBranchKey(params string[] values);
        public bool ApplyFilters(T entity);
    }
}
