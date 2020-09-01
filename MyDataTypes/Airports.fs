namespace MyDataTypes

module Airport =

    [<Measure>]
    type degree

    type Coordinates =
        { Latitude: float<degree>
          Longtitude: float<degree> }

    type IATACode = string


    //open System

    //type Person =
    //    { EmployeeID: int }

    //type AuditRecord =
    //    { UpdatedAt: DateTimeOffset
    //      UpdatedBy: Person }
    
    type Airport =
        { IATACode: IATACode
          Name: string
          Coordinates: Coordinates
          Enabled: bool
          //AuditRecord: AuditRecord
          }

    type Airports = Map<IATACode, Airport>

    type Update =
        | AirportPut of Airport: Airport     // Add new airport or replace/overwrite an existing one
        | AirportRemoved of IATACode: IATACode // Delete an existing airport (or ignore the command if airport didn't exist)
        | AirportNameChanged of IATACode: IATACode * Name: string

    let UpdateAirportName (airports: Airports) (iata: IATACode) (name: string) =
        match airports.TryFind iata with
        | None -> airports
        | Some airport ->
            let updatedAirport = { airport with Name = name }
            airports.Add(updatedAirport.IATACode, updatedAirport)

    let DisableAirport (airports: Airports) (iata: IATACode): Airports =
        match airports.TryFind iata with
        | None -> airports
        | Some airport ->
            airports.Add(airport.IATACode, { airport with Enabled = false })

    let ApplyUpdate (airports: Airports) (update: Update): Airports =
        match update with
        | AirportPut newAirport -> airports.Add(newAirport.IATACode, newAirport)
        | AirportRemoved iataCode -> DisableAirport airports iataCode
        | AirportNameChanged (iata, name) -> UpdateAirportName airports iata name
