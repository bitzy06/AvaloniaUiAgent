using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;

public class AvaloniaHeadlessService : IDisposable
{
    private AppBuilder? _appBuilder;
    private Application? _app;
    private Window? _mainWindow;
    private bool _isInitialized = false;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AvaloniaHeadlessService> _logger;

    public AvaloniaHeadlessService(IConfiguration configuration, ILogger<AvaloniaHeadlessService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string InitializeApp()
    {
        try
        {
            if (_isInitialized)
                return "App already initialized.";

            _logger.LogInformation("Initializing Avalonia Headless application");
            
            // Configure Avalonia for headless operation
            _appBuilder = AppBuilder.Configure<EconomySimApp>()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions
                {
                    UseHeadlessDrawing = true
                });

            _app = _appBuilder.Instance;
            
            // Initialize the application in headless mode
            Dispatcher.UIThread.Post(() =>
            {
                _mainWindow = new EconomySimMainWindow();
                _mainWindow.Show();
            });

            _isInitialized = true;
            _logger.LogInformation("Avalonia Headless application initialized successfully");

            return "✅ Avalonia Headless app initialized.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Avalonia Headless app");
            return "🔥 Error initializing app: " + ex.Message;
        }
    }

    public async Task<string> ClickButton(string buttonName)
    {
        if (!_isInitialized) 
            return "❌ App not initialized yet.";
        
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                if (_mainWindow == null)
                    return "❌ Main window not available.";

                // Find button by name in the visual tree
                var button = FindControlByName<Button>(_mainWindow, buttonName);
                if (button != null)
                {
                    // Simulate button click
                    button.Command?.Execute(button.CommandParameter);
                    _logger.LogDebug("Clicked button: {ButtonName}", buttonName);
                    return $"Clicked: {buttonName}";
                }
                else
                {
                    return $"❌ Button '{buttonName}' not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clicking button: {ButtonName}", buttonName);
                return "Error: " + ex.Message;
            }
        });
    }

    public async Task<string> ReadLabelText(string labelName)
    {
        if (!_isInitialized) 
            return "❌ App not initialized yet.";
        
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                if (_mainWindow == null)
                    return "❌ Main window not available.";

                // Find label by name in the visual tree
                var label = FindControlByName<TextBlock>(_mainWindow, labelName);
                if (label != null)
                {
                    var text = label.Text ?? "Label text is empty.";
                    _logger.LogDebug("Read label {LabelName}: {Text}", labelName, text);
                    return text;
                }
                else
                {
                    return $"❌ Label '{labelName}' not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading label: {LabelName}", labelName);
                return "Error: " + ex.Message;
            }
        });
    }

    public async Task<string> GetWindowContent()
    {
        if (!_isInitialized) 
            return "❌ App not initialized yet.";
        
        return await Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                if (_mainWindow == null)
                    return "❌ Main window not available.";

                // Return basic window information
                return $"Window Title: {_mainWindow.Title}, Size: {_mainWindow.Width}x{_mainWindow.Height}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting window content");
                return "Error: " + ex.Message;
            }
        });
    }

    private T? FindControlByName<T>(Control parent, string name) where T : Control
    {
        if (parent.Name == name && parent is T)
            return (T)parent;

        if (parent is Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is Control control)
                {
                    var result = FindControlByName<T>(control, name);
                    if (result != null)
                        return result;
                }
            }
        }

        return null;
    }

    public bool IsAppInitialized => _isInitialized;

    public void Dispose()
    {
        try
        {
            if (_mainWindow != null)
            {
                Dispatcher.UIThread.Post(() => _mainWindow.Close());
            }
            _isInitialized = false;
            _logger.LogInformation("AvaloniaHeadlessService disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing AvaloniaHeadlessService");
        }
    }
}

// Placeholder Avalonia Application class
public class EconomySimApp : Application
{
    public override void Initialize()
    {
        // Initialize application resources here
    }
}

// Placeholder main window for the Economy Sim
public class EconomySimMainWindow : Window
{
    public EconomySimMainWindow()
    {
        Title = "Economy Simulation";
        Width = 800;
        Height = 600;
        
        // Create a simple UI structure for testing
        var stackPanel = new StackPanel();
        
        var titleLabel = new TextBlock
        {
            Name = "TitleLabel",
            Text = "Economy Simulation Dashboard",
            FontSize = 24,
            Margin = new Thickness(10)
        };
        
        var statusLabel = new TextBlock
        {
            Name = "StatusLabel", 
            Text = "Ready",
            Margin = new Thickness(10)
        };
        
        var startButton = new Button
        {
            Name = "StartButton",
            Content = "Start Simulation",
            Margin = new Thickness(10)
        };
        
        stackPanel.Children.Add(titleLabel);
        stackPanel.Children.Add(statusLabel);
        stackPanel.Children.Add(startButton);
        
        Content = stackPanel;
    }
}