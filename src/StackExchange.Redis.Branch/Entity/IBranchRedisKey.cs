using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    internal interface IBranchRedisKey
    {
        public void SetValue(string value);
        public BranchRedisKeyEnum GetRedisKeyType();
        public void SetRedisKeyType(BranchRedisKeyEnum type);
    }
}
