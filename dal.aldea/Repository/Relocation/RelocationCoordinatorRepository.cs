using biz.premier.Entities;
using biz.premier.Repository.Relocation;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.Relocation
{
    public class RelocationCoordinatorRepository : GenericRepository<RelocationCoordinator>, IRelocationCoordinatorRepository
    {
        public RelocationCoordinatorRepository (Db_PremierContext context) : base(context) { }

        public RelocationCoordinator AddCoordinator(RelocationCoordinator coordinator)
        {
            coordinator.CreatedDate = DateTime.Now;
            var _coordinator = _context.RelocationCoordinators.FirstOrDefault(x => x.Id == coordinator.Id);

            if (_coordinator != null)
            {
                _context.RelocationCoordinators.Update(coordinator);
            }
            else
            {
                _context.RelocationCoordinators.Add(coordinator);
            }

            _context.SaveChanges();
            return coordinator;
        }
    }
}
