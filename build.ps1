########################
# THE BUILD!
########################

Push-Location $PSScriptRoot
Import-Module $PSScriptRoot\Build\Autofac.Build.psd1 -Force

$artifactsPath = "$PSScriptRoot\artifacts"
$packagesPath = "$artifactsPath\packages"
$sdkVersion = (Get-Content "$PSScriptRoot\global.json" | ConvertFrom-Json).sdk.version

# Clean up artifacts folder
if (Test-Path $artifactsPath) {
    Write-Message "Cleaning $artifactsPath folder"
    Remove-Item $artifactsPath -Force -Recurse
}

# Install dotnet CLI
Write-Message "Installing .NET Core SDK version $sdkVersion"
Install-DotNetCli -Version $sdkVersion

# Write out dotnet information
& dotnet --info

# Set version suffix
$branch = @{ $true = $env:APPVEYOR_REPO_BRANCH; $false = $(git symbolic-ref --short -q HEAD) }[$NULL -ne $env:APPVEYOR_REPO_BRANCH];
$revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $env:APPVEYOR_BUILD_NUMBER, 10); $false = "local" }[$NULL -ne $env:APPVEYOR_BUILD_NUMBER];
$versionSuffix = @{ $true = ""; $false = "$(($branch -replace '.*/','').Substring(0, [math]::Min(10,$branch.Length)))-$revision" }[$branch -eq "master" -and $revision -ne "local"]

Write-Message "Package version suffix is '$versionSuffix'"

# Package restore
Write-Message "Restoring packages"
Get-DotNetProjectDirectory -RootPath $PSScriptRoot | Restore-DependencyPackages

# Build/package
Write-Message "Building projects and packages"
Get-DotNetProjectDirectory -RootPath $PSScriptRoot\src | Invoke-DotNetPack -PackagesPath $packagesPath -VersionSuffix $versionSuffix
Get-DotNetProjectDirectory -RootPath $PSScriptRoot\samples | Invoke-DotNetBuild

# Test
Write-Message "Executing unit tests"
Get-DotNetProjectDirectory -RootPath $PSScriptRoot\test | Invoke-Test

# Finished
Write-Message "Build finished"
Pop-Location
