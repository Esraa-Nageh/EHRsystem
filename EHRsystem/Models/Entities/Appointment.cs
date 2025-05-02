using System;
using System.ComponentModel.DataAnnotations;

namespace EHRsystem.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doctor ID is required.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public int PatientId { get; set; }
    }
}
