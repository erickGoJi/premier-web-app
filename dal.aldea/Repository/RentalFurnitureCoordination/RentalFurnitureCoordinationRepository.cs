using biz.premier.Repository.RentalFurnitureCoordination;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.RentalFurnitureCoordination
{
    public class RentalFurnitureCoordinationRepository : GenericRepository<biz.premier.Entities.RentalFurnitureCoordination>, IRentalFurnitureCoordinationRepository
    {
        public RentalFurnitureCoordinationRepository(Db_PremierContext context) : base(context) { }

        public biz.premier.Entities.RentalFurnitureCoordination GetCustom(int key)
        {
            var query = _context.RentalFurnitureCoordinations
                .Include(i => i.ReminderRentalFurnitureCoordinations)
                .Include(i => i.PaymentsRentalFurnitureCoordinations)
                .Include(i => i.DocumentRentalFurnitureCoordinations)
                .Include(i => i.ExtensionRentalFurnitureCoordinations)
                .Include(i => i.CommentRentalFurnitureCoordinations)
                    .ThenInclude(i => i.User).ThenInclude(i => i.Role)
                .Include(i => i.StayExtensionRentalFurnitureCoordinations)
                .Include(i => i.PaymentRentals)
                .Single(s => s.Id == key);
            return query;
        }

        public biz.premier.Entities.RentalFurnitureCoordination UpdateCustom(biz.premier.Entities.RentalFurnitureCoordination rentalFurnitureCoordination, int key)
        {
            if (rentalFurnitureCoordination == null)
                return null;
            var exist = _context.Set<biz.premier.Entities.RentalFurnitureCoordination>()
                .Include(i => i.CommentRentalFurnitureCoordinations)
                .Single(s => s.Id == key);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(rentalFurnitureCoordination);
                foreach (var i in rentalFurnitureCoordination.CommentRentalFurnitureCoordinations)
                {
                    var comment = exist.CommentRentalFurnitureCoordinations.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (comment == null)
                    {
                        exist.CommentRentalFurnitureCoordinations.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                
                _context.SaveChanges();
            }
            
            UpdateStatusServiceRecord(exist.WorkOrderServicesId,exist.StatusId);
            return exist;
        }
    }
}
