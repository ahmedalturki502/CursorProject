# Simple API Test Script
$baseUrl = "http://localhost:5000"

Write-Host "Testing E-Commerce API endpoints..." -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Yellow
Write-Host ""

# Test 1: Check if application is running
Write-Host "1. Testing if application is running..." -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/swagger" -Method GET -TimeoutSec 5
    Write-Host "   ✅ Application is running!" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Application is not running: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Test categories endpoint
Write-Host "2. Testing categories endpoint..." -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/categories" -Method GET -TimeoutSec 5
    Write-Host "   ✅ Categories endpoint works!" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Categories endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Test products endpoint
Write-Host "3. Testing products endpoint..." -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/products" -Method GET -TimeoutSec 5
    Write-Host "   ✅ Products endpoint works!" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Products endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Test registration endpoint
Write-Host "4. Testing registration endpoint..." -ForegroundColor Cyan
$registerBody = @{
    fullName = "Test User"
    email = "test@example.com"
    password = "Test123!"
    confirmPassword = "Test123!"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" -Method POST -Body $registerBody -ContentType "application/json" -TimeoutSec 5
    Write-Host "   ✅ Registration endpoint works!" -ForegroundColor Green
    $token = $response.token
    Write-Host "   Token received: $($token.Substring(0, 20))..." -ForegroundColor Gray
} catch {
    Write-Host "   ❌ Registration endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Basic API testing completed!" -ForegroundColor Green
