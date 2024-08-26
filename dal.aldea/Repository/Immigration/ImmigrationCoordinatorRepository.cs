using biz.premier.Entities;
using dal.premier.DBContext;
using System;
using System.Collections.Generic;
using System.Text;
using biz.premier.Repository.Immigration;
using System.Linq;

namespace dal.premier.Repository.Immigration
{
    public class ImmigrationCoordinatorRepository : GenericRepository<biz.premier.Entities.ImmigrationCoodinator>, IIminigrationCoordinatorRepository
    {
        public ImmigrationCoordinatorRepository(Db_PremierContext context) : base(context) { }

        public ImmigrationCoodinator AddCoordinator(ImmigrationCoodinator coordinator)
        {
            coordinator.CreatedDate = DateTime.Now;
            var _coordinator = _context.ImmigrationCoodinators.FirstOrDefault(x => x.Id == coordinator.Id);

            if (_coordinator != null)
            {
                _context.ImmigrationCoodinators.Update(coordinator);
            }
            else
            {
                _context.ImmigrationCoodinators.Add(coordinator);
            }
            
            _context.SaveChanges();
            return coordinator;
        }

    }
}
