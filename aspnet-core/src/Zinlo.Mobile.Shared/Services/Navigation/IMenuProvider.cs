using System.Collections.Generic;
using MvvmHelpers;
using Zinlo.Models.NavigationMenu;

namespace Zinlo.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}