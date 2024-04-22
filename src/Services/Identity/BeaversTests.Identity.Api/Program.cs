using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequiredUniqueChars = 0;
    opt.Password.RequireDigit = false;
}).AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();