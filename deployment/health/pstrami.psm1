#--- deployment functions

function Deploy-Package{
	param(
		[string]$EnvironmentName,
		[string]$Package,
		[boolean]$Verbose=$false,
		[boolean]$Install=$false)
		
	Load-Configuration

	if(test-path .\pstrami_logs) {
		rmdir .\pstrami_logs -recurse | out-null
	}
	
	if(-not (test-path .\pstrami_logs)) {
		mkdir .\pstrami_logs | out-null
	}
	
	$package = resolve-path $package
	get-environments | ?{$_.Name -eq $EnvironmentName} | %{deploy-environment $_ $package $Verbose $Install}

	write-host "`nFinished!"
	exit 0
}

function Get-MSWebDeployInstallPath(){
    return (get-childitem "HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy" | Select -last 1).GetValue("InstallPath") + "MSdeploy.exe"
}

function Deploy-Environment($environment, [string]$package, [boolean]$Verbose=$false, [boolean]$Install=$false) {
	$environment.Servers | %{install-remoteserver $_ $package $environment $Verbose $Install}
}

function Install-RemoteServer{
	param(
		[object]  $server,
		[string]  $packagePath,
		[object]  $environment,
		[boolean] $Verbose=$false,
		[boolean] $OneTime=0,
		[string] $successMessage = "Deployment Succeded")

	$servername = $server.Name
	write-host "`nInstall-RemoteServer $servername"
	
	Send-Files $packagePath $server.Name $environment.InstallPath $server.Credential

	Create-RemoteBootstrapper $server.Name $environment.InstallPath $environmentName $OneTime | out-file .\pstrami_logs\bootstrap.bat -encoding ASCII

	$result = Invoke-RemoteCommand $server.Name (resolve-path .\pstrami_logs\bootstrap.bat) $server.Credential $Verbose
	
	if(-not (select-string -inputobject $result -pattern $successMessage))
	{    
		write-host "`n"
		write-host "`nRemote Command Output:`n"
	
		$result|% { foreach($line in $_.Split("`r")) { if($line){ $line|Out-Default}}}
		
		write-host "Install-RemoteServer Failed" -foregroundcolor "red"
		exit '-1'
	}
	
	if($Verbose -eq $true) {
		write-host "`n"
		write-host "`nRemote Command Output:`n"
	
		$result|% { foreach($line in $_.Split("`r")) { if($line){ $line|Out-Default}}}
	}	
	
	write-host "Install-RemoteServer Succeeded" -foregroundcolor "green"
}

function Invoke-RemoteCommand{
	param(  [string] $server,
			[string] $cmd,
			[string] $cred="",
			[boolean] $Verbose=$false)
	write-host "Invoke-RemoteCommand $server"
	
#    if($cred -ne "")
#    {
#        $cred = ",getCredentials=" + $cred
#    }
	$msdeployexe= Get-MSWebDeployInstallPath
	&"$msdeployexe" "-verb:sync" "-dest:auto,computername=$server$cred" "-source:runCommand=$cmd,waitInterval=15000,waitAttempts=50" "-verbose" "-allowUntrusted" | out-file .\pstrami_logs\invoke-remotecommand.$server.log -width 120
	
	$result = get-content .\pstrami_logs\invoke-remotecommand.$server.log
	return $result    
}

function Send-Files{
	param(  [string] $packagePath,
			[string] $server,
			[string] $remotePackagePath,
			[string] $cred)
	write-host "Sending Files to $server : $remotePackagePath"        
	$msdeployexe= Get-MSWebDeployInstallPath
   
#    if($cred -ne "")
#    {
#        $cred = ",getCredentials=" + $cred
#    }

	&"$msdeployexe" "-verb:sync" "-source:dirPath=$packagePath" "-dest:dirPath=$remotePackagePath,computername=$server$cred" "-verbose" "-allowUntrusted" "-skip:objectName=dirPath,absolutePath=pstrami_logs" | out-file .\pstrami_logs\sync-package.$server.log -width 120
	if($Verbose -eq $true) {
		get-content .\pstrami_logs\sync-package.$server.log | Out-Default
	}	
}

function Create-RemoteBootstrapper{
	param(  [string]$server,
			[string] $remotePackagePath,
			[string] $EnvironmentName,
			[boolean] $OneTime=0)
	
	$fullinstall = 0
	
	if($OneTime -eq $true) {
		$fullinstall = 1
	}
	
	return "@echo off
	cd /D $remotePackagePath    
	powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command `"& { import-module .\pstrami.psm1 -DisableNameChecking ; Load-Configuration;Install-LocalServer $server $remotePackagePath $EnvironmentName $fullinstall;if ($lastexitcode -ne 0) { write-host `"ERROR: $lastexitcode`" -fore RED }; stop-process `$pid }"
}

#--- local install functions

function Install-LocalServer {
	param([string] $serverName,[string] $packagePath,[string]$environmentName,[boolean]$OneTime=$false)
	
	set-location $packagePath
	
	dir modules\*.psm1 | Import-Module -DisableNameChecking

	$global:env = $environmentName
	write-host "Deploying for environment named $global:env"
	
	Get-ServerRoles $serverName $EnvironmentName | %{execute-role $_ $OneTime}
	
	write-host "Deployment Succeded"
}

function Get-ServerRoles {
	param([string]$serverName,[string]$environmentName)
	
	$definedRoles = $script:context.Peek().roles
	$environments = Get-Environments | ?{$_.Name -eq $EnvironmentName}
	$servers = $environments.Servers | ?{$_.name -eq $serverName}
	$servers | %{$_.Roles} | ?{$definedRoles.ContainsKey($_)} | %{$definedRoles[$_]}
}

function Execute-Role($role, [boolean]$fullinstall) {
	write-host ("Executing Role: {0}" -f $role.Name)
	
	if($fullinstall -eq $true) {
		invoke-command -scriptblock $_.FullInstall
	}

	invoke-command -scriptblock $_.Action -ErrorAction Stop
}

#--- configuration functions

function Load-Configuration {
	param([string]$configFile=".\pstrami.config.ps1")
	write-host "Loading Config from $configFile"

	if ($script:context -eq $null)
	{
		$script:context = New-Object System.Collections.Stack
	}
		
	$script:context.push(@{
		"roles" = @{}; #contains the deployment steps for each role        
		"environments" = @{};            
	})
	. $configFile
}

function Role {
	param(
	[Parameter(Position=0,Mandatory=1)]
	[string]$name = $null, 
	[Parameter(Position=1,Mandatory=1)]
	[scriptblock]$incremental = $null, 
	[Parameter(Position=1,Mandatory=1)]
	[scriptblock]$fullinstall = $null
	)
	$newTask = @{
		Name = $name
		Action = $incremental
		FullInstall = $fullinstall
	}
	
	$taskKey = $name.ToLower()
	
	Assert (-not $script:context.Peek().roles.ContainsKey($taskKey)) "Error: Role, $name, has already been defined."
	
	$script:context.Peek().roles.$taskKey = $newTask
}

function Server {
	param(
	[Parameter(Position=0,Mandatory=1)]
	[string]$name, 
	[Parameter(Position=1,Mandatory=1)]
	[string[]]
	$roles = $null,
	[string] $credential=""
	)
	$newTask = "" | select-object Name,Roles,Credential
	$newTask.Name = $name
	$newTask.Roles = $roles
	$newTask.Credential  = $credential
	
	return $newTask
}
	
function Environment {
	param(
	[Parameter(Position=0,Mandatory=1)]
	[string]$name, 
	[Parameter(Position=1,Mandatory=1)]
	[object[]]$servers ,
	[string] $installPath
	)
	$newTask = "" | select-object Name,Servers,InstallPath
	$newTask.Name = $name
	$newTask.Servers = $servers
	$newTask.InstallPath = $installPath
	
	
	$taskKey = $name.ToLower()
	
	Assert (-not $script:context.Peek().environments.ContainsKey($taskKey)) "Error: Environment, $name, has already been defined."
	
	$script:context.Peek().environments.$taskKey = $newTask
}

#--- general functions

function Assert { [CmdletBinding(
	SupportsShouldProcess=$False,
	SupportsTransactions=$False, 
	ConfirmImpact="None",
	DefaultParameterSetName="")]
	
	param(
	  [Parameter(Position=0,Mandatory=1)]$conditionToCheck,
	  [Parameter(Position=1,Mandatory=1)]$failureMessage
	)
	if (!$conditionToCheck) { throw $failureMessage }
}

function Get-Environments{
	return $script:context.Peek().environments.Values
}

function Get-Roles {
	return $script:context.Peek().roles.Values
}

Export-ModuleMember Load-Configuration, Deploy-Package, Install-LocalServer