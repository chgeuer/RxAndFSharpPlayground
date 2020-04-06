namespace Tests
{
    using System;
    using System.Linq;
    using Microsoft.FSharp.Collections;
    using Microsoft.FSharp.Core;
    using MyDataTypes;
    using Newtonsoft.Json;
    using Xunit;

    public class UnitTestUpdates
    {
        static BusinessData.Update NewCurrencyUpdate(string symbol, double rate) => 
            BusinessData.Update.NewCurrencyConversionUpdate(
                new Currency.Update(symbol, rate));

        static BusinessData.Update AddAirportUpdate(string iataCode, string airportName, double latitude, double longtitude) =>
            BusinessData.Update.NewAirportUpdate(
                Airport.Update.NewAirportAdded(
                    new Airport.Airport(
                        iATACode: iataCode, name: airportName, enabled: true,
                        coordinates: new Airport.Coordinates(
                            latitude: latitude,
                            longtitude: longtitude))));

        static BusinessData.Update RemoveAirportUpdate(string iataCode) =>
            BusinessData.Update.NewAirportUpdate(
                Airport.Update.NewAirportRemoved(iATACode: iataCode));

        static BusinessData.Update ChangeAirportName(string iataCode, string name) =>
            BusinessData.Update.NewAirportUpdate(
                Airport.Update.NewAirportNameChanged(iataCode, name));

        [Fact]
        public void Test2()
        {


            //var map = new FSharpMap<string, int>(Array.Empty<Tuple<string, int>>());
            //var map2 = map.Add("Chris", 2);
            //var map3 = map2.Add("Chris", 3);

            //Assert.False(map.ContainsKey("Chris"));
            //Assert.Equal(2, map2["Chris"]);
            //Assert.Equal(3, map3["Chris"]);

            var updates = new (long, BusinessData.Update)[]
            {
                    (1, NewCurrencyUpdate("EUR-GBP", 1.3)),
                    (2, NewCurrencyUpdate("EUR-GBP", 1.4)),
                    (3, AddAirportUpdate("CDG", "Paris Charles de Gaulle Airport", latitude: 49.009724, longtitude: 2.547778)),
                    (4, AddAirportUpdate("DUS", "Düsseldorf International Airport", latitude: 51.286998852, longtitude: 6.7666636)),
                    (5, RemoveAirportUpdate("CDG")),
                    (6, ChangeAirportName("DUS", "Dusseldorf")),
            }.Select(TupleExtensions.ToTuple);


            //var u = updates.Skip(2).First().Item2;
            //var json = JsonConvert.SerializeObject(u);
            //var u2 = JsonConvert.DeserializeObject<BusinessData.Update>(json);

            //Assert.Equal(u, u2);
            //Assert.Equal("gaga", json);


            var v0 = BusinessData.Zero;
            var v1 = v0.ApplyUpdatesCSharp(updates.Take(1));
            var v2 = v1.ApplyUpdatesCSharp(updates.Skip(1).Take(1));
            var v3 = v2.ApplyUpdatesCSharp(updates.Skip(2).Take(1));
            var v4 = v3.ApplyUpdatesCSharp(updates.Skip(3).Take(1));
            var v5 = v3.ApplyUpdatesCSharp(updates.Skip(3).Take(2));
            var v6 = v5.ApplyUpdatesCSharp(new[] { updates.Last() });
            var vEnd = v0.ApplyUpdatesCSharp(updates);

            Assert.Equal(FSharpOption<long>.None, v0.Version);

            Assert.True(v1.CurrencyConversions.Count == 1);
            Assert.Equal(1.3, v1.CurrencyConversions["EUR-GBP"]);
            Assert.Equal(1, v1.Version);

            Assert.True(v2.CurrencyConversions.Count == 1);
            Assert.Equal(1.4, v2.CurrencyConversions["EUR-GBP"]);
            Assert.Equal(2, v2.Version);

            Assert.Single(v3.Airports);
            Assert.Equal(2, v4.Airports.Where(a => a.Value.Enabled == true).Count());
            Assert.Equal(1, v5.Airports.Where(a => a.Value.Enabled == true).Count() + 0);

            Assert.Equal("Dusseldorf", v6.Airports["DUS"].Name);

            Assert.Equal(vEnd, v6);
        }
    }

    public static class ME
    {
        public static string ToJSON<T>(this T t) => JsonConvert.SerializeObject(t);
    }
}