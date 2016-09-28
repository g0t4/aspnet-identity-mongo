// clean
rm -rf CoreTests/bin CoreTests/obj CoreIntegrationTests/bin CoreIntegrationTests/obj Microsoft.AspNetCore.Identity.MongoDB/bin Microsoft.AspNetCore.Identity.MongoDB/obj

// restore nuget package dependencies
dotnet restore

// run tree command to see existing directory structure, not much

// build all matched projects via project.json
// should be 6 projects in total (3 project * 2 frameworks each)
dotnet build -c Release */project.json
// debug build with -c Debug

// run tree command, should see 6 new folders under 3 bin/Release/framework

// run tests (each will run for both frameworks targeted)
dotnet test -c Release CoreTests
dotnet test -c Release CoreIntegrationTests

// create NuGet package
dotnet pack -c Release Microsoft.AspNetCore.Identity.MongoDB

nuget add Microsoft.AspNetCore.Identity.MongoDB\bin\Release\Microsoft.AspNetCore.Identity.MongoDB.1.0.0.nupkg -Source C:\Code\scratch\localnugetfeedtesting
