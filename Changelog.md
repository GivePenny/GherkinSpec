# Changelog

## 2.1

### Added

* Support for French, German and Spanish languages.

### Changed

* Default maximum number of test cases run in parallel has been changed to 20. [Can be altered in configuration](docs/Configuration.md).

## 2.0

### Added

* Support for Gherkin features written in [different languages and targeting different cultures](docs/Localisation.md).  This includes parsing of dates and numbers as arguments to steps.
* Changelog to record major and breaking changes

### Changed

* **BREAKING CHANGE**: Feature files now default to "the invariant culture" meaning that dates and numbers will be parsed according to US English rules and a timezone of UTC.  To keep your current tests working as they did, add a `@culture(...)` tag to the top of each file as per this [Localisation guide](docs/Localisation.md).