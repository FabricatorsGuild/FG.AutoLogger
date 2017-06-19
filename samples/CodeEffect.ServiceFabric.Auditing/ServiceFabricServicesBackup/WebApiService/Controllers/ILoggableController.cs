using System;
using WebApiService.Diagnostics;

namespace WebApiService.Controllers
{
    public interface ILoggableController
    {
        IWebApiLogger Logger { get; }
        IDisposable RequestLoggingContext { get; set; }
    }
}