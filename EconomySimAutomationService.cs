using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class EconomySimAutomationService
{
    private readonly ILogger<EconomySimAutomationService> _logger;
    private readonly EconomySimService _economyService;
    private readonly EconomySimAnalyticsService _analyticsService;
    private readonly Dictionary<string, AutomationScript> _scripts;
    private bool _isRunningAutomation = false;

    public EconomySimAutomationService(
        ILogger<EconomySimAutomationService> logger,
        EconomySimService economyService,
        EconomySimAnalyticsService analyticsService)
    {
        _logger = logger;
        _economyService = economyService;
        _analyticsService = analyticsService;
        _scripts = new Dictionary<string, AutomationScript>();
        LoadDefaultScripts();
    }

    /// <summary>
    /// Register a new automation script
    /// </summary>
    public string RegisterScript(AutomationScriptDto script)
    {
        try
        {
            var automationScript = new AutomationScript
            {
                Name = script.Name,
                Description = script.Description,
                Triggers = script.Triggers ?? Array.Empty<TriggerConditionDto>(),
                Actions = script.Actions ?? Array.Empty<AutomationActionDto>(),
                IsEnabled = script.IsEnabled,
                CreatedAt = DateTime.UtcNow
            };

            _scripts[script.Name] = automationScript;
            _logger.LogInformation("Registered automation script: {ScriptName}", script.Name);
            
            return $"✅ Script '{script.Name}' registered successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register script: {ScriptName}", script.Name);
            return $"🔥 Error registering script: {ex.Message}";
        }
    }

    /// <summary>
    /// Execute a specific automation script
    /// </summary>
    public async Task<string> ExecuteScriptAsync(string scriptName)
    {
        if (!_scripts.ContainsKey(scriptName))
        {
            return $"❌ Script '{scriptName}' not found";
        }

        var script = _scripts[scriptName];
        if (!script.IsEnabled)
        {
            return $"❌ Script '{scriptName}' is disabled";
        }

        try
        {
            _logger.LogInformation("Executing script: {ScriptName}", scriptName);
            
            foreach (var action in script.Actions)
            {
                await ExecuteActionAsync(action);
                await Task.Delay(action.DelayMs ?? 1000); // Default 1 second delay between actions
            }

            script.LastExecuted = DateTime.UtcNow;
            script.ExecutionCount++;
            
            return $"✅ Script '{scriptName}' executed successfully";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute script: {ScriptName}", scriptName);
            return $"🔥 Error executing script: {ex.Message}";
        }
    }

    /// <summary>
    /// Start the automation engine that monitors triggers and executes scripts
    /// </summary>
    public async Task<string> StartAutomationEngineAsync()
    {
        if (_isRunningAutomation)
        {
            return "❌ Automation engine is already running";
        }

        _isRunningAutomation = true;
        _logger.LogInformation("Starting automation engine");

        // Run automation engine in background
        _ = Task.Run(async () =>
        {
            while (_isRunningAutomation)
            {
                try
                {
                    await CheckTriggersAndExecuteScriptsAsync();
                    await Task.Delay(5000); // Check every 5 seconds
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in automation engine loop");
                }
            }
        });

        return "✅ Automation engine started";
    }

    /// <summary>
    /// Stop the automation engine
    /// </summary>
    public string StopAutomationEngine()
    {
        _isRunningAutomation = false;
        _logger.LogInformation("Automation engine stopped");
        return "✅ Automation engine stopped";
    }

    /// <summary>
    /// Get all registered scripts
    /// </summary>
    public List<AutomationScriptInfoDto> GetAllScripts()
    {
        return _scripts.Values.Select(s => new AutomationScriptInfoDto
        {
            Name = s.Name,
            Description = s.Description,
            IsEnabled = s.IsEnabled,
            TriggerCount = s.Triggers.Length,
            ActionCount = s.Actions.Length,
            ExecutionCount = s.ExecutionCount,
            LastExecuted = s.LastExecuted,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    /// <summary>
    /// Execute a macro sequence of commands
    /// </summary>
    public async Task<string> ExecuteMacroAsync(MacroSequenceDto macro)
    {
        try
        {
            _logger.LogInformation("Executing macro: {MacroName}", macro.Name);
            var results = new List<string>();

            foreach (var command in macro.Commands)
            {
                var result = await _economyService.ExecuteGameCommandAsync(command);
                results.Add(result);
                
                if (macro.DelayBetweenCommandsMs > 0)
                {
                    await Task.Delay(macro.DelayBetweenCommandsMs);
                }
            }

            return $"✅ Macro '{macro.Name}' completed. Results: {string.Join(", ", results)}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute macro: {MacroName}", macro.Name);
            return $"🔥 Error executing macro: {ex.Message}";
        }
    }

    /// <summary>
    /// Create an AI-powered automation script based on current game state
    /// </summary>
    public async Task<string> CreateAIAutomationScriptAsync(string objective)
    {
        try
        {
            var analytics = await _analyticsService.GetGameAnalyticsAsync();
            var recommendations = await _analyticsService.GetAIRecommendationsAsync();

            var script = GenerateScriptForObjective(objective, analytics, recommendations);
            
            return RegisterScript(script);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create AI automation script for objective: {Objective}", objective);
            return $"🔥 Error creating AI script: {ex.Message}";
        }
    }

    // Private helper methods
    private void LoadDefaultScripts()
    {
        // Economic Growth Script
        var economicGrowthScript = new AutomationScriptDto
        {
            Name = "EconomicGrowth",
            Description = "Automatically build infrastructure when treasury is high",
            IsEnabled = false,
            Triggers = new[]
            {
                new TriggerConditionDto
                {
                    Type = "Treasury",
                    Operator = "GreaterThan",
                    Value = "1000000"
                }
            },
            Actions = new[]
            {
                new AutomationActionDto
                {
                    Type = "ExecuteCommand",
                    Parameters = new Dictionary<string, object>
                    {
                        ["commandType"] = "construction",
                        ["action"] = "build_factory"
                    }
                }
            }
        };
        RegisterScript(economicGrowthScript);

        // Trade Balance Script
        var tradeBalanceScript = new AutomationScriptDto
        {
            Name = "TradeBalance",
            Description = "Create export deals when trade balance is negative",
            IsEnabled = false,
            Triggers = new[]
            {
                new TriggerConditionDto
                {
                    Type = "TradeBalance",
                    Operator = "LessThan",
                    Value = "0"
                }
            },
            Actions = new[]
            {
                new AutomationActionDto
                {
                    Type = "ExecuteCommand",
                    Parameters = new Dictionary<string, object>
                    {
                        ["commandType"] = "trade",
                        ["action"] = "create_export"
                    }
                }
            }
        };
        RegisterScript(tradeBalanceScript);
    }

    private async Task CheckTriggersAndExecuteScriptsAsync()
    {
        var gameState = await _economyService.GetGameStateAsync();
        var analytics = await _analyticsService.GetGameAnalyticsAsync();

        foreach (var script in _scripts.Values.Where(s => s.IsEnabled))
        {
            var shouldExecute = await CheckScriptTriggersAsync(script, gameState, analytics);
            if (shouldExecute)
            {
                await ExecuteScriptAsync(script.Name);
            }
        }
    }

    private async Task<bool> CheckScriptTriggersAsync(AutomationScript script, GameStateDto gameState, GameAnalyticsDto analytics)
    {
        foreach (var trigger in script.Triggers)
        {
            var isTriggered = trigger.Type.ToLower() switch
            {
                "treasury" => CheckNumericTrigger(gameState.Treasury, trigger),
                "population" => CheckNumericTrigger(gameState.Population, trigger),
                "unemployment" => CheckNumericTrigger(gameState.Unemployment, trigger),
                "inflation" => CheckNumericTrigger(gameState.Inflation, trigger),
                "tradebalance" => CheckNumericTrigger(analytics.TradeSummary.TradeBalance.ToString(), trigger),
                _ => false
            };

            if (!isTriggered)
            {
                return false; // All triggers must be satisfied
            }
        }

        return script.Triggers.Length > 0; // Return true only if there were triggers to check
    }

    private bool CheckNumericTrigger(string value, TriggerConditionDto trigger)
    {
        if (!TryParseNumericValue(value, out var numericValue) ||
            !TryParseNumericValue(trigger.Value, out var triggerValue))
        {
            return false;
        }

        return trigger.Operator.ToLower() switch
        {
            "greaterthan" => numericValue > triggerValue,
            "lessthan" => numericValue < triggerValue,
            "equals" => Math.Abs(numericValue - triggerValue) < 0.01,
            _ => false
        };
    }

    private async Task ExecuteActionAsync(AutomationActionDto action)
    {
        switch (action.Type.ToLower())
        {
            case "executecommand":
                if (action.Parameters.ContainsKey("commandType") && action.Parameters.ContainsKey("action"))
                {
                    var command = new GameCommandDto
                    {
                        CommandType = action.Parameters["commandType"].ToString(),
                        Action = action.Parameters["action"].ToString(),
                        Parameters = action.Parameters.Where(p => p.Key != "commandType" && p.Key != "action")
                                                    .ToDictionary(p => p.Key, p => p.Value)
                    };
                    await _economyService.ExecuteGameCommandAsync(command);
                }
                break;
            case "changemapview":
                if (action.Parameters.ContainsKey("viewType"))
                {
                    await _economyService.ChangeMapViewAsync(action.Parameters["viewType"].ToString());
                }
                break;
        }
    }

    private AutomationScriptDto GenerateScriptForObjective(string objective, GameAnalyticsDto analytics, List<RecommendationDto> recommendations)
    {
        var script = new AutomationScriptDto
        {
            Name = $"AI_{objective}_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
            Description = $"AI-generated script for objective: {objective}",
            IsEnabled = false,
            Actions = new List<AutomationActionDto>()
        };

        // Generate actions based on recommendations
        foreach (var recommendation in recommendations.Take(3)) // Limit to top 3 recommendations
        {
            foreach (var suggestedAction in recommendation.SuggestedActions.Take(1)) // One action per recommendation
            {
                var action = MapRecommendationToAction(suggestedAction);
                if (action != null)
                {
                    script.Actions.Add(action);
                }
            }
        }

        return script;
    }

    private AutomationActionDto MapRecommendationToAction(string suggestedAction)
    {
        return suggestedAction.ToLower() switch
        {
            "build factories" => new AutomationActionDto
            {
                Type = "ExecuteCommand",
                Parameters = new Dictionary<string, object>
                {
                    ["commandType"] = "construction",
                    ["action"] = "build_factory"
                }
            },
            "increase exports" => new AutomationActionDto
            {
                Type = "ExecuteCommand",
                Parameters = new Dictionary<string, object>
                {
                    ["commandType"] = "trade",
                    ["action"] = "create_export"
                }
            },
            _ => null
        };
    }

    private bool TryParseNumericValue(string value, out double result)
    {
        result = 0;
        if (string.IsNullOrEmpty(value)) return false;

        var cleanValue = value.Replace("$", "").Replace(",", "").Replace("%", "").Trim();
        return double.TryParse(cleanValue, out result);
    }
}

// Automation Data Transfer Objects
public class AutomationScriptDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public TriggerConditionDto[] Triggers { get; set; } = Array.Empty<TriggerConditionDto>();
    public List<AutomationActionDto> Actions { get; set; } = new();
}

public class AutomationScript
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public TriggerConditionDto[] Triggers { get; set; } = Array.Empty<TriggerConditionDto>();
    public AutomationActionDto[] Actions { get; set; } = Array.Empty<AutomationActionDto>();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastExecuted { get; set; }
    public int ExecutionCount { get; set; }
}

public class TriggerConditionDto
{
    public string Type { get; set; } = string.Empty; // Treasury, Population, Unemployment, etc.
    public string Operator { get; set; } = string.Empty; // GreaterThan, LessThan, Equals
    public string Value { get; set; } = string.Empty;
}

public class AutomationActionDto
{
    public string Type { get; set; } = string.Empty; // ExecuteCommand, ChangeMapView, etc.
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int? DelayMs { get; set; } = 1000;
}

public class AutomationScriptInfoDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int TriggerCount { get; set; }
    public int ActionCount { get; set; }
    public int ExecutionCount { get; set; }
    public DateTime? LastExecuted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MacroSequenceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<GameCommandDto> Commands { get; set; } = new();
    public int DelayBetweenCommandsMs { get; set; } = 1000;
}