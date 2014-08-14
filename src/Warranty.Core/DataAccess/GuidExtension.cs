using System;

namespace Warranty.Core.DataAccess
{
    public static class GuidComb
    {
        public static Guid Generate()
        {
            byte[] destinationArray = Guid.NewGuid().ToByteArray();
            var time = new DateTime(0x76c, 1, 1);
            var now = DateTime.Now;

            // Get the days and milliseconds which will be 
            // used to build the byte string
            var span = new TimeSpan(now.Ticks - time.Ticks);
            var timeOfDay = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a  
            // millisecond so we divide by 3.333333
            var bytes = BitConverter.GetBytes(span.Days);

            var array = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds/3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(bytes);

            Array.Reverse(array);

            // Copy the bytes into the guid
            Array.Copy(bytes, bytes.Length - 2, destinationArray, destinationArray.Length - 6, 2);

            Array.Copy(array, array.Length - 4, destinationArray, destinationArray.Length - 4, 4);

            return new Guid(destinationArray);
        }
    }
}