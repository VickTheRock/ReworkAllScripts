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

	internal class LionController : Variables, IHeroController
	{
		private Ability Q, W, E, R;
		private readonly Menu skills = new Menu("Skills", "Skills");
		private readonly Menu items = new Menu("Items", "Items");
		private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");


		private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;

		private int[] rDmg;

		public void Combo()
		{
			e = me.ClosestToMouseTarget(2000);
			if (e == null) return;

			//spell
			Q = me.Spellbook.SpellQ;

			W = me.Spellbook.SpellW;

			E = me.Spellbook.SpellE;

			R = me.Spellbook.SpellR;

			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
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



			var modifEther = e.HasModifier("modifier_item_ethereal_blade_slow");
			var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");

			if (Active && me.IsAlive && e.IsAlive)
			{
				var hexMod = e.Modifiers.Where(y => y.Name == "modifier_lion_voodoo" || y.Name == "modifier_sheepstick_debuff" || y.Name == "modifier_lion_impale").DefaultIfEmpty(null).FirstOrDefault();
				var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
				if (e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade && !e.IsLinkensProtected() )
				{

					if ( // atos Blade
						atos != null
						&& atos.CanBeCasted()
						&& me.CanCast()
						&& !e.IsLinkensProtected()
						&& !e.IsMagicImmune()
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)
						&& me.Distance2D(e) <= 1500
						&& Utils.SleepCheck("atos")
						)
					{
						atos.UseAbility(e);

						Utils.Sleep(250, "atos");
					} // atos Item end
					if (
						W != null
						&& W.CanBeCasted()
						&& !e.IsHexed()
						&& me.CanCast()
						&& !e.IsStunned()
						&& me.Distance2D(e) <= 1500
						&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
						&& Utils.SleepCheck("W"))
					{
						W.UseAbility(e);
						Utils.Sleep(300, "W");
					}
					if (
						blink != null
						&& me.CanCast()
						&& blink.CanBeCasted()
						&& me.Distance2D(e) > 1000
						&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
						&& Utils.SleepCheck("blink")
						)
					{
						blink.UseAbility(e.Position);
						Utils.Sleep(250, "blink");
					}
					
					if(W == null || !W.CanBeCasted() ||
					!Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
					{
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
						if (orchid == null || !orchid.CanBeCasted() ||
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
										&& (hexMod == null || hexMod.RemainingTime <= 0.1 + Game.Ping)
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
										&& me.Health >= (me.MaximumHealth*0.4)
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
										     && me.Health <= (me.MaximumHealth*0.7))
										    || me.Health <= (me.MaximumHealth*0.3))
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

									if ( // Dagon
										me.CanCast()
										&& !W.CanBeCasted()
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
										&& me.Health <= (me.MaximumHealth*0.3)
										&& me.Distance2D(e) <= 700
										&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
										&& Utils.SleepCheck("cheese")
										)
									{
										cheese.UseAbility();
										Utils.Sleep(200, "cheese");
									} // cheese Item end
									if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
									{
										Orbwalking.Orbwalk(e, 0, 1600, true, true);
									}
								}
							}
						}
					}
				}
			}
			if (me != null && me.IsAlive && e.Distance2D(me) <= 1000)
			{
				A();
			}
		}

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("To destroy the darkness in itself!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


			skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lion_voodoo", true},
				{"lion_impale", true},
				{"lion_finger_of_death", true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"item_dagon", true},
				{"item_orchid", true},
				{ "item_bloodthorn", true},
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
			ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lion_finger_of_death", true}
			})));
			items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
				{"lion_mana_drain", true}
			})));
			Menu.AddSubMenu(skills);
			Menu.AddSubMenu(items);
			Menu.AddSubMenu(ult);
		}

		public void OnCloseEvent()
		{

		}

		public void A()
		{
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
			if (enemies.Count <= 0 || me == null || !me.IsAlive && !R.CanBeCasted()) return;

			if (Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name))
			{

                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
                foreach (var v in enemies)
				{
					orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
					atos = me.FindItem("item_rod_of_atos");

					rDmg = me.AghanimState() ? new[] {725, 875, 1025} : new[] {600, 725, 850};
					

					var lens = me.HasModifier("modifier_item_aether_lens");
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
					if (lens) damage = damage * 1.08;
					var rum = v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb");
					if (rum) damage = damage * 0.5;
					var mom = v.HasModifier("modifier_item_mask_of_madness_berserk");
					if (mom) damage = damage * 1.3;

					var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
                    if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                    {
                        var blood =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker);
                        if (blood != null)
                            damage = damage * bloodrage[blood.Spellbook.Spell1.Level];
                        else
                            damage = damage * 1.4;
                    }


                    if (v.HasModifier("modifier_chen_penitence"))
                    {
                        var chen =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen);
                        if (chen != null)
                            damage = damage * penitence[chen.Spellbook.Spell1.Level];
                    }


                    if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    {
                        var demon =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon);
                        if (demon != null)
                            damage = damage * souls[demon.Spellbook.Spell2.Level];
                    }
                    damage = damage * spellamplymult;

					if ( // vail
						vail != null
						&& vail.CanBeCasted()
						&& me.Distance2D(v)<=R.GetCastRange()+me.HullRadius
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


					if (R != null && v != null && R.CanBeCasted()
						&& !v.HasModifier("modifier_tusk_snowball_movement")
						&& !v.HasModifier("modifier_snowball_movement_friendly")
						&& !v.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !v.HasModifier("modifier_ember_spirit_flame_guard")
						&& !v.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
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
						&& v.Health <= (damage - 40)
						&& me.Distance2D(v) <= R.GetCastRange() + 50
						&& Utils.SleepCheck(v.Handle.ToString())
						)
					{
						R.UseAbility(v);
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
					if (W != null && v != null && W.CanBeCasted() && me.Distance2D(v) <= W.GetCastRange() + 30
						&& !v.IsLinkensProtected()
						&&
						(
							v.HasModifier("modifier_meepo_earthbind")
							|| v.HasModifier("modifier_pudge_dismember")
							|| v.HasModifier("modifier_naga_siren_ensnare")
							|| v.HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
							|| v.HasModifier("modifier_legion_commander_duel")
							|| v.HasModifier("modifier_kunkka_torrent")
							|| v.HasModifier("modifier_ice_blast")
							|| v.HasModifier("modifier_enigma_black_hole_pull")
							|| v.HasModifier("modifier_ember_spirit_searing_chains")
							|| v.HasModifier("modifier_dark_troll_warlord_ensnare")
							|| v.HasModifier("modifier_crystal_maiden_frostbite")
							|| v.HasModifier("modifier_axe_berserkers_call")
							|| v.HasModifier("modifier_bane_fiends_grip")
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_Magnataur &&
							v.FindSpell("magnataur_reverse_polarity").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_Magnataur &&
							v.FindSpell("magnataur_skewer").IsInAbilityPhase
							|| v.FindItem("item_blink").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_QueenOfPain &&
							v.FindSpell("queenofpain_blink").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage && v.FindSpell("antimage_blink").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_AntiMage &&
							v.FindSpell("antimage_mana_void").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_DoomBringer &&
							v.FindSpell("doom_bringer_doom").IsInAbilityPhase
							|| v.HasModifier("modifier_rubick_telekinesis")
							|| v.HasModifier("modifier_item_blink_dagger")
							|| v.HasModifier("modifier_storm_spirit_electric_vortex_pull")
							|| v.HasModifier("modifier_winter_wyvern_cold_embrace")
							|| v.HasModifier("modifier_winter_wyvern_winters_curse")
							|| v.HasModifier("modifier_shadow_shaman_shackles")
							||
							v.HasModifier("modifier_faceless_void_chronosphere_freeze") &&
							v.ClassID == ClassID.CDOTA_Unit_Hero_FacelessVoid
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_WitchDoctor &&
							v.FindSpell("witch_doctor_death_ward").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_Rattletrap &&
							v.FindSpell("rattletrap_power_cogs").IsInAbilityPhase
							||
							v.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter &&
							v.FindSpell("tidehunter_ravage").IsInAbilityPhase
							&& !v.IsMagicImmune()
							)
						&& !v.HasModifier("modifier_medusa_stone_gaze_stone")
						&& Utils.SleepCheck(v.Handle.ToString()))
					{
						W.UseAbility(v);
						Utils.Sleep(250, v.Handle.ToString());
					}
					if (v.IsLinkensProtected() &&
						(me.IsVisibleToEnemies || Active))
					{
						if (E != null && E.CanBeCasted() && me.Distance2D(v) < E.GetCastRange() &&
							Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(E.Name) &&
							Utils.SleepCheck(v.Handle.ToString()))
						{
							E.UseAbility(v);
							Utils.Sleep(500, v.Handle.ToString());
						}
					}
				}
			}
		}
	}
}