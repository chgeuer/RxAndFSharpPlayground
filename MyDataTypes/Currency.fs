namespace MyDataTypes

module Currency =
    type CurrencyName = string

    [<Measure>]
    type currencyExchangeRate

    type CurrencyConversion =
        { From: CurrencyName
          To: CurrencyName }

    type Update =
        { CurrencyConversion: CurrencyConversion
          CurrencyExchangeRate: float<currencyExchangeRate> }

    type CurrencyConversions = Map<CurrencyConversion, float<currencyExchangeRate>>

    let ApplyUpdate (conversions: CurrencyConversions) (update: Update): CurrencyConversions =
        conversions.Add(update.CurrencyConversion, update.CurrencyExchangeRate)
