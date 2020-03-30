namespace MyDataTypes

module Currency =
    type CurrencyConversion = string

    [<Measure>]
    type currencyExchangeRate

    type Update =
        { CurrencyConversion: CurrencyConversion
          CurrencyExchangeRate: float<currencyExchangeRate> }

    type CurrencyConversions = Map<CurrencyConversion, float<currencyExchangeRate>>

    let ApplyUpdate (conversions: CurrencyConversions) (update: Update): CurrencyConversions =
        conversions.Add(update.CurrencyConversion, update.CurrencyExchangeRate)
// 'MyDataTypes.Currency+CurrencyConversion'. Create a TypeConverter to convert from the string to the key type object.
