using System;

namespace Warranty.Core
{
    public static class SystemTime
    {
        private static readonly Func<DateTime> NowCore = () => DateTime.Now;

        private static Func<DateTime> _now = NowCore;

        public static DateTime Now
        {
            get { return _now(); }
        }

        public static DateTime Today
        {
            get { return _now().Date; }
        }

        public static void Stub(DateTime now, Action action)
        {
            _now = () => now;

            action();

            _now = NowCore;
        }
    }
}
