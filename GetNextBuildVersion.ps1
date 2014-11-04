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

If ($VersionManagementApiUrl.AbsoluteUri -notlike '*/')
{
    Throw 'Invalid base URI for the build-versioning API (must have a trailing '/').'
}

Add-Type -LiteralPath .\Common\Tasks\LibGit2Sharp.dll

$workingDirectory = Resolve-Path '.'
If (![LibGit2Sharp.Repository]::IsValid($workingDirectory))
{
    Throw "Current directory is not a Git repository"
}

Try
{
    [LibGit2Sharp.Repository] $repository = New-Object LibGit2Sharp.Repository $workingDirectory
    $commitId = $repository.Head.Tip.Sha
}
Finally
{
    If ($repository)
    {
        $repository.Dispose()
    }
}

Write-Host "Requesting version for release '$ReleaseName' of product '$ProductName' (commit '$commitId')..."

$webClient = New-Object System.Net.WebClient
Try
{
    # TODO: Escape product and release names.

    $targetUrl = "$($VersionManagementApiUrl.AbsoluteUri)api/v1/product/$($ProductName)/release/$($ReleaseName)/version/$commitId"

    $webClient.UseDefaultCredentials = $true;
    $version = $webClient.UploadString(
        $targetUrl,
        '' # POST body
    )

    Write-Host "Version is '$version'"

    Return $version
}
Catch
{
    $exception = [System.Exception] $_.Exception
    $webException = $null
    While ($exception)
    {
        If ($exception -is [System.Net.WebException])
        {
            $webException = [System.Net.WebException] $exception

            Break
        }

        $exception = $exception.InnerException
    }

    If (!$webException -or !$webException.Response.ContentLength) # There may not be a response body.
    {
        Throw $_ # No, Powershell doesn't retain the original exception context either way
    }

    Try
    {
        $responseStream = $webException.Response.GetResponseStream()
        $responseReader = New-Object System.IO.StreamReader $responseStream

        $responseText = $responseReader.ReadToEnd()
        
        Write-Error "Error calling version management API: $responseText"
    }
    Catch
    {
        Write-Error $webException
    }
    Finally
    {
        If ($responseStream)
        {
            $responseStream.Dispose()
        }
    }

}
Finally
{
    If ($webClient)
    {
        $webClient.Dispose()
    }
}
