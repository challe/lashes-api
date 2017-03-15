namespace Lashes.Models
{
    public class Appointment
    {
      public Appointment(string summary, string date) {
        this.summary = summary;
        this.date = date;
      }
      public string summary { get; set; }
      public string date {get; set;}
    }
}