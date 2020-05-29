using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    public class RedisEntityEvent : IRedisEntityEvent
    {
        public RedisEntity Entity { get; set; }
        //public Type RepoType { get; set; }
        public RedisEntityStateEnum RedisEntityStateEnum { get; set; }

        public RedisEntityEvent(RedisEntity entity, RedisEntityStateEnum redisEntityStateEnum)
        {
            Entity = entity;
            //RepoType = repoType;
            RedisEntityStateEnum = redisEntityStateEnum;
        }
    }
}
