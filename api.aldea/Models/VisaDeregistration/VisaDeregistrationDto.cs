using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.VisaDeregistration
{
    public class VisaDeregistrationDto
    {
        public int Id { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int ApplicantId { get; set; }
        public int? StatusId { get; set; }
        public string Name { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? HostCountryId { get; set; }
        public int? HostCityId { get; set; }
        public string City { get; set; }
        public string VisaTypeId { get; set; }
        public int? ConsularServiceId { get; set; }
        public DateTime? CountryDepertureDate { get; set; }
        public DateTime? EmploymentLastDay { get; set; }
        public DateTime? DocumentCollectionStartDate { get; set; }
        public DateTime? DocumentCollectionCompletionDate { get; set; }
        public DateTime? DocumentDeliveryDate { get; set; }
        public DateTime? ApplicationSubmissionDate { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
        public DateTime? ApplicationApprovalDate { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatApplicant Applicant { get; set; }
        //public virtual CatConsularService ConsularService { get; set; }
        //public virtual CatCity HostCity { get; set; }
        //public virtual CatCountry HostCountry { get; set; }
        //public virtual ServiceOrderService ServicesOrderServices { get; set; }
        //public virtual CatVisaCategory VisaType { get; set; }
        public virtual ICollection<CommentVisaDeregistrationDto> CommentVisaDeregistrations { get; set; }
        public virtual ICollection<DocumentVisaDeregistrationDto> DocumentVisaDeregistrations { get; set; }
        public virtual ICollection<ReminderVisaDeregistrationDto> ReminderVisaDeregistrations { get; set; }
    }
    public class VisaDeregistrationSelcetDto
    {
        public string ApplicantName { get; set; }
        public string Relationship { get; set; }
        public int Id { get; set; }
        public DateTime AuthoDate { get; set; }
        public DateTime AuthoAcceptanceDate { get; set; }
        public int ApplicantId { get; set; }
        public int? StatusId { get; set; }
        public string Name { get; set; }
        public int? WorkOrderServicesId { get; set; }
        public DateTime? ServiceCompletionDate { get; set; }
        public int? HostCountryId { get; set; }
        public int? HostCityId { get; set; }
        public string City { get; set; }
        public string VisaTypeId { get; set; }
        public int? ConsularServiceId { get; set; }
        public DateTime? CountryDepertureDate { get; set; }
        public DateTime? EmploymentLastDay { get; set; }
        public DateTime? DocumentCollectionStartDate { get; set; }
        public DateTime? DocumentCollectionCompletionDate { get; set; }
        public DateTime? DocumentDeliveryDate { get; set; }
        public DateTime? ApplicationSubmissionDate { get; set; }
        public DateTime? DocumentExpirationDate { get; set; }
        public DateTime? ApplicationApprovalDate { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //public virtual CatApplicant Applicant { get; set; }
        //public virtual CatConsularService ConsularService { get; set; }
        //public virtual CatCity HostCity { get; set; }
        //public virtual CatCountry HostCountry { get; set; }
        //public virtual ServiceOrderService ServicesOrderServices { get; set; }
        //public virtual CatVisaCategory VisaType { get; set; }
        public virtual ICollection<CommentVisaDeregistrationDto> CommentVisaDeregistrations { get; set; }
        public virtual ICollection<DocumentVisaDeregistrationSelectDto> DocumentVisaDeregistrations { get; set; }
        public virtual ICollection<ReminderVisaDeregistrationDto> ReminderVisaDeregistrations { get; set; }
    }
}
