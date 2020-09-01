namespace MyDataTypes

module Currency =
    type CurrencyConversion = string // "EUR-GBP"

    [<Measure>]
    type currencyExchangeRate

    type CurrencyConversions = Map<CurrencyConversion, float<currencyExchangeRate>>

    type Update =
        { CurrencyConversion: CurrencyConversion
          CurrencyExchangeRate: float<currencyExchangeRate> }

    let ApplyUpdate (conversions: CurrencyConversions) (update: Update): CurrencyConversions =
        conversions.Add(update.CurrencyConversion, update.CurrencyExchangeRate)


// 'MyDataTypes.Currency+CurrencyConversion'. Create a TypeConverter to convert from the string to the key type object.
