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
    using SharpDX;

    internal class NyxAssassinController : Variables, IHeroController
    {
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu skills = new Menu("Skills: ", "Skills: ");
        private Ability Q, W, R;
        private Item sheep, vail, soul, abyssal, mjollnir, arcane, blink, shiva, dagon, ethereal, cheese, halberd, satanic, mom, medall;
        

        public void Combo()
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

			Active = Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
			if (Active && me.IsAlive)
			{
				e = me.ClosestToMouseTarget(2000);
				if (e == null) return;
				Q = me.Spellbook.SpellQ;

				W = me.Spellbook.SpellW;

				R = me.Spellbook.SpellR;

				// item 
				sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

				satanic = me.FindItem("item_satanic");

				shiva = me.FindItem("item_shivas_guard");

				dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

				arcane = me.FindItem("item_arcane_boots");

				mom = me.FindItem("item_mask_of_madness");

				vail = me.FindItem("item_veil_of_discord");

				medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

				ethereal = me.FindItem("item_ethereal_blade");

				blink = me.FindItem("item_blink");

				soul = me.FindItem("item_soul_ring");

				cheese = me.FindItem("item_cheese");

				halberd = me.FindItem("item_heavens_halberd");

				abyssal = me.FindItem("item_abyssal_blade");

				mjollnir = me.FindItem("item_mjollnir");
				var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
				var linkens = e.IsLinkensProtected();
				var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
				if (e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
                {
					if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
					{
						Orbwalking.Orbwalk(e, 0, 1600, true, true);
					}
					if (
					   R != null
					   && R.CanBeCasted()
					   && !me.HasModifier("modifier_nyx_assassin_vendetta")
					   && me.Distance2D(e) <= 1400
					   && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
					   && Utils.SleepCheck("R")
					   )
					{
						R.UseAbility();
						Utils.Sleep(200, "R");
					}

					if (R != null && (R.IsInAbilityPhase || me.HasModifier("modifier_nyx_assassin_vendetta") || R.IsChanneling)) return;
					if (R == null || !R.CanBeCasted() && !me.HasModifier("modifier_nyx_assassin_vendetta")
						|| !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
                    {
                        if (stoneModif) return;
                        float angle = me.FindAngleBetween(e.Position, true);
                        Vector3 pos = new Vector3((float)(e.Position.X - 100 * Math.Cos(angle)), (float)(e.Position.Y - 100 * Math.Sin(angle)), 0);
                        if (
                            blink != null
                            && Q.CanBeCasted()
                            && me.CanCast()
                            && blink.CanBeCasted()
                            && me.Distance2D(e) >= 300
                            && me.Distance2D(pos) <= 1180
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                            && Utils.SleepCheck("blink")
                            )
                        {
                            blink.UseAbility(pos);
                            Utils.Sleep(250, "blink");
                        }
                        if ( // vail
                            vail != null
                            && vail.CanBeCasted()
                            && me.CanCast()
                            && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                            && !e.IsMagicImmune()
                            && Utils.SleepCheck("vail")
                            && me.Distance2D(e) <= 1500
                            )
                        {
                            vail.UseAbility(e.Position);
                            Utils.Sleep(250, "vail");
                        }

                        if ( // ethereal
                            ethereal != null &&
                            ethereal.CanBeCasted()
                            && (!vail.CanBeCasted()
                                || vail == null)
                            && me.CanCast() &&
                            !linkens &&
                            !e.IsMagicImmune() &&
                            Utils.SleepCheck("ethereal")
                            )
                        {
                            ethereal.UseAbility(e);
                            Utils.Sleep(150, "ethereal");
                        }
						
                        if ((vail == null || !vail.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)) && (ethereal == null || !ethereal.CanBeCasted() || !Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)))
                        {
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
                            if (Q!=null
								&& Q.CanBeCasted()
								&& Q.Cooldown<=0
								&& me.AghanimState() ? me.Distance2D(e) <= 1190 : me.Distance2D(e) <= Q.GetCastRange() - 50
								&& !e.IsMagicImmune()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                && Utils.SleepCheck("Q"))
                            {
                                Q.CastSkillShot(e);
                                Utils.Sleep(100, "Q");
                            }
                            if (W.CanBeCasted()
                                && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
								&& e.Mana >= (me.MaximumMana * 0.2)
								&& me.Position.Distance2D(e.Position) < 800
                                && Utils.SleepCheck("W"))
                            {
                                W.UseAbility(e);
                                Utils.Sleep(100, "W");
                            }

                            if ( // SoulRing Item 
                                soul != null &&
                                me.Health >= (me.MaximumHealth * 0.3) &&
                                me.Mana <= R.ManaCost &&
                                soul.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name))
                            {
                                soul.UseAbility();
                            } // SoulRing Item end

                            if ( // Arcane Boots Item
                                arcane != null &&
                                me.Mana <= Q.ManaCost &&
                                arcane.CanBeCasted()
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name))
                            {
                                arcane.UseAbility();
                            } // Arcane Boots Item end

                            if ( // Shiva Item
                                shiva != null &&
                                shiva.CanBeCasted() &&
                                me.CanCast() &&
                                !e.IsMagicImmune() &&
                                Utils.SleepCheck("shiva") &&
                                me.Distance2D(e) <= 600
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                )
                            {
                                shiva.UseAbility();
                                Utils.Sleep(250, "shiva");
                            } // Shiva Item end

                            if ( // Medall
                                medall != null &&
                                medall.CanBeCasted() &&
                                me.CanCast() &&
                                !e.IsMagicImmune() &&
                                Utils.SleepCheck("Medall")
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
                                && me.Distance2D(e) <= 500
                                )
                            {
                                medall.UseAbility(e);
                                Utils.Sleep(250, "Medall");
                            } // Medall Item end

                            if ( // MOM
                                mom != null &&
                                mom.CanBeCasted() &&
                                me.CanCast() &&
                                Utils.SleepCheck("mom") &&
                                me.Distance2D(e) <= 700
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
                                )
                            {
                                mom.UseAbility();
                                Utils.Sleep(250, "mom");
                            } // MOM Item end

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
                            if ( // Abyssal Blade
                                abyssal != null &&
                                abyssal.CanBeCasted() &&
                                me.CanCast() &&
                                !e.IsMagicImmune() &&
                                Utils.SleepCheck("abyssal") &&
                                me.Distance2D(e) <= 400
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
                                )
                            {
                                abyssal.UseAbility(e);
                                Utils.Sleep(250, "abyssal");
                            } // Abyssal Item end

                            if ( // Hellbard
                                halberd != null &&
                                halberd.CanBeCasted() &&
                                me.CanCast() &&
                                !e.IsMagicImmune() &&
                                Utils.SleepCheck("halberd") &&
                                me.Distance2D(e) <= 700
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
                                )
                            {
                                halberd.UseAbility(e);
                                Utils.Sleep(250, "halberd");
                            } // Hellbard Item end

                            if ( // Dagon
                                me.CanCast()
                                && dagon != null
                                && (ethereal == null
                                    || (e.HasModifier("modifier_item_ethereal_blade_slow")
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
							
                            if ( // Mjollnir
                                mjollnir != null &&
                                mjollnir.CanBeCasted() &&
                                me.CanCast() &&
                                !e.IsMagicImmune() &&
                                Utils.SleepCheck("mjollnir")
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
                                && me.Distance2D(e) <= 900
                                )
                            {
                                mjollnir.UseAbility(me);
                                Utils.Sleep(250, "mjollnir");
                            } // Mjollnir Item end
							
                            if ( // Satanic 
                                satanic != null &&
                                me.Health <= (me.MaximumHealth * 0.3) &&
                                satanic.CanBeCasted() &&
                                me.Distance2D(e) <= 700 &&
                                Utils.SleepCheck("Satanic")
                                && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
                                )
                            {
                                satanic.UseAbility();
                                Utils.Sleep(350, "Satanic");
                            } // Satanic Item end
                        }
                    }
                }
            }
	        if (Menu.Item("Kill").GetValue<bool>() && me.IsAlive && (me.IsVisibleToEnemies || !me.IsInvisible()))
	        {
		        var enemies =
			        ObjectManager.GetEntities<Hero>()
				        .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();
		        if (enemies.Count <= 0) return;
		        foreach (var v in enemies)
		        {
			        if (v == null) return;

					var wM = new[] { 3.5, 4, 4.5, 5 };
					var wDmg = me.TotalIntelligence*wM[W.Level - 1];

			        var damageW = Math.Floor(wDmg*(1 - v.MagicDamageResist));

			        var lens = me.HasModifier("modifier_item_aether_lens");
			        var spellamplymult = 1 + (me.TotalIntelligence/16/100);
			        if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
			        {
				        damageW =
					        Math.Floor(wDmg*
					                   (1 - (0.10 + v.Spellbook.Spell3.Level*0.04))*(1 - v.MagicDamageResist));
			        }

			        if (lens) damageW = damageW*1.08;
			        if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageW = damageW*0.5;
			        if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageW = damageW*1.3;
			        damageW = damageW*spellamplymult;
			        if (damageW > v.Mana)
				        damageW = v.Mana;


					if ( // vail
				        vail != null
				        && vail.CanBeCasted()
				        && W.CanBeCasted()
				        && v.Health <= damageW * 1.25
				        && v.Health >= damageW
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
			        int etherealdamage = (int) (((me.TotalIntelligence*2) + 75));
			        if ( // vail
				        ethereal != null
				        && ethereal.CanBeCasted()
				        && W != null
				        && W.CanBeCasted()
				        && v.Health <= etherealdamage + damageW*1.4
				        && v.Health >= damageW
				        && me.CanCast()
				        && !v.HasModifier("modifier_item_ethereal_blade_slow")
				        && !v.IsMagicImmune()
				        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
				        && me.Distance2D(v) <= ethereal.GetCastRange() + 50
				        && Utils.SleepCheck("ethereal")
				        )
			        {
				        ethereal.UseAbility(v);
				        Utils.Sleep(250, "ethereal");
			        }

					if (W != null && v != null && W.CanBeCasted()
					   && me.AghanimState() ? me.Distance2D(v) <= 1050 : me.Distance2D(v) <= W.GetCastRange() + 50     
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
					   && !v.HasModifier("modifier_necrolyte_reapers_scythe")
					   && !v.HasModifier("modifier_storm_spirit_ball_lightning")
					   && !v.HasModifier("modifier_ember_spirit_fire_remnant")
					   && !v.HasModifier("modifier_nyx_assassin_spiked_carapace")
					   && !v.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
					   && !v.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
					   !v.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
					   && !v.IsMagicImmune()
					   && v.Health < damageW
					   && Utils.SleepCheck(v.Handle.ToString()))
					{
						W.UseAbility(v);
						Utils.Sleep(150, v.Handle.ToString());
						return;
					}
				}
	        }
        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

            Print.LogMessage.Success("My purpose is clear, my targets doomed.");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddSubMenu(items);
            items.AddItem(new MenuItem("Items", "Items").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_orchid", true},
                {"item_bloodthorn", true},
                {"item_ethereal_blade", true},
                {"item_veil_of_discord", true},
                {"item_rod_of_atos", true},
                {"item_sheepstick", true},
                {"item_arcane_boots", true},
                {"item_dagon", true},
                {"item_blink", true},
                {"item_soul_ring", true},
                {"item_medallion_of_courage", true},
                {"item_mask_of_madness", true},
                {"item_abyssal_blade", true},
                {"item_mjollnir", true},
                {"item_satanic", true},
                {"item_solar_crest", true},
                {"item_ghost", true},
                {"item_cheese", true}
            })));
            Menu.AddSubMenu(skills);
            skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"nyx_assassin_impale", true},
                {"nyx_assassin_mana_burn", true},
                {"nyx_assassin_vendetta", true}
            })));
			Menu.AddItem(new MenuItem("Kill", "KillSteal W").SetValue(true)); 

			Console.WriteLine(">Nyx by VickTheRock loaded!");
        }

        public void OnCloseEvent()
        {

        }
    }
}
