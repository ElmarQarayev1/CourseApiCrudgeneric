using System;
using System.Linq.Expressions;
using Course.Data.Repostories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Course.Data.Repostories.Implementations
{
	public class Repository<TEntity>:IRepository<TEntity> where TEntity:class
	{
        private readonly AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var item in includes)
                query = query.Include(item);


            return query.Any(predicate);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var item in includes)
                query = query.Include(item);

            return query.FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            foreach (var item in includes)
                query = query.Include(item);

            return query.Where(predicate);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}

