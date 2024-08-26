using dal.premier.DBContext;
using biz.premier.Entities;
using biz.premier.Repository.Chat_Immigration_Relocation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace dal.premier.Repository.Chat_Immigration_Relocation
{
    public class ChatImmigrationRelocationRepository : GenericRepository<ChatConversationImmigrationRelocation>, IChatImmigrationRelocationRepository
    {
        public ChatImmigrationRelocationRepository(Db_PremierContext context) : base(context) { }

        public ActionResult GetConversation(int service_record_id, int service_line_id, int? user)
        {
            var service = _context.ChatImmigrationRelocations
                .Where(e => e.ChatCoversation.ServiceRecordId == service_record_id && e.ChatCoversation.ServiceLineId == service_line_id)
                .Select(k => new
                {
                    k.Id,
                    userId = k.User.Id,
                    conversationId = k.ChatCoversation.Id,
                    k.ChatCoversation.CreatedDate,
                    k.ChatCoversation.ServiceLineId,
                    user = k.User.Name,
                    k.DateComment,
                    k.Comment,
                    Avatar = k.User.Avatar == null || k.User.Avatar == "" ? "Files/assets/avatar.png" : k.User.Avatar,
                    file = k.ChatDocumentImmigrationRelocations                    
                }).OrderBy(x => x.DateComment).ToList();

            foreach (var m in service.ToList())
            {
                if (m.userId == user) { continue; }
                var message = _context.ChatImmigrationRelocations.SingleOrDefault(x => x.Id == m.Id);
                message.Status = true;
                _context.ChatImmigrationRelocations.Update(message);
            }
            _context.SaveChanges();
            
            return new ObjectResult(service);
        }

        public ActionResult GetConversationsByUser(int user)
        {
            var profilesImmigrationsCoodinators = _context.ImmigrationCoodinators.Where(x => x.Coordinator.UserId == user)
                .Select(s => s.ServiceRecordId).ToList();
            var profilesRelocationsCoodinators = _context.RelocationCoordinators.Where(x => x.Coordinator.UserId == user)
                .Select(s => s.ServiceRecordId).ToList();
            var profilesImmigrations = _context.ImmigrationSupplierPartners.Where(x => x.Supplier.UserId == user)
                .Select(s => s.ServiceRecordId).ToList();
            var profilesRelocation = _context.RelocationSupplierPartners.Where(x => x.Supplier.UserId == user)
                .Select(s => s.ServiceRecordId.Value).ToList();
            var serviceRecords = profilesImmigrationsCoodinators.Union(profilesRelocationsCoodinators).ToList()
                .Union(profilesImmigrations).ToList().Union(profilesRelocation).ToList();
            var conversations = _context.ChatConversationImmigrationRelocations
                .Where(x => serviceRecords.Contains(x.ServiceRecordId.Value)).Select(s => new
                {
                    s.Id,
                    team = $"{s.ServiceLine.ServiceLine} Team",
                    s.ServiceLineId,
                    s.ServiceRecordId,
                    s.ServiceRecord.NumberServiceRecord,
                    name = s.ChatImmigrationRelocations.OrderByDescending(o=>o.Id)
                        .Select(l => new
                            {name = $"{l.User.Name} {l.User.LastName} {l.User.MotherLastName}"})
                        .FirstOrDefault(),
                    avatar = s.ChatImmigrationRelocations
                        .OrderByDescending(o => o.Id).FirstOrDefault().User.Avatar,
                    s.ChatImmigrationRelocations.OrderByDescending(o => o.Id).FirstOrDefault().Comment,
                    s.ChatImmigrationRelocations.OrderByDescending(o => o.Id).FirstOrDefault().DateComment,
                    country = s.ServiceRecord.AssigneeInformations.FirstOrDefault().HostCountryNavigation.Name,
                }).ToList();
            return new ObjectResult(conversations);
        }

        public ActionResult GetUsersTeam(int conversation)
        {
            var serviceRecord = _context.ChatConversationImmigrationRelocations.Where(x => x.Id == conversation)
                .Select(s=> new
                {
                    s.ServiceRecordId,
                    s.ServiceLineId
                }).FirstOrDefault();
            var profilesCoordinators = serviceRecord.ServiceLineId == 1 ? _context.ImmigrationCoodinators.Where(x => serviceRecord.ServiceRecordId == x.ServiceRecordId)
                .Select(s => new
                {
                    s.Coordinator.Id,
                    name = $"{s.Coordinator.Name} {s.Coordinator.LastName} {s.Coordinator.MotherLastName}",
                    s.Coordinator.Email,
                    s.Coordinator.Photo,
                    s.Coordinator.PhoneNumber,
                    type = "Coordinator"
                } ).ToList() : _context.RelocationCoordinators.Where(x => serviceRecord.ServiceRecordId == x.ServiceRecordId)
                .Select(s => new
                {
                    s.Coordinator.Id,
                    name = $"{s.Coordinator.Name} {s.Coordinator.LastName} {s.Coordinator.MotherLastName}",
                    s.Coordinator.Email,
                    s.Coordinator.Photo,
                    s.Coordinator.PhoneNumber,
                    type = "Coordinator"
                } ).ToList();
            var profilesSupplierPartners = serviceRecord.ServiceLineId == 1 ? _context.ImmigrationSupplierPartners.Where(x => x.ServiceRecordId == serviceRecord.ServiceRecordId)
                .Select(s => new
                {
                    s.Supplier.Id,
                    name = $"{s.Supplier.Name} {s.Supplier.LastName} {s.Supplier.MotherLastName}",
                    s.Supplier.Email,
                    s.Supplier.Photo,
                    s.Supplier.PhoneNumber,
                    type = "Consultant"
                }).ToList() : _context.RelocationSupplierPartners.Where(x => x.ServiceRecordId == serviceRecord.ServiceRecordId)
                .Select(s => new
                {
                    s.Supplier.Id,
                    name = $"{s.Supplier.Name} {s.Supplier.LastName} {s.Supplier.MotherLastName}",
                    s.Supplier.Email,
                    s.Supplier.Photo,
                    s.Supplier.PhoneNumber,
                    type = "Consultant"
                }).ToList();
            var team = profilesCoordinators.Union(profilesSupplierPartners).ToList();
            return new ObjectResult(team);
        }
    }

}
