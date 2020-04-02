namespace MyDataTypes

module Airport =
    [<Measure>]
    type degree

    type Coordinates =
        { Latitude: float<degree>
          Longtitude: float<degree> }

    type IATACode = string

    type Airport =
        { IATACode: IATACode
          Name: string
          Coordinates: Coordinates }

    type Airports = Map<IATACode, Airport>

    type Update =
        | AirportAdded of Airport: Airport
        | AirportRemoved of IATACode: IATACode
        | AirportNameChanged of IATACode: IATACode * Name: string

    let UpdateAirportName (airports: Airports) (iata: IATACode) (name: string) =
        match airports.TryFind iata with
        | None -> airports
        | Some airport ->
            let updatedAirport = { airport with Name = name }
            airports.Add(updatedAirport.IATACode, updatedAirport)

    let ApplyUpdate (airports: Airports) (update: Update): Airports =
        match update with
        | AirportAdded airport -> airports.Add(airport.IATACode, airport)
        | AirportRemoved iataCode -> airports.Remove(iataCode)
        | AirportNameChanged (iata, name) -> UpdateAirportName airports iata name
