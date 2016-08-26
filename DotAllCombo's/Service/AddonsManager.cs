using System.Security.Permissions;
namespace DotaAllCombo.Service
{
	using Addons;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class AddonsManager
    {
        public static bool IsLoaded { get; private set; }
		//private static AutoDodge autoDodge;
		//private static AutoStack autoStack;
		private static CreepControl creepControl;
        //private static LastHit lastHit;
        private static OthersAddons othersAddons;
		public static void RunAddons()
		{
			if (!IsLoaded) return;

			creepControl.RunScript();
            //autoStack.RunScript();
            //autoDodge.RunScript();
            //lastHit.RunScript();
            othersAddons.RunScript();
		}

		public static void Load()
		{

            //autoDodge = new AutoDodge();
            //lastHit = new LastHit();
			creepControl = new CreepControl();
			othersAddons = new OthersAddons();

			creepControl.Load();
			//autoStack.Load();
			//autoDodge.Load();
			othersAddons.Load();
			IsLoaded = true;
		}

		public static void Unload()
        {
           // lastHit.Unload();
            othersAddons.Unload();
			//autoDodge.Unload();
			//autoStack.Unload();
			creepControl.Unload();
			IsLoaded = false;
		}
	}
}
