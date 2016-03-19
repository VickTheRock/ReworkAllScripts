//ONLY LOVE MazaiPC ;) 
using System;
using System.Linq;
using System.Collections.Generic;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage.Common.Menu;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace TinyAutoCombo
{
	internal class Program
	{
		private static readonly Menu Menu = new Menu("Tiny", "Tiny by Vick", true, "npc_dota_hero_tiny", true);
		private static Item soulring, arcane, blink, shiva, dagon, mjollnir, mom, halberd, abyssal, ethereal, cheese, satanic, medall;
		private static Ability Q, W;
		private static Font txt;
		private static Font not;
		private static bool keyCombo;

		private static bool loaded;
		private static Hero me;
		private static Hero target;
		static void Main(string[] args)
		{
			Menu.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
			Console.WriteLine("> Tiny# loaded!");
			
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
				   Height = 170,
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
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Tiny)
			{
				return;
			}
			keyCombo = Game.IsKeyDown(Menu.Item("ComboKey").GetValue<KeyBind>().Key);
			if (keyCombo)
			{
				var target = me.ClosestToMouseTarget(2000);
				if (target == null)
				{
					return;
				}
				if (target.IsAlive && !target.IsInvul())
				{


					//spell
					if (Q == null)
						Q = me.Spellbook.SpellQ;

					if (W == null)
						W = me.Spellbook.SpellW;


					// item 
					if (satanic == null)
						satanic = me.FindItem("item_satanic");

					if (shiva == null)
						shiva = me.FindItem("item_shivas_guard");

					dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

					if (arcane == null)
						arcane = me.FindItem("item_arcane_boots");

					if (mom == null)
						mom = me.FindItem("item_mask_of_madness");
					
						medall = me.FindItem("item_shivas_guard") ?? me.FindItem("item_solar_crest");

					if (ethereal == null)
						ethereal = me.FindItem("item_ethereal_blade");

					if (blink == null)
						blink = me.FindItem("item_blink");

					if (soulring == null)
						soulring = me.FindItem("item_soul_ring");

					if (cheese == null)
						cheese = me.FindItem("item_cheese");

					if (halberd == null)
						halberd = me.FindItem("item_heavens_halberd");

					if (abyssal == null)
						abyssal = me.FindItem("item_abyssal_blade");

					if (mjollnir == null)
						mjollnir = me.FindItem("item_mjollnir");
					var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
					var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");





					if (target.IsVisible && me.Distance2D(target) <= 1200)
					{
						if (
						   (!me.CanAttack()
						   || me.Distance2D(target) > 0)
						   && me.NetworkActivity != NetworkActivity.Attack
						   && me.Distance2D(target) <= 800
						   && Utils.SleepCheck("Move"))
						{
							me.Move(target.Predict(500));
							Utils.Sleep(390, "Move");
						}
						else if (
						   me.Distance2D(target) <= 280
						   && me.CanAttack()
						   && me.NetworkActivity != NetworkActivity.Attack
						   && Utils.SleepCheck("Attack")
						   )
						{
							me.Attack(target);
							Utils.Sleep(me.AttacksPerSecond+40, "Attack");
						}

						if (
							Q != null
							&& Q.CanBeCasted()
							&& blink != null
							&& blink.CanBeCasted()
							&& me.Position.Distance2D(target.Position) > 300 &&
							Utils.SleepCheck("blink"))
						{
							blink.UseAbility(target.Position);
							Utils.Sleep(250, "blink");
						}
						if (
							Q != null
						&& Q.CanBeCasted()
						&& me.Position.Distance2D(target.Position) < 350 &&
							Utils.SleepCheck("Q"))
						{
							Q.UseAbility(target.Position);
							Utils.Sleep(150, "Q");
						}
						if (W != null
							&& W.CanBeCasted()
							&& me.Position.Distance2D(target.Position) < 300
							&& Utils.SleepCheck("W"))
						{
							W.UseAbility(target);
							Utils.Sleep(150, "W");
						}

						if (// SoulRing Item 
							soulring != null &&
							me.Health >= (me.MaximumHealth * 0.3) &&
							me.Mana <= Q.ManaCost &&
							soulring.CanBeCasted())
						{
							soulring.UseAbility();
						} // SoulRing Item end

						if (// Arcane Boots Item
							arcane != null &&
							me.Mana <= Q.ManaCost &&
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

						if ( // Medall
					   medall != null &&
					   medall.CanBeCasted() &&
					   me.CanCast() &&
					   !target.IsMagicImmune() &&
					   Utils.SleepCheck("Medall") &&
					   me.Distance2D(target) <= 500
					   )
						{
							medall.UseAbility(target);
							Utils.Sleep(250, "Medall");
						} // Medall Item end

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




						if (// ethereal
					   ethereal != null &&
					   ethereal.CanBeCasted() &&
					   me.CanCast() &&
					   !linkens &&
					   !target.IsMagicImmune() &&
					   Utils.SleepCheck("ethereal")
					  )
						{
							ethereal.UseAbility(target);
							Utils.Sleep(150, "ethereal");
						} // ethereal Item end

						if (// Dagon
						   dagon != null &&
						   ethereal != null &&
						   ModifEther &&
						   dagon.CanBeCasted() &&
						   me.CanCast() &&
						   !target.IsMagicImmune() &&
						   Utils.SleepCheck("dagon")
						  )
						{
							dagon.UseAbility(target);
							Utils.Sleep(150, "dagon");
						} // Dagon Item end


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
							(ethereal == null || !ethereal.CanBeCasted()) &&
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
							me.Distance2D(target) <= 700 &&
							Utils.SleepCheck("Satanic")
							)
						{
							satanic.UseAbility();
							Utils.Sleep(350, "Satanic");
						} // Satanic Item end
						
					}
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
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Tiny)
				return;

			if (keyCombo)
			{
				txt.DrawText(null, "Tiny#: Comboing!", 4, 150, Color.Gold);
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



