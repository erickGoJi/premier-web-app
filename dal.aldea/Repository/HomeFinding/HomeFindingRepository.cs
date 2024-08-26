using biz.premier.Entities;
using biz.premier.Repository.HomeFinding;
using biz.premier.Repository.NotificationSystem;
using biz.premier.Repository.NotificationSystemType;
using biz.premier.Repository.Utility;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace dal.premier.Repository.HomeFinding
{
    public class HomeFindingRepository : GenericRepository<biz.premier.Entities.HomeFinding>, IHomeFindingRepository
    {
        private readonly IUtiltyRepository _utiltyRepository;
        private readonly INotificationSystemRepository _notificationSystemRepository;
        private readonly ICatNotificationSystemTypeRepository _notificationSystemTypeRepository;
        public HomeFindingRepository(Db_PremierContext context, IUtiltyRepository utiltyRepository,
            INotificationSystemRepository notificationSystemRepository, ICatNotificationSystemTypeRepository notificationSystemTypeRepository) : base(context) {

            _utiltyRepository = utiltyRepository;
            _notificationSystemRepository = notificationSystemRepository;
            _notificationSystemTypeRepository = notificationSystemTypeRepository;
        }

        public bool DeleteDocument(int key)
        {
            if (key == 0)
                return false;
            var query = _context.DocumentHomeFindings
                .Where(x => x.Id == key)
                .FirstOrDefault();
            if (query == null)
                return false;
            _context.DocumentHomeFindings.Remove(query);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteDocumentHousing(int key)
        {
            if (key == 0)
                return false;
            var query = _context.DocumentHousings
                .Where(x => x.Id == key)
                .FirstOrDefault();
            if (query == null)
                return false;
            _context.DocumentHousings.Remove(query);
            _context.SaveChanges();
            return true;
        }



        public bool DeleteReminder(int key)
        {
            if (key == 0)
                return false;
            var query = _context.ReminderHomeFindings
                .Where(x => x.Id == key)
                .FirstOrDefault();
            if (query == null)
                return false;
            _context.ReminderHomeFindings.Remove(query);
            _context.SaveChanges();
            return true;
        }

        public DocumentHomeFinding FindDocument(int key)
        {
            if (key == 0)
                return null;
            var query = _context.DocumentHomeFindings
                .Where(x => x.Id == key)
                .FirstOrDefault();
            if (query == null)
                return null;
            return query;
        }

        public biz.premier.Entities.StandaloneServiceWorkOrder GetAloneServiceWoId(int id_wo_service)
        {
            if (id_wo_service == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.StandaloneServiceWorkOrder>().FirstOrDefault(s => s.WorkOrderServiceId == id_wo_service);

            return exist;
        }


        public biz.premier.Entities.BundledServicesWorkOrder GetBundleServiceWoId(int id_wo_service)
        {
            if (id_wo_service == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.BundledService>().FirstOrDefault(s => s.WorkServicesId == id_wo_service);
            var b_wo = _context.Set<biz.premier.Entities.BundledServicesWorkOrder>().FirstOrDefault(s => s.Id == (exist == null ? 0 : exist.BundledServiceOrderId));


            return b_wo;
        }

        public biz.premier.Entities.HomeFinding GetCustom(int key)
        {
            if (key == 0)
                return null;
            var exist = _context.Set<biz.premier.Entities.HomeFinding>()
                .Include(i => i.DocumentHomeFindings)
                .Include(i => i.ReminderHomeFindings)
                .Include(i => i.CommentHomeFindings)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.CommentHomeFindings)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.ExtensionHomeFindings)
                .Single(s => s.Id == key);
            return exist;
        }

        public biz.premier.Entities.HomeFinding UpdateCustom(biz.premier.Entities.HomeFinding homeFinding, int key, int userId, string number_server)
        {

           // var x = _utiltyRepository.atributos_generales(1);

            int initialStatus = 0;
            if (homeFinding == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.HomeFinding>()
                .Include(i => i.DocumentHomeFindings)
                .Include(i => i.ReminderHomeFindings)
                .Include(i => i.CommentHomeFindings)
                .Include(i => i.ExtensionHomeFindings)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                initialStatus = exist.StatusId.Value;
                _context.Entry(exist).CurrentValues.SetValues(homeFinding);
                foreach (var i in homeFinding.DocumentHomeFindings)
                {
                    var document = exist.DocumentHomeFindings.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (document == null)
                    {
                        exist.DocumentHomeFindings.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(i);
                    }
                }

                foreach (var i in homeFinding.ReminderHomeFindings)
                {
                    var reminder = exist.ReminderHomeFindings.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (reminder == null)
                    {
                        exist.ReminderHomeFindings.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(reminder).CurrentValues.SetValues(i);
                    }
                }

                foreach (var i in homeFinding.CommentHomeFindings)
                {
                    var comment = exist.CommentHomeFindings.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentHomeFindings.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }

                foreach (var i in homeFinding.ExtensionHomeFindings)
                {
                    var extension = exist.ExtensionHomeFindings.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (extension == null)
                    {
                        exist.ExtensionHomeFindings.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(extension).CurrentValues.SetValues(i);
                    }
                }
                
                int[] status = new[] {26, 34,35,36,37};
                if (initialStatus != homeFinding.StatusId && status.Contains(homeFinding.StatusId.Value) )
                {

                    var coordinator = _notificationSystemRepository.GetCoordinatorbyWosId(homeFinding.WorkOrderServicesId);
                    var estatus = _context.CatStatusWorkOrders.FirstOrDefault(e => e.Id == homeFinding.StatusId).Status;

                    if (coordinator.Item2 > 0)
                    {
                        _notificationSystemRepository.Add(new biz.premier.Entities.NotificationSystem()
                        {
                            Archive = false,
                            View = false,
                            ServiceRecord = coordinator.Item1,
                            Time = DateTime.Now.TimeOfDay,
                            UserFrom = userId,
                            UserTo = coordinator.Item2,
                            NotificationType = 31,
                            Description = $"{_notificationSystemTypeRepository.Find(x => x.Id == 31).Type} | " + estatus + " |  Home Finding | " + number_server,
                            Color = "#f06689",
                            CreatedDate = DateTime.Now
                        });


                        _notificationSystemRepository.SendNotificationAsync(
                            coordinator.Item2,
                            0,
                            0,
                            _notificationSystemTypeRepository.Find(x => x.Id == 31).Type + " | " + estatus + " |  Home Finding | " + number_server,
                            "",
                            2
                        );
                    }

                    //    var serviceRecord = _context.WorkOrderServices
                    //        .Where(f => f.Id == homeFinding.WorkOrderServicesId).Select(s =>
                    //            s.StandaloneServiceWorkOrders.Any()
                    //                ? s.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId
                    //                : s.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId)
                    //        .FirstOrDefault();
                    //    var assigneeInformation =
                    //        _context.AssigneeInformations.First(f => f.ServiceRecordId == serviceRecord);
                    //    _context.HistoryHostHomes.Add(new HistoryHostHome()
                    //    {
                    //        AssigneeInformationId = assigneeInformation.Id,
                    //        CreatedBy = homeFinding.UpdateBy,
                    //        CreatedDate = DateTime.Now,
                    //        CityHomeId = assigneeInformation.HomeCityId.Value,
                    //        CityHostId = assigneeInformation.HostCityId.Value,
                    //        CountryHomeId = assigneeInformation.HomeCountryId.Value,
                    //        CountryHostId = assigneeInformation.HostCountry.Value
                    //    });
                    //    int? homeCityId = assigneeInformation.HostCityId;
                    //    int? hostCityId = assigneeInformation.HomeCityId;
                    //    int? hostCountry = assigneeInformation.HomeCountryId;
                    //    int? homeCountryId = assigneeInformation.HostCountry;

                    //    assigneeInformation.HomeCityId = homeCityId;
                    //    assigneeInformation.HostCityId = hostCityId;
                    //    assigneeInformation.HostCountry = hostCountry;
                    //    assigneeInformation.HomeCountryId = homeCountryId;
                    //    _context.AssigneeInformations.Update(assigneeInformation);
                }

                homeFinding.UpdatedDate = DateTime.Now;
                _context.SaveChanges();

            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId, exist.StatusId.Value);
            return exist;
        }
    }
}
