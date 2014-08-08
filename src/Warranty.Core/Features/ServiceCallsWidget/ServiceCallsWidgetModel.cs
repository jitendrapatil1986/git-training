namespace Warranty.Core.Features.ServiceCallsWidget
{
    using System;
    using System.Collections.Generic;

    public class ServiceCallsWidgetModel
    {
        public ServiceCallsWidgetModel()
        {
            MyServiceCalls = new List<ServiceCall>();
        }

        public IEnumerable<ServiceCall> MyServiceCalls { get; set; }

        public class ServiceCall
        {
            public string Address { get; set; }
            public string CallNumber { get; set; }
            public DateTime CreatedDate { get; set; }
            public string HomeownerName { get; set; }
            public int NumberOfDaysRemaining { get; set; }
            public int NumberOfLineItems { get; set; }
            public string PhoneNumber { get; set; }

            public int PercentComplete
            {
                get
                {
                    if (NumberOfDaysRemaining == 0)
                        return 100;

                    var complete = (7.0 - NumberOfDaysRemaining)/7.0 * 100;
                    return Convert.ToInt16(complete);
                }
            }
        }
    }
}