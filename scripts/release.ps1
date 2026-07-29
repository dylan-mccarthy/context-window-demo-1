param(
    [Parameter(Mandatory)]
    [string] $Version
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$outputPath = Join-Path $repositoryRoot "artifacts/payments-api-$Version"

dotnet test (Join-Path $repositoryRoot 'Payments.sln') --configuration Release
dotnet publish (Join-Path $repositoryRoot 'src/Payments.Api/Payments.Api.csproj') `
    --configuration Release `
    --output $outputPath `
    -p:Version=$Version

Write-Host "Release package created at $outputPath"
