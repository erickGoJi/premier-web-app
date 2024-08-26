using api.premier.Models.AssigneeInformation;
using api.premier.Models.Catalogos;
using api.premier.Models.Follow;
using api.premier.Models.HousingSpecification;
using api.premier.Models.ImmigrationCoodinator;
using api.premier.Models.ImmigrationProfile;
using api.premier.Models.ImmigrationSupplierPartner;
using api.premier.Models.RelocationCoordinator;
using api.premier.Models.RelocationSupplierPartner;
using api.premier.Models.ServiceOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ServiceRecord
{
    public class ServiceRecordSelectDto
    {
        public ServiceRecordSelectDto()
        {
            ImmigrationCoodinators = new HashSet<ImmigrationCoodinatorSelectDto>();
            ImmigrationSupplierPartners = new HashSet<ImmigrationSupplierPartnerDto>();
            RelocationCoordinators = new HashSet<RelocationCoordinatorSelectDto>();
            RelocationSupplierPartners = new HashSet<RelocationSupplierPartnerDto>();
            WorkOrders = new HashSet<WorkOrderDto>();
        }
        public int Id { get; set; }
        public string NumberServiceRecord { get; set; }
        public DateTime InitialAutho { get; set; }
        public DateTime InithialAuthoAcceptance { get; set; }
        public int? PartnerId { get; set; }
        public int ClientId { get; set; }
        public string ClientFileNumber { get; set; }
        public int? AuthorizedByImmigration { get; set; }
        public int? AuthorizedByRelocation { get; set; }
        public bool? CopyOnEmailImmigration { get; set; }
        public bool? CopyOnEmailRelocation { get; set; }
        public bool? SpocImmigration { get; set; }
        public bool? SpocRelocation { get; set; }
        public bool? Vip { get; set; }
        public bool? Urgent { get; set; }
        public bool? ConfidentialMove { get; set; }
        public string SpecialIntructions { get; set; }
        public int? StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Office { get; set; }
        public virtual string Client { get; set; }
        public virtual CatOfficeDto OfficeNavigation { get; set; }
        public virtual string Partner { get; set; }
        public virtual CatStatusDto Status { get; set; }
        public virtual ICollection<AssigneeInformationDto> AssigneeInformations { get; set; }
        public virtual ICollection<ImmigrationCoodinatorSelectDto> ImmigrationCoodinators { get; set; }
        public virtual ICollection<ImmigrationSupplierPartnerDto> ImmigrationSupplierPartners { get; set; }
        public virtual ICollection<RelocationCoordinatorSelectDto> RelocationCoordinators { get; set; }
        public virtual ICollection<RelocationSupplierPartnerDto> RelocationSupplierPartners { get; set; }
        public virtual ICollection<WorkOrderDto> WorkOrders { get; set; }
        public virtual ICollection<FollowDto> Follows { get; set; }
        public virtual ICollection<HousingSpecificationDto> HousingSpecifications { get; set; }
        public virtual ICollection<ImmigrationProfileInsertDto> ImmigrationProfiles { get; set; }



    }
    public class ServiceRecordSelectCustomDto
    {
        public int Id { get; set; }
        public DateTime InitialAutho { get; set; }
        public string client { get; set; }
        public string partner { get; set; }
        public bool status { get; set; }
        public bool Spoc { get; set; }
        public bool Vip { get; set; }
        public bool? Urgent { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public string assigneeName { get; set; }
        public int services { get; set; }
        public string coordinator { get; set; }
        public string supplierConsultant { get; set; }
    }
}
