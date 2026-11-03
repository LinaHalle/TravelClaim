# TravelClaim – Reseräkning och traktamente

## Om projektet

TravelClaim är en enkel webbapplikation för att registrera tjänsteresor och beräkna skattefritt respektive skattepliktigt traktamente enligt Skatteverkets regler för inrikes tjänsteresor.

Projektet utvecklades som en tidsbegränsad uppgift med fokus på att implementera beräkningslogik, datamodeller, grundläggande lagring och ett fungerande webbgränssnitt.

## Teknikval

- **C# och .NET 10** – backend och applikationslogik.
- **ASP.NET Core Razor Pages** – webbgränssnitt och formulärhantering.
- **Entity Framework Core** – hantering av databas och lagring.
- **SQLite** – lokal databas som är enkel att sätta upp och passar projektets omfattning.
- **xUnit** – automatiserade enhetstester av beräkningslogiken.

Jag har sedan tidigare erfarenhet av React och frontendutveckling, men valde att använda Razor Pages i det här projektet. Eftersom tekniken var ny för mig såg jag det som en möjlighet att lära mig ett annat sätt att bygga webbapplikationer. Med den begränsade tiden kunde jag hålla formulär, sidhantering och backendlogik samlade i samma projekt.

## Implementerad funktionalitet

Applikationen innehåller följande funktioner:

- Registrering av tjänsteresor med uppgifter om resenär, destination, syfte, adresser samt avrese- och hemkomsttid.
- Kontroll av grundförutsättningar för skattefritt traktamente, bland annat övernattning och avstånd till bostad och arbetsplats.
- Beräkning av hel- och halvdagstraktamente utifrån avrese- och hemkomsttid.
- Hantering av reducerade schablonbelopp efter tre månader respektive två år.
- Avdrag för arbetsgivarbetalda måltider.
- Beräkning av nattraktamente när arbetsgivaren inte har betalat logi.
- Uppdelning av utbetalt traktamente i skattefri och skattepliktig del.
- Lagring av reseräkningar och dagliga beräkningar i en SQLite-databas.
- Visning av tidigare registrerade reseräkningar och deras beräkningsunderlag.

Beräkningarna använder schablonbelopp för inkomstår 2026.

## Komma igång

### Förutsättningar

- .NET 10 SDK
- Entity Framework Core CLI (`dotnet-ef`) för databashantering

### Installera och starta

Klona projektet och gå till projektets rotmapp:

```bash
git clone git@github.com:LinaHalle/TravelClaim.git
cd TravelClaim
```

Återställ paketen:

```bash
dotnet restore
```

Uppdatera databasen med projektets EF Core-migrationer:

```bash
dotnet ef database update --project TravelClaim.Web
```

Starta webbapplikationen:

```bash
dotnet run --project TravelClaim.Web
```

Öppna sedan den lokala adress som visas i terminalen.

### Köra tester

Kör enhetstesterna från projektets rotmapp:

```bash
dotnet test
```

Testerna använder xUnit och täcker bland annat måltidsavdrag, hel- och halvdagar, längre tjänsteresor, övernattning och uppdelning mellan skattefria och skattepliktiga belopp.

## Källa och regelverk

Beräkningslogiken utgår från Skatteverkets information om traktamente och kostnadsersättningar vid inrikes tjänsteresor.

Källor:

- [Skatteverket – Traktamente för arbetsgivare](https://www.skatteverket.se/foretagochorganisationer/arbetsgivare/lonochersattning/traktamente.4.361dc8c15312eff6fd1703e.html)
- [Skatteverket – Traktamente för privatpersoner](https://www.skatteverket.se/privat/skatter/arbeteinkomst/traktamente.4.dfe345a107ebcc9baf80006547.html)
- [Skatteverket – Belopp och procent för inkomstår 2026](https://www.skatteverket.se/privat/skatter/beloppochprocent/2026/beloppochprocent2026kortversion.4.1522bf3f19aea8075ba21.html)

### Schablonbelopp för inrikes tjänsteresor 2026

| Typ av traktamente | Belopp |
|---|---:|
| Hel dag | 300 kr |
| Halv dag | 150 kr |
| Hel dag efter 3 månader | 210 kr |
| Hel dag efter 2 år | 150 kr |
| Nattraktamente | 150 kr per natt |

### Måltidsavdrag

Måltidsavdrag görs när arbetsgivaren har betalat måltider under tjänsteresan.

| Måltid | Hel dag (300 kr) | Halv dag (150 kr) |
|---|---:|---:|
| Frukost | 60 kr | 30 kr |
| Lunch | 105 kr | 53 kr |
| Middag | 105 kr | 53 kr |
| Lunch och middag | 210 kr | 105 kr |
| Alla måltider | 270 kr | 135 kr |

För reducerade heldagsbelopp efter tre månader respektive två år används följande avdrag:

| Måltid | Efter 3 månader (210 kr) | Efter 2 år (150 kr) |
|---|---:|---:|
| Frukost | 42 kr | 30 kr |
| Lunch | 74 kr | 53 kr |
| Middag | 74 kr | 53 kr |
| Lunch och middag | 147 kr | 105 kr |
| Alla måltider | 189 kr | 135 kr |

Efter tre månader och två år gäller de reducerade schablonbeloppen endast för hela dagar. Halvdagar ger då inget skattefritt traktamente.

Beloppen är hårdkodade i beräkningslogiken och behöver uppdateras om Skatteverkets schablonbelopp ändras.

## Begränsningar och vidareutveckling

På grund av den begränsade utvecklingstiden prioriterades beräkningslogik och grundläggande funktionalitet framför ett mer omfattande gränssnitt och fler funktioner.

Följande delar hann inte implementeras:

- **Milersättning:** Beräkning av ersättning vid tjänsteresor med egen bil eller förmånsbil.
- **Sökning och filtrering:** Det går att se tidigare registrerade resor, men inte att söka efter eller filtrera dem.
- **Detaljhantering av sparade resor:** Möjlighet att öppna, redigera eller ta bort en tidigare registrerad reseräkning.
- **Export:** Exempelvis export av reseräkningar till PDF eller Excel.
- **Mer avancerad måltidshantering:** Särskilda regler för exempelvis obligatorisk hotellfrukost, måltider som ingår i transportbiljetter och representation hanteras inte separat.
- **Mer detaljerad logihantering:** Applikationen utgår från att varje natt mellan avresa och hemkomst är en övernattning. Den hanterar inte enskilda nätter eller kontrollerar separat om resan pågått mellan 00.00 och 06.00.
- **Mer fullständig hantering av längre tjänsteresor:** Beräkningen utgår från resans startdatum och hanterar inte avbrott eller förändringar av tjänstestället.
- **Förbättrat gränssnitt:** Gränssnittet är avsiktligt enkelt. Mer tid hade kunnat läggas på design, responsivitet och användarupplevelse.
- **Utökad testtäckning:** De viktigaste delarna av beräkningslogiken täcks av enhetstester, men fler tester skulle kunna läggas till för ytterligare kombinationer av måltider, reslängder och avrese-/hemkomsttider. Även gränsfall, validering av formulärdata och integrationen med databasen skulle kunna testas mer omfattande.

Projektet är därför en förenklad implementation av reglerna för inrikes traktamente och ska inte ses som ett komplett lönesystem eller ett fullständigt stöd för alla situationer som Skatteverket beskriver.

## Användning av AI

ChatGPT användes som ett stöd under utvecklingen, framför allt eftersom ASP.NET Core Razor Pages och Entity Framework Core var nya tekniker för mig.

AI-stödet användes bland annat för att:

- Få hjälp att förstå hur projektet kunde struktureras med Razor Pages.
- Resonera kring datamodeller, ansvarsfördelning och beräkningslogik.
- Få förklaringar av hur C# samverkar med Entity Framework Core.
- Effektivisera arbetet med kodstruktur och felsökning.
- Ta fram förslag på enhetstester och identifiera edge cases.

AI-genererade förslag granskades och testades innan de användes. Jag använde inte förslagen okritiskt, utan kontrollerade även att beräkningsreglerna stämde med Skatteverkets information.

### Exempel på när jag ifrågasatte AI

Ett konkret exempel var beräkningen av halvdagstraktamente vid längre tjänsteresor.

ChatGPT föreslog initialt att halvdagstraktamente skulle beräknas till reducerade belopp även efter tre månader och två år. Jag ifrågasatte förslaget eftersom jag inte kunde hitta stöd för det i Skatteverkets regler.

Efter kontroll mot Skatteverkets information korrigerades logiken: efter tre månader respektive två år gäller de reducerade schablonbeloppen per hel dag, och halvdagar ger då inte något skattefritt traktamente.

Detta blev också ett testfall i projektets enhetstester.

Exemplet visar hur jag använde AI som ett verktyg för att resonera och arbeta effektivare, samtidigt som jag själv granskade förslagen och tog ansvar för att implementationen följde källmaterialet.

Ett återkommande exempel var att jag granskade och ifrågasatte förslag på kodstruktur. I vissa fall föreslog jag att slå ihop funktionalitet för att minska kodupprepning och undvika onödigt många metoder. I andra fall föreslog jag i stället att dela upp metoder för att göra koden tydligare och enklare att testa separat. Målet var att hitta en balans mellan enkel kodstruktur, återanvändbarhet och testbarhet, snarare än att automatiskt följa AI:ns förslag.