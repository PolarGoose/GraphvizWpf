Function Info($msg) {
    Write-Host -ForegroundColor DarkGreen "`nINFO: $msg`n"
}

Function Error($msg) {
    Write-Host `n`n
    Write-Error $msg
    exit 1
}

Function CheckReturnCodeOfPreviousCommand($msg) {
    if(-Not $?) {
        Error "${msg}. Error code: $LastExitCode"
    }
}

Function GetVersion() {
    $gitCommand = Get-Command -Name git

    try { $tag = & $gitCommand describe --exact-match --tags HEAD } catch { }
    if(-Not $?) {
        Info "The commit is not tagged. Use 'v0.0' as a version instead"
        $tag = "v0.0"
    }

    return "$($tag.Substring(1))"
}

Function RemoveFileIfExists($fileName) {
    Info "Remove '$fileName'"
    Remove-Item $fileName  -Force  -Recurse -ErrorAction SilentlyContinue
}

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$root = Resolve-Path "$PSScriptRoot"
$buildDir = "$root/build"
$version = GetVersion

Info "Build project. Version: '$version'"
dotnet pack `
    --configuration Release `
    /property:DebugType=None `
    /property:Platform="Any CPU" `
    /property:Version=$version `
    --output $buildDir `
    --verbosity minimal `
    $root/src/GraphvizWpf/GraphvizWpf.csproj
CheckReturnCodeOfPreviousCommand "build failed"
