## Microsoft.AspNetCore.Identity.MongoDB

This is a MongoDB provider for the ASP.NET Core Identity framework. This was ported from the v2 Identity framework that was a part of ASP.NET (AspNet.Identity.Mongo NuGet package)

I've released a new package for the ASP.NET Core Identity framework for the following reasons:
- Discoverability - named AspNetCore.
- ASP.NET Core is a rewrite of ASP.NET, this Core Identity framework won't run on traditional ASP.NET.
- Migrating isn't a matter of updating dependencies.

This project has extensive test coverage. 

If you want something easy to setup, this adapter is for you. I do not intend to cover every possible desirable configuration, if you don't like my decisions, write your own adapter. Use this as a learning tool to make your own adapter. These adapters are not complicated, but trying to make them configurable would become a complicated mess. And would confuse the majority of people that want something simple to use. So I'm favoring simplicity over making every last person happy.

## Usage

- Reference this package in project.json: Microsoft.AspNetCore.Identity.MongoDB
- Then, in ConfigureServices--or wherever you are registering services--include the following to register both the Identity services and MongoDB stores:

```csharp
services.AddIdentityWithMongoStores("mongodb://localhost/myDB");
```

- If you want to customize what is registered, refer to the tests for further options (CoreTests/MongoIdentityBuilderExtensionsTests.cs)
- Remember with the Identity framework, the whole point is that both a `UserManager` and `RoleManager` are provided for you to use, here's how you can resolve instances manually. Of course, constructor injection is also available.

```csharp
var userManager = provider.GetService<UserManager<IdentityUser>>();
var roleManager = provider.GetService<RoleManager<IdentityRole>>();
```

- The following methods help create indexes that will boost lookups by UserName, Email and role Name. These have changed since Identity v2 to refer to Normalized fields. I dislike this aspect of Core Identity, but it is what it is. Basically these three fields are stored in uppercase format for case insensitive searches.

```csharp
	IndexChecks.EnsureUniqueIndexOnNormalizedUserName(users);
	IndexChecks.EnsureUniqueIndexOnNormalizedEmail(users);
	IndexChecks.EnsureUniqueIndexOnNormalizedRoleName(roles);
```

- Here is a sample project, review the commit log for the steps taken to port the default template from EntityFramework MSSQL to MongoDB. [aspnet-identity-mongo-sample](https://github.com/g0t4/aspnet-identity-mongo-sample).

What frameworks are targeted, with rationale:

- Microsoft.AspNetCore.Identity - supports net451 and netstandard1.3
- MongoDB.Driver v2.3 - supports net45 and netstandard1.5
- Thus, the lowest common denominators are net451 (of net45 and net451) and netstandard1.5 (of netstandard1.3 and netstandard1.5) 
- FYI net451 supports netstandard1.2, that's obviously too low for a single target

## Building instructions

run commands in [](build.sh)

## Migrating from ASP.NET Identity 2.0

- Roles names need to be normalized as follows
	- On IdentityRole documents, create a NormalizedName field = uppercase(Name). Leave Name as is.
	- On IdentityUser documents, convert the values in the Roles array to uppercase
- User names need to be normalized as follows
	- On IdentityUser documents, create a NormalizedUserName field = uppercase(UserName) and create a NormalizedEmail field = uppercase(Email). Leave UserName and Email as is.