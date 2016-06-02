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

    internal class ZuusController : Variables, IHeroController
    {
        private Ability Q, W, R;
        private readonly int[] eDmg = new int[5] { 0, 4, 6, 8, 10 };
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");
        private bool Active;
        private Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
        private int[] rDmg;

		public void Combo()
		{
            target = me.ClosestToMouseTarget(2000);
            if (target == null)
            {
                return;
            }


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

            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            
            var ModifEther = target.HasModifier("modifier_item_ethereal_blade_slow");
            var stoneModif = target.HasModifier("modifier_medusa_stone_gaze_stone");

            if (Game.IsKeyDown(70) && Q.CanBeCasted() && target != null)
            {
                if (
                    Q != null
                    && Q.CanBeCasted()
                    && (target.IsLinkensProtected()
                        || !target.IsLinkensProtected())
                    && me.CanCast()
                    && me.Distance2D(target) < 900
                    && !stoneModif
                    && Utils.SleepCheck("Q")
                    )
                {
                    Q.UseAbility(target);
                    Utils.Sleep(200, "Q");
                }
            }
            if (Active && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
            {
                var noBlade = target.HasModifier("modifier_item_blade_mail_reflect");
                if (target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
                {
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
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && me.Distance2D(target) < 1300
                                    && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility(target.Position);
                                    Utils.Sleep(200, "W");
                                }
                                if (
                                    Q != null
                                    && Q.CanBeCasted()
                                    && (target.IsLinkensProtected()
                                        || !target.IsLinkensProtected())
                                    && me.CanCast()
                                    && me.Distance2D(target) < 1400
                                    && !stoneModif
                                    && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                    && Utils.SleepCheck("Q")
                                    )
                                {
                                    Q.UseAbility(target);
                                    Utils.Sleep(330, "Q");
                                }
                                if (
                                    R != null
                                    && R.CanBeCasted()
                                    && !Q.CanBeCasted()
                                    && !W.CanBeCasted()
                                    && me.CanCast()
                                    && me.Position.Distance2D(target) < 1200
                                    && target.Health <= (target.MaximumHealth * 0.5)
                                    && !stoneModif
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
                                    && me.Health >= (me.MaximumHealth * 0.6)
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
                                    && me.Distance2D(target) <= 1400
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
                    Utils.Sleep(200, "activated");
                }
            }
            A();
        }

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

			Print.LogMessage.Success("Ah these mortals and their futile games. Oh wait! I'm one of them!");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


		    skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"zuus_arc_lightning", true},
			    {"zuus_lightning_bolt", true},
			    {"zuus_thundergods_wrath", true}
			})));
			items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"item_orchid", true},
                { "item_bloodthorn", true},
			    {"item_ethereal_blade", true},
			    {"item_veil_of_discord", true},
			    {"item_rod_of_atos", true},
			    {"item_sheepstick", true},
			    {"item_arcane_boots", true},
			    {"item_shivas_guard",true},
			    {"item_blink", true},
			    {"item_soul_ring", true},
			    {"item_ghost", true},
			    {"item_cheese", true}
			})));
			ult.AddItem(new MenuItem("autoUlt", "AutoUlt").SetValue(true));
			ult.AddItem(new MenuItem("AutoUlt: ", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"zuus_thundergods_wrath", true}
			})));
			items.AddItem(new MenuItem("Link: ", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
			{
			    {"zuus_arc_lightning", true}
			})));
			ult.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(2, 1, 5)));
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
			var enemies =
				ObjectManager.GetEntities<Hero>()
					.Where(x => x.IsVisible && x.IsAlive && x.Team == me.GetEnemyTeam() && !x.IsIllusion).ToList();
            if(enemies.Count<=0)return;
			if (menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
			{
				foreach (var e in enemies)
				{

					if (me.HasItem(ClassID.CDOTA_Item_UltimateScepter))
						rDmg = new int[3] { 440, 540, 640 };
					else
						rDmg = new int[3] { 225, 325, 425 };


					var lens = me.HasModifier("modifier_item_aether_lens");

					var damage = Math.Floor(rDmg[R.Level - 1] * (1 - e.MagicDamageResist));
					if (me.Distance2D(e) < 1150)
						damage = damage + eDmg[me.Spellbook.Spell3.Level] * 0.01 * e.Health * (1 - e.MagicDamageResist);
					if (e.NetworkName == "CDOTA_Unit_Hero_Spectre" && e.Spellbook.Spell3.Level > 0)
					{
						damage =
							Math.Floor(rDmg[R.Level - 1] *
									   (1 - (0.10 + e.Spellbook.Spell3.Level * 0.04)) * (1 - e.MagicDamageResist));
						if (me.Distance2D(e) < 1150)
							damage = damage + eDmg[me.Spellbook.Spell3.Level] * 0.01 * e.Health * (1 - e.MagicDamageResist);
					}
					if (e.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
						e.Spellbook.SpellR.CanBeCasted())
						damage = 0;
					if (lens) damage = damage * 1.08;
					var rum = e.Modifiers.Any(x => x.Name == "modifier_kunkka_ghost_ship_damage_absorb");
					if (rum) damage = damage * 0.5;
					var mom = e.Modifiers.Any(x => x.Name == "modifier_item_mask_of_madness_berserk");
					if (mom) damage = damage * 1.3;
					if (
						soulring != null
						&& soulring.CanBeCasted()
						&& Utils.SleepCheck(e.Handle.ToString())
						&& me.Mana < R.ManaCost
						&& me.Mana + 150 > R.ManaCost
						)
					{
						soulring.UseAbility();
						Utils.Sleep(150, e.Handle.ToString());
					}
					if (arcane != null
						&& arcane.CanBeCasted()
						&& Utils.SleepCheck(e.Handle.ToString())
						&& me.Mana < R.ManaCost
						&& me.Mana + 135 > R.ManaCost)
					{
						arcane.UseAbility();
						Utils.Sleep(150, e.Handle.ToString());
					}
					if (arcane != null
						&& soulring != null
						&& Utils.SleepCheck(e.Handle.ToString())
						&& arcane.CanBeCasted() && soulring.CanBeCasted()
						&& me.Mana < R.ManaCost
						&& me.Mana + 285 > R.ManaCost)
					{
						arcane.UseAbility();
						soulring.UseAbility();
						Utils.Sleep(150, e.Handle.ToString());
					}

                    if (R != null && e != null && R.CanBeCasted()
						&& !e.HasModifier("modifier_tusk_snowball_movement")
						&& !e.HasModifier("modifier_snowball_movement_friendly")
						&& !e.HasModifier("modifier_templar_assassin_refraction_absorb")
						&& !e.HasModifier("modifier_ember_spirit_flame_guard")
						&& !e.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
						&& !e.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
						&& !e.HasModifier("modifier_puck_phase_shift")
						&& !e.HasModifier("modifier_eul_cyclone")
						&& !e.HasModifier("modifier_dazzle_shallow_grave")
						&& !e.HasModifier("modifier_shadow_demon_disruption")
						&& !e.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !e.HasModifier("modifier_necrolyte_reapers_scythe")
						&& !e.HasModifier("modifier_storm_spirit_ball_lightning")
						&& !e.HasModifier("modifier_ember_spirit_fire_remnant")
						&& !e.HasModifier("modifier_nyx_assassin_spiked_carapace")
						&& !e.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
						&& !e.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
						!e.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
						&& !e.IsMagicImmune()
						&& menu.Item("AutoUlt: ").GetValue<AbilityToggler>().IsEnabled(R.Name)
						&& enemies.Count(x => x.Health <= (damage - 20)) >= (menu.Item("Heel").GetValue<Slider>().Value)
						&& Utils.SleepCheck(e.Handle.ToString()))
					{
						R.UseAbility();
						Utils.Sleep(150, e.Handle.ToString());
						return;
					}

					if (e.IsLinkensProtected() && (me.IsVisibleToEnemies && !me.IsInvisible() || Active))
					{
						if (Q != null && Q.CanBeCasted() && me.Distance2D(e) < Q.CastRange &&
							menu.Item("Link: ").GetValue<AbilityToggler>().IsEnabled(Q.Name) &&
							Utils.SleepCheck(e.Handle.ToString()))
						{
							Q.UseAbility(e);
							Utils.Sleep(500, e.Handle.ToString());
						}
					}
				}
			}
		}
	}
}