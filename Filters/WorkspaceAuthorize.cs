using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;

namespace backend_apis.Filters
{
    public class WorkspaceAuthorize : AuthorizeAttribute
    {
        public WorkspaceAuthorize()
        {
            AuthenticationSchemes = WorkspaceAuthentication.AuthenticationScheme;
            Policy = WorkspaceAuthentication.RequiredPolicy;
        }
    }
}