using biz.premier.Entities;
using biz.premier.Repository.ImmigrationProfile;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace dal.premier.Repository.ImmigrationProfile
{
    public class ImmigrationProfileRepository : GenericRepository<biz.premier.Entities.ImmigrationProfile>, IImmigrationProfileRepository
    {
        public ImmigrationProfileRepository(Db_PremierContext context) : base(context) { }

        public ActionResult SelectCustom(int sr)
        {
            var query = _context.Set<biz.premier.Entities.ImmigrationProfile>().Where(s => s.ServiceRecordId == sr)
                .Include(i => i.LenguageProficiencies)
                .Include(i => i.EducationalBackgrounds)
                .Include(i => i.DependentImmigrationInfos)
                .Include(i => i.AssigmentInformation)
                .Include(i => i.PassportInformation)
                .Include(i => i.PreviousHostCountry)
                .FirstOrDefault();
            if (query != null)
            {
                return new ObjectResult(query);
            }
            else
            {
                return new ObjectResult(null);
            }
        }

        public biz.premier.Entities.ImmigrationProfile UpdateCustom(biz.premier.Entities.ImmigrationProfile immigrationProfile)
        {
            var query = _context.Set<biz.premier.Entities.ImmigrationProfile>()
                .Include(i => i.LenguageProficiencies)
                .Include(i => i.EducationalBackgrounds)
                .Include(i => i.DependentImmigrationInfos)
                .Include(i => i.AssigmentInformation)
                .Include(i => i.PassportInformation)
                .Include(i => i.PreviousHostCountry)
                .SingleOrDefault(s => s.Id == immigrationProfile.Id);
            _context.Entry(query).CurrentValues.SetValues(immigrationProfile);
            foreach (var i in immigrationProfile.LenguageProficiencies)
            {
                var lp = query.LenguageProficiencies.Where(w => w.Id == i.Id).FirstOrDefault();
                if (lp == null)
                {
                    query.LenguageProficiencies.Add(i);
                }
                else
                {
                    _context.Entry(lp).CurrentValues.SetValues(i);
                }
            }

            foreach (var e in immigrationProfile.EducationalBackgrounds)
            {
                var eb = query.EducationalBackgrounds.Where(w => w.Id == e.Id).FirstOrDefault();
                if (eb == null)
                {
                    query.EducationalBackgrounds.Add(e);
                }
                else
                {
                    _context.Entry(eb).CurrentValues.SetValues(e);
                }
            }

            foreach (var d in immigrationProfile.DependentImmigrationInfos)
            {
                var di = query.DependentImmigrationInfos.Where(w => w.Id == d.Id).FirstOrDefault();
                if (di == null)
                {
                    query.DependentImmigrationInfos.Add(d);
                }
                else
                {
                    _context.Entry(di).CurrentValues.SetValues(d);
                    //foreach (var i in d.DocumentDependentImmigrationInfos)
                    //{
                    //    var _exist = di.DocumentDependentImmigrationInfos.Where(x => x.Id == i.Id).FirstOrDefault();
                    //    if (_exist == null)
                    //    {
                    //        di.DocumentDependentImmigrationInfos.Add(i);
                    //        _context.SaveChanges();
                    //    }
                    //    else
                    //    {
                    //        _context.Entry(_exist).CurrentValues.SetValues(i);
                    //    }
                    //}
                }
            }

            if (query.PreviousHostCountry == null)
            {
                query.PreviousHostCountry = immigrationProfile.PreviousHostCountry;
            }
            else
            {
                _context.Entry(query.PreviousHostCountry).CurrentValues.SetValues(immigrationProfile.PreviousHostCountry);
            }

            if (query.PassportInformation == null)
            {
                query.PassportInformation = immigrationProfile.PassportInformation;
            }
            else
            {
                _context.Entry(query.PassportInformation).CurrentValues.SetValues(immigrationProfile.PassportInformation);
            }

            //var ai = query.AssigmentInformation.Where(w => w.Id == a.Id);
            if (query.AssigmentInformation == null)
            {
                query.AssigmentInformation = immigrationProfile.AssigmentInformation;
            }
            else
            {
                _context.Entry(query.AssigmentInformation).CurrentValues.SetValues(immigrationProfile.AssigmentInformation);
            }

            _context.SaveChanges();
            return query;
        }

        public ActionResult document_assigne(int service_record)
        {
            var query = _context.DependentInformations
                .Where(x => x.AssigneeInformation.ServiceRecordId == service_record)
                .Select(c => new
                {
                    c.Id,
                    dependent = c.Relationship.Relationship + " / " + c.Name,
                    id_dependet = c.RelationshipId,
                    avatar = c.Photo,
                    document = c.DocumentDependentInformations
                    .Select(n => new
                    {
                        n.Id,
                        DocumentType = _context.CatDocumentTypes.SingleOrDefault(x => x.Id == n.DocumentType).DocumentType,
                        n.CreatedDate,
                        n.IssueDate,
                        n.ExpirationDate,
                        n.IssuingAuthority,
                        country_origin = n.CountryOriginNavigation.Name,
                        n.Relationship,
                        avatar = n.DependentInformationNavigation.Photo
                    }).ToList()
                }).ToList();

            return new ObjectResult(query);
        }

        public ActionResult document_assigneById(int id)
        {
            var query = _context.DocumentDependentInformations
                .Where(x => x.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.DependentInformation,
                    upload_date = c.CreatedDate,
                    ulr_document = c.FileRequest,
                    document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                    relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                    c.IssueDate,
                    c.ExpirationDate,
                    c.IssuingAuthority,
                    c.CountryOriginNavigation.Name,
                    c.Comment
                }).ToList();

            return new ObjectResult(query);
        }

        public ActionResult immigration_library(int service_record, int serviceLineId)
        {
            int[] _status = new int[] { 1, 2 };
            var consultHistoryStandalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == service_record && x.WorkOrder.ServiceLineId == serviceLineId && _status.Contains(x.WorkOrder.StatusId.Value))
                .Select(s => new
                {
                    s.WorkOrder.NumberWorkOrder,
                    s.Service.Service,
                    s.ServiceNumber,
                    s.CreatedDate,
                    documents = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                    .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentEntryVisas, e => e.Id, a => a.EntryVisaId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,                        
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 1
                    })
             : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentWorkPermits, e => e.Id, a => a.WorkPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 2
                    })
             : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentVisaDeregistrations, e => e.Id, a => a.VisaDeregistrationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 3
                    })
             : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentResidencyPermits, e => e.Id, a => a.ResidencyPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 4
                    })
             : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentDocumentManagements, e => e.Id, a => a.DocumentManagementId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 5
                    })
             : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentLocalDocumentations, e => e.Id, a => a.LocalDocumentationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 6
                    })
             : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentCorporateAssistances, e => e.Id, a => a.CorporateAssistanceId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 7
                    })
             : s.CategoryId == 8 ? s.WorkOrderService.Renewals
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentRenewals, e => e.Id, a => a.RenewalId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 8
                    })
             : s.CategoryId == 9 ? s.WorkOrderService.Notifications
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentNotifications, e => e.Id, a => a.NotificationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 9
                    })
             : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentLegalReviews, e => e.Id, a => a.LegalReviewId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 10
                    })
             : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentPredecisionOrientations, e => e.Id, a => a.PredecisionOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 12
                    })
             : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentAreaOrientations, e => e.Id, a => a.AreaOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 13
                    })
             : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentSettlingIns, e => e.Id, a => a.SettlingInId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 14
                    })
             : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentSchoolingSearches, e => e.Id, a => a.SchoolingSearchId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 15
                    })
             : s.CategoryId == 16 ? s.WorkOrderService.Departures
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentDepartures, e => e.Id, a => a.DepartaureId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 16
                    })
             : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentTemporaryHousingCoordinatons, e => e.Id, a => a.TemporaryHousingCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 17
                    })
             : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
            .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentRentalFurnitureCoordinations, e => e.Id, a => a.RentalFurnitureCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 18
                    })
             : s.CategoryId == 19 ? s.WorkOrderService.Transportations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentTransportations, e => e.Id, a => a.TransportationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 19
                    })
             : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentAirportTransportationServices, e => e.Id, a => a.AirportTransportationServicesId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 20
                    })
             : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentHomeFindings, e => e.Id, a => a.HomeFindingId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 21
                    })
             : null
                }).ToList();


            var consultHistoryBundle = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == service_record && x.BundledServiceOrder.WorkOrder.ServiceLineId == serviceLineId
                && _status.Contains(x.BundledServiceOrder.WorkOrder.StatusId.Value))
                .Select(s => new
                {
                    s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    s.Service.Service,
                    s.ServiceNumber,
                    s.CreatedDate,
                    documents = s.CategoryId == 1 ? s.WorkServices.EntryVisas
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentEntryVisas, e => e.Id, a => a.EntryVisaId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 1
                    })
             : s.CategoryId == 2 ? s.WorkServices.WorkPermits
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentWorkPermits, e => e.Id, a => a.WorkPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 2
                    })
             : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentVisaDeregistrations, e => e.Id, a => a.VisaDeregistrationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 3
                    })
             : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentResidencyPermits, e => e.Id, a => a.ResidencyPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 4
                    })
             : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentDocumentManagements, e => e.Id, a => a.DocumentManagementId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 5
                    })
             : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentLocalDocumentations, e => e.Id, a => a.LocalDocumentationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 6
                    })
             : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentCorporateAssistances, e => e.Id, a => a.CorporateAssistanceId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 7
                    })
             : s.CategoryId == 8 ? s.WorkServices.Renewals
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentRenewals, e => e.Id, a => a.RenewalId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 8
                    })
             : s.CategoryId == 9 ? s.WorkServices.Notifications
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentNotifications, e => e.Id, a => a.NotificationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 9
                    })
             : s.CategoryId == 10 ? s.WorkServices.LegalReviews
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentLegalReviews, e => e.Id, a => a.LegalReviewId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 10
                    })
             : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentPredecisionOrientations, e => e.Id, a => a.PredecisionOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 12
                    })
             : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentAreaOrientations, e => e.Id, a => a.AreaOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 13
                    })
             : s.CategoryId == 14 ? s.WorkServices.SettlingIns
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentSettlingIns, e => e.Id, a => a.SettlingInId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 14
                    })
             : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentSchoolingSearches, e => e.Id, a => a.SchoolingSearchId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 15
                    })
             : s.CategoryId == 16 ? s.WorkServices.Departures
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentDepartures, e => e.Id, a => a.DepartaureId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 16
                    })
             : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
              .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentTemporaryHousingCoordinatons, e => e.Id, a => a.TemporaryHousingCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 17
                    })
             : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentRentalFurnitureCoordinations, e => e.Id, a => a.RentalFurnitureCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 18
                    })
             : s.CategoryId == 19 ? s.WorkServices.Transportations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentTransportations, e => e.Id, a => a.TransportationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 19
                    })
             : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentAirportTransportationServices, e => e.Id, a => a.AirportTransportationServicesId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 20
                    })
             : s.CategoryId == 21 ? s.WorkServices.HomeFindings
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentHomeFindings, e => e.Id, a => a.HomeFindingId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 21
                    })
             : null

                }).ToList().Union(consultHistoryStandalone).OrderBy(x => x.CreatedDate); ;

            return new ObjectResult(consultHistoryBundle);
        }

        public ActionResult All_immigration_library(int service_record, int service_line)
        {
            int[] _status = new int[] { 3 };
            var consultHistoryStandalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrder.ServiceRecordId == service_record && _status.Contains(x.WorkOrder.StatusId.Value))
                .Select(s => new
                {
                    s.WorkOrder.NumberWorkOrder,
                    s.Service.Service,
                    s.ServiceNumber,
                    s.CreatedDate,
                    documents = s.CategoryId == 1 ? s.WorkOrderService.EntryVisas
                    .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentEntryVisas, e => e.Id, a => a.EntryVisaId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 1
                    })
             : s.CategoryId == 2 ? s.WorkOrderService.WorkPermits
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentWorkPermits, e => e.Id, a => a.WorkPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 2
                    })
             : s.CategoryId == 3 ? s.WorkOrderService.VisaDeregistrations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentVisaDeregistrations, e => e.Id, a => a.VisaDeregistrationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 3
                    })
             : s.CategoryId == 4 ? s.WorkOrderService.ResidencyPermits
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentResidencyPermits, e => e.Id, a => a.ResidencyPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 4
                    })
             : s.CategoryId == 5 ? s.WorkOrderService.DocumentManagements
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentDocumentManagements, e => e.Id, a => a.DocumentManagementId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 5
                    })
             : s.CategoryId == 6 ? s.WorkOrderService.LocalDocumentations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentLocalDocumentations, e => e.Id, a => a.LocalDocumentationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 6
                    })
             : s.CategoryId == 7 ? s.WorkOrderService.CorporateAssistances
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentCorporateAssistances, e => e.Id, a => a.CorporateAssistanceId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 7
                    })
             : s.CategoryId == 8 ? s.WorkOrderService.Renewals
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentRenewals, e => e.Id, a => a.RenewalId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 8
                    })
             : s.CategoryId == 9 ? s.WorkOrderService.Notifications
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentNotifications, e => e.Id, a => a.NotificationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 9
                    })
             : s.CategoryId == 10 ? s.WorkOrderService.LegalReviews
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentLegalReviews, e => e.Id, a => a.LegalReviewId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 10
                    })
             : s.CategoryId == 12 ? s.WorkOrderService.PredecisionOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentPredecisionOrientations, e => e.Id, a => a.PredecisionOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 12
                    })
             : s.CategoryId == 13 ? s.WorkOrderService.AreaOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentAreaOrientations, e => e.Id, a => a.AreaOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 13
                    })
             : s.CategoryId == 14 ? s.WorkOrderService.SettlingIns
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentSettlingIns, e => e.Id, a => a.SettlingInId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 14
                    })
             : s.CategoryId == 15 ? s.WorkOrderService.SchoolingSearches
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentSchoolingSearches, e => e.Id, a => a.SchoolingSearchId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 15
                    })
             : s.CategoryId == 16 ? s.WorkOrderService.Departures
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentDepartures, e => e.Id, a => a.DepartaureId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 16
                    })
             : s.CategoryId == 17 ? s.WorkOrderService.TemporaryHousingCoordinatons
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentTemporaryHousingCoordinatons, e => e.Id, a => a.TemporaryHousingCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 17
                    })
             : s.CategoryId == 18 ? s.WorkOrderService.RentalFurnitureCoordinations
            .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentRentalFurnitureCoordinations, e => e.Id, a => a.RentalFurnitureCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 18
                    })
             : s.CategoryId == 19 ? s.WorkOrderService.Transportations
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentTransportations, e => e.Id, a => a.TransportationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 19
                    })
             : s.CategoryId == 20 ? s.WorkOrderService.AirportTransportationServices
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentAirportTransportationServices, e => e.Id, a => a.AirportTransportationServicesId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 20
                    })
             : s.CategoryId == 21 ? s.WorkOrderService.HomeFindings
             .Where(x => x.WorkOrderServicesId == s.WorkOrderServiceId)
                    .Join(_context.DocumentHomeFindings, e => e.Id, a => a.HomeFindingId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 21
                    })
             : null
                }).ToList();


            var consultHistoryBundle = _context.BundledServices
                .Where(x => x.BundledServiceOrder.WorkOrder.ServiceRecordId == service_record
                && _status.Contains(x.BundledServiceOrder.WorkOrder.StatusId.Value))
                .Select(s => new
                {
                    s.BundledServiceOrder.WorkOrder.NumberWorkOrder,
                    s.Service.Service,
                    s.ServiceNumber,
                    s.CreatedDate,
                    documents = s.CategoryId == 1 ? s.WorkServices.EntryVisas
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentEntryVisas, e => e.Id, a => a.EntryVisaId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 1
                    })
             : s.CategoryId == 2 ? s.WorkServices.WorkPermits
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentWorkPermits, e => e.Id, a => a.WorkPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 2
                    })
             : s.CategoryId == 3 ? s.WorkServices.VisaDeregistrations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentVisaDeregistrations, e => e.Id, a => a.VisaDeregistrationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 3
                    })
             : s.CategoryId == 4 ? s.WorkServices.ResidencyPermits
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentResidencyPermits, e => e.Id, a => a.ResidencyPermitId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 4
                    })
             : s.CategoryId == 5 ? s.WorkServices.DocumentManagements
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentDocumentManagements, e => e.Id, a => a.DocumentManagementId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 5
                    })
             : s.CategoryId == 6 ? s.WorkServices.LocalDocumentations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentLocalDocumentations, e => e.Id, a => a.LocalDocumentationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 6
                    })
             : s.CategoryId == 7 ? s.WorkServices.CorporateAssistances
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentCorporateAssistances, e => e.Id, a => a.CorporateAssistanceId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 7
                    })
             : s.CategoryId == 8 ? s.WorkServices.Renewals
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentRenewals, e => e.Id, a => a.RenewalId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 8
                    })
             : s.CategoryId == 9 ? s.WorkServices.Notifications
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentNotifications, e => e.Id, a => a.NotificationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 9
                    })
             : s.CategoryId == 10 ? s.WorkServices.LegalReviews
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentLegalReviews, e => e.Id, a => a.LegalReviewId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 10
                    })
             : s.CategoryId == 12 ? s.WorkServices.PredecisionOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentPredecisionOrientations, e => e.Id, a => a.PredecisionOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 12
                    })
             : s.CategoryId == 13 ? s.WorkServices.AreaOrientations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentAreaOrientations, e => e.Id, a => a.AreaOrientationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 13
                    })
             : s.CategoryId == 14 ? s.WorkServices.SettlingIns
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentSettlingIns, e => e.Id, a => a.SettlingInId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 14
                    })
             : s.CategoryId == 15 ? s.WorkServices.SchoolingSearches
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentSchoolingSearches, e => e.Id, a => a.SchoolingSearchId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 15
                    })
             : s.CategoryId == 16 ? s.WorkServices.Departures
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentDepartures, e => e.Id, a => a.DepartaureId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 16
                    })
             : s.CategoryId == 17 ? s.WorkServices.TemporaryHousingCoordinatons
              .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentTemporaryHousingCoordinatons, e => e.Id, a => a.TemporaryHousingCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 17
                    })
             : s.CategoryId == 18 ? s.WorkServices.RentalFurnitureCoordinations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentRentalFurnitureCoordinations, e => e.Id, a => a.RentalFurnitureCoordinationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 18
                    })
             : s.CategoryId == 19 ? s.WorkServices.Transportations
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentTransportations, e => e.Id, a => a.TransportationId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 19
                    })
             : s.CategoryId == 20 ? s.WorkServices.AirportTransportationServices
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentAirportTransportationServices, e => e.Id, a => a.AirportTransportationServicesId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 20
                    })
             : s.CategoryId == 21 ? s.WorkServices.HomeFindings
             .Where(x => x.WorkOrderServicesId == s.WorkServicesId)
                    .Join(_context.DocumentHomeFindings, e => e.Id, a => a.HomeFindingId,
                    (e, a) => new
                    {
                        a.Id,
                        DocumentType = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == a.DocumentType).DocumentType,
                        a.CreatedDate,
                        a.IssueDate,
                        a.ExpirationDate,
                        a.IssuingAuthority,
                        a.CountryOriginNavigation.Name,
                        tipo = 21
                    })
             : null

                }).ToList().Union(consultHistoryStandalone).OrderBy(x => x.CreatedDate); ;

            return new ObjectResult(consultHistoryBundle);
        }

        public ActionResult immigration_libraryById(int tipo, int id)
        {
            List<documents_library> document = new List<documents_library>();

            switch (tipo)
            {
                case 1:
                    document = _context.DocumentEntryVisas
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    
                    break;
                case 2:
                    document = _context.DocumentWorkPermits
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 3:
                    document = _context.DocumentVisaDeregistrations
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 4:
                    document = _context.DocumentResidencyPermits
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 5:
                    document = _context.DocumentDocumentManagements
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 6:
                    document = _context.DocumentLocalDocumentations
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 7:
                    document = _context.DocumentCorporateAssistances
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 8:
                    document = _context.DocumentRenewals
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 9:
                    document = _context.DocumentNotifications
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 10:
                    document = _context.DocumentLegalReviews
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 12:
                    document = _context.DocumentPredecisionOrientations
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 13:
                    document = _context.DocumentAreaOrientations
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 14:
                    document = _context.DocumentSettlingIns
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 15:
                    document = _context.DocumentSchoolingSearches
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 16:
                    document = _context.DocumentDepartures
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 17:
                    document = _context.DocumentTemporaryHousingCoordinatons
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 18:
                    document = _context.DocumentRentalFurnitureCoordinations
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 19:
                    document = _context.DocumentTransportations
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 20:
                    document = _context.DocumentAirportTransportationServices
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                case 21:
                    document = _context.DocumentHomeFindings
                        .Where(x => x.Id == id)
                        .Select(c => new documents_library
                        {
                            Id = c.Id,
                            upload_date = c.CreatedDate.Value,
                            ulr_document = c.FileRequest,
                            document_type = _context.CatDocumentTypes.FirstOrDefault(x => x.Id == c.DocumentType).DocumentType,
                            relationship = _context.CatRelationships.FirstOrDefault(x => x.Id == c.Relationship).Relationship,
                            IssueDate = c.IssueDate.Value,
                            ExpirationDate = c.ExpirationDate.Value,
                            IssuingAuthority = c.IssuingAuthority,
                            CountryOrigin = c.CountryOriginNavigation.Name,
                            Comment = c.Comment
                        }).ToList();
                    break;
                default:
                    Console.WriteLine("Default case");
                    
                    break;
            }

            return new ObjectResult(document);
        }

        public ActionResult GetDependentImmigration(int sr)
        {
            var query = _context.DependentImmigrationInfos
                .Where(x => x.ImmigrationProfile.ServiceRecordId == sr)
                 .Select(n => new
                 {
                     n.Id,
                     n.RelationshipId,
                     dependent = _context.CatRelationships.FirstOrDefault(d => d.Id == n.RelationshipId).Relationship + " / " + n.Name
                 }).OrderBy(x => x.dependent).ToList();

            return new ObjectResult(query);
        }

        public class documents_library
        {
            public int Id { get; set; }
            public DateTime upload_date { get; set; }
            public string ulr_document { get; set; }
            public string document_type { get; set; }
            public string relationship { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public string IssuingAuthority { get; set; }
            public string CountryOrigin { get; set; }
            public string Comment { get; set; }
        }

        //APP
        public ActionResult SelectDocumentDependent(int assigneeId)
        {
            var query = _context.Set<biz.premier.Entities.ImmigrationProfile>().Where(s => s.AssigmentInformationId == assigneeId)
                .Include(i => i.LenguageProficiencies)
                .Include(i => i.EducationalBackgrounds)
                .Include(i => i.DependentImmigrationInfos)
                    .ThenInclude(i => i.DependentInformationNavigation)
                        .ThenInclude(i => i.DocumentDependentInformations)
                .Include(i => i.AssigmentInformation)
                .Include(i => i.PassportInformation)
                .Include(i => i.PreviousHostCountry)
                .FirstOrDefault();
            if (query != null)
            {
                return new ObjectResult(query);
            }
            else
            {
                return new ObjectResult(null);
            }
        }
    }
}
