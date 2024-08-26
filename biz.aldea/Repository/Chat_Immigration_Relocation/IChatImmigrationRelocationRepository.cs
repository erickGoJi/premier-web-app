using biz.premier.Entities;
using Microsoft.AspNetCore.Mvc;

namespace biz.premier.Repository.Chat_Immigration_Relocation
{
    public interface IChatImmigrationRelocationRepository : IGenericRepository<ChatConversationImmigrationRelocation>
    {
        ActionResult GetConversation(int service_record_id, int service_line_id, int? user);
        ActionResult GetConversationsByUser(int user);
        ActionResult GetUsersTeam(int conversation);
    }
}
