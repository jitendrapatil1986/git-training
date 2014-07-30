# !!!DO NOT MODIFY THIS FILE TO CUSTOMIZE THE DEPLOYMENT!!!
#
# Create a file named <ENV>.vars.ps1, where <ENV> is the environment you want to customize.

$customErrorMode = "RemoteOnly"

$roundhouse_dir = ".\tools\roundhouse"
$roundhouse_output_dir = "$roundhouse_dir\output"
$roundhouse_exe_path = "$roundhouse_dir\rh.exe"
$roundhouse_version_file = ".\nsb\warranty\Warranty.Server.dll"

$smtpServer = "SMTPRELAY.CORP.WEEKLEYHOMES.COM"