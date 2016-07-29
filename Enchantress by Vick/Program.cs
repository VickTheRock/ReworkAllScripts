namespace Enchantress
{
	using System;
	using System.Linq;
	using Ensage.Common.Menu;

	using Ensage;
	using SharpDX;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using SharpDX.Direct3D9;
	using System.Windows.Input;
	using System.Collections.Generic;
	internal class Program
    {
		private static readonly Menu Menu = new Menu("Enchantress", "Enchantress", true, "npc_dota_hero_enchantress", true);
		private static bool Active;
		private static Hero me, e;
		private static Ability W, E, R;
		private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, mom, staff, pike;


		private static void OnLoadEvent(object sender, EventArgs args)
		{
			if (ObjectManager.LocalHero.ClassID != ClassID.CDOTA_Unit_Hero_Enchantress) return;

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"enchantress_impetus", true},
					{"enchantress_natures_attendants", true},
					{"enchantress_enchant", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_hurricane_pike", true},
					{"item_ethereal_blade", true},
					{"item_sheepstick", true},
					{"item_cheese", true},
					{"item_mask_of_madness", true},
					{"item_ghost", true},
					{"item_bloodthorn", true},
					{"item_orchid", true},
					{"item_rod_of_atos", true},
					{"item_soul_ring", true},
					{"item_arcane_boots", true},
					{"item_blink", true},
					{"item_shivas_guard", true},
					{"item_force_staff", true}
				})));
			Menu.AddItem(new MenuItem("piKeMe", "Min me healh use hurricane pike").SetValue(new Slider(45, 20, 100)));
			Menu.AddToMainMenu();

			Game.OnUpdate += Game_OnUpdate;
			Print.LogMessage.Success("Light of foot, light of heart!");
		}

		private static void OnCloseEvent(object sender, EventArgs args)
		{
			Game.OnUpdate -= Game_OnUpdate;
			Menu.RemoveFromMainMenu();
		}

		private static void Main()
		{
			Events.OnLoad += OnLoadEvent;
			Events.OnClose += OnCloseEvent;
		}

		private static void Game_OnUpdate(EventArgs args)
        {
            me = ObjectManager.LocalHero;

            if (!Game.IsInGame || Game.IsWatchingGame || me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Enchantress) return;
			
			if (!Menu.Item("enabled").IsActive())
				return;
			e = me.ClosestToMouseTarget(2000);
            if (e == null)return;
				
                W = me.Spellbook.SpellW;
                E = me.Spellbook.SpellE;
                R = me.Spellbook.SpellR;

            // Item
                ethereal = me.FindItem("item_ethereal_blade");
                sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
                vail = me.FindItem("item_veil_of_discord");
                cheese = me.FindItem("item_cheese");
                mom = me.FindItem("item_mask_of_madness");
                ghost = me.FindItem("item_ghost");
                orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
				atos = me.FindItem("item_rod_of_atos");
                soulring = me.FindItem("item_soul_ring");
                arcane = me.FindItem("item_arcane_boots");
                blink = me.FindItem("item_blink");
                shiva = me.FindItem("item_shivas_guard");
                staff = me.FindItem("item_force_staff");
				pike = me.FindItem("item_hurricane_pike");

			var linkens = e.IsLinkensProtected();
            var ModifRod = e.HasModifier("modifier_item_rod_of_atos_debuff");
            var Wmodif = e.HasModifier("modifier_enchantress_enchant_slow");

			var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
			if (Active && me.IsAlive && e.IsAlive && Utils.SleepCheck("activated"))
            {
	            if (pike != null)
	            {
		            //var pikeMod = me.Modifiers.ToList().Exists(y => y.Name == "modifier_item_hurricane_pike_range");
		            var pike_range = me.Modifiers.FirstOrDefault(y => y.Name == "modifier_item_hurricane_pike_range");

		            if (
			            R != null
			            && !R.CanBeCasted()
			            && pike_range != null
			            && pike_range.StackCount > 0
			            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
			            )
		            {
			            Orbwalking.Orbwalk(e, 0, 7000, true, true);
		            }
		            if (pike != null
		                && pike.CanBeCasted()
		                && me.IsAttacking()
		                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(pike.Name)
		                && (e.Health <= (((me.MinimumDamage + me.MaximumDamage) / 2 + me.BonusDamage)* 4)
		                    || me.Health <= (me.MaximumHealth/100*Menu.Item("piKeMe").GetValue<Slider>().Value))
		                && me.Distance2D(e) <= pike.GetCastRange()+me.HullRadius
		                && Utils.SleepCheck("pike"))
		            {
			            pike.UseAbility(e);
						Utils.Sleep(100, "pike");
					}
	            }
	            var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
	            if (e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
				{
					if (
						R != null
						&& R.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("orbwalk").GetValue<bool>()
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						)
					{
						Orbwalking.Orbwalk(e, 0, 7000, true, true);
					}
					else if ((R == null || !R.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
						&& Menu.Item("orbwalk").GetValue<bool>() 
						)
					{
						Orbwalking.Orbwalk(e, 0, 7000, false, true);
					}
					if (stoneModif) return;
					if (
			            W != null
			            && W.CanBeCasted()
			            && me.Distance2D(e) >= 400
			            && !ModifRod
			            && me.CanCast()
			            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
			            && !linkens
			            && Utils.SleepCheck("W"))
		            {
			            W.UseAbility(e);
			            Utils.Sleep(300, "W");
		            }
		            float angle = me.FindAngleBetween(e.Position, true);
		            Vector3 pos = new Vector3((float) (e.Position.X + 200*Math.Cos(angle)),
			            (float) (e.Position.Y + 200*Math.Sin(angle)), 0);
		            if (
			            blink != null
			            && R.CanBeCasted()
			            && me.CanCast()
			            && blink.CanBeCasted()
			            && me.Distance2D(e) >= me.GetAttackRange() + me.HullRadius
			            && me.Distance2D(pos) <= 1190
			            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
			            && Utils.SleepCheck("blink")
			            )
		            {
			            blink.UseAbility(pos);
			            Utils.Sleep(250, "blink");
		            }
		            if (
			            E != null
			            && E.CanBeCasted()
			            && me.CanCast()
			            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
			            && me.Position.Distance2D(e) <= 1200
			            && (me.Health <= (me.MaximumHealth*0.7))
			            && Utils.SleepCheck("E"))
		            {
			            E.UseAbility();
			            Utils.Sleep(200, "E");
		            }

                    if (// SoulRing Item 
                        soulring != null
						&& soulring.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
						&& me.Health >= (me.MaximumHealth * 0.3)
						&& me.Mana <= 150           
                        )
                    {
                        soulring.UseAbility();
                    } // SoulRing Item end

                    if (// Arcane Boots Item
                        arcane != null
						&& arcane.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
						&& me.Mana <= R.ManaCost 
                        )
                    {
                        arcane.UseAbility();
                    } // Arcane Boots Item end

                    if ( // staff
                       staff != null
					   && staff.CanBeCasted()
					   && me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(staff.Name)
					   && !e.IsMagicImmune()
					   && Utils.SleepCheck("staff")
					   && me.Distance2D(e) <= 250
                       )
                    {
                        staff.UseAbility(e);
                        Utils.Sleep(200, "staff");
                    } // staff Item end

                    if (//Ghost
                        ghost != null
						&& ghost.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
						&& (me.Position.Distance2D(e) <= 200
						&& me.Health <= (me.MaximumHealth * 0.5) ||
						me.Health <= (me.MaximumHealth * 0.3))
					    && Utils.SleepCheck("Ghost"))
                    {
                        ghost.UseAbility();
                        Utils.Sleep(250, "Ghost");
                    }

                    if (// Shiva Item
                        shiva != null
						&& shiva.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
						&& !e.IsMagicImmune()
						&& Utils.SleepCheck("shiva")
						&& me.Distance2D(e) <= 600
                        )
                    {
                        shiva.UseAbility();
                        Utils.Sleep(250, "shiva");
                    } // Shiva Item end

                    if (// MOM
                        mom != null
						&& mom.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
						&& me.Distance2D(e) <= 1100
						&& Utils.SleepCheck("mom")
                        )
                    {
                        mom.UseAbility();
                        Utils.Sleep(250, "mom");
                    } // MOM Item end

                    if ( // vail
                        vail != null
						&& vail.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
						&& !e.IsMagicImmune()
						&& me.Distance2D(e) <= 1500
						&& Utils.SleepCheck("vail")
                        )
                    {
                        vail.UseAbility(e.Position);
                        Utils.Sleep(250, "vail");
                    } // orchid Item end

                    if ( // orchid
                        orchid != null
						&& orchid.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
						&& !e.IsMagicImmune()
						&& !linkens
						&& Utils.SleepCheck("orchid")
						&& me.Distance2D(e) <= 1600
                        )
                    {
                        orchid.UseAbility(e);
                        Utils.Sleep(250, "orchid");
                    } // orchid Item end

                    if ( // sheep
                        sheep != null
						&& sheep.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
						&& !e.IsMagicImmune()
						&& !linkens
						&& Utils.SleepCheck("sheep")
						&& me.Distance2D(e) <= 1100
                        )
                    {
                        sheep.UseAbility(e);
                        Utils.Sleep(250, "sheep");
                    } // sheep Item end

                    if ( // atos Blade
                        atos != null        
						&& atos.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
						&& !e.IsMagicImmune()
						&& !linkens                 
						&& !Wmodif
						&& me.Distance2D(e) <= 2000
						&& Utils.SleepCheck("atos")
						)
                    {
                        atos.UseAbility(e);
                        Utils.Sleep(250, "atos");
                    } // atos Item end

                    if (// ethereal
                        ethereal != null
						&& ethereal.CanBeCasted()
						&& me.CanCast()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
						&& !linkens
						&& !e.IsMagicImmune()
						&& Utils.SleepCheck("ethereal")
                       )
                    {
                        ethereal.UseAbility(e);
                        Utils.Sleep(200, "ethereal");
                    } // ethereal Item end


					if ( // Dagon
						me.CanCast()
						&& dagon != null
						&& (ethereal == null
						|| (e.HasModifier("modifier_item_ethereal_blade_slow")
						|| ethereal.Cooldown < 17))
						&& !e.IsLinkensProtected()
						&& dagon.CanBeCasted()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
						&& !e.IsMagicImmune()
						&& Utils.SleepCheck("dagon")
						)
					{
						dagon.UseAbility(e);
						Utils.Sleep(200, "dagon");
					} // Dagon Item end
					if (
                         // cheese
                         cheese != null
						 && cheese.CanBeCasted()
						 && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
						 && me.Health <= (me.MaximumHealth * 0.3)
						 && Utils.SleepCheck("cheese")
						 && me.Distance2D(e) <= 700
                         )
                     {
                         cheese.UseAbility();
                         Utils.Sleep(200, "cheese");
                    } // cheese Item end
                }
                Utils.Sleep(200, "activated");
            }
        }
		
    }
	internal class Print
	{
		public class LogMessage
		{
			public static void Success(string text, params object[] arguments)
			{
				Game.PrintMessage("<font color='#e0007b'>" + text + "</font>", MessageType.LogMessage);
			}
		} // Console class
	}
}
 
 
