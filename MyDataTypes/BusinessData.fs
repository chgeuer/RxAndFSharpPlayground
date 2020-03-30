namespace MyDataTypes

open System.Collections.Generic
open System.Runtime.CompilerServices

[<Extension>]
module BusinessData =
    type BusinessData =
        { CurrencyConversions: Currency.CurrencyConversions
          Airports: Airport.Airports
          Version: UpdateOffset }

    let Zero: BusinessData =
        { CurrencyConversions = Map.empty
          Airports = Map.empty
          Version = UpdateOffset -1L }

    type Update =
        | AirportUpdate of Airport.Update
        | CurrencyUpdate of Currency.Update

    let ApplyUpdate (offset: UpdateOffset) (update: Update) (data: BusinessData): BusinessData =
        match update with
        | AirportUpdate airportUpdate ->
            { data with
                  Version = offset
                  Airports = Airport.ApplyUpdate data.Airports airportUpdate }
        | CurrencyUpdate currencyUpdate ->
            { data with
                  Version = offset
                  CurrencyConversions = Currency.ApplyUpdate data.CurrencyConversions currencyUpdate }

    let ApplyUpdates (updates: (UpdateOffset * Update) list) (businessData: BusinessData): BusinessData =
        updates |> List.fold (fun data (offset, update) -> ApplyUpdate offset update data) businessData

    [<Extension>]
    let ApplyUpdatesCSharp (businessData: BusinessData) (updates: IEnumerable<UpdateOffset * Update>): BusinessData =
        businessData |> ApplyUpdates( updates |> List.ofSeq)
