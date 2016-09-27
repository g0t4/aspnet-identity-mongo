## Microsoft.AspNetCore.Identity.MongoDB

This is a MongoDB provider for the ASP.NET Core Identity framework. This was ported from the v2 Identity framework that was a part of ASP.NET (AspNet.Identity.Mongo NuGet package)

I've released a new package for the ASP.NET Core Identity framework for the following reasons:
- Discoverability - named AspNetCore
- Continue to support the old package for ASP.NET (not Core)
- ASP.NET Core is a rewrite of ASP.NET, this Core Identity framework won't run on traditional ASP.NET 

This project has extensive test coverage. 

This point of this adapter is to provide a simple wrapper to work with MongoDB. I do not intende to cover every possible desirable configuration, if you don't like my decisions, write your own adapter. These adapters are not complicated, but trying to make them configurable would become a complicated mess and would confuse the majority of people that want something simple to use, so I'm favoring simplicity over making every last person happy.

## Usage

- `Install-Package Microsoft.AspNetCore.Identity.MongoDB`
- Then, in ConfigureServices--or wherever you are registering services--include the following to register both the Identity services and MongoDB stores:

```csharp
services.AddIdentityWithMongoStores("mongodb://localhost/myDB");
```

- If you want to customize what is registered, refer to the tests for further options (CoreTests/MongoIdentityBuilderExtensionsTests.cs)
- Remember with the Identity framework, the whole point is that both a `UserManager` and `RoleManager` are provided for you to use, here's how you can resolve instances:

```csharp
var userManager = provider.GetService<UserManager<IdentityUser>>();
var roleManager = provider.GetService<RoleManager<IdentityRole>>();
```

- The following methods help create indexes that will boost lookups by UserName, Email and role Name, by the way these have changed since Identity v2 to refer to Normalized fields. I dislike this aspect of Core Identity, but it is what it is. Basically these three fields are stored in uppercase format for case insensitive searches.

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

## Migrating from ASP.NET Identity 2.0


- roles names need to be normalized (user.roles)
	- Default uppercase - tell people if they customize this they have to deal with custom migration

- normalization by uppercase:
	- add IdentityRole.NormalizedName
	- add IdentityUser.NormalizedUserName, IdentityUser.NormalizedEmail
- LockoutEndDateUtc - type changed in code, but I think it is still the same in db

- Should't cause a problem, but FYI, IdentityUserLogin.ProviderDisplayName, I believe this was a change to Identity v2
