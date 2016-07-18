namespace DotaAllCombo.Heroes
{
    using SharpDX;
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
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");
        

        private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, force, cyclone;

        private int[] rDmg;

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("One little spark and before you know it, the whole world is burning.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


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
			ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"lina_laguna_blade", true}
			})));
			items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_force_staff", true},
			    {"item_cyclone", true},
			    {"item_orchid", true},
                { "item_bloodthorn", true},
			    {"item_rod_of_atos", true},
			})));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
		}
		public void Combo()
		{
			e = me.ClosestToMouseTarget(2000);
			if (e == null)
				return;
			if (!Menu.Item("enabled").IsActive())
				return;
			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			R = me.Spellbook.SpellR;

			// Item
			ethereal = me.FindItem("item_ethereal_blade");

			sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

			vail = me.FindItem("item_veil_of_discord");

			cheese = me.FindItem("item_cheese");

			ghost = me.FindItem("item_ghost");

			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

			atos = me.FindItem("item_rod_of_atos");

			soul = me.FindItem("item_soul_ring");

			arcane = me.FindItem("item_arcane_boots");

			blink = me.FindItem("item_blink");

			shiva = me.FindItem("item_shivas_guard");

			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

			force = me.FindItem("item_force_staff");

			cyclone = me.FindItem("item_cyclone");

			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");

			atos = me.FindItem("item_rod_of_atos");
			//me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
			var modifHex = e.Modifiers.Where(y => y.Name == "modifier_sheepstick_debuff")
						.DefaultIfEmpty(null)
						.FirstOrDefault();
			var modifEther = e.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
			var eulModifier = e.Modifiers.Where(y => y.Name == "modifier_eul_cyclone").DefaultIfEmpty(null).FirstOrDefault();

			var eulModif = e.HasModifier("modifier_eul_cyclone");
			var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
			if (me.IsAlive)
			{
				if (Active && e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
				{
					if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900 && !eulModif)
					{
						Orbwalking.Orbwalk(e, 0, 1600, true, true);
					}
					if (
						blink != null
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(e) > 600
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(e.Position);
						Utils.Sleep(250, "blink");
					}
					if (
						cyclone != null
						&& cyclone.CanBeCasted()
						&& me.Distance2D(e) <= 900
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name)
						&& W.CanBeCasted()
						&& Utils.SleepCheck("cyclone"))
					{
						cyclone.UseAbility(e);
						Utils.Sleep(300, "cyclone");
					}
				
					Vector3 start = e.NetworkActivity == NetworkActivity.Move ? new Vector3((float)((W.GetCastDelay(me, e, true) + 0.6 + (Game.Ping / 500)) * Math.Cos(e.RotationRad) * e.MovementSpeed + e.Position.X),
												(float)((W.GetCastDelay(me, e, true) + 0.6 + (Game.Ping / 500)) * Math.Sin(e.RotationRad) * e.MovementSpeed + e.NetworkPosition.Y), e.NetworkPosition.Z): e.Position;
						if (W != null && W.CanBeCasted() && Utils.SleepCheck("w")
						&& (eulModifier != null && eulModifier.RemainingTime <= W.GetCastDelay(me, e, true) + 0.5
						|| modifHex != null && modifHex.RemainingTime <= W.GetCastDelay(me, e, true) + 0.5
						|| (sheep == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name) || sheep.Cooldown > 0)
						&& (cyclone == null || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) || cyclone.Cooldown < 20 && cyclone.Cooldown > 0)))
					{
						W.UseAbility(start);
						Utils.Sleep(150, "w");
					}
					if (cyclone == null || !cyclone.CanBeCasted() || !W.CanBeCasted() ||
						!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
					{
						if (eulModif) return;

						if ( // sheep
							sheep != null
							&& sheep.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& me.Distance2D(e) <= 1400
							&& !stoneModif
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
							&& Utils.SleepCheck("sheep")
							)
						{
							sheep.UseAbility(e);
							Utils.Sleep(250, "sheep");
						} // sheep Item end
						if ( // atos Blade
							atos != null
							&& atos.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
							&& me.Distance2D(e) <= 2000
							&& Utils.SleepCheck("atos")
							)
						{
							atos.UseAbility(e);

							Utils.Sleep(250, "atos");
						} // atos Item end

						if ( // orchid
							orchid != null
							&& orchid.CanBeCasted()
							&& me.CanCast()
							&& !e.IsLinkensProtected()
							&& !e.IsMagicImmune()
							&& me.Distance2D(e) <= 1400
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
							&& !stoneModif
							&& Utils.SleepCheck("orchid")
							)
						{
							orchid.UseAbility(e);
							Utils.Sleep(250, "orchid");
						} // orchid Item end
						if (!orchid.CanBeCasted() || orchid == null ||
							!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
						{
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
							if (!vail.CanBeCasted() || vail == null ||
								!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name))
							{
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
								if (!ethereal.CanBeCasted() || ethereal == null ||
									!Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
									{
									if (
										Q != null
										&& Q.CanBeCasted()
										&& me.CanCast()
										&& me.Distance2D(e) < 1400
										&& !stoneModif
										&& !e.IsMagicImmune()
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
										&& Utils.SleepCheck("Q")
										)
									{
										Q.UseAbility(e);
										Utils.Sleep(200, "Q");
									}

									if (
										R != null
										&& R.CanBeCasted()
										&& !Q.CanBeCasted()
										&& !W.CanBeCasted()
										&& me.CanCast()
										&& me.Position.Distance2D(e) < 1200
										&& !stoneModif
										&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
										&& Utils.SleepCheck("R"))
									{
										R.UseAbility(e);
										Utils.Sleep(330, "R");
									}

									if ( // SoulRing Item 
										soul != null
										&& soul.CanBeCasted()
										&& me.CanCast()
										&& me.Health >= (me.MaximumHealth * 0.4)
										&& me.Mana <= R.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
										)
									{
										soul.UseAbility();
									} // SoulRing Item end

									if ( // Arcane Boots Item
										arcane != null
										&& arcane.CanBeCasted()
										&& me.CanCast()
										&& me.Mana <= R.ManaCost
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
										)
									{
										arcane.UseAbility();
									} // Arcane Boots Item end

									if ( //Ghost
										ghost != null
										&& ghost.CanBeCasted()
										&& me.CanCast()
										&& ((me.Position.Distance2D(e) < 300
											 && me.Health <= (me.MaximumHealth * 0.7))
											|| me.Health <= (me.MaximumHealth * 0.3))
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
										&& Utils.SleepCheck("Ghost"))
									{
										ghost.UseAbility();
										Utils.Sleep(250, "Ghost");
									}


									if ( // Shiva Item
										shiva != null
										&& shiva.CanBeCasted()
										&& me.CanCast()
										&& !e.IsMagicImmune()
										&& Utils.SleepCheck("shiva")
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
										&& me.Distance2D(e) <= 600
										)

									{
										shiva.UseAbility();
										Utils.Sleep(250, "shiva");
									} // Shiva Item end

									if ( // Dagon
										me.CanCast()
										&& dagon != null
										&& (ethereal == null
											|| (modifEther
												|| ethereal.Cooldown < 17))
										&& !e.IsLinkensProtected()
										&& dagon.CanBeCasted()
                                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                        && !e.IsMagicImmune()
										&& !stoneModif
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
										&& me.Health <= (me.MaximumHealth * 0.3)
										&& me.Distance2D(e) <= 700
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
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

			if (Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name) && R.CanBeCasted())
			{
				var enemies =
				 ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
				foreach (var v in enemies)
				{
					if (v == null)
						return;
					
					rDmg = new [] { 450, 650, 850 };


					var leans = me.FindItem("item_aether_lens");
					var agh = (rDmg[R.Level - 1]);
					double damage = (rDmg[R.Level - 1] * (1 - v.MagicDamageResist));
					if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
					{
						damage =
							Math.Floor(rDmg[R.Level - 1] *
									   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
					}
					if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						v.Spellbook.SpellR.CanBeCasted())
						damage = 0;
					if (leans != null) damage = damage * 1.08;

					if (!me.AghanimState() && !v.IsLinkensProtected())
					{
						var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
						if (mom) damage = damage * 1.3;
						var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
						if (rum) damage = damage * 0.5;

						var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
						damage = damage * spellamplymult;

						if ( // vail
							vail != null
							&& vail.CanBeCasted()
							&& me.Distance2D(v) <= R.GetCastRange() + me.HullRadius
							&& R.CanBeCasted()
							&& v.Health <= damage * 1.25
							&& v.Health >= damage
							&& me.CanCast()
							&& !v.HasModifier("modifier_item_veil_of_discord_debuff")
							&& !v.IsMagicImmune()
							&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
							&& me.Distance2D(v) <= W.GetCastRange()
							&& Utils.SleepCheck("vail")
							)
						{
							vail.UseAbility(v.Position);
							Utils.Sleep(250, "vail");
						}
						int etherealdamage = (int)(((me.TotalIntelligence * 2) + 75));
						if ( // vail
						  ethereal != null
						  && ethereal.CanBeCasted()
						  && R != null
						  && R.CanBeCasted()
						  && v.Health <= etherealdamage + (damage * 1.4)
						  && v.Health >= damage
						  && me.CanCast()
						  && !v.HasModifier("modifier_item_ethereal_blade_slow")
						  && !v.IsMagicImmune()
						  && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
						  && me.Distance2D(v) <= ethereal.GetCastRange() + me.HullRadius
						  && Utils.SleepCheck("ethereal")
						  )
						{
							ethereal.UseAbility(v);
							Utils.Sleep(250, "ethereal");
						}
						Console.WriteLine(damage);
						Console.WriteLine(v.Health);
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
							&& v.Health < damage
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							R.UseAbility(v);
							Utils.Sleep(150, v.Handle.ToString());
							return;
						}
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
							&& Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& v.Health <= (agh - v.HealthRegeneration * R.ChannelTime)
							&& Utils.SleepCheck(v.Handle.ToString())
							)
						{
							R.UseAbility(v);
							Utils.Sleep(150, v.Handle.ToString());
							return;
						}
					}
					if (v.IsLinkensProtected() &&
						(me.IsVisibleToEnemies || Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key)))
					{
						if (force != null && force.CanBeCasted() && me.Distance2D(v) < force.GetCastRange() &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							force.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(v) < orchid.GetCastRange() &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							orchid.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (atos != null && atos.CanBeCasted() && me.Distance2D(v) < atos.GetCastRange() - 400 &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(atos.Name) &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							atos.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(v) < dagon.GetCastRange() &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled("item_dagon") &&
								 Utils.SleepCheck(v.Handle.ToString()))
						{
							dagon.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
						else if (cyclone != null && cyclone.CanBeCasted() &&
								 me.Distance2D(v) < cyclone.GetCastRange() &&
								 Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name) &&
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
