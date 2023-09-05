using System;
using DBCore;
using DTOModels;
using Microsoft.AspNetCore.Mvc;
using DBCore.Converters;
using VkAPI;

namespace RefugioApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MetricsController : Controller
    {
        private readonly IGroupRepository _groupDb;

        private readonly IUserRepository _userDb;

        public MetricsController(IGroupRepository groupDb,
                                 IUserRepository userDb)
        {
            _groupDb = groupDb;
            _userDb = userDb;
        }

        [HttpGet("BirthDates")]
        public async Task<IActionResult> GetBirthDates()
        {
            IActionResult response;

            try
            {
                var usersBirthdaysDto = _userDb.GetUsersBirthdays().Select(x => new BirthDateDto()
                {
                    BirthDate = x
                }).OrderBy(x => x.BirthDate).ToList();

                response = Ok(usersBirthdaysDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("Cities")]
        public async Task<IActionResult> GetCities()
        {
            IActionResult response;

            try
            {
                var userCitiesDto = _userDb.GetUserCities().Select(x => new CityDto()
                {
                    City = x
                }).OrderBy(x => x.City).ToList();

                response = Ok(userCitiesDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("Universityes")]
        public async Task<IActionResult> GetUniversityes()
        {
            IActionResult response;
            try
            {
                var userUniversitiesDto = _userDb.GetUserUniversities().Select(x => new UniversityDto()
                {
                    University = x
                }).OrderBy(x => x.University).ToList();

                response = Ok(userUniversitiesDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("Facultyes")]
        public async Task<IActionResult> GetFacultyes()
        {
            IActionResult response;
            try
            {
                var userFacultiesDto = _userDb.GetUserFaculties().Select(x => new FacultyDto()
                {
                    FacultyName = x
                }).OrderBy(x => x.FacultyName).ToList();

                response = Ok(userFacultiesDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpGet("Interests")]
        public async Task<IActionResult> GetInterests()
        {
            IActionResult response;
            try
            {
                var userInterestsDto = _groupDb.GetAllActivity().Select(x => new InterestDto()
                {
                    Activity = x
                }).OrderBy(x => x.Activity).ToList();

                response = Ok(userInterestsDto);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }
    }
}
