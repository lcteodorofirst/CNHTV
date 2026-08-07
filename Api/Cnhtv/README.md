# Getting Started

## About

This app uses .NET 8 LTS.

### Style

This project uses an approach of DDD, to let things easier there is no generated valueobjects in favor of the odata, so we can balance well the commands with quering the data.

Tests are splitted in 3 different projects,

- E2E: The presentation layer should be tested, actions, permissions and etc
- Application: The Application layer should be tested
- Domain: The Domain layer should be tested

#### Why ODATA

ODATA helps fetching data from the api making pagination and filters very easy.

example:
url?$count=true&$skip=10&$take=3&$filter=startsWith(Name, 'Jonat')

it will return just 3 elements after the next 10 occurrences where name starts with Jonat

- Documented
- Easy to use/learn

The odata is also used to the incoming requests which can be very unpleasant like sending '1' on a property which accepts only number can throw unexpected exceptions while the default .net parser didnt... to avoid this behavior the Startup.endpoints.cs has this line:

`.ConfigureOData(services, showDetailedError: EnableDetailedInfo(), useNetDefaultInputProvider: true);` which ignores the odata in favor of the .net default

### Tests

The generated tests are using InMemoryDatabase, if needed check the TestSetup.cs to change
