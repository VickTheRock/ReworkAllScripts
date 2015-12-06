using System;
using System.Linq;
using System.Collections.Generic;
using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace This_is_your_Mom
{
    internal class Program
    {

        private static bool activCombo;
        private static bool activChase;
        private static bool toggle;
        private static Item mom, abyssal, Soul, orchid, shiva, halberd, mjollnir, satanic, dagon, medall, orhid, sheep, cheese;
        private static Ability Q, W, R;
        private static Hero me;
        private static Hero target, e;
        private static int spiderDenies = 65;
        private static int spiderDmgStatick = 175;
        private static readonly uint[] spiderQ = { 74, 149, 224, 299 };
        private static readonly uint[] SoulLvl = { 120, 190, 270, 360 };
        private static int spiderDmg;
        private static bool toggleLasthit = true;
        private static bool useQ = true;
        private static Font txt;
        private static Font noti;
        private static Line lines;
        private static Key keyCHASING = Key.Space;
        private static Key keyCOMBO = Key.D;
        private static Key toggleKey = Key.D5;
        private static Key toggleLasthitKey = Key.D7;
        private static Key UseQ = Key.Q;


        static void Main(string[] args)
        {

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Chasing;
            Game.OnUpdate += ChasingAll;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> This is your Mom# loaded!");

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
            if ((!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother))
            {
                return;
            }

            if (toggleLasthit && !activCombo && !activChase && Utils.SleepCheck("combo") && !Game.IsPaused)
            {
                if (Q == null)
                    Q = me.Spellbook.SpellQ;


                if (Soul == null)
                    Soul = me.FindItem("item_soul_ring");


                var spiderlingsLevel = me.Spellbook.SpellQ.Level - 1;

                var myHero = ObjectMgr.LocalHero;

                var enemies = ObjectMgr.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != me.Team).ToList();

                var creeps = ObjectMgr.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                (creep.ClassID == ClassID.CDOTA_Unit_VisageFamiliar && creep.Team == me.GetEnemyTeam()) || (creep.ClassID == ClassID.CDOTA_Unit_SpiritBear && creep.Team == me.GetEnemyTeam()) || (creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit &&
                creep.Team == me.GetEnemyTeam()) || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
                creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Health <= 259).ToList();

                var creepQ = ObjectMgr.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                creep.ClassID == ClassID.CDOTA_Unit_SpiritBear || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
                creep.IsAlive && creep.IsVisible && creep.IsSpawned)).ToList();

                var Spiderlings = ObjectMgr.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling).ToList();


                // Creep Q lasthit
                if (useQ)
                    foreach (var creep in creepQ)
                    {
                        if (Q.CanBeCasted() && creep.Health <= Math.Floor((spiderQ[spiderlingsLevel]) * (1 - creep.MagicDamageResist)) && creep.Health > 45 && creep.Team != me.Team)
                        {
                            if (Q.CanBeCasted() && creep.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
                            {
                                if (Soul != null && Soul.CanBeCasted() && me.Health >=400)
                                {
                                    Soul.UseAbility();
                                    Utils.Sleep(300, "QQQ");
                                }
                                else
                                    Q.UseAbility(creep);
                                Utils.Sleep(300, "QQQ");

                            }
                        }
                    }
                if (useQ)
                    foreach (var creep in creepQ)
                    {
                        if (me.Mana < Q.ManaCost && creep.Health <= Math.Floor((spiderQ[spiderlingsLevel]) * (1 - creep.MagicDamageResist)) && creep.Health > 55 && creep.Team != me.Team)
                        {
                            if (creep.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
                            {
								if (Soul != null && Soul.CanBeCasted() && me.Health >= 400)
								{
                                    Soul.UseAbility();
                                    Utils.Sleep(300, "QQQ");
                                }

                            }
                        }
                    }
                // Enemy Q lasthit
                foreach (var enemy in enemies)
                {
                    if (Q.CanBeCasted() && enemy.Health <= (spiderQ[spiderlingsLevel]) && enemy.Health > 0)
                    {
                        if (enemy.Position.Distance2D(me.Position) <= 600 && Utils.SleepCheck("QQQ"))
                        {
							if (Soul != null && Soul.CanBeCasted() && me.Health >= 400)
							{
                                Soul.UseAbility();
                            }
                            else
                                Q.UseAbility(target);
                            
                        }
						Utils.Sleep(300, "QQQ");
					}
                }


                // Autodenies
                    foreach (var Spider in Spiderlings)
                    {
                        if (Spider.Health > 0 && Spider.Health <= spiderDenies)
                            foreach (var spiderlings in Spiderlings)
                            {
                                if (Spider.Position.Distance2D(spiderlings.Position) <= 500 && Utils.SleepCheck(spiderlings.Handle.ToString()))
                                {
                                    spiderlings.Attack(Spider);
                                    Utils.Sleep(700, spiderlings.Handle.ToString());

                                }
                            }
                    }

                // Auto spider deny and lasthit
                    foreach (var creep in creeps)
                    {
                        var Spiderling = ObjectMgr.GetEntities<Unit>().FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling && x.IsAlive && x.IsControllable && x.Team == me.Team);

                        foreach (var spiderling in Spiderlings)
                        {
                            if (creep != null)
                            {

                                if (creep.Position.Distance2D(spiderling.Position) <= 800 &&
                                    creep.Team != me.Team && creep.Health > 0 && creep.Health < Math.Floor(spiderDmgStatick * (1 - creep.DamageResist)) && Utils.SleepCheck(spiderling.Handle.ToString()))
                                {
                                    {
                                        spiderling.Attack(creep);
                                        Utils.Sleep(300, spiderling.Handle.ToString());
                                    }
                                }
                                else if (creep.Position.Distance2D(spiderling.Position) <= 500 &&
                                    creep.Team == me.Team && creep.Health > 0 && creep.Health < Math.Floor(spiderDmgStatick * (1 - creep.DamageResist)) && Utils.SleepCheck(spiderling.Handle.ToString()))
                                {
                                    spiderling.Attack(creep);
                                    Utils.Sleep(300, spiderling.Handle.ToString());
                                }
                            }
                        }
                    }

                // Auto spider enemy lasthit
                if (Utils.SleepCheck("attacking_enemy"))
                {
                    foreach (var enemy in enemies)
                    {
                        foreach (var spiderling in Spiderlings)
                        {
                            if (enemy != null)
                            {
                                spiderDmg = Spiderlings.Count(y => y.Distance2D(enemy) < 800) * spiderling.MinimumDamage;

                                if ((enemy.Position.Distance2D(spiderling.Position)) <= 800 &&
                                    enemy.Team != me.Team && enemy.Health > 0 && enemy.Health < Math.Floor(spiderDmg * (1 - enemy.DamageResist))  && Utils.SleepCheck(spiderling.Handle.ToString()))
                                {
                                    spiderling.Attack(enemy);
                                    Utils.Sleep(500, spiderling.Handle.ToString());
                                }
                            }
                        }
                    }
                    Utils.Sleep(750, "attacking_enemy");
                }
                Utils.Sleep(290, "combo");
            }
        }


        public static void Chasing(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother || me == null)
            {
                return;
            }
            if (activChase)
            {
                var target = me.ClosestToMouseTarget(1500);
                var Spiderlings = ObjectMgr.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling).ToList();
                {
                    if (target != null && target.IsAlive && !target.IsIllusion)
                    {
                        foreach (var Spider in Spiderlings)
                            if (Spider.Distance2D(target) <= 1000 && Utils.SleepCheck(Spider.Handle.ToString()))
                            {
                                Spider.Attack(target);
                                Utils.Sleep(900, Spider.Handle.ToString());
                            }
                    }
                    else
                    {
                        foreach (var Spider in Spiderlings)
                            if (Utils.SleepCheck(Spider.Handle.ToString()))
                            {
                                Spider.Move(Game.MousePosition);
                                Utils.Sleep(900, Spider.Handle.ToString());
                            }
                    }

                }
            }
        }


        public static void ChasingAll(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;
            if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother || me == null)
            {
                return;
            }
            var target = me.ClosestToMouseTarget(1900);
            if (activCombo && target != null && target.IsAlive && !target.IsIllusion)
            {

                var Spiderlings = ObjectMgr.GetEntities<Unit>().Where(spiderlings => spiderlings.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling).ToList();

                    foreach (var Spider in Spiderlings)
                        {
                            if (Spider.Distance2D(target) <= 1500 && Utils.SleepCheck(Spider.Handle.ToString()))
                            {
                                Spider.Attack(target);
                                Utils.Sleep(900, Spider.Handle.ToString());
                            }
                        }
                    foreach (var Spider in Spiderlings)
                    {
                        if (Spider.Distance2D(target) >= 1500 && Utils.SleepCheck(Spider.Handle.ToString()))
                        {
                            Spider.Move(Game.MousePosition);
                            Utils.Sleep(900, Spider.Handle.ToString());
                        }
                    }


                    var linkens = target.Modifiers.Any(x => x.Name == "modifier_item_spheretarget") || target.Inventory.Items.Any(x => x.Name == "item_sphere");

                    var enemies = ObjectMgr.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != me.Team).ToList();
                    if (target != null && target.IsAlive && !target.IsIllusion && me.Distance2D(target) <= 1000)
                    {


                        if (Q == null)
                            Q = me.Spellbook.SpellQ;

                        if (W == null) ///////It will be added later//////////
                              W = me.Spellbook.SpellW; 

                        if (R == null)
                            R = me.Spellbook.SpellR;

                        // Item

                        if (sheep == null)
                            sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");


                        if (cheese == null)
                            cheese = me.FindItem("item_cheese");

                        if (orchid == null)
                            orchid = me.FindItem("item_orchid");

                        if (Soul == null)
                            Soul = me.FindItem("item_soul_ring");

                        if (shiva == null)
                            shiva = me.FindItem("item_shivas_guard");

                        if (dagon == null)
                            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

                        if (mom == null)
                            mom = me.FindItem("item_mask_of_madness");

                        if (abyssal == null)
                            abyssal = me.FindItem("item_abyssal_blade");

                        if (mjollnir == null)
                            mjollnir = me.FindItem("item_mjollnir");

                        if (halberd == null)
                            halberd = me.FindItem("item_heavens_halberd");

                        if (medall == null)
                            medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

                        if (satanic == null)
                            satanic = me.FindItem("item_satanic");

                        if ( // Q Skill
                               Q != null &&
                               Q.CanBeCasted() &&
                               me.CanCast() &&
                               !target.IsMagicImmune() &&
                               me.Distance2D(target) <= 600 &&
                               Utils.SleepCheck("Q")
                               )

                        {
                            Q.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "Q");
                        } // Q Skill end




                        if (//R Skill
                            R != null &&
                            R.CanBeCasted() &&
                            me.CanCast() &&
                            me.Distance2D(target) <= 350 &&
                            Utils.SleepCheck("R")
                            )
                        {
                            R.UseAbility();
                            Utils.Sleep(250 + Game.Ping, "R");
                        } // R Skill end


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
                            me.Distance2D(target) <= 600
                            )
                        {
                            sheep.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "sheep");
                        } // sheep Item end

                        if (// Soul Item 
                            Soul != null &&
                            me.Health / me.MaximumHealth <= 0.5 &&

                            me.Mana <= Q.ManaCost &&
                            Soul.CanBeCasted())
                        {
                            Soul.UseAbility();
                        } // Soul Item end

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

                        if ( // Medall
                            medall != null &&
                            medall.CanBeCasted() &&

                            Utils.SleepCheck("Medall") &&
                            me.Distance2D(target) <= 500
                            )
                        {
                            medall.UseAbility(target);
                            Utils.Sleep(250 + Game.Ping, "Medall");
                        } // Medall Item end

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
                            me.Distance2D(target) <= 600
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
                            Utils.Sleep(250 + Game.Ping, "dagon");
                        } // Dagon Item end


                        if (// Satanic 
                            satanic != null &&
                            me.Health / me.MaximumHealth <= 0.4 &&
                            satanic.CanBeCasted() &&
                            me.Distance2D(target) <= 300
                             &&
                            Utils.SleepCheck("Satanic")
                            )
                        {
                            satanic.UseAbility();
                            Utils.Sleep(250 + Game.Ping, "Satanic");
                        } // Satanic Item end

                        if    (//Attack
                              me.Distance2D(target) <= 1900 &&
                               Utils.SleepCheck("Attack")
                               )
                        {
                           me.Attack(target);
                            Utils.Sleep(300 + Game.Ping, "Attack");
                        } // Attack


					/***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/


					List<Unit> SpideWeb = ObjectMgr.GetEntities<Unit>().Where(web => web.ClassID == ClassID.CDOTA_Unit_Broodmother_Web).ToList();

					e = ObjectMgr.GetEntities<Hero>()
					.Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion) 
					.OrderBy(x => x.Position.Distance2D(SpideWeb.OrderBy(y => x.Position.Distance2D(y.Position)).FirstOrDefault().Position))
					.FirstOrDefault();

					if (e.Distance2D(SpideWeb.FirstOrDefault()) > 1100 && e != null && W != null && e.IsAlive && !e.IsIllusion)
					{
						if (e.Distance2D(SpideWeb.FirstOrDefault()) >= 1100 && me.Distance2D(e) <= 600 && Utils.SleepCheck(e.Handle.ToString()) && W.CanBeCasted())
						{
							W.UseAbility(e.Position);
							Utils.Sleep(4000, e.Handle.ToString());
						}
					}
					/***************************************WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW**********************************/
				}
			}
        }


        static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            var player = ObjectMgr.LocalPlayer;
            var me = ObjectMgr.LocalHero;
            if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Broodmother)
                return;

            if (!useQ)
            {
                DrawBox(2, 510, 110, 20, 1, new ColorBGRA(0, 0, 90, 90));
                DrawFilledBox(2, 510, 110, 20, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("  Q DISABLE", 4, 510, Color.Goldenrod, txt);
            }

			if (activChase || activCombo)
			{
				DrawBox(2, 510, 110, 20, 1, new ColorBGRA(0, 0, 90, 90));
				DrawFilledBox(2, 510, 110, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("  Brood#: Active!", 4, 510, Color.Goldenrod, txt);
			}

			if (toggle)
            {
                DrawBox(2, 530, 440, 54, 1, new ColorBGRA(0, 0, 90, 90));
                DrawFilledBox(2, 530, 440, 54, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("Brood menu#\n      LastHit/Denies[" + toggleLasthitKey + "]: " + toggleLasthit + " | UseQ [" + UseQ + "]: " + useQ + " |\n      [" + keyCOMBO + "] for COMBO | [" + keyCHASING + "] for CHASE ", 4, 530, Color.Gold, txt);
            }


            if (!toggle)
            {
                DrawBox(2, 530, 135, 20, 1, new ColorBGRA(0, 0, 90, 90));
                DrawFilledBox(2, 530, 135, 20, new ColorBGRA(0, 0, 0, 100));
                DrawShadowText("Open MENU---> [" + toggleKey + "]", 4, 530, Color.OrangeRed, txt);
            }
        }



        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Game.IsKeyDown(keyCHASING))
                    activChase = true;
                else
                {
                    activChase = false;
                }
                if (Game.IsKeyDown(keyCOMBO))
                    activCombo = true;
                else
                {
                    activCombo = false;
                }
                if (Game.IsKeyDown(toggleKey) && Utils.SleepCheck("toggle"))
                {
                    toggle = !toggle;
                    Utils.Sleep(200, "toggle");
                }

                if (Game.IsKeyDown(UseQ) && Utils.SleepCheck("useQ"))
                {
                    useQ = !useQ;
                    Utils.Sleep(150, "useQ");
                }

                if (Game.IsKeyDown(toggleLasthitKey) && Utils.SleepCheck("toggleLasthit"))
                {
                    toggleLasthit = !toggleLasthit;
                    Utils.Sleep(150, "toggleLasthit");
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


