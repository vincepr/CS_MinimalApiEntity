using Microsoft.EntityFrameworkCore;
using SixMinApi.Models;


namespace SixMinApi.Data
{
    public class CommandRepo : ICommandRepo
    {
        // In our Contructor we inject the db context (coming from our app in Program.cs)
        private readonly AppDbContext _context;
        public CommandRepo(AppDbContext ctx)
        {
            _context = ctx;
        }

        public async Task CreateCommand(Command cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));

            await _context.AddAsync(cmd);
        }

        public void DeleteCommand(Command cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));
            
            _context.Commands.Remove(cmd);
        }

        public async Task<IEnumerable<Command>> GetAllCommands()
        {
            return await _context.Commands.ToListAsync();
        }

        public async Task<Command?> GetCommandbyId(int id)
        {
            return await _context.Commands.FirstOrDefaultAsync(c => c.Id == id);    
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

    }
}