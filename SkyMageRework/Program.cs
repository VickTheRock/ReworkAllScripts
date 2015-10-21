using System;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace SkyMageRework
{
    internal class Program
    {
        
        private static bool activated;
        private static bool toggle;
        private static bool blinkToggle = true; 
        private static bool useUltimate = true;
        private static Font txt;
        private static Font noti;
        private static Line lines;
        private static Key keyCombo = Key.D;
        private static Key toggleKey = Key.M;
        private static Key blinkToggleKey = Key.P;
        private static Key UseUltimate = Key.H;
        

        static void Main(string[] args)
        {
            
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> SkyWrath# loaded!");

            txt = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Segoe UI",
                   Height = 17,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.ClearType
               });

            noti = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Segoe UI",
                   Height = 30,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.ClearType
               });

            lines = new Line(Drawing.Direct3DDevice9);

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage || me == null)
            {
                return;
            }
            var target = me.ClosestToMouseTarget(2000);
            var blink = me.FindItem("item_blink");
            var Q = me.Spellbook.SpellQ;
            var W = me.Spellbook.SpellW;
            var E = me.Spellbook.SpellE;
            var R = me.Spellbook.SpellR;
            var dagon = me.GetDagon();
            var soulring = me.FindItem("item_soul_ring");
            var atos = me.FindItem("item_rod_of_atos");
            var shiva = me.FindItem("item_shivas_guard");
            var ethereal = me.FindItem("item_ethereal_blade");
            var arcane = me.FindItem("item_arcane_boots");
            var sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
            var cheese = me.FindItem("item_cheese");
            var vail = me.FindItem("item_veil_of_discord");
            var orchid = me.FindItem("item_orchid");
            var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
            var ModifW = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_concussive_shot_slow");
            var ModifR = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mystic_flare_aura_effect");
           // var ModifE = target.Modifiers.Any(y => y.Name == "modifier_skywrath_mage_ancient_seal");
            var ModifRod = target.Modifiers.Any(y => y.Name == "modifier_item_rod_of_atos_debuf");
            var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
            // Main combo

            if (activated && toggle && me.IsAlive && target.IsAlive)
            {
                var noBlade = target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");
                if (target != null && target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
                {
                    if (W.CanBeCasted()              &&
                        me.CanCast()                 &&
                        me.Distance2D(target) < 1370 &&
                        Utils.SleepCheck("W"))
                    {
                        W.UseAbility();
                        Utils.Sleep(300, "W");
                    }

                    if (Q.CanBeCasted()     &&
                        blinkToggle         &&
                        blink.CanBeCasted() &&
                        me.Distance2D(target) > 1000 &&
                        Utils.SleepCheck("blink"))
                    {
                        blink.UseAbility(target.Position);
                        Utils.Sleep(250, "blink");
                    }
                    if (Q.CanBeCasted() &&
                        me.Distance2D(target) < 1100 &&
                        Utils.SleepCheck("Q"))
                    {
                        Q.UseAbility(target);
                        Utils.Sleep(150, "Q");
                    }
                    
                    if (E.CanBeCasted() &&
                        me.Position.Distance2D(target) < 1200 &&
                        !linkens &&
                        Utils.SleepCheck("E"))
                    {
                        E.UseAbility(target);
                        Utils.Sleep(200, "E");
                    }
                     if (R.CanBeCasted()                        &&
                         useUltimate                            &&
                         !ModifR                                &&
                         (ModifW                                ||
                         ModifEther                             ||
                         ModifRod)                              &&
                         me.Position.Distance2D(target) < 1200 &&
                         Utils.SleepCheck("R"))
                     {
                         R.UseAbility(target.Position);
                         Utils.Sleep(330, "R");
                     }

                    if (// SoulRing Item 
                        soulring != null &&
                        me.Health / me.MaximumHealth <= 0.4 &&
                        me.Mana <= R.ManaCost &&
                        soulring.CanBeCasted())
                    {
                        soulring.UseAbility();
                    } // SoulRing Item end

                    if (// Arcane Boots Item
                        arcane != null &&
                        me.Mana <= R.ManaCost &&
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
                        Utils.Sleep(250 + Game.Ping, "vail");
                    } // orchid Item end

                    if ( // orchid
                        orchid != null &&
                        orchid.CanBeCasted() &&
                        me.CanCast() &&
                        !target.IsMagicImmune() &&
                        !linkens &&
                        Utils.SleepCheck("orchid") &&
                        me.Distance2D(target) <= 1000
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
                        !linkens &&
                        Utils.SleepCheck("sheep") &&
                        me.Distance2D(target) <= 900
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
                        !linkens &&
                        Utils.SleepCheck("atos") &&
                        me.Distance2D(target) <= 2000
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
                        Utils.SleepCheck("dagon")
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
                        Utils.SleepCheck("dagon")
                       )
                    {
                        dagon.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "dagon");
                    } // Dagon Item end

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
                        Utils.Sleep(150 + Game.Ping, "ethereal");
                    } // ethereal Item end
                    if (// Dagon
                         (ethereal == null || !ethereal.CanBeCasted()) &&

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
                         // cheese
                         cheese != null && cheese.CanBeCasted() &&
                         Utils.SleepCheck("cheese") &&
                         me.Distance2D(target) <= 700)
                     {
                         cheese.UseAbility();
                         Utils.Sleep(150 + Game.Ping, "cheese");
                    } // cheese Item end
                }
            }
        }

        static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            var player = ObjectMgr.LocalPlayer;
            var me = ObjectMgr.LocalHero;
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
                return;

            if (activated && toggle)
            {
                DrawBox(2, 37, 110, 20, 1, new ColorBGRA(0, 0, 100, 100));
                DrawFilledBox(2, 37, 110, 20, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("SkyWrath#: Comboing!", 4, 37, Color.Violet, txt);
            }

            if (toggle && !activated)
            {
                DrawBox(2, 37, 560, 54, 1, new ColorBGRA(0, 0, 100, 100));
                DrawFilledBox(2, 37, 560, 54, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("SkyWrath#: Enabled\nBlink on/off(P): " + blinkToggle + " | UseUlt on/off(H): " + useUltimate + " | [" + keyCombo + "] for combo \n[" + toggleKey + "] For toggle combo | [" + blinkToggleKey + "] For toggle blink | [" + UseUltimate + "] For toggle UseUlt ", 4, 37, Color.Violet, txt);
            }
            if (!toggle)
            {
                DrawBox(2, 37, 185, 20, 1, new ColorBGRA(0, 0, 100, 100));
                DrawFilledBox(2, 37, 185, 20, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("SkyWrath#: Disabled | [" + toggleKey + "] for toggle", 4, 37, Color.Turquoise, txt);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Game.IsKeyDown(keyCombo))
                {
                    activated = true;
                }
                else
                {
                    activated = false;
                }

                if (Game.IsKeyDown(toggleKey) && Utils.SleepCheck("toggle"))
                {
                    toggle = !toggle;
                    Utils.Sleep(150, "toggle");
                }

                if (Game.IsKeyDown(UseUltimate) && Utils.SleepCheck("useUltimate"))
                {
                    useUltimate = !useUltimate;
                    Utils.Sleep(150, "useUltimate");
                }

                if (Game.IsKeyDown(blinkToggleKey) && Utils.SleepCheck("toggleBlink"))
                {
                    blinkToggle = !blinkToggle;
                    Utils.Sleep(150, "toggleBlink");
                }

               
                
            }
        }

        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            txt.Dispose();
            noti.Dispose();
            lines.Dispose();
        }

       

        static void Drawing_OnPostReset(EventArgs args)
        {
            txt.OnResetDevice();
            noti.OnResetDevice();
            lines.OnResetDevice();
        }

        static void Drawing_OnPreReset(EventArgs args)
        {
            txt.OnLostDevice();
            noti.OnLostDevice();
            lines.OnLostDevice();
        }

        public static void DrawFilledBox(float x, float y, float w, float h, Color color)
        {
            var vLine = new Vector2[2];

            lines.GLLines = true;
            lines.Antialias = false;
            lines.Width = w;

            vLine[0].X = x + w / 2;
            vLine[0].Y = y;
            vLine[1].X = x + w / 2;
            vLine[1].Y = y + h;

            lines.Begin();
            lines.Draw(vLine, color);
            lines.End();
        }

        public static void DrawBox(float x, float y, float w, float h, float px, Color color)
        {
            DrawFilledBox(x, y + h, w, px, color);
            DrawFilledBox(x - px, y, px, h, color);
            DrawFilledBox(x, y - px, w, px, color);
            DrawFilledBox(x + w, y, px, h, color);
        }

        public static void DrawShadowText(string stext, int x, int y, Color color, Font f)
        {
            f.DrawText(null, stext, x + 1, y + 1, Color.Black);
            f.DrawText(null, stext, x, y, color);
        }
    }
}
 
 
