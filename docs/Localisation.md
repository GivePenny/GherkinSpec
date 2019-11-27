# Localisation

## Overview

By default, GherkinSpec expects Feature files to be written in English following standard culture-neutral conventions.  For example, keywords such as `Feature` and `Scenario` should be exactly those words.  Dates, times and numbers follow `MM/DD/YYYY`, UTC time zone and `1,234.56` conventions respectively.  This can be changed by adding a `@culture(xx-XX)` tag to the feature, where `xx-XX` is a culture and language code.  Supported culture codes are listed below.

The `@culture(...)` tag does not need to be the first tag on the feature.

## How to Create a Feature for a Specific Culture

This example shows a Norwegian feature file, notice that the `culture()` tag can itself be localised.

```gherkin
@kultur(nb-NO)
Egenskap: Tittel på funksjoner

  Scenario: Tittel på scenario
  Gitt ...
  Når ...
  Så ...
```

When the culture for a feature is changed, any arguments to steps are parsed according to the rules of that culture.

Examples:

```gherkin
@kultur(nb-NO)
Egenskap: Tittel på funksjoner
  # This date will be parsed as the first day of the second month in the year 2010
  Scenario: Tittel på scenario
  Gitt En dato for 1.2.2010
```

```gherkin
@culture(en-US)
Feature: Feature title
  # This date will be parsed as the second day of the first month in the year 2010
  Scenario: Scenario title
  Given A date of 1/2/2010
```

```gherkin
@culture(en-GB)
Feature: Feature title
  # This date will be parsed as the first day of the second month in the year 2010
  Scenario: Scenario title
  Given A date of 1/2/2010
```

```gherkin
Feature: Feature title
  # There is no culture tag, so this date will be parsed as the second day of the first month in the year 2010
  Scenario: Scenario title
  Given A date of 1/2/2010
```

## Supported Cultures

The following culture codes are fully supported today (Gherkin can be written in these languages):

Code  | Language | Country (defines cultural rules for parsing dates, times, numbers)
------|----------|-------------------------------------------------------------------
en    | English  | Neutral
en-AU | English  | Australia
en-BZ | English  | Belize
en-CA | English  | Canada
en-GB | English  | United Kingdom
en-IE | English  | Ireland
en-IN | English  | India
en-JM | English  | Jamaica
en-MY | English  | Malaysia
en-NZ | English  | New Zealand
en-PH | English  | Republic of the Philippines
en-SG | English  | Singapore
en-TT | English  | Trinidad and Tobago
en-US | English  | United States
en-ZA | English  | South Africa
en-ZW | English  | Zimbabwe
fr    | French   | Neutral
fr-BE | French   | Belgium
fr-CA | French   | Canada
fr-CH | French   | Switzerland
fr-FR | French   | France
fr-LU | French   | Luxembourg
fr-MC | French   | Monaco
nb    | Bokmål   | Neutral
nb-NO | Bokmål   | Norway

Additionally, [any other culture code that is recognised by .NET Core](https://lonewolfonline.net/list-net-culture-country-codes/) is partially supported.  For partially supported cultures, GherkinSpec will accept that culture code so that dates, times and numbers can be written in the desired culture and parsed correctly.  Localised phrases and translations for keywords such as `Feature` aren't implemented for these cultures though and will fallback to English (Neutral).  This means that Features and Scenarios should be written in English if the culture-code you'd like to use isn't listed above.

Full support for more cultures is planned.  Please raise an issue on our GitHub issues list if you need a culture quickly.