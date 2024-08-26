using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.Appointment
{
    public class AppointmentDto
    {
        public AppointmentDto()
        {
            AppointmentWorkOrderServices = new HashSet<AppointmentWorkOrderServiceDto>();
            DocumentAppointments = new HashSet<DocumentAppointmentDto>();
        }

        public int Id { get; set; }
        public int? ServiceRecordId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? StartTime { get; set; }
        public string StartTimeMeridian { get; set; }
        public DateTime? EndTime { get; set; }
        public string EndTimeMeridian { get; set; }
        public string Description { get; set; }
        public string CommentCancel { get; set; }
        public int? Status { get; set; }
        // [DefaultValue((int?)null)]
        public int? Report { get; set; }
        public bool? Start { get; set; }
        public bool? Ended { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? To { get; set; }
        public int? From { get; set; }

        public virtual ICollection<AppointmentWorkOrderServiceDto> AppointmentWorkOrderServices { get; set; }
        public virtual ICollection<DocumentAppointmentDto> DocumentAppointments { get; set; }
    }
}
