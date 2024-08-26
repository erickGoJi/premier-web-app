using biz.premier.Entities;
using biz.premier.Repository.AreaOrientation;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.AreaOrientation
{
    public class AreaOrientationRepository : GenericRepository<biz.premier.Entities.AreaOrientation>, IAreaOrientationRepository
    {
        public AreaOrientationRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.AreaOrientation GetCustom(int key)
        {
            var query = _context.AreaOrientations
                .Include(i => i.DocumentAreaOrientations)
                .Include(i => i.ReminderAreaOrientations)
                .Include(i => i.SchoolingAreaOrientations)
                    .ThenInclude(i => i.Relationship)
                .Include(i => i.SchoolingAreaOrientations)
                    .ThenInclude(i => i.LanguagesSpokenSchoolingAreaOrientations)
                        .ThenInclude(i => i.LanguagesSpokenNavigation)
                .Include(i => i.SchoolingAreaOrientations)
                    .ThenInclude(i => i.NationalityNavigation)
                .Include(i => i.SchoolingAreaOrientations)
                    .ThenInclude(i => i.SexNavigation)
                .Include(i => i.CommentAreaOrientations)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.SchoolingAreaOrientations)
                    .ThenInclude(i => i.CurrentGradeNavigation)
                .Include(i => i.ExtensionAreaOrientations)
                .Single(s => s.Id == key);
            return query;
        }

        public biz.premier.Entities.AreaOrientation UpdateCustom(biz.premier.Entities.AreaOrientation areaOrientation, int key)
        {
            int initialStatus = 0;
            if (areaOrientation == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.AreaOrientation>()
                .Include(i => i.CommentAreaOrientations)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                initialStatus = exist.StatusId;
                _context.Entry(exist).CurrentValues.SetValues(areaOrientation);
                foreach (var i in areaOrientation.CommentAreaOrientations)
                {
                    var comment = exist.CommentAreaOrientations.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentAreaOrientations.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                
                int[] status = new[] {4, 5};
                if (initialStatus != areaOrientation.StatusId && status.Contains(areaOrientation.StatusId) )
                {
                    var serviceRecord = _context.WorkOrderServices
                        .Where(f => f.Id == areaOrientation.WorkOrderServicesId).Select(s =>
                            s.StandaloneServiceWorkOrders.Any()
                                ? s.StandaloneServiceWorkOrders.FirstOrDefault().WorkOrder.ServiceRecordId
                                : s.BundledServices.FirstOrDefault().BundledServiceOrder.WorkOrder.ServiceRecordId)
                        .FirstOrDefault();
                    var assigneeInformation =
                        _context.AssigneeInformations.First(f => f.ServiceRecordId == serviceRecord);
                    _context.HistoryHostHomes.Add(new HistoryHostHome()
                    {
                        AssigneeInformationId = assigneeInformation.Id,
                        CreatedBy = areaOrientation.UpdateBy,
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
