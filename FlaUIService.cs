using System;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class FlaUIService : IDisposable
{
    private Application _app;
    private UIA3Automation _automation;
    private Window _mainWindow;
    private bool _isLaunched = false;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FlaUIService> _logger;

    public FlaUIService(IConfiguration configuration, ILogger<FlaUIService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string LaunchApp()
    {
        try
        {
            if (_isLaunched)
                return "App already launched.";

            var executablePath = _configuration["EconomySim:ExecutablePath"] ?? 
                                @"C:\projects\Economy-sim\bin\x64\Debug\net8.0\Economy-sim.exe";

            _logger.LogInformation("Launching Avalonia app from: {ExecutablePath}", executablePath);
            
            _app = Application.Launch(executablePath);
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);

            _isLaunched = true;

            return _mainWindow == null ? "❌ Main window not found." : "✅ App launched.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error launching economy-sim app");
            return "🔥 Error launching app: " + ex.Message;
        }
    }

    public Task<string> ClickButton(string buttonText)
    {
        if (!_isLaunched) return Task.FromResult("❌ App not launched yet.");
        try
        {
            var button = _mainWindow.FindFirstDescendant(cf => cf.ByText(buttonText)).AsButton();
            button?.Invoke();
            _logger.LogDebug("Clicked button: {ButtonText}", buttonText);
            return Task.FromResult("Clicked: " + buttonText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clicking button: {ButtonText}", buttonText);
            return Task.FromResult("Error: " + ex.Message);
        }
    }

    public Task<string> ReadLabelText(string automationId)
    {
        if (!_isLaunched) return Task.FromResult("❌ App not launched yet.");
        try
        {
            var label = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsLabel();
            var text = label?.Text ?? "Label not found.";
            _logger.LogDebug("Read label {AutomationId}: {Text}", automationId, text);
            return Task.FromResult(text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading label: {AutomationId}", automationId);
            return Task.FromResult("Error: " + ex.Message);
        }
    }

    public bool IsAppLaunched => _isLaunched;

    public void Dispose()
    {
        _automation?.Dispose();
        _app?.Close();
        _isLaunched = false;
        _logger.LogInformation("FlaUIService disposed");
    }
}