using System;

namespace api.premier.Models.EntryVisa
{
    public class ReminderEntryVisaDto
    {
        public int Id { get; set; }
        public int EntryVisaId { get; set; }
        public DateTime ReminderDate { get; set; }
        public string ReminderComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual EntryVisaDto EntryVisa { get; set; }
    }
}