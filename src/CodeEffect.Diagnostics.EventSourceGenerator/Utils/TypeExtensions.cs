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