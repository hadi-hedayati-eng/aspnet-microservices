using AutoMapper;
using CommandService.Contracts;
using CommandService.Domain;

namespace CommandService.Mapping;

public class CommandsMapping : Profile
{
    public CommandsMapping()
    {
        CreateMap<Command, CommandReadDto>();
        CreateMap<CommandCreateDto, Command>();
    }
}
