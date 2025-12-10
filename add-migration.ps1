Param(
    [Parameter(Mandatory=$True)]
    [string]$Name
)
#dotnet tool update --global dotnet-ef
Push-Location
cd src/GtPrax.Infrastructure
dotnet ef migrations add $Name -o Database/Migrations --startup-project ../GtPrax.WebApp/
Pop-Location
