using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Lashes.Models;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;

namespace test.Controllers
{
    [Route("api/[controller]")]
    public class CalendarEventsController : Controller
    {        
        private static String CALENDAR_ID = "vlbetugajsl5bt8t1jrfqs4rm0@group.calendar.google.com";

        public CalendarService GetCalendarService() {
            String serviceAccountEmail = "calendar-fetcher@artful-mystery-161310.iam.gserviceaccount.com";

            var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[] { CalendarService.Scope.Calendar }
               }.FromCertificate(certificate));

            // Create the service.
            CalendarService service = new CalendarService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "Lashes Calendar",
            });

            return service;
        }

        [HttpGet]
        public IEnumerable<CalendarEvent> GetAll()
        {
            const int MaxEventsPerCalendar = 50;
            CalendarService service = GetCalendarService();

            var list = service.Events.List(CALENDAR_ID);
            
            list.MaxResults = MaxEventsPerCalendar;
            list.TimeMin = DateTime.Now;
            list.SingleEvents = false;
            list.TimeMin = DateTime.Now;
            Events allEvents = list.Execute();
            
            List<CalendarEvent> events = new List<CalendarEvent>();
            for(var i = 0; i < allEvents.Items.Count; i++) {
                var thisEvent = allEvents.Items[i];
                events.Add(new CalendarEvent(thisEvent.Summary, thisEvent.Start.DateTime.ToString()));
            }

            return events;
        }

        [HttpPost]
        public IActionResult Create()
        {
            CalendarService service = GetCalendarService();
            Event newEvent = new Event() {
                Summary = "Test appointment",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Now,
                    TimeZone = "Europe/Stockholm"
                },
                End = new EventDateTime( )
                {
                    DateTime = DateTime.Now.AddHours(1),
                    TimeZone = "Europe/Stockholm"
                }
            };

            service.Events.Insert(newEvent, CALENDAR_ID).Execute();
            
            return new ObjectResult(newEvent);
        }
    }
}