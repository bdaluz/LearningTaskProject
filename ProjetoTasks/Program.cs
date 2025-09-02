using Services.Data;
using ProjetoTasks.Menus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ProjetoTasks;
using Services.Services;
using Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;

var serviceCollection = new ServiceCollection();


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())  
    .AddJsonFile("config.json", optional: false, reloadOnChange: true)  
    .Build();


serviceCollection.AddSingleton<IConfiguration>(configuration);


serviceCollection.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");  
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
});

serviceCollection
    .AddScoped<IToDoTaskService, ToDoTaskService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IEmailService, EmailService>()
    .AddScoped<ToDoTaskMenu>()
    .AddScoped<UserMenu>()
    .AddScoped<RunProgram>()
    .BuildServiceProvider();

var serviceProvider = serviceCollection.BuildServiceProvider();

var app = serviceProvider.GetService<RunProgram>();
app?.Run().Wait();
