namespace EHRsystem.Models.Entities
{
    public class MedicalFile
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime UploadDate { get; set; }
        public string FileType { get; set; } // "Report", "Lab", "Scan"
        public string FilePath { get; set; }
        public string Description { get; set; }
    }
}
