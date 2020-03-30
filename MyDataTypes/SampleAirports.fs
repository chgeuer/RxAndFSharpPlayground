namespace MyDataTypes

module SampleAirports =
    open BusinessData
    open Airport
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

    let Curr =
        [ ({ From = "GBP"
             To = "EUR" }, 1.2<currencyExchangeRate>) ]
        |> Map.ofSeq

    let SampleData: BusinessData.BusinessData =
        { CurrencyConversions = Curr
          Airports = Airports
          Version = UpdateOffset 2L }

    let UpdateSequence: BusinessData.Update list =
        [ Update.CurrencyUpdate
            ({ CurrencyConversion =
                   { From = "GBP"
                     To = "EUR" }
               CurrencyExchangeRate = 1.2<currencyExchangeRate> })
          Update.AirportUpdate(Airport.Update.AirportRemoved(IATACode "DUS")) ]

    let SampleData1: BusinessData.BusinessData =
        BusinessData.Zero 
        |> BusinessData.ApplyUpdate
            (UpdateOffset 1L)
            (Update.CurrencyUpdate
                ({ CurrencyConversion =
                       { From = "GBP"
                         To = "EUR" }
                   CurrencyExchangeRate = 1.2<currencyExchangeRate> }))
        |> BusinessData.ApplyUpdate
            (UpdateOffset 2L)
            (Update.CurrencyUpdate
                ({ CurrencyConversion =
                       { From = "EUR"
                         To = "GBP" }
                   CurrencyExchangeRate = 0.8<currencyExchangeRate> }))
      