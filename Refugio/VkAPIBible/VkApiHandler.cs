using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using System.Text.Json;
using AngleSharp.Io;
using DBModels;
using System.Threading.Tasks;
using VkAPI.Exceptions;

namespace VkApi
{
    public class VkApiHandler : IVkApiHandler
    {
        private VkNet.VkApi apiVk;

        private ulong stepByGroupSubscribers = 1000;

        public VkApiHandler()
        {
            apiVk = AuthorizationInVk();
        }

        private VkNet.VkApi AuthorizationInVk()
        {
            try
            {
                var apiVk = new VkNet.VkApi();

                apiVk.Authorize(new ApiAuthParams
                {
                    ApplicationId = Secret.ApplicationId,
                    AccessToken = Secret.Token,
                    Settings = Settings.All
                });

                return apiVk;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<VkNet.Model.User> GetListOfSubscribersOfTheGroupInVk()
        {
            try
            {
                var numberOfSubscribersOfTheGroup = this.apiVk.Groups.GetMembers(new GroupsGetMembersParams()
                {
                    GroupId = GroupId.TypicalMSTU,
                }).TotalCount;

                var listOfUserIds = new List<long>();

                for (ulong i = 0; i < numberOfSubscribersOfTheGroup; i += this.stepByGroupSubscribers)
                {
                    var subscribersOfTheGroup = apiVk.Groups.GetMembers(new GroupsGetMembersParams()
                    {
                        GroupId = GroupId.TypicalMSTU,
                        Offset = (long?)i
                    });

                    listOfUserIds.AddRange((IEnumerable<long>)subscribersOfTheGroup.Select(x => x.Id).ToList());
                }

                var subscribers = apiVk.Users.Get(listOfUserIds.Take((int)this.stepByGroupSubscribers))
                                                               .Where(p => p.IsClosed == false
                                                                   && p.IsDeactivated == false
                                                                   && p.BanInfo == null).ToList();

                for (ulong i = this.stepByGroupSubscribers;
                     i < numberOfSubscribersOfTheGroup;
                     i += this.stepByGroupSubscribers)
                {
                    subscribers.AddRange(apiVk.Users.Get(listOfUserIds.Skip(Convert.ToInt32(i))
                                                    .Take((int)this.stepByGroupSubscribers))
                                                    .Where(x => x.IsClosed == false && x.IsDeactivated == false).ToList());
                }

                return subscribers;
            }
            catch (RequestException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<DBModels.Group> GetUserSubscriptionInformationById(string userId)
        {
            try
            {
                var us = apiVk.Users.Get(new List<long>() { (long)Convert.ToInt32(userId) });
                if (us[0].BanInfo != null || us[0].IsDeactivated == true || us[0].IsClosed == true)
                    return null;

                var userSubscriptions = apiVk.Users.GetSubscriptions(Convert.ToInt32(userId), 50);

                var groups = apiVk.Groups.GetById((IEnumerable<string>)userSubscriptions.Select(x => x.Id.ToString()).Take(500).ToList(),
                   null, GroupsFields.All).Where(x => x.IsClosed == VkNet.Enums.GroupPublicity.Public && x.Deactivated == null).Select(x => x.Id.ToString()).ToList();

                var groupInformation = new List<DBModels.Group>();
                foreach (var group in groups)
                {
                    var address = $"https://api.vk.com/method/groups.getById?group_id={group}&fields=name,type,activity,city,country,description,members_count,place&access_token={Secret.Token}&v=5.131";

                    var config = Configuration.Default.WithDefaultLoader();

                    var document = BrowsingContext.New(config).OpenAsync(address).Result;

                    var dataJson = document.Body.TextContent.ToString();

                    var json = JObject.Parse(dataJson);

                    var informationAboutGroup = json["response"].ToList()[0].Children();

                    groupInformation.Add(CreatingAGroupModel(informationAboutGroup));

                    Thread.Sleep(200);
                }

                return groupInformation;
            }
            catch (AggregateException exception)
            {
                throw exception;
            }
            catch (RequestException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public DBModels.Group CreatingAGroupModel(JEnumerable<JToken> informationAboutGroup)
        {
            var group = new DBModels.Group();

            try
            {
                foreach (var inf in informationAboutGroup)
                {
                    if (inf.ToString().Contains("name"))
                    {
                        group.Name = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("type"))
                    {
                        group.Type = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("description"))
                    {
                        group.Description = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("members_count"))
                    {
                        group.MembersCount = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("activity"))
                    {
                        group.Activity = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("place"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        group.Place = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("city"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        group.City = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("country"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        group.Country = _dataJson["title"].ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return group;
        }

        public List<DBModels.User> GetInformationAboutGroupUsersById()
        {
            var userInformation = new List<DBModels.User>();

            try
            {
                var subscribersOfTheGroup = this.GetListOfSubscribersOfTheGroupInVk().Select(x => x.Id.ToString()).ToList();

                int count = 1;
                foreach (var groupSubscriber in subscribersOfTheGroup)
                {
                    userInformation.Add(GetUserInformationById(groupSubscriber));
                    if (count > 10)
                        break;
                    Thread.Sleep(200);
                    Console.WriteLine(count++);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return userInformation;
        }

        public DBModels.User GetUserInformationById(string userId)
        {
            try
            {
                //userId = "f100ma";
                var address = $"https://api.vk.com/method/users.get?user_ids={userId}&fields=uid,first_name,last_name,nickname,sex,bdate,city,country,education&access_token={Secret.Token}&v=5.131";

                var config = Configuration.Default.WithDefaultLoader();

                var document = BrowsingContext.New(config).OpenAsync(address).Result;

                var dataJson = document.Body.TextContent.ToString();

                var json = JObject.Parse(dataJson);

                var informationAboutUser = json["response"].ToList()[0].Children();

                return CreatingAUserModel(informationAboutUser, userId);
            }
            catch (AggregateException exception)
            {
                throw exception;
            }
            catch (RequestException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public DBModels.User CreatingAUserModel(JEnumerable<JToken> informationAboutUser, string usId)
        {
            var user = new DBModels.User();

            try
            {
                //user.VkId = long.Parse(usId);
                int countStr = 0;

                foreach (var inf in informationAboutUser)
                {
                    if (inf.ToString().Contains("nickname"))
                    {
                        user.Nickname = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("id")&&countStr==0)
                    {
                        user.VkId = long.Parse(inf.First.ToString());
                    }
                    else if (inf.ToString().Contains("city"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        user.City = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("country"))
                    {
                        var _dataJson = JObject.Parse(inf.First.ToString());

                        user.Country = _dataJson["title"].ToString();
                    }
                    else if (inf.ToString().Contains("university_name"))
                    {
                        user.University = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("faculty_name"))
                    {
                        user.FacultyName = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("sex"))
                    {
                        user.Sex = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("first_name"))
                    {
                        user.FirstName = inf.First.ToString();
                    }
                    else if (inf.ToString().Contains("last_name"))
                    {
                        user.LastName = inf.First.ToString();
                    }
                    countStr++;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return user;
        }

        public List<VkNet.Model.User> GetTheUserFriendsById(string userId)
        {
            try
            {
                var userFriends = apiVk.Friends.Get(new FriendsGetParams()
                {
                    UserId = Convert.ToInt32(userId)
                }).Where(x => x.IsClosed == false).ToList();

                return apiVk.Users.Get(userFriends.Select(x => x.Id).ToList()).Where(x => x.IsClosed == false).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public WallGetObject GetRecordingsFromTheUserWallByUserId(string userId)
        {
            try
            {
                return apiVk.Wall.Get(new WallGetParams()
                {
                    OwnerId = Convert.ToInt32(userId)
                });
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<VkNet.Model.Attachments.Audio>? GetAllTheUserMusicByUserId(string userId)
        {
            try
            {
                if (apiVk.Users.Get(new List<long> { Convert.ToInt32(userId) }).Where(x => x.Music != null).Count() == 0)
                    return null;

                return apiVk.Audio.Get(new AudioGetParams()
                {
                    OwnerId = Convert.ToInt32(userId)
                }).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<VkNet.Model.Attachments.Photo> GetAllTheUserPhotosByUserId(string userId)
        {
            try
            {
                var photos = apiVk.Photo.GetAll(new PhotoGetAllParams
                {
                    OwnerId = Convert.ToInt32(userId)
                }).Select(x => x.Id.ToString()).ToList();

                return apiVk.Photo.GetById(photos, true).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public List<VkNet.Model.Attachments.Video> GetAllTheUserVideosByUserId(string userId)
        {
            try
            {
                return apiVk.Video.Get(new VideoGetParams
                {
                    OwnerId = Convert.ToInt32(userId)
                }).ToList();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public void Test()
        {

        }
    }
}