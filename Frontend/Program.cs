using Frontend.Interfaces;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Frontend;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7120") });

        // Register Services
        builder.Services.AddScoped<IBookingService, BookingService>();
        builder.Services.AddScoped<ILaptopService, LaptopService>();



        await builder.Build().RunAsync();
    }
}
