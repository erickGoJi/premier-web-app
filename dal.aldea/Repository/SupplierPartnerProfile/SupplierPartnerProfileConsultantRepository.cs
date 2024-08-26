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
using System.Threading.Tasks;

namespace dal.premier.Repository.SupplierPartnerProfile
{
    public class SupplierPartnerProfileConsultantRepository: GenericRepository<SupplierPartnerProfileConsultant>, ISupplierPartnerProfileConsultantRepository
    {
        public SupplierPartnerProfileConsultantRepository(Db_PremierContext context) : base(context) { }

        public SupplierPartnerProfileConsultant GetCustom(int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.SupplierPartnerProfileConsultant>()
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.TypeNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.CountryNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.DocumentAreasCoverageConsultants)
                        .ThenInclude(i => i.CityNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.DocumentAreasCoverageConsultants)
                        .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.DocumentAreasCoverageConsultants)
                        .ThenInclude(i => i.PrivacyNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.PaymentInformationConsultants)
                        .ThenInclude(i => i.WireTransferConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.PaymentInformationConsultants)
                        .ThenInclude(i => i.CreditCardPaymentInformationConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.CityAreasCoverageConsultants)
                        .ThenInclude(i => i.CityNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.AdministrativeContactsConsultants)
                        .ThenInclude(i => i.DocumentAdministrativeContactsConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.AdministrativeContactsConsultants)
                        .ThenInclude(i => i.ContactTypeNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.AdministrativeContactsConsultants)
                        .ThenInclude(i => i.DocumentAdministrativeContactsConsultants)
                            .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.AdministrativeContactsConsultants)
                        .ThenInclude(i => i.DocumentAdministrativeContactsConsultants)
                            .ThenInclude(i => i.PrivacyNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.LanguagesConsultantContactsConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.DocumentConsultantContactsConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.VehicleConsultants)
                            .ThenInclude(i => i.PhotosVehicleConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.VehicleConsultants)
                            .ThenInclude(i => i.VehicleTypeNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.VehicleConsultants)
                            .ThenInclude(i => i.DocumentVehicleConsultants)
                                .ThenInclude(i => i.CityNavigation)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.PersonalInformation)
                            .ThenInclude(i => i.CompesationBenefits)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.PersonalInformation)
                            .ThenInclude(i => i.EmergencyContacts)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.ProfileUsers)
                        .ThenInclude(i => i.PersonalInformation)
                            .ThenInclude(i => i.PaymentInformationProfiles)
                .Include(i => i.StatusNavigation)
                .SingleOrDefault(s => s.Id == key);
            return exist;
        }

        public async Task<SupplierPartnerProfileConsultant> UpdatedCustom(SupplierPartnerProfileConsultant supplierPartnerProfileConsultant, int key)
        {
            if (supplierPartnerProfileConsultant == null)
                return null;
            var exist = await _context.SupplierPartnerProfileConsultants
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.DocumentAreasCoverageConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.PaymentInformationConsultants)
                        .ThenInclude(i => i.WireTransferConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.PaymentInformationConsultants)
                        .ThenInclude(i => i.CreditCardPaymentInformationConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.CityAreasCoverageConsultants)
                .Include(i => i.AreasCoverageConsultants)
                    .ThenInclude(i => i.AdministrativeContactsConsultants)
                        .ThenInclude(i => i.DocumentAdministrativeContactsConsultants)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.LanguagesConsultantContactsConsultants)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.DocumentConsultantContactsConsultants)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.VehicleConsultants)
                //             .ThenInclude(i => i.PhotosVehicleConsultants)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.VehicleConsultants)
                //             .ThenInclude(i => i.DocumentVehicleConsultants)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.PersonalInformation)
                //             .ThenInclude(i => i.CompesationBenefits)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.PersonalInformation)
                //             .ThenInclude(i => i.EmergencyContacts)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.PersonalInformation)
                //             .ThenInclude(i => i.PaymentInformationProfiles)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.CountryServices)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.OperationLeaderCreatedByNavigations)
                // .Include(i => i.AreasCoverageConsultants)
                //     .ThenInclude(i => i.ProfileUsers)
                //         .ThenInclude(i => i.Offices)
                .SingleOrDefaultAsync(s => s.Id == key);
            
            if (exist != null)
            {
                var profiles = _context.ProfileUsers
                    .Include(i => i.Offices)
                    .Include(i => i.OperationLeaderCreatedByNavigations)
                    .Include(i => i.CountryServices)
                    .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.PaymentInformationProfiles)
                    .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.EmergencyContacts)
                    .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.CompesationBenefits)
                    .Include(i => i.VehicleConsultants)
                    .ThenInclude(i => i.DocumentVehicleConsultants)
                    .Include(i => i.VehicleConsultants)
                    .ThenInclude(i => i.PhotosVehicleConsultants)
                    .Include(i => i.DocumentConsultantContactsConsultants)
                    .Include(i => i.LanguagesConsultantContactsConsultants)
                    .Where(x => x.AreasCoverageNavigation.SupplierPartnerProfileConsultant == exist.Id);
                foreach (var areasCoverageConsultant in exist.AreasCoverageConsultants)
                {
                    if (profiles.Any(a => a.AreasCoverage == areasCoverageConsultant.Id))
                    {
                        foreach (var profile in profiles)
                        {
                            if (profile.AreasCoverage == areasCoverageConsultant.Id)
                            {
                                areasCoverageConsultant.ProfileUsers.Add(profile);
                            }
                        }
                    }
                }
                
                _context.Entry(exist).CurrentValues.SetValues(supplierPartnerProfileConsultant);

                foreach (var i in supplierPartnerProfileConsultant.AreasCoverageConsultants)
                {
                    var areasCoverage = exist.AreasCoverageConsultants.FirstOrDefault(p => p.Id == i.Id);
                    if (areasCoverage == null)
                    {
                        exist.AreasCoverageConsultants.Add(i);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Entry(areasCoverage).CurrentValues.SetValues(i);
                        // DOCUMENTS 
                        foreach (var document in i.DocumentAreasCoverageConsultants)
                        {
                            var doc = areasCoverage.DocumentAreasCoverageConsultants.Where(p => p.Id == document.Id).FirstOrDefault();
                            if (doc == null)
                            {
                                areasCoverage.DocumentAreasCoverageConsultants.Add(document);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                _context.Entry(doc).CurrentValues.SetValues(document);
                            }
                        }
                        // CITIES
                        foreach (var city in i.CityAreasCoverageConsultants)
                        {
                            var cities = areasCoverage.CityAreasCoverageConsultants.Where(p => p.City == city.City).FirstOrDefault();
                            if (cities == null)
                            {
                                areasCoverage.CityAreasCoverageConsultants.Add(city);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                _context.Entry(cities).CurrentValues.SetValues(city);
                            }
                        }
                        // PAYMENT
                        foreach (var payment in i.PaymentInformationConsultants)
                        {
                            var _payment = areasCoverage.PaymentInformationConsultants.Where(p => p.Id == payment.Id).FirstOrDefault();
                            if (_payment == null)
                            {
                                areasCoverage.PaymentInformationConsultants.Add(payment);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                _context.Entry(_payment).CurrentValues.SetValues(payment);
                                foreach (var wire in payment.WireTransferConsultants)
                                {
                                    var _wire = _payment.WireTransferConsultants.Where(p => p.Id == wire.Id).FirstOrDefault();
                                    if (_wire == null)
                                    {
                                        _payment.WireTransferConsultants.Add(wire);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _context.Entry(_wire).CurrentValues.SetValues(wire);
                                    }
                                }
                                foreach (var cc in payment.CreditCardPaymentInformationConsultants)
                                {
                                    var _cc = _payment.CreditCardPaymentInformationConsultants.Where(p => p.CreditCard == cc.CreditCard).FirstOrDefault();
                                    if (_cc == null)
                                    {
                                        _payment.CreditCardPaymentInformationConsultants.Add(cc);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _context.Entry(_cc).CurrentValues.SetValues(cc);
                                    }
                                }
                            }
                        }
                        // ADMINISTRATIVE
                        foreach (var administrative in i.AdministrativeContactsConsultants)
                        {
                            var _administrative = areasCoverage.AdministrativeContactsConsultants.Where(p => p.Id == administrative.Id).FirstOrDefault();
                            if (_administrative == null)
                            {
                                areasCoverage.AdministrativeContactsConsultants.Add(administrative);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                _context.Entry(_administrative).CurrentValues.SetValues(administrative);
                                foreach (var document in administrative.DocumentAdministrativeContactsConsultants)
                                {
                                    var _document = _administrative.DocumentAdministrativeContactsConsultants.Where(p => p.Id == document.Id).FirstOrDefault();
                                    if (_document == null)
                                    {
                                        _administrative.DocumentAdministrativeContactsConsultants.Add(document);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _context.Entry(_document).CurrentValues.SetValues(document);
                                    }
                                }
                            }
                        }
                        // CONSULTANT
                        foreach (var consultant in i.ProfileUsers)
                        {
                            var _consultant = areasCoverage.ProfileUsers.Where(p => p.Id == consultant.Id).FirstOrDefault();
                            if (_consultant == null)
                            {
                                User user = new User();
                                user.Email = consultant.Email;
                                user.UserTypeId = consultant.Title == 1 ? 4 : consultant.Title == 2 ? 3 : consultant.Title == 3 ? 1 : 1;
                                user.RoleId = consultant.Title == 1 ? 3 : consultant.Title == 2 ? 2 : consultant.Title == 3 ? 1 : 1;
                                user.ServiceLineId = 1;
                                user.Password = HashPassword("$" + Guid.NewGuid().ToString().Substring(0, 7));
                                user.CreatedDate = DateTime.Now;
                                user.UpdatedDate = null;
                                consultant.User = user;
                                consultant.UserId = 0;
                                areasCoverage.ProfileUsers.Add(consultant);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                _context.Entry(_consultant).CurrentValues.SetValues(consultant);
                                // Offices
                                _consultant.Offices.Clear();
                                await _context.SaveChangesAsync();
                                foreach (var o in consultant.Offices)
                                {
                                    _consultant.Offices.Add(o);
                                }
                                await _context.SaveChangesAsync();
                                // Countries Services
                                _consultant.CountryServices.Clear();
                                await _context.SaveChangesAsync();
                                foreach (var o in consultant.CountryServices)
                                {
                                    _consultant.CountryServices.Add(o);
                                }
                                await _context.SaveChangesAsync();
                                // OPERATION LEADER
                                _consultant.OperationLeaderCreatedByNavigations.Clear();
                                await _context.SaveChangesAsync();
                                foreach (var o in consultant.OperationLeaderCreatedByNavigations)
                                {
                                    _consultant.OperationLeaderCreatedByNavigations.Add(o);
                                }
                                await _context.SaveChangesAsync();
                                // DOCUMENT
                                foreach (var document in consultant.DocumentConsultantContactsConsultants)
                                {
                                    var _document = _consultant.DocumentConsultantContactsConsultants.Where(p => p.Id == document.Id).FirstOrDefault();
                                    if (_document == null)
                                    {
                                        _consultant.DocumentConsultantContactsConsultants.Add(document);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _context.Entry(_document).CurrentValues.SetValues(document);
                                    }
                                }
                                // LANGUAGE
                                foreach (var languages in consultant.LanguagesConsultantContactsConsultants)
                                {
                                    var _languages = _consultant.LanguagesConsultantContactsConsultants.Where(p => p.Language == languages.Language).FirstOrDefault();
                                    if (_languages == null)
                                    {
                                        _consultant.LanguagesConsultantContactsConsultants.Add(languages);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _context.Entry(_languages).CurrentValues.SetValues(languages);
                                    }
                                }
                                // VEHICLES
                                foreach (var vehicle in consultant.VehicleConsultants)
                                {
                                    var _vehicle = _consultant.VehicleConsultants.Where(p => p.Id == vehicle.Id).FirstOrDefault();
                                    if (_vehicle == null)
                                    {
                                        _consultant.VehicleConsultants.Add(vehicle);
                                        await _context.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        _context.Entry(_vehicle).CurrentValues.SetValues(vehicle);
                                        // DOCUMENT
                                        foreach (var document in vehicle.DocumentVehicleConsultants)
                                        {
                                            var _document = _vehicle.DocumentVehicleConsultants.Where(p => p.Id == document.Id).FirstOrDefault();
                                            if (_document == null)
                                            {
                                                _vehicle.DocumentVehicleConsultants.Add(document);
                                                await _context.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                _context.Entry(_document).CurrentValues.SetValues(document);
                                            }
                                        }
                                        // PHOTO
                                        foreach (var photo in vehicle.PhotosVehicleConsultants)
                                        {
                                            var _photo = _vehicle.PhotosVehicleConsultants.Where(p => p.Id == photo.Id).FirstOrDefault();
                                            if (_photo == null)
                                            {
                                                _vehicle.PhotosVehicleConsultants.Add(photo);
                                                await _context.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                _context.Entry(photo).CurrentValues.SetValues(photo);
                                            }
                                        }
                                    }
                                }
                                // PROFILE
                                if(_consultant.PersonalInformation == null)
                                {
                                    _consultant.PersonalInformation = consultant.PersonalInformation;
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    _context.Entry(_consultant.PersonalInformation).CurrentValues.SetValues(consultant.PersonalInformation);
                                    foreach (var emergency in consultant.PersonalInformation.EmergencyContacts)
                                    {
                                        var _emergency = _consultant.PersonalInformation.EmergencyContacts.Where(p => p.Id == emergency.Id).FirstOrDefault();
                                        if (_emergency == null)
                                        {
                                            _consultant.PersonalInformation.EmergencyContacts.Add(emergency);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            _context.Entry(_emergency).CurrentValues.SetValues(emergency);
                                        }
                                    }
                                    // Compesation
                                    foreach (var compesation in consultant.PersonalInformation.CompesationBenefits)
                                    {
                                        var _compesation = _consultant.PersonalInformation.CompesationBenefits.Where(p => p.Id == compesation.Id).FirstOrDefault();
                                        if (_compesation == null)
                                        {
                                            _consultant.PersonalInformation.CompesationBenefits.Add(compesation);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            _context.Entry(_compesation).CurrentValues.SetValues(compesation);
                                        }
                                    }
                                    // Payement
                                    foreach (var payment in consultant.PersonalInformation.PaymentInformationProfiles)
                                    {
                                        var _payment = _consultant.PersonalInformation.PaymentInformationProfiles.Where(p => p.Id == payment.Id).FirstOrDefault();
                                        if (_payment == null)
                                        {
                                            _consultant.PersonalInformation.PaymentInformationProfiles.Add(payment);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            _context.Entry(_payment).CurrentValues.SetValues(payment);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return exist;
        }

        public string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public ActionResult GetSupplierPartnerConsultant(int? country, int? city, int? serviceLine)
        {
            var supplierPartner = _context.SupplierPartnerProfileConsultants
                .Where(x => x.AreasCoverageConsultants.Select(s => s.Country).Contains(country) 
                    && (x.AreasCoverageConsultants.Select(s => s.PrimaryCity).Contains(city) 
                    || x.AreasCoverageConsultants.Select(s => s.CityAreasCoverageConsultants.Select(_s => _s.City)
                        .Where(_x => _x == city.Value)).Any())
                    
                    )
                .Select(s => new
                {
                    s.Id,
                    s.Photo,
                    s.Relocation,
                    s.Immigration,
                    s.ComercialName,
                    s.LuxuryVip,
                    s.LegalName,
                    s.SupplierSince
                }).ToList();
            if (serviceLine.HasValue)
                if(serviceLine.Value == 1)
                    supplierPartner = supplierPartner.Where(x => x.Immigration == true).ToList();
                else if(serviceLine.Value == 2)
                    supplierPartner = supplierPartner.Where(x => x.Relocation == true).ToList();
            return new ObjectResult(supplierPartner);
        }

        public ActionResult GetConsultantContactsConsultants(int? supplierPartner, int country, int city, int serviceLine)
        {
            var _country = _context.Countries.Any(x => x.Id == country)
                ? _context.Countries.FirstOrDefault(x => x.Id == country).Name
                : _context.CatCountries.FirstOrDefault(x => x.Id == country).Name;
            var _countryId = _context.CatCountries.FirstOrDefault(x => x.Name.ToLower() == _country.ToLower()).Id;
            var administrativeContact = supplierPartner.HasValue ? _context.ProfileUsers
                .Where(x => x.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.Id == supplierPartner
                    && x.Country == _countryId
                    //&& x.City == city
                    )
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    Photo = s.Photo == null || s.Photo == "" ? "Files/assets/avatar.png" : s.Photo,
                    s.PhoneNumber,
                    email = s.Email,
                    vip = s.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.LuxuryVip,
                    supplierSince = s.CreatedDate,
                    s.Immigration,
                    s.Relocation,
                    Company = s.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.ComercialName,
                    CompanyId = s.AreasCoverageNavigation.SupplierPartnerProfileConsultant,
                    SupplierType = s.SupplierType
                }).ToList()
                : _context.ProfileUsers
                .Where(x => x.Country == _countryId
                            //&& x.City == city 
                            //&& (x.SupplierType == 1 || x.SupplierType == 3 || x.SupplierType == 2 || x.SupplierType == 4)
                            && x.User.RoleId == 3
                    )
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Photo,
                    s.PhoneNumber,
                    email = s.Email,
                    vip = s.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.LuxuryVip,
                    supplierSince = s.CreatedDate,
                    s.Immigration,
                    s.Relocation,
                    Company = s.AreasCoverage.HasValue 
                        ? s.AreasCoverageNavigation.SupplierPartnerProfileConsultantNavigation.ComercialName
                        : "N/A",
                    CompanyId = s.AreasCoverage.HasValue 
                        ? s.AreasCoverageNavigation.SupplierPartnerProfileConsultant 
                        : (int?)null,
                    SupplierType = s.SupplierType.HasValue ? s.SupplierType : 0
                }).ToList();
            if (serviceLine == 1)
                administrativeContact = administrativeContact.Where(x => x.SupplierType.Value == 3).ToList();
            else
                administrativeContact = administrativeContact.Where(x => x.SupplierType.Value == 1).ToList();
            return new ObjectResult(administrativeContact);
        }

        public ActionResult GetCompany()
        {
            var companies = _context.SupplierPartnerProfileConsultants.Select(s => new
            {
                s.Id,
                s.ComercialName,
                s.CreatedDate
            }).OrderByDescending(o => o.CreatedDate).ToList();
            return new ObjectResult(companies);
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
                var payment = _context.PaymentInformationConsultants.FirstOrDefault(f => f.Id == id);
                if (payment != null)
                {
                    _context.PaymentInformationConsultants.Remove(payment);
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
                var payment = _context.WireTransferConsultants.FirstOrDefault(f => f.Id == id);
                if (payment != null)
                {
                    _context.WireTransferConsultants.Remove(payment);
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
