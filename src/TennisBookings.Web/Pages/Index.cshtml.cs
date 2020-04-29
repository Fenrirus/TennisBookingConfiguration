using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TennisBookings.Web.Services;

namespace TennisBookings.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IGreetingService _greetingService;
        private readonly IConfiguration _configuration;
        private readonly IWeatherForecaster _weatherForecaster;

        public IndexModel(IGreetingService greetingService, IConfiguration configuration, IWeatherForecaster weatherForecaster)
        {
            _greetingService = greetingService;
            _configuration = configuration;
            _weatherForecaster = weatherForecaster;
        }

        public string Greeting { get; private set; }
        public bool ShowGreeting => !string.IsNullOrEmpty(Greeting);
        public string ForecastSectionTitle { get; private set; }
        public string WeatherDescription { get; private set; }
        public bool ShowWeatherForecast { get; private set; }

        public async Task OnGet()
        {
            var homePage = _configuration.GetSection("Features:HomePage");
            if (homePage.GetValue<bool>("EnableGreeting"))
            {
                Greeting = _greetingService.GetRandomGreeting();
            }

            ShowWeatherForecast = homePage.GetValue<bool>("EnableWeatherForecast");

            if (ShowWeatherForecast)
            {
                var title = homePage["ForecastSectionTitle"];

                ForecastSectionTitle = title;
                var currenWeather = await _weatherForecaster.GetCurrentWeatherAsync();

                if (currenWeather != null)
                {
                    switch (currenWeather.Description)
                    {
                        case "Sun":
                            WeatherDescription = "Piękne słońce";
                            break;

                        case "Cloud":
                            WeatherDescription = "Zachmurzenie, czapka niepotrzebna";
                            break;

                        case "Rain":
                            WeatherDescription = "Pada, pada, pada deszcze";
                            break;
                    }
                }
            }
        }
    }
}
