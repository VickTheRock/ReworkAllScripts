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

	internal class OracleController : Variables, IHeroController
	{
		private Ability Q, W, E, R;
		private Item urn, ethereal, dagon, halberd, mjollnir, orchid, abyssal, mom, Shiva, mail, bkb, satanic, medall, glimmer, manta, pipe, guardian, sphere;

        

		public void Combo()
		{
			if (!Menu.Item("enabled").IsActive())
				return;
			Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

			Q = me.Spellbook.SpellQ;
			W = me.Spellbook.SpellW;
			E = me.Spellbook.SpellE;
			R = me.Spellbook.SpellR;

			mom = me.FindItem("item_mask_of_madness");
			glimmer = me.FindItem("item_glimmer_cape");
			manta = me.FindItem("item_manta");
			pipe = me.FindItem("item_pipe");
			guardian = me.FindItem("item_guardian_greaves") ?? me.FindItem("item_mekansm");
			sphere = me.FindItem("item_sphere");
			dagon = me.Inventory.Items.FirstOrDefault(x => x.Name.Contains("item_dagon"));
			ethereal = me.FindItem("item_ethereal_blade");
			halberd = me.FindItem("item_heavens_halberd");
			mjollnir = me.FindItem("item_mjollnir");
			orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
			abyssal = me.FindItem("item_abyssal_blade");
			mail = me.FindItem("item_blade_mail");
			bkb = me.FindItem("item_black_king_bar");
			satanic = me.FindItem("item_satanic");
			medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
			Shiva = me.FindItem("item_shivas_guard");

			var modifInv =
				me.Modifiers.All(
					x =>
						x.Name == "modifier_item_silver_edge_windwalk" || x.Name == "modifier_item_edge_windwalk" ||
						x.Name == "modifier_treant_natures_guise" || x.Name == "modifier_rune_invis");
			var v =
				   ObjectManager.GetEntities<Hero>()
					   .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					   .ToList();
			
			e = me.ClosestToMouseTarget(1800);
			A();
			if (e == null) return;
			if (Active && me.Distance2D(e) <= 1400 && e.IsAlive && !modifInv)
            {
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
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
				if ( // MOM
					Q != null
					&& Q.CanBeCasted()
					&& (!E.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name))
					&& me.CanCast()
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
					&& !e.HasModifier("oracle_fates_edict")
					&& me.Distance2D(e) <= Q.CastRange + 400
					&& Utils.SleepCheck("Q")
					)
				{
					Q.UseAbility(e);
					Utils.Sleep(250, "Q");
				}
				if ( // MOM
					E != null
					&& E.CanBeCasted()
					&& me.CanCast()
					&& !e.HasModifier("oracle_fates_edict")
					&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
					&& Utils.SleepCheck("E")
					&& me.Distance2D(e) <= E.CastRange + 400
					)
				{
					E.UseAbility(e);
					Utils.Sleep(250, "E");
				}
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
				if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900
					&& !e.HasModifier("oracle_fates_edict")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck("orchid"))
				{
					orchid.UseAbility(e);
					Utils.Sleep(100, "orchid");
				}

				if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
					&& !e.HasModifier("oracle_fates_edict")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
					&& !e.IsMagicImmune() && Utils.SleepCheck("Shiva"))
				{
					Shiva.UseAbility();
					Utils.Sleep(100, "Shiva");
				}

				if (ethereal != null && ethereal.CanBeCasted()
					&& me.Distance2D(e) <= 700 && me.Distance2D(e) <= 400
					&& !e.HasModifier("oracle_fates_edict")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) &&
					Utils.SleepCheck("ethereal"))
				{
					ethereal.UseAbility(e);
					Utils.Sleep(100, "ethereal");
				}


				if ( // Dagon
					me.CanCast()
					&& dagon != null
					&& (ethereal == null
						|| (me.HasModifier("modifier_item_ethereal_blade_slow")
							|| ethereal.Cooldown < 17))
					&& !e.IsLinkensProtected()
					&& dagon.CanBeCasted()
					&& !e.HasModifier("oracle_fates_edict")
					&& !e.IsMagicImmune()
					&& Utils.SleepCheck("dagon")
					)
				{
					dagon.UseAbility(e);
					Utils.Sleep(200, "dagon");
				} // Dagon Item end
				if ( // Abyssal Blade
					abyssal != null
					&& abyssal.CanBeCasted()
					&& me.CanCast()
					&& !e.IsStunned()
					&& !e.IsHexed()
					&& Utils.SleepCheck("abyssal")
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
					&& me.Distance2D(e) <= 400
					)
				{
					abyssal.UseAbility(e);
					Utils.Sleep(250, "abyssal");
				} // Abyssal Item end
				if (urn != null && urn.CanBeCasted() && !e.HasModifier("oracle_fates_edict") && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
				{
					urn.UseAbility(e);
					Utils.Sleep(240, "urn");
				}
				if ( // Hellbard
					halberd != null
					&& halberd.CanBeCasted()
					&& me.CanCast()
					&& !e.IsMagicImmune()
					&& !e.HasModifier("oracle_fates_edict")
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
			}
		}

		private void A()
		{
			var ally = ObjectManager.GetEntities<Hero>()
									.Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();
			var v =
				   ObjectManager.GetEntities<Hero>()
					   .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
					   .ToList();
			var countAlly = ally.Count();
			var countV = v.Count();
			for (int i = 0; i < countAlly; ++i)
			{
				if (countV <= 0) return;
				for (int z = 0; z < countV; ++z)
				{

					if (!me.IsInvisible())
					{
						/*if (
							W != null && W.CanBeCasted() && me.Distance2D(Ally[i]) <= W.CastRange+50
							&& Ally[i].Health <= (me.MaximumHealth * 0.6) && !Q.CanBeCasted()
							&& Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("W")
							)
						{
							W.UseAbility(Ally[i]);
							Utils.Sleep(200, "W");
						}
						*/
						if (
							W != null && W.CanBeCasted()
							&& !v[z].IsMagicImmune()
							&& me.Distance2D(v[z]) <= v[z].AttackRange + v[z].HullRadius + 50
							&& v[z].NetworkActivity == NetworkActivity.Attack
							&& v[z].Level >= 8
							&& Menu.Item("SkillsAutoTarget").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("W")
							)
						{
							W.UseAbility(v[z]);
							Utils.Sleep(200, "W");
						}

						if (E != null && E.CanBeCasted()
						   && me.Distance2D(ally[i]) <= E.CastRange + 50
						   && !ally[i].IsMagicImmune()
						   && ally[i].HasModifier("modifier_oracle_false_promise")
						   && Menu.Item("SkillsAutoAlly").GetValue<AbilityToggler>().IsEnabled(E.Name)
						   && Utils.SleepCheck("E")
						   )
						{
							E.UseAbility(ally[i]);
							Utils.Sleep(150, "E");
						}

						if (
							guardian != null && guardian.CanBeCasted()
							&& me.Distance2D(ally[i]) <= guardian.CastRange
							&& (v.Count(x => x.Distance2D(me) <= guardian.CastRange) >= (Menu.Item("healsetTarget").GetValue<Slider>().Value))
							&& (ally.Count(x => x.Distance2D(me) <= guardian.CastRange) >= (Menu.Item("healsetAlly").GetValue<Slider>().Value))
							&& ally[i].Health <= (ally[i].MaximumHealth / 100 * (Menu.Item("HealhHeal").GetValue<Slider>().Value))
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(guardian.Name)
							&& Utils.SleepCheck("guardian")
							)
						{
							guardian.UseAbility();
							Utils.Sleep(200, "guardian");
						}

						if (
							R != null && R.CanBeCasted()
							&& me.Distance2D(ally[i]) <= R.CastRange + 100
							&& ally[i].Health <= (ally[i].MaximumHealth / 100 * (Menu.Item("HealhHealUlt").GetValue<Slider>().Value))
							&& ally[i].Distance2D(v[z]) <= 700
							&& Menu.Item("SkillsAutoAlly").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& Utils.SleepCheck("R")
							)
						{
							R.UseAbility(ally[i]);
							Utils.Sleep(200, "R");
						}

						uint[] eDmg = { 0, 90, 180, 270, 360 };

						var damage = Math.Floor(eDmg[E.Level] * (1 - v[z].MagicDamageResist));
						var lens = me.HasModifier("modifier_item_aether_lens");
						var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
						if (v[z].NetworkName == "CDOTA_Unit_Hero_Spectre" && v[z].Spellbook.Spell3.Level > 0)
						{
							damage =
								Math.Floor(eDmg[E.Level - 1] *
										   (1 - (0.10 + v[z].Spellbook.Spell3.Level * 0.04)) * (1 - v[z].MagicDamageResist));
						}
						if (lens) damage = damage * 1.08;
						if (v[z].HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damage = damage * 0.5;
						if (v[z].HasModifier("modifier_item_mask_of_madness_berserk")) damage = damage * 1.3;
						damage = damage * spellamplymult;

						if (E != null && E.CanBeCasted()
							&& !v[z].HasModifier("modifier_tusk_snowball_movement")
							&& !v[z].HasModifier("modifier_snowball_movement_friendly")
							&& !v[z].HasModifier("modifier_templar_assassin_refraction_absorb")
							&& !v[z].HasModifier("modifier_ember_spirit_flame_guard")
							&& !v[z].HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
							&& !v[z].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
							&& !v[z].HasModifier("modifier_puck_phase_shift")
							&& !v[z].HasModifier("modifier_eul_cyclone")
							&& !v[z].HasModifier("modifier_dazzle_shallow_grave")
							&& !v[z].HasModifier("modifier_shadow_demon_disruption")
							&& !v[z].HasModifier("modifier_necrolyte_reapers_scythe")
							&& !v[z].HasModifier("modifier_necrolyte_reapers_scythe")
							&& !v[z].HasModifier("modifier_storm_spirit_ball_lightning")
							&& !v[z].HasModifier("modifier_ember_spirit_fire_remnant")
							&& !v[z].HasModifier("modifier_nyx_assassin_spiked_carapace")
							&& !v[z].HasModifier("modifier_phantom_lancer_doppelwalk_phase")
							&& !v[z].HasModifier("oracle_fates_edict")
							&& !v[z].HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
							&& !v[z].IsMagicImmune()
							&& Menu.Item("SkillsAutoTarget").GetValue<AbilityToggler>().IsEnabled(E.Name)
							&& v[z].Health <= damage - 5
							&& me.Distance2D(v[z]) <= E.CastRange + 10
							&& Utils.SleepCheck(e.Handle.ToString()))
						{
							E.UseAbility(v[z]);
							Utils.Sleep(150, e.Handle.ToString());
						}


						if (W != null && W.CanBeCasted()
							&& me.Distance2D(ally[i]) <= W.CastRange + 50
							&& !ally[i].IsMagicImmune()
							&& ally[i].HasModifier("modifier_oracle_false_promise")
							&& Menu.Item("SkillsAutoAlly").GetValue<AbilityToggler>().IsEnabled(W.Name)
							&& Utils.SleepCheck("W")
							)
						{
							W.UseAbility(ally[i]);
							Utils.Sleep(200, "W");
						}
						var spray = ally[i].Modifiers.FirstOrDefault(y => y.Name == "modifier_bristleback_quill_spray_stack");
						var napalm = ally[i].Modifiers.FirstOrDefault(y => y.Name == "modifier_batrider_sticky_napalm");

						if (Q != null && Q.CanBeCasted() && me.Distance2D(ally[i]) <= Q.CastRange + 50
							&& (ally[i].IsSilenced()
							|| ally[i].IsHexed()
							|| ally[i].HasModifier("modifier_item_diffusal_blade")
							|| ally[i].HasModifier("modifier_slardar_amplify_damage")
							|| ally[i].HasModifier("modifier_invoker_cold_snap_freeze")
							|| ally[i].HasModifier("modifier_item_urn_damage")
							|| ally[i].HasModifier("modifier_rod_of_atos_debuff")
							|| ally[i].HasModifier("modifier_brewmaster_thunder_clap")
							|| ally[i].HasModifier("modifier_brewmaster_drunken_haze")
							|| ally[i].HasModifier("modifier_ursa_earthshock")
							|| ally[i].HasModifier("modifier_night_stalker_void")
							|| ally[i].HasModifier("modifier_ogre_magi_ignite")
							|| (spray != null && spray.StackCount >= 3)
							|| (napalm != null && napalm.StackCount >= 4)
							)
							&& !ally[i].HasModifier("modifier_legion_commander_duel")
							&& !ally[i].HasModifier("modifier_riki_smoke_screen")
							&& !ally[i].HasModifier("modifier_disruptor_static_storm")
							&& !ally[i].IsMagicImmune()
							&& Menu.Item("SkillsAutoAlly").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& Utils.SleepCheck("Q")
							)
						{
							Q.UseAbility(ally[i]);
							me.Stop();
							Utils.Sleep(200, "Q");
						}


						if (Q != null && Q.CanBeCasted() && me.Distance2D(v[z]) <= Q.CastRange + 50 &&
							Menu.Item("SkillsAutoTarget").GetValue<AbilityToggler>().IsEnabled(Q.Name)
							&& (v[z].HasModifier("modifier_rune_haste")
							|| v[z].HasModifier("modifier_rune_regen")
							|| v[z].HasModifier("modifier_rune_doubledamage")
							|| v[z].HasModifier("modifier_legion_commander_press_the_attack")
							|| v[z].HasModifier("modifier_ogre_magi_bloodlust")
							)
							&& !v[z].IsMagicImmune()
							&& Utils.SleepCheck("Q")
							)
						{
							Q.UseAbility(v[z]);
							me.Stop();
							Utils.Sleep(200, "Q");
						}
						var modi = ally[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_stunned");
						var mod = ally[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_stun");
						if (R != null && R.CanBeCasted() && me.Distance2D(ally[i]) <= R.CastRange + 50
							&& ((mod != null && mod.RemainingTime >= 1.6 + Game.Ping)
							|| (modi != null && mod.RemainingTime >= 1.6 + Game.Ping)
							|| ally[i].HasModifier("modifier_batrider_flaming_lasso")
							)
							&& Menu.Item("SkillsAutoAlly").GetValue<AbilityToggler>().IsEnabled(R.Name)
							&& Utils.SleepCheck("R")
							)
						{
							R.UseAbility(ally[i]);
							Utils.Sleep(200, "R");
						}
						if (
						   manta != null && manta.CanBeCasted()
						   && (me.Distance2D(v[z]) <= me.AttackRange + me.HullRadius + 10)
						   || (me.Distance2D(v[z]) <= v[z].AttackRange + me.HullRadius + 10)
						   && Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(manta.Name)
						   && Utils.SleepCheck("manta")
						   )
						{
							manta.UseAbility();
							Utils.Sleep(200, "manta");
						}
						if (
						 pipe != null && pipe.CanBeCasted()
						 && me.Distance2D(ally[i]) <= pipe.CastRange
						 && (v.Count(x => x.Distance2D(me) <= pipe.CastRange) >= (Menu.Item("pipesetTarget").GetValue<Slider>().Value))
						 && (ally.Count(x => x.Distance2D(me) <= pipe.CastRange) >= (Menu.Item("pipesetAlly").GetValue<Slider>().Value))
						 && Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(pipe.Name)
						 && Utils.SleepCheck("pipe")
						 )
						{
							pipe.UseAbility();
							Utils.Sleep(200, "pipe");
						}

						if (
							sphere != null && sphere.CanBeCasted() && me.Distance2D(ally[i]) <= sphere.CastRange + 50
							&& !ally[i].IsMagicImmune()
							&& ((ally[i].Distance2D(v[z]) <= ally[i].AttackRange + ally[i].HullRadius + 10)
							|| (ally[i].Distance2D(v[z]) <= v[z].AttackRange + ally[i].HullRadius + 10)
							|| ally[i].Health <= (me.MaximumHealth * 0.5))
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(sphere.Name)
							&& Utils.SleepCheck("sphere")
							)
						{
							sphere.UseAbility(ally[i]);
							Utils.Sleep(200, "sphere");
						}

						if (
							glimmer != null && glimmer.CanBeCasted() && me.Distance2D(ally[i]) <= glimmer.CastRange + 50
							&& ally[i].Health <= (me.MaximumHealth * 0.5)
							&& Menu.Item("ItemsS").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
							&& Utils.SleepCheck("glimmer")
							)
						{
							glimmer.UseAbility(ally[i]);
							Utils.Sleep(200, "glimmer");
						}
						if (W != null && W.CanBeCasted() && me.Distance2D(v[z]) <= W.CastRange + 50
						   && !v[z].IsMagicImmune() &&
						   (v[z].HasModifier("modifier_sven_gods_strength")
						   || v[z].HasModifier("modifier_rune_doubledamage")
						   || ((v[z].ClassID == ClassID.CDOTA_Unit_Hero_Legion_Commander && v[z].FindSpell("legion_commander_duel").Cooldown <= 0 && ally[i].Distance2D(v[z]) <= 500)
						   || (v[z].ClassID == ClassID.CDOTA_Unit_Hero_Sniper && v[z].Level >= 8)
						   || (v[z].ClassID == ClassID.CDOTA_Unit_Hero_DrowRanger && v[z].Level >= 8)
						   || (v[z].ClassID == ClassID.CDOTA_Unit_Hero_Ursa && v[z].Distance2D(ally[i]) <= 300 && v[z].NetworkActivity == NetworkActivity.Attack)
						   || (v[z].ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin && v[z].Distance2D(ally[i]) <= 300 && v[z].IsAttacking()))
						   )
						   && Menu.Item("SkillsAutoTarget").GetValue<AbilityToggler>().IsEnabled(W.Name)
						   && Utils.SleepCheck("W")
						   )
						{
							W.UseAbility(v[z]);
							Utils.Sleep(200, "W");
						}
					}
				}
			}
		}
		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

			Print.LogMessage.Success("I Safe All Life, My Freand's!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

		    Menu items = new Menu("Items And Spel's Combo", "Items");
			Menu heal = new Menu("Healing Items Settings", "Heal");
			Menu spell = new Menu("Auto Use Spell Q+W+E+R Logic", "AutoSpell");
			Menu ally = new Menu("Auto Healing | Purge Logic", "Autoally");
			Menu enemy = new Menu("Auto Debuf | KillSteal Q+W+E Logic", "Autoenemy");
			items.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"oracle_purifying_flames", true},
				    {"oracle_fortunes_end", true},
				    {"oracle_fates_edict", true},
				    {"oracle_false_promise", true}
				})));
			items.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_mask_of_madness", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
					{"item_bloodthorn", true},
				    {"item_mjollnir", true},
				    {"item_urn_of_shadows", true},
				    {"item_ethereal_blade", true},
				    {"item_abyssal_blade", true},
				    {"item_shivas_guard", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_satanic", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			items.AddItem(
				new MenuItem("ItemsS", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_manta", true},
				    {"item_mekansm", true},
				    {"item_pipe", true},
				    {"item_guardian_greaves", true},
				    {"item_sphere", true},
				    {"item_glimmer_cape", true}
				})));
			Menu.AddItem(new MenuItem("Heel", "Min Target's to BKB").SetValue(new Slider(2, 1, 5)));
			Menu.AddItem(new MenuItem("Heelm", "Min Target's to BladeMail").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("pipesetTarget", "Min Target's to Pipe").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("pipesetAlly", "Min Ally to Pipe").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("healsetTarget", "Min Target's to Meka | Guardian").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("healsetAlly", "Min Ally to Meka | Guardian").SetValue(new Slider(2, 1, 5)));
			heal.AddItem(new MenuItem("HealhHeal", "Min healh % to item's Heal").SetValue(new Slider(35, 10, 70))); // x/ 10%
			ally.AddItem(
				new MenuItem("SkillsAutoAlly", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"oracle_purifying_flames", true},
				    {"oracle_fortunes_end", true},
				    {"oracle_fates_edict", true},
				    {"oracle_false_promise", true}
				})));
			ally.AddItem(new MenuItem("HealhHealUlt", "Min Ally Healh % to Ult").SetValue(new Slider(35, 10, 70)));
			enemy.AddItem(
				new MenuItem("SkillsAutoTarget", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"oracle_purifying_flames", true},
				    {"oracle_fortunes_end", true},
				    {"oracle_fates_edict", true},
				})));

			Menu.AddSubMenu(items);
			Menu.AddSubMenu(heal);
			Menu.AddSubMenu(spell);
			spell.AddSubMenu(ally);
			spell.AddSubMenu(enemy);
		}

		public void OnCloseEvent()
		{
			
		}
	}
}
 