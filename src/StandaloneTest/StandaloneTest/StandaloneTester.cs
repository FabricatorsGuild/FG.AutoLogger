using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneTest
{
    public class StandaloneTester
    {
        private IDomainLogger _domainLogger;

        public StandaloneTester()
        {
            _domainLogger = new DomainLogger(Environment.TickCount, Guid.NewGuid());
        }
        public void Run()
        {
            _domainLogger.StandaloneTestInvoked("Starting the test", Environment.TickCount);

            var i = 0;
            while (true)
            {
                _domainLogger.StandaloneTestRunning(i);
                try
                {
                    throw new NotImplementedException("Hepp");
                }
                catch (Exception e)
                {
                    _domainLogger.StandaloneTestFailed(e);

                    //throw;
                }
                i++;

                Task.Delay(1000).GetAwaiter().GetResult();                
            }
        }
    }
}
