using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace biz.premier.Repository.Appointment
{
    public interface IAppointmentRepository : IGenericRepository<biz.premier.Entities.Appointment>
    {
        ActionResult GetAppointmentByServiceRecordId(int service_record_id);
        ActionResult GetAllAppointment(int service_record_id);
        Task<ActionResult> GetServicesByServiceRecordAndServiceLine(int sr, int sl);
        ActionResult SelectCustom(int key);
        ActionResult AddAppointmentHousing(int appointmentId, int housingId, bool action);
        ActionResult AddAppointmentSchooling(int appointmentId, int schoolingId, bool action);
        biz.premier.Entities.Appointment UpdateCustom(Entities.Appointment appointment);
        ActionResult GetAppointmentByAssigneeId(int assigneeId);
        ActionResult GetAppointmentByUser(int userId, int? serviceRecordId, int? status, DateTime? dateRange1, DateTime? dateRange2);
        Entities.ReportDay AddReport(int appointment, int user);
        Entities.ReportDay UpdateReport(int report, int appointment, int user);
        Task<ActionResult> GetServicesByServiceRecord(int sr);
        Tuple<string, bool> IsAvailable(int user, DateTime date, DateTime StartTime, DateTime EndTime);
        ActionResult GetAppointmentByIdApp(int key);
        ActionResult GetCountHousingSchoolAppointment(List<int> servicesRelated, DateTime appointmentDate);
        ActionResult GetAppointmentByAssignee(int userId, int? status, DateTime? dateRange1, DateTime? dateRange2);
    }
}
