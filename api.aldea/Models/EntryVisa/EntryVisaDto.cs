using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.EntryVisa
{
    public class EntryVisaDto
    {
        public EntryVisaDto()
        {
            CommentsEntryVisas = new HashSet<CommentsEntryVisaDto>();
            DocumentEntryVisas = new HashSet<DocumentEntryVisaDto>();
            //ExtensionEntryVisas = new HashSet<ExtensionEntryVisaDto>();
            ReminderEntryVisas = new HashSet<ReminderEntryVisaDto>();
        }
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int StatusId { get; set; }
        public int? ApplicantId { get; set; }
        public string Name { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public string VisaTypeId { get; set; }
        public int? ConsularServiceId { get; set; }
        public DateTime? DocumentCollectionStartDate { get; set; }
        public DateTime? DocumentDeliveryDate { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
        public DateTime? DocumentCollectionDompletionDate { get; set; }
        public DateTime? ApplicationSubmissionDate { get; set; }
        public DateTime? ApplicationApprovalDate { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CommentsEntryVisaDto> CommentsEntryVisas { get; set; }
        public virtual ICollection<DocumentEntryVisaDto> DocumentEntryVisas { get; set; }
        //public virtual ICollection<ExtensionEntryVisaDto> ExtensionEntryVisas { get; set; }
        public virtual ICollection<ReminderEntryVisaDto> ReminderEntryVisas { get; set; }
    }
    public class EntryVisaSelectDto
    {
        public EntryVisaSelectDto()
        {
            CommentsEntryVisas = new HashSet<CommentsEntryVisaDto>();
            DocumentEntryVisas = new HashSet<DocumentEntryVisaSelectDto>();
            //ExtensionEntryVisas = new HashSet<ExtensionEntryVisaDto>();
            ReminderEntryVisas = new HashSet<ReminderEntryVisaDto>();
        }
        public int Id { get; set; }
        public int WorkOrderServicesId { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int StatusId { get; set; }
        public int? ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public string Relationship { get; set; }
        public string Name { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string City { get; set; }
        public string VisaTypeId { get; set; }
        public int? ConsularServiceId { get; set; }
        public DateTime? DocumentCollectionStartDate { get; set; }
        public DateTime? DocumentDeliveryDate { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
        public DateTime? DocumentCollectionDompletionDate { get; set; }
        public DateTime? ApplicationSubmissionDate { get; set; }
        public DateTime? ApplicationApprovalDate { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<CommentsEntryVisaDto> CommentsEntryVisas { get; set; }
        public virtual ICollection<DocumentEntryVisaSelectDto> DocumentEntryVisas { get; set; }
        //public virtual ICollection<ExtensionEntryVisaDto> ExtensionEntryVisas { get; set; }
        public virtual ICollection<ReminderEntryVisaDto> ReminderEntryVisas { get; set; }
    }
}
