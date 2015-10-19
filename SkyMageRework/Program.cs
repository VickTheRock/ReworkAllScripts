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

namespace SkyMage
{
    internal class Program
    {
        private static bool activated;
        private static bool toggle = true;
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
            Console.WriteLine("> SkyMage Combo# loaded!");

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
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
            {
                return;
            }

            if (activated && toggle)
                
            {
                var target = me.ClosestToMouseTarget(2000);
                var noBlade = target.Modifiers.Any(y => y.Name != "modifier_item_blade_mail_reflect");

                if (target.IsAlive && !target.IsInvul() && noBlade)
                {
                    var blink = me.FindItem("item_blink");
                    var Q = me.Spellbook.SpellQ;
                    var W = me.Spellbook.SpellW;
                    var E = me.Spellbook.SpellE;
                    var R = me.Spellbook.SpellR;
                    var dagon = me.GetDagon();
                    var soulring = me.FindItem("item_soul_ring");
                    var halberd = me.FindItem("item_heavens_halberd");
                    var atos = me.FindItem("item_rod_of_atos");
                    var shiva = me.FindItem("item_shivas_guard");
                    var mjollnir = me.FindItem("item_mjollnir");
                    var mom = me.FindItem("item_mask_of_madness");
                    var ethereal = me.FindItem("item_ethereal_blade");
                    var satanic = me.FindItem("item_satanic");
                    var arcane = me.FindItem("item_arcane_boots");
                    var sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
                    var wand = me.FindItem("item_magic_wand");
                    var stick = me.FindItem("item_magic_stick");
                    var cheese = me.FindItem("item_cheese");
                    var vail = me.FindItem("item_veil_of_discord");
                    var orchid = me.FindItem("item_orchid");
                    var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
                    var ModifW = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_concussive_shot_slow");
                    var ModifR = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect");
                    //var ModifE = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_ancient_seal");
                    var ModifRod = target.Modifiers.Any(y => y.Name == "modifier_item_rod_of_atos_debuf");
                    var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");


                    if (target != null && target.IsAlive && target.IsVisible && me.Distance2D(target) <= 1200)
                    {
                        if (Q.CanBeCasted() &&
                            blink.CanBeCasted()  &&
                            me.Position.Distance2D(target) > 900 &&
                            Utils.SleepCheck("blink"))
                        {
                            blink.UseAbility(target.Position);
                            Utils.Sleep(250, "blink");
                        }
                        if (Q.CanBeCasted() &&
                            me.Position.Distance2D(target.Position) < 900 &&
                            Utils.SleepCheck("Q"))
                        {
                            Q.UseAbility(target);
                            Utils.Sleep(150, "Q");
                        }
                        if (W.CanBeCasted()  &&
                            me.Position.Distance2D(target) < 1350 &&
                            Utils.SleepCheck("W"))
                        {
                            W.UseAbility();
                            Utils.Sleep(300, "W");
                        }
                        if (E.CanBeCasted() &&
                            me.Position.Distance2D(target) < 1400 &&
                            !linkens &&
                            Utils.SleepCheck("E"))
                        {
                            E.UseAbility(target);
                            Utils.Sleep(200, "E");
                        }
                        if (R.CanBeCasted()                       &&
                            !ModifR                               &&
                            (ModifW                                ||
                            ModifEther                             ||
                            ModifRod)                              &&
                            me.Position.Distance2D(target) < 1100 &&
                            Utils.SleepCheck("E"))
                        {
                            R.UseAbility(target.Position);
                            Utils.Sleep(330, "E");
                        }

                        if (// SoulRing Item 
                            soulring != null &&
                            me.Health / me.MaximumHealth <= 0.4 &&
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
                            (shiva.CanBeCasted() &&
                            Utils.SleepCheck("shiva") &&
                            me.Distance2D(target) <= 600)
                            )
                        {
                            shiva.UseAbility();
                            Utils.Sleep(250 + Game.Ping, "shiva");
                        } // Shiva Item end

                        if ( // vail
                            vail != null &&
                            vail.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (vail.CanBeCasted() &&
                            Utils.SleepCheck("vail") &&
                            me.Distance2D(target) <= 1000)
                            )
                        {
                            vail.UseAbility(target.Position);
                            Utils.Sleep(250 + Game.Ping, "vail");
                        } // orchid Item end

                        if ( // orchid
                            orchid != null &&
                            orchid.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (orchid.CanBeCasted() &&
                            !linkens &&
                            Utils.SleepCheck("orchid") &&
                            me.Distance2D(target) <= 1000)
                            )
                        {
                            orchid.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "orchid");
                        } // orchid Item end

                        if ( // sheep
                            sheep != null &&
                            sheep.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (sheep.CanBeCasted() &&
                            !linkens &&
                            Utils.SleepCheck("sheep") &&
                            me.Distance2D(target) <= 900)
                            )
                        {
                            sheep.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "sheep");
                        } // sheep Item end

                        if ( // atos Blade
                            atos != null &&
                            atos.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (atos.CanBeCasted() &&
                            !linkens &&
                            Utils.SleepCheck("atos") &&
                            me.Distance2D(target) <= 2000)
                            )
                        {
                            atos.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "atos");
                        } // atos Item end
                        if (// Dagon
                            vail != null &&
                            dagon != null &&
                            dagon.CanBeCasted() &&
                            ModifEther &&
                            me.CanCast() &&
                            !linkens &&
                            !target.IsMagicImmune() &&
                            (dagon.CanBeCasted() &&
                            Utils.SleepCheck("dagon"))
                           )
                        {
                            dagon.UseAbility(target);
                            Utils.Sleep(150 + Game.Ping, "dagon");
                        } // Dagon Item end

                        if (// Dagon
                            ethereal != null &&
                            dagon != null &&
                            dagon.CanBeCasted() &&
                            ModifEther &&
                            me.CanCast() &&
                            !linkens &&
                            !target.IsMagicImmune() &&
                            (dagon.CanBeCasted() &&
                            Utils.SleepCheck("dagon"))
                           )
                        {
                            dagon.UseAbility(target);
                            Utils.Sleep(150 + Game.Ping, "dagon");
                        } // Dagon Item end

                        if (// ethereal
                            ethereal != null &&
                            ethereal.CanBeCasted()  &&
                            me.CanCast()            &&
                            !linkens                &&
                            !target.IsMagicImmune() &&
                            (ethereal.CanBeCasted() &&
                            Utils.SleepCheck("ethereal"))
                           )
                        {
                            ethereal.UseAbility(target);
                            Utils.Sleep(150 + Game.Ping, "ethereal");
                        } // ethereal Item end
                        if (// Dagon
                             (ethereal== null || !ethereal.CanBeCasted()) &&
                             
                            dagon != null &&
                            dagon.CanBeCasted() &&
                            me.CanCast() &&
                            !linkens &&
                            !target.IsMagicImmune() &&
                            (dagon.CanBeCasted() &&
                            Utils.SleepCheck("dagon"))
                           )
                        {
                            dagon.UseAbility(target);
                            Utils.Sleep(150 + Game.Ping, "dagon");
                        } // Dagon Item end

                        if (
                            // Stick
                            (stick != null && stick.CanBeCasted()) ||
                            (wand != null && wand.CanBeCasted()) ||
                            (cheese != null && cheese.CanBeCasted()) &&
                            Utils.SleepCheck("stick") &&
                            me.Distance2D(target) <= 700)
                        {
                            stick.UseAbility();
                            wand.UseAbility();
                            cheese.UseAbility();
                            Utils.Sleep(150 + Game.Ping, "stick");
                        } // Stick Item end
                        
                    }
                }
            }
        }
      



        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Game.IsKeyDown(KeyCombo))
                {
                    activated = true;
                }
                else
                {
                    activated = false;
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

            var player = ObjectMgr.LocalPlayer;
            var me = ObjectMgr.LocalHero;
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
                return;

            if (activated )
            {
                txt.DrawText(null, "Skywrath Mage#: Comboing!", 4, 150, Color.Green);
            }

            if (!activated)
            {
                txt.DrawText(null, "Skywrath Mage#: go combo  [" + KeyCombo + "] for toggle combo", 4, 150, Color.Aqua);
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
 
 
 