using biz.premier.Entities;
using biz.premier.Repository.WorkOrder;
using dal.premier.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dal.premier.Repository.WorkOrder
{
    public class HousingListRepository : GenericRepository<HousingList>, IHousingListRepository
    {
        private readonly IHousingListRepository _housingListRepository;

        public HousingListRepository(Db_PremierContext context) : base(context) { }

        public Attendee AddAttendee(Attendee inspection)
        {
            _context.Attendees.Add(inspection);
            _context.SaveChanges();
            return inspection;
        }

        public List<CatPaymentRepairResponsability> GetCatPaymentRepairResponsability()
        {
            var all = _context.CatPaymentRepairResponsabilities.Where(u => u.Id == u.Id).ToList();
            return all;
        }


        public AttendeeInspec  AddAttendeeInspec(AttendeeInspec attendee)
        {
            _context.AttendeeInspecs.Add(attendee);
            _context.SaveChanges();
            return attendee;
        }

        public ActionResult GetAttendeesTitles(int id_service_detail, int property_id, int sr_id)
        {
           
            List<list_attendees> list_dependents =  _context.DependentInformations
               .Where(x => x.AssigneeInformation.ServiceRecordId == sr_id).Select(s => new list_attendees
               {
                   id_Catalog = s.Id.ToString() + "D",
                   id_person = s.Id,
                   type = "D",
                   name = s.Name ?? "Name not captured",
                   email = s.Email ?? "Email not captured",
                   text = s.Relationship.Relationship+ " / " +s.Name ?? "Name not captured",
                   title = s.Relationship.Relationship
               })
               .OrderBy(s => s.name)
               .ToList();

            List<list_attendees> list_land =  _context.LandlordHeaderDetailsHomes
                .Where(r =>  r.HousingListId == property_id) // se trae todos los LD de esa propiedad 
                .Select(s => new list_attendees
               {
                   id_Catalog = s.Id.ToString() +"L",
                   id_person = s.Id,
                   type = "L",
                   name = s.Name ?? "Name not captured",
                   email = s.PrincipalEmail == null ? "Email not captured" : s.PrincipalEmail,
                   text = "Landlord / " + (s.Name == null ? "Name not captured" : s.Name),
                   title = "Landlord"
                })
               .OrderBy(s => s.name)
               .ToList();

            List<list_attendees> list_sup = new List<list_attendees>();

            var supplierPartner = _context.HousingLists.FirstOrDefault(t => t.Id == property_id).SupplierNavigation;

            if(supplierPartner != null)
            {
                list_sup = _context.AdministrativeContactsServices
               .Where(x => x.AreasCoverageNavigation.SupplierPartnerProfileServiceNavigation.Id == supplierPartner.Id
                         //  && x.AreasCoverageNavigation.Country.Value == country
               )
               .Select(s => new list_attendees
               {
                   id_Catalog = "0S",
                   id_person = s.Id,
                   type = "S",
                   name = s.ContactName ?? "Name not captured",
                   email = s.Email ?? "Email not captured",
                   text = "Realtor / " + s.ContactName ?? "Name not captured",
                   title = "Realtor"
            }).ToList();
            }

            if(list_sup.Count < 1)
            {
                list_attendees sobj = new list_attendees();
                sobj.id_Catalog = "0S";
                sobj.id_person = 0;
                sobj.type = "S";
                sobj.name = "";
                sobj.email = "";
                sobj.text = "Realtor";
                sobj.title = "Realtor";
                list_sup.Add(sobj);
            }

            list_attendees obj = new list_attendees();
            obj.id_Catalog = 0 + "O";
            obj.id_person = 0;
            obj.type = "O";
            obj.name = "";
            obj.email = "";
            obj.text = "Other";
            obj.title = "Other";
            List<list_attendees> list_o = new List<list_attendees>();
            list_o.Add(obj);

            System.Collections.Generic.IEnumerable<list_attendees> list_all = list_dependents;

            if(list_land != null)
                list_all = list_all.Union(list_land);
            if (list_sup != null)
                list_all = list_all.Union(list_sup);

            list_all = list_all.Union(list_o);

            return new ObjectResult(list_all); 
        }

        public ActionResult GetItemsSectionInventory(int section)
        {
            var result = _context.ItemsSectionInventories
                .Where(t => t.PropertySection == (section == 0 ? t.PropertySection : section)).ToList();

            return new ObjectResult(result);
        }

        class list_attendees
        {

            public list_attendees() { }

            public string id_Catalog { get; set; }

            public int id_person { get; set; }
            public string type { get; set; }

            public string name { get; set; }
            public string email { get; set;  }
            public string text { get; set; }

            public string title { get; set; }
        }

        public class det_grales_servicio
        {
            public int? country { get; set; }
            public string country_name { get; set; }
            public string location  { get; set; }
            public int? type { get; set; }
            public int? wos_id { get; set; }
            public int? wo_id  { get; set; }
            public int? sr_id { get; set; }
            public int? ass_id { get; set; }
            public int? id_client  { get; set; }
            public DateTime? creation { get; set; }
            public int? service_category { get; set; }
        }


        public ContractDetail AddContractDetail(ContractDetail contract)
        {
            _context.ContractDetails.Add(contract);
            _context.SaveChanges();
            return contract;
        }

        public CostSavingHome AddCostSavingHome(CostSavingHome renewalDetail)
        {
            _context.CostSavingHomes.Add(renewalDetail);
            _context.SaveChanges();
            return renewalDetail;
        }

        public DepartureDetailsHome AddDepartureDetailHome(DepartureDetailsHome departure)
        {
            _context.DepartureDetailsHomes.Add(departure);
            _context.SaveChanges();
            return departure;
        }

        public GroupCostSaving AddGroupCostSaving(GroupCostSaving group)
        {
            _context.GroupCostSavings.Add(group);
            _context.SaveChanges();
            return group;
        }

        public GroupPaymnetsHousing AddGroupPaymnetsHousing(GroupPaymnetsHousing group)
        {
            _context.GroupPaymnetsHousings.Add(group);
            _context.SaveChanges();
            return group;
        }

        public biz.premier.Entities.PropertyReport AddPrpertyReport(biz.premier.Entities.PropertyReport prt)
        {
            _context.PropertyReports.Add(prt);
            _context.SaveChanges();
            return prt;
        }


        public biz.premier.Entities.GroupIr AddGroupIr(biz.premier.Entities.GroupIr gir)
        {
            _context.GroupIrs.Add(gir);
            _context.SaveChanges();
            return gir;
        }


        public KeyInventory AddKeyInventory(KeyInventory keyInventory)
        {
            _context.KeyInventories.Add(keyInventory);
            _context.SaveChanges();
            return keyInventory;
        }

        public ActionResult AddLandlordDetailsHome(LandlordDetailsHome landlord)
        {
            _context.LandlordDetailsHomes.Add(landlord);
            _context.SaveChanges();

            var list_all = _context.LandlordDetailsHomes.Where(s => s.HeaderId == landlord.HeaderId).ToList();

            return new ObjectResult(list_all);
        }

        public LandlordHeaderDetailsHome AddLandlordHeaderDetailsHomee(LandlordHeaderDetailsHome landlord)
        {
            _context.LandlordHeaderDetailsHomes.Add(landlord);
            _context.SaveChanges();
            return landlord;
        }


        public PaymentHousing AddPaymentHousing(PaymentHousing payment)
        {
            _context.PaymentHousings.Add(payment);
            _context.SaveChanges();
            return payment;
        }

        public PaymentSchooling AddPaymentSchooling(PaymentSchooling payment)
        {
            _context.PaymentSchoolings.Add(payment);
            _context.SaveChanges();
            return payment;
        }

        public PaymentRental AddPaymentRental(PaymentRental payment)
        {
            _context.PaymentRentals.Add(payment);
            _context.SaveChanges();
            return payment;
        }

        public RenewalDetailHome AddRenewalDetailHome(RenewalDetailHome renewalDetail)
        {
            _context.RenewalDetailHomes.Add(renewalDetail);
            _context.SaveChanges();
            return renewalDetail;
        }

       

        public List<HousingStatusHistory> GetAllHistory(int key)
        {
            if (key == 0)
                return null;
            var consult = _context.HousingStatusHistories
                .Include(i => i.CreatedByNavigation)
                    .ThenInclude(i => i.UserType)
                .Include(i => i.Housing)
                .Include(i => i.StatusNavigation)
                .Where(x => x.HousingId == key).ToList();
            return consult;
        }


        public biz.premier.Entities.Departure get_departure(int id_service)
        {
            var dep = _context.Departures.FirstOrDefault(d => d.Id == id_service);
            var dep2 = _context.Departures.FirstOrDefault(d => d.Id == 2000);
            return dep;
        }
        public HousingList GetCustom(int key)
        {
            if (key == 0)
                return null;
            var consult = _context.HousingLists
                .Include(i => i.SupplierNavigation)
                .Include(i => i.SupplierPartnerNavigation)
                .Include(i => i.AmenitiesHousingLists)
                .Include(i => i.CommentHousings)
                   .ThenInclude(i => i.CreationByNavigation)
                        .ThenInclude(i => i.UserType)
                .Include(i => i.DocumentHousings)
                .Include(i => i.DepartureDetailsHomes)
                .Include(i => i.RenewalDetailHomes)
                .Include(i => i.GroupCostSavings)
                .Include(i => i.GroupPaymnetsHousings)
                .Include(i => i.ContractDetails)
                    .ThenInclude(i => i.PropertyExpenses)
                .Include(i => i.LandlordHeaderDetailsHomes)
                .Include(i => i.GroupIrs)
                    .ThenInclude(j=> j.Repairs)
                        .ThenInclude(i => i.DocumentRepairs)
                .Include(i => i.GroupIrs)
                    .ThenInclude(i =>  i.Inspections)
                .Include(i => i.PropertyReports)
                    .ThenInclude(i => i.PropertyReportSections)
                        .ThenInclude(i => i.SectionInventories)
                            .ThenInclude(i => i.PhotosInventories)
                .Include(i => i.PropertyReports)
                    .ThenInclude(i => i.PropertyReportSections)
                        .ThenInclude(i => i.PhotosPropertyReportSections)
                .Include(i => i.PropertyReports)
                    .ThenInclude(i => i.KeyInventories)
                .Include(i => i.PropertyReports)
                    .ThenInclude(i => i.Attendees)
                .SingleOrDefault(x => x.Id == key);
            return consult;
        }


        public List<ContractDetail> GetLeaseVersions(int id_service_detail, int id_catCategoryId, int housing_list_id)
        {

            if (id_catCategoryId == 21) // si es Home finding solo te tare los de ese servicio
            {
                var consult = _context.ContractDetails.Where(x => x.IdServiceDetail == id_service_detail 
                                                             && x.ContractDetailId == housing_list_id
                                                             && x.ContractDetailNavigation.IdServiceDetail == id_service_detail).ToList();
                return consult;
            }
            else // si no es home finding te tare todos 
            {
                var consult = _context.ContractDetails.Where(x => x.ContractDetailId == housing_list_id
                                                             && x.ContractDetailNavigation.IdServiceDetail > 0).ToList();
                return consult;
            }
        }

        public List<GroupIr> GetInspectionsVersions(int id_service_detail, int id_catCategoryId, int housing_list_id)
        {

            if (id_catCategoryId == 21) // si es Home finding solo te tare los de ese servicio
            {
                var consult = _context.GroupIrs.Where(x => x.IdServiceDetail == id_service_detail 
                                                             && x.HousingListId == housing_list_id).ToList();
                return consult;
            }
            else // si no es home finding te tare todos 
            {
                var consult = _context.GroupIrs.Where(x => x.HousingListId == housing_list_id 
                                                        && x.HousingList.IdServiceDetail > 0).ToList();
                return consult;
            }
        }

        public HousingList GetCustom_new(int key, int servide_detail_id)
        {
            if (key == 0)
                return null;
            var consult = _context.HousingLists
                .Include(i => i.GroupPaymnetsHousings)
                    .ThenInclude(o => o.PaymentHousings)
                .Include(n => n.GroupCostSavings)
                    .ThenInclude(a => a.CostSavingHomes)
                .Include(d => d.ContractDetails)
                    .ThenInclude(i => i.PropertyExpenses).Where(x => x.Id == key)
                .Select(s => new HousingList
                {
                    Id = s.Id,
                    WorkOrder = s.WorkOrder,
                    Service = s.Service,
                    ServiceType = s.ServiceType,
                    PropertyNo = s.PropertyNo,
                    Sample = s.Sample,
                    SupplierPartner = s.SupplierPartner,
                    Supplier = s.Supplier,
                    VisitDate = s.VisitDate,
                    HousingStatus = s.HousingStatus,
                    PropertyType = s.PropertyType,
                    Address = s.Address,
                    Zip = s.Zip,
                    WebSite = s.WebSite,
                    Bedrooms = s.Bedrooms,
                    Bathrooms = s.Bathrooms,
                    ParkingSpaces = s.ParkingSpaces,
                    Price = s.Price,
                    Currency = s.Currency,
                    AdditionalComments = s.AdditionalComments,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    UpdateBy = s.UpdateBy,
                    UpdatedDate = s.UpdatedDate,
                    IdServiceDetail = s.IdServiceDetail,
                    Othersupplier = s.Othersupplier,
                    Suppliertelephone = s.Suppliertelephone,
                    Supplieremail = s.Supplieremail,
                    SupplierNavigation = s.SupplierNavigation,
                    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                    AmenitiesHousingLists = s.AmenitiesHousingLists,
                    CommentHousings = s.CommentHousings,
                    DocumentHousings = s.DocumentHousings,
                    Shared = s.Shared,
                    VisitTime = s.VisitTime,
                    Neighborhood = s.Neighborhood,
                    Wassended = s.Wassended == null ? false : s.Wassended,
                    DepartureDetailsHomes = _context.DepartureDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//
                    RenewalDetailHomes = _context.RenewalDetailHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//
                    GroupCostSavings = _context.GroupCostSavings.Where(r => r.IdServiceDetail == servide_detail_id).ToList(),  //solo agregar campo y consulta 
                    GroupPaymnetsHousings = _context.GroupPaymnetsHousings.Where(r => r.IdServiceDetail == servide_detail_id).ToList(), //solo agregar campo y consulta 
                                                                                                                                        //ContractDetails = _context.ContractDetails.Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),// llave y campos                                                        //LandlordDetailsHomes = s.LandlordDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),// llave y campos 

                    //Repairs = s.Repairs.Where(r => r.IdServiceDetail == servide_detail_id).ToList(), // solo agregar campo y consulta 
                    //Inspections = s.Inspections.Where(r => r.IdServiceDetail == servide_detail_id).ToList(), // solo agregar campo y consulta 
                    //PropertyReports = s.PropertyReports.Where(r => r.IdServiceDetail == servide_detail_id).ToList()  // solo agregar campo y consulta //
                }).ToList();

            //var consult = _context.HousingLists
            //    .Include(i => i.GroupPaymnetsHousings)
            //        .ThenInclude(o => o.PaymentHousings)
            //    .Include(n => n.GroupCostSavings)
            //        .ThenInclude(a => a.CostSavingHomes)
            //    .Include(d => d.ContractDetails)
            //        .ThenInclude(i => i.PropertyExpenses)
            //    .Where(x => x.Id == key).Select(s => new HousingList
            //{
            //    Id = s.Id,
            //    WorkOrder = s.WorkOrder,
            //    Service = s.Service,
            //    ServiceType = s.ServiceType,
            //    PropertyNo = s.PropertyNo,
            //    Sample = s.Sample,
            //    SupplierPartner = s.SupplierPartner,
            //    Supplier = s.Supplier,
            //    VisitDate = s.VisitDate,
            //    HousingStatus = s.HousingStatus,
            //    PropertyType = s.PropertyType,
            //    Address = s.Address,
            //    Zip = s.Zip,
            //    WebSite = s.WebSite,
            //    Bedrooms = s.Bedrooms,
            //    Bathrooms = s.Bathrooms,
            //    ParkingSpaces = s.ParkingSpaces,
            //    Price = s.Price,
            //    Currency = s.Currency,
            //    AdditionalComments = s.AdditionalComments,
            //    CreatedBy = s.CreatedBy,
            //    CreatedDate = s.CreatedDate,
            //    UpdateBy = s.UpdateBy,
            //    UpdatedDate = s.UpdatedDate,
            //    IdServiceDetail = s.IdServiceDetail,
            //    Othersupplier = s.Othersupplier,
            //    Suppliertelephone = s.Suppliertelephone,
            //    Supplieremail = s.Supplieremail,
            //    SupplierNavigation = s.SupplierNavigation,
            //    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
            //    AmenitiesHousingLists = s.AmenitiesHousingLists,
            //    CommentHousings = s.CommentHousings,
            //    DocumentHousings = s.DocumentHousings,
            //    Shared = s.Shared,
            //    VisitTime = s.VisitTime,
            //    Neighborhood = s.Neighborhood,
            //    DepartureDetailsHomes = s.DepartureDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//
            //    RenewalDetailHomes = s.RenewalDetailHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//
            //    GroupCostSavings = s.GroupCostSavings.Where(r => r.IdServiceDetail == servide_detail_id).ToList(),  //solo agregar campo y consulta 
            //    GroupPaymnetsHousings = s.GroupPaymnetsHousings.Where(r => r.IdServiceDetail == servide_detail_id).ToList(), //solo agregar campo y consulta 
            //    ContractDetails = s.ContractDetails.Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),// llave y campos 
            //    //LandlordDetailsHomes = s.LandlordDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),// llave y campos 
            //    Repairs = s.Repairs.Where(r => r.IdServiceDetail == servide_detail_id).ToList(), // solo agregar campo y consulta 
            //    Inspections = s.Inspections.Where(r => r.IdServiceDetail == servide_detail_id).ToList(), // solo agregar campo y consulta 
            //    PropertyReports = s.PropertyReports.Where(r => r.IdServiceDetail == servide_detail_id).ToList(),  // solo agregar campo y consulta //

            //}).ToList();

            var response = consult[0];
            return response;
        }


        public HousingList GetCustom_historic(int key, int servide_detail_id)
        {
            if (key == 0)
                return null;
            var consult = _context.HousingLists
                .Include(i=> i.GroupPaymnetsHousings)
                    .ThenInclude(o=> o.PaymentHousings)
                .Include(n=> n.GroupCostSavings)
                    .ThenInclude(a=> a.CostSavingHomes)
                .Include(d=> d.ContractDetails)
                    .ThenInclude(i=> i.PropertyExpenses)
                .Where(x => x.Id == key).Select(s => new HousingList
            {
                Id = s.Id,
                WorkOrder = s.WorkOrder,
                Service = s.Service,
                ServiceType = s.ServiceType,
                PropertyNo = s.PropertyNo,
                Sample = s.Sample,
                SupplierPartner = s.SupplierPartner,
                Supplier = s.Supplier,
                VisitDate = s.VisitDate,
                HousingStatus = s.HousingStatus,
                PropertyType = s.PropertyType,
                Address = s.Address,
                Zip = s.Zip,
                WebSite = s.WebSite,
                Bedrooms = s.Bedrooms,
                Bathrooms = s.Bathrooms,
                ParkingSpaces = s.ParkingSpaces,
                Price = s.Price,
                Currency = s.Currency,
                AdditionalComments = s.AdditionalComments,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate,
                UpdateBy = s.UpdateBy,
                UpdatedDate = s.UpdatedDate,
                IdServiceDetail = s.IdServiceDetail,
                Othersupplier = s.Othersupplier,
                Suppliertelephone = s.Suppliertelephone,
                Supplieremail = s.Supplieremail,
                SupplierNavigation = s.SupplierNavigation,
                SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                AmenitiesHousingLists = s.AmenitiesHousingLists,
                CommentHousings = s.CommentHousings,
                DocumentHousings = s.DocumentHousings,
                Shared = s.Shared,
                VisitTime = s.VisitTime,
                Neighborhood = s.Neighborhood,
                DepartureDetailsHomes = s.DepartureDetailsHomes.Where(r => r.Id == key).ToList(),//
                RenewalDetailHomes = s.RenewalDetailHomes.Where(r => r.Id == key).ToList(),//
                GroupCostSavings = s.GroupCostSavings.Where(r => r.HousingListId == key).ToList(),  
                GroupPaymnetsHousings = s.GroupPaymnetsHousings.Where(r => r.HousingListId.Value == key).ToList(),
                //GroupPaymnetsHousings = s.GroupPaymnetsHousings.Where(r => r.HousingListId.Value == key).Select(s1 => new GroupPaymnetsHousing
                //{
                //    Id = s1.Id,
                //    Visible = s1.Visible,
                //    IdServiceDetail = s1.IdServiceDetail,
                //    HousingListId = s1.HousingListId,
                //    CreatedBy = s1.CreatedBy,
                //    CreatedDate = s1.CreatedDate,
                //    UpdateBy = s1.UpdateBy,
                //    UpdatedDate = s1.UpdatedDate,
                //    ServiceNumber = s1.ServiceNumber,
                //    ServiceTypeText = s1.ServiceTypeText,
                //    WorkOrderText = s1.WorkOrderText,
                //    PaymentHousings = _context.PaymentHousings.Where(ph => ph.GroupPaymentsHousingId == s1.Id).ToList()
                //}).ToList(),
                ContractDetails = s.ContractDetails.Where(r => r.ContractDetailId == key).ToList(),
               // LandlordDetailsHomes = s.LandlordDetailsHomes.Where(r =>  r.Id == key).ToList(),
               // Repairs = s.Repairs.Where(r => r.HousingList == key).ToList(), 
               // Inspections = s.Inspections.Where(r => r.HousingList == key).ToList(), 
                PropertyReports = s.PropertyReports.Where(r => r.HousingList == key).ToList(),  

            }).ToList();

            //var p = _context.GroupPaymnetsHousings.Where(p1 => p1.HousingListId == key).ToList();

            var _hosuging = consult[0];
            HousingList response = _hosuging;
            return response;
        }

        // return new ObjectResult(query);

        public ActionResult GetHomeFindingHousingList(int id_service_detail)
        {

            var custom = _context.HousingLists.Where(i => i.IdServiceDetail == id_service_detail).Select(s => new
            {
                s.Id,
                supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                supplierCompany = s.SupplierNavigation == null ? null : s.SupplierPartnerNavigation.ComercialName,
                s.PropertyNo,
                PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                s.Address,
                s.Price,
                s.Neighborhood,
                s.VisitTime,
                Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                Status = s.HousingStatusNavigation == null ? null : s.HousingStatusNavigation.Status,
                s.Wassended,
                s.Bedrooms,
                s.Bathrooms,
                s.ParkingSpaces,
                Sise = s.Size,
                AssigneComments = "",
                RelAppointmentHousingLists = s.RelAppointmentHousingLists.Any()

            }).ToList();

            return new ObjectResult(custom); ;

        }


        

        public ActionResult GetHistoricHousingByTypeAndId(int key, int servide_detail_id)
        {
            if (key == 0)
                return null;

            
            var homef_services = _context.HomeFindings.Where(f => f.Id == servide_detail_id);
            


            if(homef_services.Count() > 0) // es home finding
            {
                var result = set_payments_type(key, servide_detail_id); // crea los paymnets que conicidan con la seccion de payment process de HF 
            }
            else
            {
                var set_hl = initial_setting_leasse_sf(key, servide_detail_id); // esto es por si algun motivo a la propiedad le falta alguna seccion del SLF o del I&R
            }
             

            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new 
                {
                    Id = s.Id,
                    WorkOrder = s.WorkOrder,
                    Service = s.Service,
                    ServiceType = s.ServiceType,
                    PropertyNo = s.PropertyNo,
                    Sample = s.Sample,
                    SupplierPartner = s.SupplierPartner,
                    Supplier = s.Supplier,
                    VisitDate = s.VisitDate,
                    HousingStatus = s.HousingStatus,
                    PropertyType = s.PropertyType,
                    Address = s.Address,
                    Zip = s.Zip,
                    WebSite = s.WebSite,
                    Bedrooms = s.Bedrooms,
                    Bathrooms = s.Bathrooms,
                    ParkingSpaces = s.ParkingSpaces,
                    Price = s.Price,
                    Currency = s.Currency,
                    AdditionalComments = s.AdditionalComments,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    UpdateBy = s.UpdateBy,
                    UpdatedDate = s.UpdatedDate,
                    IdServiceDetail = s.IdServiceDetail,
                    Othersupplier = s.Othersupplier,
                    Suppliertelephone = s.Suppliertelephone,
                    Supplieremail = s.Supplieremail,
                    SupplierNavigation = s.SupplierNavigation,
                    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                    AmenitiesHousingLists = s.AmenitiesHousingLists,
                    CommentHousings = s.CommentHousings,
                    DocumentHousings = s.DocumentHousings,
                    Shared = s.Shared,
                    VisitTime = s.VisitTime,
                    Neighborhood = s.Neighborhood,
                    Size = s.Size,

                    ////// LSF 
                DepartureDetailsHomes = _context.DepartureDetailsHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                departureHistoricDetailsHomes = _context.DepartureDetailsHomes
                .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                .OrderByDescending(h => h.CreatedDate).ToList(),// lista de pasados

                RenewalDetailHomes = _context.RenewalDetailHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                renewalHistoricDetailHomes = _context.RenewalDetailHomes
                .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                .OrderByDescending(h => h.CreatedDate).ToList(),//lista de pasados

                GroupCostSavings = _context.GroupCostSavings
                    .Include(o => o.CostSavingHomes)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                groupHistoricCostSavings = _context.GroupCostSavings
                    .Include(o => o.CostSavingHomes)
                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                .OrderByDescending(h => h.CreatedDate).ToList(), // lista de pasados

                GroupPaymnetsHousings = _context.GroupPaymnetsHousings
                    .Include(o => o.PaymentHousings)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                groupHistoricPaymnetsHousings = _context.GroupPaymnetsHousings
                    .Include(o => o.PaymentHousings)
                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key).OrderByDescending(h => h.CreatedDate).ToList(),

                ContractDetails = _context.ContractDetails
                    .Include(p => p.PropertyExpenses)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),

                contractHistoricDetails = _context.ContractDetails
                    .Include(p => p.PropertyExpenses)
                .Where(r => r.IdServiceDetail != servide_detail_id && r.ContractDetailId == key).OrderByDescending(h => h.CreatedDate).ToList(),

                LandlordHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                    .Include(p => p.LandlordDetailsHomes)
                    .Include(c=> c.CreditCardLandLordDetails)
                   // .ThenInclude(x => x.CreditCardLandLordDetails)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                Landlor_HistoricHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                    .Include(p => p.LandlordDetailsHomes)
                      //  .ThenInclude(x => x.CreditCardLandLordDetails)
                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key).OrderByDescending(h => h.CreatedDate).ToList(),

                    //I&R

               PropertyReports = _context.PropertyReports  // MOVE IN - OUT
                                    .Include(i => i.PropertyReportSections)
                                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                                .Include(i => i.PropertyReportSections)
                                    .ThenInclude(i => i.SectionInventories)
                                        .ThenInclude(i => i.PhotosInventories)
                                .Include(i => i.KeyInventories)
                                .Include(i => i.Attendees)
                                .Where(r => r.HousingList == key).ToList(),

               Inspections = _context.Inspections
                    .Include(i=> i.AttendeeInspecs)
                    .Include(i=> i.PhotosInspecs)
               .Where(r => r.HousingList == key)
               .ToList(),

             //  Repairs = s.Repairs.Where(r => r.HousingList == key).ToList(),
                
                

                }).ToList();

            var _hosuging = consult[0]; 
            return  new ObjectResult(_hosuging); ;
        }

        public ActionResult UpdateStatusHousingList(int idHl, int status)
        {
            var hl = _context.HousingLists.FirstOrDefault(x => x.Id == idHl);
            hl.HousingStatus = status;
            _context.HousingLists.Update(hl);
            _context.SaveChanges();
            return new ObjectResult(hl);
        }
        public ActionResult GetOnlyHomeDetail(int key)
        {
           
            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    Id = s.Id,
                    WorkOrder = s.WorkOrder,
                    Service = s.Service,
                    ServiceType = s.ServiceType,
                    PropertyNo = s.PropertyNo,
                    Sample = s.Sample,
                    SupplierPartner = s.SupplierPartner,
                    Supplier = s.Supplier,
                    VisitDate = s.VisitDate,
                    HousingStatus = s.HousingStatus,
                    PropertyType = s.PropertyType,
                    Address = s.Address,
                    Zip = s.Zip,
                    WebSite = s.WebSite,
                    Bedrooms = s.Bedrooms,
                    Bathrooms = s.Bathrooms,
                    ParkingSpaces = s.ParkingSpaces,
                    Price = s.Price,
                    Currency = s.Currency,
                    AdditionalComments = s.AdditionalComments,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    UpdateBy = s.UpdateBy,
                    UpdatedDate = s.UpdatedDate,
                    IdServiceDetail = s.IdServiceDetail,
                    Othersupplier = s.Othersupplier,
                    Suppliertelephone = s.Suppliertelephone,
                    Supplieremail = s.Supplieremail,
                    SupplierNavigation = s.SupplierNavigation,
                    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                    AmenitiesHousingLists = s.AmenitiesHousingLists,
                    CommentHousings = s.CommentHousings,
                    DocumentHousings = s.DocumentHousings,
                    Shared = s.Shared,
                    VisitTime = s.VisitTime,
                    Neighborhood = s.Neighborhood,
                    Size = s.Size,
                    MetricId = s.MetricId
    }).ToList();

            return new ObjectResult(consult[0]); 
        }

        public ActionResult GetHistoricHousingByServiceType(int key, int servide_detail_id, int type)
        {
            if (key == 0)
                return null;

            var date_service = new DateTime();

            if(type == 26)
            {
                var homef_service = _context.HomeFindings.FirstOrDefault(f => f.Id == servide_detail_id);
                date_service = homef_service.CreatedDate.Value;
                var result = set_payments_type(key, servide_detail_id); // crea los paymnets que conicidan con la seccion de payment process de HF 
            }
            
            var set_hl = initial_setting_leasse_sf(key, servide_detail_id); // esto es por si algun motivo a la propiedad le falta alguna seccion del SLF o del I&R
            

            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    Id = s.Id,
                    WorkOrder = s.WorkOrder,
                    Service = s.Service,
                    ServiceType = s.ServiceType,
                    PropertyNo = s.PropertyNo,
                    Sample = s.Sample,
                    SupplierPartner = s.SupplierPartner,
                    Supplier = s.Supplier,
                    VisitDate = s.VisitDate,
                    HousingStatus = s.HousingStatus,
                    PropertyType = s.PropertyType,
                    Address = s.Address,
                    Zip = s.Zip,
                    WebSite = s.WebSite,
                    Bedrooms = s.Bedrooms,
                    Bathrooms = s.Bathrooms,
                    ParkingSpaces = s.ParkingSpaces,
                    Price = s.Price,
                    Currency = s.Currency,
                    AdditionalComments = s.AdditionalComments,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    UpdateBy = s.UpdateBy,
                    UpdatedDate = s.UpdatedDate,
                    IdServiceDetail = s.IdServiceDetail,
                    Othersupplier = s.Othersupplier,
                    Suppliertelephone = s.Suppliertelephone,
                    Supplieremail = s.Supplieremail,
                    SupplierNavigation = s.SupplierNavigation,
                    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                    AmenitiesHousingLists = s.AmenitiesHousingLists,
                    CommentHousings = s.CommentHousings,
                    DocumentHousings = s.DocumentHousings,
                    Shared = s.Shared,
                    VisitTime = s.VisitTime,
                    Neighborhood = s.Neighborhood,

                    ////// LSF 
                    DepartureDetailsHomes = _context.DepartureDetailsHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    departureHistoricDetailsHomes = (type == 26 ? new List<DepartureDetailsHome>() :
                                                                    _context.DepartureDetailsHomes
                                                                    .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                                                                    .OrderByDescending(h => h.CreatedDate).ToList()),// lista de pasados

                    RenewalDetailHomes = _context.RenewalDetailHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    renewalHistoricDetailHomes = (type == 26 ? new List<RenewalDetailHome>() :
                                                                _context.RenewalDetailHomes
                                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                                                                .OrderByDescending(h => h.CreatedDate).ToList()),//lista de pasados

                    GroupCostSavings = _context.GroupCostSavings
                    .Include(o => o.CostSavingHomes)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    groupHistoricCostSavings = (type == 26 ? new List<GroupCostSaving>() :
                                                            _context.GroupCostSavings
                                                            .Include(o => o.CostSavingHomes)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                            .OrderByDescending(h => h.CreatedDate).ToList()), // lista de pasados

                    GroupPaymnetsHousings = _context.GroupPaymnetsHousings
                    .Include(o => o.PaymentHousings)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    groupHistoricPaymnetsHousings = (type == 26 ? new List<GroupPaymnetsHousing>() :
                                                                _context.GroupPaymnetsHousings
                                                                .Include(o => o.PaymentHousings)
                                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                                .OrderByDescending(h => h.CreatedDate).ToList()),

                    ContractDetails = _context.ContractDetails
                    .Include(p => p.PropertyExpenses)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),

                    contractHistoricDetails = (type == 26 ? new List<ContractDetail>() :
                                                            _context.ContractDetails
                                                            .Include(p => p.PropertyExpenses)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.ContractDetailId == key)
                                                            .OrderByDescending(h => h.CreatedDate).ToList()),

                    LandlordHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                    .Include(p => p.LandlordDetailsHomes)
                    .Include(c => c.CreditCardLandLordDetails)
                // .ThenInclude(x => x.CreditCardLandLordDetails)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    Landlor_HistoricHeaderDetailsHomes = (type == 26 ? new List<LandlordHeaderDetailsHome>() :
                                                                _context.LandlordHeaderDetailsHomes
                                                                .Include(p => p.LandlordDetailsHomes)
                                                                //  .ThenInclude(x => x.CreditCardLandLordDetails)
                                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                                .OrderByDescending(h => h.CreatedDate).ToList()),

                    //I&R

                    PropertyReports = _context.PropertyReports  // MOVE IN - OUT
                                    .Include(i => i.PropertyReportSections)
                                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                                .Include(i => i.PropertyReportSections)
                                    .ThenInclude(i => i.SectionInventories)
                                        .ThenInclude(i => i.PhotosInventories)
                                .Include(i => i.KeyInventories)
                                .Include(i => i.Attendees)
                                .Where(r => r.HousingList == key).ToList(),

                    Inspections = _context.Inspections
                    .Include(i => i.AttendeeInspecs)
                    .Include(i => i.PhotosInspecs)
               .Where(r => r.HousingList == key)
               .ToList(),


               groupIr = _context.GroupIrs
                    .Include(o => o.Repairs)
                        .ThenInclude(d=> d.DocumentRepairs)
                    .Include(t => t.Inspections)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),
                    //Repairs = s.Repairs.Where(r => r.HousingList == key).ToList(),

                groupIrhistoric = (type == 26 ? new List<GroupIr>() :
                                                            _context.GroupIrs
                                                            .Include(o => o.Repairs)
                                                            .Include(t => t.Inspections)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                            .OrderByDescending(h => h.CreatedDate).ToList()),


                }).ToList();

            var _hosuging = consult[0];
            return new ObjectResult(_hosuging); ;
        }


        public HousingList initial_setting_leasse_sf(int key, int servide_detail_id)
        {

            //var _hosuging = _context.HousingLists.FirstOrDefault(x => x.Id == key);

            if (_context.RenewalDetailHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList().Count < 1)
            {
                RenewalDetailHome renewal = new RenewalDetailHome();
                renewal.CreatedDate = DateTime.Now;
                renewal.Id = key;
                renewal.IdServiceDetail = servide_detail_id;
                RenewalDetailHome hl = AddRenewalDetailHome(renewal);
               // _hosuging.RenewalDetailHomes.Add(hl);
                //con_Cambios = true;
            }

            if (_context.ContractDetails.Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList().Count < 1)
            {
                ContractDetail contracts = new ContractDetail();
                contracts.CreatedDate = DateTime.Now;
                contracts.ContractDetailId = key;
                contracts.IdServiceDetail = servide_detail_id;
                ContractDetail hl = AddContractDetail(contracts);
                //_hosuging.ContractDetails.Add(hl);
                //con_Cambios = true;
            }

            if (_context.LandlordHeaderDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                LandlordHeaderDetailsHome land = new LandlordHeaderDetailsHome();
                land.CreatedDate = DateTime.Now;
                land.HousingListId = key;
                land.IdServiceDetail = servide_detail_id;
                LandlordHeaderDetailsHome hl = AddLandlordHeaderDetailsHomee(land);
               // _hosuging.LandlordHeaderDetailsHomes.Add(hl);
                //con_Cambios = true;
            }

            if (_context.DepartureDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList().Count < 1)
            {
                DepartureDetailsHome dep = new DepartureDetailsHome();
                dep.CreatedDate = DateTime.Now;
                dep.Id = key;
                dep.IdServiceDetail = servide_detail_id;
                DepartureDetailsHome hl =  AddDepartureDetailHome(dep);
               // _hosuging.DepartureDetailsHomes.Add(hl);
                //con_Cambios = true;
            }

            if (_context.GroupCostSavings.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                GroupCostSaving gc = new GroupCostSaving();
                gc.CreatedDate = DateTime.Now;
                gc.HousingListId = key;
                gc.IdServiceDetail = servide_detail_id;
                GroupCostSaving hl = AddGroupCostSaving(gc);
               // _hosuging.GroupCostSavings.Add(hl);
                //con_Cambios = true;
            }

            if (_context.GroupPaymnetsHousings.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                GroupPaymnetsHousing dep = new GroupPaymnetsHousing();
                dep.CreatedDate = DateTime.Now;
                dep.HousingListId = key;
                dep.IdServiceDetail = servide_detail_id;
                GroupPaymnetsHousing hl = AddGroupPaymnetsHousing(dep);
               // _hosuging.GroupPaymnetsHousings.Add(hl);
                //con_Cambios = true;
            }


            // Move In 
            if (_context.PropertyReports.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingList == key && r.PropertyInspection == 1).ToList().Count < 1)
            {
                biz.premier.Entities.PropertyReport pr = new biz.premier.Entities.PropertyReport();
                pr.CreatedDate = DateTime.Now;
                pr.HousingList = key;
                pr.PropertyInspection = 1;
                pr.IdServiceDetail = servide_detail_id;
                biz.premier.Entities.PropertyReport prt = AddPrpertyReport(pr);
                //dto.PropertyReports.Add(pr);
            }

            // Move Out
            if (_context.PropertyReports.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingList == key && r.PropertyInspection == 2).ToList().Count < 1)
            {
                biz.premier.Entities.PropertyReport pr = new biz.premier.Entities.PropertyReport();
                pr.CreatedDate = DateTime.Now;
                pr.HousingList = key;
                pr.PropertyInspection = 2;
                pr.IdServiceDetail = servide_detail_id;
                biz.premier.Entities.PropertyReport prt = AddPrpertyReport(pr);
            }


            // Inspection & Repairs 
            //if (_context.Inspections.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingList == key).ToList().Count < 1)
            //{
            //    biz.premier.Entities.Inspection ins = new biz.premier.Entities.Inspection();
            //    ins.CreatedDate = DateTime.Now;
            //    ins.HousingList = key;
            //    ins.IdServiceDetail = servide_detail_id;
            //    biz.premier.Entities.Inspection prt = AddInspection(ins);
            //}

            if (_context.GroupIrs.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                biz.premier.Entities.GroupIr gir = new biz.premier.Entities.GroupIr();
                gir.CreatedDate = DateTime.Now;
                gir.IdServiceDetail = servide_detail_id;
                gir.HousingListId = key;
                biz.premier.Entities.GroupIr gir1 = AddGroupIr(gir);
            }

            var _hosuging_resutl = _context.HousingLists.FirstOrDefault(x => x.Id == key);
            return _hosuging_resutl;
        }

        public bool set_payments_type(int key, int servide_detail_id) {

            bool respuesta = true;

            var hf_detail = _context.HomeFindings.FirstOrDefault(h => h.Id == servide_detail_id);

            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    Currency = s.Currency,
                    GroupPaymnetsHousings = _context.GroupPaymnetsHousings
                        .Include(o => o.PaymentHousings)
                    .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList()
                }).ToList();

            var paymnets_type_list = new List<PaymentHousing>();

            if (consult.Count > 0)
            {
                if (consult[0].GroupPaymnetsHousings.Count > 0)
                    paymnets_type_list = consult[0].GroupPaymnetsHousings[0].PaymentHousings.ToList();
                else
                    respuesta = false;
            }
            else
                respuesta = false; 

                if (hf_detail.SecurityDepositId > 0)
                {
                    if (paymnets_type_list.Where(d=> d.PaymentType == 3).ToList().Count < 1 )
                    {
                        PaymentHousing k = new PaymentHousing();
                        k.CreatedDate = DateTime.Now;
                        k.UpdatedDate = DateTime.Now;
                        k.Id = 0;
                        k.HousingList = key;
                        k.IdServiceDetail = servide_detail_id;
                        k.Currency = consult[0].Currency;
                        k.GroupPaymentsHousingId = consult[0].GroupPaymnetsHousings[0].Id;
                        k.PaymentType = 3;
                        k.Responsible = hf_detail.SecurityDepositId; 
                        _context.PaymentHousings.Add(k);
                        _context.SaveChanges();
                    }

                }

                if (hf_detail.OngoingRentPaymentId > 0)
                {
                    if (paymnets_type_list.Where(d => d.PaymentType == 2).ToList().Count < 1)
                    {
                        PaymentHousing k = new PaymentHousing();
                        k.CreatedDate = DateTime.Now;
                        k.UpdatedDate = DateTime.Now;
                        k.Id = 0;
                        k.HousingList = key;
                        k.IdServiceDetail = servide_detail_id;
                        k.Currency = consult[0].Currency;
                        k.GroupPaymentsHousingId = consult[0].GroupPaymnetsHousings[0].Id;
                        k.PaymentType = 1;
                        k.Responsible = hf_detail.OngoingRentPaymentId;
                        _context.PaymentHousings.Add(k);
                        _context.SaveChanges();
                    }

                }


                if (hf_detail.InitialRentPaymentId > 0)
                {
                    if (paymnets_type_list.Where(d => d.PaymentType == 1).ToList().Count < 1)
                    {
                        PaymentHousing k = new PaymentHousing();
                        k.CreatedDate = DateTime.Now;
                        k.UpdatedDate = DateTime.Now;
                        k.Id = 0;
                        k.HousingList = key;
                        k.IdServiceDetail = servide_detail_id;
                        k.Currency = consult[0].Currency;
                        k.GroupPaymentsHousingId = consult[0].GroupPaymnetsHousings[0].Id;
                        k.PaymentType = 2;
                        k.Responsible = hf_detail.InitialRentPaymentId;
                        _context.PaymentHousings.Add(k);
                        _context.SaveChanges();
                    }

                }

                if (hf_detail.RealtorCommissionId > 0)
                {
                    if (paymnets_type_list.Where(d => d.PaymentType == 4).ToList().Count < 1)
                    {
                        PaymentHousing k = new PaymentHousing();
                        k.CreatedDate = DateTime.Now;
                        k.UpdatedDate = DateTime.Now;
                        k.Id = 0;
                        k.HousingList = key;
                        k.IdServiceDetail = servide_detail_id;
                        k.Currency = consult[0].Currency;
                        k.GroupPaymentsHousingId = consult[0].GroupPaymnetsHousings[0].Id;
                        k.PaymentType = 4;
                        k.Responsible = hf_detail.RealtorCommissionId;
                        _context.PaymentHousings.Add(k);
                        _context.SaveChanges();
                    }

                }

            ContractDetail cd = _context.ContractDetails.FirstOrDefault(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key);
            if(cd != null)
            {
                Decimal listp = 0;
                Decimal finalp = 0;

                if (cd.ListRentPrice > 0)
                    listp = cd.ListRentPrice.Value;
                if (cd.FinalRentPrice > 0)
                    finalp = cd.FinalRentPrice.Value;
                cd.Rentcostsaving = listp - finalp;
                _context.ContractDetails.Update(cd);
                _context.SaveChanges();
            }

            return respuesta; 
        }

        public Attendee UpdateAttendee(Attendee inspection)
        {
            _context.Attendees.Update(inspection);
            _context.SaveChanges();
            return inspection;
        }

        public AttendeeInspec UpdateAttendeeInspec(AttendeeInspec inspection)
        {
            _context.AttendeeInspecs.Update(inspection);
            _context.SaveChanges();
            return inspection;
        }
        public ContractDetail UpdateContractDetail(ContractDetail contract)
        {
            var consult = _context.ContractDetails
                .Include(i => i.PropertyExpenses)
                .SingleOrDefault(s => s.IdContract == contract.IdContract);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(contract);
                foreach (var i in contract.PropertyExpenses)
                {
                    var property = consult.PropertyExpenses.Where(p => p.Id == i.Id).FirstOrDefault();
                    if (property == null)
                    {
                        consult.PropertyExpenses.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(property).CurrentValues.SetValues(i);
                    }
                }
                _context.SaveChanges();
            }
            return consult;
        }

        public CostSavingHome UpdateCostSanvingHome(CostSavingHome renewalDetail)
        {
            _context.CostSavingHomes.Update(renewalDetail);
            _context.SaveChanges();
            return renewalDetail;
        }

        public HousingList UpdateCustom(HousingList housingList, int key)
        {
            var consult = _context.HousingLists
                .Include(i => i.AmenitiesHousingLists)
                .SingleOrDefault(s => s.Id == key);
            if(consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(housingList);
                foreach (var i in housingList.AmenitiesHousingLists)
                {
                    var amenities = consult.AmenitiesHousingLists.Where(p => p.Amenitie == i.Amenitie).FirstOrDefault();
                    if (amenities == null)
                    {
                        consult.AmenitiesHousingLists.Add(i);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(amenities).CurrentValues.SetValues(i);
                    }
                }
                foreach(var i in housingList.DocumentHousings)
                {
                    var document = consult.DocumentHousings.SingleOrDefault(x => x.Id == i.Id);
                    if (document == null)
                    {
                        consult.DocumentHousings.Add(i);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(i);
                    }
                }
                foreach(var i in housingList.CommentHousings)
                {
                    var comment = consult.CommentHousings.SingleOrDefault(x => x.Id == i.Id);
                    if (comment == null)
                    {
                        consult.CommentHousings.Add(i);
                    }
                    else
                    {
                        _context.Entry(comment).CurrentValues.SetValues(i);
                    }
                }
                foreach (var i in housingList.HousingStatusHistories)
                {
                    var status = consult.HousingStatusHistories.SingleOrDefault(x => x.Id == i.Id);
                    if (status == null)
                    {
                        consult.HousingStatusHistories.Add(i);
                    }
                    else
                    {
                        _context.Entry(status).CurrentValues.SetValues(i);
                    }
                }

                if(housingList.HousingStatus == 7)
                {
                    var permanet_hms = _context.HousingLists.Where(s => s.IdServiceDetail == housingList.IdServiceDetail 
                                                                        && s.HousingStatus == 7 
                                                                        && s.Id != housingList.Id); //casas permamentes 
                    foreach (var i in permanet_hms)
                    {
                        i.HousingStatus = 5;
                    }
                }
                

                    _context.SaveChanges();
            }
            return consult;
        }

        public HousingList LogicDeleteHousing(int id)
        {
            var consult = _context.HousingLists.SingleOrDefault(s => s.Id == id);
            if (consult != null)
            {
                consult.IdServiceDetail = null;
                _context.HousingLists.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }
            return consult;
        }

        public DepartureDetailsHome UpdateDepartureDetailHome(DepartureDetailsHome departure)
        {
            _context.DepartureDetailsHomes.Update(departure);
            _context.SaveChanges();
            return departure;
        }

      

        public PhotosInspec getPhotoInspecById(int id)
        {
            PhotosInspec photo = _context.PhotosInspecs.FirstOrDefault(p => p.Id == id);
            return photo;
        }

        public List<int> UpdateSendPropertys(List<int> list)
        {
            foreach(int id_h in list)
            {
                var h = _context.HousingLists.FirstOrDefault(i => i.Id == id_h);
                h.HousingStatus = 9;
                h.Wassended = true;
                _context.HousingLists.Update(h);
                _context.SaveChanges();
            }

            return list;
        }

        public List<string> getdataasignado(List<string> list)
        {

            return list;
        }

        public KeyInventory UpdateKeyInventory(KeyInventory keyInventory)
        {
            _context.KeyInventories.Update(keyInventory);
            _context.SaveChanges();
            return keyInventory;
        }

       public  ActionResult save_ir_status(int status_id, int id)
        {
            var consult = _context.GroupIrs.SingleOrDefault(i=> i.Id == id);
            

            if (consult != null)
            {
                consult.IdStatus = status_id;
                _context.GroupIrs.Update(consult);
                _context.SaveChanges();
            }
            

            return new ObjectResult(consult);
        }


        public ActionResult save_ir_statusbyhousingid(int status_id, int type, int id_service_detail, int id_permanent_home)
        {
            if(type == 1)
            {
                var consult = _context.PropertyReports.SingleOrDefault(i => i.HousingList == id_permanent_home && i.IdServiceDetail == id_service_detail && i.PropertyInspection == 1);
                if (consult != null)
                {
                    consult.IdStatus = status_id;
                    _context.PropertyReports.Update(consult);
                    _context.SaveChanges();
                }
                return new ObjectResult(consult);
            }
            else if( type == 2)
            {
                var consult = _context.PropertyReports.SingleOrDefault(i => i.HousingList == id_permanent_home && i.IdServiceDetail == id_service_detail && i.PropertyInspection == 2);
                if (consult != null)
                {
                    consult.IdStatus = status_id;
                    _context.PropertyReports.Update(consult);
                    _context.SaveChanges();
                }
                return new ObjectResult(consult);
            }
            else
            {
                var consult = _context.GroupIrs.SingleOrDefault(i => i.HousingListId == id_permanent_home && i.IdServiceDetail == id_service_detail);
                if (consult != null)
                {
                    consult.IdStatus = status_id;
                    _context.GroupIrs.Update(consult);
                    _context.SaveChanges();
                }
                return new ObjectResult(consult);
            }
           
        }

        public LandlordDetailsHome UpdateLandlordDetailsHome(LandlordDetailsHome landlord)
        {
            var consult = _context.LandlordDetailsHomes
               // .Include(i => i.CreditCardLandLordDetails)
                .SingleOrDefault(s => s.IdLandlord == landlord.IdLandlord);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(landlord);

                //consult.CreditCardLandLordDetails.Clear();
                //_context.SaveChanges();
                //foreach (var o in landlord.CreditCardLandLordDetails)
                //{
                //    consult.CreditCardLandLordDetails.Add(o);
                //}

                _context.SaveChanges();
            }
            return consult;
        }


        public ActionResult DeleteBankingDetails(int id)
        {
            var consult = _context.LandlordDetailsHomes.SingleOrDefault(s => s.IdLandlord == id);
            var HeaderId = consult.HeaderId;
            if (consult != null)
            {
                consult.HeaderId = null;
                _context.LandlordDetailsHomes.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.LandlordDetailsHomes.Where(s => s.HeaderId == HeaderId).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult  DeleteExpense(int id) {

            var consult = _context.PropertyExpenses.SingleOrDefault(s => s.Id == id);
            var _Id = consult.ContractDetail;
            if (consult != null)
            {
                consult.ContractDetail = null;
                _context.PropertyExpenses.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.PropertyExpenses.Where(s => s.ContractDetail == _Id).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult DeletePaymnetType(int id)
        {
            var consult = _context.PaymentHousings.SingleOrDefault(s => s.Id == id);
            var _Id = consult.GroupPaymentsHousingId;
            if (consult != null)
            {
                consult.GroupPaymentsHousingId = null;
                _context.PaymentHousings.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.PaymentHousings.Where(s => s.GroupPaymentsHousingId == _Id).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult deletePropertyReportSection(int id)
        {
            var consult = _context.PropertyReportSections.SingleOrDefault(s => s.Id == id);
            var _Id = consult.PropertyReport;
            if (consult != null)
            {
                consult.PropertyReport = null;
                _context.PropertyReportSections.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.PropertyReportSections.Where(s => s.PropertyReport == _Id).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult AddPropertyReportSection(PropertyReportSection prs)
        {
            
            _context.PropertyReportSections.Add(prs);
            _context.SaveChanges();

            var list_all = _context.PropertyReportSections.Where(s => s.PropertyReport == prs.PropertyReport).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult deleteSectionInventory(int id)
        {
            var consult = _context.SectionInventories.SingleOrDefault(s => s.Id == id);
            var _Id = consult.PropertyReportSectionId;
            if (consult != null)
            {
                consult.PropertyReportSectionId = null;
                _context.SectionInventories.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.SectionInventories
                .Include(s => s.PhotosInventories)
                .Where(s => s.PropertyReportSectionId == _Id).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult AddSectionInventory(SectionInventory si)
        {
            _context.SectionInventories.Add(si);
            _context.SaveChanges();

            var list_all = _context.SectionInventories
                .Include(o=> o.PhotosInventories)
                .Where(s => s.PropertyReportSectionId == si.PropertyReportSectionId).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult EditSectionInventory(SectionInventory si)
        {
            var consult = _context.SectionInventories
                .Include(p=> p.PhotosInventories)
                .SingleOrDefault(s => s.Id == si.Id);

            consult.PhotosInventories = si.PhotosInventories;
            consult.Description = si.Description;
            consult.Quantity = si.Quantity;
            consult.Item = si.Item;

            if (consult != null)
            {
                //_context.Entry(consult).CurrentValues.SetValues(si);
                _context.SectionInventories.Update(consult);
                _context.SaveChanges();
            }

          

            var list_all = _context.SectionInventories
                .Include(o=> o.PhotosInventories)
                .Where(s => s.PropertyReportSectionId == si.PropertyReportSectionId).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult deletKeyInventory(int id)
        {
            var consult = _context.KeyInventories.SingleOrDefault(s => s.Id == id);
            var _Id = consult.PropertyReport;
            if (consult != null)
            {
                consult.PropertyReport = null;
                _context.KeyInventories.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.KeyInventories.Where(s => s.PropertyReport == _Id).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult deleteAttend(int id)
        {
            var consult = _context.Attendees.SingleOrDefault(s => s.Id == id);
            var _Id = consult.PropertyReport;
            if (consult != null)
            {
                consult.PropertyReport = null;
                _context.Attendees.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.Attendees.Where(s => s.PropertyReport == _Id).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult deleteCostSavings(int id)
        {
            var consult = _context.CostSavingHomes.SingleOrDefault(s => s.Id == id);
            var _Id = consult.GroupCostSavingId;
            if (consult != null)
            {
                consult.GroupCostSavingId = null;
                _context.CostSavingHomes.Update(consult);
                //_context.Entry(consult).CurrentValues.SetValues(i);
                _context.SaveChanges();
            }

            var list_all = _context.CostSavingHomes.Where(s => s.GroupCostSavingId == _Id).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult AddExpense(PropertyExpense pe)
        {
            _context.PropertyExpenses.Add(pe);
            _context.SaveChanges();
       

            var list_all = _context.PropertyExpenses.Where(s => s.ContractDetail == pe.ContractDetail).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult EditExpense(PropertyExpense pe)
        {

            var consult = _context.PropertyExpenses
                .SingleOrDefault(s => s.Id == pe.Id);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(pe);

                _context.SaveChanges();
            }


            var list_all = _context.PropertyExpenses.Where(s => s.ContractDetail == pe.ContractDetail).ToList();

            return new ObjectResult(list_all);
        }


        public ActionResult AddConsiderarion(SpecialConsideration pe)
        {
            _context.SpecialConsiderations.Add(pe);
            _context.SaveChanges();


            var list_all = _context.SpecialConsiderations.Where(s => s.ContractDetailId == pe.ContractDetailId).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult EditConsideration(SpecialConsideration pe)
        {

            var consult = _context.SpecialConsiderations
                .SingleOrDefault(s => s.Id == pe.Id);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(pe);

                _context.SaveChanges();
            }


            var list_all = _context.SpecialConsiderations.Where(s => s.ContractDetailId == pe.ContractDetailId).ToList();

            return new ObjectResult(list_all);
        }

        public ActionResult DeleteConsideration(int id)
        {

            var consult = _context.SpecialConsiderations
                .SingleOrDefault(s => s.Id == id);
            if (consult != null)
            {
                _context.Remove(consult);
                _context.SaveChanges();
            }


            var list_all = _context.SpecialConsiderations.Where(s => s.Id == id).ToList();

            return new ObjectResult(list_all);
        }

        public LandlordHeaderDetailsHome UpdateLandlordHeaderDetailsHome(LandlordHeaderDetailsHome landlord)
        {
            var consult = _context.LandlordHeaderDetailsHomes
                .Include(i => i.LandlordDetailsHomes)
                .Include(c => c.CreditCardLandLordDetails)
                .SingleOrDefault(s => s.Id == landlord.Id);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(landlord);
                consult.CreditCardLandLordDetails.Clear();
                _context.SaveChanges();
                foreach (var o in landlord.CreditCardLandLordDetails)
                {
                    consult.CreditCardLandLordDetails.Add(o);
                }

                _context.SaveChanges();
            }
            return consult;
        }


        public PaymentHousing UpdatePaymentHousing(PaymentHousing payment)
        {
            var consult = _context.PaymentHousings
                .SingleOrDefault(s => s.Id == payment.Id);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(payment);
                _context.SaveChanges();
            }

            return payment;
        }

        public PaymentRental UpdatePaymentRental(PaymentRental payment)
        {
            _context.PaymentRentals.Update(payment);
            _context.SaveChanges();
            return payment;
        }


        public PaymentSchooling UpdatePaymentSchooling(PaymentSchooling payment)
        {
            _context.PaymentSchoolings.Update(payment);
            _context.SaveChanges();
            return payment;
        }



        public RenewalDetailHome UpdateRenewalDetailHome(RenewalDetailHome renewalDetail)
        {
            _context.RenewalDetailHomes.Update(renewalDetail);
            _context.SaveChanges();
            return renewalDetail;
        }


        ///////////////////////////////////// FUNCIONES QUE MEJORAN EL PERFOMANCE DE LAS PROPIEDADES  /////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult GetOnlyPropertyDetails(int key, int servide_detail_id, int type)
        {
            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    Id = s.Id,
                    WorkOrder = s.WorkOrder,
                    Service = s.Service,
                    ServiceType = s.ServiceType,
                    PropertyNo = s.PropertyNo,
                    Sample = s.Sample,
                    SupplierPartner = s.SupplierPartner,
                    Supplier = s.Supplier,
                    VisitDate = s.VisitDate,
                    HousingStatus = s.HousingStatus,
                    PropertyType = s.PropertyType,
                    Address = s.Address,
                    Zip = s.Zip,
                    WebSite = s.WebSite,
                    Bedrooms = s.Bedrooms,
                    Bathrooms = s.Bathrooms,
                    ParkingSpaces = s.ParkingSpaces,
                    Price = s.Price,
                    Currency = s.Currency,
                    AdditionalComments = s.AdditionalComments,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    UpdateBy = s.UpdateBy,
                    UpdatedDate = s.UpdatedDate,
                    IdServiceDetail = s.IdServiceDetail,
                    Othersupplier = s.Othersupplier,
                    Suppliertelephone = s.Suppliertelephone,
                    Supplieremail = s.Supplieremail,
                    SupplierNavigation = s.SupplierNavigation,
                    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                    AmenitiesHousingLists = s.AmenitiesHousingLists,
                    CommentHousings = s.CommentHousings,
                    DocumentHousings = s.DocumentHousings,
                    Shared = s.Shared,
                    VisitTime = s.VisitTime,
                    Neighborhood = s.Neighborhood}).ToList();

            return new ObjectResult(consult[0]);
        }

        public ActionResult GetOnlyPropertyLSF(int key, int servide_detail_id, int type)
        {


                var result = set_payments_type(key, servide_detail_id); // crea los paymnets que conicidan con la seccion de payment process de HF 


          //  var set_hl = create_only_lsf_registry(key, servide_detail_id); // esto es por si algun motivo a la propiedad le falta alguna seccion del SLF o del I&R


            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                   
                    DepartureDetailsHomes = _context.DepartureDetailsHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    //departureHistoricDetailsHomes = (type == 26 ? new List<DepartureDetailsHome>() :
                    //                                                _context.DepartureDetailsHomes
                    //                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                    //                                                .OrderByDescending(h => h.CreatedDate).ToList()),// lista de pasados

                    RenewalDetailHomes = _context.RenewalDetailHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    //renewalHistoricDetailHomes = (type == 26 ? new List<RenewalDetailHome>() :
                    //                                            _context.RenewalDetailHomes
                    //                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                    //                                            .OrderByDescending(h => h.CreatedDate).ToList()),//lista de pasados

                    GroupCostSavings = _context.GroupCostSavings
                    .Include(o => o.CostSavingHomes)
                    .ThenInclude(x => x.CurrencyNavigation)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    //groupHistoricCostSavings = (type == 26 ? new List<GroupCostSaving>() :
                    //                                        _context.GroupCostSavings
                    //                                        .Include(o => o.CostSavingHomes)
                    //                                        .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                    //                                        .OrderByDescending(h => h.CreatedDate).ToList()), // lista de pasados

                    GroupPaymnetsHousings = _context.GroupPaymnetsHousings
                    .Include(o => o.PaymentHousings)
                    .ThenInclude(x => x.PaymentTypeNavigation)
                    .Include(o => o.PaymentHousings)
                    .ThenInclude(x => x.ResponsibleNavigation)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    //groupHistoricPaymnetsHousings = (type == 26 ? new List<GroupPaymnetsHousing>() :
                    //                                            _context.GroupPaymnetsHousings
                    //                                            .Include(o => o.PaymentHousings)
                    //                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                    //                                            .OrderByDescending(h => h.CreatedDate).ToList()),

                    ContractDetails = _context.ContractDetails
                    .Include(p => p.PropertyExpenses)
                    .Include(x => x.SpecialConsiderations)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),

                    //contractHistoricDetails = (type == 26 ? new List<ContractDetail>() :
                    //                                        _context.ContractDetails
                    //                                        .Include(p => p.PropertyExpenses)
                    //                                        .Where(r => r.IdServiceDetail != servide_detail_id && r.ContractDetailId == key)
                    //                                        .OrderByDescending(h => h.CreatedDate).ToList()),

                    LandlordHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                    .Include(p => p.LandlordDetailsHomes)
                    .Include(c => c.CreditCardLandLordDetails)
                // .ThenInclude(x => x.CreditCardLandLordDetails)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    //Landlor_HistoricHeaderDetailsHomes = (type == 26 ? new List<LandlordHeaderDetailsHome>() :
                    //                                            _context.LandlordHeaderDetailsHomes
                    //                                            .Include(p => p.LandlordDetailsHomes)
                    //                                            //  .ThenInclude(x => x.CreditCardLandLordDetails)
                    ////                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                    //                                            .OrderByDescending(h => h.CreatedDate).ToList()),

                  

                }).ToList();

            return new ObjectResult(consult[0]); ;
        }

        public ActionResult GetLSFBySection(int key, int servide_detail_id, int section)
        {

           // var set_hl = create_only_lsf_registry(key, servide_detail_id); // esto es por si algun motivo a la propiedad le falta alguna seccion del SLF o del I&R
           
            if(section == 1)
            {
                var result = set_payments_type(key, servide_detail_id); // crea los paymnets que conicidan con la seccion de payment process de HF

                var consult = _context.HousingLists
                    .Where(x => x.Id == key).Select(s => new
                    {
                        ContractDetails = _context.ContractDetails .Include(p => p.PropertyExpenses)
                        .Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),
                    }).ToList();

                return new ObjectResult(consult[0]); 
            }
            else if(section == 2)
                {
                    var consult = _context.HousingLists
                        .Where(x => x.Id == key).Select(s => new
                        {
                        GroupCostSavings = _context.GroupCostSavings  .Include(o => o.CostSavingHomes)  .ThenInclude(x => x.CurrencyNavigation)
                        .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),
                        }).ToList();

                    return new ObjectResult(consult[0]); ;
                }
            else if (section == 3)
            {
                var consult = _context.HousingLists
                    .Where(x => x.Id == key).Select(s => new
                    {
                        GroupPaymnetsHousings = _context.GroupPaymnetsHousings
                        .Include(o => o.PaymentHousings)
                        .ThenInclude(x => x.PaymentTypeNavigation)
                        .Include(o => o.PaymentHousings)
                        .ThenInclude(x => x.ResponsibleNavigation)
                    .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),
                    }).ToList();

                return new ObjectResult(consult[0]); ;
            }
            else if (section == 4)
            {
                var consult = _context.HousingLists
                    .Where(x => x.Id == key).Select(s => new
                    {
                        DepartureDetailsHomes = _context.DepartureDetailsHomes
                       .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    }).ToList();

                return new ObjectResult(consult[0]); ;
            }
            else if (section == 5)
            {
                var consult = _context.HousingLists
                    .Where(x => x.Id == key).Select(s => new
                    {

                     RenewalDetailHomes = _context.RenewalDetailHomes
                    .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    }).ToList();

                return new ObjectResult(consult[0]); ;
            }
            else if (section == 6)
            {
                var consult = _context.HousingLists
                    .Where(x => x.Id == key).Select(s => new
                    {

                        LandlordHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                        .Include(p => p.LandlordDetailsHomes)
                        .Include(c => c.CreditCardLandLordDetails)
                    .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    }).ToList();

                return new ObjectResult(consult[0]); ;
            }
            else
            {
                return new ObjectResult(null);
            }
        }

        public ActionResult GetOnlyPropertyInspRep(int key, int servide_detail_id, int type)
        {

            var set_hl = create_only_ir_registry(key, servide_detail_id); // esto es por si algun motivo a la propiedad le falta alguna seccion del SLF o del I&R
            var atributos_servicio = atributos_generales(servide_detail_id, type);

            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    PropertyReports = _context.PropertyReports  // MOVE IN - OUT
                                    .Include(i => i.PropertyReportSections)
                                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                                .Include(i => i.PropertyReportSections)
                                    .ThenInclude(i => i.SectionInventories)
                                        .ThenInclude(i => i.PhotosInventories)
                                .Include(i => i.KeyInventories)
                                .Include(i => i.Attendees)
                                .Where(r => r.HousingList == key
                                   && r.IdServiceDetail == servide_detail_id
                                   ).ToList(),

                    PropertyReportsHistoric = _context.PropertyReports  // Historic MOVE IN - OUT
                                    .Include(i => i.PropertyReportSections)
                                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                                .Include(i => i.PropertyReportSections)
                                    .ThenInclude(i => i.SectionInventories)
                                        .ThenInclude(i => i.PhotosInventories)
                                .Include(i => i.KeyInventories)
                                .Include(i => i.Attendees)
                                .Where(r => r.HousingList == key 
                                && r.IdServiceDetail != servide_detail_id
                               // && r.CreatedDate < atributos_servicio.creation
                                ).OrderByDescending(h => h.CreatedDate).ToList(),


                    groupIr = _context.GroupIrs
                    .Include(o => o.Repairs)
                        .ThenInclude(d => d.DocumentRepairs)
                    .Include(t => t.Inspections)
                        .ThenInclude(d => d.AttendeeInspecs)
                    .Include(t => t.Inspections)
                        .ThenInclude(d => d.PhotosInspecs)
                .Where(r => r.IdServiceDetail == servide_detail_id 
                    && r.HousingListId == key
                    ).ToList(),
                    //Repairs = s.Repairs.Where(r => r.HousingList == key).ToList(),

                    groupIrhistoric = (type == 26 ? new List<GroupIr>() :
                                                            _context.GroupIrs
                                                            .Include(o => o.Repairs)
                                                                 .ThenInclude(d => d.DocumentRepairs)
                                                            .Include(t => t.Inspections)
                                                                 .ThenInclude(d => d.AttendeeInspecs)
                                                            .Include(t => t.Inspections)
                                                                 .ThenInclude(d => d.PhotosInspecs)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id 
                                                            && r.HousingListId == key
                                                           // && r.CreatedDate < atributos_servicio.creation
                                                             ).OrderByDescending(h => h.CreatedDate).ToList()),


                }).ToList();

            return new ObjectResult(consult[0]); ;
        }


        public ActionResult GetInspRepBySection(int key, int servide_detail_id, int section)
        {

            if (section == 1)
            {
                var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    PropertyReports = _context.PropertyReports  // MOVE IN
                                                              
                    .Include(i => i.PropertyReportSections)
                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                    .Include(i => i.PropertyReportSections)
                        .ThenInclude(i => i.SectionInventories)
                        .ThenInclude(i => i.PhotosInventories)
                    .Include(i => i.KeyInventories)
                    .Include(i => i.Attendees)
                    .Where(r => r.HousingList == key && r.IdServiceDetail == servide_detail_id && r.PropertyInspection == 1).ToList(),

                }).ToList();

                return new ObjectResult(consult[0]); 
            }

            if (section == 2) // MOVE OUT
            {
                var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    PropertyReports = _context.PropertyReports  // MOVE IN - OUT
                    .Include(i => i.PropertyReportSections)
                    .ThenInclude(ti => ti.PhotosPropertyReportSections)
                    .Include(i => i.PropertyReportSections)
                        .ThenInclude(i => i.SectionInventories)
                        .ThenInclude(i => i.PhotosInventories)
                    .Include(i => i.KeyInventories)
                    .Include(i => i.Attendees)
                    .Where(r => r.HousingList == key && r.IdServiceDetail == servide_detail_id && r.PropertyInspection == 1).ToList()
                }).ToList();

                return new ObjectResult(consult[0]); 
            }
            if (section == 3) //INSP & REP
            {
                var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {

                    groupIr = _context.GroupIrs
                    .Include(o => o.Repairs)
                        .ThenInclude(d => d.DocumentRepairs)
                    .Include(t => t.Inspections)
                        .ThenInclude(d => d.AttendeeInspecs)
                    .Include(t => t.Inspections)
                        .ThenInclude(d => d.PhotosInspecs)
                    .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                }).ToList();

                return new ObjectResult(consult[0]); 
            }
            else
            {
                return new ObjectResult(null); 
            }
        }

        public string GetInspRepBySectionPrint(int key, int servide_detail_id, int section, string filepath)
        {

            var wo_id = _context.HousingLists.FirstOrDefault(a => a.Id == key).WorkOrder;

            det_grales_servicio det_grales_servicio = atributos_generales(servide_detail_id, 26);

            var sr_id = 0;

            if (wo_id != null)
            {
                   sr_id = _context.WorkOrders.FirstOrDefault(a => a.Id == wo_id).ServiceRecordId.Value ;
            }
            
            var house_data_list = _context.HousingLists.Where(x => x.Id == key).Select(s => new
            { 
             s.Id
             ,LandLord = s.LandlordHeaderDetailsHomes.FirstOrDefault(l=> l.HousingListId == key && l.IdServiceDetail == servide_detail_id) != null ?
                         s.LandlordHeaderDetailsHomes.FirstOrDefault(l => l.HousingListId == key && l.IdServiceDetail == servide_detail_id).Name : "Data not captured"
             ,Address = s.Address
             ,City_Country = det_grales_servicio != null ?  det_grales_servicio.location + ", " + det_grales_servicio.country_name : "Data not captured"
             ,Tenant = _context.AssigneeInformations.FirstOrDefault(z=> z.ServiceRecordId == sr_id).AssigneeName  
            }
                ).ToList();

            var house_data = house_data_list[0];

            string xKey = "Mgo+DSMBaFt/QHJqVVhkX1pFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9jQXxTd01mXH1ecH1WRQ==;Mgo+DSMBMAY9C3t2VVhiQlFaclxJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1hWX5WdXFXRmFcVUc=";

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(xKey);

            string returnName = $"Files/Pdf/1_{key}_{servide_detail_id}_{section}.pdf";

            filepath = Path.GetFullPath($"Files/Pdf/{key}_{servide_detail_id}_{section}.pdf");
            string filedel = Path.GetFullPath(returnName);
            if (System.IO.File.Exists(filedel))
                System.IO.File.Delete(filedel);

            if (section == 1)
            {
                var consult = _context.PropertyReports.Select(r => new
                {
                    r.Id,
                    PropertyInspectionText = r.PropertyInspectionNavigation.PropertyInspection,
                    PropertyInspection = r.PropertyInspection,
                    r.ReportDate, // Move In Date 
                    r.HousingList,
                    r.PropertyAddress,
                    r.ZipCode,
                    r.Notes,
                    CreatedByText = r.CreatedBy.HasValue ? r.CreatedByNavigation.Name + " " + r.CreatedByNavigation.LastName : "No filled",
                    r.CreatedBy,
                    r.CreatedDate,
                    r.UpdatedBy,
                    r.UpdatedDate,
                    r.IdServiceDetail,
                    status = r.IdStatus.HasValue ? r.IdStatusNavigation.Status : "In Process",
                    PropertyReportSections = _context.PropertyReportSections.Select(ps => new
                    {
                        ps.Id,
                        ps.PropertyReport,
                        ps.PropertySection,
                        PropertySectionText = ps.PropertySection.HasValue ? ps.PropertySectionNavigation.PropertySection : "Not Selected",
                        StatusText = ps.Status.HasValue ? ps.StatusNavigation.Status : "Not Selected",
                        ps.Status,
                        ps.NoneAction,
                        ps.NeedRepair,
                        ps.NeedClean,
                        ps.NeedReplace,
                        ps.ReportDate,
                        ps.ReportDetails,
                        ps.CreatedBy,
                        ps.CreatedDate,
                        ps.UpdatedBy,
                        ps.UpdatedDate,
                        ps.PhotosPropertyReportSections,
                        SectionInventories = _context.SectionInventories.Select(si => new
                        {
                            photos = si.PhotosInventories.Count,
                            si.Id,
                            si.PropertyReportSectionId,
                            si.Description,
                            si.Item,
                            itemText = si.ItemNavigation.Item,
                            si.Quantity,
                        }).Where(x => x.PropertyReportSectionId == ps.Id).ToList()

                    }).Where(ps => ps.PropertyReport == r.Id).ToList()
                })
                .Where(r => r.HousingList == key && r.IdServiceDetail == servide_detail_id && r.PropertyInspection == 1).ToList();

               

                
                // string htmlText = System.IO.File.ReadAllText(Path.GetFullPath("./images/") + "pruebaHTML.html");
                string htmlText = System.IO.File.ReadAllText(Path.GetFullPath("images/pruebaHTML.html"));
                //string baseUrl = Path.GetFullPath("./images/");
                string baseUrl = Path.GetFullPath("images/");

                //Modificar el documento

                string _date1 = "not dilled";
                if (consult[0].ReportDate != null)
                    _date1 = consult[0].ReportDate.Value.ToString("yyyy/MM/dd");

                htmlText = htmlText.Replace("@VarTenant", house_data.Tenant);
                htmlText = htmlText.Replace("@VarLandlord", house_data.LandLord);
                htmlText = htmlText.Replace("@VarAddress", house_data.Address);
                htmlText = htmlText.Replace("@VarCity", house_data.City_Country);
                htmlText = htmlText.Replace("@VarDate", _date1);
                string xSection = "";
                foreach (var item in consult[0].PropertyReportSections)
                {
                    string ColorAction = "";
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{item.PropertySectionText}</td></tr>";
                    xSection += "<tr>";
                    xSection += "<td>";
                    xSection += "<table  class=\"sinTabla\">";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += $"<tr class=\"sinTabla\"><td class=\"sinTabla\"><p><label class=\"negrita\">Status</label><input class=\"w3-input\" type=\"text\" value=\"{item.StatusText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Condition</label><input class=\"w3-input\" type=\"text\" value=\"{"Condicion"}\"></p></td>";
                    xSection += "<td colspan=\"2\" class=\"sinTabla\" align=\"center\" style=\"padding-left:5px!important; padding-right:5px!important; \">";
                    xSection += "<p>";
                    xSection += "<label class=\"negrita\">Actions</label>";

                    if (item.NoneAction.HasValue == false ? false : item.NoneAction.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i>";
                        xSection += "<label>Ready</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    if (item.NeedClean.HasValue == false ? false : item.NeedClean.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i>";
                        xSection += "<label>Clean</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    if (item.NeedRepair.HasValue == false ? false : item.NeedRepair.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i>";
                        xSection += "<label>Repare</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    if (item.NeedReplace.HasValue == false ? false : item.NeedReplace.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i>";
                        xSection += "<label>Replace</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    xSection += "</p>";
                    xSection += "</td>";
                    xSection += "</tr>";

                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr>";
                    xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<p>";
                    xSection += "<label class=\"negrita\">Reports Details</label>";
                    xSection += $"<textarea class=\"w3-input\" type=\"text\" value=\"{item.ReportDetails}\"></textarea>";
                    xSection += "</p>";
                    xSection += "</td>";
                    xSection += "</tr>";

                    //Seccion Fotos
                    // Si hay fotos se pinta el titulo
                    if (item.PhotosPropertyReportSections.Count > 0)
                    {
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Photos</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                    }

                    int ContadorPhotos = 0;
                    int ContadorGeneral = 0;
                    int numSeccionesFotos = 0;
                    foreach (var photo in item.PhotosPropertyReportSections)
                    {
                        if (ContadorGeneral <= 12)
                        {
                            int TotalPhotos = item.PhotosPropertyReportSections.Count;
                            if (ContadorPhotos == 0)
                            {
                                numSeccionesFotos++;
                                xSection += "<tr>";
                            }

                            string pathPhoto = Path.Combine(Path.GetFullPath("./"), photo.Photo);
                            if (System.IO.File.Exists(pathPhoto))
                            {
                                ContadorPhotos++;
                                ContadorGeneral++;
                                FileInfo fileInfo = new FileInfo(pathPhoto);
                                System.IO.File.WriteAllBytes(Path.Combine(baseUrl, fileInfo.Name), System.IO.File.ReadAllBytes(pathPhoto));

                                xSection += $"<td class=\"sinTabla\"><img class=\"sizePhoto\" src=\"{fileInfo.Name}\"></img></td>";

                                if (ContadorPhotos == 4 || TotalPhotos == ContadorGeneral)
                                {
                                    xSection += "</tr>";
                                    ContadorPhotos = 0;
                                }

                            }
                        }
                    }

                    //Termina Fotos
                    //Espacio de 120px al final de la hoja
                    if (numSeccionesFotos == 3)
                        xSection += "<tr><td class=\"sinTabla\" height=\"20px\"></td></tr>";
                    else if (numSeccionesFotos == 2)
                        xSection += "<tr><td class=\"sinTabla\" height=\"60px\"></td></tr>";
                    else
                        xSection += "<tr><td class=\"sinTabla\" height=\"0px\"></td></tr>";

                    //Seccion  Inventario
                    if (item.SectionInventories.Count > 0)
                    {
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Inventory</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<table class=\"inventoryTable\">";
                        xSection += "<tr><th>Item</th><th>Descripcion</th><th>Quantity</th><th>Photos</th></tr>";
                    }
                    foreach (var inventario in item.SectionInventories)
                    {
                        xSection += $"<tr><td>{inventario.itemText}</td><td>{inventario.Description}</td><td>{inventario.Quantity}</td><td>{inventario.photos}</td></tr>";
                    }

                    if (item.SectionInventories.Count <= 10)
                        xSection += "<tr><td class=\"sinTabla\" height=\"60px\"></td></tr>";
                    //Fin Seccion Inventario
                    xSection += "</table></table></td></tr>";
                    xSection += "</table>";

                }
                xSection += "</body>";
                xSection += "</html>";
                htmlText = htmlText.Replace("@Datos", xSection);
                /*
                htmlText = htmlText.Replace("@VarTitulo", "Living Room");
                htmlText = htmlText.Replace("@VarStatus", "");
                htmlText = htmlText.Replace("@VarCondition", "");

                htmlText = htmlText.Replace("@VarNothing", "red");
                htmlText = htmlText.Replace("@VarClean", "green");
                htmlText = htmlText.Replace("@VarRepare", "green");
                htmlText = htmlText.Replace("@VarReplace", "green");

                htmlText = htmlText.Replace("@VarPhoto1", "<td class=\"sinTabla\"></img></td>");
                htmlText = htmlText.Replace("@VarPhoto2", "<td class=\"sinTabla\"></img></td>");
                htmlText = htmlText.Replace("@VarPhoto3", "<td class=\"sinTabla\"></img></td>");

                htmlText = htmlText.Replace("@VarInventory", "<tr><td></td></tr>");
                htmlText = htmlText.Replace("@TableFor", "");*/


                //Initialize HTML to PDF converter with Blink rendering engine.
                HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);
                BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
                //blinkConverterSettings.AdditionalDelay = 30000;
                blinkConverterSettings.EnableJavaScript = true;
                blinkConverterSettings.Margin.All = 15;
                blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(1080, 0);
                //Assign Blink converter settings to HTML converter.
                blinkConverterSettings.TempPath = "C://tempoSYNC";
                htmlConverter.ConverterSettings = blinkConverterSettings;
                PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);
                document.Compression = PdfCompressionLevel.BestSpeed;
                //Save and close the PDF document.
                FileStream fileStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.ReadWrite);
                //Save and close the PDF document.
                document.Save(fileStream);
                document.Close(true);
                fileStream.Dispose();
                document.Dispose();
                // Reducir tamaño
                Byte[] bytes = System.IO.File.ReadAllBytes(filepath);
                FileInfo fileInfo_2 = new FileInfo(filepath);
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(bytes);
                loadedDocument.Compression = PdfCompressionLevel.BestSpeed;
                //Save the PDF document
                string nuevoNombre = Path.Combine(fileInfo_2.DirectoryName, "1_" + fileInfo_2.Name);
                fileStream = new FileStream(nuevoNombre, FileMode.CreateNew, FileAccess.ReadWrite);
                loadedDocument.Save(fileStream);
                //Close the document
                loadedDocument.Close(true);
                fileStream.Dispose();

                //bytes = System.IO.File.ReadAllBytes(nuevoNombre);
                //String base64 = Convert.ToBase64String(bytes);

                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);

                return returnName;
            }
            else if (section == 3)
            {


                // Nuevo PDF Inspection Repair

                //var consults = _context.GroupIrs
                //    .Include(t => t.Inspections)
                //        .ThenInclude(d => d.AttendeeInspecs)
                //    .Include(t => t.Inspections)
                //        .ThenInclude(d => d.PhotosInspecs)
                //    .Include(o => o.Repairs)
                //        .ThenInclude(d => d.DocumentRepairs)
                //    .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList();

                var consult = _context.GroupIrs.Select(r => new
                {
                    r.Id,
                    r.IdServiceDetail,
                    r.HousingListId,
                    InspecsSections = _context.Inspections.Select(ps => new
                    {
                        ps.Id,
                        ps.PropertySection,
                        ps.HousingList,
                        PropertySectionText = ps.PropertySection.HasValue ? ps.PropertySectionNavigation.PropertySection : "Not Selected",
                        ps.InspectType,
                        InspectTypeText = "",
                        ps.InitialInspectionDate,
                        Attendees = _context.AttendeeInspecs.Select(si => new
                        {
                            si.Att,
                            si.Email,
                            si.Name,
                            si.Inspection
                        }).Where(x => x.Inspection == ps.Id).ToList(),
                        Photos = _context.PhotosInspecs.Select(ph => new
                        {
                            ph.Id,
                            ph.Photo,
                            ph.PhotoName,
                            ph.Inspection
                        }).Where(x => x.Inspection == ps.Id).ToList(),
                        ps.InspecDetails,

                    }).Where(ps => ps.HousingList == r.HousingListId).ToList(),
                    Repairs = _context.Repairs.Select(re => new
                    {
                        re.HousingList,
                        re.Id,
                        re.RepairType,
                        RepairTypeText = re.RepairType.HasValue ? re.RepairTypeNavigation.RepairType : "Not Selected",
                        re.PaymentResponsibility,
                        PaymentResponsibilityText = re.PaymentResponsibility.HasValue ? re.PaymentResponsibilityNavigation.Responsable : "Not Selected",
                        re.TotalDays,
                        datestart = re.RepairStartDate.HasValue ? re.RepairStartDate.Value.ToString("dd/MM/yyyy") : "",
                        dateend = re.RepairEndDate.HasValue ? re.RepairEndDate.Value.ToString("dd/MM/yyyy") : "",
                        re.TotalCostRepair,
                        re.Currency,
                        CurrencyText = re.Currency.HasValue ? re.CurrencyNavigation.Currency : "Not Selected",
                        re.SupplierPartner,
                        SupplierPartnerText = "",
                        re.Comments,
                        Documents = _context.DocumentRepairs.Select(doc => new
                        {
                            doc.Id,
                            doc.FileName,
                            doc.CreatedDate,
                            doc.RepairId
                        }).Where(x => x.RepairId == re.Id).ToList()
                    }).Where(t => t.HousingList == key).ToList()
            }).Where(r => r.HousingListId == key && r.IdServiceDetail == servide_detail_id).ToList();

                string htmlText = System.IO.File.ReadAllText(Path.GetFullPath("./images/") + "InspectionRepair.html");
                string baseUrl = Path.GetFullPath("./images/");

                //Modificar el documento
                htmlText = htmlText.Replace("@VarTenant", house_data.Tenant);
                htmlText = htmlText.Replace("@VarLandlord", house_data.LandLord);
                htmlText = htmlText.Replace("@VarAddress", house_data.Address);
                htmlText = htmlText.Replace("@VarCity", house_data.City_Country);
                string xSection = "";
                foreach (var item in consult[0].InspecsSections)
                {
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{"Inspection"}</td></tr>";
                    xSection += "<tr>";
                    xSection += "<td class=\"sinTabla\">";
                    xSection += "<table  class=\"sinTabla\">";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += $"<tr class=\"sinTabla\"><td class=\"sinTabla\"><p><label class=\"negrita\">Property Section</label><input class=\"w3-input\" type=\"text\" value=\"{item.PropertySectionText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Inspection Type</label><input class=\"w3-input\" type=\"text\" value=\"{item.InspectTypeText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Inspection Date</label><input class=\"w3-input\" type=\"text\" value=\"{item.InitialInspectionDate}\"></p></td>";
                    xSection += "</tr>";

                    //xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>"; // Espacio en tabla

                    //Seccion Attendes
                    // Si hay Attendes se pinta el titulo
                    if (item.Attendees.Count > 0)
                    {
                        //Titulo
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Attendees</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                        //Se termina Titulo
                        //Inicia Cabecera tabla
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<table class=\"inventoryTable\">";
                        xSection += "<tr><th>Realtor</th><th>Name Realtor 1B</th><th>Email</th></tr>";
                        //Termina Cabecera tabla
                        foreach (var att in item.Attendees)
                        {
                            xSection += $"<tr><td>{att.Att}</td><td>{att.Name}</td><td>{att.Email}</td></tr>";
                        }
                        xSection += "</table>";
                        xSection += "</td></tr>";
                    }

                    //Photos
                    // Si hay Photos se pinta el titulo
                    if (item.Photos.Count > 0)
                    {
                        //Titulo
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Photos</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                        //Se termina Titulo
                    }
                    int ContadorPhotos = 0;
                    int ContadorGeneral = 0;
                    int numSeccionesFotos = 0;
                    foreach (var photo in item.Photos)
                    {
                        if (ContadorGeneral <= 12)
                        {
                            int TotalPhotos = item.Photos.Count;
                            if (ContadorPhotos == 0)
                            {
                                numSeccionesFotos++;
                                xSection += "<tr>";
                            }

                            string pathPhoto = Path.Combine(Path.GetFullPath("./"), photo.Photo);
                            if (System.IO.File.Exists(pathPhoto))
                            {
                                ContadorPhotos++;
                                ContadorGeneral++;
                                FileInfo fileInfo = new FileInfo(pathPhoto);
                                System.IO.File.WriteAllBytes(Path.Combine(baseUrl, fileInfo.Name), System.IO.File.ReadAllBytes(pathPhoto));

                                xSection += $"<td class=\"sinTabla\"><img class=\"sizePhoto\" src=\"{fileInfo.Name}\"></img></td>";

                                if (ContadorPhotos == 4 || TotalPhotos == ContadorGeneral)
                                {
                                    xSection += "</tr>";
                                    ContadorPhotos = 0;
                                }
                            }
                        }
                    }

                    //Termina Fotos
                    //Espacio de 120px al final de la hoja
                    if (numSeccionesFotos == 3)
                        xSection += "<tr><td class=\"sinTabla\" height=\"20px\"></td></tr>";
                    else if (numSeccionesFotos == 2)
                        xSection += "<tr><td class=\"sinTabla\" height=\"60px\"></td></tr>";
                    else
                        xSection += "<tr><td class=\"sinTabla\" height=\"0px\"></td></tr>";

                    //Comments
                    xSection += "<tr>";
                    xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<p>";
                    xSection += "<label class=\"negrita\">Notes</label>";
                    xSection += $"<textarea class=\"w3-input\" type=\"text\" value=\"{item.InspecDetails}\"></textarea>";
                    xSection += "</p>";
                    xSection += "</td>";
                    xSection += "</tr>";
                    xSection += "</table></table></td></tr>";

                    //xSection += "</table></table></td></tr>";
                    //xSection += "</table>";
                    xSection += "<div><br><br><br></div>"; // Espacio en tabla
                }
                foreach (var item in consult[0].Repairs)
                {
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{"Repairs"}</td></tr>";
                    xSection += "<tr>";
                    xSection += "<td class=\"sinTabla\">";
                    xSection += "<table  class=\"sinTabla\">";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += $"<tr class=\"sinTabla\"><td class=\"sinTabla\"><p><label class=\"negrita\">Repair Type</label><input class=\"w3-input\" type=\"text\" value=\"{item.RepairTypeText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Payment Responsibility</label><input class=\"w3-input\" type=\"text\" value=\"{item.PaymentResponsibilityText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Total Days</label><input class=\"w3-input\" type=\"text\" value=\"{item.TotalDays}\"></p></td>";
                    xSection += "</tr>";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += $"<tr class=\"sinTabla\"><td class=\"sinTabla\"><p><label class=\"negrita\">Repair Start Date</label><input class=\"w3-input\" type=\"text\" value=\"{item.datestart}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Repair End Date</label><input class=\"w3-input\" type=\"text\" value=\"{item.dateend}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Total Cost of Repair</label><input class=\"w3-input\" type=\"text\" value=\"{item.TotalCostRepair}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Currency</label><input class=\"w3-input\" type=\"text\" value=\"{item.Currency}\"></p></td>";
                    xSection += "</tr>";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += $"<tr class=\"sinTabla\"><td class=\"sinTabla\"><p><label class=\"negrita\">Supplier Partner</label><input class=\"w3-input\" type=\"text\" value=\"{item.SupplierPartnerText}\"></p></td>";
                    xSection += "</tr>";

                    //Comments
                    xSection += "<tr><td colspan=\"3\" class=\"sinTabla\"><br><br></td></tr>";
                    xSection += "<tr>";
                    xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<p>";
                    xSection += "<label class=\"negrita\">Comments</label>";
                    xSection += $"<textarea class=\"w3-input\" type=\"text\" value=\"{item.Comments}\"></textarea>";
                    xSection += "</p>";
                    xSection += "</td>";
                    xSection += "</tr>";

                    //Seccion Attendes
                    // Si hay Attendes se pinta el titulo
                    if (item.Documents.Count > 0)
                    {
                        //Titulo
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Upload Documents</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                        //Se termina Titulo
                        //Inicia Cabecera tabla
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<table class=\"inventoryTable\">";
                        xSection += "<tr><th>Document Name</th><th>Date</th></tr>";
                        //Termina Cabecera tabla
                        foreach (var att in item.Documents)
                        {
                            xSection += $"<tr><td>{att.FileName}</td><td>{att.CreatedDate}</td></tr>";
                        }
                        xSection += "</table>";
                    }

                    
                    xSection += "</table></table></td></tr>";

                    //xSection += "</table></table></td></tr>";
                    //xSection += "</table>";
                    xSection += "<div><br><br><br></div>"; // Espacio en tabla
                }
                xSection += "</body>";
                xSection += "</html>";
                htmlText = htmlText.Replace("@Datos", xSection);

                //Initialize HTML to PDF converter with Blink rendering engine.
                HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);
                
                BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
                //blinkConverterSettings.AdditionalDelay = 30000;
                blinkConverterSettings.EnableJavaScript = true;
                blinkConverterSettings.Margin.All = 15;
                blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(1080, 0);
                //Assign Blink converter settings to HTML converter.
                blinkConverterSettings.TempPath = "C://tempoSYNC";
                htmlConverter.ConverterSettings = blinkConverterSettings;
                PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);
                document.Compression = PdfCompressionLevel.BestSpeed;
                //Save and close the PDF document.
                FileStream fileStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.ReadWrite);
                //Save and close the PDF document.
                document.Save(fileStream);
                document.Close(true);
                fileStream.Dispose();
                document.Dispose();
                // Reducir tamaño
                Byte[] bytes = System.IO.File.ReadAllBytes(filepath);
                FileInfo fileInfo_2 = new FileInfo(filepath);
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(bytes);
                loadedDocument.Compression = PdfCompressionLevel.BestSpeed;
                //Save the PDF document
                string nuevoNombre = Path.Combine(fileInfo_2.DirectoryName, "1_" + fileInfo_2.Name);
                fileStream = new FileStream(nuevoNombre, FileMode.CreateNew, FileAccess.ReadWrite);
                loadedDocument.Save(fileStream);
                //Close the document
                loadedDocument.Close(true);
                fileStream.Dispose();

                //bytes = System.IO.File.ReadAllBytes(nuevoNombre);
                //String base64 = Convert.ToBase64String(bytes);

                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);

                return returnName;
            }
            else if (section == 2)
            {
                var consult = _context.PropertyReports.Select(r => new
                {
                    r.Id,
                    PropertyInspectionText = r.PropertyInspectionNavigation.PropertyInspection,
                    PropertyInspection = r.PropertyInspection,
                    r.ReportDate, // Move In Date 
                    r.HousingList,
                    r.PropertyAddress,
                    r.ZipCode,
                    r.Notes,
                    CreatedByText = r.CreatedBy.HasValue ? r.CreatedByNavigation.Name + " " + r.CreatedByNavigation.LastName : "No filled",
                    r.CreatedBy,
                    r.CreatedDate,
                    r.UpdatedBy,
                    r.UpdatedDate,
                    r.IdServiceDetail,
                    status = r.IdStatus.HasValue ? r.IdStatusNavigation.Status : "In Process",
                    PropertyReportSections = _context.PropertyReportSections.Select(ps => new
                    {
                        ps.Id,
                        ps.PropertyReport,
                        ps.PropertySection,
                        PropertySectionText = ps.PropertySection.HasValue ? ps.PropertySectionNavigation.PropertySection : "Not Selected",
                        StatusText = ps.Status.HasValue ? ps.StatusNavigation.Status : "Not Selected",
                        ps.Status,
                        ps.NoneAction,
                        ps.NeedRepair,
                        ps.NeedClean,
                        ps.NeedReplace,
                        ps.ReportDate,
                        ps.ReportDetails,
                        ps.CreatedBy,
                        ps.CreatedDate,
                        ps.UpdatedBy,
                        ps.UpdatedDate,
                        ps.PhotosPropertyReportSections,
                        SectionInventories = _context.SectionInventories.Select(si => new
                        {
                            photos = si.PhotosInventories.Count,
                            si.Id,
                            si.PropertyReportSectionId,
                            si.Description,
                            si.Item,
                            itemText = si.ItemNavigation.Item,
                            si.Quantity,
                        }).Where(x => x.PropertyReportSectionId == ps.Id).ToList()

                    }).Where(ps => ps.PropertyReport == r.Id).ToList()
                })
                   .Where(r => r.HousingList == key && r.IdServiceDetail == servide_detail_id && r.PropertyInspection == 1).ToList();

                //StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/pruebaHTML.html"));
                // string htmlText = System.IO.File.ReadAllText(Path.GetFullPath("./images/") + "pruebaHTML.html");
                string htmlText = System.IO.File.ReadAllText(Path.GetFullPath("images/pruebaHTML.html"));
                string baseUrl = Path.GetFullPath("images/");


                string _date1 = "not dilled";
                if (consult[0].ReportDate != null)
                    _date1 = consult[0].ReportDate.Value.ToString("yyyy/MM/dd");

                htmlText = htmlText.Replace("@VarTenant", house_data.Tenant);
                htmlText = htmlText.Replace("@VarLandlord", house_data.LandLord);
                htmlText = htmlText.Replace("@VarAddress", house_data.Address);
                htmlText = htmlText.Replace("@VarCity", house_data.City_Country);
                htmlText = htmlText.Replace("@VarDate", _date1);
                string xSection = "";
                foreach (var item in consult[0].PropertyReportSections)
                {
                    string ColorAction = "";
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{item.PropertySectionText}</td></tr>";
                    xSection += "<tr>";
                    xSection += "<td>";
                    xSection += "<table  class=\"sinTabla\">";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += $"<tr class=\"sinTabla\"><td class=\"sinTabla\"><p><label class=\"negrita\">Status</label><input class=\"w3-input\" type=\"text\" value=\"{item.StatusText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Condition</label><input class=\"w3-input\" type=\"text\" value=\"{"Condicion"}\"></p></td>";
                    xSection += "<td colspan=\"2\" class=\"sinTabla\" align=\"center\" style=\"padding-left:5px!important; padding-right:5px!important; \">";
                    xSection += "<p>";
                    xSection += "<label class=\"negrita\">Actions</label>";

                    if (item.NoneAction.HasValue == false ? false : item.NoneAction.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i>";
                        xSection += "<label>Ready</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    if (item.NeedClean.HasValue == false ? false : item.NeedClean.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i>";
                        xSection += "<label>Clean</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    if (item.NeedRepair.HasValue == false ? false : item.NeedRepair.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i>";
                        xSection += "<label>Repare</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    if (item.NeedReplace.HasValue == false ? false : item.NeedReplace.Value == true ? true : false)
                    {
                        xSection += "<div style=\"display: inline-block\">";
                        xSection += "<div class=\"sub-menu\">";
                        xSection += "<i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i>";
                        xSection += "<label>Replace</label>";
                        xSection += "</div>";
                        xSection += "</div>";
                    }

                    xSection += "</p>";
                    xSection += "</td>";
                    xSection += "</tr>";

                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr>";
                    xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<p>";
                    xSection += "<label class=\"negrita\">Reports Details</label>";
                    xSection += $"<textarea class=\"w3-input\" type=\"text\" value=\"{item.ReportDetails}\"></textarea>";
                    xSection += "</p>";
                    xSection += "</td>";
                    xSection += "</tr>";

                    //Seccion Fotos
                    // Si hay fotos se pinta el titulo
                    if (item.PhotosPropertyReportSections.Count > 0)
                    {
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Photos</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                    }

                    int ContadorPhotos = 0;
                    int ContadorGeneral = 0;
                    int numSeccionesFotos = 0;
                    foreach (var photo in item.PhotosPropertyReportSections)
                    {
                        if (ContadorGeneral <= 12)
                        {
                            int TotalPhotos = item.PhotosPropertyReportSections.Count;
                            if (ContadorPhotos == 0)
                            {
                                numSeccionesFotos++;
                                xSection += "<tr>";
                            }

                            string pathPhoto = Path.Combine(Path.GetFullPath("./"), photo.Photo);
                            if (System.IO.File.Exists(pathPhoto))
                            {
                                ContadorPhotos++;
                                ContadorGeneral++;
                                FileInfo fileInfo = new FileInfo(pathPhoto);
                                System.IO.File.WriteAllBytes(Path.Combine(baseUrl, fileInfo.Name), System.IO.File.ReadAllBytes(pathPhoto));

                                xSection += $"<td class=\"sinTabla\"><img class=\"sizePhoto\" src=\"{fileInfo.Name}\"></img></td>";

                                if (ContadorPhotos == 4 || TotalPhotos == ContadorGeneral)
                                {
                                    xSection += "</tr>";
                                    ContadorPhotos = 0;
                                }

                            }
                        }
                    }

                    //Termina Fotos
                    //Espacio de 120px al final de la hoja
                    if (numSeccionesFotos == 3)
                        xSection += "<tr><td class=\"sinTabla\" height=\"20px\"></td></tr>";
                    else if (numSeccionesFotos == 2)
                        xSection += "<tr><td class=\"sinTabla\" height=\"60px\"></td></tr>";
                    else
                        xSection += "<tr><td class=\"sinTabla\" height=\"0px\"></td></tr>";

                    //Seccion  Inventario
                    if (item.SectionInventories.Count > 0)
                    {
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Inventory</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<table class=\"inventoryTable\">";
                        xSection += "<tr><th>Item</th><th>Descripcion</th><th>Quantity</th><th>Photos</th></tr>";
                    }
                    foreach (var inventario in item.SectionInventories)
                    {
                        xSection += $"<tr><td>{inventario.itemText}</td><td>{inventario.Description}</td><td>{inventario.Quantity}</td><td>{inventario.photos}</td></tr>";
                    }

                    if (item.SectionInventories.Count <= 10)
                        xSection += "<tr><td class=\"sinTabla\" height=\"60px\"></td></tr>";
                    //Fin Seccion Inventario
                    xSection += "</table></table></td></tr>";
                    xSection += "</table>";

                }
                xSection += "</body>";
                xSection += "</html>";
                htmlText = htmlText.Replace("@Datos", xSection);



                HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);
                BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
               // blinkConverterSettings.AdditionalDelay = 30000;
                blinkConverterSettings.EnableJavaScript = true;
                blinkConverterSettings.Margin.All = 15;
                blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(1080, 0);
                //Assign Blink converter settings to HTML converter.
                htmlConverter.ConverterSettings = blinkConverterSettings;
               // string htmlText1 = "<html> <body> <link rel='stylesheet' href='https://www.w3schools.com/w3css/4/w3.css'> <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css'> <style> table, td, th { border: 1px solid; } table { width: 100%; border-collapse: collapse; } .columnaMorada{ background: rgb(70, 42, 64); color:#FFFFFF; font-weight: bold; } .sub-menu { display: flex; flex-direction: column; align-items: center; width: 60px; padding-right:5px; } table.sinTabla,tr.sinTabla,td.sinTabla{ border-style: none !important; margin-top:10px; margin-bottom:10px; margin-left:10px; margin-right:10px; } td.sinTabla{ padding-left:20px; padding-right:20px; } .negrita{ font-weight: bold; } .sizePhoto{ max-width: 180px; max-height: 180px; width: auto; } .inventoryTable { border-collapse: collapse; width: 100%; } .inventoryTable td, .inventoryTable th { border: 1px solid #ddd; padding: 8px; } .inventoryTable th { padding-top: 12px; padding-bottom: 12px; text-align: left; background-color: rgb(70, 42, 64); color: white; } #signaturename { text-align: left; font-weight: bold; font-size: 100%; } #signature { width: 100%; border-bottom: 1px solid black; height: 30px; } </style> <img src='logo_nuevo_pds.png' alt='Premier' style='max-width: 240px;max-height: 90px;width: auto'/> <h3 align='center'>Property Delivery / Move In Inspection</h3> <table> <tr> <td class='columnaMorada'>Tenant</td> <td>Jose Angél Fernado Juárez Gonzalez</td> <td class='columnaMorada'>Landlord</td> <td>landlord name name Edit popUp</td> </tr> <tr> <td class='columnaMorada'>Address</td> <td colspan='3'>Calle insurgentes 1789</td> </tr> <tr> <td class='columnaMorada'>City, Country</td> <td colspan='3'>Mexico City, Mexico</td> </tr> <tr> <td class='columnaMorada'>Move In Date</td> <td colspan='3'>2023/01/18</td> </tr> </table> <br> <br> <!--Esto va dentro de un for--> <table cellpadding='5px'><tr><td class='columnaMorada'>Bedroom</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Poor'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: green'></i><label>Ready</label></div></div></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value='Edit Pop up NEW'></textarea></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Photos</h3></td></tr><tr><td class='sinTabla'><img class='sizePhoto' src='16872820-c32d-4094-90fd-55a92de45e8f.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='453543c2-ec65-4ee1-9e45-faaf687ad895.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='dadd0547-7ed6-4d0a-85c6-54b3ce5da84b.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='10a0b1cf-9bfa-4a76-9ee8-0e32a60b2c0d.png'></img></td></tr><tr><td class='sinTabla'><img class='sizePhoto' src='d9be46ea-2534-47b4-9b1f-2fb1abbfd3ed.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='40dcaaea-ebdf-4d88-b283-63beb9952895.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='ab261de8-043a-408e-9235-e2c185ef04f8.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='b831ca23-6e26-432d-9bc1-0825f1ef1ce8.png'></img></td></tr><tr><td class='sinTabla'><img class='sizePhoto' src='49ce8a57-88a6-4832-89ef-9d872c0267e2.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='2d0f66a7-9377-4baa-804f-ccae74c15223.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='69bf7678-c6ff-42de-87c7-bd51c24d9f5a.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='033e3d62-c13c-4e5d-8a1c-9ae0c624ec1e.png'></img></td></tr><tr><td class='sinTabla'><img class='sizePhoto' src='f94ed241-ea6e-4bd6-bbcb-fd7b98d205b7.png'></img></td><tr><td class='sinTabla' height='0px'></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Inventory</h3></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><table class='inventoryTable'><tr><th>Item</th><th>Descripcion</th><th>Quantity</th><th>Photos</th></tr><tr><td>Head board</td><td>ddddd</td><td>34</td><td>1</td></tr><tr><td>Bedroom chest</td><td></td><td></td><td>0</td></tr><tr><td>Queen size bed (1.50 x 1.90 mts)</td><td>67890</td><td></td><td>0</td></tr><tr><td>Queen size bed (1.50 x 1.90 mts)</td><td>Algodón </td><td>5</td><td>0</td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table><table cellpadding='5px'><tr><td class='columnaMorada'>Bathroom</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Excellent'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: red'></i><label>Clean</label></div></div></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value='section 2 edit 2a edit edit '></textarea></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Photos</h3></td></tr><tr><td class='sinTabla'><img class='sizePhoto' src='ad1071aa-d65f-495c-a901-e2128ad848c2.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='8aec3b15-1e60-44a9-a862-81085f695be8.png'></img></td></tr><tr><td class='sinTabla' height='0px'></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Inventory</h3></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><table class='inventoryTable'><tr><th>Item</th><th>Descripcion</th><th>Quantity</th><th>Photos</th></tr><tr><td>shower</td><td>shower descr edit</td><td>5</td><td>2</td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table><table cellpadding='5px'><tr><td class='columnaMorada'>Bedroom</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Good'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: green'></i><label>Ready</label></div></div></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value='Bedroom test inventario '></textarea></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Photos</h3></td></tr><tr><td class='sinTabla'><img class='sizePhoto' src='99d66c1c-17f6-4d71-9537-c5dc1ed4c92c.png'></img></td><td class='sinTabla'><img class='sizePhoto' src='3c9dc442-f352-4cd1-9794-350afd054401.png'></img></td></tr><tr><td class='sinTabla' height='0px'></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Inventory</h3></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><table class='inventoryTable'><tr><th>Item</th><th>Descripcion</th><th>Quantity</th><th>Photos</th></tr><tr><td>Chair</td><td>Descripción edit 12345</td><td>5</td><td>0</td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table><table cellpadding='5px'><tr><td class='columnaMorada'>Porch</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Good'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: red'></i><label>Repare</label></div></div><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: red'></i><label>Replace</label></div></div></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value='Porch'></textarea></p></td></tr><tr><td class='sinTabla' height='0px'></td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table><table cellpadding='5px'><tr><td class='columnaMorada'>Terrace/balcon</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Very Poor'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: green'></i><label>Ready</label></div></div></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value='Terrace verybpoor'></textarea></p></td></tr><tr><td class='sinTabla' height='0px'></td></tr><tr><td colspan='4' class='sinTabla'><h4 align='left'>Inventory</h3></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><table class='inventoryTable'><tr><th>Item</th><th>Descripcion</th><th>Quantity</th><th>Photos</th></tr><tr><td>Chair</td><td>Hdhj</td><td>5</td><td>0</td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table><table cellpadding='5px'><tr><td class='columnaMorada'>Kitchen</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Not Selected'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: red'></i><label>Clean</label></div></div><div style='display: inline-block'><div class='sub-menu'><i class='fa fa-circle fa-lg'style='color: red'></i><label>Replace</label></div></div></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value=''></textarea></p></td></tr><tr><td class='sinTabla' height='0px'></td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table><table cellpadding='5px'><tr><td class='columnaMorada'>Not Selected</td></tr><tr><td><table class='sinTabla'><tr><td colspan='4' class='sinTabla'><br></td></tr><tr class='sinTabla'><td class='sinTabla'><p><label class='negrita'>Status</label><input class='w3-input' type='text' value='Not Selected'></p></td><td class='sinTabla'><p><label class='negrita'>Condition</label><input class='w3-input' type='text' value='Condicion'></p></td><td colspan='2' class='sinTabla' align='center' style='padding-left:5px!important; padding-right:5px!important; '><p><label class='negrita'>Actions</label></p></td></tr><tr><td colspan='4' class='sinTabla'><br></td></tr><tr><td colspan='4' class='sinTabla'><p><label class='negrita'>Reports Details</label><textarea class='w3-input' type='text' value=''></textarea></p></td></tr><tr><td class='sinTabla' height='0px'></td></tr><tr><td class='sinTabla' height='60px'></td></tr></table></table></td></tr></table></body></html> <h3 align='left'style='font-weight: bold; padding-top:100px;'>Signatures</h3> <table class='sinTabla'> <tr class='sinTabla'> <td span='2' class='sinTabla'> <div id='signature'> </div> <div id='signaturename'> Tenant Signature </div> </td> <td class='sinTabla'> </td> <td class='sinTabla'> <div id='signature'> </div> <div id='signaturename'> Date </div> </td> </tr> <tr class='sinTabla'><td class='sinTabla'><br></td></tr> <tr class='sinTabla'><td class='sinTabla'><br></td></tr> <tr class='sinTabla'><td class='sinTabla'><br></td></tr> <tr class='sinTabla'> <td span='2' class='sinTabla'> <div id='signature'> </div> <div id='signaturename'> LandLord Signature </div> </td> <td class='sinTabla'> </td> <td class='sinTabla'> <div id='signature'> </div> <div id='signaturename'> Date </div> </td> </tr> </table> </body> </html>";
                PdfDocument document = htmlConverter.Convert("https://aloha-mexico.com/");
                document.Compression = PdfCompressionLevel.BestSpeed;
                //Save and close the PDF document.
                FileStream fileStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.ReadWrite);
               // Save and close the PDF document.
                document.Save(fileStream);
                document.Close(true);
                fileStream.Dispose();
                document.Dispose();
                // Reducir tamaño
                Byte[] bytes = System.IO.File.ReadAllBytes(filepath);
                FileInfo fileInfo_2 = new FileInfo(filepath);
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(bytes);
                loadedDocument.Compression = PdfCompressionLevel.BestSpeed;
                //Save the PDF document
                string nuevoNombre = Path.Combine(fileInfo_2.DirectoryName, "1_" + fileInfo_2.Name);
                fileStream = new FileStream(nuevoNombre, FileMode.CreateNew, FileAccess.ReadWrite);
                loadedDocument.Save(fileStream);
               // Close the document
                 loadedDocument.Close(true);
                fileStream.Dispose();

                //bytes = System.IO.File.ReadAllBytes(nuevoNombre);
                //String base64 = Convert.ToBase64String(bytes);

                //if (System.IO.File.Exists(nuevoNombre))
                //    System.IO.File.Delete(nuevoNombre);

                //if (System.IO.File.Exists(filepath))
                //    System.IO.File.Delete(filepath);

                //return base64;

                return htmlText + "ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo" + baseUrl + "uuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu" + filepath;

            }
            else
            {
                return "";
            }
        } // nuevo VIc Feb 2023


        public string  GetLSFPrint(int key, int servide_detail_id, int type, string filepath) // nuevo VIc Feb 2023
        {

            // var _detail_srv = atributos_generales(servide_detail_id, type);

            var wo_id = _context.HousingLists.FirstOrDefault(a => a.Id == key).WorkOrder;

            det_grales_servicio det_grales_servicio = atributos_generales(servide_detail_id, type); // type es 26 

            var sr_id = 0;

            if (wo_id != null)
            {
                sr_id = _context.WorkOrders.FirstOrDefault(a => a.Id == wo_id).ServiceRecordId.Value;
            }

            var house_data_list = _context.HousingLists.Where(x => x.Id == key).Select(s => new
            {
                s.Id
             ,
                LandLord = s.LandlordHeaderDetailsHomes.FirstOrDefault(l => l.HousingListId == key && l.IdServiceDetail == servide_detail_id) != null ?
                         s.LandlordHeaderDetailsHomes.FirstOrDefault(l => l.HousingListId == key && l.IdServiceDetail == servide_detail_id).Name : "Data not captured"
             ,
                Address = s.Address
             ,
                City_Country = det_grales_servicio != null ? det_grales_servicio.location + ", " + det_grales_servicio.country_name : "Data not captured"
             ,
                Tenant = _context.AssigneeInformations.FirstOrDefault(z => z.ServiceRecordId == sr_id).AssigneeName
            }
                ).ToList();

            var house_data = house_data_list[0];



            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    Id = s.Id
                    ,
                    ContractDetails = _context.ContractDetails.Select(c => new {
                        c.IdServiceDetail,
                        c.ContractDetailId,
                        c.LeaseEndDate,
                        c.LeaseStartDate,
                        c.PaymentsDue,
                        c.Recurrence,
                        c.Namesinatore,
                        c.Emailsinatore,
                        c.Telsinatore,
                        CurrencyText = c.Currency.HasValue == true ? _context.CatCurrencies.Select(cc => new {
                                                                    cc.Currency, 
                                                                    cc.Id })
                                                                    .Where(x => x.Id == c.Currency.Value)
                                                                    .FirstOrDefault().Currency : "Not Selected",
                        RecurrenceText = c.Recurrence,
                        c.ListRentPrice,
                        c.FinalRentPrice,
                        c.Rentcostsaving,
                        InfoText = c.Leasesignatories.HasValue == true ? _context.LeaseSignators.Select(cc => new {
                                                                    cc.Signator,
                                                                    cc.Id})
                                                                    .Where(x => x.Id == c.Leasesignatories.Value)
                                                                    .FirstOrDefault().Signator : "Not Selected",
                        Expenses = _context.PropertyExpenses.Select(ex => new { 
                            ex.Expense,
                            ex.Amount,
                            CurrencyText = c.Currency.HasValue == true ? _context.CatCurrencies.Select(cc => new {
                                cc.Currency,
                                cc.Id
                            })
                            .Where(x => x.Id == ex.Currency.Value)
                            .FirstOrDefault().Currency : "Not Selected",
                            ex.Recurrence,
                            ex.Included,
                            ex.ContractDetail,
                        }).Where(x => x.ContractDetail == key).ToList(),

                        Special = _context.SpecialConsiderations.Select(a => new { 
                            a.ContractDetailId,
                            a.Id,
                            a.Consideration
                        }).Where(x => x.ContractDetailId == key).ToList()
                    })
                    .Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),

                    //, DepartureDetailsHomes = _context.DepartureDetailsHomes

                    //, RenewalDetailHomes = _context.RenewalDetailHomes

                    //, LandlordHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                    //.Include(p => p.LandlordDetailsHomes)
                    //.Include(c => c.CreditCardLandLordDetails)

                }).ToList();

            var GroupPayments = _context.GroupPaymnetsHousings.Select(x => new
            {
                x.IdServiceDetail,
                x.HousingListId,
                Payments = _context.PaymentHousings.Select(y => new {
                    y.GroupPaymentsHousingId,
                    y.IdServiceDetail,
                    y.HousingList,
                    PaymentTypeText = y.PaymentType.HasValue == true ? y.PaymentTypeNavigation.Type : "Not Selected",
                    ResponsibleText = y.Responsible.HasValue == true ? y.ResponsibleNavigation.Responsable : "Not Selected",
                    y.Amount,
                    CurrencyText = y.Currency.HasValue == true ? y.CurrencyNavigation.Currency : "Not Selected",
                    y.Description
                }).Where(a => a.HousingList == x.HousingListId && a.IdServiceDetail == servide_detail_id).ToList()
            }).Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList();

            var GroupSaving = _context.GroupCostSavings.Select(x => new
            {
                x.IdServiceDetail,
                x.HousingListId,
                Savings = _context.CostSavingHomes.Select(y => new {
                   y.GroupCostSavingId,
                   y.HousingList,
                   y.IdServiceDetail,
                   y.CostType,
                   y.CostSavings,
                   CurrencyText = y.Currency.HasValue == true ? y.CurrencyNavigation.Currency : "Not Selected",
                   y.CostDesc
                }).Where(a => a.HousingList == x.HousingListId && a.IdServiceDetail == servide_detail_id).ToList()
            }).Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList();

            var Renewal = _context.RenewalDetailHomes.Select(x => new { 
                x.Id,
                x.IdServiceDetail,
                x.Automatically,
                x.RenewalNotification,
                x.AdditionalRentIncreaseDate,
                x.RecurrentIncreasePeriod,
                x.Comments,
            }).Where(r => r.IdServiceDetail == servide_detail_id).ToList();

            var Departure = _context.DepartureDetailsHomes.Select(x => new {
                x.Id,
                x.IdServiceDetail,
                x.DiplomaticClause,
                x.EarlyTerminationNotification,
                x.Permission,
                x.Penalty,
                x.SecurityDepositReturn,
                ReturnText= x.ReturnSecurityDepositTo.HasValue == true ? x.ReturnSecurityDepositToNavigation.Name : "Not Selected",
            }).Where(r => r.IdServiceDetail == servide_detail_id).ToList();

            var LandLord = _context.LandlordHeaderDetailsHomes.Select(x => new
            {
                x.Id,
                x.IdServiceDetail,
                x.HousingListId,
                x.Name,
                x.PrincipalEmail,
                x.SecondaryEmail,
                x.PrincipalPhone,
                x.SecundaryPhone,
                x.FiscalInvoice,
                x.CreditCard,
                x.Checks,
                x.Cash,
                x.WireTransfer,
                x.PayToOrderOf,
                Cards = _context.CreditCardLandLordDetails.Select(t => new
                {
                    t.LandLord,
                    CardText = t.CreditCardNavigation.CreditCard
                }).Where(a => a.LandLord == x.Id).ToList(),
            }).Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList();


            var _hosuging = consult[0];

            string xKey = "Mgo+DSMBaFt/QHJqVVhkX1pFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9jQXxTd01mXH1ecH1WRQ==;Mgo+DSMBMAY9C3t2VVhiQlFaclxJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdk1hWX5WdXFXRmFcVUc=";

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(xKey);

            string returnName = $"Files/Pdf/1_{key}_{servide_detail_id}_{type}.pdf";

            filepath = Path.GetFullPath($"Files/Pdf/{key}_{servide_detail_id}_{type}.pdf");
            string filedel = Path.GetFullPath(returnName);
            if (System.IO.File.Exists(filedel))
                System.IO.File.Delete(filedel);

            string htmlText = System.IO.File.ReadAllText(Path.GetFullPath("./images/") + "LeasySumary.html");
            string baseUrl = Path.GetFullPath("./images/");

            //Modificar el documento
            string xSection = "";

            xSection += "<table cellpadding=\"5px\">";
            xSection += $"<tr><td class=\"columnaMorada\">{"Property Details"}</td></tr>";
            xSection += "<tr>";
            xSection += "<td class=\"sinTabla\">";
            xSection += "<table  class=\"sinTabla\">";
            xSection += $"<tr class=\"sinTabla\"><td colspan=\"4\" class=\"sinTabla\"><p><label class=\"negrita\">Tenant Name</label><input class=\"w3-input\" type=\"text\" value=\"{""}\"></p></td>";
            xSection += "</tr>";
            xSection += $"<tr class=\"sinTabla\"><td colspan=\"4\" class=\"sinTabla\"><p><label class=\"negrita\">Property Type</label><input class=\"w3-input\" type=\"text\" value=\"{""}\"></p></td>";
            xSection += "</tr>";
            xSection += $"<tr class=\"sinTabla\"><td colspan=\"4\" class=\"sinTabla\"><p><label class=\"negrita\">Property Address</label><input class=\"w3-input\" type=\"text\" value=\"{""}\"></p></td>";
            xSection += "</tr>";
            xSection += "<tr>";
            xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Zip Code</label><input class=\"w3-input\" type=\"text\" value=\"{""}\"></p></td>";
            xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">City</label><input class=\"w3-input\" type=\"text\" value=\"{""}\"></p></td>";
            xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Country</label><input class=\"w3-input\" type=\"text\" value=\"{""}\"></p></td>";
            xSection += "</tr>";
            xSection += "</table>";
            xSection += "</table>";

            xSection += "<div><br><br><br></div>"; // Espacio en tabla

            if (_hosuging.ContractDetails.Count > 0)
            {
                foreach (var contract in _hosuging.ContractDetails)
                {
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{"Contract Details"}</td></tr>";
                    xSection += "<tr>";
                    xSection += "<td class=\"sinTabla\">";
                    xSection += "<table  class=\"sinTabla\">";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr>";
                    string startDate = contract.LeaseStartDate.HasValue == false ? "" : contract.LeaseStartDate.Value.ToString("dd/MM/yyyy");
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Lease Start Date</label><input class=\"w3-input\" type=\"text\" value=\"{startDate}\"></p></td>";
                    string endDate = contract.LeaseEndDate.HasValue == false ? "" : contract.LeaseEndDate.Value.ToString("dd/MM/yyyy");
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Lease End Date</label><input class=\"w3-input\" type=\"text\" value=\"{endDate}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Payments Due</label><input class=\"w3-input\" type=\"text\" value=\"{contract.PaymentsDue}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Recurrence</label><input class=\"w3-input\" type=\"text\" value=\"{contract.RecurrenceText}\"></p></td>";
                    xSection += "</tr>";
                    xSection += "<tr>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Currency</label><input class=\"w3-input\" type=\"text\" value=\"{contract.CurrencyText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">List Rent Price</label><input class=\"w3-input\" type=\"text\" value=\"{contract.ListRentPrice}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Final Rent Price</label><input class=\"w3-input\" type=\"text\" value=\"{contract.FinalRentPrice}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Rent Cost Savings</label><input class=\"w3-input\" type=\"text\" value=\"{contract.Rentcostsaving}\"></p></td>";
                    xSection += "</tr>";

                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr>";
                    xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<h4 align=\"left\">Lease Signatore Info</h3>";
                    xSection += "</td>";
                    xSection += "</tr>";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Lease Signatories</label><input class=\"w3-input\" type=\"text\" value=\"{contract.InfoText}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Name</label><input class=\"w3-input\" type=\"text\" value=\"{contract.Namesinatore}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Email</label><input class=\"w3-input\" type=\"text\" value=\"{contract.Emailsinatore}\"></p></td>";
                    xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Telephone</label><input class=\"w3-input\" type=\"text\" value=\"{contract.Telsinatore}\"></p></td>";
                    xSection += "</tr>";


                    if (contract.Expenses.Count > 0)
                    {
                        //Titulo
                        xSection += "<table cellpadding=\"5px\">";
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr>";
                        xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<h4 align=\"left\">Property Expenses</h3>";
                        xSection += "</td>";
                        xSection += "</tr>";
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        //Se termina Titulo
                        //Inicia Cabecera tabla
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                        xSection += "<tr><td colspan=\"4\" class=\"sinTabla\">";
                        xSection += "<table class=\"inventoryTable\">";
                        xSection += "<tr><th>Expense</th><th>Amount</th><th>Currency</th><th>Recurrence</th><th>Included</th></tr>";
                        //Termina Cabecera tabla
                        foreach (var att in contract.Expenses)
                        {
                            xSection += $"<tr><td>{att.Expense}</td><td>{att.Amount}</td><td>{att.CurrencyText}</td><<td>{att.Recurrence}</td><td>{att.Included}</td>tr>";
                        }
                        xSection += "</table>";
                        xSection += "</td></tr>";
                        xSection += "</table>";
                    }
                    xSection += "</table>";
                    xSection += "</table>";
                }
                
            }

            if (GroupPayments.Count > 0)
            {
                if (GroupPayments[0].Payments.Count() > 0)
                {
                    xSection += "<div><br><br><br></div>"; // Espacio en tabla
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{"Payments"}</td></tr>";
                    //Se termina Titulo
                    //Inicia Cabecera tabla
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<table class=\"inventoryTable\">";
                    xSection += "<tr><th>Payment Type</th><th>Responsible</th><th>Amount</th><th>Currency</th><th>Description</th></tr>";
                    //Termina Cabecera tabla
                    foreach (var pay in GroupPayments[0].Payments)
                    {
                        xSection += $"<tr><td>{pay.PaymentTypeText}</td><td>{pay.ResponsibleText}</td><td>{pay.Amount}</td><td>{pay.CurrencyText}</td><td>{pay.Description}</td></tr>";
                    }
                    xSection += "</table>";
                    xSection += "</td></tr>";
                    xSection += "</table>";
                }
            }

            if (GroupSaving.Count > 0)
            {
                if (GroupSaving[0].Savings.Count() > 0)
                {
                    xSection += "<div><br><br><br></div>"; // Espacio en tabla
                    //Titulo
                    xSection += "<table cellpadding=\"5px\">";
                    xSection += $"<tr><td class=\"columnaMorada\">{"Cost Savings"}</td></tr>";
                    //Se termina Titulo
                    //Inicia Cabecera tabla
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                    xSection += "<tr><td colspan=\"4\" class=\"sinTabla\">";
                    xSection += "<table class=\"inventoryTable\">";
                    xSection += "<tr><th>Cost Type</th><th>Cost Savings</th><th>Cost Saving Currency</th><th>Cost Description</th></tr>";
                    //Termina Cabecera tabla
                    foreach (var pay in GroupSaving[0].Savings)
                    {
                        xSection += $"<tr><td>{pay.CostType}</td><td>{pay.CostSavings}</td><td>{pay.CurrencyText}</td><td>{pay.CostDesc}</td></tr>";
                    }
                    xSection += "</table>";
                    xSection += "</td></tr>";
                    xSection += "</table>";
                }
            }

            if (Renewal.Count > 0)
            {
                xSection += "<div><br><br><br></div>"; // Espacio en tabla

                xSection += "<table cellpadding=\"5px\">";
                xSection += $"<tr><td class=\"columnaMorada\">{"Renewal Details"}</td></tr>";
                xSection += "<tr>";
                xSection += "<td class=\"sinTabla\">";
                xSection += "<table  class=\"sinTabla\">";
                xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                //Se termina Titulo
                //Inicia Cabecera tabla
                xSection += "<tr>";
                bool Auto = Renewal[0].Automatically.HasValue == true ? Renewal[0].Automatically.Value : false;
                if (Auto)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Automatically Renewed</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Automatically Renewed</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Renewal Notification Days</label><input class=\"w3-input\" type=\"text\" value=\"{Renewal[0].RenewalNotification}\"></p></td>";
                xSection += "</tr>";
                xSection += "<tr>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Additional Rent Increase Date</label><input class=\"w3-input\" type=\"text\" value=\"{Renewal[0].AdditionalRentIncreaseDate}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Recurrent Increase Period</label><input class=\"w3-input\" type=\"text\" value=\"{Renewal[0].RecurrentIncreasePeriod}\"></p></td>";
                xSection += "</tr>";
                xSection += $"<tr><td colspan=\"4\" class=\"sinTabla\"><p><label class=\"negrita\">Comments</label><input class=\"w3-input\" type=\"text\" value=\"{Renewal[0].Comments}\"></p></td>";
                xSection += "</tr>";
                xSection += "</table>";
                xSection += "</td></tr>";
                xSection += "</table>";
            }

            if (Departure.Count > 0)
            {
                xSection += "<div><br><br><br></div>"; // Espacio en tabla

                xSection += "<table cellpadding=\"5px\">";
                xSection += $"<tr><td class=\"columnaMorada\">{"Departure Details"}</td></tr>";
                xSection += "<tr>";
                xSection += "<td class=\"sinTabla\">";
                xSection += "<table  class=\"sinTabla\">";
                xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                //Se termina Titulo
                //Inicia Cabecera tabla
                xSection += "<tr>";
                bool Dipomatic = Departure[0].DiplomaticClause.HasValue == true ? Departure[0].DiplomaticClause.Value : false;
                bool Permission = Departure[0].Permission.HasValue == true ? Departure[0].Permission.Value : false;
                if (Dipomatic)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Diplomatic Clause</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Diplomatic Clause</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                if (Permission)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Permission to Credit Security Deposit</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Permission to Credit Security Deposit</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                xSection += "</tr>";
                xSection += "<tr>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Early Termination Notification Days</label><input class=\"w3-input\" type=\"text\" value=\"{Departure[0].EarlyTerminationNotification}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Penalty</label><input class=\"w3-input\" type=\"text\" value=\"{Departure[0].Penalty}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Penalty Description</label><input class=\"w3-input\" type=\"text\" value=\"{Departure[0].Penalty}\"></p></td>";
                xSection += "</tr>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Security Deposit Return Days</label><input class=\"w3-input\" type=\"text\" value=\"{Departure[0].SecurityDepositReturn}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Return Security Deposit To</label><input class=\"w3-input\" type=\"text\" value=\"{Departure[0].ReturnText}\"></p></td>";
                xSection += "</tr>";
                xSection += "</table>";
                xSection += "</td></tr>";
                xSection += "</table>";
            }

            if (LandLord.Count > 0)
            {
                xSection += "<div><br><br><br></div>"; // Espacio en tabla

                xSection += "<table cellpadding=\"5px\">";
                xSection += $"<tr><td class=\"columnaMorada\">{"Landlord Details"}</td></tr>";
                xSection += "<tr>";
                xSection += "<td class=\"sinTabla\">";  
                xSection += "<table  class=\"sinTabla\">";
                xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                xSection += "<tr>";
                xSection += $"<td colspan=\"2\" class=\"sinTabla\"><p><label class=\"negrita\">Landlord Name</label><input class=\"w3-input\" type=\"text\" value=\"{LandLord[0].Name}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Lease End Date</label><input class=\"w3-input\" type=\"text\" value=\"{LandLord[0].PrincipalEmail}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Payments Due</label><input class=\"w3-input\" type=\"text\" value=\"{LandLord[0].SecondaryEmail}\"></p></td>";
                xSection += "</tr>";
                xSection += "<tr>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Landlord Principal Phone</label><input class=\"w3-input\" type=\"text\" value=\"{LandLord[0].PrincipalPhone}\"></p></td>";
                xSection += $"<td class=\"sinTabla\"><p><label class=\"negrita\">Landlord Secondary Phone</label><input class=\"w3-input\" type=\"text\" value=\"{LandLord[0].SecundaryPhone}\"></p></td>";
                bool Invoice = LandLord[0].FiscalInvoice.HasValue == true ? LandLord[0].FiscalInvoice.Value : false;
                if (Invoice)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Final Invoice</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Final Invoice</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                xSection += "</tr>";
                xSection += "<tr>";
                xSection += "<td colspan=\"4\" class=\"sinTabla\">";
                xSection += "<h4 align=\"left\">Payment Methods</h3>";
                xSection += "</td>";
                xSection += "</tr>";
                xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";

               
                bool credit = LandLord[0].CreditCard.HasValue == true ? LandLord[0].CreditCard.Value : false;
                bool cash = LandLord[0].Cash.HasValue == true ? LandLord[0].Cash.Value : false;
                bool cheks = LandLord[0].Checks.HasValue == true ? LandLord[0].Checks.Value : false;
                bool wire = LandLord[0].WireTransfer.HasValue == true ? LandLord[0].WireTransfer.Value : false;

                xSection += "<tr>";
                if (credit)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Credit Card</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Credit Card</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                xSection += "</tr>";

                xSection += "<tr>";
                if (credit)
                {
                    foreach (var card in LandLord[0].Cards)
                    {
                        xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                        xSection += $"<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">{card.CardText}</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                        xSection += "</td>";
                    }
                }
                else
                {
                    foreach (var card in LandLord[0].Cards)
                    {
                        xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                        xSection += $"<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">{card.CardText}</label><i class=\"fa fa-circle fa-lg\"style=\"color: red\"></i></div></div></p>";
                        xSection += "</td>";
                    }
                }
                xSection += "</tr>";

                xSection += "<tr>";
                if (cash)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Cash</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Cash</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                if (cheks)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Checks</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Checks</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                xSection += $"<td colspan=\"2\"class=\"sinTabla\"><p><label class=\"negrita\">Pay to the Order of</label><input class=\"w3-input\" type=\"text\" value=\"{LandLord[0].PayToOrderOf}\"></p></td>";
                xSection += "</tr>";

                xSection += "<tr>";
                if (wire)
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Wire Transfer</label><i class=\"fa fa-circle fa-lg\"style=\"color: green\"></i></div></div></p>";
                    xSection += "</td>";
                }
                else
                {
                    xSection += "<td class=\"sinTabla\" align=\"left\" style=\"padding-left:5px!important; padding-right:5px!important;\">";
                    xSection += "<p><div style=\"display: inline-block\"><div class=\"sub-menu\"><label class=\"negrita\" style=\"padding-right:10px;\">Wire Transfer</label><i class=\"fa fa-circle fa-lg\" style=\"color: red\"></i></div></div></p>";
                    xSection += "</td>";
                }
                xSection += "</tr>";
                xSection += "</table>";
                xSection += "</table>";
            }

            if (_hosuging.ContractDetails[0].Special.Count > 0)
            {
                xSection += "<div><br><br><br></div>"; // Espacio en tabla
                xSection += "<table cellpadding=\"5px\">";
                xSection += $"<tr><td class=\"columnaMorada\">{"Special Considerations"}</td></tr>";
                //Se termina Titulo
                //Inicia Cabecera tabla
                xSection += "<tr><td colspan=\"4\" class=\"sinTabla\"><br></td></tr>";
                xSection += "<tr><td colspan=\"4\" class=\"sinTabla\">";
                xSection += "<table class=\"inventoryTable\">";
                //Termina Cabecera tabla
                int contador = 1;
                foreach (var special in _hosuging.ContractDetails[0].Special)
                {
                    xSection += $"<tr><td>{contador}</td><td>{special.Consideration}</td></tr>";
                }
                xSection += "</table>";
                xSection += "</td></tr>";
                xSection += "</table>";
            }



            ////Comments
            //xSection += "<tr>";
            //xSection += "<td colspan=\"4\" class=\"sinTabla\">";
            //xSection += "<p>";
            //xSection += "<label class=\"negrita\">Notes</label>";
            //xSection += $"<textarea class=\"w3-input\" type=\"text\" value=\"{item.InspecDetails}\"></textarea>";
            //xSection += "</p>";
            //xSection += "</td>";
            //xSection += "</tr>";
            //xSection += "</table></table></td></tr>";

            xSection += "</body>";
            xSection += "</html>";
            htmlText = htmlText.Replace("@Datos", xSection);

            //Initialize HTML to PDF converter with Blink rendering engine.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);

            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            //blinkConverterSettings.AdditionalDelay = 30000;
            blinkConverterSettings.EnableJavaScript = true;
            blinkConverterSettings.Margin.All = 15;
            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(1080, 0);
            //Assign Blink converter settings to HTML converter.
            blinkConverterSettings.TempPath = "C://tempoSYNC";
            htmlConverter.ConverterSettings = blinkConverterSettings;
            PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);
            document.Compression = PdfCompressionLevel.BestSpeed;
            //Save and close the PDF document.
            FileStream fileStream = new FileStream(filepath, FileMode.CreateNew, FileAccess.ReadWrite);
            //Save and close the PDF document.
            document.Save(fileStream);
            document.Close(true);
            fileStream.Dispose();
            document.Dispose();
            // Reducir tamaño
            Byte[] bytes = System.IO.File.ReadAllBytes(filepath);
            FileInfo fileInfo_2 = new FileInfo(filepath);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(bytes);
            loadedDocument.Compression = PdfCompressionLevel.BestSpeed;
            //Save the PDF document
            string nuevoNombre = Path.Combine(fileInfo_2.DirectoryName, "1_" + fileInfo_2.Name);
            fileStream = new FileStream(nuevoNombre, FileMode.CreateNew, FileAccess.ReadWrite);
            loadedDocument.Save(fileStream);
            //Close the document
            loadedDocument.Close(true);
            fileStream.Dispose();

            //bytes = System.IO.File.ReadAllBytes(nuevoNombre);
            //String base64 = Convert.ToBase64String(bytes);

            if (System.IO.File.Exists(filepath))
                System.IO.File.Delete(filepath);

            return returnName;
        }
        public ActionResult GetLSFPropertyPrint(int key, int servide_detail_id, int type)
        {

            var _detail_srv = atributos_generales(servide_detail_id, type);

            var consult = _context.HousingLists
                .Where(x => x.Id == key).Select(s => new
                {
                    Id = s.Id,
                    WorkOrder = s.WorkOrder,
                    Service = s.Service,
                    ServiceType = s.ServiceType,
                    PropertyNo = s.PropertyNo,
                    Sample = s.Sample,
                    SupplierPartner = s.SupplierPartner,
                    Supplier = s.Supplier,
                    VisitDate = s.VisitDate,
                    HousingStatus = s.HousingStatus,
                    PropertyType = s.PropertyType,
                    Address = s.Address,
                    Zip = s.Zip,
                    WebSite = s.WebSite,
                    Bedrooms = s.Bedrooms,
                    Bathrooms = s.Bathrooms,
                    ParkingSpaces = s.ParkingSpaces,
                    Price = s.Price,
                    Currency = s.Currency,
                    AdditionalComments = s.AdditionalComments,
                    CreatedBy = s.CreatedBy,
                    CreatedDate = s.CreatedDate,
                    UpdateBy = s.UpdateBy,
                    UpdatedDate = s.UpdatedDate,
                    IdServiceDetail = s.IdServiceDetail,
                    Othersupplier = s.Othersupplier,
                    Suppliertelephone = s.Suppliertelephone,
                    Supplieremail = s.Supplieremail,
                    SupplierNavigation = s.SupplierNavigation,
                    SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                    AmenitiesHousingLists = s.AmenitiesHousingLists,
                    CommentHousings = s.CommentHousings,
                    DocumentHousings = s.DocumentHousings,
                    Shared = s.Shared,
                    VisitTime = s.VisitTime,
                    Neighborhood = s.Neighborhood,

                    ////// LSF 
                    DepartureDetailsHomes = _context.DepartureDetailsHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    departureHistoricDetailsHomes = (type == 26 ? new List<DepartureDetailsHome>() :
                                                                    _context.DepartureDetailsHomes
                                                                    .Where(r => r.IdServiceDetail != servide_detail_id
                                                                        && r.Id == key)
                                                                    //&& r.CreatedDate >= _detail_srv.creation)
                                                                    .OrderByDescending(h => h.CreatedDate).ToList()),// lista de pasados

                    RenewalDetailHomes = _context.RenewalDetailHomes
                .Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList(),//

                    renewalHistoricDetailHomes = (type == 26 ? new List<RenewalDetailHome>() :
                                                                _context.RenewalDetailHomes
                                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.Id == key)
                                                                .OrderByDescending(h => h.CreatedDate).ToList()),//lista de pasados

                    GroupCostSavings = _context.GroupCostSavings
                    .Include(o => o.CostSavingHomes)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    groupHistoricCostSavings = (type == 26 ? new List<GroupCostSaving>() :
                                                            _context.GroupCostSavings
                                                            .Include(o => o.CostSavingHomes)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                            .OrderByDescending(h => h.CreatedDate).ToList()), // lista de pasados

                    GroupPaymnetsHousings = _context.GroupPaymnetsHousings
                    .Include(o => o.PaymentHousings)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    groupHistoricPaymnetsHousings = (type == 26 ? new List<GroupPaymnetsHousing>() :
                                                                _context.GroupPaymnetsHousings
                                                                .Include(o => o.PaymentHousings)
                                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                                .OrderByDescending(h => h.CreatedDate).ToList()),

                    ContractDetails = _context.ContractDetails
                    .Include(p => p.PropertyExpenses)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList(),

                    contractHistoricDetails = (type == 26 ? new List<ContractDetail>() :
                                                            _context.ContractDetails
                                                            .Include(p => p.PropertyExpenses)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.ContractDetailId == key)
                                                            .OrderByDescending(h => h.CreatedDate).ToList()),

                    LandlordHeaderDetailsHomes = _context.LandlordHeaderDetailsHomes
                    .Include(p => p.LandlordDetailsHomes)
                    .Include(c => c.CreditCardLandLordDetails)
                // .ThenInclude(x => x.CreditCardLandLordDetails)
                .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),

                    Landlor_HistoricHeaderDetailsHomes = (type == 26 ? new List<LandlordHeaderDetailsHome>() :
                                                                _context.LandlordHeaderDetailsHomes
                                                                .Include(p => p.LandlordDetailsHomes)
                                                                //  .ThenInclude(x => x.CreditCardLandLordDetails)
                                                                .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                                .OrderByDescending(h => h.CreatedDate).ToList()),

                    //I&R

                    PropertyReports = new List<biz.premier.Entities.PropertyReport>(),

                    Inspections = new List<Inspection>(),

                    groupIr = new List<GroupIr>(),

                    groupIrhistoric = new List<GroupIr>(),

                }).ToList();

            var _hosuging = consult[0];
            return new ObjectResult(_hosuging);
        }

        public ActionResult GetIRPropertyPrint(int key, int servide_detail_id, int type)
        {
            var _detail_srv = atributos_generales(servide_detail_id, type);

            var consult = _context.HousingLists
               .Where(x => x.Id == key).Select(s => new
               {
                   Id = s.Id,
                   WorkOrder = s.WorkOrder,
                   Service = s.Service,
                   ServiceType = s.ServiceType,
                   PropertyNo = s.PropertyNo,
                   Sample = s.Sample,
                   SupplierPartner = s.SupplierPartner,
                   Supplier = s.Supplier,
                   VisitDate = s.VisitDate,
                   HousingStatus = s.HousingStatus,
                   PropertyType = s.PropertyType,
                   Address = s.Address,
                   Zip = s.Zip,
                   WebSite = s.WebSite,
                   Bedrooms = s.Bedrooms,
                   Bathrooms = s.Bathrooms,
                   ParkingSpaces = s.ParkingSpaces,
                   Price = s.Price,
                   Currency = s.Currency,
                   AdditionalComments = s.AdditionalComments,
                   CreatedBy = s.CreatedBy,
                   CreatedDate = s.CreatedDate,
                   UpdateBy = s.UpdateBy,
                   UpdatedDate = s.UpdatedDate,
                   IdServiceDetail = s.IdServiceDetail,
                   Othersupplier = s.Othersupplier,
                   Suppliertelephone = s.Suppliertelephone,
                   Supplieremail = s.Supplieremail,
                   SupplierNavigation = s.SupplierNavigation,
                   SupplierPartnerNavigation = s.SupplierPartnerNavigation,
                   AmenitiesHousingLists = s.AmenitiesHousingLists,
                   CommentHousings = s.CommentHousings,
                   DocumentHousings = s.DocumentHousings,
                   Shared = s.Shared,
                   VisitTime = s.VisitTime,
                   Neighborhood = s.Neighborhood,

                   ////// LSF 
                   DepartureDetailsHomes = new List<DepartureDetailsHome>(),//

                   departureHistoricDetailsHomes = new List<DepartureDetailsHome>(),// lista de pasados

                   RenewalDetailHomes = new List<RenewalDetailHome>(),//

                   renewalHistoricDetailHomes = new List<RenewalDetailHome>(),//lista de pasados

                   GroupCostSavings = new List<GroupCostSaving>(),

                   groupHistoricCostSavings = new List<GroupCostSaving>(), // lista de pasados

                   GroupPaymnetsHousings = new List<GroupPaymnetsHousing>(),

                   groupHistoricPaymnetsHousings = new List<GroupPaymnetsHousing>(),

                   ContractDetails = new List<ContractDetail>(),

                   contractHistoricDetails = new List<ContractDetail>(),

                   LandlordHeaderDetailsHomes = new List<LandlordHeaderDetailsHome>(),

                   Landlor_HistoricHeaderDetailsHomes = new List<LandlordHeaderDetailsHome>(),

                   //I&R

                   PropertyReports = _context.PropertyReports  // MOVE IN - OUT
                                   .Include(i => i.PropertyReportSections)
                                   .ThenInclude(ti => ti.PhotosPropertyReportSections)
                               .Include(i => i.PropertyReportSections)
                                   .ThenInclude(i => i.SectionInventories)
                                       .ThenInclude(i => i.PhotosInventories)
                               .Include(i => i.KeyInventories)
                               .Include(i => i.Attendees)
                               .Where(r => r.HousingList == key).ToList(),

                   Inspections = _context.Inspections
                   .Include(i => i.AttendeeInspecs)
                   .Include(i => i.PhotosInspecs)
              .Where(r => r.HousingList == key)
              .ToList(),


                   groupIr = _context.GroupIrs
                   .Include(o => o.Repairs)
                       .ThenInclude(d => d.DocumentRepairs)
                   .Include(t => t.Inspections)
               .Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList(),
                   //Repairs = s.Repairs.Where(r => r.HousingList == key).ToList(),

                   groupIrhistoric = (type == 26 ? new List<GroupIr>() :
                                                            _context.GroupIrs
                                                            .Include(o => o.Repairs)
                                                                 .ThenInclude(d => d.DocumentRepairs)
                                                            .Include(t => t.Inspections)
                                                                 .ThenInclude(d => d.AttendeeInspecs)
                                                            .Include(t => t.Inspections)
                                                                 .ThenInclude(d => d.PhotosInspecs)
                                                            .Where(r => r.IdServiceDetail != servide_detail_id && r.HousingListId == key)
                                                            .OrderByDescending(h => h.CreatedDate).ToList()),


               }).ToList();

            var _hosuging = consult[0];
            return new ObjectResult(_hosuging);
        }

        public det_grales_servicio atributos_generales(int id_service, int type_sr)
        {
            var wos_id = 0;


            switch (type_sr)
            {
                case 17://Pre-Decision Orientation
                    wos_id = _context.PredecisionOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 18://Area Orientation
                    wos_id = _context.AreaOrientations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 19://Settling In
                    wos_id = _context.SettlingIns.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 20://Schooling Search
                    wos_id = _context.SchoolingSearches.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 21://Departure
                    wos_id = _context.Departures.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 22://Temporary Housing Coordinaton
                    wos_id = _context.TemporaryHousingCoordinatons.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 23://Rental Furniture Coordination
                    wos_id = _context.RentalFurnitureCoordinations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 24://Transportation
                    wos_id = _context.Transportations.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 25://Airport Transportation Services
                    wos_id = _context.AirportTransportationServices.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 26://Home Finding
                    wos_id = _context.HomeFindings.FirstOrDefault(s => s.Id == id_service).WorkOrderServicesId;
                    break;

                case 27://Lease Renewal
                    wos_id = _context.LeaseRenewals.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 28://Home Sale
                    wos_id = _context.HomeSales.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 29://Home Purchase
                    wos_id = _context.HomePurchases.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 30://Property Management
                    wos_id = _context.PropertyManagements.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 32://Tenancy Management
                    wos_id = _context.TenancyManagements.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;

                case 31://Other
                    wos_id = _context.Others.FirstOrDefault(s => s.Id == id_service).WorkOrderServices.Value;
                    break;
            }

            det_grales_servicio void_r = new det_grales_servicio();

            if (wos_id != 0)
            {
                var standalone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderServiceId == wos_id)
                    .Select(s => new det_grales_servicio
                    {
                        country_name = s.DeliveringInNavigation.Name,
                        location = s.Location,
                        country = s.DeliveringIn,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.WorkOrderId,
                        sr_id = s.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(a => a.Id == s.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = type_sr
                    }).ToList();

                if (standalone.Count > 0)
                {
                    return standalone[0];
                }
                else
                {
                    var bundled = _context.BundledServices
                    .Where(x => x.WorkServicesId == wos_id)
                    .Select(s => new det_grales_servicio
                    {
                        country_name = s.DeliveringInNavigation.Name,
                        location = s.Location,
                        country = s.DeliveringIn,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.BundledServiceOrderId,
                        sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(j => j.Id == s.BundledServiceOrder.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = type_sr
                    }).ToList();

                    if (bundled.Count > 0)
                    {
                        return bundled[0];
                    }
                }
                return void_r;
            }
            else
            {
                return void_r;
            }
        }


        public det_grales_servicio atributos_generales_by_wosId(int wos_id)
        {
           

            det_grales_servicio void_r = new det_grales_servicio();

            if (wos_id != 0)
            {
                var standalone = _context.StandaloneServiceWorkOrders
                    .Where(x => x.WorkOrderServiceId == wos_id)
                    .Select(s => new det_grales_servicio
                    {
                        country_name = s.DeliveringInNavigation.Name,
                        location = s.Location,
                        country = s.DeliveringIn,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.WorkOrderId,
                        sr_id = s.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(a => a.Id == s.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = s.CategoryId
                    }).ToList();

                if (standalone.Count > 0)
                {
                    return standalone[0];
                }
                else
                {
                    var bundled = _context.BundledServices
                    .Where(x => x.WorkServicesId == wos_id)
                    .Select(s => new det_grales_servicio
                    {
                        country_name = s.DeliveringInNavigation.Name,
                        location = s.Location,
                        country = s.DeliveringIn,
                        type = s.ServiceId,
                        wos_id = wos_id,
                        wo_id = s.BundledServiceOrderId,
                        sr_id = s.BundledServiceOrder.WorkOrder.ServiceRecordId,
                        ass_id = _context.AssigneeInformations.FirstOrDefault(a => a.ServiceRecordId == s.BundledServiceOrder.WorkOrder.ServiceRecordId).Id,
                        id_client = _context.ServiceRecords.FirstOrDefault(j => j.Id == s.BundledServiceOrder.WorkOrder.ServiceRecordId).ClientId,
                        creation = s.CreatedDate,
                        service_category = s.CategoryId
                    }).ToList();

                    if (bundled.Count > 0)
                    {
                        return bundled[0];
                    }
                }
                return void_r;
            }
            else
            {
                return void_r;
            }
        }

        public HousingList create_only_lsf_registry(int key, int servide_detail_id)
        {

            //var _hosuging = _context.HousingLists.FirstOrDefault(x => x.Id == key);

            if (_context.RenewalDetailHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList().Count < 1)
            {
                RenewalDetailHome renewal = new RenewalDetailHome();
                renewal.CreatedDate = DateTime.Now;
                renewal.Id = key;
                renewal.IdServiceDetail = servide_detail_id;
                RenewalDetailHome hl = AddRenewalDetailHome(renewal);
                // _hosuging.RenewalDetailHomes.Add(hl);
                //con_Cambios = true;
            }

            if (_context.ContractDetails.Where(r => r.IdServiceDetail == servide_detail_id && r.ContractDetailId == key).ToList().Count < 1)
            {
                ContractDetail contracts = new ContractDetail();
                contracts.CreatedDate = DateTime.Now;
                contracts.ContractDetailId = key;
                contracts.IdServiceDetail = servide_detail_id;
                ContractDetail hl = AddContractDetail(contracts);
                //_hosuging.ContractDetails.Add(hl);
                //con_Cambios = true;
            }

            if (_context.LandlordHeaderDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                LandlordHeaderDetailsHome land = new LandlordHeaderDetailsHome();
                land.CreatedDate = DateTime.Now;
                land.HousingListId = key;
                land.IdServiceDetail = servide_detail_id;
                LandlordHeaderDetailsHome hl = AddLandlordHeaderDetailsHomee(land);
                // _hosuging.LandlordHeaderDetailsHomes.Add(hl);
                //con_Cambios = true;
            }

            if (_context.DepartureDetailsHomes.Where(r => r.IdServiceDetail == servide_detail_id && r.Id == key).ToList().Count < 1)
            {
                DepartureDetailsHome dep = new DepartureDetailsHome();
                dep.CreatedDate = DateTime.Now;
                dep.Id = key;
                dep.IdServiceDetail = servide_detail_id;
                DepartureDetailsHome hl = AddDepartureDetailHome(dep);
                // _hosuging.DepartureDetailsHomes.Add(hl);
                //con_Cambios = true;
            }

            if (_context.GroupCostSavings.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                GroupCostSaving gc = new GroupCostSaving();
                gc.CreatedDate = DateTime.Now;
                gc.HousingListId = key;
                gc.IdServiceDetail = servide_detail_id;
                GroupCostSaving hl = AddGroupCostSaving(gc);
                // _hosuging.GroupCostSavings.Add(hl);
                //con_Cambios = true;
            }

            if (_context.GroupPaymnetsHousings.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                GroupPaymnetsHousing dep = new GroupPaymnetsHousing();
                dep.CreatedDate = DateTime.Now;
                dep.HousingListId = key;
                dep.IdServiceDetail = servide_detail_id;
                GroupPaymnetsHousing hl = AddGroupPaymnetsHousing(dep);
                // _hosuging.GroupPaymnetsHousings.Add(hl);
                //con_Cambios = true;
            }

            var _hosuging_resutl = _context.HousingLists.FirstOrDefault(x => x.Id == key);
            return _hosuging_resutl;
        }

        public HousingList create_only_ir_registry(int key, int servide_detail_id)
        {

            // Move In 
            if (_context.PropertyReports.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingList == key && r.PropertyInspection == 1).ToList().Count < 1)
            {
                biz.premier.Entities.PropertyReport pr = new biz.premier.Entities.PropertyReport();
                pr.CreatedDate = DateTime.Now;
                pr.HousingList = key;
                pr.PropertyInspection = 1;
                pr.IdServiceDetail = servide_detail_id;
                biz.premier.Entities.PropertyReport prt = AddPrpertyReport(pr);
                //dto.PropertyReports.Add(pr);
            }

            // Move Out
            if (_context.PropertyReports.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingList == key && r.PropertyInspection == 2).ToList().Count < 1)
            {
                biz.premier.Entities.PropertyReport pr = new biz.premier.Entities.PropertyReport();
                pr.CreatedDate = DateTime.Now;
                pr.HousingList = key;
                pr.PropertyInspection = 2;
                pr.IdServiceDetail = servide_detail_id;
                biz.premier.Entities.PropertyReport prt = AddPrpertyReport(pr);
            }


            if (_context.GroupIrs.Where(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == key).ToList().Count < 1)
            {
                biz.premier.Entities.GroupIr gir = new biz.premier.Entities.GroupIr();
                gir.CreatedDate = DateTime.Now;
                gir.IdServiceDetail = servide_detail_id;
                gir.HousingListId = key;
                biz.premier.Entities.GroupIr gir1 = AddGroupIr(gir);
            }

            var _hosuging_resutl = _context.HousingLists.FirstOrDefault(x => x.Id == key);
            return _hosuging_resutl;
        }

        public List<Inspection> UpdateInspection(Inspection inspection)
        {
            _context.Inspections.Update(inspection);
            _context.SaveChanges();

            var inspections = _context.Inspections.Where(i => i.GroupIrId == inspection.GroupIrId)
                .Include(p => p.PhotosInspecs)
                .Include(a => a.AttendeeInspecs)
                .ToList();
            return inspections;
        }

        public List<PhotosInspec> DeletePhotoInspection(int id)
        {
            var consult = _context.PhotosInspecs.SingleOrDefault(s => s.Id == id);
            var gid = consult.Inspection;
            if (consult != null)
            {
                consult.Inspection = null;
                _context.PhotosInspecs.Update(consult);
                _context.SaveChanges();
            }

            var PhotosInspecs = _context.PhotosInspecs.Where(i => i.Inspection == gid)
               
                .ToList();
            return PhotosInspecs;
        }


        public List<Inspection> AddInspection(Inspection inspection)
        {
            _context.Inspections.Add(inspection);
            _context.SaveChanges();

            var inspections = _context.Inspections.Where(i => i.GroupIrId == inspection.GroupIrId)
                .Include(p => p.PhotosInspecs)
                .Include(a => a.AttendeeInspecs)
                .ToList();
            return inspections;
        }

        public List<Inspection> DeleteInspection(int id)
        {
            

            var consult = _context.Inspections.SingleOrDefault(s => s.Id == id);
            var gid = consult.GroupIrId;
            if (consult != null)
            {
                consult.GroupIrId = null;
                _context.Inspections.Update(consult);
                _context.SaveChanges();
            }

            var inspections = _context.Inspections.Where(i => i.GroupIrId == gid)
                .Include(p => p.PhotosInspecs)
                .Include(a => a.AttendeeInspecs)
                .ToList();
            return inspections;
        }

        public List<Repair> AddRepair(Repair repair)
        {
            _context.Repairs.Add(repair);
            _context.SaveChanges();

            var repairlist = _context.Repairs.Where(r => r.GroupIrId == repair.GroupIrId)
                .Include(i=> i.DocumentRepairs)
                .ToList();
            return repairlist ;
        }

        public List<Repair> UpdateRepair(Repair repair)
        {
            var consult = _context.Repairs
                .Include(i => i.DocumentRepairs)
                .SingleOrDefault(s => s.Id == repair.Id);
            if (consult != null)
            {
                _context.Entry(consult).CurrentValues.SetValues(repair);

                foreach (var i in repair.DocumentRepairs)
                {
                    var document = consult.DocumentRepairs.SingleOrDefault(x => x.Id == i.Id);
                    if (document == null)
                    {
                        consult.DocumentRepairs.Add(i);
                    }
                    else
                    {
                        _context.Entry(document).CurrentValues.SetValues(i);
                    }
                }

                _context.SaveChanges();
            }

            var repairlist = _context.Repairs.Where(r => r.GroupIrId == repair.GroupIrId)
                .Include(i => i.DocumentRepairs)
                .ToList();
            return repairlist;
        }

        public List<Repair> DeleteRepair(int id)
        {


            var consult = _context.Repairs.SingleOrDefault(s => s.Id == id);
            var gid = consult.GroupIrId;
            if (consult != null)
            {
                consult.GroupIrId = null;
                _context.Repairs.Update(consult);
                _context.SaveChanges();
            }

            var repairlist = _context.Repairs.Where(i => i.GroupIrId == gid)
                .Include(p => p.DocumentRepairs)
                .ToList();
            return repairlist;
        }

        public List<DocumentRepair> DeleteDocumentRepair(int id)
        {
            var consult = _context.DocumentRepairs.SingleOrDefault(s => s.Id == id);
            var gid = consult.RepairId;
                _context.DocumentRepairs.Remove(consult);
                _context.SaveChanges();
            var repairlist = _context.DocumentRepairs.Where(i => i.RepairId == gid)
                .ToList();
            return repairlist;
        }

        public ActionResult GetHomestovist(int wosid, DateTime? dateViste)
        {
            var id_service_detail = _context.HomeFindings.FirstOrDefault(s => s.WorkOrderServicesId == wosid).Id;

            var custom = _context.HousingLists.Where(i => i.IdServiceDetail == id_service_detail).Select(s => new
            {
                s.Id,
                supplier = s.SupplierNavigation == null ? "" : s.SupplierNavigation.ContactName,
                supplierCompany = s.SupplierNavigation == null ? null : s.SupplierPartnerNavigation.ComercialName,
                s.PropertyNo,
                PropertyType = s.PropertyTypeNavigation == null ? null : s.PropertyTypeNavigation.PropertyType,
                s.Address,
                s.Price,
                s.Neighborhood,
                s.VisitTime,
                s.VisitDate,
                Currency = s.CurrencyNavigation == null ? null : s.CurrencyNavigation.Currency,
                Status = s.HousingStatusNavigation == null ? null : s.HousingStatusNavigation.Status,
                s.Wassended,
                s.Bedrooms,
                s.Bathrooms,
                s.ParkingSpaces,
                Sise = 0,
                AssigneComments = "",
                RelAppointmentHousingLists = s.RelAppointmentHousingLists.Any()

            }).ToList();

            if (dateViste != null)
            {
                custom = custom.Where(s => (s.VisitDate != null ? s.VisitDate.Value.ToShortDateString() : Convert.ToDateTime("01/01/1900").ToShortDateString()) == dateViste.Value.ToShortDateString()).ToList();
            }

            return new ObjectResult(custom); ;

        }

        public List<PhotosPropertyReportSection> delete_photo_section(int id)
        {
            var consult = _context.PhotosPropertyReportSections.SingleOrDefault(s => s.Id == id);
            var gid = consult.PropertyReportSectionId;
            _context.PhotosPropertyReportSections.Remove(consult);
            _context.SaveChanges();
            var photoslist = _context.PhotosPropertyReportSections.Where(i => i.PropertyReportSectionId == gid)
                .ToList();
            return photoslist;
        }

        public List<PhotosInventory> delete_photo_section_inventory(int id)
        {
            var consult = _context.PhotosInventories.SingleOrDefault(s => s.Id == id);
            var gid = consult.SectionInventory;
            _context.PhotosInventories.Remove(consult);
            _context.SaveChanges();
            var photoslist = _context.PhotosInventories.Where(i => i.SectionInventory == gid)
                .ToList();
            return photoslist;
        }


        public List<PhotosInspec> delete_photo_inspection(int id)
        {
            var consult = _context.PhotosInspecs.SingleOrDefault(s => s.Id == id);
            var gid = consult.Inspection;
            _context.PhotosInspecs.Remove(consult);
            _context.SaveChanges();
            var photoslist = _context.PhotosInspecs.Where(i => i.Inspection == gid)
                .ToList();
            return photoslist;
        }


        public HousingList accept_reject_property(int id, int status_id)
        {
            var consult = _context.HousingLists.SingleOrDefault(s => s.Id == id);
            consult.HousingStatus = status_id;

            _context.HousingLists.Update(consult);
            _context.SaveChanges();
           
            return consult;
        }

        public ActionResult GetStatusIR(int servide_detail_id, int id_housing)
        {
            var consult = _context.HousingLists
                .Where(x => x.Id == id_housing).Select(s => new
                {
                    PropertyReports = _context.PropertyReports
                    .Select(r => new {
                        r.IdServiceDetail
                        , r.HousingList
                        , StatusText = r.IdStatus.HasValue ? r.IdStatusNavigation.Status : "Pending"
                        , Status = r.IdStatus
                    })
                    .Where(r => r.HousingList == id_housing && r.IdServiceDetail == servide_detail_id).ToList(),
                    groupIr = _context.GroupIrs
                    .Select(r => new {
                        r.IdServiceDetail
                        , r.HousingListId
                        , StatusText = r.IdStatus.HasValue ? _context.StatusPropertyReports.FirstOrDefault(f => f.Id == r.IdStatus.Value).Status : "Pending"
                        , Status = r.IdStatus
                    })
                    .FirstOrDefault(r => r.IdServiceDetail == servide_detail_id && r.HousingListId == id_housing)

                }).ToList();

            return new ObjectResult(consult[0]);
        }
    }
}
