namespace RxDemo
{
    using System;
    using System.Linq;
    using MyDataTypes;
    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {
            // _ = SampleAirports.Airports[IATACode.NewIATACode("DUS")].Coordinates.Latitude;

            var csharpUpdates = new (long, BusinessData.Update)[]
            {
                (10, BusinessData.Update.NewCurrencyUpdate(new Currency.Update("EUR-GBP", 1.3)))
            }
            .Select(TupleExtensions.ToTuple);

            Console.WriteLine($"{BusinessData.Zero.ApplyUpdatesCSharp(csharpUpdates).ToJSON()}");
            var d = JsonConvert.DeserializeObject<BusinessData.BusinessData>(SampleAirports.SampleData.ToJSON());

            foreach (var (offset, update) in SampleAirports.UpdateSequence)
            {
                Console.WriteLine($"{offset}: {update.ToJSON()}");
            }

            Console.WriteLine($"{SampleAirports.SampleDataFromUpdateSequence.ApplyUpdatesCSharp(SampleAirports.UpdateSequence).ToJSON()}");
        }
    }

    public static class ME
    {
        public static string ToJSON<T>(this T t) => JsonConvert.SerializeObject(t);
    }
}
