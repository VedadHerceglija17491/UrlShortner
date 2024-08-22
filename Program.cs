using UrlShortner.Models;
using Microsoft.EntityFrameworkCore;
using UrlShortner.Entities;
using UrlShortner.Services;
using UrlShortner;
using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


   var server = builder.Configuration["DBServer"] ?? "localhost";
            var port = builder.Configuration["DBPort"] ?? "1433";
            var user = builder.Configuration["DBUser"] ?? "SA";

            var password = builder.Configuration ["DBPassword"] ?? "Pa55w0rd2019";
            var database = builder.Configuration["Database"] ?? "sadas";

      

builder.Services.AddDbContext<ApplicationDbContext>(o =>
    o.UseSqlServer($"Server={server},{port};Initial Catalog={database};User ID={user}; PASSWORD={password};Encrypt=True;TrustServerCertificate=True;"));


builder.Services.AddScoped<UrlShorteningService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

     MigrationExtension.ApplayMigrations(app);
}

app.MapPost("api/shorten", async (
    ShortenUrlRequest request,
    UrlShorteningService urlShorteningService,
    ApplicationDbContext dbContext,
    HttpContext httpContext) => 
{
    //check is URL OK
    // if(Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    // {
    //     return Results.BadRequest("The specified URL is invalid.");
    // }

    //shortened URL, generete unique code,  perist URL in DB
    var code = await urlShorteningService.GenerateUniqueCode();

    var shortenedUrl = new ShortenedUrl 
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        CreatedOnUtc = DateTime.Now
    };
    
    //adding to db
    dbContext.ShortenedUrls.Add(shortenedUrl);

    await dbContext.SaveChangesAsync();

    return Results.Ok(shortenedUrl.ShortUrl);
});


app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext) => {
    
    
    var shortenedUrl = await dbContext.ShortenedUrls
        .FirstOrDefaultAsync(s => s.Code == code);

        if( shortenedUrl is null)
        {
            return Results.NotFound();
        }

    return Results.Redirect(shortenedUrl.LongUrl);
});
app.UseHttpsRedirection();

app.Run();

