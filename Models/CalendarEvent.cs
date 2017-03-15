namespace Lashes.Models
{
    public class CalendarEvent
    {
      public CalendarEvent(string summary, string date) {
        this.summary = summary;
        this.date = date;
      }
      public string summary { get; set; }
      public string date {get; set;}
    }
}