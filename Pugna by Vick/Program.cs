namespace Pugna
{
	using System;
	using System.Linq;

	using Ensage;
	using SharpDX;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using Ensage.Common.Menu;
	using SharpDX.Direct3D9;
	using System.Windows.Input;
	using System.Collections.Generic;
	internal class Program
	{
        private static readonly Menu Menu = new Menu("Pugna", "Pugna", true, "npc_dota_hero_pugna", true);
        private static bool Active;
	    private static Hero me, e;
		private static Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
		private static Ability Q, W, E, R;


		private static void OnLoadEvent(object sender, EventArgs args)
		{
			if (ObjectManager.LocalHero.ClassID != ClassID.CDOTA_Unit_Hero_Pugna) return;
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("agh", "Use items if i have Aghanim and I use Life Drain").SetValue(true));
			Menu.AddItem(
			   new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			   {
					{ "pugna_decrepify", true},
					{ "pugna_life_drain", true},
					{ "pugna_nether_ward", true},
					{ "pugna_nether_blast", true}
			   })));
			Menu.AddItem(new MenuItem("Heel", "Min targets to NetherWard").SetValue(new Slider(2, 1, 5)));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
			Print.LogMessage.Success("I'll take all you have and more.");
		}

		private static void OnCloseEvent(object sender, EventArgs args)
		{
			Game.OnUpdate -= Game_OnUpdate;
			Menu.RemoveFromMainMenu();
		}

		private static void Main()
		{
			Events.OnLoad += OnLoadEvent;
			Events.OnClose += OnCloseEvent;
		}

		public static void Game_OnUpdate(EventArgs args)
		{
			me = ObjectManager.LocalHero;

			if (!Game.IsInGame || Game.IsWatchingGame || me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Pugna) return;
            if (!Menu.Item("enabled").IsActive())
                return;
            e = me.ClosestToMouseTarget(2000);
			if (e == null) return;

			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

			// Item
			ethereal = me.FindItem("item_ethereal_blade");

			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

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
			//var ModifRod = e.Modifiers.Any(y => y.Name == "modifier_rod_of_atos_debuff");
		    var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

            if (Active && me.IsAlive && e.IsAlive && Utils.SleepCheck("Active"))
			{
				var noBlade = e.Modifiers.Any(y => y.Name == "modifier_item_blade_mail_reflect");

                if (stoneModif) return;
                if (R.IsInAbilityPhase || me.Modifiers.Any(y => y.Name == "modifier_pugna_life_drain")) return;
				if (e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
				{
					if ((!me.IsChanneling() && !me.AghanimState()) || (me.AghanimState() && Menu.Item("agh").IsActive()))
					{
                        if ( // atos Blade
                            atos != null
                            && atos.CanBeCasted()
                            && me.CanCast()
                            && !e.IsLinkensProtected()
                            && !e.IsMagicImmune()
                            && Utils.SleepCheck("atos")
                            && me.Distance2D(e) <= 2000
                            )
                        {
                            atos.UseAbility(e);
                            Utils.Sleep(250, "atos");
                        } // atos Item end
                        float angle = me.FindAngleBetween(e.Position, true);
                        Vector3 pos = new Vector3((float)(e.Position.X - 350 * Math.Cos(angle)), (float)(e.Position.Y - 350 * Math.Sin(angle)), 0);
                        if (
                            blink != null
                            && Q.CanBeCasted()
                            && me.CanCast()
                            && blink.CanBeCasted()
                            && me.Distance2D(e) >= 590
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
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                            && me.Distance2D(e) < 1400
							&& Utils.SleepCheck("W")
							)
						{
							W.UseAbility(e);
							Utils.Sleep(200, "W");
						}
						
						if (!W.CanBeCasted() || W == null || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
						{
                            if (
								Q != null
								&& Q.CanBeCasted()
								&& me.CanCast()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                && me.Distance2D(e) < 1400
								&& Utils.SleepCheck("Q")
								)
							{
								Q.UseAbility(e.Position);
								Utils.Sleep(200, "Q");
							}
							if ( // orchid
								orchid != null
								&& orchid.CanBeCasted()
								&& me.CanCast()
								&& !e.IsLinkensProtected()
								&& !e.IsMagicImmune()
								&& Utils.SleepCheck("orchid")
								&& me.Distance2D(e) <= 1400
								
								)
							{
								orchid.UseAbility(e);
								Utils.Sleep(250 , "orchid");
							} // orchid Item end
							if (!orchid.CanBeCasted() || orchid == null)
							{
								if ( // vail
								    vail != null
								    && vail.CanBeCasted()
								    && me.CanCast()
								    && e.Modifiers.Any(y => y.Name != "modifier_item_veil_of_discord_debuff")
								    && !e.IsMagicImmune()
								    && Utils.SleepCheck("vail")
								    && me.Distance2D(e) <= 1500
								    )
								{
									vail.UseAbility(e.Position);
									Utils.Sleep(250, "vail");
								} // orchid Item end
								if (!vail.CanBeCasted() || vail == null)
								{
									
									if (// ethereal
										ethereal != null
										&& ethereal.CanBeCasted()
										&& me.CanCast()
										&& !e.IsLinkensProtected()
										&& !e.IsMagicImmune()
										&& Utils.SleepCheck("ethereal")
									    )
									{
										ethereal.UseAbility(e);
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
											&& ((me.Position.Distance2D(e) < 300
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
											&& !e.IsMagicImmune()
											&& Utils.SleepCheck("shiva")
											&& me.Distance2D(e) <= 600
											)

										{
											shiva.UseAbility();
											Utils.Sleep(250 , "shiva");
										} // Shiva Item end
                                        

										if ( // sheep
											sheep != null
											&& sheep.CanBeCasted()
											&& me.CanCast()
											&& !e.IsLinkensProtected()
											&& !e.IsMagicImmune()
											&& Utils.SleepCheck("sheep")
											&& me.Distance2D(e) <= 1400
											
											)
										{
											sheep.UseAbility(e);
											Utils.Sleep(250 , "sheep");
										} // sheep Item end

										if (// Dagon
											me.CanCast()
											&& dagon != null
											&& (ethereal == null
											|| (e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
											|| ethereal.Cooldown < 17))
											&& !e.IsLinkensProtected()
											&& dagon.CanBeCasted()
											&& !e.IsMagicImmune()
											
											&& Utils.SleepCheck("dagon")
										   )
										{
											dagon.UseAbility(e);
											Utils.Sleep(200, "dagon");
										} // Dagon Item end

										if (
											 // cheese
											 cheese != null
											 && cheese.CanBeCasted()
											 && Utils.SleepCheck("cheese")
											 && me.Health <= (me.MaximumHealth * 0.3)
											 && me.Distance2D(e) <= 700)
										{
											cheese.UseAbility();
											Utils.Sleep(200 , "cheese");
										} // cheese Item end
                                        var v = ObjectManager.GetEntities<Hero>()
                                            .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && x.Distance2D(me) <= 1200).ToList();
                                        if (E != null && E.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 1200) >=
                                                                                                 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
                                                            Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name) && Utils.SleepCheck("E"))
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
                            && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                            && !me.IsChanneling()
							&& me.Modifiers.All(y => y.Name != "modifier_pugna_life_drain")
							&& (!Q.CanBeCasted() || Q == null || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
							&& (!W.CanBeCasted() || W == null || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
							&& (!atos.CanBeCasted() || atos == null)
							&& (!orchid.CanBeCasted() || orchid == null)
							&& (!sheep.CanBeCasted() || sheep == null)
							&& (!dagon.CanBeCasted() || dagon == null)
							&& (!ethereal.CanBeCasted() || ethereal == null)
							&& (!cheese.CanBeCasted() || cheese == null)
							&& me.Position.Distance2D(e) < 1000
							)
							&& Utils.SleepCheck("R"))
						{
							R.UseAbility(e);
							Utils.Sleep(200, "R");
						}
					}
				}
				Utils.Sleep(50, "Active");
			}
		}
		
    }
   internal class Print
    {
        public class LogMessage
        {
            public static void Success(string text, params object[] arguments)
            {
                Game.PrintMessage("<font color='#e0007b'>" + text + "</font>", MessageType.LogMessage);
            }
        } // Console class
    }
}


