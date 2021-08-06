﻿using BusinessLogic.DAL;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class UserService
    {
        private readonly IGenericRepository<User> _genericRepository;

        public UserService(IGenericRepository<User> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            return await _genericRepository.Query().ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _genericRepository.Query().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByLogin(string login)
        {
            return await _genericRepository.Query().Where(x => x.Login == login).FirstOrDefaultAsync();
        }

        public async Task<User> GetByName (string name)
        {
            return await _genericRepository.Query().Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<bool> CheckExists (string login)
        {
            return await _genericRepository.Query().AnyAsync(x => x.Login == login);
        }

        public async Task<User> Add(User user)
        {
            user.Password = Hash(user.Password);
            return await _genericRepository.AddAsync(user);
        }

        public async Task<User> Delete(int id)
        {
            return await _genericRepository.Delete(id);
        }

        public async Task<User> Update(User user)
        {
            user.Password = Hash(user.Password);
            return await _genericRepository.UpdateAsync(user);
        }

        public async Task<bool> Login(string login, string password)
        {
            return VerifyHashed((await GetByLogin(login)).Password, password);
        }

        private static string Hash(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }

        private static bool VerifyHashed(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }

        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}