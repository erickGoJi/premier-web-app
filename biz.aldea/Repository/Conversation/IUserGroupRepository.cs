using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Conversation
{
    public interface IUserGroupRepository : IGenericRepository<biz.premier.Entities.UserGroup>
    {
        biz.premier.Entities.UserGroup AddUser(biz.premier.Entities.UserGroup userGroup);
    }
}
