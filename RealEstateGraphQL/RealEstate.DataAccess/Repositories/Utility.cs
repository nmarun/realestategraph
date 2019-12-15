using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;

namespace RealEstate.DataAccess.Repositories
{
    public static class Utility
    {
        public static string ToXml<T>(this T objectToSerialize)
        {
            var serializedXml = string.Empty;
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, objectToSerialize);
                serializedXml = stringwriter.ToString();
            }

            return serializedXml;
        }

        public static T FromXml<T>(this string objectToDeserialize)
        {
            var serializer = new XmlSerializer(typeof(T));
            T result;

            using (TextReader reader = new StringReader(objectToDeserialize))
            {
                result = (T)serializer.Deserialize(reader);
            }

            return result;
        }

        public static string ToJson<T>(this T objectToSerialize)
        {
            var json = string.Empty;
            using (var stringwriter = new StringWriter())
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(stringwriter, objectToSerialize);
                json = stringwriter.ToString();
            }

            return json;
        }

        public static T FromJson<T>(this string objectToDeserialize)
        {
            T output = JsonConvert.DeserializeObject<T>(objectToDeserialize);

            return output;
        }
    }
}
