namespace EmberSpirit
{
	using System;
	using System.Linq;
	using System.Collections.Generic;

	using Ensage;
	using SharpDX;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using Ensage.Common.Menu;
	internal class Program
	{
		private static readonly Menu Menu = new Menu("EmberSpirit", "EmberSpirit", true, "npc_dota_hero_ember_spirit", true);
		private static Ability Q, W, E, R, D;
		private static Hero me,e;
		private static Item urn, dagon, ghost, soul, atos, vail, sheep, cheese, stick, arcane,
			halberd, mjollnir, ethereal, orchid, abyssal, mom, Shiva, mail, bkb, satanic, medall, blink;

		private static bool wKey, Active,Push;
		private static bool oneULT;

		private static void OnLoadEvent(object sender, EventArgs args)
		{
			if (ObjectManager.LocalHero.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit) return;
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

			Menu.AddItem(new MenuItem("keyEscape", "Escape key").SetValue(new KeyBind('S', KeyBindType.Press)));
			Menu.AddItem(new MenuItem("wKey", "W key").SetValue(new KeyBind('W', KeyBindType.Press)));
			Menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"ember_spirit_activate_fire_remnant", true},
					{"ember_spirit_fire_remnant", true},
					{"ember_spirit_flame_guard", true},
					{"ember_spirit_searing_chains", true},
					{"ember_spirit_sleight_of_fist", true}
				})));
			Menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_ethereal_blade", true},
					{"item_blink", true},
					{"item_heavens_halberd", true},
					{"item_orchid", true}, {"item_bloodthorn", true},
					{"item_urn_of_shadows", true},
					{"item_veil_of_discord", true},
					{"item_abyssal_blade", true},
					{"item_shivas_guard", true},
					{"item_blade_mail", true},
					{"item_black_king_bar", true},
					{"item_medallion_of_courage", true},
					{"item_solar_crest", true}
				})));
			Menu.AddItem(
				new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
					{"item_mask_of_madness", true},
					{"item_cyclone", true},
					{"item_force_staff", true},
					{"item_sheepstick", true},
					{"item_cheese", true},
					{"item_ghost", true},
					{"item_rod_of_atos", true},
					{"item_soul_ring", true},
					{"item_arcane_boots", true},
					{"item_magic_stick", true},
					{"item_magic_wand", true},
					{"item_mjollnir", true},
					{"item_satanic", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("oneult", "Use One ult").SetValue(false));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
			Print.LogMessage.Success("War's flames blaze again!");
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

		private static void Game_OnUpdate(EventArgs args)
		{
			me = ObjectManager.LocalHero;


			if (!Game.IsInGame || Game.IsWatchingGame || me==null || me.ClassID != ClassID.CDOTA_Unit_Hero_EmberSpirit) return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Push = Game.IsKeyDown(Menu.Item("keyEscape").GetValue<KeyBind>().Key);
			wKey = Game.IsKeyDown(Menu.Item("wKey").GetValue<KeyBind>().Key);
			oneULT = Menu.Item("oneult").IsActive();
			if (!Menu.Item("enabled").IsActive())
				return;

			e = me.ClosestToMouseTarget(1800);
			if (Push)
			{
				Unit fount = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.IsValid && unit.IsAlive && unit.Team == me.Team && unit.Name == "npc_dota_ember_spirit_remnant").ToList();

				if (fount != null)
				{
					float angle = me.FindAngleBetween(fount.Position, true);
					Vector3 pos = new Vector3((float)(me.Position.X + R.GetCastRange() * Math.Cos(angle)),
						(float)(me.Position.Y + R.GetCastRange() * Math.Sin(angle)), 0);

					if (remnant.Count(x => x.Distance2D(me) <= 10000) == 0)
					{
						if (R != null && R.CanBeCasted()
							&& me.FindModifier("modifier_ember_spirit_fire_remnant_charge_counter").StackCount >= 1
							&& Utils.SleepCheck("z"))
						{
							R.UseAbility(pos);
							Utils.Sleep(1000, "z");
						}
					}
					if (remnant.Count(x => x.Distance2D(me) <= 10000) > 0)
					{
						if (D != null && D.CanBeCasted())
						{
							for (int i = 0; i < remnant.Count; ++i)
							{
								var kill =
									remnant[i].Modifiers.Where(y => y.Name == "modifier_ember_spirit_fire_remnant_thinker")
										.DefaultIfEmpty(null)
										.FirstOrDefault();

								if (kill != null
									&& kill.RemainingTime < 44
									&& Utils.SleepCheck("Rem"))
								{
									D.UseAbility(fount.Position);
									Utils.Sleep(300, "Rem");
								}
							}
						}
					}
				}
			}

			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

			D = me.Spellbook.SpellD;

			ethereal = me.FindItem("item_ethereal_blade");
			mom = me.FindItem("item_mask_of_madness");
			urn = me.FindItem("item_urn_of_shadows");
			dagon =
				me.Inventory.Items.FirstOrDefault(
					item =>
						item.Name.Contains("item_dagon"));
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			satanic = me.FindItem("item_satanic");
			blink = me.FindItem("item_blink");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			if (e == null) return;
			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
			vail = me.FindItem("item_veil_of_discord");
			cheese = me.FindItem("item_cheese");
			ghost = me.FindItem("item_ghost");
			atos = me.FindItem("item_rod_of_atos");
			soul = me.FindItem("item_soul_ring");
			arcane = me.FindItem("item_arcane_boots");
			stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
			Shiva = me.FindItem("item_shivas_guard");


			var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");



			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
					.ToList();



			if ((Active || wKey) && me.Distance2D(e) <= 1400 && e != null && e.IsAlive)
			{
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
			}

			if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && (!me.IsInvisible()|| me.IsVisibleToEnemies))
			{
				if (stoneModif) return;
				if ( // MOM
					mom != null
					&& mom.CanBeCasted()
					&& me.CanCast()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
					&& Utils.SleepCheck("mom")
					&& me.Distance2D(e) <= 700
					)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if ( // Hellbard
					halberd != null
					&& halberd.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& (e.NetworkActivity == NetworkActivity.Attack
						|| e.NetworkActivity == NetworkActivity.Crit
						|| e.NetworkActivity == NetworkActivity.Attack2)
					&& Utils.SleepCheck("halberd")
					&& me.Distance2D(e) <= 700
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
					)
				{
					halberd.UseAbility(e);
					Utils.Sleep(250, "halberd");
				}
				if ( //Ghost
					ghost != null
					&& ghost.CanBeCasted()
					&& me.CanCast()
					&& ((me.Position.Distance2D(e) < 300
						 && me.Health <= (me.MaximumHealth * 0.7))
						|| me.Health <= (me.MaximumHealth * 0.3))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
					&& Utils.SleepCheck("Ghost"))
				{
					ghost.UseAbility();
					Utils.Sleep(250, "Ghost");
				}
				if ( // Arcane Boots Item
					arcane != null
					&& me.Mana <= W.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
					&& arcane.CanBeCasted()
					&& Utils.SleepCheck("arcane")
					)
				{
					arcane.UseAbility();
					Utils.Sleep(250, "arcane");
				} // Arcane Boots Item end
				if ( // Mjollnir
					mjollnir != null
					&& mjollnir.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& me.Distance2D(e) <= 900
					)
				{
					mjollnir.UseAbility(me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end
				if (
					// cheese
					cheese != null
					&& cheese.CanBeCasted()
					&& me.Health <= (me.MaximumHealth * 0.3)
					&& me.Distance2D(e) <= 700
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
					&& Utils.SleepCheck("cheese")
					)
				{
					cheese.UseAbility();
					Utils.Sleep(200, "cheese");
				} // cheese Item end
				if ( // Medall
					medall != null
					&& medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
					&& me.Distance2D(e) <= 700
					)
				{
					medall.UseAbility(e);
					Utils.Sleep(250, "Medall");
				} // Medall Item end
				if (
					W != null && W.CanBeCasted() && me.Distance2D(e) <= 700
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
					&& Utils.SleepCheck("W")
					)
				{
					W.UseAbility(e.Predict(300));
					Utils.Sleep(200, "W");
				}
				if ( // Q Skill
					Q != null
					&& Q.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& me.Distance2D(e) <= 150 &&
					Utils.SleepCheck("Q")
					)

				{
					Q.UseAbility();
					Utils.Sleep(50, "Q");
				} // Q Skill end

				if ( // W Skill
					W != null
					&& W.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
					&& Utils.SleepCheck("W")
					)
				{
					W.UseAbility(e.Position);
					Utils.Sleep(200, "W");
				} // W Skill end
				if ( // E Skill
					E != null
					&& E.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Utils.SleepCheck("E")
					&& me.Distance2D(e) <= 400
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
					)
				{
					E.UseAbility();
					Utils.Sleep(350, "E");
				} // E Skill end
				if ( //R Skill
					R != null
					&& !oneULT
					&& R.CanBeCasted()
					&& me.CanCast()
					&& me.Distance2D(e) <= 1100
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility(e.Predict(700));
					Utils.Sleep(110, "R");
				} // R Skill end
				if ( //R Skill
					R != null
					&& oneULT
					&& R.CanBeCasted()
					&& me.CanCast()
					&& me.Distance2D(e) <= 1100
					&& Utils.SleepCheck("R")
					)
				{
					R.UseAbility(e.Predict(700));
					Utils.Sleep(5000, "R");
				} // R Skill end
				if ( // sheep
					sheep != null
					&& sheep.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& me.Distance2D(e) <= 1400
					&& !stoneModif
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
					&& Utils.SleepCheck("sheep")
					)
				{
					sheep.UseAbility(e);
					Utils.Sleep(250, "sheep");
				} // sheep Item end
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
					&& Utils.SleepCheck("abyssal")
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
					Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				if ( // ethereal
					ethereal != null
					&& ethereal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
					&& Utils.SleepCheck("ethereal")
					)
				{
					ethereal.UseAbility(e);
					Utils.Sleep(200, "ethereal");
				} // ethereal Item end
				if (
					blink != null
					&& me.CanCast()
					&& blink.CanBeCasted()
					&& me.Distance2D(e) >= 450
					&& me.Distance2D(e) <= 1150
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					blink.UseAbility(e.Position);
					Utils.Sleep(250, "blink");
				}

				if ( // SoulRing Item 
					soul != null
					&& soul.CanBeCasted()
					&& me.CanCast()
					&& me.Health >= (me.MaximumHealth * 0.5)
					&& me.Mana <= R.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(soul.Name)
					)
				{
					soul.UseAbility();
				} // SoulRing Item end
				if ( // Dagon
					me.CanCast()
					&& dagon != null
					&& (ethereal == null
						|| (e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
							|| ethereal.Cooldown < 17))
					&& !e.IsLinkensProtected()
					&& dagon.CanBeCasted()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if ( // atos Blade
					atos != null
					&& atos.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(atos.Name)
					&& me.Distance2D(e) <= 2000
					&& Utils.SleepCheck("atos")
					)
				{
					atos.UseAbility(e);

					Utils.Sleep(250, "atos");
				} // atos Item end
				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
				}
				if ( // vail
					vail != null
					&& vail.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
					&& me.Distance2D(e) <= 1500
					&& Utils.SleepCheck("vail")
					)
				{
					vail.UseAbility(e.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end
				  /*	if (
					  force != null
					  && force.CanBeCasted()
					  && me.Distance2D(e) < force.GetCastRange()
					  && Utils.SleepCheck(e.Handle.ToString()))
				  {
					  force.UseAbility(e);
					  Utils.Sleep(500, e.Handle.ToString());
				  }
			  */
				if (
					stick != null
					&& stick.CanBeCasted()
					&& stick.CurrentCharges != 0
					&& me.Distance2D(e) <= 700
					&& (me.Health <= (me.MaximumHealth * 0.5)
						|| me.Mana <= (me.MaximumMana * 0.5))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name))
				{
					stick.UseAbility();
					Utils.Sleep(200, "mana_items");
				}
				if ( // Satanic 
					satanic != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(e) <= me.AttackRange + 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				} // Satanic Item end
				if (mail != null && mail.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
				{
					mail.UseAbility();
					Utils.Sleep(100, "mail");
				}
				if (bkb != null && bkb.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
														 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
					Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
				{
					bkb.UseAbility();
					Utils.Sleep(100, "bkb");
				}

				var remnant = ObjectManager.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_ember_spirit_remnant").ToList();

				if (remnant.Count <= 0)
					return;
				for (int i = 0; i < remnant.Count; ++i)
				{
					if (//D Skill
					   remnant != null
					   && D.CanBeCasted()
					   && me.CanCast()
					   && remnant[i].Distance2D(e) <= 500
					   && Utils.SleepCheck("D")
					   )
					{
						D.UseAbility(e.Position);
						Utils.Sleep(400, "D");
					}
				}
			}
			if (wKey && me.Distance2D(e) <= 1400 && e != null && e.IsAlive)
			{
				if ( // Q Skill
					Q != null
					&& Q.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& me.Distance2D(e) <= 150 &&
					Utils.SleepCheck("Q")
					)

				{
					Q.UseAbility();
					Utils.Sleep(50, "Q");
				} // Q Skill end

				if ( // W Skill
					W != null
					&& W.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
					&& Utils.SleepCheck("W")
					)
				{
					W.UseAbility(e.Position);
					Utils.Sleep(200, "W");
				} // W Skill end
			}
			else if (wKey && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && (!me.IsInvisible() || me.IsVisibleToEnemies))
			{
				if ( // MOM
					mom != null
					&& mom.CanBeCasted()
					&& me.CanCast()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
					&& Utils.SleepCheck("mom")
					&& me.Distance2D(e) <= 700
					)
				{
					mom.UseAbility();
					Utils.Sleep(250, "mom");
				}
				if ( // Hellbard
					halberd != null
					&& halberd.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& (e.NetworkActivity == NetworkActivity.Attack
						|| e.NetworkActivity == NetworkActivity.Crit
						|| e.NetworkActivity == NetworkActivity.Attack2)
					&& Utils.SleepCheck("halberd")
					&& me.Distance2D(e) <= 700
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
					)
				{
					halberd.UseAbility(e);
					Utils.Sleep(250, "halberd");
				}
				if ( //Ghost
					ghost != null
					&& ghost.CanBeCasted()
					&& me.CanCast()
					&& ((me.Position.Distance2D(e) < 300
						 && me.Health <= (me.MaximumHealth * 0.7))
						|| me.Health <= (me.MaximumHealth * 0.3))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
					&& Utils.SleepCheck("Ghost"))
				{
					ghost.UseAbility();
					Utils.Sleep(250, "Ghost");
				}
				if ( // Arcane Boots Item
					arcane != null
					&& me.Mana <= W.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
					&& arcane.CanBeCasted()
					&& Utils.SleepCheck("arcane")
					)
				{
					arcane.UseAbility();
					Utils.Sleep(250, "arcane");
				} // Arcane Boots Item end
				if ( // Mjollnir
					mjollnir != null
					&& mjollnir.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
					&& Utils.SleepCheck("mjollnir")
					&& me.Distance2D(e) <= 900
					)
				{
					mjollnir.UseAbility(me);
					Utils.Sleep(250, "mjollnir");
				} // Mjollnir Item end
				if (
					// cheese
					cheese != null
					&& cheese.CanBeCasted()
					&& me.Health <= (me.MaximumHealth * 0.3)
					&& me.Distance2D(e) <= 700
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
					&& Utils.SleepCheck("cheese")
					)
				{
					cheese.UseAbility();
					Utils.Sleep(200, "cheese");
				} // cheese Item end
				if ( // Medall
					medall != null
					&& medall.CanBeCasted()
					&& Utils.SleepCheck("Medall")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
					&& me.Distance2D(e) <= 700
					)
				{
					medall.UseAbility(e);
					Utils.Sleep(250, "Medall");
				} // Medall Item end

				if ( // Q Skill
					Q != null
					&& Q.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& me.Distance2D(e) <= 150 &&
					Utils.SleepCheck("Q")
					)

				{
					Q.UseAbility();
					Utils.Sleep(50, "Q");
				} // Q Skill end

				if ( // W Skill
					W != null
					&& W.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
					&& Utils.SleepCheck("W")
					)
				{
					W.UseAbility(e.Position);
					Utils.Sleep(200, "W");
				} // W Skill end

				if ( // sheep
					sheep != null
					&& sheep.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& me.Distance2D(e) <= 1400
					&& !stoneModif
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
					&& Utils.SleepCheck("sheep")
					)
				{
					sheep.UseAbility(e);
					Utils.Sleep(250, "sheep");
				} // sheep Item end
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
					&& Utils.SleepCheck("abyssal")
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
					Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}
				if ( // ethereal
					ethereal != null
					&& ethereal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
					&& Utils.SleepCheck("ethereal")
					)
				{
					ethereal.UseAbility(e);
					Utils.Sleep(200, "ethereal");
				} // ethereal Item end
				if (
					blink != null
					&& me.CanCast()
					&& blink.CanBeCasted()
					&& me.Distance2D(e) >= 450
					&& me.Distance2D(e) <= 1150
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
					&& Utils.SleepCheck("blink")
					)
				{
					blink.UseAbility(e.Position);
					Utils.Sleep(250, "blink");
				}

				if ( // SoulRing Item 
					soul != null
					&& soul.CanBeCasted()
					&& me.CanCast()
					&& me.Health >= (me.MaximumHealth * 0.5)
					&& me.Mana <= R.ManaCost
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(soul.Name)
					)
				{
					soul.UseAbility();
				} // SoulRing Item end
				if ( // Dagon
					me.CanCast()
					&& dagon != null
					&& (ethereal == null
						|| (e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow")
							|| ethereal.Cooldown < 17))
					&& !e.IsLinkensProtected()
					&& dagon.CanBeCasted()
					&& !e.IsMagicImmune()
					&& !stoneModif
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if ( // atos Blade
					atos != null
					&& atos.CanBeCasted()
					&& me.CanCast()
					&& !e.IsLinkensProtected()
					&& !e.IsMagicImmune()
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(atos.Name)
					&& me.Distance2D(e) <= 2000
					&& Utils.SleepCheck("atos")
					)
				{
					atos.UseAbility(e);

					Utils.Sleep(250, "atos");
				} // atos Item end
				if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
				}
				if ( // vail
					vail != null
					&& vail.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
					&& me.Distance2D(e) <= 1500
					&& Utils.SleepCheck("vail")
					)
				{
					vail.UseAbility(e.Position);
					Utils.Sleep(250, "vail");
				} // orchid Item end

				if (
					stick != null
					&& stick.CanBeCasted()
					&& stick.CurrentCharges != 0
					&& me.Distance2D(e) <= 700
					&& (me.Health <= (me.MaximumHealth * 0.5)
						|| me.Mana <= (me.MaximumMana * 0.5))
					&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name))
				{
					stick.UseAbility();
					Utils.Sleep(200, "mana_items");
				}
				if ( // Satanic 
					satanic != null &&
					me.Health <= (me.MaximumHealth * 0.3) &&
					satanic.CanBeCasted() &&
					me.Distance2D(e) <= me.AttackRange + 50
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
					&& Utils.SleepCheck("satanic")
					)
				{
					satanic.UseAbility();
					Utils.Sleep(240, "satanic");
				}
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
