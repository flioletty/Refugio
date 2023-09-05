using System;
using DTOModels;
using DBModels;
using ProjectModels;

namespace DBCore.Converters
{
    public class UserConverter
    {
        public UserConverter()
        {
        }

        public UserDto GetUserDto(UserModel user)
        {
            if (user == null)
                return new UserDto();

            var userDto = new UserDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Activity = user.Activities,
                University = user.University,
                FacultyName = user.FacultyName,
                VkIdString = $"https://vk.com/id{user.VkId}"
            };

            if (user.Activities == null || user.Activities == "")
                userDto.Activity = "отсутствуют";

            if (user.University == null || user.University == "")
                userDto.University = "отсутствует";

            if (user.FacultyName == null || user.FacultyName == "")
                userDto.FacultyName = "отсутствует";

            return userDto;
        }

        public UserModel GetUserModel(User user)
        {
            if (user == null)
                return new UserModel();

            return new UserModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Activities = user.Activities,
                University = user.University,
                FacultyName = user.FacultyName,
                City = user.City,
                Country = user.Country,
                Sex = user.Sex,
                VkId = user.VkId
            };
        }

        public PointDto GetPointDto(double x, double y, int rgb)
        {
            var pointDto = new PointDto()
            {
                pointX = x,
                pointY = y,
                color = $"rgb({255}, {255-rgb}, {255})"
            };

            return pointDto;
        }
        }
}

