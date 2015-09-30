param ( 
	$message
	)

Sleep -s 10
Get-Item -Path . | Select-Object name | ConvertTo-Json -Compress