# Changelog

## 2.0

### Added

* Support for Gherkin features written in [different languages and targeting different cultures](docs/Localisation.md).  This includes parsing of dates and numbers as arguments to steps.
* Changelog to record major and breaking changes

### Changed

* **BREAKING CHANGE**: Feature files now default to "the invariant culture" meaning that dates and numbers will be parsed according to UK English rules and a timezone of UTC. This [can be changed per-feature-file](docs/Localisation.md).