namespace Warranty.UI.Core.Enumerations
{
    using System.Collections.Generic;

    public class ToastType
    {
        public static ToastType Success = new ToastType("success");
        public static ToastType Warning = new ToastType("warning");
        public static ToastType Error = new ToastType("error");
        public static ToastType Info = new ToastType("info");

        public string Type { get; private set; }

        protected ToastType(string type)
        {
            Type = type;
        }

        public string TempDataValue(string message)
        {
            return string.Format("{0}|{1}", Type, message);
        }

        public static KeyValuePair<string, string> SplitTempDataValue(string toastData)
        {
            var value = toastData.Split('|');
            return new KeyValuePair<string, string>(value[0], value[1]);
        }
    }
}