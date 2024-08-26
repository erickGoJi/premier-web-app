using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Relocation
{
    public interface IRelocationCoordinatorRepository : IGenericRepository<RelocationCoordinator>
    {
        RelocationCoordinator AddCoordinator(RelocationCoordinator coordinator);
    }
}
