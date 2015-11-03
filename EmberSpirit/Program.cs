using System;
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
        
        private static Item mom, abyssal, soulring, arcane, shiva, halberd, mjollnir, satanic, dagon;
        private static Ability Q, W, E, R, D;
        private static bool activated;
        private static bool togleQW;
      //  private static bool Altcombo;
        private static Font txt;
        private static Font not;
        private static Key KeyCombo = Key.C;
        private const Key TogleQW = Key.Z;
       // private const Key Altthisshit = Key.F;
        private static bool loaded;
        static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdateQW;
       //     Game.OnUpdate += Game_OnUpdateAltCombo;
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
            var target = me.ClosestToMouseTarget(2000);

            if (target == null)
            {
                return;
            }

            if (activated && target.IsAlive && !target.IsInvul())
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

                // item 
                if (satanic == null)
                    satanic = me.FindItem("item_satanic");
                
                    dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

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
                        Utils.SleepCheck("W")
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
                        Utils.SleepCheck("E") &&
                        me.Distance2D(target) <= 400
                        )
                    {
                        E.UseAbility();
                        Utils.Sleep(350 + Game.Ping, "E");
                    } // E Skill end

                    if (//R Skill
                        R != null &&
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
                        Utils.SleepCheck("shiva") &&
                        me.Distance2D(target) <= 600
                        )
                    {
                        shiva.UseAbility();
                        Utils.Sleep(250 + Game.Ping, "shiva");
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
                        Utils.Sleep(250 + Game.Ping, "mom");
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
                        Utils.Sleep(250 + Game.Ping, "abyssal");
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
                        Utils.Sleep(250 + Game.Ping, "halberd");
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
                        Utils.Sleep(250 + Game.Ping, "mjollnir");
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
                        Utils.Sleep(150 + Game.Ping, "dagon");
                    } // Dagon Item end
                    
                    
                    if (// Satanic 
                        satanic != null &&
                        me.Health / me.MaximumHealth <= 0.3 &&
                        satanic.CanBeCasted() &&
                        me.Distance2D(target) <= 400)
                    {
                        satanic.UseAbility();
                    } // Satanic Item end
                var remnant = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_ember_spirit_remnant").ToList();
                if (remnant == null)
                {
                    return;
                }
               
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
            
        }

   /*     public static void Game_OnUpdateAltCombo(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
            {
                return;
            }

            var target = me.ClosestToMouseTarget(2000);

            if (target == null)
            {
                return;
            }
            if (Altcombo && target.IsAlive && !target.IsInvul())
                
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

                // item 
                if (satanic == null)
                    satanic = me.FindItem("item_satanic");

                if (dagon == null)
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
                
                var wModif = me.Modifiers.Any(y => y.Name == "modifier_ember_spirit_sleight_of_fist_caster");


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
                    Utils.SleepCheck("W")
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
                    Utils.SleepCheck("E") &&
                    me.Distance2D(target) <= 400
                    )
                {
                    E.UseAbility();
                    Utils.Sleep(350 + Game.Ping, "E");
                } // E Skill end

                if (//R Skill
                    R != null &&
                    R.CanBeCasted() &&
                    wModif           &&
                    me.CanCast() &&
                    me.Distance2D(target) <= 400 &&
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
                    Utils.SleepCheck("shiva") &&
                    me.Distance2D(target) <= 600
                    )
                {
                    shiva.UseAbility();
                    Utils.Sleep(250 + Game.Ping, "shiva");
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
                    Utils.Sleep(250 + Game.Ping, "mom");
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
                    Utils.Sleep(250 + Game.Ping, "abyssal");
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
                    Utils.Sleep(250 + Game.Ping, "halberd");
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
                    Utils.Sleep(250 + Game.Ping, "mjollnir");
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
                    Utils.Sleep(150 + Game.Ping, "dagon");
                } // Dagon Item end


                if (// Satanic 
                    satanic != null &&
                    me.Health / me.MaximumHealth <= 0.3 &&
                    satanic.CanBeCasted() &&
                    me.Distance2D(target) <= 400)
                {
                    satanic.UseAbility();
                } // Satanic Item end


                var remnant = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_ember_spirit_remnant").ToList();
                var goRemnant = target.Distance2D(remnant.First());

                if (//D Skill
                   D.CanBeCasted() &&
                   wModif          &&
                   me.CanCast()     &&
                   goRemnant <= 400 &&
                   Utils.SleepCheck("D")
                   )
                {
                    D.UseAbility(target.Position);
                    Utils.Sleep(300 + Game.Ping, "D");
                }

            }
        }*/

        public static void Game_OnUpdateQW(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit)
            {
                return;
            }

            var target = me.ClosestToMouseTarget(2000);

            if (target == null)
            {
                return;
            }
            if (togleQW && target.IsAlive && !target.IsInvul())
            {



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
                    Utils.Sleep(40 + Game.Ping, "Q");
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
            {
                if (Game.IsKeyDown(TogleQW))
                    togleQW = true;
                else
                    togleQW = false;
            }
           /* {
                if (Game.IsKeyDown(Altthisshit))
                    Altcombo = true;
                else
                    Altcombo = false;
            }*/
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
                txt.DrawText(null, "Ember#: Comboing!", 4, 170, Color.Green);
            }

            if (!activated)
            {
                txt.DrawText(null, "Ember#: go combo  [" + KeyCombo + "] for toggle combo", 4, 170, Color.Aqua);
            }
            if (togleQW)
            {
                txt.DrawText(null, "Ember#: ComboingQW!", 4, 190, Color.Green);
            }

            if (!togleQW)
            {
                txt.DrawText(null, "Ember#: go comboQW  [" + TogleQW + "] for toggle combo", 4, 190, Color.Aqua);
            }
          /*  if (Altcombo)
            {
                txt.DrawText(null, "Ember#: ComboingAltCombo(test)!", 4, 220, Color.Green);
            }

            if (!Altcombo)
            {
                txt.DrawText(null, "Ember#: ALT COMBO(NEED MORE TARGETS)  [" + Altthisshit + "] for toggle combo", 4, 220, Color.DarkOrchid);
            }*/


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
 
 
 
