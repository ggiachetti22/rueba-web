using AngularApp1.Server.Models;
using AngularApp1.Server.ModelsDTO;
using AngularApp1.Server.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IdentityModel.Tokens.Jwt;
using AngularApp1.Server.Interfaces___Class;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// String key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Imd1aWxsIiwic3ViIjoiZ3VpbGwiLCJqdGkiOiJlZWRjNGJlOSIsImF1ZCI6WyJodHRwOi8vbG9jYWxob3N0OjE2NTkzIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDgiLCJodHRwOi8vbG9jYWxob3N0OjUxNDUiLCJodHRwczovL2xvY2FsaG9zdDo3MjEyIl0sIm5iZiI6MTcyMDEyMTg0NiwiZXhwIjoxNzI4MDcwNjQ2LCJpYXQiOjE3MjAxMjE4NDcsImlzcyI6ImRvdG5ldC11c2VyLWp3dHMifQ.xLqeFVJvpHiJJKaNftYT3ifNK07-AwUhQzqe5ktqM9M";
// Add services to the container.

builder.Services.AddControllers();

// builder.Services.AddAuthorization();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyDbContext>( o => {
    o.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"));
}); // AddDbContext;



/*
IConfiguration JwtSection = builder.Configuration.GetSection("Jwt"); // IConfigurationSection
var encodingByte = Encoding.UTF8.GetBytes(JwtSection["Key"]); // byte[]
var symmetricKey = new SymmetricSecurityKey(encodingByte); // SymmetricSecurityKey
var algoritSha = SecurityAlgorithms.HmacSha256Signature; // String
var signingCredentials = new SigningCredentials(symmetricKey, algoritSha); // SigningCredentials

builder.Services.AddAuthentication(opciones => {
    opciones.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opciones.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt => {
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = symmetricKey,
        ValidateIssuer = true,
        ValidIssuer = JwtSection["Issuer"],
        ValidateAudience = false

    }; // TokenValidationParameters;
}); // AddJwtBearer;
*/




 // IConfigurationSection JWT
IConfiguration settingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(settingsSection);
AppSettings appSettings = settingsSection.Get<AppSettings>();




var encodingByte = Encoding.UTF8.GetBytes(appSettings.SecretKey); // Byte[]
var symmetricSigningKey = new SymmetricSecurityKey(encodingByte); // SymmetricSecurityKey
var algoritSha = SecurityAlgorithms.HmacSha256Signature; // String

builder.Services.AddAuthentication(x => {
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt => {

    var credenciales = new SigningCredentials(symmetricSigningKey, algoritSha); // SigningCredentials

    opt.RequireHttpsMetadata = true;
    opt.SaveToken = true;
    
    opt.TokenValidationParameters = new TokenValidationParameters {

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = symmetricSigningKey,
        ValidateAudience = false,
        ValidateIssuer = false,

    }; // TokenValidationParameters;

}); // AddJwtBearer;
// IConfigurationSection JWT end;


builder.Services.AddScoped<IUserService, UserService>(); // Alcance de Interface;

// builder.Services.AddSingleton<IUserService, UserService>();
// new MailMessage();
// new SmtpClient();
// new NetworkCredential();


// Configura CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowOrigin",
        builder => {
            builder.WithOrigins("https://localhost:4200") // Agrega aquí el origen permitido (Angular) https://localhost:7212
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
}); // AddCors;




builder.Services.AddAuthorization();

var app = builder.Build();


/*
app.Use(async (context, next) => {
    await next.Invoke();
    if (context.Response.StatusCode == 400) {
        // Aquí puedes agregar un registro o más detalles sobre el error
        Console.WriteLine("Bad Request");
    }
}); // Use;
*/



// InintMigrate();

/* void InintMigrate() {
    MyResponse r = new MyResponse();
    IServiceScope? scope = null;
    try {
        using (scope = app.Services.CreateScope()) {
            IServiceProvider pro = scope.ServiceProvider;
            MyDbContext alcance = pro.GetRequiredService<MyDbContext>();
            alcance.Database.Migrate();
        } // using;
        r.UpMessage = $"Éxoto!";
        r.Success = 1;
        r.IsSuccess = true;
    } catch (Exception e) {
        Console.WriteLine($"Error: {e.Message}\n\n{e}");
        r.UpMessage = $"Error: {e.Message}\n\n{e}";
        r.Success = 0;
        r.IsSuccess = false;
    } finally {
        scope?.Dispose();
    } // finally;
} // InintMigrate(); */


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
} // if;


app.UseHttpsRedirection();


/*
app.UseCors(c => {
    // c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    // c.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    // c.WithOrigins("*").WithMethods("*").WithHeaders("*");
    c.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
});
*/


// Aplica CORS a la aplicación
app.UseCors("AllowOrigin");



// app.MapGet("/", () => "Hello World");

// app.MapGet("/protected", (ClaimsPrincipal user) => user.Identity?.Name).RequireAuthorization();

// app.MapGet("/protectedwithscope", (ClaimsPrincipal user) => user.Identity?.Name).RequireAuthorization(x => x.RequireClaim("scope", "myapi:drunken"));



app.UseRouting();

app.UseAuthentication(); // Autenticar

app.UseAuthorization(); // Autorizar;

app.MapControllers();

/* app.UseEndpoints( enp => {
    enp.MapControllers();
}); */

app.MapFallbackToFile("/index.html");

app.Run();


// CreateMap<Messages, MessageDTO>().ReverseMap();

/* CreateMap<Messages, MessageDTO>().ForMember(a =>
    a.UserName, b => b.MapFrom( c => c.MessagesID.userName);
); */





