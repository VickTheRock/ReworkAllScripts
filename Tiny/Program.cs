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

namespace TinyAutoCombo
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
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Tiny)
            {
                return;
            }

            if (activated && toggle)
            {
                var target = me.ClosestToMouseTarget(2000);
                if (target.IsAlive && !target.IsInvul())
                {
                    var blink = me.FindItem("item_blink");
                    var Q = me.Spellbook.SpellQ;
                    var W = me.Spellbook.SpellW;
                    var dagon = me.GetDagon();
                    var soulring = me.FindItem("item_soul_ring");
                    var halberd = me.FindItem("item_heavens_halberd");
                    var abyssal = me.FindItem("item_abyssal_blade");
                    var shiva = me.FindItem("item_shivas_guard");
                    var mjollnir = me.FindItem("item_mjollnir");
                    var mom = me.FindItem("item_mask_of_madness");
                    var satanic = me.FindItem("item_satanic");
                    var arcane = me.FindItem("item_arcane_boots");
                    var diffusal = me.FindItem("item_diffusal_blade");
                    var wand = me.FindItem("item_magic_wand");
                    var stick = me.FindItem("item_magic_stick");
                    var cheese = me.FindItem("item_cheese");
                    var orchid = me.FindItem("item_orchid");
                    if (target != null && target.IsAlive && target.IsVisible && me.Distance2D(target) <= 1200)
                    {
                        if (Q.CanBeCasted() &&
                            blink.CanBeCasted()  &&
                            me.Position.Distance2D(target.Position) > 350 &&
                            Utils.SleepCheck("blink"))
                        {
                            blink.UseAbility(target.Position);
                            Utils.Sleep(250, "blink");
                        }
                        if (Q.CanBeCasted() &&
                            me.Position.Distance2D(target.Position) < 350 &&
                            Utils.SleepCheck("Q"))
                        {
                            Q.UseAbility(target.Position);
                            Utils.Sleep(150, "Q");
                        }
                        if (W.CanBeCasted() && target.IsStunned() &&
                            me.Position.Distance2D(target.Position) < 350 &&
                            Utils.SleepCheck("blink"))
                        {
                            W.UseAbility(target);
                            Utils.Sleep(100, "W");
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

                        if (// MOM
                            mom != null &&
                            mom.CanBeCasted() &&
                            me.CanCast() &&
                            (mom.CanBeCasted() &&
                            Utils.SleepCheck("mom") &&
                            me.Distance2D(target) <= 700)
                            )
                        {
                            mom.UseAbility();
                            Utils.Sleep(250 + Game.Ping, "mom");
                        } // MOM Item end


                        if ( // Abyssal Blade
                            abyssal != null &&
                            abyssal.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (abyssal.CanBeCasted() &&
                            Utils.SleepCheck("abyssal") &&
                            me.Distance2D(target) <= 400)
                            )
                        {
                            abyssal.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "abyssal");
                        } // Abyssal Item end
                        
                        if ( // Hellbard
                            halberd != null &&
                            halberd.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (halberd.CanBeCasted() &&
                            Utils.SleepCheck("halberd") &&
                            me.Distance2D(target) <= 700)
                            )
                        {
                            halberd.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "halberd");
                        } // Hellbard Item end

                        if ( // Mjollnir
                            mjollnir != null &&
                            mjollnir.CanBeCasted() &&
                            me.CanCast() &&
                            !target.IsMagicImmune() &&
                            (mjollnir.CanBeCasted() &&
                            Utils.SleepCheck("mjollnir") &&
                            me.Distance2D(target) <= 900)
                           )
                        {
                            mjollnir.UseAbility(me);
                            Utils.Sleep(250 + Game.Ping, "mjollnir");
                        } // Mjollnir Item end

                        if (// Dagon
                            dagon != null &&
                            dagon.CanBeCasted() &&
                            me.CanCast() &&
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

                        if (// Satanic 
                            satanic != null &&
                            me.Health / me.MaximumHealth <= 0.3 &&
                            satanic.CanBeCasted() &&
                            me.Distance2D(target) <= 700)
                        {
                            satanic.UseAbility();
                        } // Satanic Item end
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
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Tiny)
                return;

            if (activated )
            {
                txt.DrawText(null, "Tiny#: Comboing!", 4, 150, Color.Green);
            }

            if (!activated)
            {
                txt.DrawText(null, "Tiny#: go combo  [" + KeyCombo + "] for toggle combo", 4, 150, Color.Aqua);
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
 
 
 
