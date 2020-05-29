using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    public class RedisEntityEventHandler : IRedisEntityEventHandler<RedisEntityEvent>
    {
        public void Update(RedisEntityEvent subject)
        {
            throw new NotImplementedException();
        }
    }
}
