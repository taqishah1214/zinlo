using Abp.Zero.Ldap.Authentication;
using Abp.Zero.Ldap.Configuration;
using Zinlo.Authorization.Users;
using Zinlo.MultiTenancy;

namespace Zinlo.Authorization.Ldap
{
    public class AppLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
    {
        public AppLdapAuthenticationSource(ILdapSettings settings, IAbpZeroLdapModuleConfig ldapModuleConfig)
            : base(settings, ldapModuleConfig)
        {
        }
    }
}