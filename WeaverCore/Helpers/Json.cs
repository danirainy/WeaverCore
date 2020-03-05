﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WeaverCore.Helpers
{
    public static class Json
    {
        static Assembly NewtonsoftJson;
        static Type JsonConvert;
        static Type FormattingT;
        static Type JsonSerializerSettingsT;
        static Type ReferenceLoopHandlingT;

        static PropertyInfo ReferenceLoopHandlingProp;

        static object Indented;
        static object IgnoreLoopHandling;
        static object DefaultSettings;
        //static Func<object, string> SerializeMethod;
        static Func<string, Type, object> DeserializeMethod;

        static MethodInfo SerializeMethod;

        static Json()
        {
            NewtonsoftJson = Assembly.Load("Newtonsoft.Json");
            JsonConvert = NewtonsoftJson.GetType("Newtonsoft.Json.JsonConvert");
            FormattingT = NewtonsoftJson.GetType("Newtonsoft.Json.Formatting");
            JsonSerializerSettingsT = NewtonsoftJson.GetType("Newtonsoft.Json.JsonSerializerSettings");
            ReferenceLoopHandlingT = NewtonsoftJson.GetType("Newtonsoft.Json.ReferenceLoopHandling");

            ReferenceLoopHandlingProp = JsonSerializerSettingsT.GetProperty("ReferenceLoopHandling");

            Indented = Enum.Parse(FormattingT, "Indented");
            DefaultSettings = Activator.CreateInstance(JsonSerializerSettingsT);
            IgnoreLoopHandling = Enum.Parse(ReferenceLoopHandlingT,"Ignore");

            ReferenceLoopHandlingProp.SetValue(DefaultSettings, IgnoreLoopHandling, null);

            SerializeMethod = JsonConvert.GetMethod("SerializeObject", new Type[] { typeof(object),FormattingT,JsonSerializerSettingsT });
            //SerializeMethod = Methods.GetFunction<Func<object, string>>(JsonConvert.GetMethod("SerializeObject", new Type[] { typeof(object) }));
            DeserializeMethod = Methods.GetFunction<Func<string, Type, object>>(JsonConvert.GetMethod("DeserializeObject", new Type[] { typeof(string), typeof(Type) }));
        }

        public static string Serialize(object obj)
        {
            return (string)SerializeMethod.Invoke(null, new object[] { obj,Indented,DefaultSettings });
            //return SerializeMethod(obj);
        }

        public static T Deserialize<T>(string source)
        {
            return (T)DeserializeMethod(source, typeof(T));
        }
    }
}
