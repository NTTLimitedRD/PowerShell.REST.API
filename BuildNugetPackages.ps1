param (
    [string] $PackageVersion = '1.0.7.0-develop',
    [string] $PackageDropFolder = 'c:\temp\'
) 

# Update all version info

.\.NuGet\NuGet.exe pack '.\DynamicPowerShellApi.Host\PowerShellAPI.Host.nuspec' -NonInteractive -Verbosity Detailed -NoPackageAnalysis -Version "$PackageVersion"

Write-Host "Copying built packages to drop location '$packageDropFolder'..."
Get-ChildItem -Path . -Filter '*.nupkg' | Copy-Item -Destination $packageDropFolder