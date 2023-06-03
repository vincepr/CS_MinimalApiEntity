using SixMinApi.Models;

namespace SixMinApi.Data
{
    public interface ICommandRepo
    {
        Task SaveChanges();
        Task<Command?> GetCommandbyId(int id);
        Task<IEnumerable<Command>> GetAllCommands();
        Task CreateCommand(Command cmd);
        void DeleteCommand(Command cmd);
        // no Method for Update needed with EntityFramework. Changing the db context directly
    }
}