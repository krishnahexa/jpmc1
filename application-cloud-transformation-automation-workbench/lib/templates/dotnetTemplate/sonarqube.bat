dotnet sonarscanner begin /k:"$namespace$" /d:sonar.host.url="http://localhost:9900" /d:sonar.login="17e588c0f596acf00f79f95478e10d2e4090be6b"
dotnet build $namespace$.sln
dotnet sonarscanner end /d:sonar.login="17e588c0f596acf00f79f95478e10d2e4090be6b"