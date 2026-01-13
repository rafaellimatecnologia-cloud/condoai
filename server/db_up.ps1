param(
    [string]$ProjectPath = "$PSScriptRoot/CondoAI.Server.csproj"
)

dotnet tool restore

dotnet ef database update --project $ProjectPath
