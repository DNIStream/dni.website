$root = $PSScriptRoot
Set-Location $root
$dotnet = "dotnet.exe"
# Execute
$env:ASPNETCORE_URLS = "http://*:12341";
$env:ASPNETCORE_ENVIRONMENT = "Development";
$env:DOTNET_USE_POLLING_FILE_WATCHER = "False";

./setup-env-vars.ps1

Push-Location src/DNI.API/
try {
    & $dotnet @("watch", "run", "-c", "Debug", "--no-launch-profile", "--force")
} finally {
    Pop-Location
}
