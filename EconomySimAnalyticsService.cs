using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

public class EconomySimAnalyticsService
{
    private readonly ILogger<EconomySimAnalyticsService> _logger;
    private readonly EconomySimService _economyService;
    private readonly EconomySimGraphicsService _graphicsService;
    private readonly List<GameStateDto> _gameStateHistory;

    public EconomySimAnalyticsService(
        ILogger<EconomySimAnalyticsService> logger,
        EconomySimService economyService,
        EconomySimGraphicsService graphicsService)
    {
        _logger = logger;
        _economyService = economyService;
        _graphicsService = graphicsService;
        _gameStateHistory = new List<GameStateDto>();
    }

    /// <summary>
    /// Start periodic monitoring of game state
    /// </summary>
    public async Task StartMonitoringAsync(int intervalMs = 5000)
    {
        _logger.LogInformation("Starting game state monitoring with {Interval}ms interval", intervalMs);
        
        // This would run in a background task
        await Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    var gameState = await _economyService.GetGameStateAsync();
                    _gameStateHistory.Add(gameState);
                    
                    // Keep only last 100 records to prevent memory issues
                    if (_gameStateHistory.Count > 100)
                    {
                        _gameStateHistory.RemoveAt(0);
                    }
                    
                    await Task.Delay(intervalMs);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during game state monitoring");
                    await Task.Delay(intervalMs * 2); // Wait longer on error
                }
            }
        });
    }

    /// <summary>
    /// Get comprehensive analytics about the game state
    /// </summary>
    public async Task<GameAnalyticsDto> GetGameAnalyticsAsync()
    {
        try
        {
            var currentState = await _economyService.GetGameStateAsync();
            var tradeData = await _economyService.GetTradeDataAsync();
            var diplomaticRelations = await _economyService.GetDiplomaticRelationsAsync();
            var constructionProjects = await _economyService.GetConstructionProjectsAsync();

            return new GameAnalyticsDto
            {
                CurrentState = currentState,
                HistoricalTrends = CalculateHistoricalTrends(),
                EconomicHealth = CalculateEconomicHealth(currentState),
                DiplomaticSummary = AnalyzeDiplomaticRelations(diplomaticRelations),
                TradeSummary = AnalyzeTradeData(tradeData),
                InfrastructureSummary = AnalyzeInfrastructure(constructionProjects),
                Recommendations = GenerateRecommendations(currentState, tradeData, diplomaticRelations),
                LastAnalyzed = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate game analytics");
            throw;
        }
    }

    /// <summary>
    /// Get performance metrics about the game state over time
    /// </summary>
    public PerformanceMetricsDto GetPerformanceMetrics()
    {
        if (_gameStateHistory.Count < 2)
        {
            return new PerformanceMetricsDto
            {
                Message = "Insufficient data for performance analysis",
                SampleCount = _gameStateHistory.Count
            };
        }

        return new PerformanceMetricsDto
        {
            SampleCount = _gameStateHistory.Count,
            MonitoringDuration = _gameStateHistory.Count > 0 ? 
                _gameStateHistory.Last().LastUpdated - _gameStateHistory.First().LastUpdated : 
                TimeSpan.Zero,
            EconomicGrowthRate = CalculateGrowthRate("GDP"),
            PopulationGrowthRate = CalculateGrowthRate("Population"),
            TreasuryGrowthRate = CalculateGrowthRate("Treasury"),
            AverageGameSpeed = CalculateAverageGameSpeed(),
            LastUpdated = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Detect significant events or changes in the game state
    /// </summary>
    public Task<List<GameEventDto>> DetectGameEventsAsync()
    {
        var events = new List<GameEventDto>();

        if (_gameStateHistory.Count < 2)
        {
            return Task.FromResult(events);
        }

        var current = _gameStateHistory.Last();
        var previous = _gameStateHistory[_gameStateHistory.Count - 2];

        // Detect treasury changes
        if (TryParseNumericValue(current.Treasury, out var currentTreasury) &&
            TryParseNumericValue(previous.Treasury, out var previousTreasury))
        {
            var treasuryChange = ((currentTreasury - previousTreasury) / previousTreasury) * 100;
            if (Math.Abs(treasuryChange) > 10) // 10% change threshold
            {
                events.Add(new GameEventDto
                {
                    EventType = "TreasuryChange",
                    Description = $"Treasury changed by {treasuryChange:F1}%",
                    Severity = Math.Abs(treasuryChange) > 25 ? "High" : "Medium",
                    Timestamp = current.LastUpdated
                });
            }
        }

        // Detect date progression changes
        if (current.Date != previous.Date)
        {
            events.Add(new GameEventDto
            {
                EventType = "DateProgression",
                Description = $"Game date progressed from {previous.Date} to {current.Date}",
                Severity = "Info",
                Timestamp = current.LastUpdated
            });
        }

        return Task.FromResult(events);
    }

    /// <summary>
    /// Generate AI-powered recommendations based on current game state
    /// </summary>
    public async Task<List<RecommendationDto>> GetAIRecommendationsAsync()
    {
        try
        {
            var analytics = await GetGameAnalyticsAsync();
            var recommendations = new List<RecommendationDto>();

            // Economic recommendations
            if (analytics.EconomicHealth.OverallScore < 50)
            {
                recommendations.Add(new RecommendationDto
                {
                    Category = "Economy",
                    Priority = "High",
                    Title = "Improve Economic Performance",
                    Description = "Economic health is below optimal. Consider increasing trade, reducing unemployment, or adjusting fiscal policies.",
                    SuggestedActions = new[] { "Increase exports", "Build more factories", "Adjust tax rates" }
                });
            }

            // Diplomatic recommendations
            if (analytics.DiplomaticSummary.HostileRelations > analytics.DiplomaticSummary.FriendlyRelations)
            {
                recommendations.Add(new RecommendationDto
                {
                    Category = "Diplomacy",
                    Priority = "Medium",
                    Title = "Improve Diplomatic Relations",
                    Description = "More hostile than friendly relations detected. Consider diplomatic initiatives.",
                    SuggestedActions = new[] { "Send diplomats", "Propose trade agreements", "Offer aid" }
                });
            }

            return recommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate AI recommendations");
            throw;
        }
    }

    // Private helper methods
    private HistoricalTrendsDto CalculateHistoricalTrends()
    {
        if (_gameStateHistory.Count < 2)
        {
            return new HistoricalTrendsDto();
        }

        return new HistoricalTrendsDto
        {
            TreasuryTrend = CalculateTrend("Treasury"),
            PopulationTrend = CalculateTrend("Population"),
            GDPTrend = CalculateTrend("GDP"),
            UnemploymentTrend = CalculateTrend("Unemployment"),
            InflationTrend = CalculateTrend("Inflation")
        };
    }

    private string CalculateTrend(string metric)
    {
        if (_gameStateHistory.Count < 2) return "Stable";

        var recent = _gameStateHistory.TakeLast(5).ToList();
        if (recent.Count < 2) return "Stable";

        var values = metric switch
        {
            "Treasury" => recent.Select(x => TryParseNumericValue(x.Treasury, out var v) ? v : 0).ToList(),
            "Population" => recent.Select(x => TryParseNumericValue(x.Population, out var v) ? v : 0).ToList(),
            "GDP" => recent.Select(x => TryParseNumericValue(x.GDP, out var v) ? v : 0).ToList(),
            "Unemployment" => recent.Select(x => TryParseNumericValue(x.Unemployment, out var v) ? v : 0).ToList(),
            "Inflation" => recent.Select(x => TryParseNumericValue(x.Inflation, out var v) ? v : 0).ToList(),
            _ => new List<double>()
        };

        if (values.Count < 2) return "Stable";

        var trend = values.Last() - values.First();
        return trend > values.First() * 0.05 ? "Increasing" : 
               trend < -values.First() * 0.05 ? "Decreasing" : "Stable";
    }

    private EconomicHealthDto CalculateEconomicHealth(GameStateDto currentState)
    {
        var health = new EconomicHealthDto();
        
        // Simple scoring based on available metrics
        var scores = new List<double>();

        if (TryParseNumericValue(currentState.Unemployment, out var unemployment))
        {
            // Lower unemployment is better (inverse score)
            scores.Add(Math.Max(0, 100 - unemployment * 10));
        }

        if (TryParseNumericValue(currentState.Inflation, out var inflation))
        {
            // Inflation around 2% is optimal
            var inflationScore = 100 - Math.Abs(inflation - 2) * 20;
            scores.Add(Math.Max(0, inflationScore));
        }

        health.OverallScore = scores.Count > 0 ? scores.Average() : 50;
        health.UnemploymentScore = scores.Count > 0 ? scores[0] : 50;
        health.InflationScore = scores.Count > 1 ? scores[1] : 50;

        return health;
    }

    private DiplomaticSummaryDto AnalyzeDiplomaticRelations(List<DiplomaticRelationDto> relations)
    {
        return new DiplomaticSummaryDto
        {
            TotalRelations = relations.Count,
            FriendlyRelations = relations.Count(r => r.RelationType.Contains("friendly", StringComparison.OrdinalIgnoreCase)),
            HostileRelations = relations.Count(r => r.RelationType.Contains("hostile", StringComparison.OrdinalIgnoreCase)),
            NeutralRelations = relations.Count(r => r.RelationType.Contains("neutral", StringComparison.OrdinalIgnoreCase))
        };
    }

    private TradeSummaryDto AnalyzeTradeData(TradeDataDto tradeData)
    {
        return new TradeSummaryDto
        {
            TotalExports = tradeData.Exports.Count,
            TotalImports = tradeData.Imports.Count,
            TradeBalance = tradeData.Exports.Count - tradeData.Imports.Count,
            MainExportItems = tradeData.Exports.Take(3).Select(x => x.ItemName).ToArray(),
            MainImportItems = tradeData.Imports.Take(3).Select(x => x.ItemName).ToArray()
        };
    }

    private InfrastructureSummaryDto AnalyzeInfrastructure(List<ConstructionProjectDto> projects)
    {
        return new InfrastructureSummaryDto
        {
            TotalProjects = projects.Count,
            ActiveProjects = projects.Count(p => p.Status.Contains("active", StringComparison.OrdinalIgnoreCase)),
            CompletedProjects = projects.Count(p => p.Status.Contains("completed", StringComparison.OrdinalIgnoreCase)),
            ProjectTypes = projects.GroupBy(p => p.ProjectType).ToDictionary(g => g.Key, g => g.Count())
        };
    }

    private List<RecommendationDto> GenerateRecommendations(GameStateDto state, TradeDataDto trade, List<DiplomaticRelationDto> diplomacy)
    {
        var recommendations = new List<RecommendationDto>();
        
        // Add basic recommendations based on current state
        if (TryParseNumericValue(state.Unemployment, out var unemployment) && unemployment > 5)
        {
            recommendations.Add(new RecommendationDto
            {
                Category = "Economy",
                Priority = "High",
                Title = "Address High Unemployment",
                Description = $"Unemployment at {unemployment}% is above optimal levels",
                SuggestedActions = new[] { "Build factories", "Invest in infrastructure", "Create job programs" }
            });
        }

        return recommendations;
    }

    private double CalculateGrowthRate(string metric)
    {
        if (_gameStateHistory.Count < 2) return 0;

        var recent = _gameStateHistory.TakeLast(10).ToList();
        if (recent.Count < 2) return 0;

        var firstValue = metric switch
        {
            "GDP" => TryParseNumericValue(recent.First().GDP, out var v1) ? v1 : 0,
            "Population" => TryParseNumericValue(recent.First().Population, out var v2) ? v2 : 0,
            "Treasury" => TryParseNumericValue(recent.First().Treasury, out var v3) ? v3 : 0,
            _ => 0
        };

        var lastValue = metric switch
        {
            "GDP" => TryParseNumericValue(recent.Last().GDP, out var v1) ? v1 : 0,
            "Population" => TryParseNumericValue(recent.Last().Population, out var v2) ? v2 : 0,
            "Treasury" => TryParseNumericValue(recent.Last().Treasury, out var v3) ? v3 : 0,
            _ => 0
        };

        return firstValue > 0 ? ((lastValue - firstValue) / firstValue) * 100 : 0;
    }

    private double CalculateAverageGameSpeed()
    {
        if (_gameStateHistory.Count < 2) return 0;

        var timeSpan = _gameStateHistory.Last().LastUpdated - _gameStateHistory.First().LastUpdated;
        return _gameStateHistory.Count / Math.Max(1, timeSpan.TotalMinutes);
    }

    private bool TryParseNumericValue(string value, out double result)
    {
        result = 0;
        if (string.IsNullOrEmpty(value)) return false;

        // Remove common currency symbols and formatting
        var cleanValue = value.Replace("$", "").Replace(",", "").Replace("%", "").Trim();
        
        return double.TryParse(cleanValue, out result);
    }
}

// Analytics Data Transfer Objects
public class GameAnalyticsDto
{
    public GameStateDto CurrentState { get; set; } = new();
    public HistoricalTrendsDto HistoricalTrends { get; set; } = new();
    public EconomicHealthDto EconomicHealth { get; set; } = new();
    public DiplomaticSummaryDto DiplomaticSummary { get; set; } = new();
    public TradeSummaryDto TradeSummary { get; set; } = new();
    public InfrastructureSummaryDto InfrastructureSummary { get; set; } = new();
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public DateTime LastAnalyzed { get; set; }
}

public class HistoricalTrendsDto
{
    public string TreasuryTrend { get; set; } = "Stable";
    public string PopulationTrend { get; set; } = "Stable";
    public string GDPTrend { get; set; } = "Stable";
    public string UnemploymentTrend { get; set; } = "Stable";
    public string InflationTrend { get; set; } = "Stable";
}

public class EconomicHealthDto
{
    public double OverallScore { get; set; }
    public double UnemploymentScore { get; set; }
    public double InflationScore { get; set; }
}

public class DiplomaticSummaryDto
{
    public int TotalRelations { get; set; }
    public int FriendlyRelations { get; set; }
    public int HostileRelations { get; set; }
    public int NeutralRelations { get; set; }
}

public class TradeSummaryDto
{
    public int TotalExports { get; set; }
    public int TotalImports { get; set; }
    public int TradeBalance { get; set; }
    public string[] MainExportItems { get; set; } = Array.Empty<string>();
    public string[] MainImportItems { get; set; } = Array.Empty<string>();
}

public class InfrastructureSummaryDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public Dictionary<string, int> ProjectTypes { get; set; } = new();
}

public class PerformanceMetricsDto
{
    public string Message { get; set; } = string.Empty;
    public int SampleCount { get; set; }
    public TimeSpan MonitoringDuration { get; set; }
    public double EconomicGrowthRate { get; set; }
    public double PopulationGrowthRate { get; set; }
    public double TreasuryGrowthRate { get; set; }
    public double AverageGameSpeed { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class GameEventDto
{
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class RecommendationDto
{
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] SuggestedActions { get; set; } = Array.Empty<string>();
}