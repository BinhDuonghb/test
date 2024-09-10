using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Login.models.setting;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using AspNetCore.Identity.MongoDbCore.Extensions;
using Login.models.Aplication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Login.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

builder.Services.Configure<Database>(builder.Configuration.GetSection("Database"));
builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IDatabase>(sp => sp.GetRequiredService<IOptions<Database>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("Database:ConnectionString")));
builder.Services.AddScoped<IUserManagerS, UserManagerS>();
var _authen = builder.Configuration.GetValue<string>("JwtSettings:securityKey") ?? "default_security_key";
var database = builder.Configuration.GetSection("Database").Get<Database>();

var mongodbConfig = new MongoDbIdentityConfiguration
{


    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = database.ConnectionString,
        DatabaseName = database.DatabaseName
    },
    IdentityOptionsAction = option =>
    {
        option.Password.RequireDigit = false;
        option.Password.RequiredLength = 8;
        option.Password.RequireNonAlphanumeric = true;
        option.Password.RequireLowercase = false;
 
        option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        option.Lockout.MaxFailedAccessAttempts = 5;

        option.User.RequireUniqueEmail = true;
    }
};

builder.Services.ConfigureMongoDbIdentity<AUser, ARole, Guid>(mongodbConfig)
    .AddUserManager<UserManager<AUser>>()
    .AddSignInManager<SignInManager<AUser>>()
    .AddRoleManager<RoleManager<ARole>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authen)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
