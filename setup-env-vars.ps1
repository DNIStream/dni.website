param([string]$file = "docker-envs\dev.env")
$ErrorActionPreference = "Stop"

$root = $PSScriptRoot
Set-Location $root

$vars = Import-Csv .\$file -Delimiter '=' -Header Var, Value

$vars | % {
    $name = $_.Var
    $value = $_.Value

    New-Item env:\$name -Value $value -Force | Out-Null
}

# DotNet Core Env Variable overrides
New-Item env:\CAPTCHA__SecretKey -Value $Env:CAPTCHA_KEY -Force
New-Item env:\SendGrid__ApiKey -Value $Env:SENDGRID_KEY -Force
# New-Item env:\General__SmtpServer -Value "localhost" -Force
New-Item env:\General__SmtpServer -Value $Env:SMTP_SERVER -Force
New-Item env:\General__ErrorEmailFrom -Value $Env:ERROR_EMAIL_FROM -Force
New-Item env:\General__ErrorEmailTo -Value $Env:ERROR_EMAIL_TO -Force
New-Item env:\General__ContactEmailTo -Value $Env:CONTACT_EMAIL_TO -Force

gci env:\
