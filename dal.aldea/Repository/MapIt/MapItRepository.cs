using biz.premier.Entities;
using biz.premier.Repository.MapIt;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.MapIt
{
    public class MapItRepository : GenericRepository<biz.premier.Entities.MapIt>, IMapItRepository
    {
        public MapItRepository(Db_PremierContext context) : base(context) { }

        public ActionResult SelectCustom(int key)
        {
            var query = _context.MapIts
                .Include(i => i.Locations)
                .Where(x => x.Id == key)
                .Select(d => new
                {
                    d.Id,
                    d.ServiceRecord,
                    d.SupplierPartner,
                    SupplierPartnerName = $"{d.SupplierPartnerNavigation.Name} {d.SupplierPartnerNavigation.LastName} {d.SupplierPartnerNavigation.MotherLastName}",
                    d.ServiceLine,
                    ServiceLineName = d.ServiceLine == 1 ? "Immigration" :  "Relocation",
                    d.StartDate,
                    d.DriverName,
                    d.DriverContact,
                    d.Vehicle,
                    d.PlateNumber,
                    d.Comments,
                    d.CreatedBy,
                    d.CreatedDate,
                    d.UpdatedBy,
                    d.UpdatedDate,
                    Locations = d.Locations.Select(f => new { 
                        f.Id,
                        f.MapItId,
                        LocationType = f.LocationType,
                        LocationTypeName = f.LocationTypeNavigation.LocationType,
                        f.Service,
                        f.LocationName,
                        f.Address,
                        f.Longitude,
                        f.Latitude
                    }).ToList()
                }).ToList();
            //.Single(s => s.Id == key)
            return new ObjectResult(query);
        }


        public ActionResult GetMapIt(int ServiceLineId, int service_record_id)
        {
            var query = _context.MapIts
                .Where(x => x.ServiceLine == ServiceLineId && x.ServiceRecord == service_record_id)
                .Include(i => i.Locations)
                .Include(i => i.SupplierPartnerNavigation)
                .Select(s => new
                {
                    s.Id,
                    supplier = $"{s.SupplierPartnerNavigation.Name} {s.SupplierPartnerNavigation.LastName} {s.SupplierPartnerNavigation.MotherLastName}",
                    service_line = ServiceLineId == 1 ? "Immigration" : "Relocation",
                    s.StartDate,
                    completed_date = s.CompletedDate, //== null || s.CompletedDate == Convert.ToDateTime("01/01/1900") ? Convert.ToDateTime("--") : s.CompletedDate,
                    visit_places = s.Locations.Count(),
                    s.Comments
                }).ToList();
            return new ObjectResult(query);
        }

        public biz.premier.Entities.MapIt UpdateCustom(biz.premier.Entities.MapIt mapIt, int key)
        {
            if (mapIt == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.MapIt>()
                .Include(i => i.Locations)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(mapIt);
                foreach (var i in mapIt.Locations)
                {
                    var location = exist.Locations.Where(x => x.Id == i.Id).FirstOrDefault();
                    if (location == null)
                    {
                        exist.Locations.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(location).CurrentValues.SetValues(i);
                        _context.SaveChanges();
                    }
                }

            }
            return exist;
        }

        public ActionResult LocationType()
        {
            var service = _context.CatLocationTypes
                .Select(k => new
                {
                    k.Id,
                    k.LocationType
                }).OrderBy(x => x.LocationType).ToList();

            return new ObjectResult(service);
        }
    }
}
