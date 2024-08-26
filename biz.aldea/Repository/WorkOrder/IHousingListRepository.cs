using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.WorkOrder
{
    public interface IHousingListRepository : IGenericRepository<HousingList>
    {
        HousingList UpdateCustom(HousingList housingList, int key);

        HousingList LogicDeleteHousing(int id);

        List<HousingStatusHistory> GetAllHistory(int key);

        HousingList GetCustom_new(int key, int servide_detail_id);

        HousingList GetCustom_historic(int key, int servide_detail_id);

        ActionResult GetHomeFindingHousingList(int id_service_detail);

        ActionResult GetHomestovist(int wosid, DateTime? dateViste);

        ActionResult GetHistoricHousingByTypeAndId(int key, int servide_detail_id);

        ActionResult GetHistoricHousingByServiceType(int key, int servide_detail_id, int type);
        HousingList GetCustom(int key);

        List<ContractDetail> GetLeaseVersions(int id_service_detail, int id_catCategoryId, int housing_list_id);

        List<GroupIr> GetInspectionsVersions(int id_service_detail, int id_catCategoryId, int housing_list_id);

        biz.premier.Entities.Departure get_departure(int id_service);
        DepartureDetailsHome AddDepartureDetailHome(DepartureDetailsHome departure);

        GroupCostSaving AddGroupCostSaving(GroupCostSaving group);

        GroupPaymnetsHousing AddGroupPaymnetsHousing(GroupPaymnetsHousing group);

        DepartureDetailsHome UpdateDepartureDetailHome(DepartureDetailsHome departure);
        RenewalDetailHome AddRenewalDetailHome(RenewalDetailHome renewalDetail);
        RenewalDetailHome UpdateRenewalDetailHome(RenewalDetailHome renewalDetail);
        CostSavingHome AddCostSavingHome(CostSavingHome renewalDetail);
        CostSavingHome UpdateCostSanvingHome(CostSavingHome renewalDetail);
        PaymentHousing AddPaymentHousing(PaymentHousing payment);

        PaymentSchooling AddPaymentSchooling(PaymentSchooling payment);

        PaymentRental AddPaymentRental(PaymentRental payment);

        PaymentSchooling UpdatePaymentSchooling(PaymentSchooling payment);
        PaymentHousing UpdatePaymentHousing(PaymentHousing payment);

        PaymentRental UpdatePaymentRental(PaymentRental payment);
        ContractDetail AddContractDetail(ContractDetail contract);
        ContractDetail UpdateContractDetail(ContractDetail contract);
        ActionResult AddLandlordDetailsHome(LandlordDetailsHome landlord);

        LandlordHeaderDetailsHome AddLandlordHeaderDetailsHomee(LandlordHeaderDetailsHome landlord);
        LandlordDetailsHome UpdateLandlordDetailsHome(LandlordDetailsHome landlord);

        ActionResult DeleteBankingDetails(int id_landlord);

        ActionResult DeleteExpense(int id);

        List<PhotosPropertyReportSection> delete_photo_section(int id);

        List<PhotosInventory> delete_photo_section_inventory(int id);

        List<PhotosInspec> delete_photo_inspection(int id);

        HousingList accept_reject_property(int id, int status_id);

        ActionResult deleteCostSavings(int id);

        ActionResult DeletePaymnetType(int id);

        ActionResult  deletePropertyReportSection(int id);

        ActionResult deleteSectionInventory(int id);

        ActionResult AddSectionInventory(SectionInventory si);

        ActionResult EditSectionInventory(SectionInventory si);

        ActionResult deleteAttend(int id);

        ActionResult deletKeyInventory(int id);

        ActionResult AddExpense(PropertyExpense pe);

        ActionResult EditExpense(PropertyExpense pe);

        ActionResult AddPropertyReportSection(PropertyReportSection prs);

        LandlordHeaderDetailsHome UpdateLandlordHeaderDetailsHome(LandlordHeaderDetailsHome landlord);

        

        PhotosInspec getPhotoInspecById(int id);

        List<int> UpdateSendPropertys(List<int> list);

        List<string> getdataasignado(List<string> list);
        Attendee AddAttendee(Attendee inspection);

        List<CatPaymentRepairResponsability> GetCatPaymentRepairResponsability();

        ActionResult UpdateStatusHousingList(int idHl, int status);
        ActionResult GetOnlyHomeDetail(int key);

        AttendeeInspec AddAttendeeInspec(AttendeeInspec attendee);

        ActionResult GetAttendeesTitles(int id_service_detail, int property_id, int sr_id);

        ActionResult save_ir_status(int status_id, int id);

        ActionResult save_ir_statusbyhousingid(int status_id, int type,  int id_service_detail, int id_permanent_home);

        ActionResult GetItemsSectionInventory(int section);
        Attendee UpdateAttendee(Attendee inspection);

        AttendeeInspec UpdateAttendeeInspec(AttendeeInspec inspection);
        KeyInventory AddKeyInventory(KeyInventory keyInventory);
        KeyInventory UpdateKeyInventory(KeyInventory keyInventory);


        ///////////////////////PERFOMANCE ///////////////////////////
        ///
         ActionResult GetOnlyPropertyDetails(int key, int servide_detail_id, int type);
         ActionResult GetOnlyPropertyLSF(int key, int servide_detail_id, int type);

        ActionResult GetLSFBySection(int key, int servide_detail_id, int section);

        ActionResult GetInspRepBySection(int key, int servide_detail_id, int section);
        string GetInspRepBySectionPrint(int key, int servide_detail_id, int section, string filepath);

        string GetLSFPrint(int key, int servide_detail_id, int type, string filepath);
        ActionResult GetOnlyPropertyInspRep(int key, int servide_detail_id, int type);
        ActionResult GetStatusIR(int servide_detail_id, int id_housing);
        ActionResult GetLSFPropertyPrint(int key, int servide_detail_id, int type);
        ActionResult GetIRPropertyPrint(int key, int servide_detail_id, int type);
        List<Inspection> AddInspection(Inspection inspection);
        List<Inspection> UpdateInspection(Inspection inspection);

        List<Inspection>  DeleteInspection(int id);

        List<PhotosInspec> DeletePhotoInspection(int id);
        List<Repair> AddRepair(Repair repair);

        List<Repair> UpdateRepair(Repair repair);

         List<Repair> DeleteRepair(int id);

        List<DocumentRepair> DeleteDocumentRepair(int id);

        ActionResult AddConsiderarion(SpecialConsideration pe);

        ActionResult EditConsideration(SpecialConsideration pe);
        ActionResult DeleteConsideration(int id);
    }
}
