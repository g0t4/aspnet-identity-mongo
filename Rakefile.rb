require 'albacore'

msbuild :msbuild  do |msb|
  msb.solution = 'src/AspNet.Identity.MongoDB.sln'
  msb.properties = { :configuration => :Release }
  msb.targets = [ :Clean, :Build ]
end

nunit :tests => [:msbuild] do |tests|
  tests.command = 'src/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe'
  tests.assemblies = ['build/tests/Tests.dll']
end

nunit :integration_tests => [:msbuild]  do |tests|
  tests.command = 'src/packages/NUnit.Runners.2.6.3/tools/nunit-console.exe'
  tests.assemblies = ['build/integrationTests/IntegrationTests.dll']
end

task :all_tests => [:tests, :integration_tests]

task :package => [:all_tests] do
	sh 'src/.nuget/nuget.exe pack src/AspNet.Identity.MongoDB/AspNet.Identity.MongoDB.csproj'
end