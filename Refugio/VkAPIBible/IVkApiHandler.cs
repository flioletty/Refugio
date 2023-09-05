using System;
using Newtonsoft.Json.Linq;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkApi
{
    public interface IVkApiHandler
    {
        public List<VkNet.Model.User> GetListOfSubscribersOfTheGroupInVk();

        public List<DBModels.Group> GetUserSubscriptionInformationById(string userId);

        public DBModels.Group CreatingAGroupModel(JEnumerable<JToken> informationAboutUser);

        public List<DBModels.User> GetInformationAboutGroupUsersById(); 

        public DBModels.User GetUserInformationById(string userId);

        public DBModels.User CreatingAUserModel(JEnumerable<JToken> informationAboutUser, string usId);

        public List<VkNet.Model.User> GetTheUserFriendsById(string userId);

        public WallGetObject GetRecordingsFromTheUserWallByUserId(string userId);

        public List<VkNet.Model.Attachments.Audio>? GetAllTheUserMusicByUserId(string userId);

        public List<VkNet.Model.Attachments.Photo> GetAllTheUserPhotosByUserId(string userId);

        public List<VkNet.Model.Attachments.Video> GetAllTheUserVideosByUserId(string userId);

        public void Test();
    }
}

