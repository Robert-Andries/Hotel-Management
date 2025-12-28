using AspNetCoreRateLimit;

namespace HM.Presentation.WebUI.StartupConfig;

public static class AspNetHotelServices
{
    /// <summary>
    ///     Adds rate limiting services to the service collection.
    /// </summary>
    public static void EnableRateLimiter(this WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        builder.Services.AddInMemoryRateLimiting();
    }
}