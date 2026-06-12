# test_cms_sync.ps1
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession

# 1. Fetch Login Page to get Anti-Forgery Token
Write-Host "Fetching Login page..."
$loginPage = Invoke-WebRequest -Uri "http://localhost:5144/Account/Login" -SessionVariable 'session' -UseBasicParsing
$tokenMatch = [regex]::Match($loginPage.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"')
if (-not $tokenMatch.Success) {
    Write-Error "Could not find RequestVerificationToken on Login page."
    exit
}
$token = $tokenMatch.Groups[1].Value
Write-Host "Found Login AF Token: $token"

# 2. Login
Write-Host "Logging in..."
$loginBody = @{
    "email" = "superadmin@gfps.edu"
    "password" = "SuperAdmin@123"
    "rememberMe" = "false"
    "__RequestVerificationToken" = $token
}
$loginResponse = Invoke-WebRequest -Uri "http://localhost:5144/Account/Login" -Method Post -Body $loginBody -WebSession $session -UseBasicParsing
Write-Host "Login Status Code: $($loginResponse.StatusCode)"

# 3. Fetch Edit page of Facility 1 to get AF Token
Write-Host "Fetching Facility Edit Page..."
$editPage = Invoke-WebRequest -Uri "http://localhost:5144/Admin/Facilities/Edit/1" -WebSession $session -UseBasicParsing
$editTokenMatch = [regex]::Match($editPage.Content, 'name="__RequestVerificationToken" type="hidden" value="([^"]+)"')
if (-not $editTokenMatch.Success) {
    Write-Error "Could not find RequestVerificationToken on Edit page."
    exit
}
$editToken = $editTokenMatch.Groups[1].Value
Write-Host "Found Edit AF Token: $editToken"

# 4. Post Edit
Write-Host "Sending Edit request..."
$editBody = @{
    "Id" = "1"
    "Name" = "The Main Atrium - TEST"
    "Description" = "Luminous, high-ceiling reception and collaboration zone representing our modern corporate academic visual design."
    "Details" = "Constructed with low-emission glass and custom academic paneling, the atrium serves as the campus hub where students collaborate, study, and hold exhibitions. Fully air-conditioned and Wi-Fi enabled."
    "Icon" = "domain"
    "DisplayOrder" = "1"
    "__RequestVerificationToken" = $editToken
}
$editResponse = Invoke-WebRequest -Uri "http://localhost:5144/Admin/Facilities/Edit/1" -Method Post -Body $editBody -WebSession $session -UseBasicParsing
Write-Host "Edit Response Status Code: $($editResponse.StatusCode)"

# 5. Fetch Public Facilities Page to verify
Write-Host "Fetching Public Facilities Page..."
$publicPage = Invoke-WebRequest -Uri "http://localhost:5144/Home/Facilities" -WebSession $session -UseBasicParsing
if ($publicPage.Content -like "*The Main Atrium - TEST*") {
    Write-Host "SUCCESS: Change was instantly reflected on the public page!"
} else {
    Write-Warning "FAILURE: Change was NOT reflected on the public page."
}
