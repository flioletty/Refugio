using System;
using DBCore;
using DBModels;
using Dbscan;
using ProjectModels;

namespace Clusterization
{
    public class Clusterization_
    {
        public class SimplePoint : IPointData
        {
            public SimplePoint(double x, double y, UserModel user)
            {
                Point = new Point(x, y);
                _user = user;
            }
                
            public Point Point { get; }

            public UserModel _user { get; }
        }

        public HashSet<UserModel> ClusteringUsers(List<UserModel> users, List<UserModel> user, List<int> secondCoord)
        {
            try
            {
                HashSet<UserModel> userModels = new HashSet<UserModel>();
                
                var clusters = GetClusters(users, secondCoord);

                var result = this.SearchForUserInClusters(clusters, user, secondCoord);

                foreach(var cluster in result)
                {
                    foreach(var us in cluster.Objects)
                    {
                        userModels.Add(us._user);
                    }
                }

                new SetInterests().EstablishInterests(userModels);

                return userModels;
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        public ClusterSet<SimplePoint> GetClusters(List<UserModel> users, List<int> secondCoord)
        {
            var array = GetListOfPoints(users, secondCoord);

            var clusters = Dbscan.Dbscan.CalculateClusters(
                array,
                epsilon: 1,
                minimumPointsPerCluster: 1);

            return clusters;
        }

        public List<SimplePoint> GetListOfPoints(List<UserModel> users, List<int> secondCoord)
        {
            var points = new List<SimplePoint>();

            try
            {
                if (secondCoord[0] == -1)
                {
                    foreach (var user in users)
                    {
                        if (user.Activities != null)
                        {
                            var activity = user.Activities.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();

                            var x_y = activity[0] + activity[1] + activity[2] + activity[3];
                            points.Add(new SimplePoint(x: x_y, y: x_y, user: user));
                        }
                    }
                }
                else
                {
                    int count = 0;
                    foreach (var user in users)
                    {
                        if ((user.University != null && user.FacultyName == null) ||
                             (user.University == null && user.FacultyName != null) ||
                             (user.University != null && user.FacultyName != null))
                        {
                            points.Add(new SimplePoint(x: secondCoord[count], y: secondCoord[count], user: user));

                            count++;
                        }
                    }
                }
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return points;
        }

        public List<Cluster<SimplePoint>> SearchForUserInClusters(ClusterSet<SimplePoint> clusterSet, List<UserModel> user, List<int> secondCoord)
        {
            var result = new List<Cluster<SimplePoint>>();

            var userClust = this.GetListOfPoints(user, secondCoord);

            foreach (var cluster in clusterSet.Clusters)
            {
                foreach (var point in cluster.Objects)
                {
                    if (userClust[0]._user.VkId == point._user.VkId)
                        result.Add(cluster);
                }
            }

            return result;
        }

        private Dictionary<User, int[]> ConversionToDictionary(List<User> users)
        {
            var dic = new Dictionary<User, int[]>();

            try
            {
                foreach (var user in users)
                {
                    var activity = user.Activities.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();

                    dic.Add(user, activity);
                }
            }
            catch (ArgumentException exception)
            {
                throw new ArgumentException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return dic;
        }
    }
}