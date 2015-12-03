//ONLY LOVE MazaiPC ;) 
using System;
using System.Linq;
using System.Collections.Generic;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace Tuskar
{
    internal class Program
    {
        private static bool activated;
        private static Font txt;
        private static Font not;
        private static Key KeyCombo = Key.D;
        private static Item mom, abyssal, soulring, arcane, blink, shiva, halberd, mjollnir, satanic, dagon, medall;
        private static Ability Q, W, E, R, D, F;
        private static bool loaded;
        private static Hero me;
        private static Hero target;
        private static ParticleEffect rangeDisplay;
        static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> Tusk# loaded!");

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
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Tusk)
            {
                return;
            }
            var target = me.ClosestToMouseTarget(2000);

            if (target == null)
            {
                return;
            }

            if (activated)
			{
				var ModifW = me.Modifiers.Any(x => x.Name == "modifier_tusk_snowball_movement");

				var teamarm = ObjectMgr.GetEntities<Hero>().Where(ally =>
							 ally.Team == me.Team && ally.IsAlive && me.Distance2D(ally) <= 400
							 && !ally.Modifiers.Any(x => x.Name == "modifier_tusk_snowball_movement_friendly"));

				var unitToSnow = ObjectMgr.GetEntities<Unit>().Where(x => ((x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
				|| x.ClassID == ClassID.CDOTA_Unit_SpiritBear || x.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
				|| x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling || x.ClassID == ClassID.CDOTA_BaseNPC_Creep)
				&& !x.IsAttackImmune() && !x.IsInvul() && x.IsVisible && x.IsAlive && me.Distance2D(x) <= 395)
				   && x.IsAlive && x.IsControllable
				   && !x.Modifiers.Any(z => z.Name == "modifier_tusk_snowball_movement_friendly") && !x.Modifiers.Any(z => z.Name == "modifier_tusk_snowball_movement"));
				if (ModifW)
				{

					foreach (Hero v in teamarm.ToList())
					{
						if (ModifW && v.Distance2D(me) < 395 && !v.Modifiers.Any(z => z.Name == "modifier_tusk_snowball_movement_friendly") && !v.IsInvul() && !v.IsAttackImmune() && v.IsAlive && Utils.SleepCheck(v.Handle.ToString()))
						{
							me.Attack(v);
							Utils.Sleep(100, v.Handle.ToString());
						}
					}
					foreach (Unit v in unitToSnow)
					{
						if (ModifW && v.Distance2D(me) < 395 && !v.Modifiers.Any(z => z.Name == "modifier_tusk_snowball_movement_friendly") && !v.IsInvul() && !v.IsAttackImmune() && v.IsAlive && Utils.SleepCheck(v.Handle.ToString()))
						{
							me.Attack(v);
							Utils.Sleep(100, v.Handle.ToString());
						}
					}
				}
			
				if (target.IsAlive && !target.IsInvul() && !me.IsInvisible() && !target.IsIllusion)
                {
                    if (Q == null)
                        Q = me.Spellbook.SpellQ;

                    var W = me.Spellbook.SpellW;

                    if (E == null)
                        E = me.Spellbook.SpellE;

                    if (R == null)
                        R = me.Spellbook.SpellR;



                    // item 
                    if (satanic == null)
                        satanic = me.FindItem("item_satanic");

                        dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

                    if (halberd == null)
                        halberd = me.FindItem("item_heavens_halberd");

                    if (medall == null)
                        medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

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
                    

                    var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
                   
                    var ModifWW = me.Modifiers.All(x => x.Name == "modifier_tusk_snowball_movement");
                    var ModifInv = me.Modifiers.All(x => x.Name == "modifier_item_invisibility_edge_windwalk");
                    var medallModiff = target.Modifiers.Any(x => x.Name == "modifier_item_medallion_of_courage_armor_reduction") || target.Modifiers.Any(x => x.Name == "modifier_item_solar_crest_armor_reduction");
                    

                    if (
                        ModifInv             &&
                        Utils.SleepCheck("invi")
                        )
                    {
                        R.UseAbility(target);
                        Utils.Sleep(200 + Game.Ping, "invi");
                    }

                    if ( // Q Skill
                        Q != null                    &&
                        Q.CanBeCasted()              &&
                        me.CanCast()                 &&
                        !ModifInv &&
                        ModifW                       &&
                        !target.IsMagicImmune()      &&
                        me.Distance2D(target) <= 300 &&
                        Utils.SleepCheck("Q")
                        )

                    {
                        Q.UseAbility(target.Position);
                        Utils.Sleep(200 + Game.Ping, "Q");
                    } // Q Skill end


                    if (//R Skill
                       
                       
                       R != null &&
                       medallModiff &&
                       R.CanBeCasted() &&
                       me.CanCast() &&
                       !linkens &&
                       me.Distance2D(target) <= 350 &&
                       Utils.SleepCheck("R")
                       )
                    {
                        R.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "R");
                    } // R Skill end

					if (//R Skill


					   R != null &&
						target.IsMagicImmune() &&
					   R.CanBeCasted() &&
					   me.CanCast() &&
					   !linkens &&
					   me.Distance2D(target) <= 350 &&
					   Utils.SleepCheck("R")
					   )
					{
						R.UseAbility(target);
						Utils.Sleep(150 + Game.Ping, "R");
					} // R Skill end

					if (//R Skill
                        medall == null &&
                        R != null &&
                        R.CanBeCasted() &&
                        me.CanCast() &&
                        me.Distance2D(target) <= 350 &&
                        Utils.SleepCheck("R")
                        )
                    {
                        R.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "R");
                    } // R Skill end

                   
                    if ( // E Skill
                        E != null               &&
                        E.CanBeCasted()         &&
                        !ModifInv               &&
                        me.CanCast()            &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("E")   &&
                        me.Distance2D(target) <= 200
                        )
                    {
                        E.UseAbility();
                        Utils.Sleep(350 + Game.Ping, "E");
                    } // E Skill end
                   

                    if (// SoulRing Item 
                        soulring != null &&
                        me.Health / me.MaximumHealth <= 0.4 &&
                        !ModifInv &&
                        me.Mana <= Q.ManaCost &&
                        soulring.CanBeCasted())
                    {
                        soulring.UseAbility();
                    } // SoulRing Item end

                    if (// Arcane Boots Item
                        arcane != null &&
                        me.Mana <= Q.ManaCost &&
                        !ModifInv &&
                        arcane.CanBeCasted())
                    {
                        arcane.UseAbility();
                    } // Arcane Boots Item end

                    if (// Shiva Item
                        shiva != null &&
                        shiva.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("shiva") &&
                        me.Distance2D(target) <= 600
                        )
                    {
                        shiva.UseAbility();
                        Utils.Sleep(250 + Game.Ping, "shiva");
                    } // Shiva Item end

                    if (// MOM
                        mom != null &&
                        mom.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        Utils.SleepCheck("mom") &&
                        me.Distance2D(target) <= 700
                        )
                    {
                        mom.UseAbility();
                        Utils.Sleep(250 + Game.Ping, "mom");
                    } // MOM Item end

                    if ( // Medall
                        medall != null &&
                        medall.CanBeCasted() && 
                        !ModifInv &&
                        Utils.SleepCheck("Medall") &&
                        me.Distance2D(target) <= 500
                        )
                    {
                        medall.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "Medall");
                    } // Medall Item end

                    if ( // Abyssal Blade
                        abyssal != null &&
                        abyssal.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("abyssal") &&
                        me.Distance2D(target) <= 400
                        )
                    {
                        abyssal.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "abyssal");
                    } // Abyssal Item end

                    if ( // Hellbard
                        halberd != null &&
                        halberd.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("halberd") &&
                        me.Distance2D(target) <= 700
                        )
                    {
                        halberd.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "halberd");
                    } // Hellbard Item end

                    if ( // Mjollnir
                        mjollnir != null &&
                        mjollnir.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("mjollnir") &&
                        me.Distance2D(target) <= 900
                       )
                    {
                        mjollnir.UseAbility(me);
                        Utils.Sleep(250 + Game.Ping, "mjollnir");
                    } // Mjollnir Item end

                    if (// Dagon
                        dagon != null &&
                        dagon.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("dagon")
                       )
                    {
                        dagon.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "dagon");
                    } // Dagon Item end


                    if (// Satanic 
                        satanic != null &&
                        me.Health <= (me.MaximumHealth * 0.3) &&
                        satanic.CanBeCasted() && !ModifInv &&
                        me.Distance2D(target) <= 300
                        )
                    {
                        satanic.UseAbility();
                    } // Satanic Item end

                    if ( // W Skill
                       W != null &&
                       !ModifInv &&
                      W.CanBeCasted() &&
                      !target.IsMagicImmune() &&
                      Utils.SleepCheck("W")
                      )
                    {
                        W.UseAbility(target);
                        W.UseAbility();
                        Utils.Sleep(120 + Game.Ping, "W");
                    }
					var Sigl = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
																								  && x.IsAlive && x.IsControllable);
					if (Sigl == null)
					{
						return;
					}
					foreach (var v in Sigl)
					{

						if (target.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Follow(target);
							Utils.Sleep(350, v.Handle.ToString());
						}
					}
					if (
						(!me.CanAttack()
						|| me.Distance2D(target) >= 140)
						&& me.NetworkActivity != NetworkActivity.Attack
						&& me.Distance2D(target) <= 800
						&& Utils.SleepCheck("Move"))
					{
						me.Move(target.Predict(300));
						Utils.Sleep(390 + Game.Ping, "Move");
					}
					else if (
					   me.Distance2D(target) <= 145
					   && me.CanAttack()
					   && Utils.SleepCheck("R")
					   )
					{
						me.Attack(target);
						Utils.Sleep(150 + Game.Ping, "R");
					}
				}
            }
        }



       


        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Game.IsKeyDown(KeyCombo))
                    activated = true;
                else
                    activated = false;
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

            var me = ObjectMgr.LocalHero;
            var player = ObjectMgr.LocalPlayer;
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Tusk)
                return;

            if (activated )
            {
                txt.DrawText(null, "Tusk#: Combo Active", 5, 190, Color.Firebrick);
            }

            if (!activated)
            {
                txt.DrawText(null, "Tusk#: go combo  [" + KeyCombo + "] for toggle combo D", 5, 190, Color.Aqua);
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
 
 
 
