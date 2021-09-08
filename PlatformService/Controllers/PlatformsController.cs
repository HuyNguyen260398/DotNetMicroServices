using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.SyncDataServices.Http;
using PlatfromService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo repo;
        private readonly IMapper mapper;
        private readonly ICommandDataClient commandDataClient;

        public PlatformsController(
            IPlatformRepo repo, 
            IMapper mapper,
            ICommandDataClient commandDataClient)
        {
            this.repo = repo;
            this.mapper = mapper;
            this.commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("Getting Platforms...");

            var platforms = repo.GetAllPlatforms();

            return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name="GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = repo.GetPlatformById(id);

            if (platform != null)
                return Ok(mapper.Map<PlatformReadDto>(platform));
            
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platform = mapper.Map<Platform>(platformCreateDto);
            repo.CreatePlatform(platform);
            repo.SaveChanges();

            var platformReadDto = mapper.Map<PlatformReadDto>(platform);

            try
            {
                await commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send synchronously: {e.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id}, platformReadDto);
        }
    }
}