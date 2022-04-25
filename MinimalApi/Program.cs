using MinimalApi;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,    
    WebRootPath = "wwwroot"
});

builder.Services.AddScoped<IStreamVideos, VideoStreamer>(provider => new VideoStreamer(builder.Configuration, builder.Environment));

var app = builder.Build();

app.UseFileServer();

app.MapGet("/", () => Results.File("index.html", "text/html"));

app.MapGet("/video", async (HttpRequest request, HttpResponse response, CancellationToken cancellationToken) =>
{
    using var scope = app.Services.CreateScope();
    var videoStreamer = scope.ServiceProvider.GetRequiredService<IStreamVideos>();
    await videoStreamer.StreamVideo(request, response, cancellationToken);
});

app.Run();