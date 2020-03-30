namespace RxDemo
{
    using System;
    using MyDataTypes;
    using static MyDataTypes.Airport;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{SampleAirports.Airports[IATACode.NewIATACode("DUS")].Coordinates.Latitude}");
            Console.WriteLine($"{SampleAirports.UpdateSequence}");
            
        }
    }
}
