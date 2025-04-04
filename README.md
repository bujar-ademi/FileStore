# FileStore.Api

## Overview
FileStore.Api is a .NET Core 3.1 web API project designed to manage file storage operations. It provides endpoints for uploading, downloading, and managing files in a secure and efficient manner. The project leverages JWT for authentication and authorization, ensuring that only authorized users can access the API.

## Features
- File upload and download
- File management (CRUD operations)
- JWT-based authentication and authorization
- Client base
- Factory storage
- Secure database connection

## Configuration
The project uses an `appsettings.json` file for configuration. Below is a description of the key settings:

### Connection Strings
The `ConnectionStrings` section contains the database connection string for the FileStore database:


### JWT Settings
The `JWTSettings` section contains the configuration for JWT authentication:


## Getting Started
To get started with the project, follow these steps:

1. Clone the repository:
`git clone https://github.com/your-repo/filestore-api.git`


2. Navigate to the project directory:

   `cd filestore-api`

3. Update the `appsettings.json` file with your database connection string and JWT settings.

4. Build the project:
`dotnet build`

5. Run the project:
`dotnet run`

## Database
The project uses SQL Server database. Migrations are used with FluentMigrator

### API Client
Each client can have its own storage and need to assign api key and secret. You have to register clients in ApiClient table. `StorageType` defines where files are stored for that client. Currently are supported two types (File and AzureBlobStorage). These table is used for authentication and authorization of client when calling endpoints

## Usage
Once you register client and it's settings in ApiClient table you can start using endpoints. 

#### Two options to call endpoints:
- Getting jwt token and passing the token in Authorization header as `Bearer ...`
- Calling endpoint directly by passing header called `token` as in the example:
```csharp
var client = new HttpClient();
client.BaseAddress = new Uri("path_to_storage_api");
var apiKey = Configuration.GetValue<string>("FileServer:ApiKey");
var apiSecret = Configuration.GetValue<string>("FileServer:ApiSecret");

var bytes = System.Text.Encoding.UTF8.GetBytes($"{apiKey}+{apiSecret}");
var hash = Convert.ToBase64String(bytes);

client.DefaultRequestHeaders.Add("token", hash);
```