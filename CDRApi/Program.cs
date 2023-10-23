
using CDRApi.Services;
using CDRModel;
using CDRServices;
using Microsoft.EntityFrameworkCore;

namespace CDRApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddDbContextPool<CDRContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("CDR"), x => x.MigrationsAssembly("CDRApi"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.Services.AddScoped<ICDRRepository, CDRRepository>();
            builder.Services.AddSingleton<ICsvParser, CsvParser>();

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

            app.Run();
        }
    }
}