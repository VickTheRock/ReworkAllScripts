using System.Security.Permissions;

namespace DotaAllCombo.Addons
{
	interface IAddon
    {
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        void Load();
		void Unload();
		void RunScript();
	}
}
