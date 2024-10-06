echo Please close visual studio. If it is not closed, it will cause issues.
dotnet nuget locals all --clear
mkdir release
dotnet build common\SimpleSourceGenerators\SimpleSourceGenerators.csproj -o release -c release
