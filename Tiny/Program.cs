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
        private static bool AvalancheCasted;
        private static bool TossCasted;
        private static bool BlinkCasted;
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

            if (activated && toggle && me.CanCast())
            {
                var target = me.ClosestToMouseTarget(2000);
                if (target.IsAlive && !target.IsInvul())
                {
                    var blink = me.Inventory.Items.FirstOrDefault(x => x.Name == "item_blink");
                    var Q = me.Spellbook.SpellQ;
                    var W = me.Spellbook.SpellW;

                    if (target != null && target.IsAlive && target.IsVisible)
                    {
                        if (Vector3.DistanceSquared(me.Position, target.Position) > 400 && BlinkCasted == false)
                        {
                            blink.UseAbility(target.Position);
                            BlinkCasted = true;
                            return;
                        }
                        else
                        {
                            if (Q.AbilityState == AbilityState.Ready)
                            {
                                Q.UseAbility(target.Position);
                                AvalancheCasted = true;
                                TossCasted = false;
                                BlinkCasted = false;
                                return;
                            }
                            if (W.AbilityState == AbilityState.Ready)
                            {
                                W.UseAbility(target);
                                AvalancheCasted = false;
                                TossCasted = true;
                                return;
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
 
 
 