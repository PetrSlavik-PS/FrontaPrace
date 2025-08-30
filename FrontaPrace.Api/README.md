# FrontaPrace.Api

ASP.NET Core 8 Web API pro čtení dat z VIEWMANTA_WorkQueue.

## Spuštění
1) Upravte `appsettings.json` (ConnectionStrings:Sql).
2) `dotnet restore`
3) `dotnet run`
4) Otevřete `https://localhost:5001/swagger`.

> API používá `WITH (NOLOCK)` stejně jako view. Pro produkci zvažte RCSI/SPR a audit.