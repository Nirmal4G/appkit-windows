# Script to Run UWP UI Tests using TAEF (Test Authoring and Execution Framework).

# Variables
$RepoDir = Split-Path -Path $PSScriptRoot -Parent
$taefPkg = "${RepoDir}tools\TAEF.Redist.Wlk\"

$configuration = "Release"
$targetFramework = "netcoreapp3.1"
$targetMachine = "x64"
$targetRuntime = "win10-${targetMachine}"

$taefDir = "${taefPkg}build\Binaries\${targetMachine}\CoreClr\"
$taefExe = "${taefDir}TE.exe"

$taefUITests = "**\bin\**\UITests*TAEF.dll"
$taefLogFile = "VSTestResults.UWP.UI.wtl"

. $taefExe $taefUITests /screenCaptureOnError /enableWttLogging /logFile:$taefLogFile