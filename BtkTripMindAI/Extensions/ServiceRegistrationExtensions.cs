using BtkTripMindAI.Agent.Email;
using BtkTripMindAI.Agent.Extractors;
using BtkTripMindAI.Agent.Intents;
using BtkTripMindAI.Models.CalendarModels;
using BtkTripMindAI.Models.EmailModels;
using BtkTripMindAI.Services.AgentServices;
using BtkTripMindAI.Services.AirportServices;
using BtkTripMindAI.Services.BudgetServices;
using BtkTripMindAI.Services.CalendarServices;
using BtkTripMindAI.Services.EmailServices;
using BtkTripMindAI.Services.FlightServices;
using BtkTripMindAI.Services.HotelServices;
using BtkTripMindAI.Services.ImageServices;
using BtkTripMindAI.Services.RestaurantServices;
using BtkTripMindAI.Services.RouteServices;
using BtkTripMindAI.Services.TravelDetailServices;
using BtkTripMindAI.Services.WeatherServices;

namespace BtkTripMindAI.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAgentService, AgentService>();

            services.AddHttpClient<IWeatherService, RapidApiWeatherService>();
            services.AddHttpClient<ICityImageService, PexelsCityImageService>();
            services.AddHttpClient<IRouteService, RouteService>();
            services.AddHttpClient<IRestaurantService, RestaurantService>();
            services.AddHttpClient<IHotelService, HotelService>();
            services.AddHttpClient<IGooglePlacePhotoService, GooglePlacePhotoService>();
            services.AddHttpClient<IFlightService, FlightService>();

            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<ITravelDetailExtractor, OpenAITravelDetailExtractor>();

            services.AddHttpClient<IEmailExtractor, OpenAIEmailExtractor>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHttpClient<ICalendarEventExtractor, OpenAICalendarEventExtractor>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHttpClient<IAirportService, AirportService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });

            services.AddHttpClient<IFlightSearchExtractor, OpenAIFlightSearchExtractor>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddHttpClient<IIntentDetector, OpenAIIntentDetector>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(20);
            });

            services.AddHttpClient<ICityExtractor, OpenAICityExtractor>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddScoped<ICalendarService, GoogleCalendarService>();
            services.AddScoped<IEmailService, EmailService>();

            services.Configure<GoogleCalendarSettings>(configuration.GetSection("GoogleCalendar"));

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }
    }
}
