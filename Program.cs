using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<AvaloniaHeadlessService>();
builder.Services.AddSingleton<EconomySimService>();
builder.Services.AddSingleton<EconomySimGraphicsService>();
builder.Services.AddSingleton<EconomySimAnalyticsService>();
builder.Services.AddSingleton<EconomySimAutomationService>();

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Avalonia Headless endpoints (renamed from FlaUI)
app.MapGet("/", () => "Avalonia Headless Economy Sim Agent is running!");
app.MapPost("/initialize", (AvaloniaHeadlessService avaloniaService) => avaloniaService.InitializeApp());
app.MapPost("/click", (AvaloniaHeadlessService avaloniaService, string buttonName) => avaloniaService.ClickButton(buttonName));
app.MapGet("/readLabel", (AvaloniaHeadlessService avaloniaService, string labelName) => avaloniaService.ReadLabelText(labelName));
app.MapGet("/window-content", (AvaloniaHeadlessService avaloniaService) => avaloniaService.GetWindowContent());

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

// Analytics and AI endpoints
app.MapPost("/economy-sim/analytics/start-monitoring", async (EconomySimAnalyticsService analyticsService, int intervalMs = 5000) => 
    await analyticsService.StartMonitoringAsync(intervalMs));

app.MapGet("/economy-sim/analytics/comprehensive", async (EconomySimAnalyticsService analyticsService) => 
    await analyticsService.GetGameAnalyticsAsync());

app.MapGet("/economy-sim/analytics/performance-metrics", (EconomySimAnalyticsService analyticsService) => 
    analyticsService.GetPerformanceMetrics());

app.MapGet("/economy-sim/analytics/game-events", async (EconomySimAnalyticsService analyticsService) => 
    await analyticsService.DetectGameEventsAsync());

app.MapGet("/economy-sim/analytics/ai-recommendations", async (EconomySimAnalyticsService analyticsService) => 
    await analyticsService.GetAIRecommendationsAsync());

// Automation and Macro endpoints
app.MapPost("/economy-sim/automation/register-script", (EconomySimAutomationService automationService, AutomationScriptDto script) => 
    automationService.RegisterScript(script));

app.MapPost("/economy-sim/automation/execute-script", async (EconomySimAutomationService automationService, string scriptName) => 
    await automationService.ExecuteScriptAsync(scriptName));

app.MapPost("/economy-sim/automation/start-engine", async (EconomySimAutomationService automationService) => 
    await automationService.StartAutomationEngineAsync());

app.MapPost("/economy-sim/automation/stop-engine", (EconomySimAutomationService automationService) => 
    automationService.StopAutomationEngine());

app.MapGet("/economy-sim/automation/scripts", (EconomySimAutomationService automationService) => 
    automationService.GetAllScripts());

app.MapPost("/economy-sim/automation/execute-macro", async (EconomySimAutomationService automationService, MacroSequenceDto macro) => 
    await automationService.ExecuteMacroAsync(macro));

app.MapPost("/economy-sim/automation/create-ai-script", async (EconomySimAutomationService automationService, string objective) => 
    await automationService.CreateAIAutomationScriptAsync(objective));

app.Urls.Add("http://localhost:5000");
app.Run();