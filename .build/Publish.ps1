
cd "$PSScriptRoot/.."

$nugets = $( Get-ChildItem ./output -Filter *.nupkg)

foreach ($nuget in $nugets)
{
    dotnet nuget push $nuget --source https://api.nuget.org/v3/index.json  --api-key $ENV:NUGET_API_KEY
}