using StackExchange.Redis;
using StackExchange.Redis.Branch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RealEstate.Data
{
    public class PropertyRepository : RedisRepositoryBase<PropertyEntity>
    {
        public static string BRANCH_ALL = "BRANCH_ALL";
        public static string BRANCH_FAIROFFER = "BRANCH_FAIROFFER";
        public static string BRANCH_LOCATION = "BRANCH_LOCATION";
        public static string BRANCH_LOCATION_ROOMNUMBER = "BRANCH_LOCATION_ROOMNUMBER";
        public static string BRANCH_LOCATION_SORTBY_PRICE = "BRANCH_LOCATION_SORTBY_PRICE";
        public static string BRANCH_LOCATION_SORTBY_LASTUPDATEDATE = "BRANCH_LOCATION_SORTBY_LASTUPDATEDATE";
        public static string BRANCH_LOCATION_ROOMNUMBER_SORTBY_PRICE = "BRANCH_LOCATION_ROOMNUMBER_SORTBY_PRICE";
        public static string BRANCH_LOCATION_ROOMNUMBER_LASTUPDATEDATE = "BRANCH_LOCATION_ROOMNUMBER_LASTUPDATEDATE";


        public PropertyRepository(ConnectionMultiplexer redisConnectionMultiplexer) : base(redisConnectionMultiplexer)
        {
        }

        public override void CreateBranches()
        {
            Expression<Func<PropertyEntity, bool>> validationFilterExpression = i => i.IsActive && i.IsApproved;

            Expression<Func<PropertyEntity, string>> allBranchExpression = i => "All";
            IRedisBranch<PropertyEntity> allBranch = new RedisBranch<PropertyEntity>();
            allBranch.SetBranchId(BRANCH_ALL);
            allBranch.SetEntityType(typeof(PropertyEntity));
            allBranch.FilterBy(validationFilterExpression).GroupBy("All", allBranchExpression);
            AddBranch(allBranch);

            IRedisBranch<PropertyEntity> locationBranch = new RedisBranch<PropertyEntity>();
            locationBranch.SetBranchId(BRANCH_LOCATION);
            locationBranch.SetEntityType(typeof(PropertyEntity));
            locationBranch.FilterBy(validationFilterExpression).GroupBy("Location");
            AddBranch(locationBranch);

            IRedisBranch<PropertyEntity> locationRoomNumberBranch = new RedisBranch<PropertyEntity>();
            locationRoomNumberBranch.SetBranchId(BRANCH_LOCATION_ROOMNUMBER);
            locationRoomNumberBranch.SetEntityType(typeof(PropertyEntity));
            locationRoomNumberBranch.FilterBy(validationFilterExpression).GroupBy("Location").GroupBy("RoomNumber");
            AddBranch(locationRoomNumberBranch);

            IRedisBranch<PropertyEntity> locationSortByPriceBranch = new RedisBranch<PropertyEntity>();
            locationSortByPriceBranch.SetBranchId(BRANCH_LOCATION_SORTBY_PRICE);
            locationSortByPriceBranch.SetEntityType(typeof(PropertyEntity));
            locationSortByPriceBranch.FilterBy(validationFilterExpression).GroupBy("Location").SortBy("Price");
            AddBranch(locationSortByPriceBranch);

            IRedisBranch<PropertyEntity> locationSortByLastUpdateDateBranch = new RedisBranch<PropertyEntity>();
            locationSortByLastUpdateDateBranch.SetBranchId(BRANCH_LOCATION_SORTBY_LASTUPDATEDATE);
            locationSortByLastUpdateDateBranch.SetEntityType(typeof(PropertyEntity));
            locationSortByLastUpdateDateBranch.FilterBy(validationFilterExpression).GroupBy("Location").SortBy("LastUpdateDateTime");
            AddBranch(locationSortByLastUpdateDateBranch);

            IRedisBranch<PropertyEntity> locationRoomNumberSortByPriceBranch = new RedisBranch<PropertyEntity>();
            locationRoomNumberSortByPriceBranch.SetBranchId(BRANCH_LOCATION_ROOMNUMBER_SORTBY_PRICE);
            locationRoomNumberSortByPriceBranch.SetEntityType(typeof(PropertyEntity));
            locationRoomNumberSortByPriceBranch.FilterBy(validationFilterExpression).GroupBy("Location").GroupBy("RoomNumber").SortBy("Price");
            AddBranch(locationRoomNumberSortByPriceBranch);

            IRedisBranch<PropertyEntity> locationRoomNumberSortByLastUpdateDateBranch = new RedisBranch<PropertyEntity>();
            locationRoomNumberSortByLastUpdateDateBranch.SetBranchId(BRANCH_LOCATION_ROOMNUMBER_LASTUPDATEDATE);
            locationRoomNumberSortByLastUpdateDateBranch.SetEntityType(typeof(PropertyEntity));
            locationRoomNumberSortByLastUpdateDateBranch.FilterBy(validationFilterExpression).GroupBy("Location").GroupBy("RoomNumber").SortBy("LastUpdateDateTime");
            AddBranch(locationRoomNumberSortByLastUpdateDateBranch);

            IRedisBranch<PropertyEntity> fairOfferBranch = new RedisBranch<PropertyEntity>();
            fairOfferBranch.SetBranchId(BRANCH_FAIROFFER);
            fairOfferBranch.SetEntityType(typeof(PropertyEntity));
            fairOfferBranch.FilterBy(validationFilterExpression).GroupBy("FairOffer", i => IsOfferFair(i));
            AddBranch(fairOfferBranch);
        }

        public string IsOfferFair(PropertyEntity entity)
        {
            switch (entity.Location)
            {
                case LocationEnum.Istanbul:
                    return entity.Price <= entity.RoomNumber * 500 ? "Fair" : "NotFair";
                case LocationEnum.Berlin:
                    return entity.Price <= entity.RoomNumber * 300 ? "Fair" : "NotFair";
                case LocationEnum.London:
                    return entity.Price <= entity.RoomNumber * 400 ? "Fair" : "NotFair";
                default:
                    return "NotFair";
            }
        }
    }
}
