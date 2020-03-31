namespace Tests
{
    using System;
    using System.Linq;
    using MyDataTypes;
    using Newtonsoft.Json;
    using Xunit;

    public class UnitTestUpdates
    {
        static BusinessData.Update NewCurrencyUpdate(string symbol, double rate) => 
            BusinessData.Update.NewCurrencyUpdate(
                new Currency.Update(symbol, rate));

        static BusinessData.Update AddAirportUpdate(string iataCode, string airportName, double latitude, double longtitude) =>
            BusinessData.Update.NewAirportUpdate(
                Airport.Update.NewAirportAdded(
                    new Airport.Airport(
                        iATACode: iataCode, name: airportName,
                        coordinates: new Airport.Coordinates(
                            latitude: latitude,
                            longtitude: longtitude))));

        static BusinessData.Update RemoveAirportUpdate(string iataCode) =>
            BusinessData.Update.NewAirportUpdate(
                Airport.Update.NewAirportRemoved(iATACode: iataCode));

        [Fact]
        public void Test2()
        {
            var updates = new (long, BusinessData.Update)[]
            {
                    (1, NewCurrencyUpdate("EUR-GBP", 1.3)),
                    (2, NewCurrencyUpdate("EUR-GBP", 1.4)),
                    (3, AddAirportUpdate("CDG", "Paris Charles de Gaulle Airport", latitude: 49.009724, longtitude: 2.547778)),
                    (4, AddAirportUpdate("DUS", "Düsseldorf International Airport", latitude: 51.286998852, longtitude: 6.7666636)),
                    (5, RemoveAirportUpdate("CDG")),
            }.Select(TupleExtensions.ToTuple);

            var v0 = BusinessData.Zero;
            var v1 = v0.ApplyUpdatesCSharp(updates.Take(1));
            var v2 = v1.ApplyUpdatesCSharp(updates.Skip(1).Take(1));
            var v3 = v2.ApplyUpdatesCSharp(updates.Skip(2).Take(1));
            var v4 = v3.ApplyUpdatesCSharp(updates.Skip(3).Take(1));
            var v5 = v3.ApplyUpdatesCSharp(updates.Skip(3).Take(2));
            var vEnd = v0.ApplyUpdatesCSharp(updates);

            Assert.True(v1.CurrencyConversions.Count == 1);
            Assert.Equal(1.3, v1.CurrencyConversions["EUR-GBP"]);
            Assert.Equal(1, v1.Version);

            Assert.True(v2.CurrencyConversions.Count == 1);
            Assert.Equal(1.4, v2.CurrencyConversions["EUR-GBP"]);
            Assert.Equal(2, v2.Version);

            Assert.Equal(vEnd, v5);
        }
    }

    public static class ME
    {
        public static string ToJSON<T>(this T t) => JsonConvert.SerializeObject(t);
    }
}