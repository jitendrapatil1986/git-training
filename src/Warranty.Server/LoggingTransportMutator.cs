using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using NServiceBus;
using NServiceBus.MessageMutator;

namespace Warranty.Server
{
    public class LoggingTransportMutator : IMutateIncomingTransportMessages
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (LoggingTransportMutator));

        public void MutateIncoming(TransportMessage transportMessage)
        {
            var body = Encoding.UTF8.GetString(transportMessage.Body);
            body = body.Replace("\r", "");
            body = body.Replace("\n", "");
            _log.InfoFormat("Received {0}", body);
        }
    }
}