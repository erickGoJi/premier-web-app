using biz.premier.Entities;
using biz.premier.Repository;
using biz.premier.Repository.Follow;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace dal.premier.Repository.Follow
{
    public class FollowRepository: GenericRepository<biz.premier.Entities.Follow>, IFollowRepository
    {
        public FollowRepository(Db_PremierContext context): base(context) { }
    }
}
