using biz.premier.Entities;
using biz.premier.Repository.PredicisionOrientation;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;

namespace dal.premier.Repository.PredicisionOrientation
{
    public class PredicisionOrientationRepository :GenericRepository<biz.premier.Entities.PredecisionOrientation>, IPredicisionOrientationRepository
    {
        public PredicisionOrientationRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.PredecisionOrientation GetCustom(int key)
        {
            set_schoolings(key);
            var query = _context.PredecisionOrientations
                .Include(i => i.DocumentPredecisionOrientations)
                .Include(i => i.ReminderPredecisionOrientations)
                .Include(i => i.CommentPredecisionOrientations)
                    .ThenInclude(t => t.User).ThenInclude(i => i.Role)
                .Include(i => i.Schoolings)
                .Include(i => i.ExtensionPredecisionOrientations) 
                .Single(s => s.Id == key);
            return query;
        }

        public void set_schoolings(int id_service)
        {
            

            var wos_id = _context.PredecisionOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
            int country = 0;
            int service_type = 0;
            int assa_id = 0;
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
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn
                    ,
                    type = s.ServiceId,
                    sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id
                }).ToList();

                if (bundled.Count > 0)
                {
                    country = bundled.FirstOrDefault().country.Value;
                    service_type = bundled.FirstOrDefault().type.Value;
                    assa_id = bundled.FirstOrDefault().ass_id;
                }
            }
            
            List<DependentInformation> dependientes = new List<DependentInformation>();
            dependientes = _context.DependentInformations.Where(d => d.AssigneeInformationId == assa_id
            && d.RelationshipId == 2).ToList();

            var school_int = _context.Schoolings.Where(s => s.PredecisionOrientationId == id_service).ToList();
            
            for (int i=0; i< dependientes.Count; i++)
            {
     
                Schooling sc = new Schooling();
                sc.RelationshipId = 1878;
                sc.Name = dependientes[i].Name;
                sc.Birth = dependientes[i].Birth;
                sc.Age = dependientes[i].Age;
                sc.Sex = dependientes[i].Sex;
                //sc.Nationality = dependientes[i].NationalityId; 
                sc.CurrentGrade = dependientes[i].CurrentGrade;
                sc.Comments = dependientes[i].AditionalComments;
                sc.PredecisionOrientationId = id_service;
                sc.Active = school_int.FirstOrDefault(x => x.Name == dependientes[i].Name) == null ? false : school_int.FirstOrDefault(x => x.Name == dependientes[i].Name).Active;
               // _context.Schoolings.Add(sc);
               // _context.SaveChanges();
            }

            foreach (Schooling s in school_int)
            {
               // _context.Schoolings.Remove(s);
              //  _context.SaveChanges();
            }
        }


        public PredecisionOrientation UpdateCustom(PredecisionOrientation predecisionOrientation, int key)
        {
            int initialStatus = 0;
            if (predecisionOrientation == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.PredecisionOrientation>()
                .Include(i => i.CommentPredecisionOrientations)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                initialStatus = exist.StatusId;
                _context.Entry(exist).CurrentValues.SetValues(predecisionOrientation);
                foreach (var i in predecisionOrientation.CommentPredecisionOrientations)
                {
                    var comment = exist.CommentPredecisionOrientations.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentPredecisionOrientations.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }

                int[] status = new[] {4, 5};
                if (initialStatus != predecisionOrientation.StatusId && status.Contains(predecisionOrientation.StatusId) )
                {
                    var serviceRecord = _context.WorkOrderServices
                        .Where(f => f.Id == predecisionOrientation.WorkOrderServicesId).Select(s =>
                            s.StandaloneServiceWorkOrders.Any()
                                ? s.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId
                                : s.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId)
                        .FirstOrDefault();
                    var assigneeInformation =
                        _context.AssigneeInformations.First(f => f.ServiceRecordId == serviceRecord);
                    _context.HistoryHostHomes.Add(new HistoryHostHome()
                    {
                        AssigneeInformationId = assigneeInformation.Id,
                        CreatedBy = predecisionOrientation.UpdateBy,
                        CreatedDate = DateTime.Now,
                        CityHomeId = assigneeInformation.HomeCityId.Value,
                        CityHostId = assigneeInformation.HostCityId.Value,
                        CountryHomeId = assigneeInformation.HomeCountryId.Value,
                        CountryHostId = assigneeInformation.HostCountry.Value
                    });
                    int? homeCityId = assigneeInformation.HostCityId;
                    int? hostCityId = assigneeInformation.HomeCityId;
                    int? hostCountry = assigneeInformation.HomeCountryId;
                    int? homeCountryId = assigneeInformation.HostCountry;
                    
                    assigneeInformation.HomeCityId = homeCityId;
                    assigneeInformation.HostCityId = hostCityId;
                    assigneeInformation.HostCountry = hostCountry;
                    assigneeInformation.HomeCountryId = homeCountryId;
                    _context.AssigneeInformations.Update(assigneeInformation);
                }
                
                _context.SaveChanges();
            }
            UpdateStatusServiceRecord(exist.WorkOrderServicesId,exist.StatusId);
            return exist;
        }
    }
}
