using System.Collections.Generic;
using EHRsystem.Models.Base;

namespace EHRsystem.Models.Entities
{
    public class Patient : User
    {
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalFile> MedicalFiles { get; set; }
    }
}
