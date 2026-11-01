# Builds KopatelOnline2014.exe next to this folder's parent.
#   powershell -ExecutionPolicy Bypass -File launcher\build.ps1 [-Payload path\to\payload.zip]
# payload.zip (firefox/ + webplayer/) is taken from the 2012 launcher when not given.
param([string]$Payload = "")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$res = Join-Path $PSScriptRoot "resources"
$out = Join-Path $root "KopatelOnline2014.exe"

if (-not $Payload) {
    $Payload = Join-Path $root "work\from2012\payload.zip"
    if (-not (Test-Path $Payload)) {
        $exe2012 = Join-Path (Split-Path -Parent $root) "KopatelOnline2012.exe"
        Write-Host "Taking payload.zip from $exe2012"
        New-Item -ItemType Directory -Force (Split-Path $Payload) | Out-Null
        $asm = [Reflection.Assembly]::ReflectionOnlyLoadFrom($exe2012)
        $src = $asm.GetManifestResourceStream("payload.zip")
        $dst = [IO.File]::Create($Payload)
        $src.CopyTo($dst)
        $dst.Close()
    }
}

$fw = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319"
& "$fw\csc.exe" /nologo /target:winexe /optimize+ /platform:anycpu "/out:$out" `
    /reference:System.Windows.Forms.dll /reference:System.Drawing.dll `
    "/reference:$fw\System.IO.Compression.dll" "/reference:$fw\System.IO.Compression.FileSystem.dll" `
    "/resource:$Payload,payload.zip" `
    "/resource:$res\payload_id.txt,payload_id.txt" `
    "/resource:$res\default_server.txt,default_server.txt" `
    "/resource:$res\user.js,user.js" `
    "/resource:$res\userChrome.css,userChrome.css" `
    "/resource:$res\autoconfig.js,autoconfig.js" `
    "/resource:$res\kopatel.cfg,kopatel.cfg" `
    (Join-Path $PSScriptRoot "Launcher.cs")
if ($LASTEXITCODE -ne 0) { throw "csc failed" }
Write-Host "Built $out"
