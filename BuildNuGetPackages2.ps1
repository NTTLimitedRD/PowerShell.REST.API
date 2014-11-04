Param(
    [ValidateNotNullOrEmpty()]
    [Parameter(Mandatory = $true)]
    [String] $ProductName,

    [ValidateNotNullOrEmpty()]
    [Parameter(Mandatory = $true)]
    [String] $ReleaseName,

    [Parameter(Mandatory = $false)]
    [Switch] $Publish,

    [ValidateNotNull()]
    [Parameter(Mandatory = $false)]
    [Uri] $VersionManagementApiUrl = 'https://tfs.gmgmt.dimensiondata.com:8443/version-management/'
)

$semanticVersion = .\GetNextBuildVersion.ps1 -ProductName $ProductName -ReleaseName $ReleaseName -VersionManagementApiUrl $VersionManagementApiUrl
If ($semanticVersion -match '(.*)-(.*)')
{
    $buildVersion = [Version]::Parse($Matches[1])
    $SpecialVersion = $Matches[2]
}
Else
{
    $buildVersion = [Version]::Parse($semanticVersion)
    $SpecialVersion = ''
}

$PackageVersion = $semanticVersion

If ($SpecialVersion)
{
    Write-Host "Building PRE-RELEASE packages (explicitly-specified package version = '$PackageVersion')"
}
Else
{
    Write-Host "Building RELEASE packages (package version = '$PackageVersion')"
}

$MajorVersion   = $buildVersion.Major
$MinorVersion   = $buildVersion.Minor
$BuildNumber    = $buildVersion.Build
$RevisionNumber = $buildVersion.Revision

# Prevent stupid NuGet warning.
Set-Item -Path 'Env:\EnableNuGetPackageRestore' -Value 'true'

If (Test-Path Env:\TF_BUILD_BINARIESDIRECTORY)
{
	$outputFolder = Get-Content Env:\TF_BUILD_BINARIESDIRECTORY
    $haveOutputFolder = $true

	Write-Host "Team build output folder is '$outputFolder'."
}
Else
{
	$outputFolder = $null
    $haveOutputFolder = $false

	Write-Warning 'Warning - TF_BUILD_BINARIESDIRECTORY environment variable is not defined. Packages that need to be built with the team build output folder as their base folder will be skipped.'
}

# Build up a list of package properties that specify dependency versions.
# $dependencyVersionXml = [xml](Get-Content '.\PackageDependencyVersions.xml')
# $packageDependencyVersions = @{ version = $PackageVersion }
# $dependencyVersionXml.packageDependencies.packageDependency | % { $packageDependencyVersions.Add("Version." + $_.name, $_.version) }

$nuGetPackagesFolder = '.\NuGetPackages'
If (-not (Test-Path $nuGetPackagesFolder))
{
    mkdir $nuGetPackagesFolder
}
cd $nuGetPackagesFolder

Function Build-Project([string]$ProjectFile, [string]$ProjectConfiguration = 'Release')
{
    If ([string]::IsNullOrWhiteSpace($ProjectFile))
    {
        Throw "Invalid project file (cannot be null, empty, or entirely composed of whitespace)."
    }

    Write-Host '================================================================================'
	Write-Host "Building database project '$ProjectFile'..."
    C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe "$ProjectFile" /t:Build /p:Configuration=$ProjectConfiguration /v:minimal
}

Function Update-NuSpecVersions([string]$NuSpecFile)
{
    If ([string]::IsNullOrWhiteSpace($NuSpecFile))
    {
        Throw "Invalid NuSpec file (cannot be null, empty, or entirely composed of whitespace)."
    }

    Write-Host "Updating version info for package specification '$NuSpecFile'..."
    $packageSpecification = [xml](Get-Content $NuSpecFile)
    ForEach ($packageDependency In $packageSpecification.package.metadata.dependencies.group.dependency)
    {
        If ($packageDependency.version -notmatch '\$(.*)\$')
        {
            Write-Host "'$($packageDependency.id)' = '$($packageDependency.version)'"

            Continue
        }

        $dependencyName = $Matches[1]
        If (!$dependencyName)
        {
            Throw "Failed to match dependency name in package dependency version '$($packageDependency.version)'."
        }

        $dependencyVersion = $packageDependencyVersions[$dependencyName];
        If (!$dependencyVersion)
        {
            Throw "Failed to find a matching dependency version named '$dependencyName'."
        }

        Write-Host "Updating dependency version named '$dependencyName' to '$dependencyVersion' for dependency '$($packageDependency.id)' in package specification '$NuSpecFile'..."
        $packageDependency.version = $dependencyVersion
    }

    $packageSpecification.Save($NuSpecFile)
}

Function Build-PackageFromNuSpec([string]$NuSpecFile, [string] $BaseDirectory = $null)
{
    If ([string]::IsNullOrWhiteSpace($NuSpecFile))
    {
        Throw "Invalid NuSpec file (cannot be null, empty, or entirely composed of whitespace)."
    }

    If (-not [string]::IsNullOrWhiteSpace($BaseDirectory))
    {
        Write-Host "Building package '$NuSpecFile' using base directory '$BaseDirectory'..."
        ..\Common\BuildSupport\NuGet\NuGet.exe pack "$NuSpecFile" -BasePath "$BaseDirectory" -NonInteractive -Verbosity Detailed -NoPackageAnalysis -Version "$PackageVersion"
    }
    Else
    {
        Write-Host "Building package '$NuSpecFile'..."
        ..\Common\BuildSupport\NuGet\NuGet.exe pack "$NuSpecFile" -NonInteractive -Verbosity Detailed -NoPackageAnalysis -Version "$PackageVersion"
    }
}

Function Build-PackageFromProject([string]$PackageProjectFile, [string]$ProjectConfiguration = 'Release')
{
    If ([string]::IsNullOrWhiteSpace($PackageProjectFile))
    {
        Throw "Invalid package project file (cannot be null, empty, or entirely composed of whitespace)."
    }
    
    Write-Host '================================================================================'
    $projectFolder = Split-Path $PackageProjectFile -Parent
    Get-ChildItem (Join-Path $projectFolder '*.nuspec') -File -Recurse | ForEach-Object {
        Update-NuSpecVersions $_.FullName
    }

    Write-Host "Building NuGet package from project '$PackageProjectFile'..."
    ..\Common\BuildSupport\NuGet\NuGet.exe pack "$PackageProjectFile" -Build -Prop "Configuration=$ProjectConfiguration" -Prop Platform=AnyCPU -NonInteractive -Verbosity Detailed -NoPackageAnalysis -Version "$PackageVersion"
}

# Update all version info
# $nuSpecFiles = Dir "..\Portal\*.nuspec" -File -Recurse
# ForEach ($nuSpecFile in $nuSpecFiles)
# {
#    Update-NuSpecVersions -NuSpecFile $nuSpecFile
# }

# Build all packages
Build-PackageFromNuSpec -NuSpecFile '..\DynamicPowerShellApi.Host\PowerShellAPI.Host.nuspec'

# If ($haveOutputFolder)
# {
    # $publishedWebFolder = Join-Path $outputFolder 'CloudServicesPortal\_PublishedWebsites'

    # Portal web
    # Copy-Item '..\Portal\Deployment\PortalPackageScripts\ConfigurePortalWeb.ps1' -Destination $publishedWebFolder
    # Build-PackageFromNuSpec -NuSpecFile '..\Portal\PortalNuGetPackage\Aperture.Portal.WebSite.nuspec' -BaseDirectory $publishedWebFolder
    
    # Master design-time package
    # Build-PackageFromNuSpec -NuSpecFile '..\Portal\PortalNuGetPackage\Aperture.DeveloperPortal.nuspec'

    # ETW manifests (portal only)

	# $platformFolder = Join-Path $outputFolder 'Platform'
    # If (!(Test-Path $platformFolder -PathType Container))
    # {
        # New-Item $platformFolder -ItemType Directory
    # }

    # $etwFolder = Dir '..\packages\Aperture.Platform.Diagnostics.EtwReg.*' | Select -Last 1
	# If (!$etwFolder)
	# {
		# Throw "Cannot find package folder for 'Aperture.Platform.Diagnostics.EtwReg'."
	# }
    # Copy-Item "$etwFolder\tools\*.*" -Destination $platformFolder

    # $platformCoreEtwFolder = Dir '..\packages\Aperture.Platform.Core.EventSources.*' | Select -Last 1
	# If (!$platformCoreEtwFolder)
	# {
		# Throw "Cannot find package folder for 'Aperture.Platform.Core.EventSources'."
	# }
	# Copy-Item "$platformCoreEtwFolder\etw\*.*" -Destination $platformFolder

    # Build-PackageFromNuSpec -NuSpecFile '..\EventSourcePackage\Aperture.EventSources.Portal.nuspec' -BaseDirectory $outputFolder
# }
# Else
# {
    # Write-Warning 'Skipped build of ETW, STS web site, and portal web site packages (unable to determine team build output folder).'
# }

cd ..

# If ($haveOutputFolder)
# {
    # $packageDropFolder = Join-Path $outputFolder 'NuGetPackages'
    # If (-not (Test-Path $packageDropFolder -PathType Container))
    # {
        # New-Item $packageDropFolder -ItemType Directory
    # }

    # Write-Host "Copying built packages to drop location '$packageDropFolder'..."
    # Get-ChildItem -Path $nuGetPackagesFolder -Filter '*.nupkg' | Copy-Item -Destination $packageDropFolder
# }
# Else
# {
    # Write-Warning 'Not copying built packages to drop location because the drop location cannot be determined.'
# }

If ($Publish)
{
	If (!$haveOutputFolder)
	{
		Throw 'Cannot publish packages because some were not built (unable to determine team build output folder).'
	}

    Get-ChildItem -Path $nuGetPackagesFolder -Filter '*.nupkg' | ForEach-Object {
		.\Common\BuildSupport\NuGet\NuGet.exe Push "$($_.FullName)" -Source 'CSFMAperture' -ApiKey 'ThisIsNotUsedBecauseProGetUsesWindowsAuth' -ConfigFile '.\Common\BuildSupport\NuGet\NuGet.config' -NonInteractive
	}
}
