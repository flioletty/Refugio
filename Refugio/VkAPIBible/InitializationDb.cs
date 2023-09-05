using System;
using VkApi;
using DBCore;

namespace VkAPI
{
    public class InitializationDb
    {
        private IGroupRepository _groupDb;

        private IUserRepository _userDb;

        public InitializationDb(IGroupRepository groupDb,
                                IUserRepository userDb)
        {
            _groupDb = groupDb;
            _userDb = userDb;
        }

        public void Initialization()
        {
            try
            {
                var vk = new VkApiHandler();
                //var UsersFromVk = vk.GetInformationAboutGroupUsersById();
                var UsersFromVk = _userDb.GetUsers().Skip(2169).Take(1000).ToList();
                var count = 1;
                foreach (var user in UsersFromVk)
                {
                    //db.AddUser(user);
                    var groups = vk.GetUserSubscriptionInformationById(user.VkId.ToString());

                    if (groups == null)
                        continue;

                    Console.WriteLine(groups.Count());
                    foreach (var group in groups)
                    {
                        _groupDb.AddGroup(group, user);
                    }
                    Console.WriteLine(count++);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}

