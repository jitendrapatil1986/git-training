namespace Warranty.Core.Services
{
    using System;
    using System.Collections.Generic;

    public class SurveyClientSystemMonitor : ISystemMonitor
    {
        private readonly int _failuresToAssumeDown;
        private DateTime _exceptionStartTime;
        private readonly TimeSpan _timeToTryAgain;
        private readonly List<KeyValuePair<string, Exception>> _exceptions = new List<KeyValuePair<string, Exception>>();

        public SurveyClientSystemMonitor(int failuresToAssumeDown, TimeSpan timeToTryAgain)
        {
            _failuresToAssumeDown = failuresToAssumeDown;
            _timeToTryAgain = timeToTryAgain;
        }

        public bool SurveyClientAppearsDown
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
            get { return SurveyClientAppearsDown; }
        }

        public IEnumerable<string> Messages
        {
            get
            {
                if (SurveyClientAppearsDown)
                    yield return "The Survey API appears to be down. Please try again later.";
            }
        }
    }
}