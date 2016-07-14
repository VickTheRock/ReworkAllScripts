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
       // private static readonly Menu dodgeMenu = new Menu("AutoDodgeSpells", "Auto Dodge All Spell's");
		//private static readonly Menu stackMenu = new Menu("Stack Camp's", "Stack Camp's");
		private static readonly Menu ccMenu    = new Menu("Auto Controll All Unit's", "Auto Controll All Unit's");
		private static readonly Menu othersMenu = new Menu("Others Addon's", "Others Addon's");
		public static Menu Menu		  { get { return mainMenu;   } }
		public static Menu AddonsMenu { get { return addonsMenu; } }

		//public static Menu DodgeMenu { get { return dodgeMenu; } }
		//public static Menu StackMenu { get { return stackMenu; } }
		public static Menu OthersMenu { get { return othersMenu; } }
		public static Menu CCMenu    { get { return ccMenu;	   } }

		public static void Load()
		{
			// Инициализируем главное меню
			mainMenu = new Menu("DotaAllCombo's", "menuName", true);
			addonsMenu = new Menu("Addons", "addonsMenu");
			//Menu ul = new Menu("Escape", "Auto Escape Target Attack");
			// Инициализация меню для аддонов
			//dodgeMenu.AddItem(new MenuItem("dodge", "Auto Dodge Spell's").SetValue(true));
			//addonsMenu.AddSubMenu(dodgeMenu);

			othersMenu.AddItem(new MenuItem("others", "Others Addon's").SetValue(true));
			othersMenu.AddItem(new MenuItem("ShowAttakRange", "Show AttackRange").SetValue(true));

			//ul.AddItem(new MenuItem("minLVL", "Min Hero Level to Escape").SetValue(new Slider(7, 5, 25)));
			//ul.AddItem(new MenuItem("EscapeAttack", "Auto Escape Target Attack").SetValue(true));
			othersMenu.AddItem(new MenuItem("Auto Un Aggro", "Auto Un Aggro Towers|Fountain").SetValue(true));
			//othersMenu.AddSubMenu(ul);
			addonsMenu.AddSubMenu(othersMenu);

			//stackMenu.AddItem(new MenuItem("Stack", "Stack Camp's").SetValue(new KeyBind('T', KeyBindType.Toggle)));
			//stackMenu.AddItem(new MenuItem("mepos", "Stack Meepo's Camp's").SetValue(false));
			//stackMenu.AddItem(new MenuItem("mestack", "Stack Me Camp's").SetValue(false));
			//addonsMenu.AddSubMenu(stackMenu);

			ccMenu.AddItem(new MenuItem("controll", "Auto Controll Unit's").SetValue(true));
			addonsMenu.AddSubMenu(ccMenu);
			
			// Добавление меню с аддонами в главное
			mainMenu.AddSubMenu(addonsMenu);

			if (!HeroSelector.IsSelected)
			{
				Print.ConsoleMessage.Success("[DotaAllCombo's] Initialization complete!"); return;
			}
			
			// Подключаем меню настроек героя
			mainMenu.AddSubMenu((Menu)HeroSelector.HeroClass.GetField("Menu").GetValue(HeroSelector.HeroInst));

			// Выводим автора текущего скрипта
#pragma warning disable CS0618 // 'MenuItem.SetFontStyle(FontStyle, Color?)' is obsolete: 'SetFontStyle is deprecated, please use SetFontColor instead'
			mainMenu.AddItem(new MenuItem("scriptAutor", HeroSelector.HeroName + " by " + AssemblyExtensions.GetAuthor(), true).SetFontStyle(
				System.Drawing.FontStyle.Bold, Color.Coral));
#pragma warning restore CS0618 // 'MenuItem.SetFontStyle(FontStyle, Color?)' is obsolete: 'SetFontStyle is deprecated, please use SetFontColor instead'

			// Выводим версию текущего скрипта
#pragma warning disable CS0618 // 'MenuItem.SetFontStyle(FontStyle, Color?)' is obsolete: 'SetFontStyle is deprecated, please use SetFontColor instead'
			mainMenu.AddItem(new MenuItem("scriptVersion", "Version " + AssemblyExtensions.GetVersion()).SetFontStyle(
				System.Drawing.FontStyle.Bold, Color.Coral));
#pragma warning restore CS0618 // 'MenuItem.SetFontStyle(FontStyle, Color?)' is obsolete: 'SetFontStyle is deprecated, please use SetFontColor instead'
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
