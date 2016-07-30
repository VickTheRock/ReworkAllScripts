namespace Terrorblade
{//ONLY LOVE MazaiPC ;) 
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Ensage;
	using Ensage.Common.Extensions;
	using Ensage.Common;
	using Ensage.Common.Menu;

	internal class Program
	{
		private static bool Active;
		private static Item soulring, arcane, blink, shiva, dagon, mjollnir, mom, halberd, abyssal, ethereal, cheese, satanic, medall, mail, orchid, bkb, phase, sheep, manta;
		private static Ability Q, W, E, R;
		private static Hero me, e;

		private static readonly Menu Menu = new Menu("Terrorblade", "Terrorblade", true, "npc_dota_hero_terrorblade", true);
		private static readonly Menu skills = new Menu("Skills", "Skills");
		private static readonly Menu items = new Menu("Items", "Items");
		private static void OnLoadEvent(object sender, EventArgs args)
		{
			if (ObjectManager.LocalHero.ClassID != ClassID.CDOTA_Unit_Hero_Terrorblade) return;

			me = ObjectManager.LocalHero;
			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Key Combo").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			skills.AddItem(new MenuItem("Skills", "Skills:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{   {"terrorblade_reflection",true},
				{"terrorblade_conjure_image",true},
				{"terrorblade_metamorphosis",true},
				{"terrorblade_sunder",true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_phase_boots", true},
				{"item_cheese", true},
				{"item_blade_mail",true},
				{"item_black_king_bar",true},
				{"item_mjollnir",true},
				{"item_satanic", true},
				{"item_heavens_halberd",true},
				{"item_sheepstick",true},
				{"item_manta", true}
			})));
			items.AddItem(new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_mask_of_madness", true},
				{"item_dagon",true},
				{"item_orchid",true},
				{"item_bloodthorn", true},
				{"item_ethereal_blade",true},
				{"item_shivas_guard",true},
				{"item_abyssal_blade", true},
				{"item_medallion_of_courage", true},
				{"item_solar_crest", true}
			})));

			Menu.AddItem(new MenuItem("ult", "Auto Ult").SetValue(true));

			Menu.AddItem(new MenuItem("ultEnem", "Use Ult in Enemy").SetValue(true));

			Menu.AddItem(new MenuItem("heal", "Min target Healt % to Ult").SetValue(new Slider(50, 1)));

			Menu.AddItem(new MenuItem("healme", "Min me Healt % to Ult").SetValue(new Slider(30, 1)));
			Menu.AddItem(new MenuItem("ultIllu", "Use Ult in Illusion if is no suitable enemy").SetValue(true));
			Menu.AddItem(new MenuItem("ultAlly", "Use Ult in Ally if is no suitable enemy and Illusion").SetValue(false));
			Menu.AddItem(new MenuItem("Heel", "Min targets to BKB|BladeMail").SetValue(new Slider(2, 1, 5)));
			Menu.AddToMainMenu();
			Game.OnUpdate += Game_OnUpdate;
			LogMessage.Success("I am sworn to turn the tide where ere I can.");
		}
		private static void OnCloseEvent(object sender, EventArgs args)
		{
			Game.OnUpdate -= Game_OnUpdate;
			Menu.RemoveFromMainMenu();
		} // OnClose

		static void Main()
		{
			Events.OnLoad += OnLoadEvent;
			Events.OnClose += OnCloseEvent;
		}

		private static void Game_OnUpdate(EventArgs args)
		{
			if (!Game.IsInGame || me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Terrorblade || Game.IsWatchingGame) return;

			if (!Menu.Item("enabled").IsActive())
				return;
			//spell
			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			E = me.Spellbook.SpellE;
			R = me.Spellbook.SpellR;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);
			var s = ObjectManager.GetEntities<Hero>()
		   .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion && x.Distance2D(me) <= 1500).ToList();
			if (Active)
			{
				e = me.ClosestToMouseTarget(2000);
				if (e == null) return;
				if (e.IsAlive && !e.IsInvul())
				{
					if (me.IsAlive && me.Distance2D(e) <= 1800)
					{
					
						// item 
						satanic = me.FindItem("item_satanic");
						shiva = me.FindItem("item_shivas_guard");
						dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
						arcane = me.FindItem("item_arcane_boots");
						mom = me.FindItem("item_mask_of_madness");
						medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
						ethereal = me.FindItem("item_ethereal_blade");
						blink = me.FindItem("item_blink");
						soulring = me.FindItem("item_soul_ring");
						cheese = me.FindItem("item_cheese");
						halberd = me.FindItem("item_heavens_halberd");
						abyssal = me.FindItem("item_abyssal_blade");
						mjollnir = me.FindItem("item_mjollnir");
						manta = me.FindItem("item_manta");
						mail = me.FindItem("item_blade_mail");
						orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
						bkb = me.FindItem("item_black_king_bar");
						phase = me.FindItem("item_phase_boots");
						sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
						var linkens = e.IsLinkensProtected();


						if (
							(R.CanBeCasted()
							 || me.Health >= (me.MaximumHealth * 0.4))
							&& blink.CanBeCasted()
							&& me.Position.Distance2D(e) > 300
							&& me.Position.Distance2D(e) < 1180
							&& Utils.SleepCheck("blink"))
						{
							blink.UseAbility(e.Position);
							Utils.Sleep(250, "blink");
						}
						if ( // sheep
							sheep != null
							&& sheep.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& me.Distance2D(e) <= 1400
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
							&& Utils.SleepCheck("sheep")
							)
						{
							sheep.UseAbility(e);
							Utils.Sleep(250, "sheep");
						} // sheep Item end
						  //var ModifEther = e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
						var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
						if (phase != null
							&& phase.CanBeCasted()
							&& Utils.SleepCheck("phase")
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(phase.Name)
							&& !blink.CanBeCasted()
							&& me.Distance2D(e) >= me.GetAttackRange() + 20)
						{
							phase.UseAbility();
							Utils.Sleep(200, "phase");
						}
						if ( // Dagon
							dagon != null
							&& (e.Health <= (e.MaximumHealth * 0.4)
								|| !R.CanBeCasted())
							&& dagon.CanBeCasted()
							&& me.CanCast()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled("item_dagon")
							&& !e.IsMagicImmune() &&
							Utils.SleepCheck("dagon"))
						{
							dagon.UseAbility(e);
							Utils.Sleep(150, "dagon");
						} // Dagon Item end
						if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)) &&
							manta.CanBeCasted() && me.IsSilenced() && Utils.SleepCheck("manta"))
						{
							manta.UseAbility();
							Utils.Sleep(400, "manta");
						}
						if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)) &&
							manta.CanBeCasted() && (e.Position.Distance2D(me.Position) <= me.GetAttackRange() + me.HullRadius) &&
							Utils.SleepCheck("manta"))
						{
							manta.UseAbility();
							Utils.Sleep(150, "manta");
						}
						if (
							Q.CanBeCasted()
							&& ((dagon != null && !dagon.CanBeCasted())
								|| (R.CanBeCasted()
									&& me.Health >= (me.MaximumHealth * 0.3))
								|| (!R.CanBeCasted() && me.Health <= (me.MaximumHealth * 0.3)))
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& me.Position.Distance2D(e) < Q.GetCastRange() &&
							Utils.SleepCheck("Q"))
						{
							Q.UseAbility();
							Utils.Sleep(150, "Q");
						}
						if (W.CanBeCasted()
							&& !Q.CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& me.Position.Distance2D(e) < me.GetAttackRange()
							&& Utils.SleepCheck("W"))
						{
							W.UseAbility();
							Utils.Sleep(150, "W");
						}
						if (E.CanBeCasted()
							&& !Q.CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
							&& me.Position.Distance2D(e) < me.GetAttackRange()
							&& Utils.SleepCheck("E"))
						{
							E.UseAbility();
							Utils.Sleep(150, "E");
						}
						if ( // orchid
							orchid != null
							&& orchid.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& me.Distance2D(e) <= me.GetAttackRange()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
							&& !stoneModif
							&& Utils.SleepCheck("orchid")
							)
						{
							orchid.UseAbility(e);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (ethereal != null
							&& ethereal.CanBeCasted()
							&& me.Distance2D(e) <= 700
							&& me.Distance2D(e) <= 400
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
							&& Utils.SleepCheck("ethereal"))
						{
							ethereal.UseAbility(e);
							Utils.Sleep(100, "ethereal");
						}
						var q = (Q != null && Q.Cooldown <= 0 && Q.ManaCost < me.Mana);
						var w = (W != null && W.Cooldown <= 0 && W.ManaCost < me.Mana);
						var _e = (E != null && E.Cooldown <= 0 && E.ManaCost < me.Mana);
						var r = (R != null && R.Cooldown <= 0 && R.ManaCost < me.Mana && R.Level <= 2);
						var d = (dagon != null && dagon.Cooldown <= 0 && dagon.ManaCost < me.Mana);


						if ( // SoulRing Item 
							soulring != null &&
							me.Health >= (me.MaximumHealth * 0.3)
							&& (q || w || _e || d)
							&& soulring.CanBeCasted()
							&& Utils.SleepCheck("soulring"))
						{
							soulring.UseAbility();
							Utils.Sleep(150, "soulring");
						} // SoulRing Item end

						if ( // Arcane Boots Item
							arcane != null
							&& (q || w || _e || d)
							&& arcane.CanBeCasted()
							&& Utils.SleepCheck("arcane"))
						{
							arcane.UseAbility();
							Utils.Sleep(150, "arcane");
						} // Arcane Boots Item end

						if ( // Shiva Item
							shiva != null
							&& shiva.CanBeCasted()
							&& me.CanCast()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
							&& !e.IsMagicImmune()
							&& (shiva.CanBeCasted()
								&& Utils.SleepCheck("shiva")
								&& me.Distance2D(e) <= 600)
							)
						{
							shiva.UseAbility();
							Utils.Sleep(250, "shiva");
						} // Shiva Item end
						if (mail != null && mail.CanBeCasted() && (s.Count(x => x.Distance2D(me) <= 650) >=
																   (Menu.Item("Heelm").GetValue<Slider>().Value)) &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
						{
							mail.UseAbility();
							Utils.Sleep(100, "mail");
						}
						if ( // Medall
							medall != null
							&& medall.CanBeCasted()
							&& me.CanCast()
							&& !e.IsMagicImmune()
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(medall.Name)
							&& Utils.SleepCheck("Medall")
							&& me.Distance2D(e) <= me.GetAttackRange() + me.HullRadius
							)
						{
							medall.UseAbility(e);
							Utils.Sleep(250, "Medall");
						} // Medall Item end

						if ( // MOM
							mom != null
							&& mom.CanBeCasted()
							&& me.CanCast()
							&& Utils.SleepCheck("mom")
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mom.Name)
							&& me.Distance2D(e) <= me.GetAttackRange() + me.HullRadius
							)
						{
							mom.UseAbility();
							Utils.Sleep(250, "mom");
						} // MOM Item end


						if ( // Abyssal Blade
							abyssal != null
							&& abyssal.CanBeCasted()
							&& me.CanCast()
							&& Utils.SleepCheck("abyssal")
							&& Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
							&& me.Distance2D(e) <= 400
							)
						{
							abyssal.UseAbility(e);
							Utils.Sleep(250, "abyssal");
						} // Abyssal Item end

						if ( // Hellbard
							halberd != null
							&& halberd.CanBeCasted()
							&& me.CanCast()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
							&& !e.IsMagicImmune()
							&& Utils.SleepCheck("halberd")
							&& me.Distance2D(e) <= 700
							)
						{
							halberd.UseAbility(e);
							Utils.Sleep(250, "halberd");
						} // Hellbard Item end

						if ( // Mjollnir
							mjollnir != null
							&& mjollnir.CanBeCasted()
							&& me.CanCast()
							&& !e.IsMagicImmune()
							&& Utils.SleepCheck("mjollnir")
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
							&& me.Distance2D(e) <= 900
							)
						{
							mjollnir.UseAbility(me);
							Utils.Sleep(250, "mjollnir");
						} // Mjollnir Item end

						if ( // Satanic 
							satanic != null
							&& me.Health <= (me.MaximumHealth * 0.4)
							&& (!R.CanBeCasted() || me.IsSilenced()
								|| e.Health <= (e.MaximumHealth * 0.4))
							&& satanic.CanBeCasted()
							&& me.Distance2D(e) <= me.GetAttackRange() + me.HullRadius
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
							&& Utils.SleepCheck("Satanic")
							)
						{
							satanic.UseAbility();
							Utils.Sleep(350, "Satanic");
						} // Satanic Item end
						if ( // cheese
							cheese != null
							&& cheese.CanBeCasted()
							&& me.Health <= (me.MaximumHealth * 0.3)
							&& (!R.CanBeCasted()
								|| !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
							&& me.Distance2D(e) <= me.GetAttackRange() + me.HullRadius
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
							&& Utils.SleepCheck("cheese")
							)
						{
							cheese.UseAbility();
							Utils.Sleep(200, "cheese");
						} // cheese Item end
						if (bkb != null && bkb.CanBeCasted() && (s.Count(x => x.Distance2D(me) <= 650) >=
																 (Menu.Item("Heel").GetValue<Slider>().Value)) &&
							Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
						{
							bkb.UseAbility();
							Utils.Sleep(100, "bkb");
						}

						if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
						{
							Orbwalking.Orbwalk(e, 0, 1600, true, true);
						}
					}
				var illu = ObjectManager.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Hero_Terrorblade && x.IsIllusion)
									&& x.IsAlive && x.IsControllable).ToList();
					if (illu.Count > 0)
					{
						foreach (var v in illu)
						{
							if (
						   v.Distance2D(e) <= v.GetAttackRange() + v.HullRadius + 24 && !e.IsAttackImmune()
						   && v.NetworkActivity != NetworkActivity.Attack && v.CanAttack() && Utils.SleepCheck(v.Handle.ToString())
						   )
							{
								v.Attack(e);
								Utils.Sleep(270, v.Handle.ToString());
							}
							if (
							(!v.CanAttack() || v.Distance2D(e) >= 0) && !v.IsAttacking()
							&& v.NetworkActivity != NetworkActivity.Attack &&
							v.Distance2D(e) <= 1200 && Utils.SleepCheck(v.Handle.ToString())
							)
							{
								v.Move(e.Predict(400));
								Utils.Sleep(400, v.Handle.ToString());
							}
						}
					}
				}
			}
			if (Menu.Item("ult").GetValue<bool>())
			{
				if (me == null || !me.IsAlive) return;
				if (R == null || !R.CanBeCasted()) return;
				var ult = ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible
					&& x.IsAlive
					&& x.IsValid
					&& x.Team != me.Team
					&& !x.IsIllusion
					&& x.Distance2D(me) <= R.GetCastRange() + me.HullRadius + 24).ToList().OrderBy(x => ((double)x.MaximumHealth / x.Health)).FirstOrDefault();

				var illu = ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible
					&& x.IsAlive
					&& x.IsValid
					&& x.IsIllusion
					&& x.Distance2D(me) <= R.GetCastRange() + me.HullRadius + 24).ToList().OrderBy(x => ((double)x.MaximumHealth / x.Health)).FirstOrDefault();

				var ally = ObjectManager.GetEntities<Hero>()
					.Where(x =>
					x.IsVisible
					&& x.IsAlive
					&& x.IsValid
					&& x.Team == me.Team
					&& !x.IsIllusion
					&& x.Distance2D(me) <= R.GetCastRange() + me.HullRadius + 24).ToList().OrderBy(x => ((double)x.MaximumHealth / x.Health)).FirstOrDefault();

				if (ult != null && Menu.Item("ultEnem").GetValue<bool>())
				{
					var linkens = ult.IsLinkensProtected();
					if (!linkens
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& ult.Health >= (ult.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value)
						&& me.Health <= (me.MaximumHealth / 100 * Menu.Item("healme").GetValue<Slider>().Value) && Utils.SleepCheck("R"))
					{
						R.UseAbility(ult);
						Utils.Sleep(500, "R");
					}
				}
				if (ult == null || ult.Health < (ult.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value))
				{
					if (illu != null && Menu.Item("ultIllu").GetValue<bool>())
					{
						if (Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& illu.Health > (illu.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value)
							&& me.Health < (me.MaximumHealth / 100 * Menu.Item("healme").GetValue<Slider>().Value) && Utils.SleepCheck("R"))
						{
							R.UseAbility(illu);
							Utils.Sleep(500, "R");
						}
					}
				}
				if (ult == null || ult.Health < (ult.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value) || illu == null || illu.Health < (illu.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value))
				{
					if (ally != null && Menu.Item("ultAlly").GetValue<bool>())
					{
						if (Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& ally.Health > (ally.MaximumHealth / 100 * Menu.Item("heal").GetValue<Slider>().Value)
							&& me.Health < (me.MaximumHealth / 100 * Menu.Item("healme").GetValue<Slider>().Value) && Utils.SleepCheck("R"))
						{
							R.UseAbility(ally);
							Utils.Sleep(500, "R");
						}
					}
				}
			}
		}
	}
	internal class LogMessage
	{
		public static void Success(string text, params object[] arguments)
		{
			Game.PrintMessage("<font color='#00ff00'>" + text + "</font>", MessageType.LogMessage);
		}
	} // Console class
}