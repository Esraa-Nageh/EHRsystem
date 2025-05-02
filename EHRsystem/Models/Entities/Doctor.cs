using System.Collections.Generic;
using EHRsystem.Models.Base;

using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    [Table("Users")] // IMPORTANT!
    public class Doctor : User
    {
        public string Specialty { get; set; } = "";
        public string Location { get; set; } = "";

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
