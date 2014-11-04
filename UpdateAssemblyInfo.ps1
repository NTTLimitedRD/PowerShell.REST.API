Param(
    [ValidateNotNullOrEmpty()]
    [Parameter(Mandatory = $true)]
    [String] $ProductName,

    [ValidateNotNullOrEmpty()]
    [Parameter(Mandatory = $true)]
    [String] $ReleaseName,

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

Function UpdateAssemblyInfoWithBuildNumber([string] $solutionAssemblyInfoFile)
{
    $solutionAssemblyInfo = Get-Content $solutionAssemblyInfoFile
    $updatedVersionInfo = $solutionAssemblyInfo -replace "Version\(`"\d+\.\d+\.\d+\.\d+`"\)\]`$", "Version(`"$MajorVersion.$MinorVersion.$BuildNumber.$RevisionNumber`")]"
    $updatedVersionInfo = $updatedVersionInfo -replace "AssemblyConfiguration\(`".*`"\)\]`$", "AssemblyConfiguration(`"$SpecialVersion`")]"

    $updatedVersionInfo | Out-File $solutionAssemblyInfoFile
}

Function UpdateMergeModuleScriptWithBuildNumber([string] $wixScriptFile)
{
    $wixScript = New-Object -TypeName 'System.Xml.XmlDocument'
    $wixScript.PreserveWhitespace = $true
    $wixScript.Load($wixScriptFile)

    $wixScript.Wix.Module.Version = $wixScript.Wix.Module.Version -replace "\d+\.\d+\.\d+", "$MajorVersion.$MinorVersion.$BuildNumber"

    $wixScript.Save($wixScriptFile)
}

Function UpdateSetupScriptWithBuildNumber([string] $wixScriptFile)
{
    $wixScript = New-Object -TypeName 'System.Xml.XmlDocument'
    $wixScript.PreserveWhitespace = $true
    $wixScript.Load($wixScriptFile)

    $wixScript.Wix.Product.Version = $wixScript.Wix.Product.Version -replace "\d+\.\d+\.\d+", "$MajorVersion.$MinorVersion.$BuildNumber"

    $wixScript.Save($wixScriptFile)
}

Function UpdateSqlProjectWithBuildNumber([string] $sqlProjectFile)
{
    $sqlProject = Get-Content $sqlProjectFile
    $sqlProject = $sqlProject -replace "\<DacVersion\>\d+\.\d+\.\d+\.\d+\<\/DacVersion\>", "<DacVersion>$MajorVersion.$MinorVersion.$BuildNumber.$RevisionNumber</DacVersion>"

    Set-Content $sqlProjectFile -Value $sqlProject
}

Function UpdateNuSpecWithPackageVersion([string] $nuSpecFile)
{
    Write-Host "Updating version info to '$PackageVersion' for package specification '$nuSpecFile'..."
	$nuSpec = Get-Content $nuSpecFile
    $nuSpec = $nuSpec -replace '\$version\$', "$PackageVersion"

    Set-Content $nuSpecFile -Value $nuSpec
}

$currentDir = (Get-Location).Path
UpdateAssemblyInfoWithBuildNumber(Join-Path $currentDir "SolutionAssemblyInfo.cs")

