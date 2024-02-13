using System.Reflection;
using CurvesForAge.Data;
using CurvesForAge.ViewModels;
using CurvesForAge.Views;
using Microsoft.Extensions.Logging;

namespace CurvesForAge;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Copiar la base de datos desde Resources a un directorio accesible
        string databasePath = Path.Combine(FileSystem.AppDataDirectory, "curves.sqlite");

        if (!File.Exists(databasePath))
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
            using Stream stream = assembly.GetManifestResourceStream("CurvesForAge.Resources.curves.sqlite");
            using MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            File.WriteAllBytes(databasePath, memoryStream.ToArray());
        }
        builder.Services.AddSingleton<DatabaseContext>();
        builder.Services.AddSingleton<ResultViewModel>();
        builder.Services.AddSingleton<ResultPage>();

        return builder.Build();
    }
}