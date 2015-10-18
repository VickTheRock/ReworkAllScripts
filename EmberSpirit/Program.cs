﻿using System;
using System.Linq;
using System.Collections.Generic;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace EmberSpirit
{
    internal class Program
    {
        private static Item mjollnir, mom, stick, wand, dagon, satanic, diffusal, ethereal, soulring, halberd, abyssal, shiva, arcane, cheese, orchid;
        private static bool activated;
        private static bool toggle = true;
        private static Font txt;
        private static Font not;
        private static Key KeyCombo = Key.D;
        private static bool loaded;
        private static Hero me;
        private static Hero target;
        private const Key ChaseKey = Key.D;
        private static ParticleEffect rangeDisplay;
        static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
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
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
            {
                return;
            }

            if (activated && toggle && me.CanCast())
            {
                var target = me.ClosestToMouseTarget(2000);
                if (target.IsAlive && !target.IsInvul() && target != null)
                {
                    var Q = me.Spellbook.SpellQ;
                    var W = me.Spellbook.SpellW;
                    var E = me.Spellbook.SpellE;
                    var R = me.Spellbook.SpellR;
                    var D = me.Spellbook.SpellD;
                    
                    if (dagon == null)
                        dagon = me.GetDagon();
                    if (ethereal == null)
                        ethereal = me.FindItem("item_ethereal_blade");
                    if (soulring == null)
                        soulring = me.FindItem("item_soul_ring");
                    if (halberd == null)
                        halberd = me.FindItem("item_heavens_halberd");
                    if (abyssal == null)
                        abyssal = me.FindItem("item_abyssal_blade");
                    if (shiva == null)
                        shiva = me.FindItem("item_shivas_guard");
                    if (mjollnir == null)
                        mjollnir = me.FindItem("item_mjollnir");
                    if (mom == null)
                        mom = me.FindItem("item_mask_of_madness");
                    if (satanic == null)
                        satanic = me.FindItem("item_satanic");
                    if (arcane == null)
                        arcane = me.FindItem("item_arcane_boots");
                    if (diffusal == null)
                        diffusal = me.FindItem("item_diffusal_blade");
                    if (wand == null)
                        wand = me.FindItem("item_magic_wand");
                    if (stick == null)
                        stick = me.FindItem("item_magic_stick");
                    if (cheese == null)
                        cheese = me.FindItem("item_cheese");
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
                        Utils.Sleep(20 + Game.Ping, "Q");
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
                        Utils.Sleep(200 + Game.Ping, "W");
                    } // W Skill end

                    if ( // E Skill
                        E != null &&
                        E.CanBeCasted() &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        (E.CanBeCasted() &&
                        Utils.SleepCheck("E") &&
                        me.Distance2D(target) <= 400)
                        )
                    {
                        E.UseAbility();
                        Utils.Sleep(350 + Game.Ping, "E");
                    } // E Skill end

                    if (//R Skill
                        R.CanBeCasted() &&
                        me.CanCast() &&
                        me.Distance2D(target) <= 1100 &&
                        Utils.SleepCheck("R")
                        )
                    {
                        R.UseAbility(target.Position);
                        Utils.Sleep(90 + Game.Ping, "R");
                    } // R Skill end

                    if (// SoulRing Item 
                        soulring != null &&
                        me.Health / me.MaximumHealth <= 0.4 &&
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
                        (abyssal.CanBeCasted() &&
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


                    var remnant = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_ember_spirit_remnant").ToList();
                    var goRemnant = target.Distance2D(remnant.First());

                    if (//D Skill
                       D.CanBeCasted() &&
                       me.CanCast() &&
                       goRemnant <= 600 &&
                       Utils.SleepCheck("D")
                       )
                    {
                        D.UseAbility(target.Position);
                        Utils.Sleep(350 + Game.Ping, "D");
                    }
                     
                    }
                var range = 1600;
                var canAttack = !Orbwalking.AttackOnCooldown(target) && !target.IsInvul() && !target.IsAttackImmune()
                             && me.CanAttack();
                if (canAttack)
                    if (me.Distance2D(target) <= range)
                        if (me.Distance2D(target) <= 550 && Utils.SleepCheck("attack"))
                        {
                            me.Attack(target);
                            Utils.Sleep(250, "attack");
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
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
                return;

            if (activated )
            {
                txt.DrawText(null, "Ember#: Comboing!", 4, 150, Color.Green);
            }

            if (!activated)
            {
                txt.DrawText(null, "Ember#: go combo  [" + KeyCombo + "] for toggle combo", 4, 150, Color.Aqua);
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
 
 
 
