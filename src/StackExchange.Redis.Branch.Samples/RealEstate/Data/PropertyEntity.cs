using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Data
{
    public class PropertyEntity : RedisEntity
    {
        public string Title { get; set; }
        public LocationEnum Location { get; set; }
        public long Price { get; set; }
        public int RoomNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTimeOffset LastUpdateDateTime { get; set; }
    }

    public enum LocationEnum
    {
        Istanbul = 0,
        Berlin = 1,
        London = 2
    }
}
