using BtkTripMindAI.Agent.Extractors;
using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Services.AgentServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAgentService, AgentService>();

builder.Services.AddHttpClient<IIntentDetector, OpenAIIntentDetector>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(20);
    });

builder.Services.AddHttpClient<ICityExtractor, OpenAICityExtractor>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
