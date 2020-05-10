<#
.SYNOPSIS
    Updates the solution's version. Must be run in an Administrator Powershell console.
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
      $lineValue = $_
      # Undo selected ConvertTo-Json encoding
      $lineValue = $lineValue.Replace('\u0026', '&');
      $lineValue = $lineValue.Replace('\u003e', '>');
      $lineValue = $lineValue.Replace('\u003c', '<');

      $line = (" " * $indent * 2) + $lineValue.TrimStart().Replace(":  ", ": ")
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
        [Parameter(Mandatory=$true, ValueFromPipeline)]
        [string] $path,
        
        [Parameter(Mandatory=$true)]
        [string] $version
    )
    Begin {
        $v = $version + ".0";
    }

    Process {
        Write-Verbose "Updating $path with version $v";

        $versionNodeNames = "AssemblyVersion", "FileVersion", "Version"
        [xml]$xmlDoc = Get-Content $path -Raw

        $parent = (Select-Xml -Xml $xmlDoc -XPath "/Project/PropertyGroup[1]").Node;

        $versionNodeNames | % {
            $versionNode = $parent.SelectNodes($_);
            if($versionNode.Count -gt 0) {
                Write-Verbose "Update $_"
                $versionNode[0].InnerText = $v;
            } else {
                Write-Verbose "Add $_"
                $newNode = $xmlDoc.CreateElement($_);
                $newNode.InnerText = $v;
                $parent.AppendChild($newNode) | Out-Null;
            }
        }

        $xmlDoc.Save($path)
    }

    End {
        
    }
}

function Set-NGEnvironmentVersions() {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true, ValueFromPipeline)]
        [string] $path,
        
        [Parameter(Mandatory=$true)]
        [string] $version
    )
    Begin {
    }

    Process {
        Write-Verbose "Updating $path with version $v";

        $ts = Get-Content $path -Raw
        $ts = $ts -replace "version:([ \t]*)'[0-9\.]{3,}'", "version: '$version'"
        Out-FileNoBOM -path $path -content $ts
    }

    End {
        
    }
}

Write-Verbose "Script start"

Push-Location -Path $PSScriptRoot

$versionFile = Resolve-Path "VERSION"
$packageJsonFile = Resolve-Path "src/*.Web/package.json"
$packageLockJsonFile = Resolve-Path "src/*.Web/package-lock.json"
$appsettingsFile = Resolve-Path "src/*.API/appsettings.json"
$ngEnvironmentFiles = Resolve-Path "src/*.Web/src/environments/environment*.ts"
$csProjSearchRoot = $PSScriptRoot

Write-Verbose "Reading .csproj files from '$csProjSearchRoot'"
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

Write-Verbose "Writing '$versionFile'..."
Out-FileNoBOM $versionFile $newVersion

Write-Verbose "Writing '$packageJsonFile'..."
Set-PackageJsonVersion $packageJsonFile $newVersion

Write-Verbose "Writing '$packageLockJsonFile'..."
Set-PackageJsonVersion $packageLockJsonFile $newVersion

Write-Verbose "Writing '$appsettingsFile'..."
Set-AppSettingsVersion $appsettingsFile $newVersion

Write-Verbose "Writing $($csProjFiles.Length) .csproj files..."
$csProjFiles | Set-CSProjFileVersion -version $newVersion

Write-Verbose "Writing $($ngEnvironmentFiles.Length) Angular Environment files..."
$ngEnvironmentFiles | Set-NGEnvironmentVersions -version $newVersion

Write-Host "Done. You should now stage, commit, tag and push your changes."

Pop-Location