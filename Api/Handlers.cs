using AutoMapper;
using SixMinApi.Data;
using SixMinApi.Dtos;
using System.Windows.Input;

namespace SixMinApi.Api
{
    public static class Handle
    {
        public async static Task<IResult> GetAllCommands(ICommandRepo repo, IMapper mapper)
        {
            try
            {
                var commands = await repo.GetAllCommands();
                return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
            } catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
            
        }
    }
}
