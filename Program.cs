using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<FlaUIService>();
builder.Services.AddSingleton<EconomySimService>();
builder.Services.AddSingleton<EconomySimGraphicsService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Original FlaUI endpoints
app.MapGet("/", () => "FlaUI Agent is running!");
app.MapPost("/launch", (FlaUIService flaUI) => flaUI.LaunchApp());
app.MapPost("/click", (FlaUIService flaUI, string buttonText) => flaUI.ClickButton(buttonText));
app.MapGet("/readLabel", (FlaUIService flaUI, string labelAutomationId) => flaUI.ReadLabelText(labelAutomationId));

// Economy Sim Integration endpoints
app.MapPost("/economy-sim/initialize", async (EconomySimService economyService) => 
    await economyService.InitializeAsync());

app.MapGet("/economy-sim/game-state", async (EconomySimService economyService) => 
    await economyService.GetGameStateAsync());

app.MapGet("/economy-sim/diplomatic-relations", async (EconomySimService economyService) => 
    await economyService.GetDiplomaticRelationsAsync());

app.MapGet("/economy-sim/trade-data", async (EconomySimService economyService) => 
    await economyService.GetTradeDataAsync());

app.MapGet("/economy-sim/construction-projects", async (EconomySimService economyService) => 
    await economyService.GetConstructionProjectsAsync());

app.MapPost("/economy-sim/execute-command", async (EconomySimService economyService, GameCommandDto command) => 
    await economyService.ExecuteGameCommandAsync(command));

app.MapPost("/economy-sim/change-map-view", async (EconomySimService economyService, string viewType) => 
    await economyService.ChangeMapViewAsync(viewType));

// Graphics and UI endpoints
app.MapGet("/economy-sim/graphics/screenshot", async (EconomySimGraphicsService graphicsService) => 
    await graphicsService.CaptureMapScreenshotAsync());

app.MapGet("/economy-sim/graphics/map-view-info", async (EconomySimGraphicsService graphicsService) => 
    await graphicsService.GetMapViewInfoAsync());

app.MapGet("/economy-sim/graphics/ui-overlay-info", async (EconomySimGraphicsService graphicsService) => 
    await graphicsService.GetUIOverlayInfoAsync());

app.MapGet("/economy-sim/graphics/color-analysis", async (EconomySimGraphicsService graphicsService) => 
    await graphicsService.AnalyzeMapColorsAsync());

app.MapGet("/economy-sim/graphics/element-screenshot", async (EconomySimGraphicsService graphicsService, string elementName) => 
    await graphicsService.CaptureUIElementScreenshotAsync(elementName));

app.Run();
app.Urls.Add("http://localhost:5000");