using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixMinApi.Data;
using SixMinApi.Api;
using AutoMapper;
using SixMinApi.Dtos;
using SixMinApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* dependency inject our Db context into the app: */
var sqlConBuilder = new SqlConnectionStringBuilder();

// add the public infro from the appsettings.Development.json to the builder.ConnectionString
sqlConBuilder.ConnectionString = builder.Configuration.GetConnectionString("ConnectionStrings");
// add missing info (hidden in our dotnet user-secrets) to the builder.ConnectionString
sqlConBuilder.UserID = builder.Configuration["UserId"];
sqlConBuilder.Password = builder.Configuration["Password"];
// dependency inject this info into our builder -> app
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlConBuilder.ConnectionString));


// dependency inject our SQL-Implementation (swappable for ex for a reddis/mongodb) db implementation into a 'ScopedContainer'
// this way it would be easier to change it, or change library etc...
// basically we just say we use this ICommandRepo interface.
builder.Services.AddScoped<ICommandRepo, CommandRepo>();

// dependency inject our AutoMapping (mapping Models -> Dtos ) to the builder
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// example of a Handler Function separated out to a extra file
app.MapGet("/api/v1/commands", Handle.GetAllCommands);

// example of a inline Handler Function
app.MapGet("/api/v1/{id}", async(ICommandRepo repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandbyId(id);
    if (command != null) return Results.Ok(mapper.Map<CommandReadDto>(command));
    return Results.NotFound();
});


app.MapPost("api/v1/commands", async(ICommandRepo repo, IMapper mapper, CommandCreateDto cmdCreateDto) =>{
    var commandModel = mapper.Map<Command>(cmdCreateDto);
    await repo.CreateCommand(commandModel);     // this will ONLY create the command-context but not flush it down/persist it to the db
    await repo.SaveChanges();                   // this will flush all gathered changes down to our db
    // now we want to pass down the (new) id of the freshly generated entry:
    var cmdReadDto = mapper.Map<CommandReadDto>(commandModel);
    return Results.Created($"api/v1/commands/{cmdReadDto.Id}", cmdReadDto);
    // this will return a 'link' to our newly generated Command, like: api/v1/commands/12
});

app.MapPut("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id, CommandUpdateDto cmdUpdateDto) =>{
    var command = await repo.GetCommandbyId(id);
    if (command == null ) return Results.NotFound();
    mapper.Map(cmdUpdateDto, command);
    await repo.SaveChanges();
    return Results.Ok();
});

app.MapDelete("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandbyId(id);
    if (command == null) return Results.NotFound();
    repo.DeleteCommand(command);
    await repo.SaveChanges();
    return Results.Ok();
});

app.Run();
