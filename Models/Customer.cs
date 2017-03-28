using System.Collections.Generic;

namespace Lashes.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        List<Appointment> Appointments { get; set; }
    }
}