namespace Warranty.Core.Services
{
    using System;
    using System.Collections.Generic;

    public class AccountingClientSystemMonitor : ISystemMonitor
    {
        private readonly int _failuresToAssumeDown;
        private DateTime _exceptionStartTime;
        private readonly TimeSpan _timeToTryAgain;
        private readonly List<KeyValuePair<string, Exception>> _exceptions = new List<KeyValuePair<string, Exception>>();

        public AccountingClientSystemMonitor(int failuresToAssumeDown, TimeSpan timeToTryAgain)
        {
            _failuresToAssumeDown = failuresToAssumeDown;
            _timeToTryAgain = timeToTryAgain;
        }

        public bool AccountingClientAppearsDown
        {
            get
            {
                var assumeDown = _exceptions.Count >= _failuresToAssumeDown;
                if (_exceptionStartTime + _timeToTryAgain < SystemTime.Now && assumeDown)
                {
                    _exceptions.Clear();
                    return false;
                }
                return assumeDown;
            }
        }

        public void LogException(string requestUri, Exception webException)
        {
            _exceptions.Add(new KeyValuePair<string, Exception>(requestUri, webException));

            if (_exceptions.Count == 1) // only keep track of the time from the first exception
                _exceptionStartTime = SystemTime.Now;
        }

        public bool HasMessages
        {
            get { return AccountingClientAppearsDown; }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                if (AccountingClientAppearsDown)
                    yield return "The Accounting API appears to be down. Please try again later.";
            }
        }
    }
}