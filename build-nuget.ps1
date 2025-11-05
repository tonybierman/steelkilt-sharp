# Define directories
$NuGetDirectory = "c:/usr/var/nuget"

# Ensure the directories exist
New-Item -ItemType Directory -Path $NuGetDirectory -Force | Out-Null

Write-Host "Setting up .NET environment..."
dotnet --version

$timestamp = Get-Date -Format "yyyyMMdd.HHmm"

Write-Host "Determining version with MinVer..."
$Version = $(minver)
Write-Host "Version determined: $Version"

Write-Host "Building the solution..."
dotnet build steelkilt-sharp.sln --configuration Debug

Write-Host "Packing SteelkiltSharp..."
dotnet pack Steelkilt --configuration Debug --output $NuGetDirectory /p:Version=$Version

Write-Host "Running tests..."
dotnet test SteelkiltSharp.Tests.csproj --configuration Debug

Write-Host "NuGet Packages are ready in: $NuGetDirectory"

Write-Host "Listing NuGet packages:"
Get-ChildItem $NuGetDirectory/*.nupkg
