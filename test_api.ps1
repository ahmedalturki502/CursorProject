# E-Commerce API Testing Script
# This script tests all endpoints to ensure they work correctly

$baseUrl = "https://localhost:7001"
$customerToken = ""
$adminToken = ""

Write-Host "🚀 Starting E-Commerce API Testing..." -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Yellow
Write-Host ""

# Function to make HTTP requests
function Invoke-TestRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [string]$Body = "",
        [string]$Token = "",
        [string]$TestName = ""
    )
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    if ($Token) {
        $headers["Authorization"] = "Bearer $Token"
    }
    
    try {
        if ($Body) {
            $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method $Method -Headers $headers -Body $Body -SkipCertificateCheck
        } else {
            $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method $Method -Headers $headers -SkipCertificateCheck
        }
        
        Write-Host "✅ $TestName - SUCCESS" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "❌ $TestName - FAILED: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Test 1: Authentication Endpoints
Write-Host "🔐 Testing Authentication Endpoints..." -ForegroundColor Cyan

# Register customer
$registerBody = @{
    fullName = "John Customer"
    email = "customer@example.com"
    password = "Customer123!"
    confirmPassword = "Customer123!"
} | ConvertTo-Json

$registerResponse = Invoke-TestRequest -Method "POST" -Endpoint "/api/auth/register" -Body $registerBody -TestName "Register Customer"
if ($registerResponse) {
    $customerToken = $registerResponse.token
    Write-Host "   Customer Token: $($customerToken.Substring(0, 20))..." -ForegroundColor Gray
}

# Login admin
$loginBody = @{
    email = "admin@example.com"
    password = "Admin123!"
} | ConvertTo-Json

$loginResponse = Invoke-TestRequest -Method "POST" -Endpoint "/api/auth/login" -Body $loginBody -TestName "Login Admin"
if ($loginResponse) {
    $adminToken = $loginResponse.token
    Write-Host "   Admin Token: $($adminToken.Substring(0, 20))..." -ForegroundColor Gray
}

Write-Host ""

# Test 2: Category Endpoints
Write-Host "📂 Testing Category Endpoints..." -ForegroundColor Cyan

Invoke-TestRequest -Method "GET" -Endpoint "/api/categories" -TestName "Get All Categories"
Invoke-TestRequest -Method "GET" -Endpoint "/api/categories/1" -TestName "Get Category by ID"

# Create category (admin only)
$categoryBody = @{
    name = "Test Category"
} | ConvertTo-Json

$categoryResponse = Invoke-TestRequest -Method "POST" -Endpoint "/api/categories" -Body $categoryBody -Token $adminToken -TestName "Create Category (Admin)"

Write-Host ""

# Test 3: Product Endpoints
Write-Host "🛍️ Testing Product Endpoints..." -ForegroundColor Cyan

Invoke-TestRequest -Method "GET" -Endpoint "/api/products" -TestName "Get All Products"
Invoke-TestRequest -Method "GET" -Endpoint "/api/products?page=1&pageSize=5" -TestName "Get Products with Pagination"
Invoke-TestRequest -Method "GET" -Endpoint "/api/products?categoryId=1" -TestName "Get Products by Category"
Invoke-TestRequest -Method "GET" -Endpoint "/api/products?search=smartphone" -TestName "Search Products"
Invoke-TestRequest -Method "GET" -Endpoint "/api/products/1" -TestName "Get Product by ID"

# Create product (admin only)
$productBody = @{
    name = "Test Product"
    description = "This is a test product"
    price = 99.99
    imageUrl = "https://via.placeholder.com/300x300?text=TestProduct"
    stockQuantity = 50
    categoryId = 1
} | ConvertTo-Json

$productResponse = Invoke-TestRequest -Method "POST" -Endpoint "/api/products" -Body $productBody -Token $adminToken -TestName "Create Product (Admin)"

Write-Host ""

# Test 4: Cart Endpoints
Write-Host "🛒 Testing Cart Endpoints..." -ForegroundColor Cyan

Invoke-TestRequest -Method "GET" -Endpoint "/api/cart" -Token $customerToken -TestName "Get Cart"

# Add product to cart
$addToCartBody = @{
    productId = 1
    quantity = 2
} | ConvertTo-Json

Invoke-TestRequest -Method "POST" -Endpoint "/api/cart/add" -Body $addToCartBody -Token $customerToken -TestName "Add Product to Cart"

# Add another product
$addToCartBody2 = @{
    productId = 2
    quantity = 1
} | ConvertTo-Json

Invoke-TestRequest -Method "POST" -Endpoint "/api/cart/add" -Body $addToCartBody2 -Token $customerToken -TestName "Add Another Product to Cart"

Invoke-TestRequest -Method "GET" -Endpoint "/api/cart" -Token $customerToken -TestName "Get Cart with Items"

# Update cart item
$updateCartBody = @{
    quantity = 3
} | ConvertTo-Json

Invoke-TestRequest -Method "PUT" -Endpoint "/api/cart/items/1" -Body $updateCartBody -Token $customerToken -TestName "Update Cart Item"

Write-Host ""

# Test 5: Order Endpoints
Write-Host "📦 Testing Order Endpoints..." -ForegroundColor Cyan

# Place order
$orderBody = @{
    shippingAddress = "123 Main Street, City, State 12345"
} | ConvertTo-Json

$orderResponse = Invoke-TestRequest -Method "POST" -Endpoint "/api/orders" -Body $orderBody -Token $customerToken -TestName "Place Order"

Invoke-TestRequest -Method "GET" -Endpoint "/api/orders" -Token $customerToken -TestName "Get User Orders"
Invoke-TestRequest -Method "GET" -Endpoint "/api/orders/1" -Token $customerToken -TestName "Get Specific Order"
Invoke-TestRequest -Method "GET" -Endpoint "/api/orders/admin/all" -Token $adminToken -TestName "Get All Orders (Admin)"
Invoke-TestRequest -Method "GET" -Endpoint "/api/orders/admin/all?customerEmail=customer" -Token $adminToken -TestName "Get Orders with Filter (Admin)"

Write-Host ""

# Test 6: Error Testing
Write-Host "⚠️ Testing Error Scenarios..." -ForegroundColor Cyan

# Try to register with existing email
$duplicateBody = @{
    fullName = "Duplicate User"
    email = "customer@example.com"
    password = "Password123!"
    confirmPassword = "Password123!"
} | ConvertTo-Json

Invoke-TestRequest -Method "POST" -Endpoint "/api/auth/register" -Body $duplicateBody -TestName "Register with Existing Email (Should Fail)"

# Try to login with wrong password
$wrongPasswordBody = @{
    email = "customer@example.com"
    password = "WrongPassword123!"
} | ConvertTo-Json

Invoke-TestRequest -Method "POST" -Endpoint "/api/auth/login" -Body $wrongPasswordBody -TestName "Login with Wrong Password (Should Fail)"

# Try to access protected endpoint without token
Invoke-TestRequest -Method "GET" -Endpoint "/api/cart" -TestName "Access Protected Endpoint Without Token (Should Fail)"

# Try to access admin endpoint with customer token
$unauthorizedProductBody = @{
    name = "Unauthorized Product"
    description = "This should fail"
    price = 99.99
    imageUrl = "https://example.com/image.jpg"
    stockQuantity = 10
    categoryId = 1
} | ConvertTo-Json

Invoke-TestRequest -Method "POST" -Endpoint "/api/products" -Body $unauthorizedProductBody -Token $customerToken -TestName "Access Admin Endpoint with Customer Token (Should Fail)"

Write-Host ""

# Test 7: Cleanup
Write-Host "🧹 Testing Cleanup Operations..." -ForegroundColor Cyan

Invoke-TestRequest -Method "DELETE" -Endpoint "/api/cart/items/2" -Token $customerToken -TestName "Remove Cart Item"
Invoke-TestRequest -Method "DELETE" -Endpoint "/api/cart/clear" -Token $customerToken -TestName "Clear Cart"
Invoke-TestRequest -Method "GET" -Endpoint "/api/cart" -Token $customerToken -TestName "Get Empty Cart"
Invoke-TestRequest -Method "POST" -Endpoint "/api/auth/logout" -Token $customerToken -TestName "Logout"

Write-Host ""
Write-Host "🎉 API Testing Complete!" -ForegroundColor Green
Write-Host "Check the results above to see which endpoints are working correctly." -ForegroundColor Yellow
