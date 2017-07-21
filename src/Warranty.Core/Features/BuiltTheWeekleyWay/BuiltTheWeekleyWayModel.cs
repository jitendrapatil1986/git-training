namespace Warranty.Core.Features.BuiltTheWeekleyWay
{
    using System.Collections.Generic;

    public class BuiltTheWeekleyWayModel
    {
        public IList<ContentRow> Contents { get; set; }

        public string MarketName { get; set; }

        public class ContentRow
        {
            public string DisplayName { get; set; }
            public string PathToFile { get; set; }
            public string FileName { get; set; }
            public string MarketCode { get; set; }

            public bool IsSection
            {
                get
                {
                    var periodIndex = DisplayName.IndexOf('.', 0);
                    return DisplayName[periodIndex + 1] == '0';
                }
            }
        }
    }
}
