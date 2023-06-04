using AutoMapper;
using SixMinApi.Dtos;
using SixMinApi.Models;

namespace SixMinApi.Profiles
{
    // were implementing Profile from Automapper
    // to setupt quick mapping from Dtos <-> Models.Command
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // Source(Model) -> Target(Dtos.CommandReadDto)
            CreateMap<Command, CommandReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<CommandUpdateDto, Command>();
        }
    }
}
