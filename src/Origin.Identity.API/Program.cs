using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Origin.Identity.Infrastructure.DependencyInjection;
using Origin.Identity.Infrastructure.Identity.OpenIddict;
using Origin.Identity.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowClients",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200", "https://origin-client-sample.netlify.app")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});
#endregion

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.IsDevelopment());

builder.Services.AddAuthorization();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

    await db.Database.MigrateAsync();

    await app.Services.SeedOpenIddictAsync();
}



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var authSpaPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "auth");

app.UseHttpsRedirection();

app.UseCors("AllowClients");

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(authSpaPath),
        RequestPath = "/auth",
    }
);

app.MapWhen(
    context =>
        context.Request.Path.StartsWithSegments("/auth")
        && !Path.HasExtension(context.Request.Path.Value),
    branch =>
    {
        branch.Run(async context =>
        {
            context.Response.ContentType = "text/html";

            await context.Response.SendFileAsync(Path.Combine(authSpaPath, "index.html"));
        });
    }
);

app.MapControllers();

app.MapGet("/debug-scheme", (HttpContext context) =>
{
    return Results.Ok(new
    {
        Scheme = context.Request.Scheme,
        Host = context.Request.Host.Value,
        ForwardedProto = context.Request.Headers["X-Forwarded-Proto"].ToString()
    });
});

app.Run();