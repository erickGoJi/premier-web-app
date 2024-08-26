using biz.premier.Entities;
using biz.premier.Repository.AssigneeName;
using dal.premier.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.AssigneeName
{
    public class AssigneeNameRepository : GenericRepository<biz.premier.Entities.AssigneeInformation>, IAssigneeNameRepository
    {
        public AssigneeNameRepository(Db_PremierContext context): base(context) { }

        public AssigneeInformation updateCustom(AssigneeInformation assigmentInformation)
        {
            if (assigmentInformation == null)
                return null;
            var exist = _context.Set<AssigneeInformation>()
                .Include(i => i.PetsNavigation)
                .Include(i => i.DependentInformations)
                    .ThenInclude(i => i.LanguageDependentInformations)
                .Include(i => i.LanguagesSpokens)
                .Include(i=> i.NationalityAssigneeInformations)
                .Single(s => s.Id == assigmentInformation.Id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(assigmentInformation);

                exist.LanguagesSpokens.Clear();
                _context.SaveChanges();
                foreach (var o in assigmentInformation.LanguagesSpokens)
                {
                    exist.LanguagesSpokens.Add(o);
                }

                exist.NationalityAssigneeInformations.Clear();
                _context.SaveChanges();
                foreach (var o in assigmentInformation.NationalityAssigneeInformations)
                {
                    o.Id = 0;
                    exist.NationalityAssigneeInformations.Add(o);
                }


                foreach (var p in assigmentInformation.PetsNavigation)
                {
                    var pet = exist.PetsNavigation.Where(x => x.Id == p.Id).FirstOrDefault();
                    if(pet == null)
                    {
                        exist.PetsNavigation.Add(p);
                    }
                    else
                    {
                        _context.Entry(pet).CurrentValues.SetValues(p);
                    }
                }

                foreach(var d in assigmentInformation.DependentInformations)
                {
                    var dependent = exist.DependentInformations.Where(x => x.Id == d.Id).FirstOrDefault();
                    if (dependent == null)
                    {
                        exist.DependentInformations.Add(d);
                    }
                    else
                    {
                        _context.Entry(dependent).CurrentValues.SetValues(d);
                       // dependent.LanguageDependentInformations.Clear();
                        _context.SaveChanges();
                       // foreach (var o in d.LanguageDependentInformations)
                       // {
                        //    dependent.LanguageDependentInformations.Add(o);
                       // }
                       // _context.SaveChanges();
                    }
                }
                _context.SaveChanges();
            }
            return exist;
        }

        public AssigneeInformation GetAssigneeInfoByWOSId(int wos_id)
        {
            int country = 0;
            int service_type = 0;
            int assa_id = 0;
            var standalone = _context.StandaloneServiceWorkOrders
                .Where(x => x.WorkOrderServiceId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn,
                    type = s.ServiceId,
                    wo_id = s.WorkOrderId,
                    sr_id = s.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a=> a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id
                }).ToList();

            if (standalone.Count > 0)
            {
                country = standalone.FirstOrDefault().country.Value;
                service_type = standalone.FirstOrDefault().type.Value;
                assa_id = standalone.FirstOrDefault().ass_id;
            }
            else
            {
                var bundled = _context.BundledServices
                .Where(x => x.WorkServicesId == wos_id)
                .Select(s => new
                {
                    country = s.DeliveringIn
                    ,type = s.ServiceId,
                    sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                    ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id
                }).ToList();

                if (bundled.Count > 0)
                {
                    country = bundled.FirstOrDefault().country.Value;
                    service_type = bundled.FirstOrDefault().type.Value;
                    assa_id = bundled.FirstOrDefault().ass_id;
                }
            }


            var exist = _context.Set<AssigneeInformation>()
                .Include(i => i.PetsNavigation)
                .Include(i => i.DependentInformations)
                .ThenInclude(i => i.LanguageDependentInformations)
                .Include(i => i.LanguagesSpokens)
                .Single(s => s.Id == assa_id);

            return exist;
        }

        public AssigneeInformation GetAssigneeInfoBySRId(int sr_id) { 

            var exist = _context.Set<AssigneeInformation>()
                .Include(i => i.PetsNavigation)
                .Include(i => i.DependentInformations)
                    .ThenInclude(i => i.LanguageDependentInformations)
                .Include(i => i.LanguagesSpokens)
                .Include(i=> i.NationalityAssigneeInformations)
                .Single(s => s.ServiceRecordId == sr_id);


            return exist;
        }

        public DependentInformation GetDependentInformationId(int id)
        {

            var exist = _context.Set<DependentInformation>()
                .Include(i => i.LanguageDependentInformations)
                .Single(s => s.Id == id);

            return exist;
        }

        public DependentInformation updateDepCustom(DependentInformation dep)
        {
            if (dep == null)
                return null;
            var dependent = _context.Set<DependentInformation>()
                .Include(i => i.LanguageDependentInformations)
                .Single(s => s.Id == dep.Id);
            if (dependent != null)
            {
                _context.Entry(dependent).CurrentValues.SetValues(dep);
                dependent.LanguageDependentInformations.Clear();
                _context.SaveChanges();
                foreach (var o in dep.LanguageDependentInformations)
                {
                    dependent.LanguageDependentInformations.Add(o);
                }
                _context.SaveChanges();
            }
            return dependent;
        }


        public int DeleteDependent(int id)
        {
            var consult = _context.DependentInformations.SingleOrDefault(s => s.Id == id);

            if (consult != null)
            {
                _context.DependentInformations.Remove(consult);
                _context.SaveChanges();
            }

            return id;
        }


        public int DeletePet(int id)
        {
            var consult = _context.Pets.SingleOrDefault(s => s.Id == id);

            if (consult != null)
            {
                _context.Pets.Remove(consult);
                _context.SaveChanges();
            }

            return id;
        }

        public DependentInformation AddDepCustom(DependentInformation dep)
        {

            if (dep != null)
            {
                _context.DependentInformations.Add(dep);

                //foreach (var o in dep.LanguageDependentInformations)
                //{
                //    dependent.LanguageDependentInformations.Add(o);
                //}
                _context.SaveChanges();
            }
            return dep;
        }

        public Pet GetPetId(int id)
        {

            var exist = _context.Set<Pet>()
                .Single(s => s.Id == id);

            return exist;
        }


        public Pet updatePetCustom(Pet pet)
        {
            if (pet == null)
                return null;
            var _pet = _context.Set<Pet>()
                .Single(s => s.Id == pet.Id);
            if (_pet != null)
            {
                _context.Entry(_pet).CurrentValues.SetValues(pet);
                _context.SaveChanges();
                
                _context.SaveChanges();
            }
            return _pet;
        }

        public Pet AddPetCustom(Pet dep)
        {

            if (dep != null)
            {
                _context.Pets.Add(dep);
                _context.SaveChanges();
            }
            return dep;
        }
    }
}
