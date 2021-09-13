using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo repo;
        private readonly IMapper mapper;

        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands(int platformId)
        {
            Console.WriteLine($"Get Commands for Platform: {platformId}");

            if (!repo.IsPlatformExisted(platformId))
            {
                return NotFound();
            }

            var commands = repo.GetCommandsForPlatform(platformId);

            return Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"Get Command {commandId} for Platform {platformId}");

            if (!repo.IsPlatformExisted(platformId))
            {
                return NotFound();
            }

            var command = repo.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"Create Command for Platform: {platformId}");

            if (!repo.IsPlatformExisted(platformId))
            {
                return NotFound();
            }

            var command = mapper.Map<Command>(commandCreateDto);
            repo.CreateCommand(platformId, command);
            repo.SaveChanges();

            var commandReadDto = mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
                new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}