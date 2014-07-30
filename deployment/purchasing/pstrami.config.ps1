Environment "dev" -servers @(
	Server "wkcorpappdev1" @("PurchasingWeb";) 
	Server "wkcorpappdev1" @("VendorWeb";) 
	Server "wksql3" @("PurchasingEtlJde";)
	Server "wksql3" @("PurchasingEtlPO";)
	Server "wksql3" @("PurchasingNotifier";)
	Server "wkcorpappdev1" @("Nsb";)
	Server "wksql3" @("Database";) 
	) -installPath "C:\Installs\PurchasingTest"
	
Environment "training" -servers @(
	Server "wkcorpapptrain1" @("PurchasingWeb";) 
	Server "wkcorpapptrain1" @("VendorWeb";) 
	Server "wksql3" @("PurchasingEtlJde";)
	Server "wksql3" @("PurchasingEtlPO";)
	Server "wksql3" @("PurchasingNotifier";)
	Server "wkcorpapptrain1" @("Nsb";)
	Server "wksql3" @("Database";) 
	) -installPath "C:\Installs\PurchasingTraining"
	
Environment "prod" -servers @(
	Server "wkcorpappprod1" @("PurchasingWeb";) 
	Server "wkcorpappprod1" @("VendorWeb";) 
	Server "wkcorpappprod1" @("PurchasingEtlJde";)
	Server "wksql1" @("PurchasingEtlPO";)
	Server "wksql1" @("PurchasingNotifier";)
	Server "wkcorpappprod1" @("Nsb";)
	Server "wksql1" @("Database";) 
	) -installPath "C:\Installs\Purchasing"


if(Test-Path .\environments.ps1) {
	. .\environments.ps1
}

Role "Nsb" -Incremental {
	. load-vars "nsb\Purchasing"

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
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/connectionStrings/add[@name='PurchasingDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=$db_nsb_application_name;"
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='PurchasingDbName']/@value" $db_name
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='DataBusSharePath']/@value" $dataBusSharePath
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='DocumentSharePath']/@value" $documentSharePath
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='ActionMailerPickupDirectory']/@value" $actionMailerPickupDirectory
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='PaymentRequestShared']/@value" $paymentRequestsSharedDirectory
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='PurchaseOrderURL']/@value" $purchaseOrderUrl
	poke-xml "$nsb_directory\Purchasing.Server.dll.config" "configuration/appSettings/add[@key='EnableServiceControlHeartbeat']/@value" $nsb_enableServiceControlHeartbeat
		
	#Install NSB Service
	&"$nsb_directory\NServiceBus.Host.exe" "/install" "/serviceName:$nsb_service_name" "/username:dwh\svc-purchasing-nsb" "/password:O2I(&3J,5`$V1h24"

	#Start NSB Service
	Start-Service "$nsb_service_name"
	Start-Service "WolfpackAgent"

} -FullInstall {
	# You could do IIS setup here... but it is much easier to just do that with server provisioning
}

Role "PurchasingWeb" -Incremental {
	. load-vars "web\Purchasing"

	Set-AppOffline $purchasing_web_directory

	# backup existing site
	backup-directory $purchasing_web_directory

	# copy new files
	$skips = @(
		@{"objectName"="filePath";"skipAction"="Delete";"absolutePath"='App_Offline\.htm$'}
	)
	sync-files $source_dir $purchasing_web_directory $skips

	# config
	poke-xml "$purchasing_web_directory\web.config" "configuration/connectionStrings/add[@name='PurchasingDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=$db_p_ui_application_name;"
	poke-xml "$purchasing_web_directory\web.config" "configuration/system.identityModel/identityConfiguration/audienceUris/add/@value" $purchasing_identity_uri
	poke-xml "$purchasing_web_directory\web.config" "configuration/system.identityModel.services/federationConfiguration/wsFederation/@realm" $purchasing_identity_uri
	poke-xml "$purchasing_web_directory\web.config" "configuration/appSettings/add[@key='sendFeedbackAddresses']/@value" $sendFeedbackAddresses
	poke-xml "$purchasing_web_directory\web.config" "configuration/appSettings/add[@key='DocumentSharePath']/@value" $documentSharePath
	poke-xml "$purchasing_web_directory\web.config" "configuration/appSettings/add[@key='DataBusSharePath']/@value" $dataBusSharePath
	poke-xml "$purchasing_web_directory\web.config" "configuration/appSettings/add[@key='ActionMailerPickupDirectory']/@value" $actionMailerPickupDirectory
	poke-xml "$purchasing_web_directory\web.config" "configuration/appSettings/add[@key='NewRelic.AppName']/@value" $newRelicPurchasingName
	poke-xml "$purchasing_web_directory\web.config" "configuration/appSettings/add[@key='Accounting.API.BaseUri']/@value" $accounting_api_uri
	poke-xml "$purchasing_web_directory\web.config" "configuration/UnicastBusConfig/MessageEndpointMappings/add[@Messages='HelpDesk.Messages']/@Endpoint" $helpDeskMessagesEndPoint
	
	poke-xml "$purchasing_web_directory\web.config" "configuration/elmah/errorMail/@to" $errorReportingEmailAddresses
	poke-xml "$purchasing_web_directory\web.config" "configuration/elmah/errorMail/@subject" $errorReportingSubject
	poke-xml "$purchasing_web_directory\web.config" "configuration/elmah/errorMail/@smtpServer" $smtpServer
	
	poke-xml "$purchasing_web_directory\web.config" "configuration/system.net/mailSettings/smtp/@deliveryMethod" $smtpDeliveryMethod

	Rename-Header-File $purchasing_web_directory $header_image_file_name

	Set-AppOnline $purchasing_web_directory
} -FullInstall {
	# You could do IIS setup here... but it is much easier to just do that with server provisioning
}

Role "PurchasingEtlJde" -Incremental {
	. load-vars "tools\Purchasing.ETL.JDE"

	# backup existing site
	# backup-directory $etl_jde_directory

	# copy new files
	# $skips = @(
	#	@{"objectName"="filePath";"skipAction"="Delete";"absolutePath"='App_Offline\.htm$'}
	# )
	# sync-files $source_dir $etl_jde_directory $skips

	# config
	poke-xml "$etl_jde_directory\Purchasing.ETL.JDE.exe.config" "configuration/connectionStrings/add[@name='PurchasingDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=Purchasing.ETL.JDE;"
	poke-xml "$etl_jde_directory\Purchasing.ETL.JDE.exe.config" "configuration/connectionStrings/add[@name='JDE']/@connectionString" "DataSource=$jde_db_server;CharBitDataAsString=TRUE;UserID=$jde_db_username;Password=$jde_db_password;DefaultCollection=$jde_db_collection;"
	
	# Rename-Header-File $etl_jde_directory $header_image_file_name

	# Set-AppOnline $etl_jde_directory
} -FullInstall {
	# Everything else has a comment, why not this one
}

Role "PurchasingEtlPO" -Incremental {
	. load-vars "tools\Purchasing.ETL.JDE.PO"	
	# config
	poke-xml "$etl_jde_po_directory\purchasing.jde.etl.dtsConfig" "DTSConfiguration/Configuration[@Path='\Package.Connections[PurchasingSQL].Properties[ConnectionString]']/ConfiguredValue" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=Purchasing.ETL.JDE.PO;"
	poke-xml "$etl_jde_po_directory\purchasing.jde.etl.dtsConfig" "DTSConfiguration/Configuration[@Path='\Package.Connections[JDE].Properties[ConnectionString]']/ConfiguredValue" "Data Source=$jde_db_server;User ID=$jde_db_username;Initial Catalog=DWHOMES;Provider=IBMDA400.DataSource.1;Force Translate=0;Default Collection=$jde_db_collection;Password=$jde_db_password;"

} -FullInstall {
	# Everything else has a comment, why not this one
}

Role "PurchasingNotifier" -Incremental {
	. load-vars "tools\Purchasing.Notifier"

	# backup existing site
	# backup-directory $etl_jde_directory

	# copy new files
	# $skips = @(
	#	@{"objectName"="filePath";"skipAction"="Delete";"absolutePath"='App_Offline\.htm$'}
	# )
	# sync-files $source_dir $etl_jde_directory $skips

	# config
	poke-xml "$notifier_directory\Purchasing.Notifier.exe.config" "configuration/connectionStrings/add[@name='PurchasingDB']/@connectionString" "Server=$db_server;Initial Catalog=$db_name;Trusted_Connection=yes"
	poke-xml "$notifier_directory\Purchasing.Notifier.exe.config" "configuration/appSettings/add[@key='PurchaseOrderURL']/@value" $purchaseOrderUrl
	poke-xml "$notifier_directory\Purchasing.Notifier.exe.config" "configuration/appSettings/add[@key='SubjectPrefix']/@value" $notifyEmailSubjectPrefix

	# Rename-Header-File $etl_jde_directory $header_image_file_name

	# Set-AppOnline $etl_jde_directory
} -FullInstall {
	# Everything else has a comment, why not this one
}

Role "VendorWeb" -Incremental {
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
	poke-xml "$vendor_web_directory\web.config" "configuration/connectionStrings/add[@name='PurchasingDB']/@connectionString" "Data Source=$db_server;Initial Catalog=$db_name;Integrated Security=SSPI;Application Name=$db_v_ui_application_name;"
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
	run-dbtask "Update" "database\Purchasing" "db_server" "db_name"
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
	
	check-pathvar $purchasing_web_directory '$purchasing_web_directory'
	check-pathvar $vendor_web_directory '$vendor_web_directory'
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
