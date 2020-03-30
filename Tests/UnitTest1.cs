namespace Tests
{
    using System;
    using System.Linq;
    using MyDataTypes;
    using Newtonsoft.Json;
    using Xunit;

    public class UnitTests
    {
        [Fact]
        public void Test1()
        {
            Func<string, double, BusinessData.Update> currencyUpdate = (symbol, rate) => BusinessData.Update.NewCurrencyUpdate(new Currency.Update(symbol, rate));

            var csharpUpdates = new (long, BusinessData.Update)[]
            {
                (10, currencyUpdate("EUR-GBP", 1.3)),
                (11, currencyUpdate("EUR-GBP", 1.4)),
            }
            .Select(TupleExtensions.ToTuple);

            var data10 = BusinessData.Zero.ApplyUpdatesCSharp(csharpUpdates.Take(1));
            var data11 = BusinessData.Zero.ApplyUpdatesCSharp(csharpUpdates.Take(2));
            var data11_2 = data10.ApplyUpdatesCSharp(csharpUpdates.Skip(1));

            Assert.True(data10.CurrencyConversions.Count == 1);
            Assert.Equal(1.3, data10.CurrencyConversions["EUR-GBP"]);
            Assert.Equal(10, data10.Version);

            Assert.True(data11.CurrencyConversions.Count == 1);
            Assert.Equal(1.4, data11.CurrencyConversions["EUR-GBP"]);
            Assert.Equal(11, data11.Version);

            Assert.Equal(data11, data11_2);
        }
    }

    public static class ME
    {
        public static string ToJSON<T>(this T t) => JsonConvert.SerializeObject(t);
    }
}