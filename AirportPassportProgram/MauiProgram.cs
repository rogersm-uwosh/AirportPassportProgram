using Microsoft.Extensions.Logging;
using AirportPassportProgram.Model;

namespace AirportPassportProgram;

public static class MauiProgram
{
	    public static IBusinessLogic BusinessLogic = new BusinessLogic(new Database());


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

		return builder.Build();
	}
}
