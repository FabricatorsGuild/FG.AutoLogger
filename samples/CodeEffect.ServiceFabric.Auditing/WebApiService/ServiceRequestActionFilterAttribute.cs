using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FG.ServiceFabric.Services.Remoting.FabricTransport;
using WebApiService.Controllers;

namespace WebApiService
{


	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	internal sealed class ServiceRequestActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
		    var valuesController = actionContext.ControllerContext.Controller as ILoggableController;
		    if (valuesController != null)
		    {
                valuesController.RequestLoggingContext = valuesController.Logger.RecieveWebApiRequest(
                    requestUri: actionContext.Request.RequestUri, 
                    payload: "",
                    correlationId: ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId],
                    userId: ServiceRequestContext.Current?[ServiceRequestContextKeys.UserId]);
		    }
		}

		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
            var valuesController = actionExecutedContext.ActionContext.ControllerContext.Controller as ILoggableController;
		    valuesController?.RequestLoggingContext?.Dispose();
		}

	    public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
	    {
            var valuesController = actionContext.ControllerContext.Controller as ILoggableController;
            if (valuesController != null)
            {
                valuesController.RequestLoggingContext = valuesController.Logger.RecieveWebApiRequest(
                    requestUri: actionContext.Request.RequestUri,
                    payload: "",
                    correlationId: ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId],
                    userId: ServiceRequestContext.Current?[ServiceRequestContextKeys.UserId]);
            }

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
	    }

	    public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
	    {
            var valuesController = actionExecutedContext.ActionContext.ControllerContext.Controller as ILoggableController;
	        if (actionExecutedContext?.Exception != null)
	        {
	            valuesController?.Logger.RecieveWebApiRequestFailed(actionExecutedContext.Request.RequestUri, actionExecutedContext.Request.ToString(), ServiceRequestContext.Current?[ServiceRequestContextKeys.CorrelationId], ServiceRequestContext.Current?[ServiceRequestContextKeys.UserId], actionExecutedContext.Exception);
	        }

            valuesController?.RequestLoggingContext?.Dispose();

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
	    }
	}
}
