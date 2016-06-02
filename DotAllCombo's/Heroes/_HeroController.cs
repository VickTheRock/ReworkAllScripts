using System.Security.Permissions;

namespace DotaAllCombo.Heroes
{
	using Ensage;
	using Ensage.Common.Menu;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class Variables
	{
		protected Hero target, e;
		public Hero me;
		public Menu menu;
	}

	interface IHeroController
	{
		void Combo();
		void OnLoadEvent();
		void OnCloseEvent();
	}
}
