using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport;
using CodeEffect.ServiceFabric.Services.Remoting.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace CodeEffect.ServiceFabric.Services.Remoting.Runtime
{
    public static class ServiceRemotingDispatcherExtensions
    {
        public static string GetMethodDispatcherMapName(this Microsoft.ServiceFabric.Services.Remoting.Runtime.ServiceRemotingDispatcher that, int interfaceId, int methodId)
        {
            try
            {
                var methodDispatcherMapFieldInfo = typeof(Microsoft.ServiceFabric.Services.Remoting.Runtime.ServiceRemotingDispatcher).GetField("methodDispatcherMap",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
                var methodDispatcherMap = methodDispatcherMapFieldInfo?.GetValue(that);
                var methodDispatcher = methodDispatcherMap?.GetType()
                    .InvokeMember("Item", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, methodDispatcherMap,
                        new object[] {interfaceId});
                var getMethodNameMethodInfo =
                    methodDispatcher?.GetType().GetInterface("Microsoft.ServiceFabric.Services.Remoting.IMethodDispatcher").GetMethod("GetMethodName");
                //var getMethodNameMethodInfo = methodDispatcher?.GetType()
                //    .GetMethod("Microsoft.ServiceFabric.Services.Remoting.IMethodDispatcher.GetMethodName", BindingFlags.NonPublic | BindingFlags.Instance);
                var methodName = getMethodNameMethodInfo?.Invoke(methodDispatcher, new object[] {methodId}) as string;
                return methodName;
            }
            catch (Exception)
            {
                // Ignore
                return null;
            }
        }
    }
}