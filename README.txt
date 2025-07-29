# Avalonia UI Agent with FlaUI

## Setup
1. Update the path in `FlaUIService.cs` to your Avalonia app EXE.
2. Run: `dotnet restore`
3. Run: `dotnet run`

## API Endpoints
- POST /launch — Launches the Avalonia app.
- POST /click?buttonText=Start — Clicks a button by its visible text.
- GET /readLabel?labelAutomationId=statusLabel — Reads label text.