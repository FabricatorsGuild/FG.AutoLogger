using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Utils
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> TypeToFriendlyName = new Dictionary<Type, string>
        {
            {typeof(string), "string"},
            {typeof(object), "object"},
            {typeof(bool), "bool"},
            {typeof(byte), "byte"},
            {typeof(char), "char"},
            {typeof(decimal), "decimal"},
            {typeof(double), "double"},
            {typeof(short), "short"},
            {typeof(int), "int"},
            {typeof(long), "long"},
            {typeof(sbyte), "sbyte"},
            {typeof(float), "float"},
            {typeof(ushort), "ushort"},
            {typeof(uint), "uint"},
            {typeof(ulong), "ulong"},
            {typeof(void), "void"}
        };


        public static Type ParseType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case ("string"):
                case ("system.string"):
                    return typeof(string);
                case ("int"):
                case ("system.int32"):
                    return typeof(int);
                case ("long"):
                case ("system.int64"):
                    return typeof(long);
                case ("bool"):
                case ("system.boolean"):
                    return typeof(bool);
                case ("datetime"):
                case ("system.dateTime"):
                    return typeof(System.DateTime);
                case ("guid"):
                case ("system.guid"):
                    return typeof(Guid);
                default:
                    return typeof(object);
            }
        }

        public static string RenderCLRType(Type type)
        {
            if (type == typeof(string))
                return @"string";
            if (type == typeof(int))
                return @"int";
            if (type == typeof(long))
                return @"long";
            if (type == typeof(bool))
                return @"bool";
            if (type == typeof(DateTime))
                return @"DateTime";
            if (type == typeof(Guid))
                return @"Guid";

            return @"string";
        }

        public static bool IsComplexType(string type)
        {
            var parsedType = ParseType(type);
            return parsedType == typeof(object);
        }

        public static string GetFriendlyName(this Type type)
        {
            string friendlyName;
            if (TypeToFriendlyName.TryGetValue(type, out friendlyName))
            {
                return friendlyName;
            }

            friendlyName = type.FullName;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; i++)
                {
                    string typeParamName = typeParameters[i].GetFriendlyName();
                    friendlyName += (i == 0 ? typeParamName : ", " + typeParamName);
                }
                friendlyName += ">";
            }

            if (type.IsArray)
            {
                return type.GetElementType().GetFriendlyName() + "[]";
            }

            return friendlyName;
        }


        public static IEnumerable<MethodInfo> GetAllInterfaceMethods(this Type type)
        {
            var methodInfos = new List<MethodInfo>();

            GetAllInterfaceMethodsInternal(type, methodInfos);

            return methodInfos;
        }

        private static void GetAllInterfaceMethodsInternal(Type type, List<MethodInfo> accumulator)
        {
            foreach (var methodInfo in type.GetMethods())
            {
                if (!accumulator.Contains(methodInfo))
                {
                    accumulator.Add(methodInfo);
                }
            }

            foreach (var typeInterface in type.GetInterfaces())
            {
                GetAllInterfaceMethodsInternal(typeInterface, accumulator);
            }           
        }
    }
}