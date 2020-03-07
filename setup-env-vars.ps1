param([string]$file = ".env")
$ErrorActionPreference = "Stop"

$root = $PSScriptRoot
Set-Location $root

$vars = Import-Csv .\$file -Delimiter '=' -Header Var, Value

$vars | % {
    $name = $_.Var
    $value = $_.Value

    New-Item env:\$name -Value $value -Force | Out-Null
}

# DotNet Core Env Variable overrides. Out-Null for safe streaming.
New-Item env:\CAPTCHA__SecretKey -Value "$Env:CAPTCHA_KEY" -Force | Out-Null
New-Item env:\General__SmtpServer -Value "$Env:SMTP_SERVER" -Force | Out-Null
New-Item env:\General__SmtpServerPort -Value "$Env:SMTP_PORT" -Force | Out-Null
New-Item env:\General__SmtpEnableSSL -Value "$Env:SMTP_ENABLE_SSL" -Force | Out-Null
New-Item env:\General__SmtpUsername -Value "$Env:SMTP_USERNAME" -Force | Out-Null
New-Item env:\General__SmtpPassword -Value "$Env:SMTP_PASSWORD" -Force | Out-Null
New-Item env:\General__ErrorEmailFrom -Value "$Env:ERROR_EMAIL_FROM" -Force | Out-Null
New-Item env:\General__ErrorEmailTo -Value "$Env:ERROR_EMAIL_TO" -Force | Out-Null
New-Item env:\General__ContactEmailTo -Value "$Env:CONTACT_EMAIL_TO" -Force | Out-Null

Write-Host "======================================================="
Write-Host "="
Write-Host "= .Net environment variables set from Docker .env file"
Write-Host "="
Write-Host "======================================================="
# gci env:\
