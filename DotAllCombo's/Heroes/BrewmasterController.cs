namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using SharpDX;
    using SharpDX.Direct3D9;
	
	using Service;
	using Service.Debug;

	internal class BrewmasterController : Variables, IHeroController
    {
        private readonly Menu Menu_Items = new Menu("Items", "Items");
        private readonly Menu Menu_Skills = new Menu("Skills: ", "Skills: ");
        private Ability Q, W, R;
        private Item blink, bkb, orchid, necronomicon, urn, medal, shiva, manta;
        private Vector3 mousepos;

	    public void Combo()
		{
			blink = me.FindItem("item_blink");
			bkb = me.FindItem("item_black_king_bar");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			necronomicon = me.FindItem("item_necronomicon");
			urn = me.FindItem("item_urn_of_shadows");
			medal = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			shiva = me.FindItem("item_shivas_guard");
			manta = me.FindItem("item_manta");
			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			R = me.Spellbook.SpellR;
			//manta (when silenced)
			if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)) &&
				manta.CanBeCasted() && me.IsSilenced() && Utils.SleepCheck("manta"))
			{
				manta.UseAbility();
				Utils.Sleep(400, "manta");
			}
			if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
			{
				if (me.CanCast())
				{
					mousepos = Game.MousePosition;
                    e = Toolset.ClosestToMouse(me);
                    if (me.Distance2D(mousepos) <= 1200)
					{
						//drunken haze (main combo)
						if ((W != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(W.Name)) &&
							W.CanBeCasted() &&
							((e.Position.Distance2D(me.Position) < 850) &&
							 (e.Position.Distance2D(me.Position) > 300)) && R.CanBeCasted() &&
							Utils.SleepCheck("W"))
						{
							W.UseAbility(e);
							Utils.Sleep(150, "W");
						}
						//drunken haze (if can't cast ult) --->Сюда добавить переменную отвечающую за ручное выключение ульты из комбо && если ульт выключен
						if ((W != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(W.Name)) &&
							W.CanBeCasted() && e.Position.Distance2D(me.Position) < 850 &&
							(!R.CanBeCasted() || (e.Health < (e.MaximumHealth * 0.50)) ||
							 !(Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(R.Name))) && !e.HasModifier("modifier_brewmaster_drunken_haze") &&
							(e.Health > (e.MaximumHealth * 0.07)) && Utils.SleepCheck("W"))
						{
							W.UseAbility(e);
							Utils.Sleep(150, "W");
						}
						//black king bar
						if ((bkb != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name)) &&
							bkb.CanBeCasted() && Utils.SleepCheck("bkb"))
						{
							bkb.UseAbility();
							Utils.Sleep(150, "bkb");
						}
						//blink
						if ((blink != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)) &&
							blink.CanBeCasted() && e.Position.Distance2D(me.Position) <= 1150 &&
							e.Position.Distance2D(me.Position) > 300 && Utils.SleepCheck("blink"))
						{
							blink.UseAbility(e.Position);
							//blink.UseAbility(me.Distance2D(mousepos) < 1200 ? mousepos : new Vector3(me.NetworkPosition.X + 1150 * (float)Math.Cos(me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), me.NetworkPosition.Y + 1150 * (float)Math.Sin(me.NetworkPosition.ToVector2().FindAngleBetween(mousepos.ToVector2(), true)), 100), false);
							Utils.Sleep(150, "blink");
						}
						//orchid
						if ((orchid != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)) &&
							orchid.CanBeCasted() && (e.Position.Distance2D(me.Position) < 300) &&
							Utils.SleepCheck("orchid"))
						{
							orchid.UseAbility(e);
							Utils.Sleep(150, "orchid");
						}
						//necronomicon
						if (necronomicon != null && necronomicon.CanBeCasted() &&
							(e.Health > (e.MaximumHealth * 0.20)) &&
							(e.Position.Distance2D(me.Position) < 400) && Utils.SleepCheck("necronomicon"))
						{
							necronomicon.UseAbility();
							Utils.Sleep(150, "necronomicon");
						}
						//thunder clap
						if ((Q != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(Q.Name)) &&
							Q.CanBeCasted() && (e.Position.Distance2D(me.Position) < 300) && Utils.SleepCheck("Q"))
						{
							Q.UseAbility();
							Utils.Sleep(150, "Q");
						}
						//urn of shadow
						if ((urn != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name)) &&
							urn.CanBeCasted() && Utils.SleepCheck("urn"))
						{
							urn.UseAbility(e);
							Utils.Sleep(150, "urn");
						}
						//medal
						if ((medal != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medal.Name)) &&
							medal.CanBeCasted() && (e.Position.Distance2D(me.Position) < 300) &&
							Utils.SleepCheck("medal"))
						{
							medal.UseAbility(e);
							Utils.Sleep(150, "medal");
						}
						//shiva
						if ((shiva != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)) &&
							shiva.CanBeCasted() && (e.Position.Distance2D(me.Position) <= 800) &&
							Utils.SleepCheck("shiva"))
						{
							shiva.UseAbility();
							Utils.Sleep(150, "shiva");
						}
						//manta
						if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)) &&
							manta.CanBeCasted() && (e.Position.Distance2D(me.Position) <= 450) &&
							Utils.SleepCheck("manta"))
						{
							manta.UseAbility();
							Utils.Sleep(150, "manta");
						}
						//ULTIMATE: R
						if ((R != null && Menu.Item("Skills: ").GetValue<AbilityToggler>().IsEnabled(R.Name)) &&
							R.CanBeCasted() && (e.Position.Distance2D(me.Position) < 500) &&
							(e.Health > (e.MaximumHealth * 0.35)) && !Q.CanBeCasted() && !orchid.CanBeCasted() &&
							!necronomicon.CanBeCasted() && !urn.CanBeCasted() && !medal.CanBeCasted() &&
							!shiva.CanBeCasted() && !manta.CanBeCasted() && Utils.SleepCheck("R"))
						{
							R.UseAbility();
							Utils.Sleep(1000, "R");
						}
						//Moving+Attaking
						if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
						{
							Orbwalking.Orbwalk(e, 0, 1600, true, true);
						}
					}
				}
			}
			var targets =
				ObjectManager.GetEntities<Hero>()
					.Where(
						enemy =>
							enemy.Team == me.GetEnemyTeam() && !enemy.IsIllusion() && enemy.IsVisible && enemy.IsAlive &&
							enemy.Health > 0)
					.ToList();


			if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key))
			{
				if (e.IsAlive && !e.IsInvul() &&
					(Game.MousePosition.Distance2D(e) <= 1000 || e.Distance2D(me) <= 600))
				{
					var CheckDrunken = e.HasModifier("modifier_brewmaster_drunken_haze");
					var CheckClap = e.HasModifier("modifier_brewmaster_thunder_clap");


					var Necronomicons =
						ObjectManager.GetEntities<Creep>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep)
																  && x.IsAlive && x.IsControllable);
					if (Necronomicons == null)
					{
						return;
					}
					foreach (var v in Necronomicons)
					{
						var archer =
							ObjectManager.GetEntities<Unit>()
								.Where(
									unit =>
										unit.Name == "npc_dota_necronomicon_archer" && unit.IsAlive &&
										unit.IsControllable)
								.ToList();
						if (archer != null && e.Position.Distance2D(v.Position) <= 650 &&
							v.Spellbook.SpellQ.CanBeCasted() &&
							Utils.SleepCheck(v.Handle.ToString()))

						{
							v.Spellbook.SpellQ.UseAbility(e);
							Utils.Sleep(300, v.Handle.ToString());
						}

						if (e.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(e);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}

					//Necronomicon Warrior
					var Necrowarrior =
						ObjectManager.GetEntities<Creep>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep)
																  && x.IsAlive && x.IsControllable);
					if (Necrowarrior == null)
					{
						return;
					}
					foreach (var v in Necrowarrior)
					{
						var warrior =
							ObjectManager.GetEntities<Unit>()
								.Where(
									unit =>
										unit.Name == "npc_dota_necronomicon_warrior" && unit.IsAlive &&
										unit.IsControllable)
								.ToList();


						if (e.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(e);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}
					//Illusions	
					var illus =
						ObjectManager.GetEntities<Hero>()
							.Where(x => x.IsAlive && x.IsControllable && x.Team == me.Team && x.IsIllusion)
							.ToList();
					if (illus == null)
					{
						return;
					}
					foreach (Unit illusion in illus.TakeWhile(illusion => Utils.SleepCheck("illu")))
					{
						illusion.Attack(e);
						Utils.Sleep(1000, "illu");
					}

					var primalstorm =
						ObjectManager.GetEntities<Unit>()
							.Where(x => x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm && x.IsAlive);
					var primalearth =
						ObjectManager.GetEntities<Unit>()
							.Where(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth && x.IsAlive));
					var primalfire =
						ObjectManager.GetEntities<Unit>()
							.Where(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalFire && x.IsAlive));
					if (primalearth == null)
					{
						return;
					}
					foreach (var v in primalearth)
					{
						if (e.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellQ.CanBeCasted() &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellQ.UseAbility(e);
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (e.Position.Distance2D(v.Position) < 300 && v.Spellbook.SpellR.CanBeCasted() &&
							!CheckClap &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellR.UseAbility();
							Utils.Sleep(400, v.Handle.ToString());
						}

						if (e.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(e);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}

					if (primalstorm == null)
					{
						return;
					}


					foreach (var v in primalstorm)
					{
						if (e.Position.Distance2D(v.Position) < 500 && v.Spellbook.SpellQ.CanBeCasted() &&
							(Menu.Item("Primalstorm: Use Dispel Magic").GetValue<bool>()) &&
							(!(Menu.Item("Save mana for Cyclone").GetValue<bool>()) ||
							 (v.Mana > (v.Spellbook.SpellQ.ManaCost + v.Spellbook.SpellW.ManaCost))) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellQ.UseAbility(e.Position);
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (e.Position.Distance2D(v.Position) < 900 && v.Spellbook.SpellE.CanBeCasted() &&
							(!(Menu.Item("Save mana for Cyclone").GetValue<bool>()) ||
							 (v.Mana > (v.Spellbook.SpellE.ManaCost + v.Spellbook.SpellW.ManaCost))) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellE.UseAbility();
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (e.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellR.CanBeCasted() &&
							!CheckDrunken &&
							(!(Menu.Item("Save mana for Cyclone").GetValue<bool>()) ||
							 (v.Mana > (v.Spellbook.SpellR.ManaCost + v.Spellbook.SpellW.ManaCost))) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Spellbook.SpellR.UseAbility(e);
							Utils.Sleep(400, v.Handle.ToString());
						}
						if (e.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(e);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}
					// 2 Skill

					foreach (var target1 in targets)
					{
						if ((target1.Health > (target1.MaximumHealth * 0.85)) || ((target1.IsChanneling())))
						{
							foreach (var v in primalstorm)
							{
								W = v.Spellbook.SpellW;
								if (v.Spellbook.SpellW.CanBeCasted() &&
									((target1.Position.Distance2D(v.Position) < 600) || (target1.IsChanneling())) &&
									(target1.Position.Distance2D(v.Position) < 1550) &&
									((Menu.Item("Primalstorm: Use Cyclone").GetValue<bool>()) ||
									 (target1.IsChanneling())) &&
									((target1.Position != e.Position) || (target1.IsChanneling())) &&
									Utils.SleepCheck("ulti"))
								{
									v.Spellbook.SpellW.UseAbility(target1);
									Utils.Sleep(700, "ulti");
								}
							}
						}
					}
					//

					if (primalfire == null)
					{
						return;
					}
					foreach (var v in primalfire)
					{
						if (e.Position.Distance2D(v.Position) < 1550 &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							v.Attack(e);
							Utils.Sleep(700, v.Handle.ToString());
						}
					}
				}
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("Need to Creet!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
			Menu.AddSubMenu(Menu_Items);
			Menu_Items.AddItem(new MenuItem("Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_blink", true},
			    {"item_black_king_bar", true},
			    {"item_orchid", true}, {"item_bloodthorn", true},
			    {"item_necronomicon", true},
			    {"item_solar_crest", true},
			    {"item_urn_of_shadows", true},
			    {"item_medallion_of_courage", true},
			    {"item_shivas_guard", true},
			    {"item_manta", true}
			})));
			Menu.AddSubMenu(Menu_Skills);
			Menu_Skills.AddItem(new MenuItem("Skills: ", "Skills: ").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"brewmaster_thunder_clap", true},
			    {"brewmaster_drunken_haze", true},
			    {"brewmaster_primal_split", true}
			})));
			Console.WriteLine(">Brewmaster by VickTheRock loaded!");
			Menu.AddItem(
				new MenuItem("Primalstorm: Use Cyclone", "Primalstorm: Use Cyclone").SetValue(false)
					.SetTooltip(
						"If disabled, casts Cyclone only in targets, which channeling abilitys like: tp's, blackhole's, deathward's and e.t.c."));
			Menu.AddItem(
				new MenuItem("Primalstorm: Use Dispel Magic", "Primalstorm: Use Dispel Magic").SetValue(false)
					.SetTooltip("If enabled, always safe mana for Cyclon."));
			Menu.AddItem(
				new MenuItem("Save mana for Cyclone", "Save mana for Cyclone").SetValue(false)
					.SetTooltip(
						"Do not cast Dispel Magic, Drunken Haze or Invisability if after cast there will be no mana for Cyclone."));
			
		}

		public void OnCloseEvent()
		{
			
		}
	}
}