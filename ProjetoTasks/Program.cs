using Services.Data;
using ProjetoTasks.Menus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ProjetoTasks;
using Services.Services;




//var builder = new ConfigurationBuilder();
//var configuration = builder.Build();
var serviceCollection = new ServiceCollection();
var serviceProvider = serviceCollection.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=localhost;Integrated Security=true;Database=TaskProject;TrustServercertificate=true"))
    .AddScoped<IToDoTaskService, ToDoTaskService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<ToDoTaskMenu>()
    .AddScoped<UserMenu>()
    .AddScoped<RunProgram>()
    .BuildServiceProvider();

var app = serviceProvider.GetService<RunProgram>();

app?.Run().Wait();