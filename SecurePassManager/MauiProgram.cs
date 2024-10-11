using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Storage;
using SecurePassManager;
using SecurePassManager.Services;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<EncryptionService>();
        builder.Services.AddSingleton<PasswordGeneratorService>();
        builder.Services.AddSingleton<ValidationService>();
        builder.Services.AddSingleton<SecurityLogger>();
        builder.Services.AddSingleton<PasswordStrengthService>();
        
        builder.Services.AddSingleton<MasterPasswordService>(serviceProvider => 
            new MasterPasswordService(Path.Combine(FileSystem.AppDataDirectory, "users.json")));

        builder.Services.AddSingleton<PasswordStorageService>(serviceProvider => 
        {
            var encryptionService = serviceProvider.GetRequiredService<EncryptionService>();
            return new PasswordStorageService(
                encryptionService, 
                Path.Combine(FileSystem.AppDataDirectory, "passwords.json"));
        });

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<PasswordListPage>();
        builder.Services.AddTransient<PasswordEntryPage>();

        builder.Logging.AddDebug();

        return builder.Build();
    }
}