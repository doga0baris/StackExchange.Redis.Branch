using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    public interface IRedisEntity
    {
        /// <summary>
        /// Attach a redis entity event handler to the redis entity.
        /// </summary>
        /// <param name="observer"></param>
        void Attach(IRedisEntityEventHandler<IRedisEntityEvent> handler);

        /// <summary>
        /// Detach a redis entity event handler from the redis entity.
        /// </summary>
        /// <param name="observer"></param>
        void Detach(IRedisEntityEventHandler<IRedisEntityEvent> handler);

        /// <summary>
        /// Notify all redis entity event handlers about an event.
        /// </summary>
        void Notify();
    }
}
