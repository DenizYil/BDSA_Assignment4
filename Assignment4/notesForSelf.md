# notes

To use postgres, a few modifications to Rasmus' code. Mainly using the Npgsql package, which does not allow schema creation. So schemas must be made by SQL before the initial migration.  

'User' entity has been renamed to 'Developer' since 'User' is a reserved keyword in sql.

connection string syntax: "Server=localhost;Port=5432;Database=BDSA/Assignment4;User ID=postgres;Password=password;"

To migrate: cd to Assignment4 then: "dotnet ef migrations add MigrationId --project ../Assignment4.Entities"