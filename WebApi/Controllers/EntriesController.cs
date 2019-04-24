﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Data.Entities;

namespace WebApi.Controllers
{
    [Route("/api/users/{userId}/[controller]")]
    [Authorize]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly IMbaRepository repository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public EntriesController(IMbaRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<Entry[]>> Get(string userId)
        {
            try
            {
                var user = await repository.GetUserById(userId, true);
                if (user == null)
                {
                    return BadRequest("The user doesn't exist.");
                }

                return Ok(user.Entries);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{entryId}")]
        public async Task<ActionResult<Entry>> Get(string userId, int entryId)
        {
            try
            {
                var entry = await repository.GetEntry(userId, entryId);
                if (entry == null)
                {
                    return BadRequest("The entry is not found.");
                }
                return Ok(entry);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Entry>> Post(string userId, Entry entry)
        {
            try
            {
                var user = await repository.GetUserById(userId, true);
                if (user == null)
                {
                    return BadRequest("The user is not found.");
                }
                entry.Id = 0; // The Id is generated by the DB.
                user.Entries.Add(entry);
                if (await repository.SaveChangesAsync())
                {
                    var link = linkGenerator.GetPathByAction("Get", "Entries", new { userId, entryId = entry.Id });
                    return Created(link, entry);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        [HttpPut("{entryId:int}")]
        public async Task<ActionResult<User>> Put(string userId, int entryId, Entry entry)
        {
            try
            {
                var dbEntry = await repository.GetEntry(userId, entryId);
                if (dbEntry == null)
                {
                    return BadRequest("Entry not found.");
                }
                entry.Id = entryId;
                mapper.Map(entry, dbEntry);

                if (await repository.SaveChangesAsync())
                {
                    var link = linkGenerator.GetPathByAction("Get", "Users", new { id = dbEntry.Id });
                    return Accepted(link, dbEntry);
                }
                return StatusCode(StatusCodes.Status304NotModified, dbEntry);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpDelete("{entryId:int}")]
        public async Task<IActionResult> Delete(string userId, int entryId)
        {
            try
            {
                var entry = await repository.GetEntry(userId, entryId);
                if (entry == null)
                {
                    return BadRequest("Entry not found.");
                }

                repository.Delete(entry);

                if (await repository.SaveChangesAsync())
                {
                    return Ok();
                }
                return StatusCode(StatusCodes.Status304NotModified, entry);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
