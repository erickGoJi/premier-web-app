using api.premier.Models.Catalogos;
using api.premier.Models.HousingSpecification;
using api.premier.Models.PropertyReport;
using api.premier.Models.ServiceOrder;
using api.premier.Models.SupplierPartnerProfileService;
using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.premier.Models.WorkOrder
{
    public class HousingListDto
    {
        public int Id { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? ServiceType { get; set; }
        public int? PropertyNo { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Supplier { get; set; }
        public DateTime? VisitDate { get; set; }
        public int? HousingStatus { get; set; }
        public int? PropertyType { get; set; }
        public string Address { get; set; }
        public int? Zip { get; set; }
        public string WebSite { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? ParkingSpaces { get; set; }
        public decimal? Price { get; set; }
        public int? Currency { get; set; }
        public string AdditionalComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? IdServiceDetail { get; set; }
        public string Othersupplier { get; set; }
        public string Suppliertelephone { get; set; }
        public string Supplieremail { get; set; }

        public int? Shared { get; set; }

        public string Neighborhood { get; set; }
        public string VisitTime { get; set; }

        public bool? Sample { get; set; }

        public bool? Wassended { get; set; }

        public decimal? Size { get; set; }
        public int? MetricId { get; set; }


        public int? WorkOrderServicesId { get; set; }
        public int? CatCategoryId { get; set; }
        public string CatCategoryText { get; set; }
        public DateTime? DateCreatedInApp { get; set; }
        public bool? CreatedInApp { get; set; }

        public int? IdCity { get; set; }

        //public virtual CatCurrencyDto CurrencyNavigation { get; set; }

        //public virtual CatStatusHousingDto HousingStatusNavigation { get; set; }
        //public virtual CatPropertyTypeHousingDto PropertyTypeNavigation { get; set; }
        //public virtual CatSupplierDto SupplierNavigation { get; set; }
        //public virtual CatSupplierCompanyDto SupplierPartnerNavigation { get; set; }
        //public virtual WorkOrderDto WorkOrderNavigation { get; set; }
        public virtual ICollection<ContractDetailDto> ContractDetails { get; set; }
        public virtual ICollection<DepartureDetailsHomeDto> DepartureDetailsHomes { get; set; }
        // public virtual ICollection<LandlordDetailsHomeDto> LandlordDetailsHomes { get; set; }
        public virtual ICollection<LandlordHeaderDetailsHomeDto> LandlordHeaderDetailsHomes { get; set; }
        public virtual ICollection<RenewalDetailHomeDto> RenewalDetailHomes { get; set; }
        public virtual ICollection<AmenitiesHousingListDto> AmenitiesHousingLists { get; set; }
        public virtual ICollection<CommentHousingDto> CommentHousings { get; set; }
        public virtual ICollection<DocumentHousingDto> DocumentHousings { get; set; }
        public virtual ICollection<HousingStatusHistoryDto> HousingStatusHistories { get; set; }

        // public virtual ICollection<CostSavingHomeDto> CostSavingHomes { get; set; }

        public virtual ICollection<GroupCostSavingDto> GroupCostSavings { get; set; }
        public virtual ICollection<GroupPaymentsHousingDto> GroupPaymnetsHousings { get; set; }

        public virtual ICollection<HousingReportDto> HousingReports { get; set; }
        // public virtual ICollection<InspectionDto> Inspections { get; set; }

       // public virtual ICollection<PaymentHousingDto> PaymentHousings { get; set; }
        public virtual ICollection<PropertyReportDto> PropertyReports { get; set; }
        // public virtual ICollection<RepairDto> Repairs { get; set; }

        public virtual ICollection<GroupIrDto> GroupIrs { get; set; }
    }

    public class HousingListSelectDto
    {
        public int Id { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? ServiceType { get; set; }
        public int? PropertyNo { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Supplier { get; set; }
        public DateTime? VisitDate { get; set; }
        public int? HousingStatus { get; set; }
        public int? PropertyType { get; set; }
        public string Address { get; set; }
        public int? Zip { get; set; }
        public string WebSite { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? ParkingSpaces { get; set; }
        public decimal? Price { get; set; }
        public int? Currency { get; set; }
        public string AdditionalComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? IdServiceDetail { get; set; }

        public string Othersupplier { get; set; }
        public string Suppliertelephone { get; set; }
        public string Supplieremail { get; set; }

        public int? Shared { get; set; }

        public string Neighborhood { get; set; }
        public string VisitTime { get; set; }

        public bool? Sample { get; set; }

        public bool? Wassended { get; set; }

        public int? MetricId { get; set; }

        public int? WorkOrderServicesId { get; set; }
        public int? CatCategoryId { get; set; }
        public int? CatCategoryText { get; set; }
        public DateTime? DateCreatedInApp { get; set; }
        public bool? CreatedInApp { get; set; }

        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual CatStatusHousingDto HousingStatusNavigation { get; set; }
        public virtual CatPropertyTypeHousingDto PropertyTypeNavigation { get; set; }
        //public virtual ConsultantContactsServiceDto SupplierNavigation { get; set; }
        public virtual AdministrativeContactsServiceDto SupplierNavigation { get; set; }
        public virtual SupplierPartnerProfileServiceDto SupplierPartnerNavigation { get; set; }
        public virtual WorkOrderDto WorkOrderNavigation { get; set; }
        public virtual ICollection<ContractDetailDto> ContractDetails { get; set; }
        public virtual ICollection<DepartureDetailsHomeDto> DepartureDetailsHomes { get; set; }
       // public virtual ICollection<LandlordDetailsHomeDto> LandlordDetailsHomes { get; set; }
        public virtual ICollection<RenewalDetailHomeDto> RenewalDetailHomes { get; set; }
        public virtual ICollection<AmenitiesHousingListDto> AmenitiesHousingLists { get; set; }

       // public virtual ICollection<CostSavingHomeDto> CostSavingHomes { get; set; }
        public virtual ICollection<CommentHousingSelectDto> CommentHousings { get; set; }
        public virtual ICollection<DocumentHousingDto> DocumentHousings { get; set; }
        public virtual ICollection<HousingReportDto> HousingReports { get; set; }
        public virtual ICollection<HousingStatusHistoryDto> HousingStatusHistories { get; set; }
        public virtual ICollection<GroupCostSavingDto> GroupCostSavings { get; set; }
        public virtual ICollection<GroupPaymentsHousingDto> GroupPaymnetsHousings { get; set; }

        // public virtual ICollection<PaymentHousingDto> PaymentHousings { get; set; }
        public virtual ICollection<InspectionDto> Inspections { get; set; }
        public virtual ICollection<PropertyReportSelectDto> PropertyReports { get; set; }
        public virtual ICollection<RepairDto> Repairs { get; set; }
    }

    public class HousingListSelectLSFDto
    {
        public int Id { get; set; }
        public int? WorkOrder { get; set; }
        public int? Service { get; set; }
        public int? ServiceType { get; set; }
        public int? PropertyNo { get; set; }
        public int? SupplierPartner { get; set; }
        public int? Supplier { get; set; }
        public DateTime? VisitDate { get; set; }
        public int? HousingStatus { get; set; }
        public int? PropertyType { get; set; }
        public string Address { get; set; }
        public int? Zip { get; set; }
        public string WebSite { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? ParkingSpaces { get; set; }
        public decimal? Price { get; set; }
        public int? Currency { get; set; }
        public string AdditionalComments { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? IdServiceDetail { get; set; }
        public string Othersupplier { get; set; }
        public string Suppliertelephone { get; set; }
        public string Supplieremail { get; set; }
        public int? Shared { get; set; }
        public string Neighborhood { get; set; }
        public string VisitTime { get; set; }

        public bool? Sample { get; set; }
        public bool? Wassended { get; set; }

        public int? MetricId { get; set; }

        public int? WorkOrderServicesId { get; set; }
        public int? CatCategoryId { get; set; }
        public int? CatCategoryText { get; set; }
        public DateTime? DateCreatedInApp { get; set; }
        public bool? CreatedInApp { get; set; }
        public virtual CatCurrencyDto CurrencyNavigation { get; set; }
        public virtual CatStatusHousingDto HousingStatusNavigation { get; set; }
        public virtual CatPropertyTypeHousingDto PropertyTypeNavigation { get; set; }
        //public virtual ConsultantContactsServiceDto SupplierNavigation { get; set; }
        public virtual AdministrativeContactsServiceDto SupplierNavigation { get; set; }
        public virtual SupplierPartnerProfileServiceDto SupplierPartnerNavigation { get; set; }
        public virtual WorkOrderDto WorkOrderNavigation { get; set; }
        public virtual ICollection<AmenitiesHousingListDto> AmenitiesHousingLists { get; set; }
        public virtual ICollection<CommentHousingSelectDto> CommentHousings { get; set; }
        public virtual ICollection<DocumentHousingDto> DocumentHousings { get; set; }
        public virtual ICollection<HousingReportDto> HousingReports { get; set; }
        public virtual ICollection<HousingStatusHistoryDto> HousingStatusHistories { get; set; }

        ////////////////////////// LSEASE SUMMARY ///////////////////////////////

        public virtual ICollection<ContractDetailDto> ContractDetails { get; set; }
        public virtual ICollection<DepartureDetailsHomeDto> DepartureDetailsHomes { get; set; }
      //  public virtual ICollection<LandlordDetailsHomeDto> LandlordDetailsHomes { get; set; }
        public virtual ICollection<LandlordHeaderDetailsHome> LandlordHeaderDetailsHomes { get; set; }

        //public virtual ICollection<LandlordHeaderDetailsHomeDto> Landlor_dHistoricHeaderDetailsHomes { get; set; }

        public virtual ICollection<RenewalDetailHomeDto> RenewalDetailHomes { get; set; }
        public virtual ICollection<GroupCostSavingDto> GroupCostSavings { get; set; }
        public virtual ICollection<GroupPaymentsHousingDto> GroupPaymnetsHousings { get; set; }

        ////////////////////////// LSEASE SUMMARY ///////////////////////////////

        ////////////////////////// INSPECTION & REPAIRS  ///////////////////////////////

        public virtual ICollection<InspectionDto> Inspections { get; set; }
        public virtual ICollection<PropertyReportSelectDto> PropertyReports { get; set; }
        public virtual ICollection<RepairDto> Repairs { get; set; }

        ////////////////////////// INSPECTION & REPAIRS  ///////////////////////////////
        ///
        
        

    }
}
