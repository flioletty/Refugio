using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBModels;
using Microsoft.EntityFrameworkCore;

namespace DBCore
{
    public class PostgresSQLReposUser : BaseRepository, IUserRepository
    {
        public PostgresSQLReposUser(VKContext context) : base(context) { }

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void AddUser(User user)
        {
            try
            {
                User us = _context.Users.Where(x => x.VkId == user.VkId).FirstOrDefault();

                if (us == null)
                {
                    CreateUser(user);
                }
                else
                {
                    UpdateUser(user, us);
                    _context.Entry(us).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                var b = ex.Message;
            }
        }
        public void DeleteUser(long Id)
        {
            User user = _context.Users.Where(x => x.Id == Id).FirstOrDefault();

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            else throw new Exception("User with such id doesn't exist");
        }

        public List<Group> GetListOfGroups(long Id)
        {
            User us = _context.Users.Where(u => u.Id == Id).FirstOrDefault();

            if (us == null)
                throw new Exception("User with such id doesn't exist");

            return us.Groups.ToList();
        }

        public int CountOfUsers()
        {
            return _context.Users.Count();
        }

        public List<User> GetUsers()
        {
            try
            {
                return _context.Users.Include(x => x.Groups).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void UpdateUser(User NewUser, User OldUser)
        {
            if(OldUser != null)
            {
                if(OldUser.Country != NewUser.Country)
                    OldUser.Country = NewUser.Country;
                if(OldUser.FirstName != NewUser.FirstName)
                    OldUser.FirstName = NewUser.FirstName;
                if(OldUser.LastName != NewUser.LastName)
                    OldUser.LastName = NewUser.LastName;
                if(OldUser.Activities!= NewUser.Activities)
                    OldUser.Activities = NewUser.Activities;
                if(OldUser.City != NewUser.City)
                    OldUser.City = NewUser.City;
                if(OldUser.FacultyName != NewUser.FacultyName)
                    OldUser.FacultyName = NewUser.FacultyName;
                if(OldUser.Nickname != NewUser.Nickname)
                    OldUser.Nickname = NewUser.Nickname;
                if(OldUser.University != NewUser.University)
                    OldUser.University = NewUser.University;
                if(OldUser.Sex != NewUser.Sex)
                    OldUser.Sex = NewUser.Sex;

                _context.Entry(OldUser).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void UpdateActivitiesUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges(); 
        }

        public List<string?> GetUsersBirthdays()
        {
            try
            {
                return _context.Users.Where(x => x.BirthDate != null).Select(x => x.BirthDate).Distinct().ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<string?> GetUserCities()
        {
            try
            {
                return _context.Users.Where(x => x.City != null).Select(x => x.City).Distinct().ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<string?> GetUserUniversities()
        {
            try
            {
                return _context.Users.Where(x => x.University != null).Select(x => x.University).Distinct().ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<string?> GetUserFaculties()
        {
            try
            {
                return _context.Users.Where(x => x.FacultyName != null).Select(x => x.FacultyName).Distinct().ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
