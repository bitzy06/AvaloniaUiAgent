# Avalonia UI Agent with FlaUI and Economy-Sim Integration

## Overview
This is a comprehensive UI automation agent that provides both basic FlaUI automation and deep integration with the Economy-Sim application. It offers full control over the economy simulation game and can read any data required during runtime, including graphics and game state. Additionally, it features advanced analytics, AI-powered recommendations, and sophisticated automation capabilities.

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

### Advanced Analytics and AI
- **Historical Trend Analysis**: Track game state changes over time
- **Economic Health Scoring**: Calculate economic performance metrics
- **Performance Monitoring**: Monitor game state changes and calculate growth rates
- **Event Detection**: Automatically detect significant game events
- **AI Recommendations**: Get AI-powered suggestions for optimal gameplay

### Automation and Scripting
- **Automation Scripts**: Create conditional scripts that execute based on game state triggers
- **Macro Execution**: Execute complex command sequences with customizable delays
- **AI Script Generation**: Automatically generate automation scripts based on objectives
- **Background Automation Engine**: Continuously monitor and execute automation rules
- **Pre-built Scripts**: Includes default scripts for economic growth and trade balance

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

### Analytics and AI
- `POST /economy-sim/analytics/start-monitoring?intervalMs=5000` — Start monitoring game state
- `GET /economy-sim/analytics/comprehensive` — Get comprehensive game analytics
- `GET /economy-sim/analytics/performance-metrics` — Get performance metrics over time
- `GET /economy-sim/analytics/game-events` — Detect recent game events
- `GET /economy-sim/analytics/ai-recommendations` — Get AI-powered recommendations

### Automation and Scripting
- `POST /economy-sim/automation/register-script` — Register new automation script
- `POST /economy-sim/automation/execute-script?scriptName=name` — Execute specific script
- `POST /economy-sim/automation/start-engine` — Start automation engine
- `POST /economy-sim/automation/stop-engine` — Stop automation engine
- `GET /economy-sim/automation/scripts` — Get all registered scripts
- `POST /economy-sim/automation/execute-macro` — Execute macro sequence
- `POST /economy-sim/automation/create-ai-script?objective=goal` — Create AI automation script

## Automation Script Examples

### Conditional Automation Script
```json
{
  "name": "AutoTrade",
  "description": "Automatically create export deals when treasury is high",
  "isEnabled": true,
  "triggers": [
    {
      "type": "Treasury",
      "operator": "GreaterThan", 
      "value": "2000000"
    }
  ],
  "actions": [
    {
      "type": "ExecuteCommand",
      "parameters": {
        "commandType": "trade",
        "action": "create_export",
        "item": "Steel"
      }
    }
  ]
}
```

### Macro Sequence
```json
{
  "name": "InfrastructureBuild",
  "description": "Build multiple infrastructure projects",
  "delayBetweenCommandsMs": 2000,
  "commands": [
    {
      "commandType": "construction",
      "action": "build_factory"
    },
    {
      "commandType": "construction", 
      "action": "build_road"
    }
  ]
}
```

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
- **Historical Analytics**: Trend analysis, performance metrics, event detection
- **AI Intelligence**: Recommendations, automated script generation

## Advanced Features

### AI-Powered Automation
- Automatically generate scripts based on objectives
- AI recommendations for optimal gameplay strategies
- Intelligent event detection and response

### Performance Analytics
- Track economic growth rates over time
- Monitor population and treasury changes
- Calculate diplomatic and trade effectiveness

### Background Automation
- Continuously monitor game state
- Execute conditional automation scripts
- Maintain historical data for analysis

## Usage Examples
See `AvaloniaUiAgent.http` for comprehensive examples of all available endpoints.

## Requirements
- .NET 8.0
- Windows environment (for FlaUI)
- Economy-Sim application installed and accessible

## Architecture
The system consists of five main service layers:
1. **FlaUIService**: Basic UI automation
2. **EconomySimService**: Core game integration
3. **EconomySimGraphicsService**: Visual data access
4. **EconomySimAnalyticsService**: Analytics and AI
5. **EconomySimAutomationService**: Scripting and automation