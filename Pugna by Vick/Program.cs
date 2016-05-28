using System;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using  Ensage.Common.Menu;
using SharpDX.Direct3D9;
using System.Windows.Input;
using System.Collections.Generic;
namespace Pugna
{
	internal class Program
	{
        private static readonly Menu menu = new Menu("Pugna", "Pugna", true, "npc_dota_hero_pugna", true);
        private static bool Active;
	    private static Hero me, target;
		private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
		private static Ability Q, W, E, R;
		private static Font txt;
		private static Font noti;
		private static Line lines;


		static void Main(string[] args)
		{

			Game.OnUpdate += Game_OnUpdate;
            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
            menu.AddItem(new MenuItem("agh", "Use items if i have Aghanim and I use Life Drain").SetValue(true));
            menu.AddItem(
               new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
               {
                    { "pugna_decrepify", true},
                    { "pugna_life_drain", true},
                    { "pugna_nether_ward", true},
                    { "pugna_nether_blast", true}
               })));
            menu.AddItem(new MenuItem("Heel", "Min targets to NetherWard").SetValue(new Slider(2, 1, 5)));
            menu.AddToMainMenu();
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

            Print.LogMessage.Success("|Pugna|");
            Print.ConsoleMessage.Success("|Pugna|");
            Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

		public static void Game_OnUpdate(EventArgs args)
		{
			me = ObjectManager.LocalHero;

			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Pugna || me == null) return;
            if (!menu.Item("enabled").IsActive())
                return;
            target = me.ClosestToMouseTarget(2000);
			if (target == null) return;

			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

			// Item
			ethereal = me.FindItem("item_ethereal_blade");

			sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

			vail = me.FindItem("item_veil_of_discord");

			cheese = me.FindItem("item_cheese");

			ghost = me.FindItem("item_ghost");

			orchid = me.FindItem("item_orchid");

			atos = me.FindItem("item_rod_of_atos");

			soulring = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");

			blink = me.FindItem("item_blink");

			shiva = me.FindItem("item_shivas_guard");

			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
			//var ModifRod = target.Modifiers.Any(y => y.Name == "modifier_rod_of_atos_debuff");
		    var stoneModif = target.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);

            if (Active && me.IsAlive && target.IsAlive && Utils.SleepCheck("Active"))
			{
				var noBlade = target.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");

                if (stoneModif) return;
                if (R.IsInAbilityPhase || me.Modifiers.Any(y => y.Name == "modifier_pugna_life_drain")) return;
				if (target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
				{
					if ((!me.IsChanneling() && !me.AghanimState()) || (me.AghanimState() && menu.Item("agh").IsActive()))
					{
                        if ( // atos Blade
                            atos != null
                            && atos.CanBeCasted()
                            && me.CanCast()
                            && !target.IsLinkensProtected()
                            && !target.IsMagicImmune()
                            && Utils.SleepCheck("atos")
                            && me.Distance2D(target) <= 2000
                            )
                        {
                            atos.UseAbility(target);
                            Utils.Sleep(250, "atos");
                        } // atos Item end
                        float angle = me.FindAngleBetween(target.Position, true);
                        Vector3 pos = new Vector3((float)(target.Position.X - 350 * Math.Cos(angle)), (float)(target.Position.Y - 350 * Math.Sin(angle)), 0);
                        if (
                            blink != null
                            && Q.CanBeCasted()
                            && me.CanCast()
                            && blink.CanBeCasted()
                            && me.Distance2D(target) >= 590
                            && me.Distance2D(pos) <= 1190
                            && Utils.SleepCheck("blink")
                            )
                        {
                            blink.UseAbility(pos);
                            Utils.Sleep(250, "blink");
                        }
                        if (
							W != null
							&& W.CanBeCasted()
                            && (Q.CanBeCasted()
                            || R.CanBeCasted())
							&& me.CanCast()
                            && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                            && me.Distance2D(target) < 1400
							&& Utils.SleepCheck("W")
							)
						{
							W.UseAbility(target);
							Utils.Sleep(200, "W");
						}
						
						if (!W.CanBeCasted() || W == null || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						{
                            if (
								Q != null
								&& Q.CanBeCasted()
								&& me.CanCast()
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                && me.Distance2D(target) < 1400
								&& Utils.SleepCheck("Q")
								)
							{
								Q.UseAbility(target.Position);
								Utils.Sleep(200, "Q");
							}
							if ( // orchid
								orchid != null
								&& orchid.CanBeCasted()
								&& me.CanCast()
								&& !target.IsLinkensProtected()
								&& !target.IsMagicImmune()
								&& Utils.SleepCheck("orchid")
								&& me.Distance2D(target) <= 1400
								
								)
							{
								orchid.UseAbility(target);
								Utils.Sleep(250 , "orchid");
							} // orchid Item end
							if (!orchid.CanBeCasted() || orchid == null)
							{
								if ( // vail
								    vail != null
								    && vail.CanBeCasted()
								    && me.CanCast()
								    && target.Modifiers.Any(y => y.Name != "modifier_item_veil_of_discord_debuff")
								    && !target.IsMagicImmune()
								    && Utils.SleepCheck("vail")
								    && me.Distance2D(target) <= 1500
								    )
								{
									vail.UseAbility(target.Position);
									Utils.Sleep(250, "vail");
								} // orchid Item end
								if (!vail.CanBeCasted() || vail == null)
								{
									
									if (// ethereal
										ethereal != null
										&& ethereal.CanBeCasted()
										&& me.CanCast()
										&& !target.IsLinkensProtected()
										&& !target.IsMagicImmune()
										&& Utils.SleepCheck("ethereal")
									    )
									{
										ethereal.UseAbility(target);
										Utils.Sleep(200, "ethereal");
									} // ethereal Item end
									if (!ethereal.CanBeCasted() || ethereal == null)
									{
										if (// SoulRing Item 
											soulring != null
											&& soulring.CanBeCasted()
											&& me.CanCast()
                                            && me.Health >= (me.MaximumHealth * 0.3)
                                            && me.Mana <= R.ManaCost
											)
										{
											soulring.UseAbility();
										} // SoulRing Item end

										if (// Arcane Boots Item
											arcane != null
											&& arcane.CanBeCasted()
											&& me.CanCast()
											&& me.Mana <= R.ManaCost
											)
										{
											arcane.UseAbility();
										} // Arcane Boots Item end

										if (//Ghost
											ghost != null
											&& ghost.CanBeCasted()
											&& me.CanCast()
											&& ((me.Position.Distance2D(target) < 300
											&& me.Health <= (me.MaximumHealth * 0.7))
											|| me.Health <= (me.MaximumHealth * 0.3))
											&& Utils.SleepCheck("Ghost"))
										{
											ghost.UseAbility();
											Utils.Sleep(250, "Ghost");
										}


										if (// Shiva Item
											shiva != null
											&& shiva.CanBeCasted()
											&& me.CanCast()
											&& !target.IsMagicImmune()
											&& Utils.SleepCheck("shiva")
											&& me.Distance2D(target) <= 600
											)

										{
											shiva.UseAbility();
											Utils.Sleep(250 , "shiva");
										} // Shiva Item end
                                        

										if ( // sheep
											sheep != null
											&& sheep.CanBeCasted()
											&& me.CanCast()
											&& !target.IsLinkensProtected()
											&& !target.IsMagicImmune()
											&& Utils.SleepCheck("sheep")
											&& me.Distance2D(target) <= 1400
											
											)
										{
											sheep.UseAbility(target);
											Utils.Sleep(250 , "sheep");
										} // sheep Item end

										if (// Dagon
											me.CanCast()
											&& dagon != null
											&& (ethereal == null
											|| (target.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
											|| ethereal.Cooldown < 17))
											&& !target.IsLinkensProtected()
											&& dagon.CanBeCasted()
											&& !target.IsMagicImmune()
											
											&& Utils.SleepCheck("dagon")
										   )
										{
											dagon.UseAbility(target);
											Utils.Sleep(200, "dagon");
										} // Dagon Item end

										if (
											 // cheese
											 cheese != null
											 && cheese.CanBeCasted()
											 && Utils.SleepCheck("cheese")
											 && me.Health <= (me.MaximumHealth * 0.3)
											 && me.Distance2D(target) <= 700)
										{
											cheese.UseAbility();
											Utils.Sleep(200 , "cheese");
										} // cheese Item end
                                        var v = ObjectManager.GetEntities<Hero>()
                                            .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && x.Distance2D(me) <= 1200).ToList();
                                        if (E != null && E.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 1200) >=
                                                                                                 (menu.Item("Heel").GetValue<Slider>().Value)) &&
                                                            menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name) && Utils.SleepCheck("E"))
                                        {
                                            E.UseAbility(Prediction.InFront(me, 70));
                                            Utils.Sleep(100, "E");
                                        }
                                    }
								}
							}
						}
                        if (
							(R != null
							&& R.CanBeCasted()
                            && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                            && !me.IsChanneling()
							&& me.Modifiers.All(y => y.Name != "modifier_pugna_life_drain")
							&& (!Q.CanBeCasted() || Q == null || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
							&& (!W.CanBeCasted() || W == null || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
							&& (!atos.CanBeCasted() || atos == null)
							&& (!orchid.CanBeCasted() || orchid == null)
							&& (!sheep.CanBeCasted() || sheep == null)
							&& (!dagon.CanBeCasted() || dagon == null)
							&& (!ethereal.CanBeCasted() || ethereal == null)
							&& (!cheese.CanBeCasted() || cheese == null)
							&& me.Position.Distance2D(target) < 1000
							)
							&& Utils.SleepCheck("R"))
						{
							R.UseAbility(target);
							Utils.Sleep(200, "R");
						}
					}
				}
				Utils.Sleep(50, "Active");
			}
		}
		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			var me = ObjectManager.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Pugna)
				return;

			if (Active)
			{
				DrawBox(2, 510, 130, 20, 1, new ColorBGRA(0, 0, 100, 100));
				DrawFilledBox(2, 510, 130, 20, new ColorBGRA(0, 0, 0, 100));
				DrawShadowText("Pugna#: Comboing!", 4, 510, Color.DeepPink, txt);
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
    class Print
    {
        public class LogMessage
        {
            public static void Success(string text, params object[] arguments)
            {
                Game.PrintMessage("<font color='#e0007b'>" + text + "</font>", MessageType.LogMessage);
            }
        } // Console class

        public class ConsoleMessage
        {
            public static void Encolored(string text, ConsoleColor color, params object[] arguments)
            {
                var clr = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(text, arguments);
                Console.ForegroundColor = clr;
            }
            public static void Success(string text, params object[] arguments)
            {
                Encolored(text, ConsoleColor.Magenta, arguments);
            }
        } // LogMessage class
    }
}


