using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;

namespace WebApi.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMbaRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public UsersController(IMbaRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(string id, bool includeEntries = false)
        {
            try
            {
                var user = await repository.GetUserById(id, includeEntries);
                if (user == null)
                {
                    return BadRequest("The user doesn't exist.");
                }
                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            try
            {
                var existing = await repository.GetUserByEmail(user.Email);
                if (existing != null)
                {
                    return BadRequest("Email already used.");
                }

                user.Id = null; // The DB creates the id, not the user.
                repository.Add(user);
                if (await repository.SaveChangesAsync())
                {
                    var link = linkGenerator.GetPathByAction("Get", "Users", new { id = user.Id });
                    return Created(link, user);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<User>> Put(string userId, User user)
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
                    return Accepted(link, user);
                }
                return StatusCode(StatusCodes.Status304NotModified, user);

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
                return StatusCode(StatusCodes.Status304NotModified, user);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
