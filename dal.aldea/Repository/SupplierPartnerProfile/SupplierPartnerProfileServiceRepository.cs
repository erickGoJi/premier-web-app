using biz.premier.Entities;
using biz.premier.Repository.SupplierPartnerProfile;
using CryptoHelper;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.SupplierPartnerProfile
{
    public class SupplierPartnerProfileServiceRepository : GenericRepository<SupplierPartnerProfileService>, ISupplierPartnerProfileServiceRepository
    {
        public SupplierPartnerProfileServiceRepository(Db_PremierContext context) : base(context) { }

        public SupplierPartnerProfileService GetCustom(int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.SupplierPartnerProfileService>()
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.TypeNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.CountryNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.DocumentAreasCoverageServices)
                        .ThenInclude(i => i.CityNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.DocumentAreasCoverageServices)
                        .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.DocumentAreasCoverageServices)
                        .ThenInclude(i => i.PrivacyNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.PaymentInformationServices)
                        .ThenInclude(i => i.WireTransferServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.PaymentInformationServices)
                        .ThenInclude(i => i.CreditCardPaymentInformationServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.CityAreasCoverageServices)
                        .ThenInclude(i => i.CityNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.AdministrativeContactsServices)
                        .ThenInclude(i => i.DocumentAdministrativeContactsServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.AdministrativeContactsServices)
                        .ThenInclude(i => i.ContactTypeNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.AdministrativeContactsServices)
                        .ThenInclude(i => i.DocumentAdministrativeContactsServices)
                            .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.AdministrativeContactsServices)
                        .ThenInclude(i => i.DocumentAdministrativeContactsServices)
                            .ThenInclude(i => i.PrivacyNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.LanguagesConsultantContactsServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.DocumentConsultantContactsServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.VehicleServices)
                            .ThenInclude(i => i.PhotosVehicleServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.VehicleServices)
                            .ThenInclude(i => i.VehicleTypeNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.VehicleServices)
                            .ThenInclude(i => i.DocumentVehicleServices)
                                .ThenInclude(i => i.CityNavigation)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.Campuses)
                .Include(i => i.SupplierPartnerDetails)
                    .ThenInclude(i => i.TypeVehiclesSupplierPartnerDetails)
                .Include(i => i.ServiceLineNavigation)
                .Include(i => i.SupplierTypeNavigation)
                .Include(i => i.StatusNavigation)
                .SingleOrDefault(s => s.Id == key);
            return exist;
        }

        public ActionResult GetSupplierPartners(int? supplierCategory, int? partnerType, int? country, int? city, int? status)
        {
            var consultant = _context.AreasCoverageConsultants.Select(s => new
            {
                s.SupplierPartnerProfileConsultantNavigation.Id,
                s.SupplierPartnerProfileConsultantNavigation.Status,
                StatusName = s.SupplierPartnerProfileConsultantNavigation.StatusNavigation.Status,
                supplierCategory = "Consultant supplier company",
                supplierPartner = s.SupplierPartnerProfileConsultantNavigation.ComercialName,
                supplierPartnerType = s.SupplierPartnerProfileConsultantNavigation.Immigration == true ? s.SupplierPartnerProfileConsultantNavigation.Immigration == true ? "Relocation & Immigration" : "Immigration" : s.SupplierPartnerProfileConsultantNavigation.Relocation == true ? "Relocation" : "N/A",
                supplierPartnerTypeId = 3,
                avatar = s.SupplierPartnerProfileConsultantNavigation.Photo == null || s.SupplierPartnerProfileConsultantNavigation.Photo == "" ? "Files/assets/avatar.png" : s.SupplierPartnerProfileConsultantNavigation.Photo,
                s.SupplierPartnerProfileConsultantNavigation.LuxuryVip,
                s.CountryNavigation.Name,
                s.Country,
                s.PrimaryCityNavigation.City,
                s.PrimaryCity,
                contactName = s.AdministrativeContactsConsultants.Count > 0 ? s.AdministrativeContactsConsultants.FirstOrDefault().ContactName : "N/A",
                phone = s.AdministrativeContactsConsultants.Count > 0 ? s.AdministrativeContactsConsultants.FirstOrDefault().PhoneNumber : "N/A",
                totalServices = 0,
                experience = "0%",
                contact = s.AdministrativeContactsConsultants.Count > 0 ? s.AdministrativeContactsConsultants.FirstOrDefault().Email : "N/A",
                s.CreatedDate,
                rol_id = 0
            }).ToList();
            var services = _context.AreasCoverageServices.Select(s => new
            {
                s.SupplierPartnerProfileServiceNavigation.Id,
                s.SupplierPartnerProfileServiceNavigation.Status,
                StatusName = s.SupplierPartnerProfileServiceNavigation.StatusNavigation.Status,
                supplierCategory = "Service supplier company",
                supplierPartner = s.SupplierPartnerProfileServiceNavigation.ComercialName,
                supplierPartnerType = s.SupplierPartnerProfileServiceNavigation.SupplierTypeNavigation.SupplierType,
                supplierPartnerTypeId = s.SupplierPartnerProfileServiceNavigation.SupplierTypeNavigation.Id,
                avatar = s.SupplierPartnerProfileServiceNavigation.Photo == null || s.SupplierPartnerProfileServiceNavigation.Photo == "" ? "Files/assets/avatar.png" : s.SupplierPartnerProfileServiceNavigation.Photo,
                s.SupplierPartnerProfileServiceNavigation.LuxuryVip,
                s.CountryNavigation.Name,
                s.Country,
                s.PrimaryCityNavigation.City,
                s.PrimaryCity,
                contactName = s.AdministrativeContactsServices.Count > 0 ? s.AdministrativeContactsServices.FirstOrDefault().ContactName : "N/A",
                phone = s.AdministrativeContactsServices.Count > 0 ? s.AdministrativeContactsServices.FirstOrDefault().PhoneNumber : "N/A",
                totalServices = 0,
                experience = "0%",
                contact = s.AdministrativeContactsServices.Count > 0 ? s.AdministrativeContactsServices.FirstOrDefault().Email : "N/A",
                s.CreatedDate,
                rol_id = 0
            }).ToList();
            var alones = _context.ProfileUsers.Where(x => x.SupplierType == 1 || x.SupplierType == 3).Select(s => new
            {
                s.Id,
                Status = s.Status.Value ? (int?)1 : (int?)2,
                StatusName = s.Status.Value ? "Active" : "Inactive",
                supplierCategory = s.AreasCoverage == null ? "Premier Consultant" : s.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.ComercialName + " Consultant" ,
                supplierPartner =  $"{s.Name} {s.LastName} {s.MotherLastName}",
                supplierPartnerType = s.SupplierTypeNavigation.SupplierType,
                supplierPartnerTypeId = s.SupplierTypeNavigation.Id,
                avatar = s.Photo == null || s.Photo == "" ? "Files/assets/avatar.png" : s.Photo,
                LuxuryVip = (bool?)false,
                s.CountryNavigation.Name,
                s.Country,
                s.CityNavigation.City,
                PrimaryCity = s.City,
                contactName = $"{s.Name} {s.LastName} {s.MotherLastName}",
                phone = s.PhoneNumber,
                totalServices = (from a in _context.AssignedServiceSupliers
                                 join d in _context.ImmigrationSupplierPartners on a.ImmigrationSupplierPartnerId equals d.Id
                                 where d.SupplierId == s.Id  select new   {  d.Id  }).Count() +
                                 (from a in _context.AssignedServiceSupliers
                                 join d in _context.RelocationSupplierPartners on a.RelocationSupplierPartnerId equals d.Id
                                 where d.SupplierId == s.Id
                                 select new { d.Id }).Count() 
                                 ,
                experience = "0%",
                contact = s.Email,
                s.CreatedDate,
                rol_id = s.User.RoleId
            }).Where(r=> r.rol_id == 3).ToList();
            var supplierPartners = consultant.Union(services).ToList();
            supplierPartners = supplierPartners.Union(alones).ToList();
            if (supplierCategory.HasValue)
            {
                if (supplierCategory.Value == 1)
                    supplierPartners = supplierPartners.Where(x => x.supplierCategory == "Consultant supplier company").ToList();
                if (supplierCategory.Value == 2)
                    supplierPartners = supplierPartners.Where(x => x.supplierCategory == "Service supplier company").ToList();
                if (supplierCategory.Value == 3)
                    supplierPartners = supplierPartners.Where(x => x.supplierCategory == "Premier Consultant").ToList();
                if (supplierCategory.Value == 4)
                    supplierPartners = supplierPartners.Where(x => x.supplierCategory != "Consultant supplier company"
                                                                   && x.supplierCategory != "Service supplier company"
                                                                   && x.supplierCategory != "Premier Consultant").ToList();
            }

            ////// se metio para evitar que se muetren compañias y consultores de compañias 

            supplierPartners = supplierPartners.Where(x => (x.supplierCategory == "Service supplier company")
            || (x.supplierCategory == "Premier Consultant") ).ToList();
            
              
            /////////////////////////////////



            if (partnerType.HasValue)
                supplierPartners = supplierPartners.Where(x => x.supplierPartnerTypeId == partnerType.Value).ToList();
            if (country.HasValue)
                supplierPartners = supplierPartners.Where(x => x.Country == country.Value).ToList();
            if (city.HasValue)
                supplierPartners = supplierPartners.Where(x => x.PrimaryCity == city.Value).ToList();
            if (status.HasValue)
                supplierPartners = supplierPartners.Where(x => x.Status == status.Value).ToList();

            return new ObjectResult(supplierPartners.OrderByDescending(x => x.Id));

        }

        public SupplierPartnerProfileService UpdatedCustom(SupplierPartnerProfileService supplierPartnerProfileService, int key)
        {
            if (supplierPartnerProfileService == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.SupplierPartnerProfileService>()
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.DocumentAreasCoverageServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.PaymentInformationServices)
                        .ThenInclude(i => i.WireTransferServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.PaymentInformationServices)
                        .ThenInclude(i => i.CreditCardPaymentInformationServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.CityAreasCoverageServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.AdministrativeContactsServices)
                        .ThenInclude(i => i.DocumentAdministrativeContactsServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.LanguagesConsultantContactsServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.DocumentConsultantContactsServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.VehicleServices)
                            .ThenInclude(i => i.PhotosVehicleServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.ConsultantContactsServices)
                        .ThenInclude(i => i.VehicleServices)
                            .ThenInclude(i => i.DocumentVehicleServices)
                .Include(i => i.AreasCoverageServices)
                    .ThenInclude(i => i.Campuses)
                .Include(i => i.SupplierPartnerDetails).ThenInclude(i => i.TypeVehiclesSupplierPartnerDetails)
                .SingleOrDefault(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(supplierPartnerProfileService);
                foreach (var i in supplierPartnerProfileService.SupplierPartnerDetails)
                {
                    var detail = exist.SupplierPartnerDetails.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (detail == null)
                    {
                        exist.SupplierPartnerDetails.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(detail).CurrentValues.SetValues(i);

                        detail.TypeVehiclesSupplierPartnerDetails.Clear();
                        //exist.SupplierPartnerDetails.Where(p => p.Id == i.Id).FirstOrDefault().TypeVehiclesSupplierPartnerDetails.Clear();
                        _context.SaveChanges();
                        foreach (var vehicle in i.TypeVehiclesSupplierPartnerDetails)
                        {
                            //exist.SupplierPartnerDetails.Where(p => p.Id == i.Id).FirstOrDefault().TypeVehiclesSupplierPartnerDetails.Add(vehicle);
                            detail.TypeVehiclesSupplierPartnerDetails.Add(vehicle);
                        }
                    }
                }

                foreach (var i in supplierPartnerProfileService.AreasCoverageServices)
                {
                    var areasCoverage = exist.AreasCoverageServices.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (areasCoverage == null)
                    {
                        exist.AreasCoverageServices.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(areasCoverage).CurrentValues.SetValues(i);
                        // DOCUMENTS 
                        foreach (var document in i.DocumentAreasCoverageServices)
                        {
                            var doc = areasCoverage.DocumentAreasCoverageServices.Where(p => p.Id == document.Id).FirstOrDefault();
                            if (doc == null)
                            {
                                areasCoverage.DocumentAreasCoverageServices.Add(document);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(doc).CurrentValues.SetValues(document);
                            }
                        }
                        // CITIES
                        foreach (var city in i.CityAreasCoverageServices)
                        {
                            var cities = areasCoverage.CityAreasCoverageServices.Where(p => p.City == city.City).FirstOrDefault();
                            if (cities == null)
                            {
                                areasCoverage.CityAreasCoverageServices.Add(city);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(cities).CurrentValues.SetValues(city);
                            }
                        }
                        // PAYMENT
                        foreach (var payment in i.PaymentInformationServices)
                        {
                            var _payment = areasCoverage.PaymentInformationServices.Where(p => p.Id == payment.Id).FirstOrDefault();
                            if (_payment == null)
                            {
                                areasCoverage.PaymentInformationServices.Add(payment);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(_payment).CurrentValues.SetValues(payment);
                                foreach (var wire in payment.WireTransferServices)
                                {
                                    var _wire = _payment.WireTransferServices.Where(p => p.Id == wire.Id).FirstOrDefault();
                                    if (_wire == null)
                                    {
                                        _payment.WireTransferServices.Add(wire);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_wire).CurrentValues.SetValues(wire);
                                    }
                                }
                                foreach (var cc in payment.CreditCardPaymentInformationServices)
                                {
                                    var _cc = _payment.CreditCardPaymentInformationServices.Where(p => p.CreditCard == cc.CreditCard).FirstOrDefault();
                                    if (_cc == null)
                                    {
                                        _payment.CreditCardPaymentInformationServices.Add(cc);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_cc).CurrentValues.SetValues(cc);
                                    }
                                }
                            }
                        }
                        // ADMINISTRATIVE
                        foreach (var administrative in i.AdministrativeContactsServices)
                        {
                            var _administrative = areasCoverage.AdministrativeContactsServices.Where(p => p.Id == administrative.Id).FirstOrDefault();
                            if (_administrative == null)
                            {
                                areasCoverage.AdministrativeContactsServices.Add(administrative);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(_administrative).CurrentValues.SetValues(administrative);
                                foreach (var document in administrative.DocumentAdministrativeContactsServices)
                                {
                                    var _document = _administrative.DocumentAdministrativeContactsServices.Where(p => p.Id == document.Id).FirstOrDefault();
                                    if (_document == null)
                                    {
                                        _administrative.DocumentAdministrativeContactsServices.Add(document);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_document).CurrentValues.SetValues(document);
                                    }
                                }
                            }
                        }
                        // CONSULTANT
                        foreach (var consultant in i.ConsultantContactsServices)
                        {
                            var _consultant = areasCoverage.ConsultantContactsServices.Where(p => p.Id == consultant.Id).FirstOrDefault();
                            if (_consultant == null)
                            {
                                areasCoverage.ConsultantContactsServices.Add(consultant);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(_consultant).CurrentValues.SetValues(consultant);
                                // DOCUMENT
                                foreach (var document in consultant.DocumentConsultantContactsServices)
                                {
                                    var _document = _consultant.DocumentConsultantContactsServices.Where(p => p.Id == document.Id).FirstOrDefault();
                                    if (_document == null)
                                    {
                                        _consultant.DocumentConsultantContactsServices.Add(document);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_document).CurrentValues.SetValues(document);
                                    }
                                }
                                // LANGUAGE
                                foreach (var languages in consultant.LanguagesConsultantContactsServices)
                                {
                                    var _languages = _consultant.LanguagesConsultantContactsServices.Where(p => p.Language == languages.Language).FirstOrDefault();
                                    if (_languages == null)
                                    {
                                        _consultant.LanguagesConsultantContactsServices.Add(languages);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_languages).CurrentValues.SetValues(languages);
                                    }
                                }
                                // VEHICLES
                                foreach (var vehicle in consultant.VehicleServices)
                                {
                                    var _vehicle = _consultant.VehicleServices.Where(p => p.Id == vehicle.Id).FirstOrDefault();
                                    if (_vehicle == null)
                                    {
                                        _consultant.VehicleServices.Add(vehicle);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        _context.Entry(_vehicle).CurrentValues.SetValues(vehicle);
                                        // DOCUMENT
                                        foreach (var document in vehicle.DocumentVehicleServices)
                                        {
                                            var _document = _vehicle.DocumentVehicleServices.Where(p => p.Id == document.Id).FirstOrDefault();
                                            if (_document == null)
                                            {
                                                _vehicle.DocumentVehicleServices.Add(document);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                _context.Entry(_document).CurrentValues.SetValues(document);
                                            }
                                        }
                                        // PHOTO
                                        foreach (var photo in vehicle.PhotosVehicleServices)
                                        {
                                            var _photo = _vehicle.PhotosVehicleServices.Where(p => p.Id == photo.Id).FirstOrDefault();
                                            if (_photo == null)
                                            {
                                                _vehicle.PhotosVehicleServices.Add(photo);
                                                _context.SaveChanges();
                                            }
                                            else
                                            {
                                                _context.Entry(photo).CurrentValues.SetValues(photo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // CAMPUS 
                        foreach (var campus in i.Campuses)
                        {
                            var doc = areasCoverage.Campuses.FirstOrDefault(p => p.Id == campus.Id);
                            if (doc == null)
                            {
                                areasCoverage.Campuses.Add(campus);
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(doc).CurrentValues.SetValues(campus);
                            }
                        }
                    }
                }
                _context.SaveChanges();
            }
            return exist;
        }

        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public ActionResult GetSupplierPartnerServiceByServices(int workOrderService, int? supplierType, int? serviceLine)
        {
            var standalone = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrderServiceId == workOrderService).Select(s => new
            {
                country = s.DeliveringIn
            }).ToList();
            var bundled = _context.BundledServices.Where(x => x.WorkServicesId == workOrderService).Select(s => new
            {
                country = s.DeliveringIn
            }).ToList();
            var countries = standalone.Union(bundled).ToList();
            var country = countries.FirstOrDefault().country;
            var supplierPartner = _context.SupplierPartnerProfileServices
                .Where(x => x.AreasCoverageServices.Select(s => s.Country).Contains(country))
                .Select(s => new
                {
                    s.Id,
                    s.Photo,
                    s.ServiceLineNavigation.ServiceLine,
                    serviceLineId = s.ServiceLine,
                    Immigration = s.Immigration,
                    Relocation = s.Relocation,
                    s.ComercialName,
                    s.LuxuryVip,
                    s.LegalName,
                    s.SupplierSince,
                    s.SupplierType,
                    ContactName = s.AreasCoverageServices.FirstOrDefault().AdministrativeContactsServices.FirstOrDefault(x => x.ContactType == 1).ContactName,
                    wireTransfer = _context.WireTransferServices.Where(x =>
                        x.PaymentInformationServiceNavigation.AreasCoverageServiceNavigation.SupplierPartnerProfileService == s.Id
                        && x.PaymentInformationServiceNavigation.AreasCoverageServiceNavigation.Country == country).Select(_s => new
                        {
                            _s.Id,
                            _s.AccountTypeNavigation.AccountType,
                            _s.AccountHoldersName,
                            _s.BankName,
                            _s.AccountNumber,
                            _s.InternationalPaymentAcceptance
                        }).ToList()
                }).ToList();
            if (supplierType.HasValue)
                supplierPartner = supplierPartner.Where(x => x.SupplierType == supplierType.Value).ToList();
            if (serviceLine.HasValue)
                supplierPartner = serviceLine.Value == 1 
                    ? supplierPartner.Where(x => x.Immigration == true).ToList()
                    : supplierPartner.Where(x => x.Relocation == true).ToList();
            return new ObjectResult(supplierPartner);
        }

        public ActionResult GetServiceProviderByServiceId(int workOrderService)
        {
            int country = 0;
            int service_type = 0;

            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == workOrderService)
                .Select(s => new
            {
                country = s.DeliveringIn,
                type = s.ServiceId
            }).ToList();

            if(standalone.Count > 0)
            {
                country = standalone.FirstOrDefault().country.Value;
                service_type = standalone.FirstOrDefault().type.Value;
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == workOrderService)
                .Select(s => new
                {
                     country = s.DeliveringIn
                    ,type = s.ServiceId
                }).ToList();
                
                if(bundled.Count > 0)
                {
                    country = bundled.FirstOrDefault().country.Value;
                    service_type = bundled.FirstOrDefault().type.Value;
                }
            }

            var supplierPartner = _context.SupplierPartnerProfileServices
                .Where(x => x.AreasCoverageServices.Select(s => s.Country).Contains(country))
                .Select(s => new
                {
                     s.Id
                    ,s.ComercialName 
                    ,s.SupplierType
                    ,
                    s.Photo,
                    s.ServiceLineNavigation.ServiceLine,
                    serviceLineId = s.ServiceLine,
                    Immigration = s.Immigration,
                    Relocation = s.Relocation, 
                    s.LuxuryVip,
                    s.LegalName,
                    s.SupplierSince,
                }).ToList();

            List<int> suptypes = get_supplierstype_byservicetype(service_type);

           // supplierPartner = supplierPartner.Where(x => suptypes.Contains(x.SupplierType.Value)).ToList();
            supplierPartner = supplierPartner.Where(x => suptypes.Any(y=> y == x.SupplierType.Value)).ToList();

            return new ObjectResult(supplierPartner);
        }

        public ActionResult GetServProvByServiceTypeCountry(int workOrderService, int _type)
        {
            int country = 0;
            int sup_type = 0;

            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == workOrderService)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId
                }).ToList();

            if (standalone.Count > 0)
            {
                country = standalone.FirstOrDefault().country.Value;
              //  service_type = standalone.FirstOrDefault().type.Value;
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == workOrderService)
                .Select(s => new
                {
                    country = s.DeliveringIn
                    ,
                    type = s.ServiceId
                }).ToList();

                if (bundled.Count > 0)
                {
                    country = bundled.FirstOrDefault().country.Value;
                  //  service_type = bundled.FirstOrDefault().type.Value;
                }
            }

            var supplierPartner = _context.SupplierPartnerProfileServices
                .Where(x => x.AreasCoverageServices.Select(s => s.Country).Contains(country))
                .Select(s => new
                {
                    s.Id
                    ,
                    s.ComercialName
                    ,
                    s.SupplierType
                    ,
                    s.Photo,
                    s.ServiceLineNavigation.ServiceLine,
                    serviceLineId = s.ServiceLine,
                    Immigration = s.Immigration,
                    Relocation = s.Relocation,
                    s.LuxuryVip,
                    s.LegalName,
                    s.SupplierSince,
                }).ToList();

            sup_type = _type;
            List<int> suptypes = get_supplierstype_bySupServType(sup_type);

            // supplierPartner = supplierPartner.Where(x => suptypes.Contains(x.SupplierType.Value)).ToList();
            supplierPartner = supplierPartner.Where(x => suptypes.Any(y => y == x.SupplierType.Value)).ToList();

            return new ObjectResult(supplierPartner);
        }


        public List<int> get_supplierstype_bySupServType(int service_type)
        {
            List<int> result = new List<int>();

            if (service_type == 21 || service_type == 26
                || service_type == 17 || service_type == 18 //, 22 Temporary Housing, 23 Rental Furniture,27 lease renewal
                || service_type == 22 || service_type == 27
                || service_type == 29 || service_type == 28) // 29 home purchase , 28 home sale
            {
                result.Add(5);
                result.Add(9);
            }

            if (service_type == 24 || service_type == 25) // 24 Trasnportation, 25 airport 
            {
                result.Add(10);
                result.Add(23);
            }

            if (service_type == 23)
            {
                result.Add(12);
            }

            if (service_type == 20)
            {
                result.Add(34);
            }
            if(service_type == 25000)
            {
                result.Add(26);
                result.Add(21);
                result.Add(19);
                result.Add(18);
                result.Add(17);
                result.Add(16);
                result.Add(15);
                result.Add(29);
            }

            return result;
        }

        public List<int> get_supplierstype_byservicetype(int service_type)
        {
            List<int> result = new List<int>();

            if(service_type == 21 || service_type == 26 || service_type == 18 //, 22 Temporary Housing, 23 Rental Furniture,27 lease renewal
                || service_type == 22 || service_type == 27
                || service_type == 29 || service_type == 28) // 29 home purchase , 28 home sale
            {
                result.Add(5);
                result.Add(9);
            }

            if (service_type == 24 || service_type == 25) // 24 Trasnportation, 25 airport 
            {
                result.Add(10);
                result.Add(23);
            }

            if(service_type == 23)
            {
                result.Add(12);
            }

            if (service_type == 20 || service_type == 17)
            {
                result.Add(34);
            }

            return result;
        }


        public ActionResult GetAdministrativeContactsServiceBySupplierPartner(int workOrderService, int supplierPartner)
        {
            var standalone = _context.StandaloneServiceWorkOrders.Where(x => x.WorkOrderServiceId == workOrderService).Select(s => new
            {
                country = s.DeliveringIn == 1
                    ? s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId
                    : s.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry.Value
            }).ToList();
            var bundled = _context.BundledServices.Where(x => x.WorkServicesId == workOrderService).Select(s => new
            {
                country = s.DeliveringIn == 1
                    ? s.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HomeCountryId
                    : s.BundledServiceOrder.WorkOrder.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountry.Value
            }).ToList();
            var countries = standalone.Union(bundled).ToList();
            var country = countries.FirstOrDefault().country;
            var administrativeContact = _context.AdministrativeContactsServices
                .Where(x => x.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.Id == supplierPartner)
                .Select(s => new
                {
                    s.Id,
                    s.Photo,
                    s.PhoneNumber,
                    email = "mail@mail.test",
                    vip = true,
                    supplierSince = s.CreatedDate
                }).ToList();
            return new ObjectResult(administrativeContact);
        }
       
        
        public ActionResult GetConsultantContactsService(int? supplierPartner, int? supplierType)
        {
            var administrativeContact = supplierPartner.HasValue ? _context.ConsultantContactsServices
                .Where(x => x.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.Id == supplierPartner && x.SupplierType == supplierType)
                .Select(s => new
                {
                    s.Id,
                    s.ContactName,
                    s.Photo,
                    s.PhoneNumber,
                    email = s.Email,
                    vip = s.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.LuxuryVip,
                    supplierSince = s.CreatedDate,
                    paymentMethods = _context.CatPaymetMethods.Where(x =>
                        x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().CreditCard.Value == true ? 1 : 0)
                        || x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().WireTransferServices.Any() == true ? 2 : 0)
                        || x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().Checks.Value == true ? 3 : 0)
                        || x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().Cash.Value == true ? 4 : 0)
                        ).Select(_s => new
                        {
                            _s.Id,
                            _s.PaymentMethods
                        }).ToList()
                }).ToList()
                : _context.ConsultantContactsServices
                .Where(x => x.SupplierType == supplierType)
                .Select(s => new
                {
                    s.Id,
                    s.ContactName,
                    s.Photo,
                    s.PhoneNumber,
                    email = s.Email,
                    vip = s.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.LuxuryVip,
                    supplierSince = s.CreatedDate,
                    paymentMethods = _context.CatPaymetMethods.Where(x =>
                        x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().CreditCard.Value == true ? 1 : 0)
                        || x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().WireTransferServices.Any() == true ? 2 : 0)
                        || x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().Checks.Value == true ? 3 : 0)
                        || x.Id == (s.AreasCoverageNavigation.PaymentInformationServices.FirstOrDefault().Cash.Value == true ? 4 : 0)
                        ).Select(_s => new
                        {
                            _s.Id,
                            _s.PaymentMethods
                        }).ToList()
                }).ToList();
            return new ObjectResult(administrativeContact);
        }

        public ActionResult GetAdmintContactsServiceProv(int supplierPartner, int workOrderService)
        {
            int country = 0;
            int service_type = 0;
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == workOrderService)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId
                }).ToList();

            if (standalone.Count > 0)
            {
                country = standalone.FirstOrDefault().country.Value;
                service_type = standalone.FirstOrDefault().type.Value;
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == workOrderService)
                .Select(s => new
                {
                    country = s.DeliveringIn
                    ,
                    type = s.ServiceId
                }).ToList();

                if (bundled.Count > 0)
                {
                    country = bundled.FirstOrDefault().country.Value;
                    service_type = bundled.FirstOrDefault().type.Value;
                }
            }

            var administrativeContact = _context.AdministrativeContactsServices
                .Where(x => x.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.Id == supplierPartner
                            && x.AreasCoverageNavigation.Country.Value == country)
                .Select(s => new
                {
                    s.Id,
                    s.ContactName,
                    s.Photo,
                    s.PhoneNumber,
                    email = s.Email
                }).ToList();

            return new ObjectResult(administrativeContact);
        }

        public ActionResult GetSupplierPartnerServiceInvoice(int sr) {

            var supplierPartner = _context.Payments
                .Where(s => s.ServiceRecord == sr)
                .Select(x => new
            {
                WorkOrder = x.SupplierPartner == null ? x.Id : x.SupplierPartner,
                supplier_partner = x.SupplierName == null ? x.SupplierPartnerNavigation.ComercialName : x.SupplierName
            }).Distinct().ToList();

            //var supplier = _context.PaymentConcepts
            //    .Include(i => i.SupplierPartnerNavigation)
            //    .Where(s => s.ServiceRecord == sr && s.SupplierPartner != null)
            //    .Select(x => new
            //{
            //    WorkOrder = x.SupplierPartner.Value,
            //    supplier_partner = x.SupplierPartnerNavigation.ComercialName
            //}).Union(supplierPartner).ToList();

            //supplierPartner.Union(supplier);
            return new ObjectResult(supplierPartner);
        }

        public ActionResult GetSupplierPartnersBySR(int sr)
        {
            var supplierPartner = _context.ProfileUsers
                .Where(x => (x.ImmigrationSupplierPartners.Select(s => s.ServiceRecordId).Contains(sr) || x.RelocationSupplierPartners.Select(s => s.ServiceRecordId).Contains(sr)))
                .Select(s => new
                {
                    s.Id,
                    name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                    location = s.CountryNavigation.Name,
                    phone = s.PhoneNumber,
                    photo = s.Photo
                }).ToList();
            return new ObjectResult(supplierPartner);
        }

        public bool DeleteAdministrativeContact(int id)
        {
            bool isSuccess =false;
            var find = _context.AdministrativeContactsServices.Find(id);
            if (find != null)
            {
                _context.AdministrativeContactsServices.Remove(find);
                _context.SaveChanges();
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool DeleteConsultantContact(int id)
        {
            bool isSuccess =false;
            var find = _context.ConsultantContactsServices.Find(id);
            if (find != null)
            {
                _context.ConsultantContactsServices.Remove(find);
                _context.SaveChanges();
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        public bool DeletePaymentInformation(int id)
        {
            bool isSuccess = false;
            if (id == 0)
            {
                isSuccess = false;
            }
            else
            {
                var payment = _context.PaymentInformationServices.FirstOrDefault(f => f.Id == id);
                if (payment != null)
                {
                    _context.PaymentInformationServices.Remove(payment);
                    _context.SaveChanges();
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            
            return isSuccess;
        }
        
        public bool DeleteWireTransfer(int id)
        {
            bool isSuccess = false;
            if (id == 0)
            {
                isSuccess = false;
            }
            else
            {
                var payment = _context.WireTransferServices.FirstOrDefault(f => f.Id == id);
                if (payment != null)
                {
                    _context.WireTransferServices.Remove(payment);
                    _context.SaveChanges();
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            
            return isSuccess;
        }
        
    }
}
