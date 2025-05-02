using System;

namespace EHRsystem.Models.Entities
{
    public class MedicalFile
    {
        public int Id { get; set; }

        // Relations
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        // File details
        public string FileType { get; set; } = string.Empty;        // e.g. "PDF", "Image", etc.
        public string FilePath { get; set; } = string.Empty;        // Path to the uploaded file
        public string Description { get; set; } = string.Empty;     // Notes or description about the file

        public DateTime UploadedAt { get; set; } = DateTime.Now;    // Timestamp of upload
    }
}
