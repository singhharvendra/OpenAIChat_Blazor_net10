using OpenAIChat.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.PropertyNameCaseInsensitive = true);

// HttpClient for Blazor + outbound OpenAI
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(sp.GetRequiredService<NavigationManager>().BaseUri) });
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("openai");

builder.Services.AddSingleton<ChatService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Minimal endpoint to test server alive
app.MapGet("/api/ping", () => Results.Ok(new { ok = true, now = DateTimeOffset.UtcNow }));

// Optional API endpoint for chat (component uses service directly; this is handy for testing)
app.MapPost("/api/chat", async (ChatService svc, List<ChatService.ChatMessage> messages, CancellationToken ct) =>
{
    var reply = await svc.GetChatCompletionAsync(messages, ct);
    return Results.Ok(new { reply });
}).WithOpenApi();

app.Run();
