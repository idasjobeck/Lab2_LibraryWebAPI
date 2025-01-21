
# This script is used to test the endpoints
param(
    [string]$environment = "Development",
    [string]$launchProfile = "https-Development",
    [string]$connectionStringKey = "BooksDb",
    [bool]$dropDatabase = $false,
    [bool]$createDatabase = $false
)


# function for displaying the result of the POST response
function DisplayResponseForPost {
    param ($StatusCode)

    if ($StatusCode -eq 201) {
        Write-Host "Item(s) created successfully"
    } elseif ($StatusCode -eq 200) {
        Write-Host "Item(s) already exists`n"
    } else {
        Write-Host "Item(s) not created`n"
    }
}

# function for displaying the result of the DELETE response
function DisplayResponseForDelete {
    param ($StatusCode)

    if ($StatusCode -eq 204) {
        Write-Host "Item(s) deleted successfully`n"
    } else {
        Write-Host "Item(s) not deleted`n"
    }
}

# function for displaying the result of the PUT response
function DisplayResponseForPut {
    param ($StatusCode)

    if ($StatusCode -eq 204) {
        Write-Host "Item(s) updated successfully`n"
    } else {
        Write-Host "Item(s) not updated`n"
    }
}


# Get the project name
$projectName = Get-ChildItem -Recurse -Filter "*.csproj" | Select-Object -First 1 | ForEach-Object { $_.Directory.Name } # Projectname can also be set manually

# Get the base URL of the project
$launchSettings = Get-Content -LiteralPath ".\$projectName\Properties\launchSettings.json" | ConvertFrom-Json
$baseUrl = ($launchSettings.profiles.$launchProfile.applicationUrl -split ";")[0] # Can also set manually -> $baseUrl = "https://localhost:7253"

#Install module SqlServer
if (-not (Get-Module -ErrorAction Ignore -ListAvailable SqlServer)) {
    Write-Verbose "Installing SqlServer module for the current user..."
    Install-Module -Scope CurrentUser SqlServer -ErrorAction Stop
}
Import-Module SqlServer

# Set the environment variable
$env:ASPNETCORE_ENVIRONMENT = $environment



# Read the connection string from appsettings.Development.json
$appSettings = Get-Content ".\$projectName\appsettings.$environment.json" | ConvertFrom-Json
$connectionString = $appSettings.ConnectionStrings.$connectionStringKey
Write-Host "Database Connection String: $connectionString" -ForegroundColor Blue


# Get the database name from the connection string
if ($connectionString -match "Database=(?<dbName>[^;]+)")
{
    $databaseName = $matches['dbName']
    Write-Host "Database Name: $databaseName" -ForegroundColor Blue
}else{
    Write-Host "Database Name not found in connection string" -ForegroundColor Red
    exit
}


# Check if the database exists
$conStringDbExcluded = $connectionString -replace "Database=[^;]+;", ""
$queryDbExists = Invoke-Sqlcmd -ConnectionString $conStringDbExcluded -Query "Select name FROM sys.databases WHERE name='$databaseName'"
if($queryDbExists){
    if($dropDatabase -or (Read-Host "Do you want to drop the database? (y/n)").ToLower() -eq "y") { 

        # Drop the database
        Invoke-Sqlcmd -ConnectionString $connectionString -Query  "USE master;ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE $databaseName;"
        Write-Host "Database $databaseName dropped." -ForegroundColor Green
    }
}

# Create the database from the model
if(Select-String -LiteralPath ".\$projectName\Program.cs" -Pattern "EnsureCreated()"){
    Write-Host "The project uses EnsureCreated() to create the database from the model." -ForegroundColor Yellow
} else {
    if($createDatabase -or (Read-Host "Should dotnet ef migrate and update the database? (y/n)").ToLower() -eq "y") { 

        dotnet ef migrations add "UpdateModelFromScript_$(Get-Date -Format "yyyyMMdd_HHmmss")" --project ".\$projectName\$projectName.csproj"
        dotnet ef database update --project ".\$projectName\$projectName.csproj"
    }
}

# Run the application
if((Read-Host "Start the server from Visual studio? (y/n)").ToLower() -ne "y") { 
    Start-Process -FilePath "dotnet" -ArgumentList "run --launch-profile $launchProfile --project .\$projectName\$projectName.csproj" -WindowStyle Normal    
    Write-Host "Wait for the server to start..." -ForegroundColor Yellow 
}

# Continue with the rest of the script
Read-Host "Press Enter to continue when the server is started..."



### =============================================================
### =============================================================
### =============================================================

# Send requests to the API endpoint




### Copy below code to test the endpoints




# ###

# ### ------------Post a movie


# Write-Host "`nCreate a movie"


# $httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"

# $endPoint = "$baseUrl/api/Movies"

# $json = '{ 
#     "Title": "The Usual Suspects", 
#     "ReleaseYear": 1995 
# }'

# $response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json"
# $response | Format-Table


# ### ------------ Query Movies from the database
# $sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Movies"
# $sqlResult | Format-Table




###

### ------------Post some authors

Write-Host "`n *** Create some authors (using /api/Authors) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Authors"

# Create array of authors
$authors = @(
    @{firstName = "Robert"; lastName = "Jordan"},
    @{firstName = "J.R.R."; lastName = "Tolkien"}
)

# Create an empty array to store the responses
$responseArray = @()
# Loop through the array and post each author to the API
foreach ($author in $authors) {
    $json = $author | ConvertTo-Json
    $response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
    $responseArray += $response
}
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$responseArray | Format-Table

### ------------ Query Authors from the database
Write-Host "SQL query result:" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "Select * FROM Authors"
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$responseArray = $null
$statusCode = $null

#Write-Host "" -BackgroundColor white
#Write-Host "" -NoNewline #-BackgroundColor # -BackgroundColor DarkYellow -ForegroundColor Black
# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""




###

### ------------Post some books

Write-Host "`n *** Create some books (using /api/Books) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books"
$bookSqlQuery = "SELECT Books.Id, Titles.TitleName AS Title, Series.SeriesName AS Series, Books.NumberInSeries, CONCAT_WS(' ', Authors.FirstName, Authors.LastName) AS Author, Genres.GenreName AS Genre, Books.ISBN, YEAR(Books.PublishedYear) AS PublishedYear, Publishers.PublisherName AS Publisher, Editions.EditionName AS Edition, Books.TotalQty, Books.AvailableQty
FROM Books
JOIN Titles ON Books.TitleId = Titles.Id
LEFT JOIN Series ON Books.SeriesId = Series.Id
JOIN AuthorBook ON Books.Id = AuthorBook.BookId
JOIN Authors ON AuthorBook.AuthorId = Authors.Id
JOIN Genres ON Books.GenreId = Genres.Id
JOIN Publishers ON Books.PublisherId = Publishers.Id
JOIN Editions ON Books.EditionId = Editions.Id"

# Create array of books
$books = @(
    @{title = "Armada"; series = $null; numberInSeries = $null; authors = @(@{firstName = "Ernest"; lastName = "Cline"}); genre = "Sci-fi"; isbn = "978-0-09-958674-6"; publishedYear = 2016; publisher = "Arrow Books"; edition = "Paperback"; totalQty = 2; availableQty = 2},
    @{title = "The Gathering Storm"; series = "The Wheel of Time"; numberInSeries = 12; authors = @(@{firstName = "Robert"; lastName = "Jordan"}, @{firstName = "Brandon"; lastName = "Sanderson"}); genre = "Fantasy"; isbn = "978-1-84149-232-2"; publishedYear = 2010; publisher = "Orbit"; edition = "Paperback"; totalQty = 2; availableQty = 2},
    @{title = "Ready Player One"; series = $null; numberInSeries = 0; authors = @(@{firstName = "Ernest"; lastName = "Cline"}); genre = "Sci-fi"; isbn = "978-0-09-956043-2"; publishedYear = 2012; publisher = "Arrow Books"; edition = "Paperback"; totalQty = 3; availableQty = 3},
    @{title = "The Fellowship of the Ring"; series = "The Lord of the Rings"; numberInSeries = 1; authors = @(@{firstName = "J.R.R."; lastName = "Tolkien"}); genre = "Fantasy"; isbn = "0-261-10235-1"; publishedYear = 1999; publisher = "Harper Collins"; edition = "Paperback"; totalQty = 1; availableQty = 1}
)

# Create an empty array to store the responses
$responseArray = @()
# Loop through the array and post each book to the API
foreach ($book in $books) {
    $json = $book | ConvertTo-Json
    $response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
    $responseArray += $response
}
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$responseArray | Format-Table -Property Id, Title, Series, NumberInSeries, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty, Authors

### ------------ Query Books from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $bookSqlQuery
$sqlResult | Format-Table -Wrap -Property Id, Title, Series, NumberInSeries, Author, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty

# Clear variables being reused
$response = $null
$responseArray = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""




###

### ------------Post some more books using additional endpoints

Write-Host "`n *** Create a book (using /api/Books/CreateBookWithAuthorId) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/CreateBookWithAuthorId"

$book = @{title = "The Eye of the World"; series = "The Wheel of Time"; numberInSeries = 1; authorIds = @(1); genre = "Fantasy"; isbn = "978-1-85723-076-5"; publishedYear = 1991; publisher = "Orbit"; edition = "Paperback"; totalQty = 2; availableQty = 2}

$json = $book | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table -Property Id, Title, Series, NumberInSeries, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty, Authors

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Create a book (using /api/Books/CreateBookWithIdsNewTitle) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/CreateBookWithIdsNewTitle"

$book = @{title = "A Memory of Light"; series = "The Wheel of Time"; numberInSeries = 14; authorIds = @(1, 4); genreId = 2; isbn = "978-1-84149-871-3"; publishedYear = 2013; publisherId = 2; editionId = 1; totalQty = 1; availableQty = 1}

$json = $book | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table -Property Id, Title, Series, NumberInSeries, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty, Authors

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Create a book (using /api/Books/CreateBookWithIdsNewEdition) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/CreateBookWithIdsNewEdition"

$book = @{titleId = 1; seriesId = $null; numberInSeries = $null; authorIds = @(3); genreId = 1; isbn = "978-1-780-89304-4"; publishedYear = 2015; publisher = "Century"; edition = "Hardback"; totalQty = 1; availableQty = 1}

$json = $book | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table -Property Id, Title, Series, NumberInSeries, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty, Authors

### ------------ Query Books from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $bookSqlQuery
$sqlResult | Format-Table -Wrap -Property Id, Title, Series, NumberInSeries, Author, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty

# Clear variables being reused
$response = $null
$responseArray = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Post another book

Write-Host "`n *** Create a book (using /api/Books) with existing ISBN - expected to produce an error (as ISBN must be unique) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books"

$book = @{title = "The Eye of the World"; series = "The Wheel of Time"; numberInSeries = 1; authors = @(@{firstName = "Robert"; lastName = "Jordan"}); genre = "Fantasy"; isbn = "978-1-85723-076-5"; publishedYear = 1991; publisher = "Orbit"; edition = "Paperback"; totalQty = 2; availableQty = 2}

$json = $book | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table -Property Id, Title, Series, NumberInSeries, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty, Authors

### ------------ Query Books from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $bookSqlQuery
$sqlResult | Format-Table -Wrap -Property Id, Title, Series, NumberInSeries, Author, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Post some users

Write-Host "`n *** Create a user (using /api/Users) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Users"

$user = @{firstName = "James"; lastName = "Carter"; email = "james.carter@email.com"}

$json = $user | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Create a user (using /api/Users) with an invalid email - expected to produce an error (as email format is checked) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Users"

$user = @{firstName = "Emily"; lastName = "Davis"; email = "emily.davis@email"}

$json = $user | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Create a user (using /api/Users) with an existing email - expected to produce an error (as email must be unique) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Users"

$user = @{firstName = "James"; lastName = "Carter"; email = "james.carter@email.com"}

$json = $user | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Create another two users (using /api/Users) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Users"

$users = @(
    @{firstName = "Emily"; lastName = "Davis"; email = "emily.davis@email.com"},
    @{firstName = "Laura"; lastName = "Wilson"; email = "laura.wilson@email.com"}
)

# Create an empty array to store the responses
$responseArray = @()
# Loop through the array and post each book to the API
foreach ($user in $users) {
    $json = $user | ConvertTo-Json
    $response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
    $responseArray += $response
}
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$responseArray | Format-Table

### ------------ Query Users from the database
Write-Host "SQL query result:" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "SELECT * FROM Users"
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$responseArray = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Get all books

Write-Host "`n *** Get all books (using /api/Books) ***" -ForegroundColor Yellow

$httpMethod = "Get"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod
Write-Host "Response from the API:" -ForegroundColor Green
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

### 

Write-Host "`n *** Get all books (using /api/Books/GetBooksWithIds) ***" -ForegroundColor Yellow

$httpMethod = "Get"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/GetBooksWithIds"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod
Write-Host "Response from the API:" -ForegroundColor Green
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Get a specific book

Write-Host "`n *** Get a specific book (using /api/Books/{id}) ***" -ForegroundColor Yellow

$httpMethod = "Get"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/2"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod
Write-Host "Response from the API:" -ForegroundColor Green
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Get a specific book (using /api/Books/GetBookWithId/{id}) ***" -ForegroundColor Yellow

$httpMethod = "Get"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/GetBookWithId/3"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod
Write-Host "Response from the API:" -ForegroundColor Green
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Post some loans

Write-Host "`n *** Create a loan (using /api/Loans) ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Loans"
$loanSqlQuery = "SELECT Loans.Id, Titles.TitleName AS Title, CONCAT_WS(' ', Users.FirstName, Users.LastName) AS [User], Users.CardNumber, Loans.LoanDate, Loans.ReturnedDate, Loans.Rating
FROM Loans
JOIN Books ON Loans.BookId = Books.Id
JOIN Titles ON Books.TitleId = Titles.Id
JOIN Users ON Loans.UserId = Users.Id"

$loan = @{title = "Ready Player One"; cardNumber = 900001}

$json = $loan | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

###

Write-Host "`n *** Create two loans (using /api/Loans/CreateLoanWithIds) - also shows one user can have multiple loans ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Loans/CreateLoanWithIds"

$loans = @(
    @{bookId = 4; userId = 2},
    @{bookId = 5; userId = 1}
)

# Create an empty array to store the responses
$responseArray = @()
# Loop through the array and post each book to the API
foreach ($loan in $loans) {
    $json = $loan | ConvertTo-Json
    $response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
    $responseArray += $response
}
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$responseArray | Format-Table

# Clear variables being reused
$response = $null
$responseArray = $null
$statusCode = $null

###

Write-Host "`n *** Create a loan (using /api/Loans) - expected to produce an error as there are no copies available for the book in the request ***" -ForegroundColor Yellow

$httpMethod = "Post"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Loans"

$loan = @{title = "The Fellowship of the Ring"; cardNumber = 900001}

$json = $loan | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPost $statusCode
$response | Format-Table

### ------------ Query Loans from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $loanSqlQuery
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Put a loan

Write-Host "`n *** Return a loan (using /api/Loans/ReturnLoan/{id}) ***" -ForegroundColor Yellow

$httpMethod = "Put"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Loans/ReturnLoan/1"

$loan = @{bookRating = 5}

$json = $loan | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPut $statusCode
$response | Format-Table

### ------------ Query Loans from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $loanSqlQuery
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Delete a user

Write-Host "`n *** Delete a user (using /api/Users/{id}) - expected to produce an error as the user has outstanding loans ***" -ForegroundColor Yellow

$httpMethod = "Delete"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Users/2"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForDelete $statusCode
$response | Format-Table

### ------------ Query Users from the database
Write-Host "SQL query result:" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "SELECT * FROM Users"
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Delete a book

Write-Host "`n *** Delete a book (using /api/Books/{id}) - expected to produce an error as the book has outstanding loans ***" -ForegroundColor Yellow

$httpMethod = "Delete"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/4"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForDelete $statusCode
$response | Format-Table

### ------------ Query Books from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $bookSqlQuery
$sqlResult | Format-Table -Wrap -Property Id, Title, Series, NumberInSeries, Author, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Put a loan

Write-Host "`n *** Return outstanding loan (using /api/Loans/ReturnLoan/{id}) for the user being deleted ***" -ForegroundColor Yellow

$httpMethod = "Put"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Loans/ReturnLoan/2"

$loan = @{bookRating = $null}

$json = $loan | ConvertTo-Json
$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -Body $json -ContentType "application/json" -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForPut $statusCode
$response | Format-Table

### ------------ Query Loans from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $loanSqlQuery
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Delete a user

Write-Host "`n *** Delete a user (using /api/Users/{id}) ***" -ForegroundColor Yellow

$httpMethod = "Delete"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Users/2"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForDelete $statusCode
$response | Format-Table

### ------------ Query Users from the database
Write-Host "SQL query result:" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "SELECT * FROM Users"
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""



###

### ------------Delete a book

Write-Host "`n *** Delete a book (using /api/Books/{id}) ***" -ForegroundColor Yellow

$httpMethod = "Delete"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Books/4"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForDelete $statusCode
$response | Format-Table

### ------------ Query Books from the database
Write-Host "SQL query result (the query contains joins on other tables):" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query $bookSqlQuery
$sqlResult | Format-Table -Wrap -Property Id, Title, Series, NumberInSeries, Author, Genre, ISBN, PublishedYear, Publisher, Edition, TotalQty, AvailableQty

# Clear variables being reused
$response = $null

# Continue with the rest of the script
Read-Host "Press Enter to continue..."
Write-Host "  ~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~  " -BackgroundColor white -ForegroundColor Black -NoNewline
Write-Host ""

$statusCode = $null
$statusCode = $null

###

### ------------Delete an author

Write-Host "`n *** Delete an author (using /api/Authors/{id}) - expected to produce an error as the author is associated with books ***" -ForegroundColor Yellow

$httpMethod = "Delete"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Authors/1"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForDelete $statusCode
$response | Format-Table

### ------------ Query Authors from the database
Write-Host "SQL query result:" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "SELECT * FROM Authors"
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null

### 

Write-Host "`n *** Delete an author (using /api/Authors/{id}) ***" -ForegroundColor Yellow

$httpMethod = "Delete"   ### "Get", "Post", "Put", "Delete"
$endPoint = "$baseUrl/api/Authors/2"

$response = Invoke-RestMethod -Uri $endPoint -Method $httpMethod -StatusCodeVariable statusCode
Write-Host "Response from the API:" -ForegroundColor Green
DisplayResponseForDelete $statusCode
$response | Format-Table

### ------------ Query Authors from the database
Write-Host "SQL query result:" -ForegroundColor Green
$sqlResult = Invoke-Sqlcmd -ConnectionString $connectionString -Query "SELECT * FROM Authors"
$sqlResult | Format-Table

# Clear variables being reused
$response = $null
$statusCode = $null


Write-Host "`n *** The script has now finished testing the endpoints. ***" -BackgroundColor Yellow -ForegroundColor Black -NoNewline
Write-Host ""

# Check if the database exists
#$conStringDbExcluded = $connectionString -replace "Database=[^;]+;", ""
$queryDbExists = Invoke-Sqlcmd -ConnectionString $conStringDbExcluded -Query "Select name FROM sys.databases WHERE name='$databaseName'"
if($queryDbExists){
    if($dropDatabase -or (Read-Host "Do you want to drop the database? (y/n)").ToLower() -eq "y") { 

        # Drop the database
        Invoke-Sqlcmd -ConnectionString $connectionString -Query  "USE master;ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE $databaseName;"
        Write-Host "Database $databaseName dropped." -ForegroundColor Green
    }
}