namespace MyDataTypes

module SampleAirports =
    open BusinessData
    open Airport
    open Currency

    let UpdateSequence: (UpdateOffset * BusinessData.Update) list =
        [ 1L,
          Update.CurrencyConversionUpdate
              ({ CurrencyConversion = "GBP-EUR"
                 CurrencyExchangeRate = 1.2<currencyExchangeRate> })
          2L,
          Update.CurrencyConversionUpdate
              ({ CurrencyConversion = "EUR-GBP"
                 CurrencyExchangeRate = 0.8<currencyExchangeRate> })
          3L,
          Update.AirportUpdate
              (Airport.Update.AirportAdded
                  ({ IATACode = "DUS"
                     Name = "Düsseldorf"
                     Enabled = true
                     Coordinates =
                         { Latitude = 51.286998852<degree>
                           Longtitude = 6.7666636<degree> } }))
          4L, Update.AirportUpdate(Airport.Update.AirportRemoved("DUS")) ]

    let SampleDataFromUpdateSequence: BusinessData.BusinessData option =
        Some BusinessData.Zero |> BusinessData.ApplyUpdates(UpdateSequence)

    let SampleData: BusinessData.BusinessData =
        { CurrencyConversions = [ ("GBP-EUR", 1.2<currencyExchangeRate>) ] |> Map.ofSeq
          Airports =
              [ { IATACode = "LHR"
                  Name = "London Heathrow"
                  Enabled = true
                  Coordinates =
                      { Latitude = 51.470020<degree>
                        Longtitude = -0.454295<degree> } }
                { IATACode = "DUS"
                  Name = "Düsseldorf"
                  Enabled = true
                  Coordinates =
                      { Latitude = 51.286998852<degree>
                        Longtitude = 6.7666636<degree> } } ]
              |> Seq.map (fun ap -> ap.IATACode, ap)
              |> Map.ofSeq
          Version = Some 2L }
