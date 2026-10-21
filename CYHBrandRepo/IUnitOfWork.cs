using CYHBrandRepo.IRepository;

namespace CYHBrandRepo
{
    public interface IUnitOfWork : IDisposable
    {
        IDoctorRepository   DoctorRepo   { get; }
        ITemplateRepository TemplateRepo { get; }
        IDesignRepository   DesignRepo   { get; }
        int SaveChanges();
    }
}
