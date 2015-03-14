$root = (split-path -parent $MyInvocation.MyCommand.Definition) + '\..'
$version = [System.Reflection.Assembly]::LoadFile("$root\Result.OrDefault\bin\Release\Result.OrDefault.dll").GetName().Version
$versionStr = "{0}.{1}.{2}" -f ($version.Major, $version.Minor, $version.Build)

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\.nuget\Result.OrDefault.nuspec) 
$content = $content -replace '\$version\$',$versionStr

$content | Out-File $root\.nuget\Result.OrDefault.compiled.nuspec

& $root\NuGet\NuGet.exe pack $root\.nuget\Result.OrDefault.compiled.nuspec