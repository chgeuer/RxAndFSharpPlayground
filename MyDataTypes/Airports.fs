namespace MyDataTypes

module Airport =
    [<Measure>]
    type degree

    type Coordinates =
        { Latitude: float<degree>
          Longtitude: float<degree> }

    type IATACode = IATACode of string

    type Airport =
        { IATACode: IATACode
          Name: string
          Coordinates: Coordinates }

    type Airports = Map<IATACode, Airport>

    type Update =
        | AirportAdded of Airport: Airport
        | AirportRemoved of IATACode: IATACode

    let ApplyUpdate (airports: Airports) (update: Update): Airports =
        match update with
        | AirportAdded airport -> airports.Add(airport.IATACode, airport)
        | AirportRemoved iataCode -> airports.Remove(iataCode)
