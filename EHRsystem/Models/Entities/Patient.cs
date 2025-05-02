using System.Collections.Generic;
using EHRsystem.Models.Base;

using System.ComponentModel.DataAnnotations.Schema;

namespace EHRsystem.Models.Entities
{
    [Table("Users")] // IMPORTANT!
    public class Patient : User
    {
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalFile> MedicalFiles { get; set; }
    }
}
