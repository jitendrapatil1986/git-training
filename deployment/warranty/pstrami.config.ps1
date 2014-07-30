Environment "dev" -servers @(
	Server "wkcorpappdev1" @("WarrantyWeb";) 
	Server "wksql3" @("Database";) 
	) -installPath "C:\Installs\WarrantyTest"
	
Environment "training" -servers @(
	Server "wkcorpappdev1" @("WarrantyWeb";) 
	Server "wksql3" @("Database";) 
	) -installPath "C:\Installs\WarrantyTraining"
	
Environment "prod" -servers @(
	Server "wkcorpappdev1" @("WarrantyWeb";) 
	Server "wksql3" @("Database";) 
	) -installPath "C:\Installs\Warranty"


if(Test-Path .\environments.ps1) {
	. .\environments.ps1
}

Role "Nsb" -Incremental {
	. load-vars "nsb\Warranty"

	# backup existing site
	backup-directory $nsb_directory

	#Stop NSB Service
	Stop-Service "WolfpackAgent"
	Stop-Service "$nsb_service_name"

	#Uninstall NSB Service
	&"$nsb_directory\NServiceBus.Host.exe" "/uninstall" "/serviceName:$nsb_service_name"

	# copy new files
	sync-files $source_dir $nsb_directory
	Remove-Item "$nsb_directory\mscorlib.dll"

	# config
	poke-xml "$nsb_directory\Warranty.Server.dll.config" "configuration/connectionStrings/add[@name='WarrantyDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=$db_nsb_application_name;"
	poke-xml "$nsb_directory\Warranty.Server.dll.config" "configuration/appSettings/add[@key='WarrantyDbName']/@value" $db_name
	poke-xml "$nsb_directory\Warranty.Server.dll.config" "configuration/appSettings/add[@key='DataBusSharePath']/@value" $dataBusSharePath
	poke-xml "$nsb_directory\Warranty.Server.dll.config" "configuration/appSettings/add[@key='DocumentSharePath']/@value" $documentSharePath
	poke-xml "$nsb_directory\Warranty.Server.dll.config" "configuration/appSettings/add[@key='ActionMailerPickupDirectory']/@value" $actionMailerPickupDirectory
		
	#Install NSB Service
	&"$nsb_directory\NServiceBus.Host.exe" "/install" "/serviceName:$nsb_service_name" "/username:dwh\svc-Warranty-nsb" "/password:O2I(&3J,5`$V1h24"

	#Start NSB Service
	Start-Service "$nsb_service_name"
	Start-Service "WolfpackAgent"

} -FullInstall {
	# You could do IIS setup here... but it is much easier to just do that with server provisioning
}

Role "WarrantyWeb" -Incremental {
	. load-vars "web\Warranty"

	Set-AppOffline $warranty_web_directory

	# backup existing site
	backup-directory $warranty_web_directory

	# copy new files
	$skips = @(
		@{"objectName"="filePath";"skipAction"="Delete";"absolutePath"='App_Offline\.htm$'}
	)
	sync-files $source_dir $warranty_web_directory $skips

	# config
	poke-xml "$warranty_web_directory\web.config" "configuration/connectionStrings/add[@name='WarrantyDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=$db_p_ui_application_name;"
	poke-xml "$warranty_web_directory\web.config" "configuration/system.identityModel/identityConfiguration/audienceUris/add/@value" $warranty_identity_uri
	poke-xml "$warranty_web_directory\web.config" "configuration/system.identityModel.services/federationConfiguration/wsFederation/@realm" $warranty_identity_uri
	poke-xml "$warranty_web_directory\web.config" "configuration/appSettings/add[@key='sendFeedbackAddresses']/@value" $sendFeedbackAddresses
	poke-xml "$warranty_web_directory\web.config" "configuration/appSettings/add[@key='DocumentSharePath']/@value" $documentSharePath
	poke-xml "$warranty_web_directory\web.config" "configuration/appSettings/add[@key='DataBusSharePath']/@value" $dataBusSharePath
	poke-xml "$warranty_web_directory\web.config" "configuration/appSettings/add[@key='ActionMailerPickupDirectory']/@value" $actionMailerPickupDirectory
	
	poke-xml "$warranty_web_directory\web.config" "configuration/elmah/errorMail/@to" $errorReportingEmailAddresses
	poke-xml "$warranty_web_directory\web.config" "configuration/elmah/errorMail/@subject" $errorReportingSubject
	poke-xml "$warranty_web_directory\web.config" "configuration/elmah/errorMail/@smtpServer" $smtpServer
	
	poke-xml "$warranty_web_directory\web.config" "configuration/system.net/mailSettings/smtp/@deliveryMethod" $smtpDeliveryMethod

	Rename-Header-File $warranty_web_directory $header_image_file_name

	Set-AppOnline $warranty_web_directory
} -FullInstall {
	# You could do IIS setup here... but it is much easier to just do that with server provisioning
}

	. load-vars "web\Vendor"

	Set-AppOffline $vendor_web_directory

	# backup existing site
	backup-directory $vendor_web_directory

	# copy new files
	$skips = @(
		@{"objectName"="filePath";"skipAction"="Delete";"absolutePath"='App_Offline\.htm$'}
	)
	sync-files $source_dir $vendor_web_directory $skips

	# config
	poke-xml "$vendor_web_directory\web.config" "configuration/connectionStrings/add[@name='WarrantyDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=$db_v_ui_application_name;"
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='BaseAuthorityUrl']/@value" $vendor_auth_base_url
	
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='ida:FederationMetadataLocation']/@value" $vendor_auth_metadata
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='ida:Issuer']/@value" $vendor_auth_issuer

	poke-xml "$vendor_web_directory\web.config" "configuration/system.identityModel/identityConfiguration/audienceUris/add/@value" $vendor_identity_uri
	poke-xml "$vendor_web_directory\web.config" "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/authority/@name" $vendor_auth_authority
	poke-xml "$vendor_web_directory\web.config" "configuration/system.identityModel/identityConfiguration/issuerNameRegistry/authority/validIssuers/add/@name" $vendor_auth_authority

	poke-xml "$vendor_web_directory\web.config" "configuration/system.identityModel.services/federationConfiguration/wsFederation/@realm" $vendor_identity_uri
	poke-xml "$vendor_web_directory\web.config" "configuration/system.identityModel.services/federationConfiguration/wsFederation/@reply" $vendor_identity_uri
	poke-xml "$vendor_web_directory\web.config" "configuration/system.identityModel.services/federationConfiguration/wsFederation/@issuer" $vendor_auth_issuer
	
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='sendFeedbackAddresses']/@value" $sendFeedbackAddresses
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='DocumentSharePath']/@value" $documentSharePath
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='DataBusSharePath']/@value" $dataBusSharePath
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='ActionMailerPickupDirectory']/@value" $actionMailerPickupDirectory
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='NewRelic.AppName']/@value" $newRelicVendorName
	poke-xml "$vendor_web_directory\web.config" "configuration/appSettings/add[@key='Accounting.API.BaseUri']/@value" $accounting_api_uri
	
	poke-xml "$vendor_web_directory\web.config" "configuration/elmah/errorMail/@to" $errorReportingEmailAddresses
	poke-xml "$vendor_web_directory\web.config" "configuration/elmah/errorMail/@subject" $errorReportingSubject
	poke-xml "$vendor_web_directory\web.config" "configuration/elmah/errorMail/@smtpServer" $smtpServer
	
	poke-xml "$vendor_web_directory\web.config" "configuration/system.net/mailSettings/smtp/@deliveryMethod" $smtpDeliveryMethod
	
	Rename-Header-File $vendor_web_directory $header_image_file_name	
	
	$taskName = "KeepVendorPortalActive"
	$task = Get-ScheduledTask -TaskName $taskName -TaskPath \

	$A = New-ScheduledTaskAction -Execute "$keepAliveDirectory\KeepVendorPortalActive.bat" -Argument $vendor_identity_uri
	$T = New-ScheduledTaskTrigger -Once -At 7am -RepetitionDuration  (New-TimeSpan -Days 1)  -RepetitionInterval  (New-TimeSpan -Minutes 15)
	$D = New-ScheduledTask -Action $A -Trigger $T
	Register-ScheduledTask $taskName -InputObject $D

	
	Set-AppOnline $vendor_web_directory
} -FullInstall {
	# You could do IIS setup here... but it is much easier to just do that with server provisioning
}

Role "Database" -Incremental {
	run-dbtask "Update" "database\Warranty" "db_server" "db_name"
} -FullInstall {
	# You could do a database rebuild here, but why, just restore a production backup first and then do an update
}

function script:load-vars($relativeSourceDir) {
	new-variable -name base_dir -value (resolve-path .) -Option Constant
	new-variable -name source_dir -value "$base_dir\$relativeSourceDir" -Option Constant

	. "$base_dir\vars.ps1"

	if(test-path "$base_dir\$global:env.vars.ps1") {
		write-host "Loading $base_dir\$global:env.vars.ps1"
		. "$base_dir\$global:env.vars.ps1"
	}
	
	$error_msg = "ERROR: Missing variable {0}!!"
	$invalid_path_msg = "ERROR: Invalid path for variable {0}!!"
	
	check-pathvar $warranty_web_directory '$warranty_web_directory'
	check-pathvar $nsb_directory '$nsb_directory'
	check-var $db_server = "$db_server"
}

function script:check-var($var,$var_name) {
	$error_msg = "ERROR: Missing variable {0}!!"
	Assert ($var -ne $null -and $var -ne "") ($error_msg -f $var_name)
}

function script:check-pathvar($var,$var_name) {
	check-var $var $var_name
	$invalid_path_msg = "ERROR: Invalid path for variable {0}!!"
	Assert (test-path $var -IsValid) ($invalid_path_msg -f $var_name)
}

function script:run-dbtask($task,$relativeSourceDir,$server_var,$db_var) {
	. load-vars $relativeSourceDir
	if ($task -eq "Update"){
		&$roundhouse_exe_path -s (get-variable -name $server_var).Value -d (get-variable -name $db_var).Value -f $source_dir -vf $roundhouse_version_file --silent -o $roundhouse_output_dir --ct 300
	}
}

function script:Rename-Header-File($directory, $filename){
	Move-Item "$directory\img\$filename" "$directory\img\DWH logo_color_233x34.png" -force
}

function script:Set-AppOffline($destination) {
	Copy-Item -ErrorAction SilentlyContinue -Force "$destination\__app_offline.htm"  "$destination\app_offline.htm"
}

function script:Set-AppOnline($destination) {
	Remove-Item -ErrorAction SilentlyContinue -Force "$destination\app_offline.htm"
}

function script:copy-files($source,$destination,$exclude=@()) {
	Copy-Item $source -Destination $destination -Exclude $exclude -recurse -force
}

function script:sync-files($source,$destination,$skip=@()) {
	Write-Host "Syncing files between '$source' and '$destination'"
	$msdeployexe = Get-MSWebDeployInstallPath

	$arguments = @("-verb:sync";"-source:dirPath=$source";"-dest:dirPath=$destination")
	$skip | %{ 
		$arg = "-skip:"
		$_.GetEnumerator() | %{ $arg += $_.Key + "=" + $_.Value + "," }
		$arguments += $arg.trimend(",")
	}
	
	Write-Host "sync-files arguments: $arguments"
	
	&"$msdeployexe" $arguments | out-null
}

function Get-MSWebDeployInstallPath(){
    return (get-childitem "HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy" | Select -last 1).GetValue("InstallPath_x86") + "MSdeploy.exe"
}

function script:create-directory($directory_name) {
	if(-not (test-path $directory_name)) {
		mkdir $directory_name
	} else {
		write-host ("WARNING: Cannot create directory {0} because it already exists!" -f $directory_name)
	}
}

function script:backup-directory($directory_name) {
	if(-not (test-Path $directory_name)) {
		write-host ("WARNING: Cannot backup directory {0} because it does not exist!" -f $directory_name)
		return $null
	}

	$backup_dir = $directory_name + "_bak"
	sync-files $directory_name $backup_dir
}

function script:peek-xml($filePath, $xpath, $namespaces = @{}) {
	[xml] $fileXml = Get-Content $filePath

	if($namespaces -ne $null -and $namespaces.Count -gt 0) {
		$ns = New-Object Xml.XmlNamespaceManager $fileXml.NameTable
		$namespaces.GetEnumerator() | %{ [Void]$ns.AddNamespace($_.Key,$_.Value) }
		$node = $fileXml.SelectSingleNode($xpath,$ns)
	} else {
		$node = $fileXml.SelectSingleNode($xpath)
	}

	Assert ($node -ne $null) "could not find node @ $xpath"

	if($node.NodeType -eq "Element") {
		return $node.InnerText
	} else {
		return $node.Value
	}
}

function script:poke-xml($filePath, $xpath, $value, $namespaces = @{}) {
	[xml] $fileXml = Get-Content $filePath

	if($namespaces -ne $null -and $namespaces.Count -gt 0) {
		$ns = New-Object Xml.XmlNamespaceManager $fileXml.NameTable
		$namespaces.GetEnumerator() | %{ [Void]$ns.AddNamespace($_.Key,$_.Value) }
		$node = $fileXml.SelectSingleNode($xpath,$ns)
	} else {
		$node = $fileXml.SelectSingleNode($xpath)
	}

	Assert ($node -ne $null) "could not find node @ $xpath"

	if($node.NodeType -eq "Element") {
		$node.InnerText = $value
	} else {
		$node.Value = $value
	}

	[Void]$fileXml.Save($filePath)
}
