using api.premier.Models.AssigneeInformation;
using api.premier.Models.Catalogos;
using api.premier.Models.ImmigrationCoodinator;
using api.premier.Models.ImmigrationSupplierPartner;
using api.premier.Models.RelocationCoordinator;
using api.premier.Models.RelocationSupplierPartner;
using api.premier.Models.ServiceOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.ServiceRecord
{
    public class ServiceRecordInsertDto
    {
        public ServiceRecordInsertDto()
        {
            ImmigrationCoodinators = new HashSet<ImmigrationCoodinatorDto>();
            ImmigrationSupplierPartners = new HashSet<ImmigrationSupplierPartnerDto>();
            RelocationCoordinators = new HashSet<RelocationCoordinatorDto>();
            RelocationSupplierPartners = new HashSet<RelocationSupplierPartnerDto>();
            //ServiceOrders = new HashSet<ServiceOrder>();
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Initial autho date is required")]
        public DateTime InitialAutho { get; set; }
        [Required(ErrorMessage = "Initial autho acceptance date is required")]
        public DateTime InithialAuthoAcceptance { get; set; }
        [Required(ErrorMessage = "Partner is required.")]
        public int? PartnerId { get; set; }
        [Required(ErrorMessage = "Client is required.")]
        public int ClientId { get; set; }
        [Required(ErrorMessage = "Client file number is required.")]
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
        public virtual CatStatusDto Status { get; set; }
        public virtual ICollection<AssigneeInformationDto> AssigneeInformations { get; set; }
        public virtual ICollection<ImmigrationCoodinatorDto> ImmigrationCoodinators { get; set; }
        public virtual ICollection<ImmigrationSupplierPartnerDto> ImmigrationSupplierPartners { get; set; }
        public virtual ICollection<RelocationCoordinatorDto> RelocationCoordinators { get; set; }
        public virtual ICollection<RelocationSupplierPartnerDto> RelocationSupplierPartners { get; set; }
        public virtual ICollection<WorkOrderInsertDto> ServiceOrders { get; set; }
    }
}
