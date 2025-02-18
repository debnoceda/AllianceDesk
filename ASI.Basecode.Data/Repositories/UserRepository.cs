﻿using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public IQueryable<User> GetUsers()
        {
            return this.GetDbSet<User>();
        }

        public bool UserExists(string email)
        {
            return this.GetDbSet<User>().Any(x => x.Email == email);
        }

        public void AddUser(User user)
        {
            this.GetDbSet<User>().Add(user);
            UnitOfWork.SaveChanges();
        }

        public void UpdateUser(User user) 
        {
            this.GetDbSet<User>().Update(user);
            UnitOfWork.SaveChanges();
        }

        public void DeleteUser(Guid userId)
        {
            var deleteUser = this.GetDbSet<User>().Where(u => u.UserId == userId).FirstOrDefault();
            if (deleteUser != null)
            {
                this.GetDbSet<User>().Remove(deleteUser);
                UnitOfWork.SaveChanges();
            }
        }
       
        public User GetUserById(Guid userId)
        {
            return this.GetDbSet<User>().Where(u => u.UserId == userId).FirstOrDefault();
        }
        
        public IEnumerable<User> GetUsersByIds(IEnumerable<Guid> userIds)
        {
           return this.GetDbSet<User>().Where(x => userIds.Contains(x.UserId));
        }
    }
}