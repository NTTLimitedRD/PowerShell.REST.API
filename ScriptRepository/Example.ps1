param ($upn)

Get-TfsServer -Name "https://tfs.gmgmt.dimensiondata.com:8443/tfs" | Select-Object Name | ConvertTo-Json -Compress