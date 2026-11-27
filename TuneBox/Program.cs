using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TuneBox.Models;
using TuneBox.Options;
using TuneBox.Profiles;
using TuneBox.Repositories;
using TuneBox.Repositories.Interfaces;
using TuneBox.Services;
using TuneBox.Validators;

namespace TuneBox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Відстеження змін у appsettings.json без перезапуску
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Логування через Serilog із конфігурації
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            // Конфігурація (шаблон Options)
            builder.Services.AddOptions<TuneBoxOptions>()
                .BindConfiguration(TuneBoxOptions.SectionName);

            // Кешування в пам'яті
            builder.Services.AddMemoryCache();

            // Підключення БД
            builder.Services.AddDbContext<TuneBoxDbContext>(options =>
            {
                options.UseSqlite("Data Source=tunebox.db");
            });

            // AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TuneBoxProfile>();
            });

            builder.Services.AddControllers();

            // FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<ArtistValidator>();

            // РЕПОЗИТОРІЇ
            builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
            builder.Services.AddScoped<ISongRepository, SongRepository>();

            // СЕРВІСИ
            builder.Services.AddScoped<ArtistService>();
            builder.Services.AddScoped<SongService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Автоматичне застосування міграцій при запуску
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TuneBoxDbContext>();
                db.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware для логування HTTP-запитів через Serilog
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}