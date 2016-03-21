//ONLY LOVE MazaiPC ;) 
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
namespace Nyx_by_Vick
{
    internal class Program
    {
		private static readonly Menu Menu = new Menu("Nyx", "Nyx by Vick", true, "npc_dota_hero_nyx_assassin", true);
        private static Item soulring, arcane, blink, shiva, dagon, mjollnir, mom, halberd, abyssal, ethereal, cheese, satanic, medall, vail;
        private static Ability Q, W, R;
		private static bool keyCombo;
		private static Font txt;
        private static Font not;
        private static bool loaded;
        private static Hero me;
        private static Hero target;
		private static readonly Dictionary<string, bool> Skills = new Dictionary<string, bool>
			{
				{"nyx_assassin_vendetta",true}
			};
		static void Main(string[] args)
        {
			Menu.AddItem(new MenuItem("ComboKey", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("Skills", ":").SetValue(new AbilityToggler(Skills)));
			Menu.AddToMainMenu();

			Game.OnUpdate += Game_OnUpdate;
            Console.WriteLine("> Nyx by Vick# loaded!");

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
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Nyx_Assassin)
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

					if (R == null)
						R = me.Spellbook.SpellR;

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

					vail = me.FindItem("item_veil_of_discord");

					if (medall == null)
                        medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

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
						|| me.Distance2D(target) >= 120)
						&& me.NetworkActivity != NetworkActivity.Attack
						&& me.Distance2D(target) <= 1200
						&& Utils.SleepCheck("Move"))
						{
							me.Move(target.Predict(300));
							Utils.Sleep(390, "Move");
						}
						else if (
						   me.Distance2D(target) <= 145
						   && me.CanAttack()
						   && Utils.SleepCheck("R")
						   )
						{
							me.Attack(target);
							Utils.Sleep(230, "R");
						}
						if (
						   R != null
						   && R.CanBeCasted()
						   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						   && !me.Modifiers.Any(x => x.Name == "modifier_nyx_assassin_vendetta")
						   && me.Distance2D(target) <= 1400
						   && Utils.SleepCheck("R")
						   )
						{
							R.UseAbility();
							Utils.Sleep(200, "R");
						}
						if (me.Modifiers.Any(x => x.Name == "modifier_nyx_assassin_vendetta"))
							return;
						if (!R.CanBeCasted() || R == null && !me.Modifiers.Any(x => x.Name == "modifier_nyx_assassin_vendetta") || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
						{


							if ( // vail
							vail != null &&
							vail.CanBeCasted() &&
							me.CanCast() &&
							!target.IsMagicImmune() &&
							Utils.SleepCheck("vail") &&
							me.Distance2D(target) <= 1500
							)
							{
								vail.UseAbility(target.Position);
								Utils.Sleep(250, "vail");
							}

							if (
								Q != null 
								&& Q.CanBeCasted() &&
								blink.CanBeCasted() &&
								me.Position.Distance2D(target.Position) > 300 &&
								Utils.SleepCheck("blink"))
							{
								blink.UseAbility(target.Position);
								Utils.Sleep(250, "blink");
							}
							if (// ethereal
						   ethereal != null &&
						   ethereal.CanBeCasted()
						   && (!vail.CanBeCasted()
						   || vail == null)
						   && me.CanCast() &&
						   !linkens &&
						   !target.IsMagicImmune() &&
						   Utils.SleepCheck("ethereal")
						  )
							{
								ethereal.UseAbility(target);
								Utils.Sleep(150, "ethereal");
							}


							
							if ((vail ==null|| !vail.CanBeCasted()) && (ethereal ==null || !ethereal.CanBeCasted()) && !R.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
							{

								if (
									Q != null 
									&& Q.CanBeCasted() 
									&& me.Position.Distance2D(target.Position) < Q.CastRange - 50 &&
									Utils.SleepCheck("Q"))
								{
									Q.CastSkillShot(target);
									Utils.Sleep(100, "Q");
								}
								if (W != null 
									&& W.CanBeCasted()
									&& target.Mana >= (target.MaximumMana * 0.3)
									&& me.Position.Distance2D(target.Position) < 800
									&& Utils.SleepCheck("W"))
								{
									W.UseAbility(target);
									Utils.Sleep(100, "W");
								}

								// orchid Item end
								if (// SoulRing Item 
									soulring != null &&
									me.Health >= (me.MaximumHealth * 0.3) &&
									me.Mana <= R.ManaCost &&
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




								// ethereal Item end

								if (// Dagon
								   (dagon != null &&
								   ethereal != null &&
								   ModifEther || !ethereal.CanBeCasted())
								   && dagon.CanBeCasted() &&
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
									(ethereal == null || !ethereal.CanBeCasted())
									&& dagon.CanBeCasted() &&
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
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Nyx_Assassin)
                return;

            if (keyCombo)
            {
                txt.DrawText(null, "Nyx Combo Active!", 4, 150, Color.Gold);
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
 
 
 
