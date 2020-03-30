namespace MyDataTypes

module BusinessData =
    type BusinessData =
        { CurrencyConversions: Currency.CurrencyConversions
          Airports: Airport.Airports
          Version: UpdateOffset }

    type Update =
        | AirportUpdate of Airport.AirportUpdate
        | CurrencyUpdate of Currency.CurrencyUpdate

    let ApplyUpdate (data: BusinessData) (version: UpdateOffset)  (update: Update): BusinessData =
        match update with
        | AirportUpdate airportUpdate -> 
            { data with 
                Version  = version
                Airports = Airport.ApplyUpdate data.Airports  airportUpdate }
        | CurrencyUpdate currencyUpdate ->
            { data with 
                Version  = version
                CurrencyConversions = Currency.ApplyUpdate data.CurrencyConversions currencyUpdate }
