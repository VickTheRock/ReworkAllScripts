namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

	using Service;
	using Service.Debug;

    internal class SilencerController : Variables, IHeroController
    {
        private Ability Q, W, E, R;
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");
        private bool Active;

        private Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, force, cyclone;

        public void Combo()
		{
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

			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

			atos = me.FindItem("item_rod_of_atos");

			soulring = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");

			blink = me.FindItem("item_blink");

			shiva = me.FindItem("item_shivas_guard");

			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            
			var ModifEther = target.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = target.HasModifier("modifier_medusa_stone_gaze_stone");
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.GetEnemyTeam() && !x.IsIllusion);


			if (Active && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
			{
				var noBlade = target.HasModifier("modifier_item_blade_mail_reflect");
				if (target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
				{
					if (
						Q != null
						&& Q.CanBeCasted()
						&& me.CanCast()
						&& me.Distance2D(target) < 1400
						&& !stoneModif
						&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
						&& Utils.SleepCheck("Q")
						)
					{
						Q.UseAbility(target.Position);
						Utils.Sleep(200, "Q");
					}
					if ( // atos Blade
						atos != null
						&& atos.CanBeCasted()
						&& me.CanCast()
						&& !target.IsLinkensProtected()
						&& !target.IsMagicImmune()
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
						&& me.Distance2D(target) <= 2000
						&& Utils.SleepCheck("atos")
						)
					{
						atos.UseAbility(target);

						Utils.Sleep(250 + Game.Ping, "atos");
					} // atos Item end

					if (
						blink != null
						&& Q.CanBeCasted()
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(target) > 1000
						&& !stoneModif
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(target.Position);

						Utils.Sleep(250, "blink");
					}
					if (
						E != null
						&& E.CanBeCasted()
						&& me.CanCast()
						&& !target.IsLinkensProtected()
						&& me.Position.Distance2D(target) < 1400
						&& !stoneModif
						&& Utils.SleepCheck("E"))
					{
						E.UseAbility(target);
						Utils.Sleep(200, "E");
					}
					if (!E.CanBeCasted() || E == null)
					{
						if ( // orchid
							orchid != null
							&& orchid.CanBeCasted()
							&& me.CanCast()
							&& !target.IsLinkensProtected()
							&& !target.IsMagicImmune()
							&& me.Distance2D(target) <= 1400
							&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
							&& !stoneModif
							&& Utils.SleepCheck("orchid")
							)
						{
							orchid.UseAbility(target);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!orchid.CanBeCasted() || orchid == null ||
							!menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
							if ( // vail
								vail != null
								&& vail.CanBeCasted()
								&& me.CanCast()
								&& !target.IsMagicImmune()
								&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
								&& me.Distance2D(target) <= 1500
								&& Utils.SleepCheck("vail")
								)
							{
								vail.UseAbility(target.Position);
								Utils.Sleep(250, "vail");
							} // orchid Item end
							if (!vail.CanBeCasted() || vail == null ||
								!menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name))
							{
								if ( // ethereal
									ethereal != null
									&& ethereal.CanBeCasted()
									&& me.CanCast()
									&& !target.IsLinkensProtected()
									&& !target.IsMagicImmune()
									&& !stoneModif
									&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
									&& Utils.SleepCheck("ethereal")
									)
								{
									ethereal.UseAbility(target);
									Utils.Sleep(200, "ethereal");
								} // ethereal Item end
								if (!ethereal.CanBeCasted() || ethereal == null ||
									!menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
								{
									if (
										W != null
										&& target.IsVisible
										&& W.CanBeCasted()
										&& me.CanCast()
										&& me.Distance2D(target) < 900
										&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
										&& Utils.SleepCheck("W"))
									{
										W.UseAbility(target);
										Utils.Sleep(150, "W");
									}
									if (
										R != null
										&& R.CanBeCasted()
										&& me.CanCast()
										&& (v.Count(x => x.Distance2D(me) <= 700) >=
											(menu.Item("Heel").GetValue<Slider>().Value))
										&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
										&& Utils.SleepCheck("R"))
									{
										R.UseAbility();
										Utils.Sleep(330, "R");
									}

									if ( // SoulRing Item 
										soulring != null
										&& soulring.CanBeCasted()
										&& me.CanCast()
                                        && me.Health <= (me.MaximumHealth * 0.5)
                                        && me.Mana <= R.ManaCost
										&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
										)
									{
										soulring.UseAbility();
									} // SoulRing Item end

									if ( // Arcane Boots Item
										arcane != null
										&& arcane.CanBeCasted()
										&& me.CanCast()
										&& me.Mana <= R.ManaCost
										&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
										)
									{
										arcane.UseAbility();
									} // Arcane Boots Item end

									if ( //Ghost
										ghost != null
										&& ghost.CanBeCasted()
										&& me.CanCast()
										&& ((me.Position.Distance2D(target) < 300
											 && me.Health <= (me.MaximumHealth * 0.7))
											|| me.Health <= (me.MaximumHealth * 0.3))
										&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if ( // Shiva Item
										shiva != null
										&& shiva.CanBeCasted()
										&& me.CanCast()
										&& !target.IsMagicImmune()
										&& Utils.SleepCheck("shiva")
										&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
										&& me.Distance2D(target) <= 600
										)

									{
										shiva.UseAbility();
										Utils.Sleep(250, "shiva");
									} // Shiva Item end


									if ( // sheep
										sheep != null
										&& sheep.CanBeCasted()
										&& me.CanCast()
										&& !target.IsLinkensProtected()
										&& !target.IsMagicImmune()
										&& me.Distance2D(target) <= 1400
										&& !stoneModif
										&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
										&& Utils.SleepCheck("sheep")
										)
									{
										sheep.UseAbility(target);
										Utils.Sleep(250, "sheep");
									} // sheep Item end

									if ( // Dagon
										me.CanCast()
										&& dagon != null
										&& (ethereal == null
											|| (ModifEther
												|| ethereal.Cooldown < 17))
										&& !target.IsLinkensProtected()
										&& dagon.CanBeCasted()
										&& !target.IsMagicImmune()
										&& !stoneModif
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
										&& me.Health <= (me.MaximumHealth * 0.3)
										&& me.Distance2D(target) <= 700
										&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
										&& Utils.SleepCheck("cheese")
										)
									{
										cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
								}
							}
						}
					}
				}
				Utils.Sleep(200, "activated");
				if (
					(!me.CanAttack() || me.Distance2D(target) >= 300) && me.NetworkActivity != NetworkActivity.Attack &&
					me.Distance2D(target) <= 850 && Utils.SleepCheck("Move")
					)
				{
					me.Move(target.Predict(300));
					Utils.Sleep(390, "Move");
				}
				else if (
					me.Distance2D(target) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !target.IsAttackImmune())
					&& me.NetworkActivity != NetworkActivity.Attack && (W.Level <= 1 || me.Mana <= 30) && me.CanAttack() &&
					Utils.SleepCheck("attack")
					)
				{
					me.Attack(target);
					Utils.Sleep(150, "attack");
				}
			}
			A();
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("What's wrong, Queenie? Cat got your tongue?");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


		    skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"silencer_last_word", true},
			    {"silencer_global_silence", true},
			    {"silencer_curse_of_the_silent", true},
			    {"silencer_glaives_of_wisdom", true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_orchid", true}, {"item_bloodthorn", true},
			    {"item_ethereal_blade", true},
			    {"item_veil_of_discord", true},
			    {"item_rod_of_atos", true},
			    {"item_sheepstick", true},
			    {"item_arcane_boots", true},
			    {"item_blink", true},
			    {"item_soul_ring", true},
			    {"item_ghost", true},
			    {"item_cheese", true}
			})));
			ult.AddItem(new MenuItem("AutoUlt: ", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"silencer_global_silence", true}
			})));
			ult.AddItem(new MenuItem("Heel", "Min targets to AutoUlt").SetValue(new Slider(2, 1, 5)));
			items.AddItem(new MenuItem("Link: ", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_force_staff", true},
			    {"item_cyclone", true},
			    {"item_rod_of_atos", true},
			    {"item_dagon", true}
			})));
			menu.AddSubMenu(skills);
			menu.AddSubMenu(items);
			menu.AddSubMenu(ult);
		}

		public void OnCloseEvent()
		{
			
		}

		public void A()
		{
            if (!me.IsAlive)return;
			var v =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
            if (v.Count <= 0) return;
            if (ult.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name))
			{
				if (
					R != null
					&& R.CanBeCasted()
					&& me.CanCast()
					&& (v.Count(x => x.Distance2D(me) <= 700) >=
						(menu.Item("Heel").GetValue<Slider>().Value))
					&& Utils.SleepCheck("R"))
				{
					R.UseAbility();
					Utils.Sleep(330, "R");
				}
			}
			force = me.FindItem("item_force_staff");
			cyclone = me.FindItem("item_cyclone");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
           
			foreach (var e in v)
			{
				if (e.IsLinkensProtected() && (me.IsVisibleToEnemies || Active))
				{
					if (force != null && force.CanBeCasted() && me.Distance2D(e) < force.CastRange &&
						menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(force.Name) &&
						Utils.SleepCheck(e.Handle.ToString()))
					{
						force.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
					else if (cyclone != null && cyclone.CanBeCasted() && me.Distance2D(e) < cyclone.CastRange &&
							 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) &&
							 Utils.SleepCheck(e.Handle.ToString()))
					{
						cyclone.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
					else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) < orchid.CastRange &&
							 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
							 Utils.SleepCheck(e.Handle.ToString()))
					{
						orchid.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
					else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(e) < dagon.CastRange &&
							 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled("item_dagon") &&
							 Utils.SleepCheck(e.Handle.ToString()))
					{
						dagon.UseAbility(e);
						Utils.Sleep(500, e.Handle.ToString());
					}
				}
			}
		}
	}
}