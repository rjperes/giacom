# Introduction

First, we need to create the database by updating the database with the current migration:

`dotnet ef database update`

To execute the unit tests:

`dotnet test`


# Architectural Decisions

1) All controller and service actions are asynchronous for better scalability
2) Logging everywhere, to the appropriate levels
3) The API project does not expose any types from any of the other projects, just its own DTOs
4) AutoMapper is used to map from and to entities and DTOs
5) The EF context and model lives on its own project, migrations live on the web project. Using SQLite as the DB provider, but easy to change for a different DB
6) The services live on its own project too
7) MediatR is used as a decoupling/extensibility mechanism
8) Using Swagger/OpenAPI as a self-documenting and testing mechanism for the API, together with attributes for describing the status codes and content types
9) Using xUnit and NSubstitute for the unit tests and mocking framework


# Assumptions

1) Some assumptions on the types of columns, but they should be fine
2) Using the provided .csv file as a test input, realised it had errors, provided a configuration flag to continue importing in the case of an error on a line


# Future Enhancements

1) Instead of making changes to the DB directly on the controller, we could post a command using MediatR and have a handler take care of it. The same for queries
2) Currently, there is no check to see whether or not the imported line already exists on the DB, it should be implemented
