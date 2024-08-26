using biz.premier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace biz.premier.Repository.Immigration
{
    public interface IIminigrationCoordinatorRepository : IGenericRepository<biz.premier.Entities.ImmigrationCoodinator>
    {
        ImmigrationCoodinator AddCoordinator(ImmigrationCoodinator coordinator);
    }
}
