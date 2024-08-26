using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Conversation
{
    public interface IConversationRepository : IGenericRepository<biz.premier.Entities.Conversation>
    {
        Entities.Message AddMessage(Entities.Message message);
        ActionResult SeeChats(int user);
        ActionResult GetConversation(int conversation, int userID);
        ActionResult GetConversationComplete(int conversation, int userID);
        ActionResult GetUserList(int user, int country);
        ActionResult GetChatNotification(int user);
        ActionResult DeleteMessage(int idMessage);
        bool CheckMessage(int message, int user);
        int User_Reciver(int message);
        ActionResult SeeChatsById(int user, int conversationId);
        ActionResult GetUserListConversation(int conversationId);
        int? GetChatNotificationCount(int user);
        int? GetNotificationCount(int user);
        ActionResult UpdateToSeeNotification(int user);
        ActionResult UpdateToSeeMessage(int user, int conversationId);
        ActionResult GetConversationAppPush(int conversation, int userID);
        ActionResult GetConversationCompleteApp(int conversation, int userID);
        ActionResult SeeChatsApp(int user);
    }
}
