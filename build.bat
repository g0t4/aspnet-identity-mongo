REM optional clean
REM rm -rf artifacts CoreTests/bin CoreTests/obj CoreIntegrationTests/bin CoreIntegrationTests/obj Microsoft.AspNetCore.Identity.MongoDB/bin Microsoft.AspNetCore.Identity.MongoDB/obj

dotnet restore src
dotnet test -c Release src/CoreTests
dotnet test -c Release src/CoreIntegrationTests
dotnet pack -c Release -o artifacts src/Microsoft.AspNetCore.Identity.MongoDB

REM nuget add artifacts\X.nupkg -Source C:\Code\scratch\localnugetfeedtesting
REM nuget publish artifacts\X.nupkg