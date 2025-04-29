using System.Collections.Generic;
using EHRsystem.Models.Base;

namespace EHRsystem.Models.Entities
{
    public class Doctor : User
    {
        public string Specialty { get; set; }
        public string Location { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
