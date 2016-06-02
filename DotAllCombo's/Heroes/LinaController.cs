namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

	using Service;
	using Service.Debug;

    internal class LinaController : Variables, IHeroController
    {
        private Ability Q, W, R;
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu link = new Menu("Link", "Link");
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");
        private bool Active;

        private Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, force, cyclone;

        private int[] rDmg;

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("One little spark and before you know it, the whole world is burning.");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


		    skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"lina_dragon_slave", true},
			    {"lina_light_strike_array", true},
			    {"lina_laguna_blade", true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_cyclone", true},
			    {"item_orchid", true}, {"item_bloodthorn", true},
			    {"item_ethereal_blade", true},
			    {"item_veil_of_discord", true},
			    {"item_rod_of_atos", true},
			    {"item_sheepstick", true},
                {"item_dagon", true},
                {"item_arcane_boots", true},
			    {"item_blink", true},
			    {"item_shivas_guard", true},
			    {"item_soul_ring", true},
			    {"item_ghost", true},
			    {"item_cheese", true}
			})));
			ult.AddItem(new MenuItem("AutoUlt: ", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"lina_laguna_blade", true}
			})));
			items.AddItem(new MenuItem("Link: ", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_force_staff", true},
			    {"item_cyclone", true},
			    {"item_orchid", true},
                { "item_bloodthorn", true},
			    {"item_rod_of_atos", true},
			})));
			menu.AddSubMenu(skills);
			menu.AddSubMenu(items);
			menu.AddSubMenu(ult);
		}
		public void Combo()
		{
			target = me.ClosestToMouseTarget(2000);
			if (target == null)
				return;
			if (!menu.Item("enabled").IsActive())
				return;
			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

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

			force = me.FindItem("item_force_staff");

			cyclone = me.FindItem("item_cyclone");

			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

			atos = me.FindItem("item_rod_of_atos");
			//me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
			var modifHex = target.Modifiers.Where(y => y.Name == "modifier_sheepstick_debuff")
						.DefaultIfEmpty(null)
						.FirstOrDefault();
			var ModifEther = target.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = target.HasModifier("modifier_medusa_stone_gaze_stone");
			var EulModifier = target.Modifiers.Where(y => y.Name == "modifier_eul_cyclone").DefaultIfEmpty(null).FirstOrDefault();

			var EulModif = target.HasModifier("modifier_eul_cyclone");
			var noBlade = target.HasModifier("modifier_item_blade_mail_reflect");
			if (me.IsAlive)
			{
				if (Active && target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
				{
					if (
					me.Distance2D(target) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !target.IsAttackImmune())
					&& me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
						  )
					{
						me.Attack(target);
						Utils.Sleep(150, "attack");
					}
					else if (
						(!me.CanAttack() || me.Distance2D(target) >= 0) && me.NetworkActivity != NetworkActivity.Attack 
						&& (!me.IsAttackImmune() || !target.IsAttackImmune())
						&& me.Distance2D(target) <= 1000 && Utils.SleepCheck("Move")
						)
					{
						me.Move(target.Predict(300));
						Utils.Sleep(390, "Move");
					}
					if (
						blink != null
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(target) > 600
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(target.Position);
						Utils.Sleep(250, "blink");
					}
					if (
						cyclone != null
						&& cyclone.CanBeCasted()
						&& me.Distance2D(target) <= 900
						&& menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name)
						&& W.CanBeCasted()
						&& Utils.SleepCheck("cyclone"))
					{
						cyclone.UseAbility(target);
						Utils.Sleep(300, "cyclone");
					}
					if (W != null && W.CanBeCasted() && Utils.SleepCheck("w")
						&& (EulModifier != null && EulModifier.RemainingTime <= W.GetCastDelay(me, target, true) + 0.5
						|| modifHex != null && modifHex.RemainingTime <= W.GetCastDelay(me, target, true) + 0.5
						|| (sheep == null || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name) || sheep.Cooldown > 0)
						&& (cyclone == null || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) || cyclone.Cooldown < 20 && cyclone.Cooldown > 0)))
					{
						W.UseAbility(W.GetPrediction(target, W.GetCastDelay(me, target)));
						Utils.Sleep(150 + Game.Ping, "w");
					}
					
					if (cyclone == null || !cyclone.CanBeCasted() || !W.CanBeCasted() ||
						!menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
					{
						if (EulModif) return;

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

							Utils.Sleep(250, "atos");
						} // atos Item end

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
										Q != null
										&& Q.CanBeCasted()
										&& me.CanCast()
										&& me.Distance2D(target) < 1400
										&& !stoneModif
										&& !target.IsMagicImmune()
										&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
										&& Utils.SleepCheck("Q")
										)
									{
										Q.UseAbility(target);
										Utils.Sleep(200, "Q");
									}

									if (
										R != null
										&& R.CanBeCasted()
										&& !Q.CanBeCasted()
										&& !W.CanBeCasted()
										&& me.CanCast()
										&& me.Position.Distance2D(target) < 1200
										&& !stoneModif
										&& menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
										&& Utils.SleepCheck("R"))
									{
										R.UseAbility(target);
										Utils.Sleep(330, "R");
									}

									if ( // SoulRing Item 
										soulring != null
										&& soulring.CanBeCasted()
										&& me.CanCast()
										&& me.Health >= (me.MaximumHealth * 0.4)
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

									if ( // Dagon
										me.CanCast()
										&& dagon != null
										&& (ethereal == null
											|| (ModifEther
												|| ethereal.Cooldown < 17))
										&& !target.IsLinkensProtected()
										&& dagon.CanBeCasted()
                                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
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

				A();
			}
		}


		public void OnCloseEvent()
		{
			
		}

		private void A()
		{

			if (menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name) && R.CanBeCasted())
			{
				var enemies =
				 ObjectManager.GetEntities<Hero>()
					 .Where(x => x.IsVisible && x.IsAlive && x.Team == me.GetEnemyTeam() && !x.IsIllusion);
				foreach (var v in enemies)
				{
					if (v == null)
						return;

					if (me.AghanimState())
						rDmg = new int[3] { 450, 650, 850 };


					var leans = me.FindItem("item_aether_lens");
					var agh = (rDmg[R.Level - 1]);
					var damage = Math.Floor(rDmg[R.Level - 1] * (1 - v.MagicDamageResist));
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damage =
							Math.Floor(rDmg[R.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damage = 0;
					if (v.NetworkName == "CDOTA_Unit_Hero_Tusk" &&
						v.Spellbook.SpellW.CooldownLength - 3 > v.Spellbook.SpellQ.Cooldown)
						damage = 0;
					if (leans != null) damage = damage * 1.08;
					var rum = v.Modifiers.Any(x => x.Name == "modifier_kunkka_ghost_ship_damage_absorb");
					if (rum) damage = damage * 0.5;
					var mom = v.Modifiers.Any(x => x.Name == "modifier_item_mask_of_madness_berserk");
					if (mom) damage = damage * 1.3;

					if (!me.AghanimState() && !v.IsLinkensProtected())
					{
						if (vail != null && v.Health <= (damage * 1.25)
							&& vail.CanBeCasted()
							&& me.Distance2D(v) <= R.CastRange + 30
							&& R.CanBeCasted()
							&& me.Distance2D(v) <= 1200
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							vail.UseAbility(v.Position);
							Utils.Sleep(500, v.Handle.ToString());
						}

						if (ethereal != null && v.Health <= (damage * 1.5)
							&& me.Distance2D(v) <= ethereal.CastRange + 30
							&& R.CanBeCasted()
							&& !v.IsMagicImmune()
							&& ethereal.CanBeCasted()
							&& !v.IsLinkensProtected()
							&& me.Distance2D(v) <= 1000
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							ethereal.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
                        }

                        if (R != null && v != null && R.CanBeCasted()
							&& !v.HasModifier("modifier_tusk_snowball_movement")
							&& !v.HasModifier("modifier_snowball_movement_friendly")
							&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
							&& !v.HasModifier("modifier_ember_spirit_flame_guard")
							&&
							!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
							&&
							!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
							&& !v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
							&& !v.HasModifier("modifier_puck_phase_shift")
							&& !v.HasModifier("modifier_eul_cyclone")
							&& !v.HasModifier("modifier_dazzle_shallow_grave")
							&& !v.HasModifier("modifier_shadow_demon_disruption")
							&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
							&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
							&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
							&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
							&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
							&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
							!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
							&& !v.IsMagicImmune()
							&& v.Health <= (damage - 40 - v.HealthRegeneration)
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							R.UseAbility(v);
							Utils.Sleep(150, v.Handle.ToString());
							return;
						}
						if (me.AghanimState() && !v.IsLinkensProtected())
						{
							if (R != null && v != null && R.CanBeCasted()
								&& !v.HasModifier("modifier_tusk_snowball_movement")
								&& !v.HasModifier("modifier_snowball_movement_friendly")
								&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
								&& !v.HasModifier("modifier_ember_spirit_flame_guard")
								&&
								!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
								&&
								!v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
								&&
								!v.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
								&& !v.HasModifier("modifier_puck_phase_shift")
								&& !v.HasModifier("modifier_eul_cyclone")
								&& !v.HasModifier("modifier_dazzle_shallow_grave")
								&& !v.HasModifier("modifier_shadow_demon_disruption")
								&& !v.HasModifier("modifier_necrolyte_reapers_scythe")
								&& !v.HasModifier("modifier_storm_spirit_ball_lightning")
								&& !v.HasModifier("modifier_ember_spirit_fire_remnant")
								&& !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
								&& !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
								&& !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
								!v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
								&& !v.IsMagicImmune()
								&& menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
								&& v.Health <= (agh - v.HealthRegeneration)
								&& Utils.SleepCheck(v.Handle.ToString())
								)
							{
								R.UseAbility(v);
								Utils.Sleep(150, v.Handle.ToString());
								return;
							}
						}
					}
					if (v.IsLinkensProtected() &&
						(me.IsVisibleToEnemies || Game.IsKeyDown(menu.Item("Combo Key").GetValue<KeyBind>().Key)))
					{
						if (force != null && force.CanBeCasted() && me.Distance2D(v) < force.CastRange &&
							menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(force.Name) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							force.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(v) < orchid.CastRange &&
								 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							orchid.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (atos != null && atos.CanBeCasted() && me.Distance2D(v) < atos.CastRange - 400 &&
								 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(atos.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							atos.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(v) < dagon.CastRange &&
								 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled("item_dagon") &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							dagon.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (cyclone != null && cyclone.CanBeCasted() &&
								 me.Distance2D(v) < cyclone.CastRange &&
								 menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							cyclone.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
					}
				}
			}
		}
	}
}