# Belitsoft Software Test Task

This repository contains a .NET project for interacting with the TripAdvisor API using RestSharp, NUnit, and FluentAssertions.

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio Code](https://code.visualstudio.com/) (or any other IDE of your choice)
- [Git](https://git-scm.com/)

## Setup

1. **Clone the repository:**

    ```bash
    git clone https://github.com/mefistov/BelitsoftSoftwareTestTask.git
    cd BelitsoftSoftwareTestTask
    ```

2. **Install dependencies:**

    ```bash
    dotnet restore
    ```

3. **Build the project:**

    ```bash
    dotnet build
    ```

## Running Tests

1. **Run all tests:**

    ```bash
    dotnet test
    ```

    This will execute all the tests in the `BelitsoftSoftwareTestTask.Tests` project.

## Task 
Create a backend automation framework in C# for the following API endpoint https://rapidapi.com/DataCrawler/api/tripadvisor16/

Create a unit test that will print out cruises with destination “Caribbean” and sort them by number of crew

Query ‘Get Cruises Location’ endpoint to get destinationId for Caribbean
Use the destinationId in ‘Search Cruises’ endpoint call to print out all ship names and sort the records in descending order by number of crew

Please add a README which contains information on how to set up the framework and run the test/s 
Follow SOLID and DRY principles 
Bonus points  
Create CI/CD pipeline (e.g. Github actions)

