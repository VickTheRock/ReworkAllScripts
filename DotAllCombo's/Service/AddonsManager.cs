using System.Security.Permissions;

namespace DotaAllCombo.Service
{
	using Addons;
	using System;
	using System.Threading;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class AddonsManager
    {
        public static bool IsLoaded { get; private set; }
		private static CreepControl creepControl;
		private static OthersAddons othersAddons;
		public static void RunAddons()
		{
			if (!IsLoaded) return;
            
			creepControl.RunScript();
			othersAddons.RunScript();
		}

		public static void Load()
		{
			
			creepControl = new CreepControl();
			othersAddons = new OthersAddons();
            
			creepControl.Load();
			othersAddons.Load();
			IsLoaded = true;
		}

		public static void Unload()
		{
			othersAddons.Unload();
			creepControl.Unload();

			IsLoaded = false;
		}
	}
}
