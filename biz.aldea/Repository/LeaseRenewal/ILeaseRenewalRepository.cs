namespace biz.premier.Repository.LeaseRenewal
{
    public interface ILeaseRenewalRepository : IGenericRepository<Entities.LeaseRenewal>
    {
        Entities.LeaseRenewal GetCustom(int key);
        Entities.LeaseRenewal UpdateCustom(Entities.LeaseRenewal leaseRenewal);
        bool DeleteReminder(int key);
        Entities.DocumentLeaseRenewal FindDocument(int key);
        Entities.DocumentLeaseRenewal DeleteDocument(int key);
    }
}