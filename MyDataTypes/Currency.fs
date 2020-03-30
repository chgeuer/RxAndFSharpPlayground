namespace MyDataTypes

module Currency =
    type CurrencyName = string

    [<Measure>]
    type currencyExchangeRate

    type CurrencyConversion =
        { From: CurrencyName
          To: CurrencyName }

    type CurrencyUpdate =
        { CurrencyConversion: CurrencyConversion
          CurrencyExchangeRate: float<currencyExchangeRate> }

    type CurrencyConversions =
        Map<CurrencyConversion, float<currencyExchangeRate>>

    let ApplyUpdate (conversions: CurrencyConversions) (update: CurrencyUpdate): CurrencyConversions =
        conversions.Add(update.CurrencyConversion, update.CurrencyExchangeRate)
