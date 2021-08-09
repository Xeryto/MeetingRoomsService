﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.DAL
{
    public class FakeRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IId
    {
        protected readonly List<TEntity> dbset = new List<TEntity>();

        public FakeRepository() { }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                dbset.Add(entity);

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(AddAsync)} could not be saved: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return dbset;
            }
            catch (Exception ex)
            {
                throw new Exception($"Couldn't retrieve entities: {ex.Message}");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                dbset[dbset.FindIndex(x => x.Id == entity.Id)] = entity;
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated: {ex.Message}");
            }
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = dbset.FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new Exception($"Entity with id {id} doesn't exist");
            }

            return entity;
        }

        public async Task<TEntity> Delete(int id)
        {
            var entity = await GetByIdAsync(id);

            dbset.Remove(entity);
            return entity;
        }

        public IQueryable<TEntity> Query()
        {
            return dbset.AsQueryable();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}