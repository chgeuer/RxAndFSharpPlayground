namespace RxDemo
{
    using System;
    using System.Linq;
    using MyDataTypes;

    class Program
    {
        static void Main(string[] args)
        {
            // _ = SampleAirports.Airports[IATACode.NewIATACode("DUS")].Coordinates.Latitude;

            var updates = new (long, BusinessData.Update)[]
            {
                (10, BusinessData.Update.NewCurrencyUpdate(new Currency.Update(new Currency.CurrencyConversion(from: "EUR", to: "GBP"), 1.3)))
            }
            .Select(t => (UpdateOffset.NewUpdateOffset(t.Item1), t.Item2))
            .Select(TupleExtensions.ToTuple);


            var bd = BusinessData.Zero.ApplyUpdatesCSharp(updates);
            Console.WriteLine($"{bd}");
        }
    }
}
