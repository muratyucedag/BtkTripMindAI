using BtkTripMindAI.Dtos.CalendarDtos;
using BtkTripMindAI.Models.CalendarModels;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;

namespace BtkTripMindAI.Services.CalendarServices
{
    public class GoogleCalendarService : ICalendarService
    {
        private readonly GoogleCalendarSettings _settings;
        private readonly ILogger<GoogleCalendarService> _logger;
        public GoogleCalendarService(IOptions<GoogleCalendarSettings> options, ILogger<GoogleCalendarService> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }
        public async Task<CalendarEventResult> CreateEventAsync(CreateCalendarEventDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return CreateFailureResult("Takvim etkinliği için başlık gereklidir.");
            }

            if (request.StartDateTime == default)
            {
                return CreateFailureResult("Etkinlik başlangıç tarihi gereklidir.");
            }

            if (request.EndDateTime == default)
            {
                return CreateFailureResult("Etkinlik bitiş tarihi gereklidir.");
            }

            if (request.EndDateTime <= request.StartDateTime)
            {
                return CreateFailureResult("Etkinlik bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(_settings.ClientId) || string.IsNullOrWhiteSpace(_settings.ClientSecret))
            {
                return CreateFailureResult("Google Calendar OAuth bilgileri yapılandırılmamış.");
            }

            try
            {
                UserCredential credential = await AuthorizeAsync();

                using var calendarService = new CalendarService(
                        new BaseClientService.Initializer
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = _settings.ApplicationName
                        });

                var calendarEvent = new Event
                {
                    Summary = request.Title,
                    Description = request.Description,
                    Location = request.Location,
                    Start =
                        new EventDateTime
                        {
                            DateTimeRaw = request.StartDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                            TimeZone = request.TimeZone
                        },

                    End =
                            new EventDateTime
                            {
                                DateTimeRaw = request.EndDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                                TimeZone = request.TimeZone
                            },

                    Reminders = new Event.RemindersData
                    {
                        UseDefault = false,
                        Overrides =
                                new List<EventReminder>
                                    {
                                        new EventReminder
                                        {
                                            Method = "email",
                                            Minutes = 24 * 60
                                        },

                                        new EventReminder
                                        {
                                            Method = "popup",
                                            Minutes = 60
                                        }
                                    }
                    }
                };

                string calendarId = string.IsNullOrWhiteSpace(_settings.CalendarId) ? "primary" : _settings.CalendarId;
                EventsResource.InsertRequest insertRequest = calendarService.Events.Insert(calendarEvent, calendarId);
                Event createdEvent = await insertRequest.ExecuteAsync();

                return new CalendarEventResult
                {
                    IsSuccess = true,
                    EventId = createdEvent.Id,
                    EventLink = createdEvent.HtmlLink,
                    Title = createdEvent.Summary,
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime,
                    Message = "Etkinlik Google Calendar'a başarıyla eklendi."
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Google Calendar etkinliği oluşturulamadı. " + "Title: {Title}", request.Title);

                return CreateFailureResult("Google Calendar etkinliği oluşturulamadı.");
            }
        }
        private async Task<UserCredential> AuthorizeAsync()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret
            };

            string[] scopes =
            {
                CalendarService.Scope.CalendarEvents
            };

            string tokenStorePath = Path.Combine(AppContext.BaseDirectory, "GoogleCalendarTokens");
            string userKey = string.IsNullOrWhiteSpace(_settings.UserEmail) ? "BtkTripMindAIUser" : _settings.UserEmail;

            return await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, userKey, CancellationToken.None, new FileDataStore(tokenStorePath, true));
        }
        private static CalendarEventResult CreateFailureResult(string message)
        {
            return new CalendarEventResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
