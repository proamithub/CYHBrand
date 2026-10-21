using CYHBrandData;
using CYHBrandData.Dapper;
using CYHBrandRepo.IRepository;
using CYHBrandRepo.Repository;
using CYHBrandRepo.Utilities;
using Microsoft.Extensions.Logging;

namespace CYHBrandRepo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext    _db;
        private readonly IDapperContext _dapper;
        private readonly ILoggerFactory _loggerFactory;

        public UnitOfWork(DataContext db, IDapperContext dapper,
                          IMailClient mailClient, ILoggerFactory loggerFactory)
        {
            _db            = db;
            _dapper        = dapper;
            _loggerFactory = loggerFactory;
        }

        // ─── Doctor ──────────────────────────────────────────────────────────
        private IDoctorRepository? _doctorRepo;
        public IDoctorRepository DoctorRepo =>
            _doctorRepo ??= new DoctorRepository(_db, _dapper,
                _loggerFactory.CreateLogger<DoctorRepository>());

        // ─── Template ────────────────────────────────────────────────────────
        private ITemplateRepository? _templateRepo;
        public ITemplateRepository TemplateRepo =>
            _templateRepo ??= new TemplateRepository(_db, _dapper,
                _loggerFactory.CreateLogger<TemplateRepository>());

        // ─── Design ──────────────────────────────────────────────────────────
        private IDesignRepository? _designRepo;
        public IDesignRepository DesignRepo =>
            _designRepo ??= new DesignRepository(_db, _dapper,
                _loggerFactory.CreateLogger<DesignRepository>());

        public int SaveChanges() => _db.SaveChanges();
        public void Dispose()    => _db.Dispose();
    }
}
