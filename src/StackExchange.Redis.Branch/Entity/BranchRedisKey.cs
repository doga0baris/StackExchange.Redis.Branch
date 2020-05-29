using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    /// <summary>
    /// Redis Key.
    /// </summary>
    public class BranchRedisKey : IBranchRedisKey
    {
        public BranchRedisKeyEnum Type { get; private set; }
        public string By { get; private set; }
        public string Value { get; private set; }

        public BranchRedisKey() { }

        public BranchRedisKey(BranchRedisKeyEnum type, string byOrValue)
        {
            Type = type;
            switch (Type)
            {
                case BranchRedisKeyEnum.Data:
                    Value = byOrValue;
                    break;
                case BranchRedisKeyEnum.Sort:
                    By = byOrValue;
                    break;
                case BranchRedisKeyEnum.Group:
                    By = byOrValue;
                    break;
                default:
                    break;
            }
        }

        public BranchRedisKey(BranchRedisKeyEnum type, string by, string value)
        {
            Type = type;
            By = by;
            Value = value;
        }

        public BranchRedisKeyEnum GetRedisKeyType()
        {
            return Type;
        }

        public void SetRedisKeyType(BranchRedisKeyEnum type)
        {
            Type = type;
        }

        public void SetValue(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case BranchRedisKeyEnum.Data:
                    return $"data:{Value}";
                case BranchRedisKeyEnum.Sort:
                    return $"sorted:{By}";
                case BranchRedisKeyEnum.Group:
                    if (!string.IsNullOrEmpty(Value))
                    {
                        return $"grouped:{By}:{Value}";
                    }
                    else
                    {
                        return $"grouped:{By}";
                    }
                default:
                    return "";
            }
        }
    }
}
