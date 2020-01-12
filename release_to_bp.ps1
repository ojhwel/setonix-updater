Write-Host "Releasing..."
& "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe" C:\Users\OJ\Source\Repos\SetonixUpdater\SetonixUpdater.csproj -Build "Release|Any CPU" | Out-Null
if ($LASTEXITCODE -ne 0 -and $LASTEXITCODE -ne $null) {
    Write-Host "Build error"
    exit
}
Copy-Item "C:\Users\OJ\Source\Repos\SetonixUpdater\bin\release\setonix_updater.exe" -Destination "C:\Users\OJ\Source\Repos\BerlinalePlaner\BerlinalePlaner" -force
Copy-Item "C:\Users\OJ\Source\Repos\SetonixUpdater\bin\release\de\*.*" -Destination "C:\Users\OJ\Source\Repos\BerlinalePlaner\BerlinalePlaner\de" -force
Write-Host "Done"
