using System;
using System.Collections.Generic;
using DBModels;

namespace DBCore
{
    public interface IUserRepository
    {
        void CreateUser(User user);

        void AddUser(User user);

        void DeleteUser(long Id);

        List<Group> GetListOfGroups(long Id);

        int CountOfUsers();

        List<User> GetUsers();

        void UpdateUser(User NewUser, User OldUser);

        void UpdateActivitiesUser(User user);

        List<string?> GetUsersBirthdays();

        List<string?> GetUserCities();

        List<string?> GetUserUniversities();

        List<string?> GetUserFaculties();
    }
}
