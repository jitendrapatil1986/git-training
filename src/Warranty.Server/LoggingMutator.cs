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
    public class LoggingMutator : IMutateIncomingMessages
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (LoggingMutator));

        public object MutateIncoming(object message)
        {
            var messageType = message.GetType().Name;

            var messageId = "null";
            var messageSource = "null";

            if (message is IMessage)
            {
                messageId = Headers.GetMessageHeader(message, "NServiceBus.MessageId");

                var originatingEndpoint = Headers.GetMessageHeader(message, "NServiceBus.OriginatingEndpoint");
                var originatingMachine = Headers.GetMessageHeader(message, "NServiceBus.OriginatingMachine");
                messageSource = string.Format("{0}@{1}", originatingEndpoint, originatingMachine);
            }

            _log.WarnFormat("Received {0} in {1} from {2}", messageType, messageId, messageSource);
            return message;
        }
    }
}