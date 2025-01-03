using Microsoft.Extensions.Logging;
using PocketMapApp.Services;
using PocketMapApp.Data;

namespace PocketMapApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();

#endif

            builder.Services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient);
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<AuthStateService>();
            builder.Services.AddScoped<TransactionService>();
            builder.Services.AddScoped<DebtService>();

            return builder.Build();
        }
    }
}
