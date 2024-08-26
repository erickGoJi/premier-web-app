using biz.premier.Entities;
using biz.premier.Repository.ProfileUser;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using biz.premier.Models.PricingInfo;

namespace dal.premier.Repository.ProfileUser
{
    public class ProfileUserRepository : GenericRepository<biz.premier.Entities.ProfileUser>, IProfileUserRepository
    {
        private IProfileUserRepository _profileUserRepositoryImplementation;
        public ProfileUserRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.ProfileUser AddConsultant(biz.premier.Entities.ProfileUser consultantContactsConsultant)
        {
            _context.ProfileUsers.Add(consultantContactsConsultant);
            _context.SaveChanges();
            return consultantContactsConsultant;
        }

        public biz.premier.Entities.ProfileUser GetConsultant(int key)
        {
            if (key == 0)
                return null;
            var _consultant = _context.Set<biz.premier.Entities.ProfileUser>()
                .Include(i => i.SupplierTypeNavigation)
                .Include(i => i.User)
                .Include(i => i.Offices)
                    .ThenInclude(i => i.Office1Navigation)
                .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.EmergencyContacts)
                        .ThenInclude(i => i.RelationshipNavigation)
                .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.CompesationBenefits)
                .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.PaymentInformationProfiles)
                .Include(i => i.DocumentConsultantContactsConsultants)
                    .ThenInclude(i => i.DocumentTypeNavigation)
                .Include(i => i.LanguagesConsultantContactsConsultants)
                .Include(i => i.VehicleConsultants)
                    .ThenInclude(i => i.PhotosVehicleConsultants)
                .Include(i => i.VehicleConsultants)
                    .ThenInclude(i => i.DocumentVehicleConsultants)
                .Include(i => i.CountryServices)
                    .ThenInclude(i => i.CountryNavigation)
                
                .Include(i => i.OperationLeaderCreatedByNavigations)
                    .ThenInclude(i => i.CreatedByNavigation)
                .Include(i => i.OperationLeaderCreatedByNavigations)
                    .ThenInclude(i => i.ConsultantNavigation)
                        .ThenInclude(i => i.TitleNavigation)
                .Include(i => i.OperationLeaderCreatedByNavigations)
                    .ThenInclude(i => i.ConsultantNavigation)
                        .ThenInclude(i => i.CountryNavigation)
                
                .Include(i => i.OperationLeaderConsultantNavigations)
                    .ThenInclude(i => i.ConsultantNavigation)
                        .ThenInclude(i => i.CountryNavigation)
                .Include(i => i.OperationLeaderConsultantNavigations)
                    .ThenInclude(i => i.ConsultantNavigation)
                        .ThenInclude(i => i.TitleNavigation)
                .Include(i => i.OperationLeaderConsultantNavigations)
                    .ThenInclude(i => i.CreatedByNavigation)
                
                .Include(i => i.AssignedTeamAssignedByNavigations)
                    .ThenInclude(i => i.AssignedToNavigation)
                .Include(i => i.AssignedTeamAssignedToNavigations)
                    .ThenInclude(i => i.AssignedToNavigation)
                .Include(i => i.AssignedTeamAssignedToNavigations)
                    .ThenInclude(i => i.AssignedByNavigation)
                .Include(i => i.TitleNavigation)
                .SingleOrDefault(s => s.Id == key);
            return _consultant;
        }

        public biz.premier.Entities.ProfileUser UpdateCustom(biz.premier.Entities.ProfileUser consultantContactsConsultant, int key)
        {
            if (key == 0)
                return null;
            var _consultant = _context.Set<biz.premier.Entities.ProfileUser>()
                .Include(i => i.AssignedTeamAssignedByNavigations)
                .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.EmergencyContacts)
                .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.CompesationBenefits)
                .Include(i => i.PersonalInformation)
                    .ThenInclude(i => i.PaymentInformationProfiles)
                .Include(i => i.OperationLeaderCreatedByNavigations)
                .Include(i => i.OperationLeaderConsultantNavigations)
                .Include(i => i.CountryServices)
                .Include(i => i.Offices)
                .Include(i => i.DocumentConsultantContactsConsultants)
                .Include(i => i.LanguagesConsultantContactsConsultants)
                .Include(i => i.VehicleConsultants)
                    .ThenInclude(i => i.PhotosVehicleConsultants)
                .Include(i => i.VehicleConsultants)
                    .ThenInclude(i => i.DocumentVehicleConsultants)
                .SingleOrDefault(s => s.Id == key);
            if (_consultant != null)
            {
                _context.Entry(_consultant).CurrentValues.SetValues(consultantContactsConsultant);
                // DOCUMENT
                foreach (var document in consultantContactsConsultant.DocumentConsultantContactsConsultants)
                {
                    var _document = _consultant.DocumentConsultantContactsConsultants.Where(p => p.Id == document.Id).FirstOrDefault();
                    if (_document == null)
                    {
                        _consultant.DocumentConsultantContactsConsultants.Add(document);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(_document).CurrentValues.SetValues(document);
                    }
                }
                // Offices
                _consultant.Offices.Clear();
                _context.SaveChanges();
                foreach (var o in consultantContactsConsultant.Offices)
                {
                    _consultant.Offices.Add(o);
                }
                _context.SaveChanges();
                // Countries Services
                _consultant.CountryServices.Clear();
                _context.SaveChanges();
                foreach (var o in consultantContactsConsultant.CountryServices)
                {
                    _consultant.CountryServices.Add(o);
                }
                _context.SaveChanges();
                // OPERATION LEADER Created By
                _consultant.OperationLeaderCreatedByNavigations.Clear();
                _context.SaveChanges();
                foreach (var o in consultantContactsConsultant.OperationLeaderCreatedByNavigations)
                {
                    _consultant.OperationLeaderCreatedByNavigations.Add(o);
                }
                _context.SaveChanges();
                // OPERATION LEADER Consultant
                _consultant.OperationLeaderConsultantNavigations.Clear();
                _context.SaveChanges();
                foreach (var o in consultantContactsConsultant.OperationLeaderConsultantNavigations)
                {
                    _consultant.OperationLeaderConsultantNavigations.Add(o);
                }
                _context.SaveChanges();
                // Assigned Team
                _consultant.AssignedTeamAssignedByNavigations.Clear();
                _context.SaveChanges();
                foreach (var o in consultantContactsConsultant.AssignedTeamAssignedByNavigations)
                {
                    _consultant.AssignedTeamAssignedByNavigations.Add(o);
                }
                _context.SaveChanges();
                // LANGUAGE
                _consultant.LanguagesConsultantContactsConsultants.Clear();
                _context.SaveChanges();
                foreach (var o in consultantContactsConsultant.LanguagesConsultantContactsConsultants)
                {
                    _consultant.LanguagesConsultantContactsConsultants.Add(o);
                }
                _context.SaveChanges();
                // VEHICLES
                foreach (var vehicle in consultantContactsConsultant.VehicleConsultants)
                {
                    var _vehicle = _consultant.VehicleConsultants.Where(p => p.Id == vehicle.Id).FirstOrDefault();
                    if (_vehicle == null)
                    {
                        _consultant.VehicleConsultants.Add(vehicle);
                        _context.SaveChanges();
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
                                _context.SaveChanges();
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
                                _context.SaveChanges();
                            }
                            else
                            {
                                _context.Entry(photo).CurrentValues.SetValues(photo);
                            }
                        }
                    }
                }
                // PROFILE
                if (_consultant.PersonalInformation == null)
                {
                    _consultant.PersonalInformation = consultantContactsConsultant.PersonalInformation;
                    _context.SaveChanges();
                }
                else
                {
                    _context.Entry(_consultant.PersonalInformation).CurrentValues.SetValues(consultantContactsConsultant.PersonalInformation);
                    // Emergency
                    foreach (var emergency in consultantContactsConsultant.PersonalInformation.EmergencyContacts)
                    {
                        var _emergency = _consultant.PersonalInformation.EmergencyContacts.Where(p => p.Id == emergency.Id).FirstOrDefault();
                        if (_emergency == null)
                        {
                            _consultant.PersonalInformation.EmergencyContacts.Add(emergency);
                            _context.SaveChanges();
                        }
                        else
                        {
                            _context.Entry(_emergency).CurrentValues.SetValues(emergency);
                        }
                    }
                    // Compesation
                    foreach (var compesation in consultantContactsConsultant.PersonalInformation.CompesationBenefits)
                    {
                        var _compesation = _consultant.PersonalInformation.CompesationBenefits.Where(p => p.Id == compesation.Id).FirstOrDefault();
                        if (_compesation == null)
                        {
                            _consultant.PersonalInformation.CompesationBenefits.Add(compesation);
                            _context.SaveChanges();
                        }
                        else
                        {
                            _context.Entry(_compesation).CurrentValues.SetValues(compesation);
                        }
                    }
                    // Payement
                    foreach (var payment in consultantContactsConsultant.PersonalInformation.PaymentInformationProfiles)
                    {
                        var _payment = _consultant.PersonalInformation.PaymentInformationProfiles.Where(p => p.Id == payment.Id).FirstOrDefault();
                        if (_payment == null)
                        {
                            _consultant.PersonalInformation.PaymentInformationProfiles.Add(payment);
                            _context.SaveChanges();
                        }
                        else
                        {
                            _context.Entry(_payment).CurrentValues.SetValues(payment);
                        }
                    }
                }
            _context.SaveChanges();
            }
            return consultantContactsConsultant;
        }

        public ActionResult GetDirectory(int? title, int? country, int? city, int? company, int? office)
        {
            var query = _context.Offices
                .Where(x => x.ConsultantNavigation.Title != 1)
                .Select(c => new
                {
                    id = c.ConsultantNavigation.Id,
                    servce_line = c.ConsultantNavigation.User.ProfileUsers.FirstOrDefault().Immigration.Value 
                    ? 1 
                    : c.ConsultantNavigation.User.ProfileUsers.FirstOrDefault().Relocation.Value ? 2
                    : 0,
                    user_id = c.ConsultantNavigation.User.Id,
                    user = c.ConsultantNavigation.User.ProfileUsers.FirstOrDefault().Name,
                    role = c.ConsultantNavigation.User.Role.Role,
                    title_id = c.ConsultantNavigation.Title,
                    title = c.ConsultantNavigation.TitleNavigation.Title,
                    company = "",
                    company_id = 1,
                    c.Office1Navigation.Office,
                    office_id = c.Office1Navigation.Id,
                    country_id = c.ConsultantNavigation.CountryNavigation.Id,
                    country = c.ConsultantNavigation.CountryNavigation.Name,
                    city_id = c.ConsultantNavigation.CityNavigation.Id,
                    city = c.ConsultantNavigation.CityNavigation.City,
                    c.ConsultantNavigation.PhoneNumber,
                    c.ConsultantNavigation.Email
                }).ToList();

            if (title != null) {
                query = query.Where(x => x.title_id == title).ToList();
            }
            if (country != null)
            {
                query = query.Where(x => x.country_id == country).ToList();
            }
            if (city != null)
            {
                query = query.Where(x => x.city_id == city).ToList();
            }
            if (company != null)
            {
                query = query.Where(x => x.company_id == company).ToList();
            }
            if (office != null)
            {
                query = query.Where(x => x.office_id == office).ToList();
            }

            return new ObjectResult(query);
        }

        public ActionResult DashboardInicio(int userId)
        {
            List<famous> famous = new List<famous>();
            famous.Add(new famous { 
                Image = "/Files/DashboardInicio/login-bg.png",
                Text = "Lorem Ipsum Dolor Sit amet, consectetur lorem"
            });
            famous.Add(new famous
            {
                Image = "/Files/DashboardInicio/login-bg.png",
                Text = "Lorem Ipsum Dolor Sit amet, consectetur lorem"
            });

            List<InCompany> company = new List<InCompany>();
            company.Add(new InCompany
            {
                avatar = "/Files/Pets/2d0a3d38-a34e-4b80-b018-683d4e24bce9.jpg",
                name = "Lorem Ipsum",
                title = "Coordinator"
            });
            company.Add(new InCompany
            {
                avatar = "/Files/Pets/2d0a3d38-a34e-4b80-b018-683d4e24bce9.jpg",
                name = "Lorem Ipsum",
                title = "Coordinator"
            });
            company.Add(new InCompany
            {
                avatar = "/Files/Pets/2d0a3d38-a34e-4b80-b018-683d4e24bce9.jpg",
                name = "Lorem Ipsum",
                title = "Coordinator"
            });
            company.Add(new InCompany
            {
                avatar = "/Files/Pets/2d0a3d38-a34e-4b80-b018-683d4e24bce9.jpg",
                name = "Lorem Ipsum",
                title = "Coordinator"
            });
            company.Add(new InCompany
            {
                avatar = "/Files/Pets/2d0a3d38-a34e-4b80-b018-683d4e24bce9.jpg",
                name = "Lorem Ipsum",
                title = "Coordinator"
            });

            List<UpcomingEvents> upcoming = new List<UpcomingEvents>();
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });
            upcoming.Add(new UpcomingEvents
            {
                date_event = DateTime.Now,
                upcomming_event = "Lorem Ipsum Dolor Sit Amet, Consectetur",
                country = "México",
                city = "CDMX",
                description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus vel rutrum libero, a suscipit lectus. "
            });

            var query = _context.Users
                .Where(x => x.Id == userId)
                .Select(c => new
                {
                    c.Id,
                    famous_phrases = famous,
                    incompany = company,
                    experience_team = _context.ClientPartnerProfileExperienceTeams
                    .Where(x => x.UserId == userId)
                    .Select(t => new
                    {
                        t.User.Avatar,
                        name = t.User.Name + " " + t.User.LastName,
                        title = t.User.Role.Role
                    }).ToList(),
                    upcomig_event = upcoming
                }).ToList();

            return new ObjectResult(query);
        }

        public string GetSupplierPartner(int key)
        {
            var supplierName = _context.AreasCoverageConsultants
                .Include(i => i.SupplierPartnerProfileConsultantNavigation)
                .SingleOrDefault(x => x.Id == key).SupplierPartnerProfileConsultantNavigation.ComercialName;
            return supplierName;
        }

        public PricingInfo GetPricingInfo(int key)
        {
            var pricingInfo = _context.AreasCoverageConsultants
                .Include(i => i.SupplierPartnerProfileConsultantNavigation)
                .Where(x => x.Id == key).Select(s => new PricingInfo
            {
                Currency = s.SupplierPartnerProfileConsultantNavigation.CurrencyNavigation.Currency,
                CreditTerms = s.SupplierPartnerProfileConsultantNavigation.CreditTermsNavigation.CreditTerm,
                TaxesPercentage = s.SupplierPartnerProfileConsultantNavigation.TaxesPercentageNavigation.Taxe,
                AmountPerHour = s.SupplierPartnerProfileConsultantNavigation.AmountPerHour.Value,
            }).FirstOrDefault();
            return pricingInfo;
        }

        public bool isVip(int key)
        {
            var pricingInfo = _context.AreasCoverageConsultants
                .Include(i => i.SupplierPartnerProfileConsultantNavigation)
                .Where(x => x.Id == key).Select(s => new
                {
                    isVip = s.SupplierPartnerProfileConsultantNavigation.LuxuryVip
                }).FirstOrDefault();
            return pricingInfo.isVip.Value;
        }

        public ActionResult GetClients(int key)
        {
            var clients = _context.ClientPartnerProfileExperienceTeams.Where(x => x.UserId == key).Select(s => new
            {
                s.Id,
                s.IdClientPartnerProfileNavigation.Name,
                country = s.IdClientPartnerProfileNavigation.OfficeInformations.FirstOrDefault().IdCountryNavigation.Name,
                s.IdClientPartnerProfileNavigation.Photo
            }).ToList();
            return new ObjectResult(clients);
        }

        public ActionResult GetCountryLeader(int key)
        {
            var countriesLeader = _context.OperationLeaders
                .Where(x => x.ConsultantNavigation.Country == key)
                .Select(s => new
                {
                    name = $"{s.ConsultantNavigation.Name} {s.ConsultantNavigation.LastName} {s.ConsultantNavigation.MotherLastName}",
                    s.ConsultantNavigation.TitleNavigation.Title,
                    s.ConsultantNavigation.Photo,
                    country = s.ConsultantNavigation.CountryNavigation.Name
                }).ToList();
            return new ObjectResult(countriesLeader);
        }

        public ActionResult GetAssignedTeam(int? type)
        {
            var profiles = _context.ProfileUsers.Select(s => new
            {
                s.Id,
                s.TitleNavigation.Title,
                name = $"{s.Name} {s.LastName} {s.MotherLastName}",
                s.Photo,
                titleId = s.Title
            }).ToList();
            if (type.HasValue)
            {
                profiles = profiles.Where(x => x.titleId == type.Value).ToList();
            }
            return new ObjectResult(profiles);
        }

        public ActionResult GetProfilesByTitle(int? key, int? client, int? serviceLine)
        {
            int[] role = new[] { 1, 2 };

            var profiles = _context.ProfileUsers
                .Where(x => role.Contains(x.User.RoleId))
                .Select(s => new
                {
                    s.Id,
                    s.Photo,
                    s.User.Email,
                    s.PhoneNumber,
                    s.User.MobilePhone,
                    coordinator = $"{s.Name} {s.LastName} {s.MotherLastName}",
                    title = s.TitleNavigation.Title,
                    titleId = s.Title,
                    role = s.User.Role.Role,
                    roleId = s.User.Role.Id,
                    office = s.ResponsablePremierOfficeNavigation.Office,
                    experience = s.User.ClientPartnerProfileExperienceTeams,
                    user = s.UserId,
                    s.Immigration,
                    s.Relocation
                }).ToList();
            if (client.Value != 0)
                profiles = profiles.Where(x => x.experience.Select(s => s.IdClientPartnerProfile).Contains(client.Value)).ToList();
            if (serviceLine.HasValue)
            {
                if(serviceLine == 1)
                    profiles = profiles.Where(x => x.Immigration == true).ToList();
                else
                    profiles = profiles.Where(x => x.Relocation == true).ToList();
            }

            if (key.HasValue)
            {
                profiles = profiles.Where(x => x.roleId == key.Value).ToList();
            }
            return new ObjectResult(profiles.OrderBy(x => x.coordinator));
        }

        public biz.premier.Entities.ProfileUser DeleteCustom(biz.premier.Entities.ProfileUser user)
        {
            _context.ProfileUsers.Remove(user);
            _context.SaveChanges();
            return user;
        }

        public class famous {
            public string Image { get; set; }
            public string Text { get; set; }
        }

        public class InCompany
        {
            public string avatar { get; set; }
            public string name { get; set; }
            public string title { get; set; }
        }

        public class UpcomingEvents {
            public DateTime date_event { get; set; }
            public string upcomming_event { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string description { get; set; }
        }
    }
}
