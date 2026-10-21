using CYHBrandRepo.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CYHBrandRepo.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _db;

        public Repository(DbContext db) => _db = db;

        public IEnumerable<TEntity> GetAll()
            => _db.Set<TEntity>().ToList();

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
            => _db.Set<TEntity>().Where(predicate);

        public TEntity? GetById(object id)
            => _db.Set<TEntity>().Find(id);

        public void Add(TEntity entity)
            => _db.Set<TEntity>().Add(entity);

        public void AddRange(IEnumerable<TEntity> entities)
            => _db.Set<TEntity>().AddRange(entities);

        public void Update(TEntity entity)
            => _db.Entry(entity).State = EntityState.Modified;

        public void Delete(TEntity entity)
            => _db.Set<TEntity>().Remove(entity);

        public void Delete(object id)
        {
            var entity = _db.Set<TEntity>().Find(id);
            if (entity != null) Delete(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
            => _db.Set<TEntity>().RemoveRange(entities);
    }
}
