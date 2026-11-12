
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Application.Services;
using HelpDesk.Domain.Entity;
using HelpDesk.Infrastructure.Persistence;
using HelpDesk.Infrastructure.Repositories;
using HelpDesk.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HelpDesk
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<AppDbContext>(opt=>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))    
            );
            builder.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ITicketCommentRepository, TicketCommentRepository>();
            builder.Services.AddScoped<ITicketCommentService, TicketCommentService>();




            builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>{
                        options.InvalidModelStateResponseFactory = context =>
                        {
                            var errors = context.ModelState
                                .Where(x => x.Value?.Errors.Count > 0)
                                .ToDictionary( x => x.Key, x => x.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray());

                            var response = new ApiErrorResponse
                            {
                                StatusCode = StatusCodes.Status400BadRequest,
                                Message = "Validation failed.",
                                Errors = errors
                            };

                            return new BadRequestObjectResult(response);
                        };});
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,

                                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                                    ValidAudience = builder.Configuration["JwtSettings:Audience"],

                                    IssuerSigningKey = new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(
                                            builder.Configuration["Jwt:SecretKey"]!
                                        )
                                    )
                                };
                            });

            builder.Services.AddAuthorization();


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
                await DbSeeder.SeedAsync(db, hasher, app.Configuration);
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                
            }
            
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
