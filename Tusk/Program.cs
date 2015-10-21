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

            if (activated)
            {
                var target = me.ClosestToMouseTarget(2000);
                if (target.IsAlive && !target.IsInvul() && !me.IsInvisible() && !target.IsIllusion)
                {
                    var Q = me.Spellbook.SpellQ;
                    var W = me.Spellbook.SpellW;
                    var E = me.Spellbook.SpellE;
                    var R = me.Spellbook.SpellR;
                    var dagon = me.GetDagon();
                    var soulring = me.FindItem("item_soul_ring");
                    var halberd = me.FindItem("item_heavens_halberd");
                    var abyssal = me.FindItem("item_abyssal_blade");
                    var shiva = me.FindItem("item_shivas_guard");
                    var mjollnir = me.FindItem("item_mjollnir");
                    var mom = me.FindItem("item_mask_of_madness");
                    var satanic = me.FindItem("item_satanic");
                    var arcane = me.FindItem("item_arcane_boots");
                    var medall = me.FindItem("item_medallion_of_courage");
                    var solar = me.FindItem("item_solar_crest");
                    var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
                    bool ModifW = me.Modifiers.Any(x => x.Name == "modifier_tusk_snowball_movement");
                    bool ModifWW = me.Modifiers.All(x => x.Name == "modifier_tusk_snowball_movement");
                    var ModifInv = me.Modifiers.Any(x => x.Name == "modifier_item_invisibility_edge_windwalk") || me.Modifiers.Any(x => x.Name == "modifier_item_invisibility_edge_windwalk");
                    


                    if ( // Q Skill
                        Q != null                    &&
                        Q.CanBeCasted()              &&
                        me.CanCast()                 &&
                        ModifW                       &&
                        !target.IsMagicImmune()      &&
                        me.Distance2D(target) <= 300 &&
                        Utils.SleepCheck("Q")
                        )

                    {
                        Q.UseAbility(target.Position);
                        Utils.Sleep(200 + Game.Ping, "Q");
                    } // Q Skill end

                    if ( // W Skill
                         W != null &&
                        W.CanBeCasted() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("W")
                        )
                    {
                        W.UseAbility(target);
                        W.UseAbility();
                        Utils.Sleep(140 + Game.Ping, "W");
                    }
                    if(
                          ModifWW  &&
                        Utils.SleepCheck("WW")
                        )
                        W.UseAbility();// W Skill end
                    Utils.Sleep(150 + Game.Ping, "WW");


                    if ( // E Skill
                        E != null               &&
                        E.CanBeCasted()         &&
                        !ModifInv &&
                        me.CanCast()            &&
                        ModifW                  &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("E")   &&
                        me.Distance2D(target) <= 200
                        )
                    {
                        E.UseAbility();
                        Utils.Sleep(350 + Game.Ping, "E");
                    } // E Skill end
                    if (//R Skill
                        (solar != null || medall != null) &&
                        R != null                         &&
                        R.CanBeCasted()                   &&
                        me.CanCast()                      &&
                        !linkens                          &&
                        me.Distance2D(target) <= 350      &&
                        Utils.SleepCheck("R")
                        )
                    {
                        R.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "R");
                    } // R Skill end

                    if (//R Skill
                        (solar == null||medall == null)&&
                        R != null                    &&
                        R.CanBeCasted()              &&
                        me.CanCast()                 &&
                        me.Distance2D(target) <= 350 &&
                        Utils.SleepCheck("R")
                        )
                    {
                        R.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "R");
                    } // R Skill end

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
                        medall.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("Medall") &&
                        me.Distance2D(target) <= 500
                        )
                    {
                        medall.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "Medall");
                    } // Medall Item end

                    if ( // solar
                        solar != null &&
                        solar.CanBeCasted() && !ModifInv &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("solar") &&
                        me.Distance2D(target) <= 500
                        )
                    {
                        solar.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "solar");
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
                        me.Health / me.MaximumHealth <= 0.3 &&
                        satanic.CanBeCasted() && !ModifInv &&
                        me.Distance2D(target) <= 300
                        )
                    {
                        satanic.UseAbility();
                    } // Satanic Item end
                    var enemyHeroes = ObjectMgr.GetEntities<Hero>().Where(x =>  x.Team == me.GetEnemyTeam() && x.IsAlive && x.IsVisible && !x.IsMagicImmune()
                         && Utils.SleepCheck(x.ClassID.ToString()) && !x.IsIllusion);
                    var Sigl = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
                        && x.IsAlive  && x.IsControllable);

                    foreach (var enemy in enemyHeroes)
                    {
                            foreach (var SigVar in Sigl)
                            {
                                if (enemy.Position.Distance2D(SigVar.Position) < 1500 &&
                                    Utils.SleepCheck(SigVar.Handle.ToString()))
                                {
                                    SigVar.Follow(enemy);
                                    Utils.Sleep(1000, SigVar.Handle.ToString());
                                }
                            }
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
 
 
 
