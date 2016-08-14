using System.Security.Permissions;

namespace DotaAllCombo.Heroes
{
    using Ensage;
	using Ensage.Common.Menu;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class Variables
	{
		protected Hero e;
		public Hero me;
		public Menu Menu;
	    public bool Active, CastW, CastE, Push, CastQ;
	}

	interface IHeroController
	{
		void Combo();
		void OnLoadEvent();
		void OnCloseEvent();
	}
}
