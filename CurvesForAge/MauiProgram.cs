using System.Reflection;
using CurvesForAge.Data;
using CurvesForAge.ViewModels;
using CurvesForAge.Views;
using Microsoft.Extensions.Logging;
using Plugin.MauiMTAdmob;
using SkiaSharp.Views.Maui.Controls.Hosting; 

namespace CurvesForAge;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseSkiaSharp(true) 
            .UseMauiApp<App>()
            .UseMauiMTAdmob()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Copiar la base de datos desde Resources a un directorio accesible
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "curves.sqlite");

        if (!File.Exists(databasePath))
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            using var stream = assembly.GetManifestResourceStream("CurvesForAge.Resources.curves.sqlite");
            using var memoryStream = new MemoryStream();
            stream?.CopyTo(memoryStream);

            File.WriteAllBytes(databasePath, memoryStream.ToArray());
        }
        builder.Services.AddSingleton<DatabaseContext>();
        builder.Services.AddSingleton<ResultViewModel>();
        builder.Services.AddSingleton<ResultPage>();

        return builder.Build();
    }
}