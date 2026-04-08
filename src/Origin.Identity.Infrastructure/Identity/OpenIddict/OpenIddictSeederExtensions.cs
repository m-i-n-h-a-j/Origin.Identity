namespace Origin.Identity.Infrastructure.Identity.OpenIddict
{
    public static class OpenIddictSeederExtensions
    {
        public static async Task SeedOpenIddictAsync(this IServiceProvider services)
        {
            await OpenIddictSeeder.SeedAsync(services);

        }
    }
}
