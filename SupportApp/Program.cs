using Microsoft.EntityFrameworkCore;
using SupportApp.Helper;
using SupportApp.Models;
using SupportApp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SupportApp.SignalR;
using SupportApp.DependencyContainer;


var builder = WebApplication.CreateBuilder(args);

//------------------------ Service Extension Register ---------------------
try {
    builder.Services.AddTransientServices();
    builder.Services.AddScopedServices();
    builder.Services.AddSingletonServices();
    builder.Services.RegistrationServices();
}
catch(Exception ex) {
    Console.WriteLine(ex);
    throw;
}
// SignalR Hub 
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register email setting
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// config Dependency Injection
builder.Services.AddDbContext<SupportAppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase")));

// Add JWT servcices
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


// JWT 
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://192.168.0.11:9999",
                                "http://localhost:5173", 
                                "http://192.168.0.12:50002", 
                                "http://192.168.0.13")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



var app = builder.Build();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else {
	app.UseSwagger();
	app.UseSwaggerUI();
}

// jwt service
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ReviewHub>("/reviewHub").RequireCors("Open").RequireCors("MyAllowSpecificOrigins"); ;
app.Run();
