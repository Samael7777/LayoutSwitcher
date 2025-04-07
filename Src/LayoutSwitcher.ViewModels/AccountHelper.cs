using System.Security.Principal;

namespace LayoutSwitcher.ViewModels;

public static class AccountHelper
{
    public static bool IsAdministrator()
    {
        var principals = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        return principals.IsInRole(WindowsBuiltInRole.Administrator);
    }
}