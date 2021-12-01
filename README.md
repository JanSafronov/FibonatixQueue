# FibonatixQueue
Microservice project based on Redis queue management and additional NoSQL services.

## Services & Frameworks
Redis StackExchange library is used together with .NET Core,
Additionally, the API supports Azure queue storage and MongoDB NoSQL services but interprets them with appropriate Redis terms.
The application automatically redirects you to swagger url (you can change settings directly, read more: https://swagger.io/)

The API uses external libraries and environments such as:
 - [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis/blob/main/README.md) - General purpose Redis library supported by Stack Exchange
 - [Azure.Storage](https://github.com/Azure/azure-sdk-for-net/blob/Azure.Storage.Queues_12.8.0/sdk/storage/README.md) - Azure supported library for storing various types of data, blobs, queues and more
 - [MongoDB.Driver](https://docs.mongodb.com/drivers/csharp/) - Official MongoDB C#/.NET library provides asynchronous interactions with MongoDB
 - [Newtonsoft.Json](https://www.newtonsoft.com/json) - High-performance JSON framework for .NET
## Compatibility
|.NET version|C# version|
|------------|----------|
|  Core 3.1  |   8.0    |

The project is kept up to date with the latest versioning of the frameworks and libraries used.

I recommend to use httprepl utility for testing and using the api directly in the terminal.

Also the terminal can be used for connection string and password and redirection to Swagger.

## Copyright
We do not want anyone to see our source code, 
but if for any reason our code is stolen or otherwise obtained, 
we want to have a license that does not allow disclosure of any kind.
