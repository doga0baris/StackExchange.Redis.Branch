using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    /// <summary>
    /// Base class for redis entity.
    /// </summary>
    public abstract class RedisEntity : IRedisEntity
    {
        public string Id { get; set; }

        private IBranchRedisKey _redisKey { get; set; }

        [IgnoreDataMember]
        private List<IRedisEntityEventHandler<IRedisEntityEvent>> _handlers = new List<IRedisEntityEventHandler<IRedisEntityEvent>>();

        [IgnoreDataMember]
        protected RedisEntityStateEnum _currentState;


        public RedisEntity()
        {
            _currentState = RedisEntityStateEnum.NotSet;
        }

        public virtual string GetRedisKey()
        {
            if (_redisKey == default)
            {
                _redisKey = new BranchRedisKey(BranchRedisKeyEnum.Data, Id);
            }

            return $"{this.GetType().Name}:{_redisKey}";
        }

        public void Attach(IRedisEntityEventHandler<IRedisEntityEvent> handler)
        {
            this._handlers.Add(handler);
        }

        public void Detach(IRedisEntityEventHandler<IRedisEntityEvent> handler)
        {
            this._handlers.Remove(handler);
        }

        public void Notify()
        {
            foreach (var handler in _handlers)
            {
                handler.Update(new RedisEntityEvent(this, _currentState));
            }
        }
    }
}
