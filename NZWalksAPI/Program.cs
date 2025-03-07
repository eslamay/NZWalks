
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NZWalksAPI.Data;
using NZWalksAPI.Mappings;
using NZWalksAPI.Repositories;
using System.Text;

namespace NZWalksAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddHttpContextAccessor();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "NZ walks API", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name="Authorization",
                    In=ParameterLocation.Header,
                    Type=SecuritySchemeType.ApiKey,
                    Scheme=JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                { 
                  {
                    new OpenApiSecurityScheme
                    {
                        Reference=new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id=JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme="Oauth2",
                        Name=JwtBearerDefaults.AuthenticationScheme,
                        In=ParameterLocation.Header
                    },
                    new List<string>()
                  }
                });
            });

            builder.Services.AddDbContext<NZWalksDbContext>(options=>
            options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));

			builder.Services.AddDbContext<NZWalksAuthDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("Auth")));

			builder.Services.AddScoped<IRegionRepository,SQLRegionRepository>();
			builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();
			builder.Services.AddScoped<ITokenRepository, TokenRepository>();
			builder.Services.AddScoped<IImageRepository, ImageRepository>();

			builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

			builder.Services.AddIdentityCore<IdentityUser>()
	           .AddRoles<IdentityRole>()
	           .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZwalks")
               .AddEntityFrameworkStores<NZWalksAuthDbContext>()
	           .AddDefaultTokenProviders();


			builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>
			options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer=builder.Configuration["Jwt:Issuer"],
                ValidAudience= builder.Configuration["Jwt:Audience"],
                IssuerSigningKey=new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
			});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider=new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Images")),
                RequestPath="/Images"
            });

            app.MapControllers();

            app.Run();
        }
    }
}
