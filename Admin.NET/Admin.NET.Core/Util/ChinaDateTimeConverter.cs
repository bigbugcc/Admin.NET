using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace Admin.NET.Core
{
    /// <summary>
    /// JSON时间序列化yyyy-MM-dd HH:mm:ss
    /// </summary>
    public class ChinaDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }
        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }

    /// <summary>
    /// JSON时间序列化yyyy-MM-dd HH:mm
    /// </summary>
    public class ChinaDateTimeConverterHH : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }
        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }

    /// <summary>
    /// JSON时间序列化yyyy-MM-dd
    /// </summary>
    public class ChinaDateTimeConverterDate : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };
        /// <summary>
        /// ReadJson
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }
        /// <summary>
        /// WriteJson
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dtConverter.WriteJson(writer, value, serializer);
        }
    }
}
