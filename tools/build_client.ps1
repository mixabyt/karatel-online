# Builds server\www\Digger2014.unity3d: the original 2014 client with Assembly-CSharp.dll patched
# to use our server (see docs\protocol.md).
#   powershell -ExecutionPolicy Bypass -File tools\build_client.ps1
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$work = Join-Path $root "work"
$original = Join-Path $work "client\Digger2014.unity3d"
$unpacked = Join-Path $work "unpacked"
$patched = Join-Path $work "patched"
$webplayer = Join-Path $work "from2012\payload\webplayer"
$monoLib = Join-Path $webplayer "mono\Stable3.x.x\Data\lib"
$playerLib = Join-Path $webplayer "player\Stable3.x.x\Data\lib"
$out = Join-Path $root "server\www\Digger2014.unity3d"

if (-not (Test-Path $original)) {
    New-Item -ItemType Directory -Force (Split-Path $original) | Out-Null
    $url = "http://web.archive.org/web/20260413070227id_/https://kopatel-online.ru/Digger/Digger2014.unity3d"
    Write-Host "Downloading $url"
    & curl.exe -sL --retry 3 -o $original $url
    if ($LASTEXITCODE -ne 0) { throw "download failed" }
}
if (-not (Test-Path $unpacked)) {
    & python -I (Join-Path $PSScriptRoot "unityweb.py") unpack $original $unpacked
    if ($LASTEXITCODE -ne 0) { throw "unpack failed" }
}

# KopatelPatch.dll against the player's own mscorlib 2.0.5.0, so the copied IL references match.
$patchDll = Join-Path $work "KopatelPatch.dll"
$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
& $csc /nologo /noconfig /nostdlib+ /target:library /optimize+ "/out:$patchDll" `
    "/reference:$monoLib\mscorlib.dll" "/reference:$playerLib\UnityEngine.dll" `
    (Join-Path $PSScriptRoot "patcher\KopatelPatch.cs")
if ($LASTEXITCODE -ne 0) { throw "csc failed" }

if (Test-Path $patched) { Remove-Item -Recurse -Force $patched }
Copy-Item -Recurse $unpacked $patched
Remove-Item (Join-Path $patched "Assembly-CSharp.dll")
& dotnet run --project (Join-Path $PSScriptRoot "patcher\Patcher") -c Release -- `
    (Join-Path $unpacked "Assembly-CSharp.dll") $patchDll (Join-Path $patched "Assembly-CSharp.dll") `
    $unpacked $monoLib $playerLib
if ($LASTEXITCODE -ne 0) { throw "patcher failed" }

New-Item -ItemType Directory -Force (Split-Path $out) | Out-Null
& python -I (Join-Path $PSScriptRoot "unityweb.py") pack $original $patched $out
if ($LASTEXITCODE -ne 0) { throw "pack failed" }
