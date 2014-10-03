using System;

namespace Warranty.Core.Attributes
{
    public class CuteUiFileUploadOptionsAttribute : Attribute
    {
        public string FileTypes { get; set; }
        public bool AllowMultiple { get; set; }
        public int MaxFileSizeKb { get; set; }

        public CuteUiFileUploadOptionsAttribute()
        {
            AllowMultiple = true;
            MaxFileSizeKb = 20480;
        }
    }
}
