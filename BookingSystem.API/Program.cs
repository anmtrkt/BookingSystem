using BookingSystem.API.Services.Extensions;
using BookingSystem.API.Services.Identity;
using BookingSystem.API.Services.UserServices;
using BookingSystem.Core.Domain.Entities.Users;
using BookingSystem.Infrastructure.EventHandlers.BookingHandlers;
using BookingSystem.Infrastructure.Services.Interfaces;
using BookingSystem.Infrastructure.Services.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);



// Добавление конфигурации


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(BookingCreatedEventHandler).Assembly);
});


builder.Services.AddAuth();
// Регистрация репозиториев
builder.Services.AddScoped<IUserMiddleware, UserMiddleware>();
builder.Services.AddScoped<IInstitutionService, InstitutionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IBuildingService, BuildingService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHangfireServer();
builder.Services.AddHttpContextAccessor();


builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseDefaultTypeSerializer();
    config.UseMemoryStorage();

});

;
// Добавление MediatR
//builder.Services.AddMediatR(typeof(Program))

// Добавление других зависимостей
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // включаем поддержку циклических ссылок
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
}); ;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "BookingsAPI"
    });

/*
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);*/
});


var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed ролей
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Roles>>();
    await SeedRolesAsync(roleManager);
}

app.UseHangfireDashboard();
//Добавляем наш Middleware

app.UseRouting();

// 2. Политика куки должна применяться до аутентификации
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always,
});

// 3. Аутентификация / авторизация
app.UseAuthentication();
app.UseAuthorization();

// 4. Маршруты контроллеров
app.MapControllers();

app.Run();


static async Task SeedRolesAsync(RoleManager<Roles> roleManager)
{
    string[] roles =
        { Roles.Admin, Roles.User, Roles.Manager, Roles.Admininstration };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new Roles
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors);
                Console.WriteLine($"Ошибка создания роли {roleName}: {errors}");
            }
        }
    }
}

///TODO                                         DONE
///                                             ПРОВЕРИТЬ ВСЕ FOREIGN KEY И PRIMARY KEY
///                                             ДОБАВИТЬ ИНИЦИАЛИЗАЦИЮ РОЛЕЙ
///                                             ДОБАВИТЬ СЕРВИСЫ
///         ЗАПОЛНИТЬ СЕРВИСЫ - изменить апдейты
///                                                      В АЙРУМ ДОДЕЛАТЬ КРИЭЙТ
///РЕАЛИЗОВАТЬ КОНТРОЛЛЕРЫ
///ПОДКЛЮЧИТЬ ЕМЕЙЛ И МОНГОДБ
///                                                 ДОБАВИТЬ АУТЕНТИФИКАЦИЮ JWT



/////                                               ДОДЕЛАТЬ SCHEDULE
///
///
    