using System;
using System.Linq;
using System.Collections.Generic;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;
using Ensage.Common.Menu;
namespace EmberSpirit
{
	internal class Program
	{
		private static readonly Menu Menu = new Menu("EmberSpirit", "EmberSpirit", true, "npc_dota_hero_ember_spirit", true);
		private static Item mom, abyssal, soulring, arcane, shiva, halberd, mjollnir, satanic, dagon, orchid;
		private static Ability Q, W, E, R, D;
		
		private static Font txt;
		private static Font not;
		private static bool loaded;
		static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;
			Game.OnUpdate += Game_OnUpdateQW;
			Menu.AddItem(new MenuItem("w", "Use W").SetValue(new KeyBind('W', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("full", "Use Full Combo").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("oneUlt", "Use One Ult").SetValue(false));
			Menu.AddItem(new MenuItem("useUlt", "Use Ult").SetValue(true));
			Menu.AddToMainMenu();
			Console.WriteLine("> EmberSpirit# loaded!");

			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Tahoma",
				   Height = 12,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.Default
			   });

			not = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Tahoma",
				   Height = 20,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.Default
			   });

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public static void Game_OnUpdate(EventArgs args)
		{
			var me = ObjectManager.LocalHero;


			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
			{
				return;
			}
			var target = me.ClosestToMouseTarget(2000);

			if (target == null)
			{
				return;
			}

			if (Game.IsKeyDown(Menu.Item("full").GetValue<KeyBind>().Key) && target.IsAlive && !target.IsInvul())
			{

				//spell
				if (Q == null)
					Q = me.Spellbook.SpellQ;

				if (W == null)
					W = me.Spellbook.SpellW;

				if (E == null)
					E = me.Spellbook.SpellE;

				if (R == null)
					R = me.Spellbook.SpellR;

				if (D == null)
					D = me.Spellbook.SpellD;

				var linkens = target.IsLinkensProtected();
				// item 
				if (satanic == null)
					satanic = me.FindItem("item_satanic");

				dagon = me.GetDagon();

				if (halberd == null)
					halberd = me.FindItem("item_heavens_halberd");

				if (abyssal == null)
					abyssal = me.FindItem("item_abyssal_blade");

				if (mjollnir == null)
					mjollnir = me.FindItem("item_mjollnir");

				if (soulring == null)
					soulring = me.FindItem("item_soul_ring");

				if (arcane == null)
					arcane = me.FindItem("item_arcane_boots");

				if (mom == null)
					mom = me.FindItem("item_mask_of_madness");

				if (shiva == null)
					shiva = me.FindItem("item_shivas_guard");

				if (orchid == null)
					orchid = me.FindItem("item_orchid");

				if ( // Q Skill
						Q != null &&
						Q.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						me.Distance2D(target) <= 140 &&
						Utils.SleepCheck("Q")
						)

				{
					Q.UseAbility();
					Utils.Sleep(20, "Q");
				} // Q Skill end

				if ( // W Skill
					W != null &&
					W.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("W")
					)
				{
					W.UseAbility(target.Position);
					Utils.Sleep(200, "W");
				} // W Skill end

				if ( // E Skill
					E != null &&
					E.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("E") &&
					me.Distance2D(target) <= 400
					)
				{
					E.UseAbility();
					Utils.Sleep(350, "E");
				} // E Skill end

				if (//R Skill
					R != null
					&& Menu.Item("useUlt").IsActive()
					&& !Menu.Item("oneUlt").IsActive()
					&& R.CanBeCasted()
					&& me.CanCast()
					&& me.Distance2D(target) <= 1100
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility(target.Position);
					Utils.Sleep(90, "R");
				} // R Skill end

				if (//R Skill
					R != null
					&& Menu.Item("useUlt").IsActive()
					&& Menu.Item("oneUlt").IsActive()
					&& R.CanBeCasted()
					&& me.CanCast()
					&& me.Distance2D(target) <= 1100
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility(target.Position);
					Utils.Sleep(7000, "R");
				} // R Skill end

				if ( // orchid
							orchid != null &&
							orchid.CanBeCasted() &&
							me.CanCast() &&
							!target.IsMagicImmune() &&
							!linkens &&
							Utils.SleepCheck("orchid") &&
							me.Distance2D(target) <= 1000
							)
				{
					orchid.UseAbility(target);
					Utils.Sleep(250, "orchid");
				} // orchid Item end

				if (// SoulRing Item 
					soulring != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					me.Mana <= D.ManaCost &&
					soulring.CanBeCasted())
				{
					soulring.UseAbility();
				} // SoulRing Item end

				if (// Arcane Boots Item
					arcane != null &&
					me.Mana <= D.ManaCost &&
					arcane.CanBeCasted())
				{
					arcane.UseAbility();
				} // Arcane Boots Item end

				if (// Shiva Item
					shiva != null &&
					shiva.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("shiva") &&
					me.Distance2D(target) <= 600
					)
				{
					shiva.UseAbility();
					Utils.Sleep(250, "shiva");
				} // Shiva Item end

				if (// MOM
					mom != null &&
					mom.CanBeCasted() &&
					me.CanCast() &&
					Utils.SleepCheck("mom") &&
					me.Distance2D(target) <= 700
					)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				} // MOM Item end


				if ( // Abyssal Blade
					abyssal != null &&
					abyssal.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("abyssal") &&
					me.Distance2D(target) <= 400
					)
				{
					abyssal.UseAbility(target);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end

				if ( // Hellbard
					halberd != null &&
					halberd.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("halberd") &&
					me.Distance2D(target) <= 700
					)
				{
					halberd.UseAbility(target);
					Utils.Sleep(250, "halberd");
				} // Hellbard Item end

				if ( // Mjollnir
					mjollnir != null &&
					mjollnir.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("mjollnir") &&
					me.Distance2D(target) <= 900
				   )
				{
					mjollnir.UseAbility(me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end

				if (// Dagon
					dagon != null &&
					dagon.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("dagon")
				   )
				{
					dagon.UseAbility(target);
					Utils.Sleep(150, "dagon");
				} // Dagon Item end


				if (// Satanic 
					satanic != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(target) <= 400)
				{
					satanic.UseAbility();
				} // Satanic Item end
				if (
					(!me.CanAttack() || me.Distance2D(target) >= 0) && me.NetworkActivity != NetworkActivity.Attack &&
					me.Distance2D(target) <= 600 && Utils.SleepCheck("Move")
					)
				{
					me.Move(target.Predict(300));
					Utils.Sleep(390, "Move");
				}
				if (
					me.Distance2D(target) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !target.IsAttackImmune())
					&& me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
					)
				{
					me.Attack(target);
					Utils.Sleep(150, "attack");
				}
				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_ember_spirit_remnant").ToList();

				if (remnant.Count <= 0)
					return;
				for (int i = 0; i < remnant.Count; i++)
				{
						if (//D Skill
						   remnant != null
						   && D.CanBeCasted()
						   && me.CanCast()
						   && remnant[i].Distance2D(target) <= 600
						   && Utils.SleepCheck("D")
						   )
						{
							D.UseAbility(target.Position);
							Utils.Sleep(400, "D");
						}
				}
				
			}

		}



		public static void Game_OnUpdateQW(EventArgs args)
		{
			var me = ObjectManager.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
			{
				return;
			}

			var target = me.ClosestToMouseTarget(2000);

			if (target == null)
			{
				return;
			}
			if (Game.IsKeyDown(Menu.Item("w").GetValue<KeyBind>().Key) && target.IsAlive && !target.IsInvul())
			{

				var linkens = target.IsLinkensProtected();
				// item 
				if (satanic == null)
					satanic = me.FindItem("item_satanic");

				dagon = me.GetDagon();

				if (halberd == null)
					halberd = me.FindItem("item_heavens_halberd");

				if (abyssal == null)
					abyssal = me.FindItem("item_abyssal_blade");

				if (mjollnir == null)
					mjollnir = me.FindItem("item_mjollnir");

				if (soulring == null)
					soulring = me.FindItem("item_soul_ring");

				if (arcane == null)
					arcane = me.FindItem("item_arcane_boots");

				if (mom == null)
					mom = me.FindItem("item_mask_of_madness");

				if (shiva == null)
					shiva = me.FindItem("item_shivas_guard");

				if (orchid == null)
					orchid = me.FindItem("item_orchid");

				//spell
				if (Q == null)
					Q = me.Spellbook.SpellQ;

				if (W == null)
					W = me.Spellbook.SpellW;

				if ( // Q Skill
						Q != null &&
						Q.CanBeCasted() &&
						me.CanCast() &&
						!target.IsMagicImmune() &&
						me.Distance2D(target) <= 140 &&
						Utils.SleepCheck("Q")
						)

				{
					Q.UseAbility();
					Utils.Sleep(40, "Q");
				} // Q Skill end

				if ( // W Skill
					W != null &&
					W.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					(W.CanBeCasted() &&
					Utils.SleepCheck("W"))
					)
				{
					W.UseAbility(target.Position);
					Utils.Sleep(200, "W");
				} // W Skill end
				if ( // orchid
							orchid != null &&
							orchid.CanBeCasted() &&
							me.CanCast() &&
							!target.IsMagicImmune() &&
							!linkens &&
							Utils.SleepCheck("orchid") &&
							me.Distance2D(target) <= 1000
							)
				{
					orchid.UseAbility(target);
					Utils.Sleep(250, "orchid");
				} // orchid Item end

				if (// SoulRing Item 
					soulring != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					me.Mana <= D.ManaCost &&
					soulring.CanBeCasted())
				{
					soulring.UseAbility();
				} // SoulRing Item end

				if (// Arcane Boots Item
					arcane != null &&
					me.Mana <= D.ManaCost &&
					arcane.CanBeCasted())
				{
					arcane.UseAbility();
				} // Arcane Boots Item end

				if (// Shiva Item
					shiva != null &&
					shiva.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("shiva") &&
					me.Distance2D(target) <= 600
					)
				{
					shiva.UseAbility();
					Utils.Sleep(250, "shiva");
				} // Shiva Item end

				if (// MOM
					mom != null &&
					mom.CanBeCasted() &&
					me.CanCast() &&
					Utils.SleepCheck("mom") &&
					me.Distance2D(target) <= 700
					)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				} // MOM Item end


				if ( // Abyssal Blade
					abyssal != null &&
					abyssal.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("abyssal") &&
					me.Distance2D(target) <= 400
					)
				{
					abyssal.UseAbility(target);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end

				if ( // Hellbard
					halberd != null &&
					halberd.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("halberd") &&
					me.Distance2D(target) <= 700
					)
				{
					halberd.UseAbility(target);
					Utils.Sleep(250, "halberd");
				} // Hellbard Item end

				if ( // Mjollnir
					mjollnir != null &&
					mjollnir.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("mjollnir") &&
					me.Distance2D(target) <= 900
				   )
				{
					mjollnir.UseAbility(me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end

				if (// Dagon
					dagon != null &&
					dagon.CanBeCasted() &&
					me.CanCast() &&
					!target.IsMagicImmune() &&
					Utils.SleepCheck("dagon")
				   )
				{
					dagon.UseAbility(target);
					Utils.Sleep(150, "dagon");
				} // Dagon Item end


				if (// Satanic 
					satanic != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(target) <= 400)
				{
					satanic.UseAbility();
				} // Satanic Item end


				if (
					(!me.CanAttack() || me.Distance2D(target) >= 0) && me.NetworkActivity != NetworkActivity.Attack &&
					me.Distance2D(target) <= 600 && Utils.SleepCheck("Move")
					)
				{
					me.Move(target.Predict(300));
					Utils.Sleep(390, "Move");
				}
				if (
					me.Distance2D(target) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !target.IsAttackImmune())
					&& me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
					)
				{
					me.Attack(target);
					Utils.Sleep(150, "attack");
				}

			}
		}



		

		static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			not.Dispose();
		}

		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			var me = ObjectManager.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
				return;

			if (Game.IsKeyDown(Menu.Item("full").GetValue<KeyBind>().Key))
			{
				txt.DrawText(null, "Ember: Comboing!", 4, 170, Color.Green);
			}

			if (!Game.IsKeyDown(Menu.Item("full").GetValue<KeyBind>().Key))
			{
				txt.DrawText(null, "Ember: go combo for toggle combo", 4, 170, Color.Aqua);
			}
			if (Game.IsKeyDown(Menu.Item("w").GetValue<KeyBind>().Key))
			{
				txt.DrawText(null, "Ember: ComboingQW!", 4, 190, Color.Green);
			}

			if (!Game.IsKeyDown(Menu.Item("w").GetValue<KeyBind>().Key))
			{
				txt.DrawText(null, "Ember: go comboQW  for toggle combo", 4, 190, Color.Aqua);
			}
			if(!Menu.Item("useUlt").IsActive())
			{
				txt.DrawText(null, "Ember: Don't Use Ult!", 4, 230, Color.DarkRed);
			}
			if (Menu.Item("oneUlt").IsActive())
			{
				txt.DrawText(null, "Ember: Use 1 Remnant!", 4, 220, Color.DarkOrchid);
			}
			if (!Menu.Item("oneUlt").IsActive())
			{
				txt.DrawText(null, "Ember: Use All Remnant!", 4, 220, Color.DarkOrchid);
			}


		}

		static void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			not.OnResetDevice();
		}

		static void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			not.OnLostDevice();
		}
	}
}
