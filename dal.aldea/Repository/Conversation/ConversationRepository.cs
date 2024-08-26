using biz.premier.Entities;
using biz.premier.Repository.Conversation;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace dal.premier.Repository.Conversation
{
    public class ConversationRepository : GenericRepository<biz.premier.Entities.Conversation>, IConversationRepository
    {
        public ConversationRepository(Db_PremierContext context) : base(context) { }

        public Message AddMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
            return message;
        }

        public ActionResult SeeChats(int user)
        {
            if (user == 0)
                return null;
            var messages = _context.Conversations
                .Include(i => i.UserReciverNavigation).ThenInclude(i => i.ProfileUsers)
                .Include(i => i.UserGroups).ThenInclude(i => i.UserReciverNavigation).ThenInclude(i => i.ProfileUsers)
                .Where(x => x.UserTo == user || x.UserReciver == user || x.UserGroups.Select(s => s.UserReciver).Contains(user))
                .Select(s => new
                {
                    iduser = s.Groups.Value 
                        ? (user != s.UserTo ? s.UserTo : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciver ) 
                        : (user != s.UserTo ? s.UserTo : s.UserReciver),
                    name = s.Groups.Value 
                        ? (user != s.UserTo ? s.UserToNavigation.Name : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Name) 
                        : (user != s.UserTo ? s.UserToNavigation.Name : s.UserReciverNavigation.Name),
                    lastname = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.LastName : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.LastName)
                        : (user != s.UserTo ? s.UserToNavigation.LastName : s.UserReciverNavigation.LastName),
                    photo = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Avatar : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Avatar)
                        : (user != s.UserTo ? s.UserToNavigation.Avatar : s.UserReciverNavigation.Avatar),
                    profile = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Role.Role : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Role.Role)
                        : (user != s.UserTo ? s.UserToNavigation.Role.Role : s.UserReciverNavigation.Role.Role),
                    conversationId = s.Id,
                    s.UserReciverNavigation.ProfileUsers.FirstOrDefault().PhoneNumber,
                    s.UserReciver,
                    lastMessage = s.Messages.OrderByDescending(r => r.Id).FirstOrDefault(x => x.Message1 != "").Message1,
                    lastMessageTime = s.Messages.OrderByDescending(r => r.Id).FirstOrDefault().Time,
                    unreadMessages = s.Messages.Where(x => x.Message1 != "").Count(r => r.Status == false && r.UserId != user),
                    s.GroupName,
                    s.Groups,
                    Names = s.Groups.Value 
                        ? s.UserGroups.Select(group => new
                        {
                            profile = group.UserReciverNavigation.ProfileUsers.FirstOrDefault().Id,
                            avatar = group.UserReciverNavigation.Avatar,
                            participant = $"{group.UserReciverNavigation.Name} {group.UserReciverNavigation.LastName} {group.UserReciverNavigation.MotherLastName}"
                        }).ToList() 
                        : null
                }).ToList();


            return new ObjectResult(messages.OrderByDescending(x => x.lastMessageTime));
        }

        public ActionResult SeeChatsApp(int user)
        {
            if (user == 0)
                return null;
            var messages = _context.Conversations
                .Include(i => i.UserReciverNavigation).ThenInclude(i => i.ProfileUsers)
                .Include(i => i.UserGroups).ThenInclude(i => i.UserReciverNavigation).ThenInclude(i => i.ProfileUsers)
                .Where(x => x.UserTo == user || x.UserReciver == user || x.UserGroups.Select(s => s.UserReciver).Contains(user))
                .Select(s => new
                {
                    iduser = s.Groups.Value
                        ? (user != s.UserTo ? s.UserTo : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciver)
                        : (user != s.UserTo ? s.UserTo : s.UserReciver),
                    name = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Name : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Name)
                        : (user != s.UserTo ? s.UserToNavigation.Name : s.UserReciverNavigation.Name),
                    lastname = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.LastName : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.LastName)
                        : (user != s.UserTo ? s.UserToNavigation.LastName : s.UserReciverNavigation.LastName),
                    photo = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Avatar : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Avatar)
                        : (user != s.UserTo ? s.UserToNavigation.Avatar : s.UserReciverNavigation.Avatar),
                    profile = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Role.Role : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Role.Role)
                        : (user != s.UserTo ? s.UserToNavigation.Role.Role : s.UserReciverNavigation.Role.Role),
                    conversationId = s.Id,
                    s.UserReciverNavigation.ProfileUsers.FirstOrDefault().PhoneNumber,
                    s.UserReciver,
                    lastMessage = s.Messages.OrderByDescending(r => r.Id).FirstOrDefault(x => x.Message1 != "").Message1,
                    lastMessageTime = s.Messages.OrderByDescending(r => r.Id).FirstOrDefault().Time,
                    unreadMessages = s.Messages.Where(x => x.Message1 != "").Count(r => r.Status == false && r.UserId != user),
                    s.GroupName,
                    s.Groups,
                    Names = s.Groups.Value
                        ? s.UserGroups.Select(group => new
                        {
                            profile = group.UserReciverNavigation.ProfileUsers.FirstOrDefault().Id,
                            avatar = group.UserReciverNavigation.Avatar,
                            participant = $"{group.UserReciverNavigation.Name} {group.UserReciverNavigation.LastName} {group.UserReciverNavigation.MotherLastName}"
                        }).ToList()
                        : null
                }).ToList();

           
            return new ObjectResult(messages.Where(x => x.unreadMessages > 0).OrderByDescending(x => x.lastMessageTime));
        }

        public ActionResult SeeChatsById(int user, int conversationId)
        {
            if (user == 0)
                return null;
            var messages = _context.Conversations
                .Include(i => i.UserReciverNavigation).ThenInclude(i => i.ProfileUsers)
                .Include(i => i.UserGroups).ThenInclude(i => i.UserReciverNavigation).ThenInclude(i => i.ProfileUsers)
                .Where(x => x.Id == conversationId)
                .Select(s => new
                {
                    iduser = s.Groups.Value
                        ? (user != s.UserTo ? s.UserTo : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciver)
                        : (user != s.UserTo ? s.UserTo : s.UserReciver),
                    name = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Name : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Name)
                        : (user != s.UserTo ? s.UserToNavigation.Name : s.UserReciverNavigation.Name),
                    lastname = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.LastName : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.LastName)
                        : (user != s.UserTo ? s.UserToNavigation.LastName : s.UserReciverNavigation.LastName),
                    photo = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Avatar : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Avatar)
                        : (user != s.UserTo ? s.UserToNavigation.Avatar : s.UserReciverNavigation.Avatar),
                    profile = s.Groups.Value
                        ? (user != s.UserTo ? s.UserToNavigation.Role.Role : s.UserGroups.FirstOrDefault(x => x.UserReciver != user).UserReciverNavigation.Role.Role)
                        : (user != s.UserTo ? s.UserToNavigation.Role.Role : s.UserReciverNavigation.Role.Role),
                    conversationId = s.Id,
                    s.UserReciverNavigation.ProfileUsers.FirstOrDefault().PhoneNumber,
                    s.UserReciver,
                    lastMessage = s.Messages.OrderByDescending(r => r.Id).FirstOrDefault().Message1,
                    lastMessageTime = s.Messages.OrderByDescending(r => r.Id).FirstOrDefault().Time,
                    unreadMessages = s.Messages.Count(r => r.Status == false && r.UserId != user),
                    s.GroupName,
                    s.Groups,
                    Names = s.Groups.Value
                        ? s.UserGroups.Select(group => new
                        {
                            profile = group.UserReciverNavigation.ProfileUsers.FirstOrDefault().Id,
                            avatar = group.UserReciverNavigation.Avatar,
                            participant = $"{group.UserReciverNavigation.Name} {group.UserReciverNavigation.LastName} {group.UserReciverNavigation.MotherLastName}"
                        }).ToList()
                        : null
                }).ToList();
            return new ObjectResult(messages.OrderByDescending(x => x.lastMessageTime));
        }

        //public ActionResult UpdateReadingMessage(int messageId)
        //{
        //    var message = _context.Messages.SingleOrDefault(x => x.Id == messageId);
        //    message.Status = true;
        //    _context.Messages.Update(message);

        //    _context.SaveChanges();
        //}
        public ActionResult GetConversation(int conversation, int userID)
        {
            var service = _context.Messages
                .Where(e => e.Conversation == conversation && e.Message1 != "")
                .OrderByDescending(x => x.Time).Take(20)
                .Select(k => new
                {
                    k.Id,
                    //k.ConversationNavigation.GroupName,
                    userId = k.User.Id,
                    //conversationId = k.ConversationNavigation.Id,
                    //k.ConversationNavigation.CreatedDate,
                    //user = k.User.Name,
                    k.Time,
                    k.Message1,
                    //Avatar = k.User.Avatar == null || k.User.Avatar == "" ? "Files/assets/avatar.png" : k.User.Avatar,
                    k.DocumentMessages,
                    k.Status,
                    unreadMessages = k.Status == false ? 0 : 1,
                    //profile = k.User.ProfileUsers.Select(s => new
                    //{
                    //    s.Id,
                    //    k.User.Role.Role
                    //}).FirstOrDefault()
                }).ToList();

            //foreach (var m in service)
            //{
            //    if (m.userId != userID)
            //    {
            //        var message = _context.Messages.SingleOrDefault(x => x.Id == m.Id);
            //        message.Status = true;
            //        _context.Messages.Update(message);
            //    }
            //}
            //_context.SaveChanges();

            return new ObjectResult(service.OrderBy(x => x.Time));
        }

        public ActionResult GetConversationAppPush(int conversation, int userID)
        {
            var service = _context.Messages
                .Where(e => e.Conversation == conversation && e.UserId != userID && e.Message1 != "" && e.Status == false)
                .OrderByDescending(x => x.Time).Take(20)
                .Select(k => new
                {
                    k.Id,
                    //k.ConversationNavigation.GroupName,
                    userId = k.User.Id,
                    //conversationId = k.ConversationNavigation.Id,
                    //k.ConversationNavigation.CreatedDate,
                    //user = k.User.Name,
                    k.Time,
                    k.Message1,
                    //Avatar = k.User.Avatar == null || k.User.Avatar == "" ? "Files/assets/avatar.png" : k.User.Avatar,
                    k.DocumentMessages,
                    k.Status,
                    unreadMessages = k.Status == false ? 0 : 1,
                    //profile = k.User.ProfileUsers.Select(s => new
                    //{
                    //    s.Id,
                    //    k.User.Role.Role
                    //}).FirstOrDefault()
                }).ToList();

            //foreach (var m in service)
            //{
            //    if (m.userId != userID)
            //    {
            //        var message = _context.Messages.SingleOrDefault(x => x.Id == m.Id);
            //        message.Status = true;
            //        _context.Messages.Update(message);
            //    }
            //}
            //_context.SaveChanges();

            return new ObjectResult(service.OrderBy(x => x.Time));
        }

        public ActionResult GetConversationComplete(int conversation, int userID)
        {
            var service = _context.Messages
                .Where(e => e.Conversation == conversation && e.Message1 != "")
                .Select(k => new
                {
                    k.Id,
                    //k.ConversationNavigation.GroupName,
                    userId = k.User.Id,
                    //conversationId = k.ConversationNavigation.Id,
                    //k.ConversationNavigation.CreatedDate,
                    //user = k.User.Name,
                    k.Time,
                    k.Message1,
                    //Avatar = k.User.Avatar == null || k.User.Avatar == "" ? "Files/assets/avatar.png" : k.User.Avatar,
                    k.DocumentMessages,
                    k.Status,
                    unreadMessages = k.Status == false ? 0 : 1,
                    //profile = k.User.ProfileUsers.Select(s => new
                    //{
                    //    s.Id,
                    //    k.User.Role.Role
                    //}).FirstOrDefault()
                }).OrderBy(x => x.Time).ToList();

            return new ObjectResult(service);
        }

        public ActionResult GetConversationCompleteApp(int conversation, int userID)
        {
            var service = _context.Messages
                .Where(e => e.Conversation == conversation && e.Message1 != "")
                .Select(k => new
                {
                    k.Id,
                    //k.ConversationNavigation.GroupName,
                    userId = k.User.Id,
                    //conversationId = k.ConversationNavigation.Id,
                    //k.ConversationNavigation.CreatedDate,
                    //user = k.User.Name,
                    k.Time,
                    k.Message1,
                    //Avatar = k.User.Avatar == null || k.User.Avatar == "" ? "Files/assets/avatar.png" : k.User.Avatar,
                    k.DocumentMessages,
                    k.Status,
                    unreadMessages = k.Status == false ? 0 : 1,
                    //profile = k.User.ProfileUsers.Select(s => new
                    //{
                    //    s.Id,
                    //    k.User.Role.Role
                    //}).FirstOrDefault()
                }).OrderBy(x => x.Time).ToList();

            service = service.Take(service.Count() - 20).ToList();
            return new ObjectResult(service);
        }

        public ActionResult DeleteMessage(int idMessage)
        {
            var message = _context.Messages.FirstOrDefault(x => x.Id == idMessage);

            _context.Messages.Remove(message);
            _context.SaveChanges();

            return new ObjectResult(message);
        }

        public bool CheckMessage(int message, int user)
        {
            bool isSuccess = false;
            var m = _context.Messages.FirstOrDefault(f => f.Id == message);
            if (m!=null)
            {
                if(user != 0)
                {
                    if (m.UserId != user)
                    {
                        m.Status = true;
                        _context.Messages.Update(m);
                        _context.SaveChanges();
                        isSuccess = true;
                    }
                }
                else
                {
                    m.Status = true;
                    _context.Messages.Update(m);
                    _context.SaveChanges();
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public int User_Reciver(int message)
        {
            var m = _context.Messages.FirstOrDefault(f => f.Id == message);

            return m.ConversationNavigation.UserReciver.Value;
        }

        public ActionResult GetUserList(int user, int country)
        {
            var users = _context.Users.Where(x => x.ProfileUsers.Select(s => s.CityNavigation.IdCountry).Contains(country))
                .Select(s => new
                {
                    s.Id,
                    user = s.ProfileUsers.FirstOrDefault().TitleNavigation.Title + "/" + s.Name,
                    userName  = s.Name,
                    title = s.ProfileUsers.FirstOrDefault().User.Role.Role,
                    s.UserTypeId,
                    Avatar = s.Avatar == null || s.Avatar == "" ? "Files/assets/avatar.png" : s.Avatar,
                }).Where(x => x.UserTypeId == 1 || x.UserTypeId == 2 || x.UserTypeId == 3 || x.UserTypeId == 4).ToList();
            return new ObjectResult(users.Where(x => x.Id != user));
        }

        public ActionResult GetUserListConversation(int conversationId)
        {
            var users = _context.Conversations
                .Include(x => x.UserGroups)
                .ThenInclude(x => x.UserReciverNavigation)
                .Where(x => x.Id == conversationId)
                .Select(s => new
                {
                    s.Id,
                    userGroup = s.UserGroups.Select(x => new
                    {
                        Avatar = x.UserReciverNavigation.Avatar == null || x.UserReciverNavigation.Avatar == "" ? "Files/assets/585e4d1ccb11b227491c339b.png" : x.UserReciverNavigation.Avatar,
                        userName = x.UserReciverNavigation.Name,
                        title = x.UserReciverNavigation.ProfileUsers.FirstOrDefault().User.Role.Role
                    }).ToList()
                                        
                }).ToList();
            return new ObjectResult(users);
        }

        public ActionResult GetChatNotification(int user)
        {
            if (user == 0)
                return null;
            var messages = _context.Messages
                .Where(x => x.ConversationNavigation.UserTo == user || x.ConversationNavigation.UserReciver == user 
                || x.ConversationNavigation.UserGroups.Select(u => u.UserReciver).Contains(user))
                .Select(s => new
                {
                    iduser = s.UserId,
                    name = s.User.Name,
                    lastname = s.User.LastName,
                    photo = s.User.Avatar,
                    conversationId = s.Conversation,
                    lastMessage = s.Message1,
                    lastMessageTime = s.Time,
                    s.ConversationNavigation.Groups,
                    s.Status
                }).Where(x => user != x.iduser && x.Status == false && x.lastMessage != "").Take(10).ToList();
            return new ObjectResult(messages);
        }

        public int? GetChatNotificationCount(int user)
        {
            if (user == 0)
                return null;
            var messages = _context.Messages
                .Where(x => (x.ConversationNavigation.UserTo == user || x.ConversationNavigation.UserReciver == user
                || x.ConversationNavigation.UserGroups.Select(u => u.UserReciver).Contains(user)) && x.Status == false && x.Message1 != "")
                .Count();

            return messages;
        }

        public ActionResult UpdateToSeeNotification(int user)
        {
            if (user == 0)
                return null;

            var notificacion = _context.NotificationSystems.Where(x => x.UserTo == user && x.View == false).ToList();

            foreach(var noti in notificacion)
            {
                noti.View = true;
                _context.NotificationSystems.UpdateRange(noti);
                _context.SaveChanges();
            }
            

            return new ObjectResult(notificacion);
        }

        public ActionResult UpdateToSeeMessage(int user, int conversationId)
        {
            if (user == 0)
                return null;

            var Message = _context.Messages.Where(x => x.UserId != user && x.Status== false && x.Conversation == conversationId).ToList();

            foreach (var messa in Message)
            {
                messa.Status = true;
                _context.Messages.UpdateRange(messa);
                _context.SaveChanges();
            }


            return new ObjectResult(Message);
        }

        public int? GetNotificationCount(int user)
        {
            if (user == 0)
                return null;
          
            var notifications = _context.NotificationSystems.Where(x => x.UserTo == user && x.Archive == false && x.View == false)
                .Count();
            return notifications;
        }
    }
}
