namespace Warranty.Tests.Core
{
    using NUnit.Framework;
    using Warranty.Core.Extensions;
    using Should;

    [TestFixture]
    public class IntExtensionsTester
    {
        [TestCase(1, "1st")]
        [TestCase(2, "2nd")]
        [TestCase(3, "3rd")]
        [TestCase(4, "4th")]
        [TestCase(5, "5th")]
        [TestCase(6, "6th")]
        [TestCase(7, "7th")]
        [TestCase(8, "8th")]
        [TestCase(9, "9th")]
        [TestCase(10, "10th")]
        [TestCase(11, "11th")]
        [TestCase(20, "20th")]
        [TestCase(21, "21st")]
        [TestCase(22, "22nd")]
        [TestCase(23, "23rd")]
        [TestCase(24, "24th")]
         public void Should_return_correct_value(int number, string result)
         {
             number.ToOrdinalSuffixed().ShouldEqual(result);
         }
    }
}