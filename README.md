AspNet.Identity.Mongo
=====================

A mongodb provider for the new ASP.NET Identity framework. My aim is to ensure this project is well tested and configurable.

## Usage

	var client = new MongoClient("mongodb://localhost:27017");
	var database = client.GetServer().GetDatabase("mydb");
	var users = database.GetCollection<IdentityUser>("users");

	var context = new IdentityContext(users);
	var store = new UserStore<IdentityUser>(context);
	var manager = new UserManager<IdentityUser>(store);

## Installation

via nuget:

	Install-Package AspNet.Identity.MongoDB -Pre

## Building and Testing

I'm using the albacore project with rake.

To build:

	rake msbuild
	
To test:

	rake tests
	rake integration_tests

To package:
	
	rake package

## Documentation

I'm writing about my design decisions on my blog:

- [Building a mongodb provider for the new ASP.NET Identity framework - Part 1](http://devblog.wesmcclure.com/posts/building-a-mongodb-provider-for-the-new-asp.net-identity-framework-part-1)
