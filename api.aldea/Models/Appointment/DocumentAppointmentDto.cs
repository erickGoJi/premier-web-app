using System;

namespace api.premier.Models.Appointment
{
    public class DocumentAppointmentDto
    {
        public int Id { get; set; }
        public string FileRequest { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int AppointmentId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual AppointmentDto Appointment { get; set; }
    }
}