using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ArangoDbPoc.Infra.Repository
{
    public static class Serialization
    {
        public static string Serialize(object instance)
        {
            var serialized = JsonConvert.SerializeObject(instance, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return serialized;
        }

        public static object Deserialize(string serialized, Type type)
        {
            var deserialized = JsonConvert.DeserializeObject(serialized, type, new JsonSerializerSettings
            {
                ContractResolver = new PrivateSetterCamelCasePropertyNamesContractResolver()
            });
            return deserialized;
        }
    }

    public class PrivateSetterCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
            {
                return jProperty;
            }

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }

    internal static class MemberInfoExtensions
    {
        internal static bool IsPropertyWithSetter(this MemberInfo member)
        {
            var property = member as PropertyInfo;

            return property?.GetSetMethod(true) != null;
        }
    }
}
