using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    public interface IRedisEntityEventHandler<in TRedisEntityEvent> where TRedisEntityEvent : IRedisEntityEvent
    {
        /// <summary>
        /// Receive update from redis entity.
        /// </summary>
        /// <param name="subject"></param>
        void Update(TRedisEntityEvent subject);
    }
}
