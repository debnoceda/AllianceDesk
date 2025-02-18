﻿using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsers();
        bool UserExists(string email);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(Guid userId);
        IEnumerable<User> GetUsersByIds(IEnumerable<Guid> userIds);
        User GetUserById(Guid userId);
    }
}
