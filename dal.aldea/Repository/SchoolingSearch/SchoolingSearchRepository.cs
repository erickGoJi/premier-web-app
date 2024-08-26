using biz.premier.Entities;
using biz.premier.Repository.SchoolingSearch;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.SchoolingSearch
{
    public class SchoolingSearchRepository : GenericRepository<biz.premier.Entities.SchoolingSearch>, ISchoolingSearchRepository
    {
        public SchoolingSearchRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.SchoolingSearch GetCustom(int key)
        {
            //int wo_id = set_schoolings_info(key); // setear los schholing list 
            var query = _context.SchoolingSearches
                .Include(s => s.SchoolingInformations)
                    .ThenInclude(s => s.Relationship)
                        .ThenInclude(s => s.AssigneeInformation)
                            .ThenInclude(s => s.DependentInformations)
                                .ThenInclude(s => s.LanguageDependentInformations)
                                    .ThenInclude(s => s.LanguageNavigation)
                .Include(s => s.SchoolingInformations)
                    .ThenInclude(s => s.PaymentSchoolingInformations)
                       .ThenInclude(s => s.CurrencyNavigation)
                .Include(s => s.SchoolingInformations)
                    .ThenInclude(s => s.PaymentSchoolingInformations)
                       .ThenInclude(s => s.ResponsibleNavigation)
                .Include(s => s.SchoolingInformations)
                    .ThenInclude(s => s.PaymentSchoolingInformations)
                       .ThenInclude(s => s.RecurrenceNavigation)
                .Include(s => s.SchoolingInformations)
                    .ThenInclude(s => s.PaymentSchoolingInformations)
                       .ThenInclude(s => s.SchoolExpensesPaymentNavigation)
                .Include(i => i.DocumentSchoolingSearches)
                .Include(i => i.ReminderSchoolingSearches)
                .Include(i => i.SchoolingInformations)
                    .ThenInclude(i => i.NationalityNavigation)
                .Include(i => i.SchoolingInformations)
                    .ThenInclude(i => i.SexNavigation)
                //.Include(i => i.SchoolingInformations)
                //    .ThenInclude(i => i.CurrentGradeNavigation)
                //.Include(i => i.SchoolingInformations)
                //    .ThenInclude(i => i.LanguangeSpokenSchoolingInformations)
                //        .ThenInclude(i => i.LanguageSpokenNavigation)
                //.Include(i => i.SchoolingInformations)
                //    .ThenInclude(i => i.Relationship)
                .Include(i => i.ExtensionSchoolingSearches)
                .Include(i => i.CommentSchoolingSearches)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Single(s => s.Id == key);


            foreach (var r in query.SchoolingInformations)
            {
                //  ICollection<SchoolsList> sch_l = r.Relationship.SchoolsLists.Where(s=> s.WorkOrder == wo_id && s.SchoolingStatus == 7 ).ToList();
                ICollection<SchoolsList> sch_l = r.Relationship.SchoolsLists.Where(s => s.SchoolingStatus == 7).ToList();
                r.Relationship.SchoolsLists = sch_l;
            }


            return query;
        }

        public List<biz.premier.Entities.DependentInformation> GetDependent(int key)
        {
            var query = _context.DependentInformations
                .Where(x => x.AssigneeInformation.ServiceRecordId == key).ToList();
                

            return query;
        }


        public ActionResult GetCustomList(int key)
        {
            int wo_id = set_schoolings_info(key);
            var query = _context.SchoolingInformations
                .Where(x => x.SchoolingSearchId == key)
                .Select(s => new 
                {
                    Relationship = s.Relationship,
                    list = s.Relationship.SchoolsLists.Where(y=> y.WorkOrder == wo_id)
                }).ToList();


            var query3 = _context.SchoolingInformations
                 .Include(i => i.LanguangeSpokenSchoolingInformations)
                        .ThenInclude(i => i.LanguageSpokenNavigation)
                .Where(x => x.SchoolingSearchId == key)
                .Select(s => new
                {
                    name = s.Name,
                    s.Birth,
                    s.Sex,
                    s.Age,
                    s.LanguangeSpokenSchoolingInformations,
                    schoolsLists = s.Relationship.SchoolsLists.Where(y => y.WorkOrder == wo_id),
                   // list = s.Relationship.SchoolsLists.Where(y => y.WorkOrder == wo_id)
                }).ToList();



            return new ObjectResult(query);
        }


        public List<biz.premier.Entities.SchoolingInformation> get_si(int key)
        {
            var query = _context.SchoolingInformations
                    .Include(s => s.Relationship)
                       .ThenInclude(s => s.SchoolsLists)
                           .ThenInclude(s => s.Supplier)
                              .ThenInclude(s => s.SupplierPartnerDetails)
                .Where(s => s.SchoolingSearchId == key).ToList();

            return query;
        }


        public biz.premier.Entities.SchoolingSearch GetSchoolsList(int key)
        {

            var query = _context.SchoolingSearches
                .Include(s => s.SchoolingInformations)
                    .ThenInclude(s => s.Relationship)
                       .ThenInclude(s => s.SchoolsLists)
                           .ThenInclude(s => s.Supplier)
                              .ThenInclude(s => s.SupplierPartnerDetails)
                .Single(s => s.Id == key);

            return query;
        }

        public biz.premier.Entities.SchoolsList GetSchoolsListsingle(int key)
        {

            var query = _context.SchoolsLists.FirstOrDefault(x=> x.Id == x.Id);

            return query;
        }

    public int set_schoolings_info(int id_service)
        {

            var wos_id = _context.SchoolingSearches.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
            int country = 0;
            int service_type = 0;
            int assa_id = 0;
            int wo_id = 0;
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId,
                    wo_id = s.WorkOrderId,
                    sr_id = s.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id
                }).ToList();

            if (standalone.Count > 0)
            {
                country = standalone.FirstOrDefault().country.Value;
                service_type = standalone.FirstOrDefault().type.Value;
                assa_id = standalone.FirstOrDefault().ass_id;
                wo_id = standalone.FirstOrDefault().wo_id == null ? 0 : standalone.FirstOrDefault().wo_id.Value;
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn ,
                    type = s.ServiceId,
                    wo_id = s.BundledServiceOrderId,
                    sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id
                }).ToList();

                if (bundled.Count > 0)
                {
                    country = bundled.FirstOrDefault().country.Value;
                    service_type = bundled.FirstOrDefault().type.Value;
                    assa_id = bundled.FirstOrDefault().ass_id;
                    wo_id = bundled.FirstOrDefault().wo_id == null ? 0 : bundled.FirstOrDefault().wo_id.Value;
                }
            }

            List<DependentInformation> dependientes = new List<DependentInformation>();
            dependientes = _context.DependentInformations.Where(d => d.AssigneeInformationId == assa_id
            && d.RelationshipId == 2).Include(l=> l.LanguageDependentInformations).ToList();

            var school_int = _context.SchoolingInformations.Where(s => s.SchoolingSearchId == id_service)
                .Include(l=> l.LanguangeSpokenSchoolingInformations).ToList();

            var languajes = _context.LanguangeSpokenSchoolingInformations.Where(i => i.SchoolingInformationNavigation.SchoolingSearchId == id_service);

            foreach (LanguangeSpokenSchoolingInformation ls in languajes)
            {
                _context.LanguangeSpokenSchoolingInformations.Remove(ls);
                //  _context.SaveChanges();
            }

            for (int i = 0; i < dependientes.Count; i++)
            {
                List<LanguangeSpokenSchoolingInformation> lang_list = new List<LanguangeSpokenSchoolingInformation>();
                foreach(LanguageDependentInformation l in dependientes[i].LanguageDependentInformations)
                {
                    LanguangeSpokenSchoolingInformation ls = new LanguangeSpokenSchoolingInformation();
                    ls.LanguageSpoken = l.Language;
                    lang_list.Add(ls);
                }

                SchoolingInformation sc = new SchoolingInformation();
                sc.RelationshipId = dependientes[i].Id;
                sc.Name = dependientes[i].Name;
                sc.Birth = dependientes[i].Birth;
                sc.Age = dependientes[i].Age;
                sc.Sex = dependientes[i].Sex;
                sc.Nationality = dependientes[i].NationalityId;
                sc.LanguangeSpokenSchoolingInformations = lang_list;
                sc.Avatar = dependientes[i].Photo == null || dependientes[i].Photo == "" ? "Files/assets/avatar.png" : dependientes[i].Photo; 
                sc.CurrentGrade = dependientes[i].CurrentGrade;
                sc.Comments = dependientes[i].AditionalComments;
                sc.SchoolingSearchId = id_service;
                sc.Active = school_int.FirstOrDefault(x => x.Name == dependientes[i].Name && x.SchoolingSearchId == id_service) == null ? false : school_int.FirstOrDefault(x => x.Name == dependientes[i].Name).Active;
                _context.SchoolingInformations.Add(sc);
                _context.SaveChanges();
            }


            foreach (SchoolingInformation s in school_int)
            {
                _context.SchoolingInformations.Remove(s);
                _context.SaveChanges();
            }

            return wo_id; 
        }
        public biz.premier.Entities.SchoolingSearch UpdateCustom(biz.premier.Entities.SchoolingSearch schoolingSearch)
        {
            if (schoolingSearch == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.SchoolingSearch>()
                .Include(i => i.CommentSchoolingSearches)
                .Single(s => s.Id == schoolingSearch.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(schoolingSearch);
                foreach (var comment in schoolingSearch.CommentSchoolingSearches)
                {
                    var existingComment = exist.CommentSchoolingSearches.Where(p => p.Id == comment.Id).FirstOrDefault();
                    if (existingComment == null)
                    {
                        exist.CommentSchoolingSearches.Add(comment);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(existingComment).CurrentValues.SetValues(comment);
                    }
                }
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId,exist.StatusId);
            return exist;
        }

        public bool deleteSchool(int key)
        {
            var query = _context.SchoolsLists
                .FirstOrDefault(s => s.Id == key);

            _context.Remove(query);
            _context.SaveChanges();

            return true;
        }
        public List<int> UpdateSendSchools(List<int> list)
        {
            foreach (int id_h in list)
            {
                var h = _context.SchoolsLists.FirstOrDefault(i => i.Id == id_h);
                h.SchoolingStatus = 2;
                h.SendSchool = true;
                _context.SchoolsLists.Update(h);
                _context.SaveChanges();
            }

            return list;
        }
    }
}
