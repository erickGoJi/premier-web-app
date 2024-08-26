using biz.premier.Entities;
using biz.premier.Models;
using biz.premier.Paged;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace biz.premier.Repository.ServiceRecord
{
    public interface IServiceRecordRepository : IGenericRepository<Entities.ServiceRecord>
    {
        PagedList<Entities.ServiceRecord> GetAllPagedCustom(int pageNumber, int pageSize, bool? vip, int? client,
            int? partner, int? country, int? city, int? supplier);
        //Entities.ServiceOrder AddServiceOrder(Entities.ServiceOrder order, Entities.DependentInformation dependent);
        Entities.ServiceRecord AddNewService(Entities.ServiceRecord record);
        Entities.ServiceRecord UpdateCustom(Entities.ServiceRecord record, int key);
        int getHomeCountry(int id);
        Entities.ServiceRecord SelectCustom(int key, int user);
        ActionResult GetServiceRecordByIdApp(int sr);
        string returnHomeCountry(int id);
        string returnHomeCity(int id);
        ActionResult GetServiceRecord(bool? vip, int? client,
            int? partner, int? country, int? city, int? supplier, DateTime? startDate, 
            DateTime? endDate, string sr, int? status, int? coordiantor, string serviceLine, int user);
        ActionResult GetServiceRecordListApp(int? partner, DateTime? startDate, DateTime? endDate, int? status, bool? pending_acceptance_services, bool? pending_activity_reports, int user);
        ActionResult GetProfile(int pageNumber, int pageSize);
        string CompressString(string text);
        string DecompressString(string compressedText);
        string UploadImageBase64(string image);
        ActionResult GetServices(int service_record_id, int type, int? status, int? deliverTo, int? serviceType, int? program, int? userId);
        ActionResult GetApplicant(int sr);

        ActionResult GetChildrenBySrId(int sr);
        List<Entities.DependentInformation> GetDependets(int key);

        List<Entities.LeaseSignator> GetSignatorsbyId(int key);

        List<Entities.SecurityDeposit> GetSecurityDeposit(int key);
        List<Entities.InitialRentPayment> GetInitialRentPayment(int key);
        List<Entities.OngoingRentPayment> GetOngoingRentPayment(int key);
        List<Entities.RealtorCommission> GetRealtorCommission(int key);

        Tuple<bool, string> CompleteService(int key, int serviceLine);
        List<Entities.WorkOrder> GetWorkOrdersByServiceLine(int sr, int sl);
        ActionResult GetServiceByWorkOrder(int wo, int idUser);
        ActionResult GetServiceByServiceLine(int sr, int sl, int idUser);

        ActionResult GetServiceByServiceLineApp(int sr, int sl, int idUser);
        Boolean AcceptOrRejectCoordinator(int coordinator, bool accepted, int sr);
        Boolean AccpetOrRejectRelocationCoordinator(int coordinator, bool accepted, int sr);
        Boolean AccpetImmigrationSupplierPartner(int supplier, bool accepted, int sr);
        bool AccpetImmigrationSupplierPartnerIndividual(int supplier, bool accepted, int sr, int ServiceOrderServicesId);
        Boolean AccpetRelocationSupplierPartner(int supplier, bool accepted, int sr);
        bool AccpetRelocationSupplierPartnerIndividual(int supplier, bool accepted, int sr, int ServiceOrderServicesId);
        ActionResult GetDashboard(int user, string serviceLine, int? country, int? city, int? partner, int? client, int? coordinator, int? supplier, int? status, DateTime? rangeDate1, DateTime? rangeDate2);
        ActionResult GetCalendar(int user, int? serviceLine, int? country, int? city, int? partner, int? client, int? coordinator, int? supplier, DateTime? rangeDate1, DateTime? rangeDate2);
        ActionResult GetFollowing(int user, int? sr, int? coordinator);
        ActionResult GetFollowingsSr(int use);
        ActionResult GetFollowingCoordinators(int user);
        ActionResult GetCoordinators(int user, int? country, int? city, int? serviceLine, int? office);
        ActionResult GetEscalation(int user, Boolean? status, DateTime? rangeDate1,DateTime? rangeDate2, int? level, int? client, int? supplierPartner, 
            int? city, int? partner, int? country, int? serviceLine, int? office);
        Task<ActionResult> GetPendingAuthorizations(int user, int? country, int? city, int? service_line, int? sr);
        ActionResult GetUpcomingArrivals(int user, DateTime? rangeDate1, DateTime? rangeDate2, int? city, int? partner, int? client, int? coordinator, int? supplierPartner);
        ActionResult GetCalls(int user, string caller, int? sr, int? wo, int? service);
        ActionResult GetworkOrderBySR(int sr);
        ActionResult GetReminders(int user, DateTime? rangeDate1, DateTime? rangeDate2, int? city, int? sr, int? sl, int? wo);
        ActionResult AddReminder(int user, int service, DateTime? reminderDate, int category, string comment);
        ActionResult SupplierPartnersActive(int? country, int? city, int? supplierPartner, DateTime? range1, DateTime? range2);
        //ActionResult GetActivity(int? office, int? status, int? serviceLine, DateTime? range1, DateTime? range2);
        ActionResult GetReportByCountry(int country, int? partner, int? client, int? supplierPartner, DateTime? range1, DateTime? range2,
            int? status, int? serviceLine, int? serviceCategory, int? city);
        ActionResult GetReportByClientPartner(int country, int partner, int? client, int? supplierPartner, DateTime? range1, DateTime? range2,
            int? status, int? serviceLine, int? serviceCategory, int? city);
        ActionResult GetServiceRecordByStatus(int status, int? country, int? city, int? serviceLine, int? serviceRecord, int? partner, int? client, int? supplierPartner,
            DateTime? range1, DateTime? range2, int? serviceCategory);
        ActionResult GetReportBySupplierPartner(int? country, int? partner, int? client, int supplierPartner, DateTime? range1, DateTime? range2, int? status,
            int? serviceLine, int? serviceCategory, int? city, int? consultant);
        ActionResult GetAllActiveServices(int? country);
        ActionResult GetCountryByServiceRecord(int sr);
        ActionResult GetServiceRecordByUserApp(int user);
        ActionResult GetCategoryByCountry(int idcountry, int IdClientPartnerProfile, int IdserviceLine);
        ActionResult GetDeliveredTo(int service, int category, int serviceType);
        ActionResult CompleteReport(int sr, int serviceLine);
        ActionResult CompleteReportInfo(int sr);
        //ActionResult GetCountriesSr(int sr);
        ActionResult GetCoordinatorsAndSupplierBySr(int sr);
        ActionResult GetEmails(int sr);
        bool IsNewServiceRecord(int sr);
        ActionResult GetSuppliersConsultantAndServiceBySr(int sr);
        Tuple<bool, string> OnHoldServiceRecord(int sr, int serviceLine);
        bool DeleteCoordinator(int coordinator, int serviceLine);
        Tuple<bool, int> DeleteSupplier(int supplier, int serviceLine);
        int GetUserNotification(int idUserProfile);
        int GetUserBySupplier(int user);
        List<int> GetUserByCoordinatorImm(int sr);
        List<int> GetUserByCoordinatorRelo(int sr);
        ActionResult getServicesBySupplierAdmin(int sr, int? userId);
        ActionResult getServicesBySupplier(int sr, int idUser);
        int GetServicesSupplierCount(int id);
        List<biz.premier.Entities.WorkOrder> GetWorkOrderByServiceRecord(int sr);
        Boolean IsAssigned(int swo);
        List<biz.premier.Models.DashboardDto> GetDashboardTest(int user, string serviceLine, int? country, int? city, int? partner, int? client,
         int? coordinator, int? supplier, int? status, DateTime? rangeDate1, DateTime? rangeDate2);
        Tuple<int, string> SetStatusServiceRecord(int sr, int serviceLine);
        string returnClient(int id);
        string returnPartner(int id);
        ActionResult GetServicesApp(int service_record_id, int? status, int? serviceLine, int? user);
        ActionResult GetAssigneeInformationApp(int service_record_id);

        AssigneeInformation GetAssigneeInformationAppAll(int service_record_id);
        ActionResult GetDashboardApp(int user);
        ActionResult GetDashboardAppAssigne(int user, int? serviceLine);
        ActionResult GetInboxHome(int userId);
        ActionResult GetMessageHome(int userId);
        ActionResult GetMessageNotificatiosApp(int userId);
        bool ValidateEmailDependent(string email);
        ActionResult GetChangeNotificationsToRead(int userId);
        ActionResult GetReportDayApp(int id_ReporDay);
        bool DeleteSupplierChat(int serviceRecordId, int userReciver);
        int GetRole(int user);
        ActionResult GetServicesAppAssignee(int? status, int? serviceLine, int? user);
    }
}
