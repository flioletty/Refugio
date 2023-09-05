using System;
using DBCore;
using DTOModels;
using Microsoft.AspNetCore.Mvc;
using DBCore.Converters;
using VkAPI;
using Clusterization;
using ProjectModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Refugio.Controllers
{
    [Authorize]
    [ApiController]
    [Route(template: "api/[controller]")]
    public class UserController : Controller
    {
        private readonly IGroupRepository _groupDb;

        private readonly IUserRepository _userDb;

        private readonly UserConverter _userConverter;

        private readonly SetInterests _interests;

        private readonly CombiningInterests _cmb;

        public UserController(IGroupRepository groupDb,
                              IUserRepository userDb,
                              UserConverter userConverter,
                              SetInterests interests,
                              CombiningInterests cmb)
        {
            _groupDb = groupDb;
            _userDb = userDb;
            _userConverter = userConverter;
            _interests = interests;
            _cmb = cmb;
        }

        [HttpPost("Initialize")]
        public async Task<IActionResult> InitializeDB()
        {
            IActionResult response;
            try
            {
                new InitializationDb(_groupDb, _userDb).Initialization();

                response = Ok("updated!");
            }
            catch (ArgumentException ex)
            {
                response = StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }

        [HttpPost("AssignInterestsToUsers")]
        public async Task<IActionResult> AssignInterestsToUsers()
        {
            IActionResult response;

            try
            {
                var activity = _groupDb.GetAllActivity();

                _interests.Activity();
                var user = _userDb.GetUsers();
                _interests.SetActivities(user, _userDb, _cmb);

                response = Ok();
            }
            catch (ArgumentException ex)
            {
                response = StatusCode(StatusCodes.Status403Forbidden);
            }
            catch (Exception ex)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;
        }
    }

    
}