using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    public enum RedisEntityStateEnum
    {
        NotSet = 0,
        Added = 1,
        Updated = 2,
        Deleted = 3
    }
}
