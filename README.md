Min inlämningsuppgift för kursen "Utveckling mot databaser och databasadministration".

# Labb 2 – Bibliotek
Du ska bygga ett webb-API som i framtiden ska användas till för att bygga ett online-bibliotek. Den nuvarande
tabellen har tagits fram av de anställda på biblioteket och syns nedan. Den behöver då normaliseras.<br/>
* Kraven för biblioteket
  * Databasen ska kunna lagra böcker och vem som författat dessa.
  * En bok kan ha era författare.
  * Flera av samma bok kan nnas i biblioteket.
  * Databasen ska kunna registrera en bok som utlånad och när den lämnats tillbaka.

## Uppgift G-krav
Ett ASP.NET Core Webb-API ska sedan skapas där du med Entity Framework genererar databasen utifrån dina
entiteter i din modell. _Tips. Ett ER-diagram över tabellerna där primary keys, foreign keys och kopplingstabeller
finns kan underlätta designfasen men är inget krav._<br/>
* Endpoints för följande funktionalitet i Webb-APIet ska finnas
  * Skapa en författare
  * Skapa en bok
  * Skapa en ny låntagare
  * Lista alla böcker
  * Hämta information om en specik bok
  * Låna en bok
  * Lämna tillbaka en bok
  * Ta bort låntagare
  * Ta bort böcker
  * Ta bort författare
* Kongurera projektet så att APIet ansluter mot en SQLServer databas som du också publicerat på Azure.
  * ConnectionStrings ska ha Azure connectionsträngen i appsettings.json respektive appsettings.Development.json och vara namngiven till BooksDb.
* Filen TestEndpoints.ps1 ska nnas, jämte projektet, som kör Invoke-RestMethod mot respektive endpoint

## Uppgift VG-krav
* Formulera frågor att ställa till beställaren för att få behövlig information så att du ska kunna välja optimal typ av databas för projektet.
* Motivera varför du ställer respektive fråga och hur svaret hjälper dig göra valet av optimal databas.
* Analysen ska innehålla 100-200 ord.
