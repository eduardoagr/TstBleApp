using Microsoft.Extensions.Logging;

using System.Text;

using TstBleApp.ViewModels;
using TstBleApp.Views;

namespace TstBleApp {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {

            // 🔥 REQUIRED for Encoding.GetEncoding(437)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<BluetoothPage>();

            builder.Services.AddSingleton<BluetoothPageViewModel>();

            return builder.Build();
        }
    }
}
