namespace Lashes.Models
{
    public class AppointmentLash
    {
        public int ID { get; set; }
        public int AppointmentID { get; set; }
        public string Curve { get; set; }
        public int Length { get; set; }
    }
}