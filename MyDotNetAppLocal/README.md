# Väderapp

En modern väderapplikation byggd med .NET 10 och distribuerad till Azure.

## Funktioner

- 🌤️ Modern, responsiv design med gradientbakgrund
- 🌡️ Nuvarande väderinformation (temperatur, känns som, fuktighet, vind, tryck)
- 📅 5-dagars väderprognos
- 🔍 Sök väder för valfri stad
- 🎨 Väderikoner från Font Awesome
- 📱 Mobilanpassad design
- 🔄 Riktig väderdata (med OpenWeatherMap API) eller mock-data för demo

## Teknisk stack

- **Backend**: .NET 10, ASP.NET Core Web API
- **Frontend**: HTML5, CSS3, JavaScript (ES6+)
- **Styling**: CSS Grid, Flexbox, Gradienter
- **Ikoner**: Font Awesome
- **Deployment**: Azure Web Apps, Docker
- **Väderdata**: OpenWeatherMap API

## Kom igång

### Förutsättningar
- .NET 10 SDK
- Visual Studio 2022 eller VS Code

### Lokal körning

1. Klona repositoriet
2. Navigera till projektmappen
3. Kör applikationen:

```bash
dotnet run
```

4. Öppna `https://localhost:7XXX` i din webbläsare

### Konfigurera riktig väderdata (valfritt)

1. Skaffa en gratis API-nyckel från [OpenWeatherMap](https://openweathermap.org/api)
2. Lägg till din API-nyckel i `appsettings.json`:

```json
{
  "OpenWeatherMap": {
    "ApiKey": "DIN_API_NYCKEL_HÄR"
  }
}
```

3. Starta om applikationen för att använda riktig väderdata

## Projektstruktur

```
MyDotNetAppLocal/
├── Controllers/
│   └── WeatherController.cs    # API-endpoints för väderdata
├── wwwroot/
│   ├── index.html              # Huvudsida
│   ├── css/
│   │   └── style.css           # Modern CSS med gradienter och animationer
│   └── js/
│       └── app.js              # JavaScript för interaktivitet
├── Program.cs                  # Applikationsstart och middleware
├── appsettings.json            # Konfiguration
├── Dockerfile                  # Docker-konfiguration
└── azure.yaml                  # Azure deployment konfiguration
```

## API Endpoints

- `GET /api/weather/current?location={stad}` - Nuvarande väder
- `GET /api/weather/forecast?location={stad}` - 5-dagars prognos
- `GET /weatherforecast` - Legacy endpoint (mock-data)

## Deployment till Azure

### Alternativ 1: Azure CLI

```bash
# Skapa resursgrupp
az group create --name weatherapp-rg --location westeurope

# Skapa app service plan
az appservice plan create --name weatherapp-plan --resource-group weatherapp-rg --sku B1 --is-linux

# Skapa web app
az webapp create --resource-group weatherapp-rg --plan weatherapp-plan --name weatherapp-unique-name --runtime "DOTNETCORE|10.0"

# Deploy
az webapp up --name weatherapp-unique-name --resource-group weatherapp-rg --location westeurope
```

### Alternativ 2: Visual Studio

1. Högerklicka på projektet
2. Välj "Publish"
3. Välj "Azure" och följ guiden

### Alternativ 3: GitHub Actions

1. Lägg till Azure credentials till GitHub secrets
2. Använd `.github/workflows/azure-deploy.yml` (kan skapas vid behov)

## Funktioner i detalj

### Design
- Modern gradientbakgrund (lila till blå)
- Glas-effekt med backdrop-filter
- Mjuka övergångar och hover-effekter
- Responsiv grid-layout
- Mobilanpassad med media queries

### Väderdata
- Temperatur i Celsius
- "Känns som"-temperatur
- Luftfuktighet i procent
- Vindhastighet i m/s
- Lufttryck i hPa
- Väderbeskrivningar på svenska

### Användarinteraktion
- Sökfält med enter-stöd
- Omedelbar visning av resultat
- Laddningsindikatorer
- Felmeddelanden vid problem

## Licens

Detta projekt är öppen källkod och får användas fritt.

## Bidrag

Välkommen att skapa issues och pull requests!
