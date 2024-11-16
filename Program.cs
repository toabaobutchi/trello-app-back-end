using System.Text;
using backend_apis.Data;
using backend_apis.Handlers;
using backend_apis.Hubs;
using backend_apis.Repositories;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var secretKey = configuration["AppSettings:AccessTokenSecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // tự cấp token
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        // ký vào token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        ClockSkew = TimeSpan.Zero
    };
})
.AddCookie(WorkspaceAuthentication.AuthenticationScheme, options =>
{
    options.Cookie = new CookieBuilder()
    {
        Name = "workspace-auth",
        HttpOnly = true,
        SameSite = SameSiteMode.None,
        SecurePolicy = CookieSecurePolicy.Always,
    };
})
.AddCookie(ProjectAuthentication.AuthenticationScheme, options =>
{
    options.Cookie = new CookieBuilder()
    {
        Name = "project-auth",
        HttpOnly = true,
        SameSite = SameSiteMode.None,
        SecurePolicy = CookieSecurePolicy.Always,
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        WorkspaceAuthentication.RequiredPolicy,
        policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, WorkspaceAuthentication.AuthenticationScheme)
            .RequireClaim(WorkspaceClaimType.WorkspaceId));
    options.AddPolicy(
        ProjectAuthentication.RequiredPolicy,
        policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, ProjectAuthentication.AuthenticationScheme)
            .RequireClaim(ProjectClaimType.ProjectId));
    options.AddPolicy(ProjectAuthentication.HubPolicy, policy => policy.AddAuthenticationSchemes(ProjectAuthentication.AuthenticationScheme).RequireClaim(ProjectClaimType.ProjectId));
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddDbContext<ProjectManagerDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddSignalR();

builder.Services.AddExceptionHandler<ExceptionHandler>();

builder.Services.AddScoped<IEmailSender, MailSender>();
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IListRepository, ListRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<ISubtaskRepository, SubtaskRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IInvitationRepository, InvitationRepository>();
builder.Services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<ProjectMemberService>();
builder.Services.AddScoped<NotificationHub>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChangeLogService>();
builder.Services.AddScoped<EmailService>();


var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler((exceptionHandlerApp) =>
//     {
//         exceptionHandlerApp.Run(async context =>
//         {
//             context.Response.StatusCode = StatusCodes.Status200OK;
//             var error = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
//             await context.Response.WriteAsJsonAsync(ResponseHelper.InternalServerError(error));
//         });
//     });
// }
// else
// {
//     app.UseExceptionHandler((exceptionHandlerApp) =>
//     {
//         exceptionHandlerApp.Run(async context =>
//         {
//             context.Response.StatusCode = StatusCodes.Status200OK;
//             var error = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
//             await context.Response.WriteAsJsonAsync(ResponseHelper.InternalServerError(null));
//         });
//     });
// }

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:5173").WithMethods("GET", "POST", "PUT", "DELETE")
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true);
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ProjectHub>("/projectHub");
app.MapHub<DragHub>("/dragHub");
app.MapHub<TaskHub>("/taskHub");
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
