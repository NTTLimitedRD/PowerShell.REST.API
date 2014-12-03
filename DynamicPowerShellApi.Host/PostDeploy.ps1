$path = (Get-Item -Path ".\" -Verbose)
$manifest = [xml] (gc (Join-Path -Path $path -ChildPath ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.man))
$manifest.instrumentationManifest.instrumentation.events.provider
$manifestDll = Join-Path -Path $path -ChildPath "ETW\DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.dll"
$manifestDll
$manifest.instrumentationManifest.instrumentation.events.provider.SetAttribute("resourceFileName",$manifestDll)
$manifest.instrumentationManifest.instrumentation.events.provider.SetAttribute("messageFileName", $manifestDll)
$manifest.Save((Join-Path -Path $path -ChildPath ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.man))
& wevtutil.exe um ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.man
& wevtutil.exe im ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.man /rf:"ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.dll" /mf:"ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.dll" /pf:"ETW/DynamicPowerShellApi.DDCloud-DynamicPowershellApi.etwManifest.dll"