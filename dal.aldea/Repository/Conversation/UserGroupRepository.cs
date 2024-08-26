using biz.premier.Repository.Conversation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Conversation
{
    public class UserGroupRepository : GenericRepository<biz.premier.Entities.UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.UserGroup AddUser(biz.premier.Entities.UserGroup userGroup)
        {
            _context.UserGroups.Add(userGroup);
            _context.SaveChanges();
            return userGroup;
        }
    }
}
