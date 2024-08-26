using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using biz.premier.Entities;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.ClientPartner
{
    public class ServiceLocationsRepository : GenericRepository<biz.premier.Entities.ServiceLocation>, IServiceLocationsRepository
    {
        public ServiceLocationsRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetServiceLocationById(int id)
        {
            var consult = _context.ServiceLocations
                .Where(x => x.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.IdClientPartnerProfile,
                    c.IdServiceLine,
                    c.IdService,
                    c.NickName,
                    ServiceLocationCountries = c.ServiceLocationCountries
                    .Where(x => x.IdServiceLocation == c.Id)
                    .Select(v => new { 
                        v.Id,
                        v.IdCountry,
                        country_name = v.IdCountryNavigation.Name,
                        v.IdServiceLocation,
                        v.ScopeDescription,
                        v.StandarScopeDocuments,
                        DocumentLocationCountries = v.DocumentLocationCountries.Where(x => x.IdServiceLocationCountry == v.Id),
                        TotalDocumentLocationCountries = v.DocumentLocationCountries.Where(x => x.IdServiceLocationCountry == v.Id).Count()
                    })
                }).ToList();

            return new ObjectResult(consult);
        }

        public ServiceLocation UpdateCustom(ServiceLocation serviceLocation, int key)
        {
            if (serviceLocation == null) 
                return null;
            var exist = _context.ServiceLocations
                .Include(i => i.ServiceLocationCountries)
                .ThenInclude(i => i.DocumentLocationCountries)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(serviceLocation);
                // LOCATION COUNTRIES
                foreach (var i in serviceLocation.ServiceLocationCountries)
                {
                    var country = exist.ServiceLocationCountries.FirstOrDefault(p => p.Id == i.Id);
                    if (country == null)
                    {
                        exist.ServiceLocationCountries.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(country).CurrentValues.SetValues(i);
                        // DOCUMENTS
                        foreach (var documentLocationCountry in i.DocumentLocationCountries)
                        {
                            var photo = country.DocumentLocationCountries.FirstOrDefault(p => p.Id == documentLocationCountry.Id);
                            if (photo == null)
                            {
                                country.DocumentLocationCountries.Add(documentLocationCountry);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(photo).CurrentValues.SetValues(documentLocationCountry);
                            }
                        }
                    }
                }

                _context.SaveChanges();
            }

            return exist;
        }
    }
}
