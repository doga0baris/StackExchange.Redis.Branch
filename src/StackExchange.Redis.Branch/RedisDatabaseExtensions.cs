using Newtonsoft.Json;
using StackExchange.Redis.Branch.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StackExchange.Redis.Branch
{
    public static class RedisDatabaseExtensions
    {
        /// <summary>
        /// Adds instance of an object to redis exchange as hash set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisDatabase"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task HashSetAsync<T>(this IDatabase redisDatabase, T entity) where T : RedisEntity
        {
            await redisDatabase.HashSetAsync(entity.GetRedisKey(), entity.ConvertToHashEntryList().ToArray());
        }

        /// <summary>
        /// Converts instance of an object to redis exchange hash entry list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static IEnumerable<HashEntry> ConvertToHashEntryList<T>(this T instance) where T : RedisEntity
        {
            var members = typeof(T).GetMembers();

            for (var index = 0; index < members.Count(); index++)
            {
                var member = members[index];
                if (member.GetCustomAttribute(typeof(IgnoreDataMemberAttribute), true) != default)
                {
                    continue;
                }

                var type = member.MemberType.GetType();
                var underlyingType = Nullable.GetUnderlyingType(type);
                var effectiveType = underlyingType ?? type;

                var val = instance.GetType().GetProperty(member.Name).GetValue(instance, null);

                if (!type.IsValueType)//ATM supports only value types
                {
                    if (type != typeof(string))
                    {
                        yield return new HashEntry(member.Name, JsonConvert.SerializeObject(val, Formatting.None));
                        continue;
                    }
                }

                if (val != null)
                {
                    if (effectiveType == typeof(DateTime))
                    {
                        var date = (DateTime)val;
                        if (date.Kind == DateTimeKind.Utc)
                        {

                            yield return new HashEntry(member.Name, $"{date.Ticks}|UTC");
                        }
                        else
                        {
                            yield return new HashEntry(member.Name, $"{date.Ticks}|LOC");
                        }

                    }
                    else if (effectiveType.IsEnum)
                    {
                        yield return new HashEntry(member.Name, (int)val);
                    }
                    else
                    {
                        yield return new HashEntry(member.Name, val.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Converts from hash entry list and create instance of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entries">The entries returned from StackExchange.redis.</param>
        /// <returns>Instance of Type T </returns>
        public static T ConvertFromHashEntryList<T>(this IEnumerable<HashEntry> entries) where T : RedisEntity, new()
        {
            var instance = new T();
            var hashEntries = entries as HashEntry[] ?? entries.ToArray();
            var members = typeof(T).GetMembers();

            for (var index = 0; index < members.Count(); index++)
            {
                try
                {
                    var member = members[index];

                    if (member.IsDefined(typeof(IgnoreDataMemberAttribute)))
                    {
                        continue;
                    }

                    var type = member.MemberType.GetType();

                    var entry = hashEntries.FirstOrDefault(e => e.Name.ToString().Equals(member.Name));

                    if (entry.Equals(new HashEntry()))
                    {
                        continue;
                    }

                    var value = entry.Value.ToString();

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    if (!type.IsValueType) //ATM supports only value types
                    {
                        if (type != typeof(string))
                        {
                            var propertyInfo = instance.GetType().GetProperty(member.Name);
                            propertyInfo.SetValue(instance, Convert.ChangeType(JsonConvert.DeserializeObject(value, type), propertyInfo.PropertyType), null);
                            continue;
                        }
                    }

                    var underlyingType = Nullable.GetUnderlyingType(type);
                    var effectiveType = underlyingType ?? type;

                    if (effectiveType == typeof(DateTime))
                    {
                        if (value.EndsWith("|UTC"))
                        {
                            value = value.TrimEnd("|UTC".ToCharArray());
                            DateTime date;

                            long ticks;
                            if (long.TryParse(value, out ticks))
                            {
                                date = new DateTime(ticks);
                                var propertyInfo = instance.GetType().GetProperty(member.Name);
                                propertyInfo.SetValue(instance, Convert.ChangeType(DateTime.SpecifyKind(date, DateTimeKind.Utc), propertyInfo.PropertyType), null);
                            }
                        }
                        else
                        {
                            value = value.TrimEnd("|LOC".ToCharArray());
                            DateTime date;
                            long ticks;
                            if (long.TryParse(value, out ticks))
                            {
                                date = new DateTime(ticks);
                                var propertyInfo = instance.GetType().GetProperty(member.Name);
                                propertyInfo.SetValue(instance, Convert.ChangeType(DateTime.SpecifyKind(date, DateTimeKind.Local), propertyInfo.PropertyType), null);
                            }
                        }
                    }
                    else if (member.GetType().IsEnum)
                    {
                        var propertyInfo = instance.GetType().GetProperty(member.Name);
                        propertyInfo.SetValue(instance, Convert.ChangeType(Enum.ToObject(member.GetType(), Convert.ToInt32(entry.Value.ToString())), propertyInfo.PropertyType), null);
                    }
                    else
                    {
                        var propertyInfo = instance.GetType().GetProperty(member.Name);
                        propertyInfo.SetValue(instance, Convert.ChangeType(entry.Value.ToString(), propertyInfo.PropertyType), null);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return instance;
        }
    }
}
