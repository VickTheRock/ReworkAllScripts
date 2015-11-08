using System;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace Enchantress
{
    internal class Program
    {
        
        private static bool activated;
        private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, mom, staff;
        private static Ability W, E, R;
        private static bool toggle;
        private static bool blinkToggle = true; 
        private static bool useUltimate = true;
        private static Font txt;
        private static Font noti;
        private static Line lines;
        private static Key keyCombo = Key.D;
        private static Key toggleKey = Key.M;
        private static Key blinkToggleKey = Key.P;
        

        static void Main(string[] args)
        {
            
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> Enchantress # loaded!");

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

            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Enchantress || me == null)
            {
                return;
            }

            var target = me.ClosestToMouseTarget(2000);
            if (target == null)
				{
                return;
				}

            //spell
            if (W == null)
                W = me.Spellbook.SpellW;
            if (E == null)
                E = me.Spellbook.SpellE;
            if (R == null)
                R = me.Spellbook.SpellR;

            // Item
            if (ethereal == null)
                ethereal = me.FindItem("item_ethereal_blade");
            if (sheep == null)
                sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
            if (vail == null)
                vail = me.FindItem("item_veil_of_discord");
            if (cheese == null)
                cheese = me.FindItem("item_cheese");
            if (mom == null)
                mom = me.FindItem("item_mask_of_madness");
            if (ghost == null)
                ghost = me.FindItem("item_ghost");
            if (orchid == null)
                orchid = me.FindItem("item_orchid");
            if (atos == null)
                atos = me.FindItem("item_rod_of_atos");
            if (soulring == null)
                soulring = me.FindItem("item_soul_ring");
            if (arcane == null)
                arcane = me.FindItem("item_arcane_boots");
            if (blink == null)
                blink = me.FindItem("item_blink");
            if (shiva == null)
                shiva = me.FindItem("item_shivas_guard");
            if (staff == null)
                staff = me.FindItem("item_force_staff");


        var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");
            var ModifRod = target.Modifiers.Any(y => y.Name == "modifier_item_rod_of_atos_debuff");
            var ModifEther = target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
            var ModifVail = target.Modifiers.Any(y => y.Name == "modifier_item_veil_of_discord_debuff");
            var Wmodif = target.Modifiers.Any(y => y.Name == "modifier_enchantress_enchant_slow");


			if (activated && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
            {
                var noBlade = target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");
                if ( target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
                {
                    if (
                        W!= null                     &&
						W.CanBeCasted()              &&
						me.Distance2D(target) >= 400 &&
						!ModifRod					 &&
                        me.CanCast()                 &&
                        !linkens                     &&
                        Utils.SleepCheck("W"))
                    {
                        W.UseAbility(target);
                        Utils.Sleep(300, "W");
                    }

                    if (
                        blink!= null        &&
                        R.CanBeCasted()     &&
                        me.CanCast()        &&
                        blinkToggle         &&
                        blink.CanBeCasted() &&
                        me.Distance2D(target) >= 800 &&
                        me.Distance2D(target) <= 1500 &&
                        Utils.SleepCheck("blink")
                        )
                    {
                        blink.UseAbility(target.Position);
                        Utils.Sleep(250, "blink");
                    }
               
                    
                    if (
                        E!= null                               &&
                        E.CanBeCasted()                        &&
                        me.CanCast()                           &&
                        me.Position.Distance2D(target) <= 1200 &&
						(me.Health <= (me.MaximumHealth * 0.7)) &&
						Utils.SleepCheck("E"))
                    {
                        E.UseAbility();
                        Utils.Sleep(200, "E");
                    }
                     if (
                        R!=null                                 &&
                        R.CanBeCasted()                         &&
                        me.CanCast()                            &&
                         Utils.SleepCheck("R"))
                     {
                         R.UseAbility(target);
                         Utils.Sleep(330, "R");
                     }

                    if (// SoulRing Item 
                        soulring != null                    &&
                        soulring.CanBeCasted()              &&
                        me.CanCast()                        &&
                        me.Health / me.MaximumHealth <= 0.5 &&
                        me.Mana <= R.ManaCost               
                        )
                    {
                        soulring.UseAbility();
                    } // SoulRing Item end

                    if (// Arcane Boots Item
                        arcane != null &&
                        arcane.CanBeCasted() &&
                        me.CanCast() &&
                        me.Mana <= R.ManaCost 
                        )
                    {
                        arcane.UseAbility();
                    } // Arcane Boots Item end

                    if ( // staff
                       staff != null &&
                       staff.CanBeCasted() &&
                       me.CanCast() &&
                       !target.IsMagicImmune() &&
                       Utils.SleepCheck("staff") &&
                       me.Distance2D(target) <= 250
                       )
                    {
                        staff.UseAbility(target);
                        Utils.Sleep(200 + Game.Ping, "staff");
                    } // staff Item end

                    if (//Ghost
                        ghost != null                         &&
                        ghost.CanBeCasted()                   &&
                        me.CanCast()                          &&
                        (me.Position.Distance2D(target) <= 200 &&
						me.Health <= (me.MaximumHealth * 0.7) ||
						me.Health <= (me.MaximumHealth * 0.3)) &&
                        Utils.SleepCheck("Ghost"))
                    {
                        ghost.UseAbility();
                        Utils.Sleep(250, "Ghost");
                    }

                    if (// Shiva Item
                        shiva != null               &&
                        shiva.CanBeCasted()         &&
                        me.CanCast()                &&
                        !target.IsMagicImmune()     &&
                        Utils.SleepCheck("shiva")   &&
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
                        me.Distance2D(target) <= 1100
                        )
                    {
                        mom.UseAbility();
                        Utils.Sleep(250 + Game.Ping, "mom");
                    } // MOM Item end

                    if ( // vail
                        vail != null             &&
                        vail.CanBeCasted()       &&
                        me.CanCast()             &&
                        !target.IsMagicImmune()  &&
                        Utils.SleepCheck("vail") &&
                        me.Distance2D(target) <= 1500
                        )
                    {
                        vail.UseAbility(target.Position);
                        Utils.Sleep(250 + Game.Ping, "vail");
                    } // orchid Item end

                    if ( // orchid
                        orchid != null             &&
                        orchid.CanBeCasted()       &&
                        me.CanCast()               &&
                        !target.IsMagicImmune()    &&
                        !linkens                   &&
                        Utils.SleepCheck("orchid") &&
                        me.Distance2D(target) <= 1600
                        )
                    {
                        orchid.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "orchid");
                    } // orchid Item end

                    if ( // sheep
                        sheep != null             &&
                        sheep.CanBeCasted()       &&
                        me.CanCast()              &&
                        !target.IsMagicImmune()   &&
                        !linkens                  &&
                        Utils.SleepCheck("sheep") &&
                        me.Distance2D(target) <= 1100
                        )
                    {
                        sheep.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "sheep");
                    } // sheep Item end

                    if ( // atos Blade
                        atos != null             &&
                        atos.CanBeCasted()       &&
                        me.CanCast()             &&
                        !target.IsMagicImmune()  &&
                        !linkens                 &&
						!Wmodif				 &&
						Utils.SleepCheck("atos") &&
                        me.Distance2D(target) <= 2000
                        )
                    {
                        atos.UseAbility(target);
                        Utils.Sleep(250 + Game.Ping, "atos");
                    } // atos Item end

                    if (// ethereal
                        ethereal != null        &&
                        ethereal.CanBeCasted()  &&
                        me.CanCast()            &&
                        !linkens                &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("ethereal")
                       )
                    {
                        ethereal.UseAbility(target);
                        Utils.Sleep(200 + Game.Ping, "ethereal");
                    } // ethereal Item end
                    

                    if (// Dagon
                         ethereal == null        &&
                        dagon != null            &&
                        dagon.CanBeCasted()      &&
                        me.CanCast()             &&
                        !linkens                 &&
                        !target.IsMagicImmune()  &&
                        Utils.SleepCheck("dagon")
                       )
                    {
                        dagon.UseAbility(target);
                        Utils.Sleep(200 + Game.Ping, "dagon");
                    } // Dagon Item end

					if (// Attack
						(R.Level <=0 ||
						target.IsMagicImmune()) &&
						me.Distance2D(target) <= 1500 &&
						Utils.SleepCheck("Attack")
					   )
					{
						me.Attack(target);
						Utils.Sleep(250 + Game.Ping, "Attack");
					} // Attack


					if (
                         // cheese
                         cheese != null             && 
                         cheese.CanBeCasted()       &&
                         me.MaximumHealth / me.Health <= 0.3 &&
                         Utils.SleepCheck("cheese") &&
                         me.Distance2D(target) <= 700
                         )
                     {
                         cheese.UseAbility();
                         Utils.Sleep(200 + Game.Ping, "cheese");
                    } // cheese Item end
                }

                Utils.Sleep(200, "activated");
            }
        }

        static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            var player = ObjectMgr.LocalPlayer;
            var me = ObjectMgr.LocalHero;
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Enchantress)
                return;

            if (activated && toggle)
            {
                DrawBox(2, 37, 110, 20, 1, new ColorBGRA(0, 0, 100, 100));
                DrawFilledBox(2, 37, 110, 20, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("Enchantress#: Comboing!", 4, 37, Color.Violet, txt);
            }

            if (toggle && !activated)
            {
                DrawBox(2, 37, 560, 54, 1, new ColorBGRA(0, 0, 100, 100));
                DrawFilledBox(2, 37, 560, 54, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("Enchantress#: MENU\nBlink on/off(P): " + blinkToggle + " | UseUlt on/off(H): " + useUltimate + " | [" + keyCombo + "] for combo \n[" + toggleKey + "] For toggle combo | [" + blinkToggleKey + "] For toggle blink ", 4, 37, Color.Violet, txt);
            }
            if (!toggle)
            {
                DrawBox(2, 37, 185, 20, 1, new ColorBGRA(0, 0, 100, 100));
                DrawFilledBox(2, 37, 185, 20, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("Open MENU | [" + toggleKey + "]", 4, 37, Color.Turquoise, txt);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Game.IsKeyDown(keyCombo))
                    activated = true;
                else
                {
                    activated = false;
                }
                if (Game.IsKeyDown(toggleKey) && Utils.SleepCheck("toggle"))
                {
                    toggle = !toggle;
                    Utils.Sleep(150, "toggle");
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
 
 
