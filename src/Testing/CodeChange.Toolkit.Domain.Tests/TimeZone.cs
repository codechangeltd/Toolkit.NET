using System;
using CodeChange.Toolkit.Culture;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeChange.Toolkit.Domain.Tests
{
    [TestClass]
    public class TimeZone
    {
        [TestMethod]
        public void TestMethod1()
        {
            ILocaleConfiguration localeConfiguration = new DefaultLocaleConfiguration();

            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Central Standard Time");
            DateTime datetime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);
            datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Local);

            Console.WriteLine("timeZone.BaseUtcOffset:" + timeZone.BaseUtcOffset);
            Console.WriteLine("timeZone.BaseUtcOffset.Ticks:" + timeZone.BaseUtcOffset.Ticks);

            Console.WriteLine("timeZone.BaseUtcOffset.TotalMinutes:" + timeZone.BaseUtcOffset.TotalMinutes);

            Console.WriteLine("timeZone.GetUtcOffset():" + timeZone.GetUtcOffset(DateTime.Now));

            Console.WriteLine("datetime:" + datetime);

            var inputOffset = Convert.ToInt32(timeZone.BaseUtcOffset.TotalMinutes);
            localeConfiguration.SetTimeZoneOffset(-inputOffset);

            var datetime2 = datetime.LocalToUtcTime(localeConfiguration);

            Console.WriteLine(datetime2);
        }
    }
}
