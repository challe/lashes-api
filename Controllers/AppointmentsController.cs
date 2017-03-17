using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lashes.Database;
using Lashes.Models;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;

namespace test.Controllers
{
  [Route("api/[controller]")]
    public class AppointmentsController : Controller
    {        
        private static String CALENDAR_ID = "vlbetugajsl5bt8t1jrfqs4rm0@group.calendar.google.com";

        [HttpGet]
        public IEnumerable<Appointment> GetAll()
        {
            using (var db = new DatabaseContext())
            {
               List<Appointment> appointments = (from a in db.Appointments
                    join c in db.Customers
                    on a.CustomerID equals c.ID
                    orderby a.FromTime descending
                    select new Appointment
                    {
                        ID = a.ID,
                        FromTime = a.FromTime,
                        ToTime = a.ToTime,
                        CustomerID = a.CustomerID,
                        Customer = c
                    }).ToList();

                return appointments;
            }
        }

        [HttpGet("{id}", Name = "GetAppointment")]
        public IActionResult GetById(int id)
        {
            using (var db = new DatabaseContext())
            {
                Appointment appointment = db.Appointments.Find(id);

                if (appointment == null)
                {
                    return NotFound();
                }
                return new ObjectResult(appointment);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                return BadRequest();
            }
            using (var db = new DatabaseContext())
            {
                db.Appointments.Add(appointment);
                db.SaveChanges();

                appointment.Customer = db.Customers.Find(appointment.CustomerID);

                this.AddAppointmentToCalendar(appointment);
            }
        
            return new ObjectResult(appointment);
        }

        [HttpDelete("{id}", Name = "RemoveAppointment")]
        public IActionResult Delete(int id)
        {
            using (var db = new DatabaseContext()) {
                Appointment appointment = db.Appointments.Find(id);
                db.Remove(appointment);
                db.SaveChanges();

                return new ObjectResult(appointment);
            }
        }

        public void AddAppointmentToCalendar(Appointment appointment) {
            CalendarService service = GetCalendarService();
            Event newEvent = new Event() {
                Summary = appointment.Customer.Name,
                Start = new EventDateTime()
                {
                    DateTime = appointment.FromTime,
                    TimeZone = "Europe/Stockholm"
                },
                End = new EventDateTime()
                {
                    DateTime = appointment.ToTime,
                    TimeZone = "Europe/Stockholm"
                }
            };

            service.Events.Insert(newEvent, CALENDAR_ID).Execute();


            /*
            // Fetches calendar events:

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
            */
        }

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
    }
}
