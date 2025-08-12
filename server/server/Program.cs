using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("register", async (CreateUserDto userDto, ApplicationDbContext dbContext) =>
{
    string hashedPass = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

    var newUser = new User
    {
        Name = userDto.Name,
        HashedPass = hashedPass,
        Role = (int)Roles.User,
        CreatedAt = DateTime.UtcNow
    };

    dbContext.users.Add(newUser);
    await dbContext.SaveChangesAsync();
    return Results.Ok(new { Message = "User Created" });

});

app.Run();
