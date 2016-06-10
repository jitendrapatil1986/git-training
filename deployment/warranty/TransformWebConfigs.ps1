<#
.SYNOPSIS
Create a configuration transformation

.DESCRIPTION
This script runs an ASP.NET configuration transformation, given a source
configuration and transformation file.  MSBuild.exe is assumed to be in
the path, and Visual Studio 2012 should be installed.  Modify the path to
Microsoft.Web.Publishing.Tasks.dll if a different version of Visual Studio
is installed.  From https://gist.github.com/mpicker0/5680072/.

.PARAMETER SourceFile
The source file to use for transformations

.PARAMETER TransformFile
The transformations to apply to the source file

.PARAMETER OutputFile
Where to write the resulting output

.EXAMPLE
TransformWebConfigs -SourceFile C:\path\to\project\Web.config -TransformFile C:\path\to\project\Web.Debug.config

.LINK
http://msdn.microsoft.com/en-us/library/dd465326.aspx
#>
param(
    [Parameter(Mandatory=$true)]
    [string]$SourceFile,

    [Parameter(Mandatory=$true)]
    [string]$TransformFile,

    [Parameter(Mandatory=$true)]
    [string]$OutputFile,

    [Parameter(Mandatory=$false)]
    [string]$MSBuildLibPath
)

$dotNetVersion = "4.0"
$regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dotNetVersion"
$regProperty = "MSBuildToolsPath"

$msbuildExe = join-path -path (Get-ItemProperty $regKey).$regProperty -childpath "msbuild.exe"

if($MSBuildLibPath.length -eq 0) { $MSBuildLibPath = "deployment\modules\msbuild\" }

# Remove any trailing slashes
$MSBuildLibPath  = $MSBuildLibPath -replace '\\$','' 

# write the project build file
$BuildXml = @"
<Project ToolsVersion="4.0" DefaultTargets="TransformWebConfig" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <UsingTask TaskName="TransformXml"
             AssemblyFile="$MSBuildLibPath\Microsoft.Web.Publishing.Tasks.dll"/>
  <Target Name="TransformWebConfig">
    <TransformXml Source="${SourceFile}"
                  Transform="${TransformFile}"
                  Destination="${OutputFile}"
                  StackTrace="true" />
  </Target>
</Project>
"@

# Append a GUID so VS can build in parallel without experiencing "file in use" conflicts on build.xml
$buildFile = "build-" + [System.Guid]::NewGuid() + ".xml"

$BuildXml | Out-File $buildFile

# call msbuild
&$msbuildExe $buildFile

Remove-Item $buildFile
