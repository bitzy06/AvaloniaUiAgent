using System;
using System.Threading.Tasks;
using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;

public class FlaUIService : IDisposable
{
    private Application _app;
    private UIA3Automation _automation;
    private Window _mainWindow;
    private bool _isLaunched = false;

    public string LaunchApp()
    {
        try
        {
            if (_isLaunched)
                return "App already launched.";

            Console.WriteLine("Launching Avalonia app...");
            _app = Application.Launch(@"C:\projects\Economy-sim\bin\x64\Debug\net8.0\Economy-sim.exe"); // Change this path
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation);

            _isLaunched = true;

            return _mainWindow == null ? "❌ Main window not found." : "✅ App launched.";
        }
        catch (Exception ex)
        {
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
            return Task.FromResult("Clicked: " + buttonText);
        }
        catch (Exception ex)
        {
            return Task.FromResult("Error: " + ex.Message);
        }
    }

    public Task<string> ReadLabelText(string automationId)
    {
        if (!_isLaunched) return Task.FromResult("❌ App not launched yet.");
        try
        {
            var label = _mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId))?.AsLabel();
            return Task.FromResult(label?.Text ?? "Label not found.");
        }
        catch (Exception ex)
        {
            return Task.FromResult("Error: " + ex.Message);
        }
    }

    public void Dispose()
    {
        _automation?.Dispose();
        _app?.Close();
    }
}