namespace MyDataTypes

module SampleAirports =
    open Airport
    open BusinessData
    open Currency

    let Airports =
        [ { IATACode = IATACode "LHR"
            Name = "London Heathrow"
            Coordinates =
                { Latitude = 51.470020<degree>
                  Longtitude = -0.454295<degree> } }
          { IATACode = IATACode "DUS"
            Name = "Düsseldorf"
            Coordinates =
                { Latitude = 51.286998852<degree>
                  Longtitude = 6.7666636<degree> } } ]
        |> Seq.map (fun ap -> ap.IATACode, ap)
        |> Map.ofSeq

    let UpdateSequence: Update list =
        [ Update.CurrencyUpdate
            ({ CurrencyConversion =
                   { From = "GBP"
                     To = "EUR" }
               CurrencyExchangeRate = 1.2<currencyExchangeRate> })
          Update.AirportUpdate(AirportUpdate.AirportRemoved(IATACode "DUS")) ]
