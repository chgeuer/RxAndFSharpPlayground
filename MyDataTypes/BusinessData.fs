namespace MyDataTypes

open System.Collections.Generic
open System.Runtime.CompilerServices

[<Extension>]
module BusinessData =
    type BusinessData =
        { CurrencyConversions: Currency.CurrencyConversions
          Airports: Airport.Airports
          Version: UpdateOffset option }

    let Zero: BusinessData =
        { CurrencyConversions = Map.empty
          Airports = Map.empty
          Version = None }

    type Update =
        | AirportUpdate of Airport.Update
        | CurrencyConversionUpdate of Currency.Update

    // A single update step
    let ApplyUpdate (offset: UpdateOffset) (update: Update) (data: BusinessData option): BusinessData option =
        match data with
        | None -> None
        | Some businessData ->
            let newVersion =
                match (businessData.Version, offset) with
                | (None, current) -> Some current
                | (Some previous, current) when previous < current -> Some current
                | (Some previous, current) when previous >= current -> None
                | _ -> None

            match (newVersion, update) with
            | (None, _) -> None // here we prevent updates from applying previous versions
            | (_, AirportUpdate airportUpdate) ->
                Some
                    { businessData with
                          Version = newVersion
                          Airports = Airport.ApplyUpdate businessData.Airports airportUpdate }
            | (_, CurrencyConversionUpdate currencyUpdate) ->
                Some
                    { businessData with
                          Version = newVersion
                          CurrencyConversions = Currency.ApplyUpdate businessData.CurrencyConversions currencyUpdate }

    // For the F# pipe operator (|>), the receiver needs to be the last argument
    let ApplyUpdates (updates: (UpdateOffset * Update) list) (businessData: BusinessData option): BusinessData option =
        List.fold (fun data (offset, update) -> ApplyUpdate offset update data) (businessData) (updates)
        
    // For the C# extension syntax, the receiver must be the first argument
    [<Extension>]
    let ApplyUpdatesCSharp (businessData: BusinessData) (updates: IEnumerable<UpdateOffset * Update>): BusinessData =
        match Some businessData |> ApplyUpdates(updates |> List.ofSeq) with
        | None -> failwith "Failed to apply updates" // On the outer border of the system, throw exceptions
        | Some x -> x

