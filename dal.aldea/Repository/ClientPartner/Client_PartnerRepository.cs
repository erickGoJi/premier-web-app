using biz.premier.Entities;
using biz.premier.Repository.ClientPartner;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.premier.Models.ClientPartnerProfile;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.ClientPartner
{
    public class Client_PartnerRepository : GenericRepository<biz.premier.Entities.ClientPartnerProfile>, IClient_PartnerRepository
    {
        public Client_PartnerRepository(Db_PremierContext context) : base(context)
        {
        }

        public ActionResult GetAuthorizedby(int client, int country, int city)
        {
            var authorizedBy = _context.OfficeContacts
                .Where(x => x.IdOfficeInformationNavigation.IdClientPartnerProfile == client && x.IdOfficeInformationNavigation.IdCountry == country && x.IdCity == city)
                .Select(s => new
                {
                    s.Id,
                    name = $"{s.ContactName} / {s.IdOfficeInformationNavigation.CommercialName}"
                }).ToList();
            return new ObjectResult(authorizedBy);
        }

        public ActionResult GetClientList(int partner)
        {
            var _client = partner == 0 ? _context.ClientPartnerProfiles
                .Where(x => x.IdCompanyType == 2)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Photo
                }).ToList()
                : _context.ClientPartnerProfileClients
                .Where(x => x.IdClientFrom == partner)
                .Select(s => new
                {
                    s.IdClientToNavigation.Id,
                    s.IdClientToNavigation.Name,
                    s.IdClientToNavigation.Photo
                }).ToList();
            //var test = _context.ClientPartnerProfileClients
            //    .Where(x => x.IdClientFrom == partner)
            //    .Select(s => new
            //    {
            //        s.IdClientToNavigation.Id,
            //        s.IdClientToNavigation.Name,
            //        s.IdClientToNavigation.Photo
            //    }).ToList();
            return new ObjectResult(_client.OrderBy(x => x.Name).Distinct());
        }

        public ActionResult GetClientPartner(DateTime? date_range_in, DateTime? date_range_fi, int? type, int? company_type, 
            int? country, int? city, int? status, int lead_or_client, int? state)
        {
            var query = _context.ClientPartnerProfiles
                .Select(c => new
                {
                    c.Id,
                    company = c.Name,
                    company_type = c.IdCompanyTypeNavigation.CompanyType1,
                    company_type_id = c.IdCompanyTypeNavigation.Id,
                    lead_type = c.IdTypePartnerClientProfileNavigation.Type,
                    lead_type_id = c.IdTypePartnerClientProfileNavigation.Id,
                    assigned = c.AssignedToNavigation.Name == null || c.AssignedToNavigation.Name == "" ? "Not assigned" : c.AssignedToNavigation.Name,
                    office = _context.OfficeInformations
                    .Where(x => x.IdClientPartnerProfile == c.Id)
                    .Select(g => new { 
                        g.CommercialName,
                        g.LegalName,
                        country = g.IdCountryNavigation.Name,
                        g.IdCountry,
                        city = g.IdCityNavigation.Name,
                        g.IdCity,
                        contact = c.OfficeInformations.Select(h => new { h.CommercialName, h.CurrentAddress }).ToList(),
                        contact_main = c.OfficeInformations.Select(h => new { h.CommercialName, h.CurrentAddress,
                            OfficeContacts = h.OfficeContacts.Where(x => x.IdContactType == 1).ToList() 
                        }).ToList()
                    }).ToList(),
                    //country = c..IdCountryNavigation.Name,
                    //country_id = c.IdCountryNavigation.Id,
                    //city = c.IdCityNavigation.Name,
                    //city_id = c.IdCityNavigation.Id,
                    dateFirst = c.CreatedDate == null ? DateTime.Now : c.CreatedDate,
                    c.IdStatusNavigation.Status,
                    status_id = c.IdStatusNavigation.Id,
                    c.IdResponsiblePremierOfficeNavigation.ResponsiblePremierOffice1,
                    //c.ContactName,
                    //c.PhoneNumber,
                    //c.Email,
                    c.IdLifeCircle,
                    //state = c.IdStateNavigation.Name,
                    //stateId = c.IdOfficeInformationNavigation.IdState
                }).ToList();

            if (date_range_in != null && date_range_fi != null)
            {
                query = query.Where(x => x.dateFirst.Value.Date >= date_range_in.Value.Date && x.dateFirst.Value.Date <= date_range_fi.Value.Date).ToList();
                //query.Distinct();
            }
            if (type != null)
            {
                query = query.Where(x => x.lead_type_id == type).ToList();
            }
            if (company_type != null)
            {
                query = query.Where(x => x.company_type_id == company_type).ToList();
            }
            //if (country != null)
            //{
            //    query = query.Where(x => x.country_id == country).ToList();
            //}
            //if (city != null)
            //{
            //    query = query.Where(x => x.city_id == city).ToList();
            //}
            //if (state != null)
            //{
            //    query = query.Where(x => x.stateId == state).ToList();
            //}
            if (status != null)
            {
                query = query.Where(x => x.status_id == status).ToList();
            }
            if (lead_or_client == 4)
            {
                query = query.Where(x => x.IdLifeCircle == lead_or_client).ToList();
            }
            else
            {
                query = query.Where(x => x.IdLifeCircle != 4).ToList();
            }

            return new ObjectResult(query);
        }

        public ActionResult GetClientPartnerById(int Id)
        {
            var query = _context.ClientPartnerProfiles
                .Where(x => x.Id == Id)
                .Select(c => new
                {
                    c.Id,
                    c.Photo,
                    c.IdTypePartnerClientProfile,
                    c.BelongsToPartner,
                    c.Name,
                    c.PartnerClientSince,
                    c.IdResponsiblePremierOffice,
                    c.AssignedTo,
                    c.IdReferralFee,
                    c.IdPaymentRecurrence,
                    c.IdPricingType,
                    c.IdCompanyType,
                    c.About,
                    c.ActivityLogs,
                    c.TermsDeals,
                    generalContractPricingInfos = c.GeneralContractPricingInfos.Select(x => new
                    {
                        x.Id,
                        x.ContractEffectiveDate,
                        x.ContractExpirationDate,
                        idPricingSchedule = x.IdPricingSchedule,
                        x.IdPricingScheduleNavigation.PricingSchedule1,
                        idPaymentRecurrence = x.IdPaymentRecurrence,
                        PaymentRecurrence1 = x.IdPaymentRecurrenceNavigation.Name,
                        x.ReferralFee,
                        x.IdReferralFee,
                        x.DocumentGeneralContractPricingInfos,
                        x.Description
                    }),
                    headquarterOffice = c.OfficeInformations.SingleOrDefault(x => x.IdTypeOffice == 1),
                    OfficeInformations = c.OfficeInformations.Select(v => new
                    {
                        v.Id,
                        idTypeOffice = v.IdTypeOffice,
                        idCountry = v.IdCountry,
                        idCity = v.IdCity,
                        TypeOffice = v.IdTypeOfficeNavigation.Type,
                        Country = v.IdCountryNavigation.Name,
                        City = v.IdCityNavigation.Name,
                        v.CommercialName,
                        v.LegalName,
                        v.CurrentAddress,
                        v.ZipCode,
                        v.OfficeContacts,
                        v.DocumentOfficeInformations,
                        PaymentInformationOffices = _context.PaymentInformationOffices.Include(i => i.WireTransferPaymentInformationOffices).Where(q => q.IdOfficeInformation == v.Id).ToList()
                    }),
                    contacts = _context.OfficeContacts.Where(x => x.IdOfficeInformationNavigation.IdClientPartnerProfile == Id).Select(f => new
                    {
                        f.Id,
                        idOfficeInformation = f.IdOfficeInformation,
                        office = f.IdOfficeInformationNavigation.CommercialName,
                        idContactType = f.IdContactType,
                        f.IdContactTypeNavigation.ContactType,
                        f.ContactName,
                        f.Tittle,
                        f.PhoneNumber,
                        f.Email,
                        idCity = f.IdCity,
                        f.IdCityNavigation.Name,
                    }),
                    //c.ClientPartnerProfileClientIdClientFromNavigations,
                    //c.ClientPartnerProfileClientIdClientToNavigations,
                    client = _context.ClientPartnerProfileClients.Where(x => x.IdClientFrom == Id).Select(n => new
                    {
                        n.Id,
                        n.IdClientToNavigation.Name,
                        n.IdClientTo,
                        country = _context.OfficeInformations.FirstOrDefault(x => x.IdClientPartnerProfile == n.IdClientTo).IdCountryNavigation.Name,
                        service_line = "",
                        PricingType = n.IdClientToNavigation.IdPricingTypeNavigation.PricingType1
                    }),
                    service_location = c.ServiceLocations.Where(x => x.IdClientPartnerProfile == Id)
                    .Select(h => new
                    {
                        immi = c.ServiceLocations.Where(x => x.IdServiceLine == 1 && x.IdClientPartnerProfile == Id).Select(g => new
                        {
                            g.Id,
                            g.IdClientPartnerProfile,
                            g.IdServiceLine,
                            g.IdService,
                            immi = g.IdServiceNavigation.Service,
                            g.NickName,
                            country_total = g.ServiceLocationCountries.Count(),
                            servicelocationcountries = g.ServiceLocationCountries.Where(x => x.IdServiceLocation == g.Id).Select(f => new
                            {
                                f.Id,
                                f.IdServiceLocation,
                                f.IdCountry,
                                f.StandarScopeDocuments,
                                f.ScopeDescription,
                                f.IdCountryNavigation.Name,
                                count = f.DocumentLocationCountries.Count(),
                                f.DocumentLocationCountries,
                                Documents = f.DocumentLocationCountries.Select(w => new
                                {
                                    w.Id,
                                    w.IdDocumentTypeNavigation.DocumentType,
                                    w.UploadDate,
                                    w.StatusNavigation.Status,
                                    w.PrivacyNavigation.Privacy
                                }).ToList()
                            }).ToList(),

                        }).ToList(),
                        relo = c.ServiceLocations.Where(x => x.IdServiceLine == 2 && x.IdClientPartnerProfile == Id).Select(g => new
                        {
                            g.Id,
                            g.IdClientPartnerProfile,
                            g.IdServiceLine,
                            g.IdService,
                            immi = g.IdServiceNavigation.Service,
                            g.NickName,
                            country_total = g.ServiceLocationCountries.Count(),
                            servicelocationcountries = g.ServiceLocationCountries.Where(x => x.IdServiceLocation == g.Id).Select(f => new
                            {
                                f.Id,
                                f.IdServiceLocation,
                                f.IdCountry,
                                f.StandarScopeDocuments,
                                f.ScopeDescription,
                                f.IdCountryNavigation.Name,
                                count = f.DocumentLocationCountries.Count(),
                                f.DocumentLocationCountries,
                                Documents = f.DocumentLocationCountries.Select(w => new
                                {
                                    w.Id,
                                    w.IdDocumentTypeNavigation.DocumentType,
                                    w.UploadDate,
                                    w.StatusNavigation.Status,
                                    w.PrivacyNavigation.Privacy
                                }).ToList()
                            }).ToList(),
                        }).ToList()
                    }).ToList(),
                    documents = c.DocumentClientPartnerProfiles.Select(d => new
                    {
                        d.Id,
                        d.IdDocumentTypeNavigation.DocumentType,
                        d.UploadDate,
                        d.Description,
                        d.Comments,
                        Privacy = d.Privacy == true ? "Private" : "Public",

                    }),
                    service_score_awards = c.ServiceScoreAwards.Select(y => new
                    {
                        y.Id,
                        type = y.IdType1.Type,
                        y.Description,
                        y.IdServiceLineNavigation.ServiceLine,
                        y.Year,
                        y.Comment
                    }),
                    experience_team = c.ClientPartnerProfileExperienceTeams.Select(t => new
                    {
                        immi = c.ClientPartnerProfileExperienceTeams.Where(x => x.IdServiceLine == 1).Select(g => new
                        {
                            g.Id,
                            Avatar = g.User.ProfileUsers.FirstOrDefault().Photo,
                            name = g.User.Name + " " + g.User.LastName,
                            title = g.User.Role.Role
                        }),
                        relo = c.ClientPartnerProfileExperienceTeams.Where(x => x.IdServiceLine == 2).Select(g => new
                        {
                            g.Id,
                            Avatar = g.User.ProfileUsers.FirstOrDefault().Photo,
                            name = g.User.Name + " " + g.User.LastName,
                            title = g.User.Role.Role
                        })
                    })
                }).ToList();

            return new ObjectResult(query);
        }

        public List<ClientPartnerProfileDto> GetClientPartnerBytestId(int Id)
        {
            var client = _context.ClientPartnerProfiles
                .Include(c => c.ServiceLocations)
                .Where(x => x.Id == Id)
                .Select(c => new ClientPartnerProfileDto
                {
                    Id = c.Id,
                    Photo = c.Photo,
                    IdTypePartnerClientProfile = c.IdTypePartnerClientProfile,
                    BelongsToPartner = c.BelongsToPartner,
                    Name = c.Name,
                    PartnerClientSince = c.PartnerClientSince,
                    IdResponsiblePremierOffice = c.IdResponsiblePremierOffice,
                    AssignedTo = c.AssignedTo,
                    IdReferralFee = c.IdReferralFee,
                    IdPaymentRecurrence = c.IdPaymentRecurrence,
                    IdPricingType = c.IdPricingType,
                    IdCompanyType = c.IdCompanyType,
                    About = c.About,
                    ActivityLogs = c.ActivityLogs.Select(z => new ActivityLogDto
                    {
                        Id = z.Id,
                        IdClientPartnerProfile = z.IdClientPartnerProfile,
                        PremierSalesForce = z.PremierSalesForce,
                        Activity = z.Activity,
                        Date = z.Date
                    }).ToList(),
                    TermsDeals = c.TermsDeals.Select(f => new TermsDealDto
                    {
                        Id = f.Id,
                        IdClientPartnerProfile = f.IdClientPartnerProfile,
                        IdPrice = f.IdPrice,
                        IdCurrency = f.IdPrice,
                        Volume = f.Volume,
                        Sevices = f.Sevices,
                        Amount = f.Amount,
                        Fixed = f.Fixed,
                        BussinesUnit = f.BussinesUnit
                    }).ToList(),
                    GeneralContractPricingInfos = c.GeneralContractPricingInfos.Select(x => new GeneralContractPricingInfoDto
                    {
                        Id = x.Id,
                        ContractEffectiveDate = x.ContractEffectiveDate,
                        ContractExpirationDate = x.ContractExpirationDate,
                        IdPricingSchedule = x.IdPricingSchedule,
                        PricingSchedule1 = x.IdPricingScheduleNavigation.PricingSchedule1,
                        IdPaymentRecurrence = x.IdPaymentRecurrence,
                        PaymentRecurrence1 = x.IdPaymentRecurrenceNavigation.Name,
                        ReferralFee = x.ReferralFee,
                        IdReferralFee = x.IdReferralFee,
                        DocumentGeneralContractPricingInfos = x.DocumentGeneralContractPricingInfos.Select(b => new DocumentGeneralContractPricingInfoDto
                        {
                            Id = b.Id,
                            IdGeneralContractPricingInfo = b.IdGeneralContractPricingInfo,
                            IdDocumentType = b.IdDocumentType,
                            UpdateDate = b.UpdateDate,
                            DocumentName = b.DocumentName,
                            Description = b.Description,
                            FileRequest = b.FileRequest
                        }).ToList(),
                        Description = x.Description
                    }).ToList(),
                    HeadquarterOffice = c.OfficeInformations.Where(x => x.IdTypeOffice == 1)
                    .Select(b => new OfficeInformationDto
                    {
                        Id = b.Id,
                        IdClientPartnerProfile = b.IdClientPartnerProfile,
                        IdTypeOffice = b.IdTypeOffice,
                        CommercialName = b.CommercialName,
                        LegalName = b.LegalName,
                        IdCountry = b.IdCountry,
                        IdCity = b.IdCity,
                        CurrentAddress = b.CurrentAddress,
                        ZipCode = b.ZipCode
                    }).ToList(),
                    OfficeInformations = c.OfficeInformations
                    .Select(v => new OfficeInformationDto
                    {
                        Id = v.Id,
                        IdTypeOffice = v.IdTypeOffice,
                        IdCountry = v.IdCountry,
                        IdCity = v.IdCity,
                        IdState = v.IdState,
                        TypeOffice = v.IdTypeOfficeNavigation.Type,
                        Country = v.IdCountryNavigation.Name,
                        City = v.IdCityNavigation.Name,
                        State = v.IdStateNavigation.Name,
                        CommercialName = v.CommercialName,
                        LegalName = v.LegalName,
                        CurrentAddress = v.CurrentAddress,
                        ZipCode = v.ZipCode,
                        OfficeContacts = v.OfficeContacts.Select(k => new OfficeContactDto
                        {
                            Id = k.Id,
                            IdOfficeInformation = k.IdOfficeInformation,
                            IdContactType = k.IdContactType,
                            ContactName = k.ContactName,
                            Tittle = k.Tittle,
                            PhoneNumber = k.PhoneNumber,
                            Email = k.Email,
                            IdCity = k.IdCity,
                            // IdCountry = v.IdCountry,
                            City = k.IdCityNavigation.Name,
                            // Country = v.IdCountryNavigation.Name,
                            State = v.IdStateNavigation.Name,
                            IdState = v.IdState
                        }).ToList(),
                        DocumentOfficeInformations = v.DocumentOfficeInformations.Select(k => new DocumentOfficeInformationDto
                        {
                            Id = k.Id,
                            IdOfficeInformation = k.IdOfficeInformation,
                            IdDocumentType = k.IdDocumentType,
                            UpdatedDate = k.UpdatedDate,
                            Description = k.Description,
                            FileName = k.FileName,
                            FileRequest = k.FileRequest
                        }).ToList(),
                        PaymentInformationOffices = _context.PaymentInformationOffices
                        .Include(i => i.WireTransferPaymentInformationOffices)
                        .Where(q => q.IdOfficeInformation == v.Id)
                        .Select(l => new PaymentInformationOfficeDto
                        {
                            Id = l.Id,
                            IdOfficeInformation = l.IdOfficeInformation,
                            FiscalInvoice = l.FiscalInvoice,
                            CreditCard = l.CreditCard,
                            Checks = l.Checks,
                            PayToOrderOf = l.PayToOrderOf,
                            Cash = l.Cash,
                            Comment = l.Comment,
                            GeneralComment = l.GeneralComment
                        }).ToList()
                    }).ToList(),
                    documents = c.DocumentClientPartnerProfiles.Select(d => new DocumentClientPartnerProfileDto
                    {
                        Id = d.Id,
                        DocumentType = d.IdDocumentTypeNavigation.DocumentType,
                        UploadDate = d.UploadDate,
                        Description = d.Description,
                        Comments = d.Comments,
                        Privacy = d.Privacy == true ? "Private" : "Public",

                    }).ToList(),
                    service_score_awards = c.ServiceScoreAwards.Select(y => new ServiceScoreAwardDto
                    {
                        Id = y.Id,
                        Type = y.IdType1.Type,
                        Description = y.Description,
                        ServiceLine = y.IdServiceLineNavigation.ServiceLine,
                        Year = y.Year,
                        Comment = y.Comment
                    }).ToList(),
                    experience_team_imm = c.ClientPartnerProfileExperienceTeams
                    .Where(a => a.IdServiceLine == 1)
                    .Select(t => new ClientPartnerProfileExperienceTeamDto
                    {
                        Id = t.Id,
                        Avatar = t.User.ProfileUsers.FirstOrDefault().Photo,
                        UserId = t.UserId,
                        IdServiceLine = t.IdServiceLine,
                        name = t.User.Name + " " + t.User.LastName,
                        title = t.User.Role.Role,
                        ProfileId = t.User.ProfileUsers.FirstOrDefault().Id
                    }).ToList(),
                    experience_team_relo = c.ClientPartnerProfileExperienceTeams
                    .Where(a => a.IdServiceLine == 2)
                    .Select(t => new ClientPartnerProfileExperienceTeamDto
                    {
                        Id = t.Id,
                        Avatar = t.User.ProfileUsers.FirstOrDefault().Photo,
                        UserId = t.UserId,
                        IdServiceLine = t.IdServiceLine,
                        name = t.User.Name + " " + t.User.LastName,
                        title = t.User.Role.Role,
                        ProfileId = t.User.ProfileUsers.FirstOrDefault().Id
                    }).ToList(),
                    Client = c.ClientPartnerProfileClientIdClientFromNavigations.Select(s => new ClientPartnerProfileDataDto
                    {
                        Id = s.Id,
                        Country = s.IdClientToNavigation.OfficeInformations.FirstOrDefault(x => x.IdClientPartnerProfile == s.IdClientTo).IdCountryNavigation.Name,
                        PricingType = s.IdClientToNavigation.IdPricingTypeNavigation.PricingType1,
                        Name = s.IdClientToNavigation.Name,
                        IdClientTo = s.IdClientTo
                    }).ToList()
                }).ToList();

            return client;
        }

        public List<ServiceLocationDto> GetServiceLocation(int id, int serviceLine) {

            var ServiceLocations = _context.ServiceLocations
                  .Where(a => a.IdClientPartnerProfile == id && a.IdServiceLine == serviceLine)
                  .Select(g => new ServiceLocationDto
                  {
                      Id = g.Id,
                      IdClientPartnerProfile = g.IdClientPartnerProfile,
                      IdServiceLine = g.IdServiceLine,
                      IdService = g.IdService,
                      ServicesName = g.IdServiceNavigation.Service,
                      NickName = g.NickName,
                      country_total = g.ServiceLocationCountries.Count(),
                      ServiceLocationCountries = g.ServiceLocationCountries.Select(f => new ServiceLocationCountryDto
                      {
                          Id = f.Id,
                          IdServiceLocation = f.IdServiceLocation,
                          IdCountry = f.IdCountry,
                          StandarScopeDocuments = f.StandarScopeDocuments,
                          ScopeDescription = f.ScopeDescription,
                          Name = f.IdCountryNavigation.Name,
                          Count = f.DocumentLocationCountries.Count(),
                          DocumentLocationCountries = f.DocumentLocationCountries.Select(w => new DocumentLocationCountryDto
                          {
                              Id = w.Id,
                              DocumentType = w.IdDocumentTypeNavigation.DocumentType,
                              UploadDate = w.UploadDate,
                              StatusName = w.StatusNavigation.Status,
                              Status = w.Status,
                              PrivacyName = w.PrivacyNavigation.Privacy,
                              Privacy = w.Privacy,
                              FileName = w.FileName,
                              FileRequest = w.FileRequest,
                              IdDocumentType = w.IdDocumentType,
                              IdServiceLocationCountry = w.IdServiceLocationCountry
                          }).ToList(),
                      }).ToList()
                  }).ToList();

                  return ServiceLocations;
        }

        public ActionResult GetClientPartnerCatalog()
        {
            var query = _context.ClientPartnerProfiles
                .Select(c => new
                {
                    c.Id,
                    c.Name
                }).ToList();

            return new ObjectResult(query);
        }

        public List<int> GetIdServicesByPartner(int id, int serviceLine)
        {

            var idPartner = _context.ClientPartnerProfiles.FirstOrDefault(x => x.Id == id).Id;
            var typePartner = _context.ClientPartnerProfiles.FirstOrDefault(x => x.Id == id).IdTypePartnerClientProfile;
            var query = _context.Services
               .Where(x => x.ServiceLine == serviceLine)
               .Select(c => c.Service1.Value).ToList();

            if (typePartner != 1)
            {
                idPartner = _context.ClientPartnerProfiles.FirstOrDefault(x => x.Id == id).BelongsToPartner.Value;
                query = _context.ServiceLocations
                  .Where(x => x.IdClientPartnerProfile == idPartner && x.IdServiceLine == serviceLine)
                  .Select(c => c.IdService).ToList();
            }

            return query;
        }

        public ActionResult GetExperienceTeamCatalog(int serviceLine, int? client)
        {
        
            int[] _i = { 1,2 };

            var _partner = _context.ClientPartnerProfiles.SingleOrDefault(x => x.Id == client && x.IdCompanyType == 2 && x.BelongsToPartner != null);

            if (_partner != null) {

                var query = _context.ClientPartnerProfileExperienceTeams.Where(x => x.IdClientPartnerProfile == _partner.BelongsToPartner && x.IdServiceLine == serviceLine)
                    .Select(s => new
                    {
                        Id = s.User.Id,
                        name = $"{s.User.Name} {s.User.LastName}"
                    }).ToList();

                return new ObjectResult(query);
            }
            else
            {
                var query = _context.Users
                .Where(x => _i.Contains(x.RoleId)
                    && (serviceLine == 1
                        ? x.ProfileUsers.FirstOrDefault().Immigration == true
                            ? true : false
                        : x.ProfileUsers.FirstOrDefault().Relocation == true
                            ? true : false))
                .Select(c => new
                {
                    c.Id,
                    name = $"{c.Name} {c.LastName}"
                }).ToList();
                if (client.HasValue)
                {
                    var experienceTeam =
                        _context.ClientPartnerProfileExperienceTeams.Where(x => x.IdClientPartnerProfile == client && x.IdServiceLine == serviceLine);
                    query = query.Where(x => !experienceTeam.Select(s => s.UserId).Contains(x.Id)).ToList();
                }

                return new ObjectResult(query);
            }
        }

        public bool UpdatePartner(int client, int partner)
        {
            bool result = false;
            var belongPartner = _context.ClientPartnerProfiles
                .SingleOrDefault(s => s.Id == client);
            if (belongPartner != null)
            {
                var changePartner = belongPartner.BelongsToPartner == partner ? true : false;
                if (!changePartner)
                {
                    var fromTo = _context.ClientPartnerProfileClients
                        .FirstOrDefault(x =>
                            x.IdClientFrom == belongPartner.BelongsToPartner && x.IdClientTo == client);

                    if (fromTo != null) { _context.ClientPartnerProfileClients.Remove(fromTo);}

                    ClientPartnerProfileClient clientPartner = new ClientPartnerProfileClient();
                    clientPartner.IdClientFrom = partner;
                    clientPartner.IdClientTo = client;
                    _context.ClientPartnerProfileClients.Add(clientPartner);

                    belongPartner.BelongsToPartner = partner;
                    Update(belongPartner, belongPartner.Id);
                    result = true;
                }
            }
            return result;
        }
    }
}
