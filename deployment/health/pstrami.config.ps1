Environment "dev" -servers @(
    Server "wkcorpappdev1" @("Nsb";)
    Server "wksql3" @("Database";)
    ) -installPath "C:\Installs\WarrantyHealthTest"
    
Environment "training" -servers @(
    Server "wkcorpapptrain1" @("Nsb";)
    Server "wksql3" @("Database";)
    ) -installPath "C:\Installs\WarrantyHealthTraining"
    
Environment "prod" -servers @(
    Server "wkcorpappprod2" @("Nsb";)
    Server "wksql1" @("Database";)
    ) -installPath "C:\Installs\WarrantyHealth"


if(Test-Path .\environments.ps1) {
    . .\environments.ps1
}

Role "Nsb" -Incremental {
    . load-vars "nsb\healthcheck"

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
    poke-xml "$nsb_directory\Warranty.HealthCheck.dll.config" "configuration/connectionStrings/add[@name='warranty']/@connectionString" "Data Source=$warranty_db_server;Initial Catalog=$warranty_db_name;Integrated Security=SSPI;Application Name=$db_nsb_application_name;"   
	poke-xml "$nsb_directory\Warranty.HealthCheck.dll.config" "configuration/connectionStrings/add[@name='tips']/@connectionString" "Data Source=$tips_db_server;Initial Catalog=$tips_db_name;Integrated Security=SSPI;Application Name=$db_nsb_application_name;"   
		
	poke-xml "$nsb_directory\Warranty.HealthCheck.dll.config" "configuration/appSettings/add[@key='notifications.to']/@value" $notificationTo
    
	#Update the NSB connection so it can upgrade the NSB database
	#This is needed due to the "double-hop" authentication problem when running deployments this way
	poke-xml "$nsb_directory\Warranty.HealthCheck.dll.config" "configuration/connectionStrings/add[@name='NServiceBus/Persistence']/@connectionString" "Data Source=$warranty_db_server;Initial Catalog=$db_name_nsb;User Id=$nsb_installer_user;Password=$nsb_installer_pwd;Application Name=$db_nsb_application_name;"
    
    #Install NSB Service
    &"$nsb_directory\NServiceBus.Host.exe" "/install" "/serviceName:$nsb_service_name" "/username:dwh\svc-Warranty-nsb" "/password:8k6+_6xft#y`$K_Xd" "/dependsOn:MSMQ" "NServiceBus.Production"
	
	#Set the NSB connection to the run-time setting
	poke-xml "$nsb_directory\Warranty.HealthCheck.dll.config" "configuration/connectionStrings/add[@name='NServiceBus/Persistence']/@connectionString" "Data Source=$warranty_db_server;Initial Catalog=$db_name_nsb;Integrated Security=SSPI;Application Name=$db_nsb_application_name;"

    #Start NSB Service
    Start-Service "$nsb_service_name"
    Start-Service "WolfpackAgent"

} -FullInstall {
    # You could do IIS setup here... but it is much easier to just do that with server provisioning
}

Role "Database" -Incremental {
    # Nothing to run here as part of HealthCheck
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
    
    check-pathvar $nsb_directory '$nsb_directory'
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
