# Fronta Práce – multiplatformní řešení (Windows + Android)

Tento balíček obsahuje:
- **FrontaPrace.Api** – ASP.NET Core 8 Web API napojené na MS SQL Server 2022 (čte `VIEWMANTA_WorkQueue`).
- **FrontaPrace.App** – .NET MAUI aplikace pro Windows a Android (responsivní UI, QR sken přes kameru, příprava pro NFC/RFID).

## Rychlý start

1) Otevřete solution `FrontaPrace.sln` ve Visual Studio 2022.
2) V projektu **FrontaPrace.Api** nastavte `appsettings.json` (ConnectionStrings:Sql).
3) Spusťte **FrontaPrace.Api** (zobrazí se Swagger).
4) V projektu **FrontaPrace.App** upravte `Resources/Raw/appsettings.json` (`ApiBaseUrl` na URL API).
5) Spusťte MAUI aplikaci (Windows/Android).

### NuGet balíčky (MAUI)
- `CommunityToolkit.Mvvm` (MVVM)
- `ZXing.Net.MAUI` (QR/čárové kódy)
- `Plugin.NFC` (NFC/RFID – potřeba přidat Android permission a otestovat s vaším čtečkovým HW/SDK)

### Poznámky k RFID
- Pro LF/HF/UHF čtečky se typicky používá vendor SDK (Zebra, Honeywell, Chainway aj.).
- Do aplikace lze integrovat přes `DependencyService`/`partial` třídu s `#if ANDROID` a obsluhu intentů/SDK callbacků.
- Základní NFC (13.56 MHz, NDEF) je podporováno přes `Plugin.NFC`.

### Bezpečnost & výkon
- API pracuje jen s **viewem** a používá parametrizaci.
- Pro mobilní přístup **nedoporučujeme** přímé spojení na SQL – používejte toto API.
- V produkci zapojte JWT/OAuth2, rate limiting, logging, cachování a RCSI na SQL.