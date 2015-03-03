namespace Warranty.Core
{
    using System.Collections.Generic;

    public interface ISystemMonitor
    {
        bool HasMessages { get; }

        IEnumerable<string> Messages { get; }
    }
}