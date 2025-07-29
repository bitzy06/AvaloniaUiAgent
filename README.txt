# Avalonia UI Agent with FlaUI and Economy-Sim Integration

## Overview
This is a comprehensive UI automation agent that provides both basic FlaUI automation and deep integration with the Economy-Sim application. It offers full control over the economy simulation game and can read any data required during runtime, including graphics and game state.

## Features

### Basic FlaUI Automation
- Launch Avalonia applications
- Click buttons by text
- Read label text by automation ID

### Economy-Sim Deep Integration
- **Game State Access**: Real-time access to treasury, population, GDP, unemployment, inflation data
- **Diplomatic Relations**: Read and manage diplomatic relations with other countries
- **Trade Management**: Access import/export data and create trade deals
- **Construction Control**: Monitor and manage construction projects (factories, roads, bridges, ports, airports)
- **Policy Management**: Execute policy commands and governmental actions
- **Map Control**: Change map views (Political, Terrain, Economy, Population)

### Graphics and Visual Data Access
- **Map Screenshots**: Capture screenshots of the current map view
- **UI Element Capture**: Take screenshots of specific UI elements
- **Color Analysis**: Analyze map colors to understand regions and boundaries
- **UI Overlay Information**: Get information about HUD elements and active popups
- **Map View Information**: Access current zoom level, center coordinates, and view type

## Setup
1. Configure the economy-sim executable path in `appsettings.json`
2. Run: `dotnet restore`
3. Run: `dotnet run`

## Configuration
Edit `appsettings.json` to configure the Economy-Sim integration:

```json
{
  "EconomySim": {
    "ExecutablePath": "C:\\path\\to\\Economy-sim.exe",
    "WorkingDirectory": "C:\\path\\to\\Economy-sim\\",
    "LaunchArguments": "",
    "InitializationTimeoutMs": 10000
  }
}
```

## API Endpoints

### Basic FlaUI Automation
- `POST /launch` — Launches the Economy-Sim application
- `POST /click?buttonText=ButtonName` — Clicks a button by its visible text
- `GET /readLabel?labelAutomationId=labelId` — Reads label text by automation ID

### Economy-Sim Integration
- `POST /economy-sim/initialize` — Initialize the economy-sim service
- `GET /economy-sim/game-state` — Get current game state (treasury, population, etc.)
- `GET /economy-sim/diplomatic-relations` — Get diplomatic relations data
- `GET /economy-sim/trade-data` — Get trade data (imports/exports)
- `GET /economy-sim/construction-projects` — Get construction projects
- `POST /economy-sim/execute-command` — Execute game commands (diplomacy, trade, construction, policy)
- `POST /economy-sim/change-map-view?viewType=political` — Change map view

### Graphics and UI Data
- `GET /economy-sim/graphics/screenshot` — Capture map screenshot
- `GET /economy-sim/graphics/map-view-info` — Get map view information
- `GET /economy-sim/graphics/ui-overlay-info` — Get UI overlay information
- `GET /economy-sim/graphics/color-analysis` — Analyze map colors
- `GET /economy-sim/graphics/element-screenshot?elementName=element` — Capture UI element screenshot

## Game Command Examples

### Diplomacy Commands
```json
{
  "commandType": "diplomacy",
  "action": "propose_treaty",
  "parameters": {
    "targetCountry": "United Kingdom"
  }
}
```

### Trade Commands
```json
{
  "commandType": "trade",
  "action": "create_export",
  "parameters": {
    "item": "Steel",
    "quantity": "1000",
    "partner": "France"
  }
}
```

### Construction Commands
```json
{
  "commandType": "construction",
  "action": "build_factory",
  "parameters": {
    "factoryType": "Steel Mill",
    "location": "Detroit"
  }
}
```

## Data Access
The integration provides structured access to all game data:

- **Real-time Game State**: Treasury, population, date, player role, controlled entity
- **Economic Indicators**: GDP, unemployment rate, inflation rate
- **Visual Data**: Map screenshots, UI element positions, color analysis
- **Game Objects**: Diplomatic relations, trade deals, construction projects

## Usage Examples
See `AvaloniaUiAgent.http` for comprehensive examples of all available endpoints.

## Requirements
- .NET 8.0
- Windows environment (for FlaUI)
- Economy-Sim application installed and accessible