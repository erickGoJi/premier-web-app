using dal.premier.DBContext;
using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Repository.Chat_Immigration_Relocation;

namespace dal.premier.Repository.Chat_Immigration_Relocation
{
    public class ChatCommentImmigrationRelocationRepository : GenericRepository<ChatImmigrationRelocation>, IChatCommentImmigrationRelocationRepository
    {
        public ChatCommentImmigrationRelocationRepository(Db_PremierContext context) : base(context) { }
    }
}
