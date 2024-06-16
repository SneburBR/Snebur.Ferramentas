#Path Visual Studio 2022 - Community
$exePath = "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe"

# Define the registry path
$regPath = "HKCU:\Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers"

# Check if the registry key already exists
$currentValue = Get-ItemProperty -Path $regPath -Name $exePath -ErrorAction SilentlyContinue

if ($currentValue.$exePath -eq "RUNASADMIN") {
    Write-Host "The registry key is already set to run $exePath as administrator."
    return
}
 
# Set the registry key value
Set-ItemProperty -Path $regPath -Name $exePath -Value "RUNASADMIN"

# Verify the registry key value
$updatedValue = Get-ItemProperty -Path $regPath -Name $exePath -ErrorAction SilentlyContinue

if ($updatedValue.$exePath -eq "RUNASADMIN") {
    Write-Host "Registry updated to run Visual Studio 2022 as administrator."
}
else {
    Write-Host "Failed to update the registry key for $exePath."
}
 