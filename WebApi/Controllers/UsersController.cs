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
        public async Task<IActionResult> Post(LoginModel login)
        {
            var result = await userManager.CreateAsync(new User { UserName = login.Email, Email = login.Email }, login.Password);
            if (result.Succeeded)
            {
                var user = await repository.GetUserByEmail(login.Email);
                return Ok(mapper.Map<User, UserModel>(user));
            }
            StringBuilder jsonErrors = new StringBuilder();
            foreach (var error in result.Errors)
            {
                if (jsonErrors.Length != 0)
                {
                    jsonErrors.Append(",");
                }
                jsonErrors.Append($"\"{error.Description}\"");
            }

            return BadRequest($"{{\"errors\":[{jsonErrors.ToString()}]}}");

        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserModel>> Put(string userId, User user)
        {
            try
            {
                var existing = await repository.GetUserById(userId);
                if (existing == null)
                {
                    return BadRequest("User not found.");
                }

                var userWithEmail = await repository.GetUserByEmail(user.Email);
                if (userWithEmail != null && !existing.Id.Equals(userWithEmail?.Id))
                {
                    return BadRequest("Email already used.");
                }

                mapper.Map(user, existing);

                if (await repository.SaveChangesAsync())
                {
                    var link = linkGenerator.GetPathByAction("Get", "Users", new { id = user.Id });
                    return Accepted(link, mapper.Map<User, UserModel>(user));
                }
                return StatusCode(StatusCodes.Status304NotModified, mapper.Map<User, UserModel>(user));

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                var user = await repository.GetUserById(userId, true);
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
