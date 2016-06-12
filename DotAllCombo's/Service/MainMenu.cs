using System.Security.Permissions;

namespace DotaAllCombo.Service
{
	using SharpDX;
	using Ensage.Common.Menu;

	using Debug;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    class MainMenu
	{
		private static Menu mainMenu;
		private static Menu addonsMenu;
		private static readonly Menu ccMenu    = new Menu("Auto Controll All Unit's", "Auto Controll All Unit's");
		private static readonly Menu othersMenu = new Menu("Others Addon's", "Others Addon's");
		public static Menu Menu		  { get { return mainMenu;   } }
		public static Menu AddonsMenu { get { return addonsMenu; } }
        
		public static Menu OthersMenu { get { return othersMenu; } }
		public static Menu CCMenu    { get { return ccMenu;	   } }

		public static void Load()
		{
			// Инициализируем главное меню
			mainMenu = new Menu("DotaAllCombo's", "menuName", true);
			addonsMenu = new Menu("Addons", "addonsMenu");

			// Инициализация меню для аддонов

			othersMenu.AddItem(new MenuItem("others", "Others Addon's").SetValue(true));
			othersMenu.AddItem(new MenuItem("ShowAttakRange", "Show AttackRange").SetValue(true));
            othersMenu.AddItem(new MenuItem("Auto Un Aggro", "Auto Un Aggro Creeps|Towers|Fountain").SetValue(true));
            addonsMenu.AddSubMenu(othersMenu);
            

			ccMenu.AddItem(new MenuItem("controll", "Auto Controll Unit's").SetValue(true));
			addonsMenu.AddSubMenu(ccMenu);
			
			// Добавление меню с аддонами в главное
			mainMenu.AddSubMenu(addonsMenu);

			if (!HeroSelector.IsSelected)
			{
				Print.ConsoleMessage.Success("[DotaAllCombo's] Initialization complete!"); return;
			}
			
			// Подключаем меню настроек героя
			mainMenu.AddSubMenu((Menu)HeroSelector.HeroClass.GetField("menu").GetValue(HeroSelector.HeroInst));

			// Выводим автора текущего скрипта
			mainMenu.AddItem(new MenuItem("scriptAutor", HeroSelector.HeroName + " by " + AssemblyExtensions.GetAuthor(), true).SetFontStyle(
				System.Drawing.FontStyle.Bold, Color.Coral));

			// Выводим версию текущего скрипта
			mainMenu.AddItem(new MenuItem("scriptVersion", "Version " + AssemblyExtensions.GetVersion()).SetFontStyle(
				System.Drawing.FontStyle.Bold, Color.Coral));
			mainMenu.AddToMainMenu();

			Print.ConsoleMessage.Success("[DotaAllCombo's] Initialization complete!");
		}

		public static void Unload()
		{
			if (mainMenu == null) return;

			mainMenu.RemoveFromMainMenu();
			mainMenu = null;
		}
	}
}
