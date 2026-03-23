#Requires -Version 5.1
<#
.SYNOPSIS
    Builds the MvbaCore solution. Replaces the Ruby/Rake build system.

.PARAMETER Configuration
    Build configuration: Debug or Release. Defaults to Debug.

.PARAMETER TargetFramework
    Target framework subfolder under bin\<Configuration>\ to copy from. Defaults to net48.

.PARAMETER Artifacts
    Destination folder for the NuGet package. Defaults to .\dist.

.PARAMETER Task
    Which task to run: default, clean, assemblyinfo, build, copy_artifacts, test, pack.
    Defaults to 'default' (full pipeline: clean → assemblyinfo → build → copy_artifacts → test → pack).

.EXAMPLE
    .\build.ps1
    .\build.ps1 -Configuration Release
    .\build.ps1 -Task test
    .\build.ps1 -Artifacts "\\fsa\Shares2\dev\Build Artifacts\MvbaCore\master"
#>
param(
    [string]$Configuration   = "Debug",
    [string]$TargetFramework = "net48",
    [string]$Artifacts       = "",
    [string]$Task            = "default"
)

$ErrorActionPreference = "Stop"
$startTime = Get-Date

# --- Constants (mirrors RakeFile) ---
$PRODUCT_NAME         = "MvbaCore"
$CLR_TOOLS_VERSION    = "v4.8"
$DEFAULT_BUILD_NUMBER = "1.1.0"
$COMPANY_NAME         = "MVBA, P.C."
$COPYRIGHT            = "MVBA, P.C. (c) 2009-2020"
$ROOT                 = $PSScriptRoot
$COMPILE_TARGET       = $Configuration
$TARGET_FRAMEWORK     = $TargetFramework
$STAGE                = Join-Path $ROOT "build"

# Artifacts folder: parameter wins, then fall back to .\dist
if ($Artifacts -eq "") {
    $ARTIFACTS = Join-Path $ROOT "dist"
} else {
    $ARTIFACTS = $Artifacts
}

$nunit_cmd = Join-Path $ROOT "tools\NUnit\nunit-console.exe"

# ---------------------------------------------------------------------------
# Helper functions
# ---------------------------------------------------------------------------

function Get-BuildNumber {
    try {
        $gittag = (& git describe --long 2>$null) -join ""
        if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($gittag)) {
            return $DEFAULT_BUILD_NUMBER
        }
        $gittag = $gittag.Trim()
        Write-Host "gittag: $gittag"
        $parts              = $gittag -split "-"
        $base_version       = $parts[0] -replace "^v", ""
        $git_build_revision = $parts[1]
        $git_short_hash     = $parts[2]
        Write-Host "base_version:       $base_version"
        Write-Host "git_build_revision: $git_build_revision"
        Write-Host "git_short_hash:     $git_short_hash"
        return "$base_version.$git_build_revision"
    } catch {
        return $DEFAULT_BUILD_NUMBER
    }
}

function Get-GitHash {
    try {
        $gittag = (& git describe --long 2>$null) -join ""
        if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($gittag)) {
            return "git unavailable"
        }
        $parts = $gittag.Trim() -split "-"
        return $parts[2]
    } catch {
        return "git unavailable"
    }
}

function Find-MSBuild {
    # 1. Try vswhere (present with VS 2017+)
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $found = & $vswhere -latest -requires Microsoft.Component.MSBuild `
                            -find "MSBuild\**\Bin\MSBuild.exe" 2>$null |
                 Select-Object -First 1
        if ($found -and (Test-Path $found)) { return $found }
    }

    # 2. Fallback: well-known VS / Build Tools paths
    $candidates = @(
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    )
    foreach ($c in $candidates) {
        if (Test-Path $c) { return $c }
    }

    # 3. Last resort: whatever is on PATH
    $onPath = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($onPath) { return $onPath.Source }

    throw "MSBuild not found. Install Visual Studio or the Build Tools."
}

function Copy-OutputFiles {
    param(
        [string]$FromDir,
        [string[]]$Extensions,
        [string]$OutDir
    )
    if (-not (Test-Path $FromDir)) {
        Write-Warning "Source directory not found, skipping: $FromDir"
        return
    }
    Get-ChildItem -Path $FromDir |
        Where-Object { -not $_.PSIsContainer -and $_.Extension -in $Extensions } |
        ForEach-Object { Copy-Item $_.FullName -Destination $OutDir -Force }
}

# ---------------------------------------------------------------------------
# Tasks
# ---------------------------------------------------------------------------

function Invoke-Clean {
    Write-Host ""
    Write-Host "=== clean ==="
    if (Test-Path $STAGE) { Remove-Item -Recurse -Force $STAGE }
    # Brief wait to let the filesystem settle (mirrors the rake wait_for loop)
    $deadline = (Get-Date).AddSeconds(5)
    while ((Test-Path $STAGE) -and (Get-Date) -lt $deadline) { Start-Sleep -Milliseconds 500 }
    New-Item -ItemType Directory -Path $STAGE | Out-Null
    if (-not (Test-Path $ARTIFACTS)) { New-Item -ItemType Directory -Path $ARTIFACTS | Out-Null }
    Write-Host "Build directory ready: $STAGE"
}

function Invoke-AssemblyInfo {
    Write-Host ""
    Write-Host "=== assemblyinfo ==="
    $build_number = Get-BuildNumber
    $git_hash     = Get-GitHash
    $outFile      = Join-Path $ROOT "src\Directory.Build.props"

    Write-Host "Writing $outFile  (version $build_number, hash $git_hash)"

    $content = @"
<Project>
  <PropertyGroup>
    <Authors>$COMPANY_NAME</Authors>
    <Company>$COMPANY_NAME</Company>
    <Description>A set of common utility classes that we use in our projects at Mvba. (git sha for this version: $git_hash)</Description>
    <Copyright>$COPYRIGHT</Copyright>
    <PackageLicenseFile>License.txt</PackageLicenseFile>
    <NeutralLanguage>en-US</NeutralLanguage>
    <RepositoryUrl>https://github.com/mvbalaw/$PRODUCT_NAME</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <Version>$build_number</Version>
    <FileVersion>$build_number</FileVersion>
    <ProductName>$PRODUCT_NAME</ProductName>
  </PropertyGroup>

  <ItemGroup>
    <Content Include="..\..\License.txt" Pack="true" Visible="false" PackagePath="" />  </ItemGroup>
</Project>
"@

    # Write UTF-8 without BOM
    [System.IO.File]::WriteAllText($outFile, $content, [System.Text.UTF8Encoding]::new($false))
}

function Invoke-Build {
    Write-Host ""
    Write-Host "=== build ==="
    Write-Host "Compiling $PRODUCT_NAME in $COMPILE_TARGET mode..."
    $msbuild = Find-MSBuild
    Write-Host "Using MSBuild: $msbuild"
    & $msbuild "src\$PRODUCT_NAME.sln" /t:restore`;Rebuild /verbosity:quiet /nologo /p:Configuration=$COMPILE_TARGET
    if ($LASTEXITCODE -ne 0) { throw "MSBuild failed (exit code $LASTEXITCODE)." }
}

function Invoke-CopyArtifacts {
    Write-Host ""
    Write-Host "=== copy_artifacts ==="
    Write-Host "Copying build outputs to: $STAGE"
    $exts = @(".dll", ".pdb", ".exe")
    Copy-OutputFiles "src\$PRODUCT_NAME\bin\$COMPILE_TARGET\$TARGET_FRAMEWORK"       $exts $STAGE
    Copy-OutputFiles "src\$PRODUCT_NAME.Tests\bin\$COMPILE_TARGET\$TARGET_FRAMEWORK" $exts $STAGE
}

function Invoke-Test {
    Write-Host ""
    Write-Host "=== test ==="
    Write-Host "Running unit tests: $STAGE\$PRODUCT_NAME.Tests.dll"
    & $nunit_cmd "$STAGE\$PRODUCT_NAME.Tests.dll" /framework:$CLR_TOOLS_VERSION
    if ($LASTEXITCODE -ne 0) { throw "NUnit tests failed (exit code $LASTEXITCODE)." }
}

function Invoke-Pack {
    Write-Host ""
    Write-Host "=== pack ==="
    Write-Host "Creating NuGet package in: $ARTIFACTS"
    $msbuild = Find-MSBuild
    & $msbuild "src\$PRODUCT_NAME\$PRODUCT_NAME.csproj" /p:PackageOutputPath="$ARTIFACTS" /t:pack /verbosity:quiet /nologo
    if ($LASTEXITCODE -ne 0) { throw "MSBuild pack failed (exit code $LASTEXITCODE)." }
    Write-Host "Artifacts available at: $ARTIFACTS"
}

# ---------------------------------------------------------------------------
# Task dispatcher
# ---------------------------------------------------------------------------

switch ($Task.ToLower()) {
    "clean"         { Invoke-Clean }
    "assemblyinfo"  { Invoke-AssemblyInfo }
    "build" {
        Invoke-Clean
        Invoke-AssemblyInfo
        Invoke-Build
    }
    "copy_artifacts" {
        Invoke-Clean
        Invoke-AssemblyInfo
        Invoke-Build
        Invoke-CopyArtifacts
    }
    "test" {
        Invoke-Clean
        Invoke-AssemblyInfo
        Invoke-Build
        Invoke-CopyArtifacts
        Invoke-Test
    }
    "pack" {
        Invoke-Clean
        Invoke-AssemblyInfo
        Invoke-Build
        Invoke-CopyArtifacts
        Invoke-Test
        Invoke-Pack
    }
    "default" {
        Invoke-Clean
        Invoke-AssemblyInfo
        Invoke-Build
        Invoke-CopyArtifacts
        Invoke-Test
        Invoke-Pack
    }
    default {
        Write-Error "Unknown task '$Task'. Valid tasks: default, clean, assemblyinfo, build, copy_artifacts, test, pack"
        exit 1
    }
}

$elapsed = (Get-Date) - $startTime
Write-Host ""
Write-Host "Build Succeeded - time elapsed: $([Math]::Round($elapsed.TotalSeconds, 1)) seconds"
