using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Messages;
using NServiceBus;
using Warranty.HealthCheck.Handlers;

namespace Warranty.Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new List<Assembly>
            {
                Assembly.GetAssembly(typeof (InitiateApprovedShowcasesHealthCheckSaga))
            };

            Bus = Configure.With(assemblies)
                .DefaultBuilder()
                .UnicastBus()
                .UseTransport<Msmq>()
                .DefiningEventsAs(e => e.IsBusEvent())
                .DefiningCommandsAs(e => e.IsBusCommand())
                .DefiningMessagesAs(e => e.IsBusMessage())
                .SendOnly();

            Bus.Send(new InitiateApprovedShowcasesHealthCheckSaga());
            Console.WriteLine("Sent!");
            Console.Read();
        }

        public static IBus Bus { get; set; }
    }
}
