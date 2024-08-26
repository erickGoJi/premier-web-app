using biz.premier.Entities;
using biz.premier.Repository.AdminCenter;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.AdminCenter
{
    public class ServiceRepository : GenericRepository<biz.premier.Entities.Service>, IServiceRepository
    {
        public ServiceRepository(Db_PremierContext context) : base(context) { }

        public Service GetCustom(int key)
        {
            if (key == 0)
                return null;
            var service = _context.Services
                .Include(i => i.Service1Navigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.DocumentServiceCountries)
                        .ThenInclude(i => i.PrivacyNavigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.DocumentServiceCountries)
                        .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.DocumentServiceCountries)
                        .ThenInclude(i => i.StatusNavigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.CountryNavigation)
                .SingleOrDefault(s => s.Id == key);
            return service;
        }

        public Service UpdateCustom(Service service, int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Services
                .Include(i => i.Service1Navigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.DocumentServiceCountries)
                        .ThenInclude(i => i.PrivacyNavigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.DocumentServiceCountries)
                        .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.DocumentServiceCountries)
                        .ThenInclude(i => i.StatusNavigation)
                .Include(i => i.ServiceCountries)
                    .ThenInclude(i => i.CountryNavigation)
                .SingleOrDefault(s => s.Id == key);
            if(exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(service);
                foreach (var country in service.ServiceCountries)
                {
                    var existingCountry = exist.ServiceCountries.Where(p => p.Id == country.Id).FirstOrDefault();
                    if (existingCountry == null)
                    {
                        exist.ServiceCountries.Add(country);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingCountry).CurrentValues.SetValues(country);
                        foreach (var document in country.DocumentServiceCountries)
                        {
                            var existingDocument = existingCountry.DocumentServiceCountries.Where(p => p.Id == document.Id).FirstOrDefault();
                            if (existingDocument == null)
                            {
                                existingCountry.DocumentServiceCountries.Add(document);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(existingDocument).CurrentValues.SetValues(document);
                            }
                        }
                    }
                }
                _context.SaveChanges();
            }
            return service;
        }

        public ActionResult GetScopeDocuments_viejo(int service, int client)
        {
            var workOrderData = _context.WorkOrderServices.Where(x => x.Id == service).Select(s => new
            {
                country = s.StandaloneServiceWorkOrders.Any()
                    ? s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringIn
                    : s.BundledServices.FirstOrDefault().DeliveringIn,
                service = s.StandaloneServiceWorkOrders.Any()
                    ? s.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                    : s.BundledServices.FirstOrDefault().ServiceId
            }).FirstOrDefault();
            var clientData = _context.ServiceLocationCountries
                    .Include(i => i.IdServiceLocationNavigation)
                        .ThenInclude(i => i.ServiceLocationCountries)
                            .Include(i => i.DocumentLocationCountries)
                .Where(x => x.IdServiceLocationNavigation.IdClientPartnerProfile == client).Select(s => new
                {
                    s.ScopeDescription,
                    s.IdCountry,
                    s.IdServiceLocationNavigation.IdService,
                    Documentcountries = s.StandarScopeDocuments == 0
                        ? s.DocumentLocationCountries.Select(q => new
                        {
                            q.PrivacyNavigation.Privacy,
                            q.FileName,
                            q.FileRequest,
                            q.StatusNavigation.Status,
                            q.IdDocumentTypeNavigation.DocumentType
                        }).ToList()
                        : s.IdServiceLocationNavigation.IdServiceNavigation.Services.FirstOrDefault().ServiceCountries
                            .FirstOrDefault(x => x.Country == s.IdCountry).DocumentServiceCountries.Select(q => new
                            {
                                q.PrivacyNavigation.Privacy,
                                FileName = "",
                                FileRequest = q.FilePath,
                                q.StatusNavigation.Status,
                                q.DocumentTypeNavigation.DocumentType
                            }).ToList()
                }).ToList();
            var data = clientData.Where(x => x.IdCountry == workOrderData.country 
                                          && x.IdService == workOrderData.service).ToList();
            return new ObjectResult(clientData.FirstOrDefault(x => x.IdCountry == workOrderData.country 
                                          && x.IdService == workOrderData.service));
        }

        public ActionResult GetScopeDocuments(int wos_id, int client)
        {


            var workOrderData = _context.WorkOrderServices.Where(x => x.Id == wos_id).Select(s => new
            {
                country = s.StandaloneServiceWorkOrders.Any()
                    ? s.StandaloneServiceWorkOrders.FirstOrDefault().DeliveringIn
                    : s.BundledServices.FirstOrDefault().DeliveringIn,
                service = s.StandaloneServiceWorkOrders.Any()
                    ? s.StandaloneServiceWorkOrders.FirstOrDefault().ServiceId
                    : s.BundledServices.FirstOrDefault().ServiceId,
                clie_id = s.StandaloneServiceWorkOrders.Any()
                    ? s.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecord.ClientId
                    : s.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecord.ClientId

            }).FirstOrDefault();

            int dd = 0;

            var clientData = _context.ServiceLocationCountries
                    .Include(i => i.IdServiceLocationNavigation)
                        .ThenInclude(i => i.ServiceLocationCountries)
                            .Include(i => i.DocumentLocationCountries)
                .Where(x => x.IdServiceLocationNavigation.IdClientPartnerProfile == workOrderData.clie_id).Select(s => new
                {
                    ScopeDescription = s.StandarScopeDocuments == 0 
                            ? s.ScopeDescription
                            : s.IdServiceLocationNavigation.IdServiceNavigation.Services.FirstOrDefault().ServiceCountries.FirstOrDefault(x => x.Country == s.IdCountry).ScopeDescription == null 
                                 ? "Client Service Scope not captured or empty." : s.IdServiceLocationNavigation.IdServiceNavigation.Services.FirstOrDefault().ServiceCountries.FirstOrDefault(x => x.Country == s.IdCountry).ScopeDescription,
                    s.IdCountry,
                    s.IdServiceLocationNavigation.IdService,
                    s.StandarScopeDocuments,
                    Documentcountries = s.StandarScopeDocuments == 0
                        ? s.DocumentLocationCountries.Select(q => new
                        {
                            q.PrivacyNavigation.Privacy,
                            q.FileName,
                            q.FileRequest,
                            q.StatusNavigation.Status,
                            q.IdDocumentTypeNavigation.DocumentType,
                            updated_date = q.UploadDate
                        }).ToList()
                        : s.IdServiceLocationNavigation.IdServiceNavigation.Services.FirstOrDefault().ServiceCountries
                            .FirstOrDefault(x => x.Country == s.IdCountry).DocumentServiceCountries.Select(q => new
                            {
                                q.PrivacyNavigation.Privacy,
                                FileName = "",
                                FileRequest = q.FilePath,
                                q.StatusNavigation.Status,
                                q.DocumentTypeNavigation.DocumentType,
                                updated_date = q.UpdatedDate == null ? DateTime.Now : q.UpdatedDate.Value
                            }).ToList()
                }).ToList();
            var data = clientData.Where(x => x.IdCountry == workOrderData.country
                                          && x.IdService == workOrderData.service).ToList();
            return new ObjectResult(clientData.FirstOrDefault(x => x.IdCountry == workOrderData.country
                                          && x.IdService == workOrderData.service));
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            bool isSuccess = false;
            var documentLocationCountries = await _context.DocumentServiceCountries.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (documentLocationCountries != null)
            {
                _context.DocumentServiceCountries.Remove(documentLocationCountries);
                await _context.SaveChangesAsync();
                isSuccess = true;
            }
            return isSuccess;
        }

        public bool IsExistServiceInCountry(int country, int service) {
            return _context.ServiceCountries.Any(x => x.Service == service && x.Country == country);
        }

        public bool IsExistService(int service)
        {
            return _context.Services.Any(x => x.Service1 == service);
        }

        public int searchServiceId(int service)
        {
            return _context.Services.FirstOrDefault(x => x.Service1 == service).Id;
        }
    }
}
