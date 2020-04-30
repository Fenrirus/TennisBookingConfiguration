using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TennisBookings.Web.Configuration;
using TennisBookings.Web.External;
using TennisBookings.Web.External.Models;
using TennisBookings.Web.Services;

namespace TennisBookings.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IGreetingService _greetingService;
        private readonly HomePageConfiguration _homePageConfig;
        private readonly IWeatherForecaster _weatherForecaster;
        private readonly IProductsApiClient _productsApiClient;

        public IndexModel(
            IGreetingService greetingService,
            IWeatherForecaster weatherForecaster,
            IOptionsSnapshot<HomePageConfiguration> options,
            IProductsApiClient productsApiClient)
        {
            _greetingService = greetingService;
            _homePageConfig = options.Value;
            _weatherForecaster = weatherForecaster;
            _productsApiClient = productsApiClient;
            GreetingColour = _greetingService.GreetingColour ?? "black";
        }

        public string Greeting { get; private set; }
        public bool ShowGreeting => !string.IsNullOrEmpty(Greeting);
        public string ForecastSectionTitle { get; private set; }
        public string WeatherDescription { get; private set; }
        public bool ShowWeatherForecast { get; private set; }
        public string GreetingColour { get; private set; }
        public IReadOnlyCollection<Product> Products { get; set; }

        public async Task OnGet()
        {
            if (_homePageConfig.EnableGreeting)
            {
                Greeting = _greetingService.GetRandomGreeting();
            }

            ShowWeatherForecast = _homePageConfig.EnableWeatherForecast;

            if (ShowWeatherForecast)
            {
                var title = _homePageConfig.ForecastSectionTitle;

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
            var productsResult = await _productsApiClient.GetProducts();

            Products = productsResult.Products;
        }
    }
}
