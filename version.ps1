<#
.SYNOPSIS
    Updates the solutions version. Must be run in ad Administrator Powershell console.
.DESCRIPTION
    Performs version increment validation, then updates all artefacts to the specified MAJOR MINOR and PATCH.
    
    Pass -Verbose to see detailed output.
.PARAMETER Major
    The Major version number
.PARAMETER Minor
    The Minor version number
.PARAMETER Patch
    The Patch version number
.PARAMETER Force
    Optional. If specified overrides the default version validation checks.
.EXAMPLE
    ./version.ps1 -Major 2 -Minor 6 -Patch 3

    or:

    ./version.ps1 2 6 3

    or:

    ./version.ps1 1 0 0 -Force -Verbose
.NOTES
    Author: Chris Sebok
    Date:   17th October 2018
#>
param(
    [Parameter(Position=0, Mandatory=$true)]
    [int]$Major,

    [Parameter(Position=1, Mandatory=$true)]
    [int]$Minor,

    [Parameter(Position=2, Mandatory=$true)]
    [int]$Patch,

    [Parameter(Position=3, Mandatory=$false)]
    [switch]$Force = $false
)

function Out-FileNoBOM() {
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=$true)]
        [string] $path,
        
        [Parameter(Position=1, Mandatory=$true)]
        [string[]] $content
    )
    $Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False
    [System.IO.File]::WriteAllLines($path, $content, $Utf8NoBomEncoding)
}

function Set-AppSettingsVersion() {
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=$true)]
        [string] $path,
        
        [Parameter(Position=1, Mandatory=$true)]
        [string] $version
    )
    $json = Get-Content $path -Raw | ConvertFrom-Json
    $json.General.Version = $version

    $jsonString = $json | ConvertTo-Json -Depth 20 | Format-Json
    $jsonString | Set-Content -Path $path
}

function Set-PackageJsonVersion() {
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=$true)]
        [string] $path,
        
        [Parameter(Position=1, Mandatory=$true)]
        [string] $version
    )
    $json = Get-Content $path -Raw | ConvertFrom-Json
    $json.version = $version

    $jsonString = $json | ConvertTo-Json -Depth 20 | Format-Json
    $jsonString | Set-Content -Path $path
}

function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
  $newLine = [System.Environment]::NewLine

  $indent = 0;
  ($json -Split $newLine |
    % {
      if ($_ -match '[\}\]]') {
        # This line contains  ] or }, decrement the indentation level
        $indent--
      }
      if($indent -lt 0) {
        $indent = 0
      }
      $line = (" " * $indent * 2) + $_.TrimStart().Replace(":  ", ": ")
      if ($_ -match '[\{\[]') {
        # This line contains [ or {, increment the indentation level
        $indent++
      }
      $line
  }) -Join $newLine
}

function Set-CSProjFileVersion() {
    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=$true, ValueFromPipeline)]
        [string] $path,
        
        [Parameter(Position=1, Mandatory=$true)]
        [string] $version
    )
    [xml]$xmlDoc = Get-Content $path
    $xmlDoc.Project.PropertyGroup[0].AssemblyVersion = $version + ".0"
    $xmlDoc.Project.PropertyGroup[0].FileVersion = $version + ".0"
    $xmlDoc.Save($path)
}

Push-Location -Path $PSScriptRoot

$versionFile = Resolve-Path "VERSION"
$packageJsonFile = Resolve-Path "src/*.Web/package.json"
$packageLockJsonFile = Resolve-Path "src/*.Web/package-lock.json"
$appsettingsFile = Resolve-Path "src/*.API/appsettings.json"
$csProjSearchRoot = Resolve-Path "/"
$csProjFiles = (Get-ChildItem $csProjSearchRoot -Recurse -Exclude "*\node_modules\*" -Filter "*.csproj")

if(!$Force) {
    # VERSION file is the primary source for all CD versioning, read that to perform validation checks
    $curVer = (Get-Content $versionFile -Raw).Split('.')
    Write-Verbose "Found Version $curVer in '$versionFile' file..."

    $curMaj = $curVer[0]
    $curMin = $curVer[1]
    $curPatch = $curVer[2]

    if($Major -lt $curMaj) {
        throw 'Passed Major version is less than the current Major version'
    }

    if($Major -eq $curMaj -and $Minor -lt $curMin) {
        throw 'Passed Minor version is less than the current Minor version'
    }

    if($Minor -eq $curMin -and $Patch -lt $curPatch) {
        throw 'Passed Patch version is less than the current Patch version'
    }
}

# Validation skipped or passed
$newVersion = "$Major.$Minor.$Patch"

Write-Verbose "Writing new version '$newVersion'..."
<#
Write-Verbose "Writing '$versionFile'..."
Out-FileNoBOM $versionFile $newVersion

Write-Verbose "Writing '$packageJsonFile'..."
Set-PackageJsonVersion $packageJsonFile $newVersion

Write-Verbose "Writing '$packageLockJsonFile'..."
Set-PackageJsonVersion $packageLockJsonFile $newVersion

Write-Verbose "Writing '$appsettingsFile'..."
Set-AppSettingsVersion $appsettingsFile $newVersion

#>

Write-Verbose "Writing $($csProjFiles.Length) .csproj files..."
$csProjFiles | Set-CSProjFileVersion -version $newVersion

Write-Host "Done. You should now stage + commit the changes, git tag, then push to publish"

Pop-Location