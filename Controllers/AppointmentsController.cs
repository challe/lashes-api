using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Lashes.Models;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace test.Controllers
{
    [Route("api/[controller]")]
    public class AppointmentsController : Controller
    {        
        private static String CALENDAR_ID = "vlbetugajsl5bt8t1jrfqs4rm0@group.calendar.google.com";

        [HttpGet]
        public IEnumerable<Appointment> GetAll()
        {
            const int MaxEventsPerCalendar = 50;

            String serviceAccountEmail = "calendar-fetcher@artful-mystery-161310.iam.gserviceaccount.com";

            var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[] { CalendarService.Scope.Calendar }
                   //,User = "challe128@gmail.com"

               }.FromCertificate(certificate));

            // Create the service.
            var service = new CalendarService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = "Lashes Calendar",
            });

            
            var list = service.Events.List(CALENDAR_ID);
            
            list.MaxResults = MaxEventsPerCalendar;
            list.TimeMin = DateTime.Now;
            list.SingleEvents = false;
            list.TimeMin = DateTime.Now;
            var allEvents = list.Execute();
            
            List<Appointment> events = new List<Appointment>();
            for(var i = 0; i < allEvents.Items.Count; i++) {
                var thisEvent = allEvents.Items[i];
                events.Add(new Appointment(thisEvent.Summary, thisEvent.Start.DateTime.ToString()));
            }

            return events;
        }
    }
}