using System;

namespace Lashes.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}