using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<FlaUIService>();
var app = builder.Build();

app.MapGet("/", () => "FlaUI Agent is running!");
app.MapPost("/launch", (FlaUIService flaUI) => flaUI.LaunchApp());
app.MapPost("/click", (FlaUIService flaUI, string buttonText) => flaUI.ClickButton(buttonText));
app.MapGet("/readLabel", (FlaUIService flaUI, string labelAutomationId) => flaUI.ReadLabelText(labelAutomationId));

app.Run();
app.Urls.Add("http://localhost:5000");