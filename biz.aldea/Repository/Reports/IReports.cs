using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Reports
{
    public interface IReports : IGenericRepository<Entities.Appointment>
    {
        void createExcelAppointment(int service_record_id, string path);
        void createExcelHousing(int id_service_detail, string path);
        void createExcelSchooling(int wo_id, string path);
        void createPDF(string xUrl, string filePath, int miliseconds);
        void pdfExample();
    }
}
