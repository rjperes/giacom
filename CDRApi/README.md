# Introduction

First, we need to create the database by updating the database with the current migration:

`dotnet ef database update`

# Architectural Decisions

1) All controller actions are asynchronous
2) All service methods are asynchronous
3) The API project does not expose any types from any of the other projects
4) AutoMapper is used to map from and to entities and DTOs
5) The EF context and model lives on its own project. Using SQLite, but easy to change for a different DB
6) The services live on its own project too
7) MediatR is used as a decoupling/extensibility mechanism
8) Using Swagger/OpenAPI as a self-documenting and testing mechanism for the API, together with attributes for describing the return codes and content types
9) Using xUnit and NSubstitute for the unit tests and mocking framework