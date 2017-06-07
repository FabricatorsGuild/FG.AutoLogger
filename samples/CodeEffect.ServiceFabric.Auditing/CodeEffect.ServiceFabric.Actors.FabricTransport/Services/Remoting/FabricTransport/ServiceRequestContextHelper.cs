using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CodeEffect.ServiceFabric.Actors.FabricTransport.Diagnostics;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CodeEffect.ServiceFabric.Services.Remoting.FabricTransport
{
    public static class ServiceRequestContextHelper
    {
        public static Task RunInContext(this IActorProxyFactory actorProxyFactory, Action<IActorProxyFactory> action, IServiceCommunicationLogger logger, IEnumerable<ServiceRequestHeader> headers)
        {
            Task task = null;
            var headersArray = headers.ToArray();

            task = new Task(() =>
            {
                logger?.StartRequestContext(headersArray);
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headersArray);
                ServiceRequestContext.Current.Logger = logger;
                try
                {
                    action(actorProxyFactory);
                }
                catch (Exception ex)
                {
                    logger?.FailedRequestContext(headers, ex);
                    throw ex;
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                    logger?.StopRequestContext(headersArray);
                }
            });

            task.Start();

            return task;
        }

        public static Task RunInContext(this IActorProxyFactory actorProxyFactory, Action<IActorProxyFactory> action, IServiceCommunicationLogger logger, params ServiceRequestHeader[] headers)
        {
            Task task = null;
            var headersArray = headers.ToArray();

            task = new Task(() =>
            {
                logger?.StartRequestContext(headersArray);
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headers);
                ServiceRequestContext.Current.Logger = logger;
                try
                {
                    action(actorProxyFactory);
                }
                catch (Exception ex)
                {
                    logger.FailedRequestContext(headers, ex);
                    throw ex;
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                    logger.StopRequestContext(headersArray);
                }
            });

            task.Start();

            return task;
        }

        public static Task RunInContext(this IServiceProxyFactory actorProxyFactory, Action<IServiceProxyFactory> action, IServiceCommunicationLogger logger, IEnumerable<ServiceRequestHeader> headers)
        {
            Task task = null;
            var headersArray = headers.ToArray();

            task = new Task(() =>
            {
                logger.StartRequestContext(headersArray);
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headersArray);
                ServiceRequestContext.Current.Logger = logger;
                try
                {
                    action(actorProxyFactory);
                }
                catch (Exception ex)
                {
                    logger.FailedRequestContext(headers, ex);
                    throw ex;
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                    logger.StopRequestContext(headersArray);
                }
            });

            task.Start();

            return task;
        }

        public static Task RunInContext(this IServiceProxyFactory actorProxyFactory, Action<IServiceProxyFactory> action, IServiceClientLogger logger, params ServiceRequestHeader[] headers)
        {
            Task task = null;
            var headersArray = headers.ToArray();

            task = new Task(() =>
            {
                logger?.StartRequestContext(headersArray);
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headers);
                try
                {
                    action(actorProxyFactory);
                }
                catch (Exception ex)
                {
                    logger?.FailedRequestContext(headers, ex);
                    throw ex;
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                    logger?.StopRequestContext(headersArray);
                }
            });

            task.Start();

            return task;
        }




        public static Task RunInRequestContext(Action action, IEnumerable<ServiceRequestHeader> headers)
        {
            Task task = null;

            task = new Task(() =>
            {
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headers);
                try
                {
                    action();
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                }
            });

            task.Start();

            return task;
        }

        public static Task RunInRequestContext(Func<Task> action, IEnumerable<ServiceRequestHeader> headers)
        {
            Task<Task> task = null;

            task = new Task<Task>(async () =>
            {
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headers);
                try
                {
                    await action();
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                }
            });

            task.Start();

            return task.Unwrap();
        }

        public static Task<TResult> RunInRequestContext<TResult>(Func<Task<TResult>> action, IEnumerable<ServiceRequestHeader> headers)
        {
            Task<Task<TResult>> task = null;

            task = new Task<Task<TResult>>(async () =>
            {
                Debug.Assert(ServiceRequestContext.Current == null);
                ServiceRequestContext.Current = new ServiceRequestContext(headers);
                try
                {
                    return await action();
                }
                finally
                {
                    ServiceRequestContext.Current = null;
                }

            });

            task.Start();

            return task.Unwrap<TResult>();
        }
    }
}