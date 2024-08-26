using biz.premier.Entities;
using biz.premier.Repository.Catalogue;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace dal.premier.Repository.Catalogue
{
    public class SupplierRepository : GenericRepository<CatSupplier>, ISupplierRepository
    {
        public SupplierRepository(Db_PremierContext context): base(context) { }

        public ActionResult GetSupplierBySR(int ServiceLineId, int SR)
        {
            var service = _context.ServiceRecords
                .Where(x => x.Id == SR)
                .Select(k => new
                {
                    supplier = k.ImmigrationSupplierPartners.Any() ? k.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == k.Id).Select(c => new { 
                        c.SupplierId,
                        supplier_name = _context.CatSuppliers.FirstOrDefault(x => x.Id == c.SupplierId).Supplier
                    }).Distinct() 
                    : k.RelocationSupplierPartners.Any() ? k.RelocationSupplierPartners.Where(x => x.ServiceRecordId == k.Id).Select(c => new {
                        c.SupplierId,
                        supplier_name = _context.CatSuppliers.FirstOrDefault(x => x.Id == c.SupplierId).Supplier
                    }).Distinct() : null,
                }).ToList();

            return new ObjectResult(service);
        }
    }
}
