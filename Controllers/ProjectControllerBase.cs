using System.Security.Claims;
using backend_apis.Utils;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    public class ProjectControllerBase : ControllerBase
    {
        protected ProjectCookie GetProjectCookie() => new(User);
        protected ProjectCookie ProjectCookie => GetProjectCookie();
    }
}