using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petty.Services.Local.PermissionsFolder
{
    /// <remarks>https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?tabs=android</remarks>
    public interface IPermission
    {
        Task<PermissionStatus> GetPermissionAsync();
    }
}
