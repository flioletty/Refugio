using System;
using DBCore;
using DTOModels;
using Microsoft.AspNetCore.Mvc;
using DBCore.Converters;
using VkAPI;
using Clusterization;
using ProjectModels;
using Microsoft.AspNetCore.Authorization;
using VkApi;

namespace RefugioApp.Controllers
{
    [ApiController]
    [Route(template: "api/[controller]")]
    public class ClussterizationController : Controller
	{
        private readonly Clusterization_ _clust;

        private readonly IGroupRepository _groupDb;

        private readonly IUserRepository _userDb;

        private readonly UserConverter _userConverter;

        private readonly SetInterests _interests;

        private readonly SetMetrics _metrics;

        private readonly CombiningInterests _cmb;

        private readonly ModelSaver _userWithActivities;

        public ClussterizationController(IGroupRepository groupDb,
                                         IUserRepository userDb,
                                         Clusterization_ clust,
                                         UserConverter userConverter,
                                         SetInterests interests,
                                         CombiningInterests cmb,
                                         SetMetrics metrics,
                                         ModelSaver modelSaver)
        {
            _groupDb = groupDb;
            _userDb = userDb;
            _clust = clust;
            _userConverter = userConverter;
            _interests = interests;
            _cmb = cmb;
            _metrics = metrics;
            _userWithActivities = modelSaver;
        }

        [HttpGet("SetUser/{Link}")]
        public async Task<IActionResult> SetUser(string Link)
        {
            IActionResult response;

            try
            {
                _userWithActivities.user = GetUserInterests(Link);

                response = Ok();
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("InterestsClusters")]
        public async Task<IActionResult> InterestsClussterisation()
        {
            IActionResult response;

            try
            {
                Thread.Sleep(10000);
                var usModel = GetUsers(_userWithActivities.user);
                if (usModel == null)
                    throw new Exception("choose user");

                var users = new List<UserModel>() { _userConverter.GetUserModel(_userWithActivities.user) };

                var clusters = _clust.ClusteringUsers(usModel, users, new List<int> { -1});

                var points = _clust.GetListOfPoints(users, new List<int> { -1 });

                var usersDto = new List<UserDto>();
                foreach (var cluster in clusters)
                {
                    usersDto.Add(_userConverter.GetUserDto(cluster));
                }

                response = Ok(usersDto.Take(50));
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("UniversitiesClusters")]
        public async Task<IActionResult> UniversitiesClussterisation()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.University == null)
                    return StatusCode(StatusCodes.Status204NoContent);

                var usModel = GetUsers(_userWithActivities.user);

                var secondCoord = _metrics.SetUniversitiesToNumbers(usModel);

                var clusters = _clust.ClusteringUsers(usModel, new List<UserModel>()
                {
                    _userConverter.GetUserModel(_userWithActivities.user)
                }, secondCoord);

                var usersDto = new List<UserDto>();
                foreach (var cluster in clusters)
                    usersDto.Add(_userConverter.GetUserDto(cluster));

                response = Ok(usersDto.Take(50));
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("FacultiesClussters")]
        public async Task<IActionResult> FacultiesClussterisation()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.FacultyName==null)
                    return StatusCode(StatusCodes.Status204NoContent);

                var usModel = GetUsers(_userWithActivities.user);

                var secondCoord = _metrics.SetFacultiesToNumbers(usModel);

                var clusters = _clust.ClusteringUsers(usModel, new List<UserModel>()
                {
                    _userConverter.GetUserModel(_userWithActivities.user)
                }, secondCoord);

                var usersDto = new List<UserDto>();
                foreach (var cluster in clusters)
                    usersDto.Add(_userConverter.GetUserDto(cluster));

                response = Ok(usersDto.Take(50));
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("GetPointsInterests")]
        public async Task<IActionResult> GetPointsInterests()
        {
            IActionResult response;

            try
            {
                //Thread.Sleep(10000);
                var users = GetUsers(_userWithActivities.user);

                var clusters = _clust.GetClusters(users, new List<int> { -1 });

                var pointDto = new List<PointDto>();
                int count = 0;
                foreach (var cluster in clusters.Clusters)
                {
                    foreach (var point in cluster.Objects)
                    {
                        if (point._user.VkId == _userWithActivities.user.VkId)
                        {
                            var us = (_userConverter.GetPointDto(point.Point.X, point.Point.Y, 20 * count));
                            us.color = "rgb(255,255,255)";
                            pointDto.Add(us);
                        }
                        else
                            pointDto.Add(_userConverter.GetPointDto(point.Point.X, point.Point.Y, 20 * count));
                    }
                    count++;
                }

                response = Ok(pointDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("GetPointsUniversities")]
        public async Task<IActionResult> GetPointsUniversities()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.University == null)
                    return StatusCode(StatusCodes.Status204NoContent);
                var users = GetUsers(_userWithActivities.user);

                var secondCoord = _metrics.SetUniversitiesToNumbers(users);

                var clusters = _clust.GetClusters(users, secondCoord);

                var pointDto = new List<PointDto>();
                int count = 0;
                foreach (var cluster in clusters.Clusters)
                {
                    foreach (var point in cluster.Objects)
                    {
                        if (point._user.VkId == _userWithActivities.user.VkId)
                        {
                            var us = (_userConverter.GetPointDto(point.Point.X, point.Point.Y, 20 * count));
                            us.color = "rgb(240,7,7)";
                            pointDto.Add(us);
                        }
                        else
                            pointDto.Add(_userConverter.GetPointDto(point.Point.X, point.Point.Y, 20 * count));
                    }
                    count++;
                }

                response = Ok(pointDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("GetPointsFaculties")]
        public async Task<IActionResult> GetPointsFaculties()
        {
            IActionResult response;

            try
            {
                if (_userWithActivities.user.FacultyName == null)
                    return StatusCode(StatusCodes.Status204NoContent);
                var users = GetUsers(_userWithActivities.user);

                var secondCoord = _metrics.SetFacultiesToNumbers(users);

                var clusters = _clust.GetClusters(users, secondCoord);

                var pointDto = new List<PointDto>();
                int count = 0;
                foreach (var cluster in clusters.Clusters)
                {
                    foreach (var point in cluster.Objects)
                    {
                        if (point._user.VkId == _userWithActivities.user.VkId)
                        {
                            var us = (_userConverter.GetPointDto(point.Point.X, point.Point.Y, 20 * count));
                            us.color = "rgb(240,7,7)";
                            pointDto.Add(us);
                        }
                        else
                            pointDto.Add(_userConverter.GetPointDto(point.Point.X, point.Point.Y, 20 * count));
                    }
                    count++;
                }

                response = Ok(pointDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        DBModels.User GetUserInterests(string Link)
        {
            if (Link.Contains('%'))
                Link = Link.Substring(Link.LastIndexOf('F') + 1);
            else if (Link.Contains('/'))
                Link = Link.Substring(Link.LastIndexOf('/') + 1);
            
            var user = new VkApiHandler().GetUserInformationById(Link);
            var groupsUser = new VkApiHandler().GetUserSubscriptionInformationById(user.VkId.ToString());
            var interests = new SetInterests();
            interests.Activity();

            return interests.SetActivitiesForOneUser(user, groupsUser, _cmb);
        }

        List<UserModel> GetUsers(DBModels.User userWithActivities)
        {
            var users = _userDb.GetUsers();
            //interests.SetActivities(users, _userDb, _cmb);
            List<UserModel> usModel = new List<UserModel>();
            foreach (var us in users)
                usModel.Add(_userConverter.GetUserModel(us));

            usModel.Add(_userConverter.GetUserModel(userWithActivities));
            return usModel;
        }
    }
}

