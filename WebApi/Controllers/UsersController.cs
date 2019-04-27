using AutoMapper;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;
using WebApi.Utilities;

namespace WebApi.Controllers
{
    [Route("/api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMbaRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;
        private readonly UserManager<User> userManager;

        public UsersController(IMbaRepository repository, IMapper mapper, LinkGenerator linkGenerator, UserManager<User> userManager)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
            this.userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> Get(string id, bool includeEntries = false)
        {
            try
            {
                if (!HttpContext.User.CompareIdWithTokenId(id))
                {
                    return Unauthorized();
                }
                var user = await repository.GetUserById(id, includeEntries);
                if (user == null)
                {
                    return BadRequest("The user doesn't exist.");
                }
                return Ok(mapper.Map<User, UserModel>(user));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(LoginModel login)
        {
            var userCreation = await userManager.CreateAsync(new User { UserName = login.Email, Email = login.Email }, login.Password);
            if (userCreation.Succeeded)
            {
                var user = await repository.GetUserByEmail(login.Email);
                return Ok(mapper.Map<User, UserModel>(user));
            }
            StringBuilder jsonErrors = new StringBuilder();
            foreach (var error in userCreation.Errors)
            {
                if (jsonErrors.Length != 0)
                {
                    jsonErrors.Append(",");
                }
                jsonErrors.Append($"\"{error.Description}\"");
            }

            return BadRequest($"{{\"errors\":[{jsonErrors.ToString()}]}}");

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserModel>> Put(string id, User user)
        {
            try
            {
                if (!HttpContext.User.CompareIdWithTokenId(id))
                {
                    return Unauthorized();
                }
                var userFromId = await repository.GetUserById(id);
                if (userFromId == null)
                {
                    return BadRequest("User not found.");
                }

                var userFromEmail = await repository.GetUserByEmail(user.Email);
                if (userFromEmail != null)
                {
                    if (userFromId.Id.Equals(userFromEmail.Id))
                    {
                        return StatusCode(StatusCodes.Status304NotModified);
                    }
                    else
                    {
                        BadRequest("Email already used.");
                    }
                }

                mapper.Map(user, userFromId);

                if (await repository.SaveChangesAsync())
                {
                    var link = linkGenerator.GetPathByAction("Get", "Users", new { id = user.Id });
                    return Accepted(link, mapper.Map<User, UserModel>(user));
                }
                return StatusCode(StatusCodes.Status304NotModified, mapper.Map<User, UserModel>(user));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!HttpContext.User.CompareIdWithTokenId(id))
                {
                    return Unauthorized();
                }
                var user = await repository.GetUserById(id, true);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                repository.Delete(user);

                if (await repository.SaveChangesAsync())
                {
                    return Ok();
                }
                return StatusCode(StatusCodes.Status304NotModified);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
