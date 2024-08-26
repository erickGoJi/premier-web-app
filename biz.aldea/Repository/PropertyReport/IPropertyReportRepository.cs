using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.PropertyReport
{
    public interface IPropertyReportRepository : IGenericRepository<Entities.PropertyReport>
    {
        Entities.PropertyReport UpdateCustom(Entities.PropertyReport propertyReport, int key);

        biz.premier.Entities.PropertyReportSection UpdateCustomSection(biz.premier.Entities.PropertyReportSection propertyReportSection, int key);

        List<Entities.PropertyReport> GetCustom(int key);
        Entities.PhotosInventory FindphotoInventory(int key);
        ActionResult GetsupplierPartner(int? supplier, int? supplier_company, int sr);

        List<biz.premier.Entities.StatusPropertyReport> GetAllStatusPropertyReport();
    }
}
