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
    using Service;
    using Service.Debug;

    internal class AntimageController : Variables, IHeroController
    {
        private Ability W, R;
        private Item urn, dagon, soul, phase, cheese, halberd, ethereal,
            mjollnir, orchid, abyssal, stick, mom, Shiva, mail, bkb, satanic, medall, blink, sheep, manta;
        
        private readonly double[] ult = { 0, 0.6, 0.85, 1.1 };
        public void Combo()
        {
            if (!Menu.Item("enabled").IsActive())
                return;

            e = me.ClosestToMouseTarget(1800);
            if (e == null)
                return;
            W = me.Spellbook.SpellW;
            R = me.Spellbook.SpellR;
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key);

            Shiva = me.FindItem("item_shivas_guard");
            ethereal = me.FindItem("item_ethereal_blade");
            mom = me.FindItem("item_mask_of_madness");
            urn = me.FindItem("item_urn_of_shadows");
			manta = me.FindItem("item_manta");
			dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            halberd = me.FindItem("item_heavens_halberd");
            mjollnir = me.FindItem("item_mjollnir");
            orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
            abyssal = me.FindItem("item_abyssal_blade");
            mail = me.FindItem("item_blade_mail");
            bkb = me.FindItem("item_black_king_bar");
            satanic = me.FindItem("item_satanic");
            blink = me.FindItem("item_blink");
            medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
            sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
            cheese = me.FindItem("item_cheese");
            soul = me.FindItem("item_soul_ring");
            stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
            phase = me.FindItem("item_phase_boots");
            var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                    .ToList();

	        var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
            if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive)
            {
				if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
				{
					Orbwalking.Orbwalk(e, 0, 1600, true, true);
				}
			}
            if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !me.IsInvisible())
            {
                if (
                       W != null 
                       && W.CanBeCasted() 
                       && me.Distance2D(e) <= W.GetCastRange()-100
                       && me.Distance2D(e) >= me.AttackRange+200
                       && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                       && Utils.SleepCheck("W")
                       )
                {
                    W.UseAbility(Prediction.InFront(e, 230));
                    Utils.Sleep(200, "W");
                }

				if ((manta != null 
					&& Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name)) 
					&& manta.CanBeCasted() && me.IsSilenced() && Utils.SleepCheck("manta"))
				{
					manta.UseAbility();
					Utils.Sleep(400, "manta");
				}
				if ((manta != null && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(manta.Name))
					&& manta.CanBeCasted() && (e.Position.Distance2D(me.Position) <= me.GetAttackRange()+me.HullRadius)
					&& Utils.SleepCheck("manta"))
				{
					manta.UseAbility();
					Utils.Sleep(150, "manta");
				}
				if ( // MOM
                    mom != null
                    && mom.CanBeCasted()
                    && me.CanCast()
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mom.Name)
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
                if ( // Mjollnir
                    mjollnir != null
                    && mjollnir.CanBeCasted()
                    && me.CanCast()
                    && !e.IsMagicImmune()
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
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
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
					&& Utils.SleepCheck("orchid"))
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
                if (phase != null
                    && phase.CanBeCasted()
                    && Utils.SleepCheck("phase")
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(phase.Name)
                    && !blink.CanBeCasted()
                    && me.Distance2D(e) >= me.AttackRange + 20)
                {
                    phase.UseAbility();
                    Utils.Sleep(200, "phase");
                }
                if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
                {
                    urn.UseAbility(e);
                    Utils.Sleep(240, "urn");
                }
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
                    satanic != null 
					&& me.Health <= (me.MaximumHealth * 0.3) 
					&& satanic.CanBeCasted() 
					&& me.Distance2D(e) <= me.AttackRange + 50
                    && Menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
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
            if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
            {
                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };

                R = me.Spellbook.SpellR;
                var ultLvl = R.Level;
                var enemy =
                    ObjectManager.GetEntities<Hero>()
                        .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                        .ToList();
                if (enemy.Count == 0) return;
                foreach (var z in enemy)
                {
                    if (!z.IsVisible || !z.IsAlive) continue;
                    var manna = (z.MaximumMana - z.Mana);
                    var damage = Math.Floor((manna*ult[ultLvl])*(1 - z.MagicDamageResist));

                    var lens = me.HasModifier("modifier_item_aether_lens");

                    if (z.NetworkName == "CDOTA_Unit_Hero_Spectre" && z.Spellbook.Spell3.Level > 0)
                    {
                        damage =
                            Math.Floor((manna*ult[ultLvl])*
                                       (1 - (0.10 + z.Spellbook.Spell3.Level*0.04))*(1 - z.MagicDamageResist));
                    }
                    if (z.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
                        z.Spellbook.SpellR.CanBeCasted())
                        damage = 0;
                    if (lens) damage = damage*1.08;
                    if (z.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damage = damage*0.5;
                    if (z.HasModifier("modifier_item_mask_of_madness_berserk")) damage = damage*1.3;

                    if (z.HasModifier("modifier_chen_penitence"))
                        damage = damage*
                                 penitence[
                                     ObjectManager.GetEntities<Hero>()
                                         .FirstOrDefault(
                                             x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen)
                                         .Spellbook.Spell1.Level];

                    if (z.HasModifier("modifier_shadow_demon_soul_catcher"))
                        damage = damage*
                                 souls[
                                     ObjectManager.GetEntities<Hero>()
                                         .FirstOrDefault(
                                             x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon)
                                         .Spellbook.Spell2.Level];
                    
                    if (R != null && R.CanBeCasted()
                        && !z.HasModifier("modifier_tusk_snowball_movement")
                        && !z.HasModifier("modifier_snowball_movement_friendly")
                        && !z.HasModifier("modifier_templar_assassin_refraction_absorb")
                        && !z.HasModifier("modifier_ember_spirit_flame_guard")
                        && !z.HasModifier("modifier_ember_spirit_sleight_of_fist_caster_invulnerability")
                        && !z.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                        && !z.HasModifier("modifier_puck_phase_shift")
                        && !z.HasModifier("modifier_eul_cyclone")
                        && !z.HasModifier("modifier_dazzle_shallow_grave")
                        && !z.HasModifier("modifier_shadow_demon_disruption")
                        && !z.HasModifier("modifier_necrolyte_reapers_scythe")
                        && !z.HasModifier("modifier_medusa_stone_gaze_stone")
                        && !z.HasModifier("modifier_storm_spirit_ball_lightning")
                        && !z.HasModifier("modifier_ember_spirit_fire_remnant")
                        && !z.HasModifier("modifier_nyx_assassin_spiked_carapace")
                        && !z.HasModifier("modifier_phantom_lancer_doppelwalk_phase")
                        && !z.FindSpell("abaddon_borrowed_time").CanBeCasted() &&
                        !z.HasModifier("modifier_abaddon_borrowed_time_damage_redirect")
                        && me.Distance2D(z) <= R.GetCastRange() + 50
                        && !z.IsMagicImmune()
                        && enemy.Count(x => (x.Health - damage) <= 0 && x.Distance2D(z) <= 500)
                        >= Menu.Item("ulti").GetValue<Slider>().Value
                        && enemy.Count(x => x.Distance2D(z) <= 500)
                        >= Menu.Item("ulti").GetValue<Slider>().Value
                        && damage >= Menu.Item("minDMG").GetValue<Slider>().Value
                        && Utils.SleepCheck(z.Handle.ToString()))
                    {
                        R.UseAbility(z);
                        Utils.Sleep(150, z.Handle.ToString());
                        return;
                    }
                }
            }
        } // Combo

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

            Print.LogMessage.Success("Blood!");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            var ul = new Menu("AutoUlt", "ult");
            //Menu.AddItem(new MenuItem("hpdraw", "Draw HP Bar").SetValue(true).SetTooltip("Will show ulti damage on HP Bar"));
            Menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"antimage_blink", true},
                    {"antimage_mana_void", true}
                })));
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_ethereal_blade", true},
                    {"item_blink", true},
                    {"item_heavens_halberd", true},
                    {"item_orchid", true}, {"item_bloodthorn", true},
                    {"item_urn_of_shadows", true},
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
                   {"item_sheepstick", true},
                   {"item_cheese", true},
                   {"item_soul_ring", true},
                   {"item_arcane_boots", true},
                   {"item_magic_stick", true},
                   {"item_magic_wand", true},
                   {"item_mjollnir", true},
                   {"item_satanic", true},
                   {"item_phase_boots", true},
				   {"item_manta", true}
			   })));
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(ul);
			ul.AddItem(new MenuItem("minDMG", "Min Damage to Ult").SetValue(new Slider(200, 100, 1000)));
			ul.AddItem(new MenuItem("autoUlt", "EnableAutoUlt").SetValue(true));
            ul.AddItem(new MenuItem("ulti", "Min targets to ult").SetValue(new Slider(2, 1, 5)));

            Drawing.OnDraw += DrawUltiDamage;
        }
        private void DrawUltiDamage(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
            {
                return;
            }

            double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
            double[] soul = { 0, 1.2, 1.3, 1.4, 1.5 };

            R = me.Spellbook.SpellR;
            var ultLvl = R.Level;
            var enemy =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                    .ToList();
            if (enemy.Count == 0) return;
            foreach (var v in enemy)
            {
                if (!v.IsVisible || !v.IsAlive) continue;
                var manna = (v.MaximumMana - v.Mana);
                var damage = Math.Floor((manna * ult[ultLvl]) * (1 - v.MagicDamageResist));

                var lens = me.HasModifier("modifier_item_aether_lens");

                if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                {
                    damage =
                        Math.Floor((manna * ult[ultLvl]) *
                                   (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)) * (1 - v.MagicDamageResist));
                }
                if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
                    v.Spellbook.SpellR.CanBeCasted())
                    damage = 0;
                if (lens) damage = damage * 1.08;
                if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damage = damage * 0.5;
                if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damage = damage * 1.3;

                if (v.HasModifier("modifier_chen_penitence"))
                    damage = damage * penitence[ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen).Spellbook.Spell1.Level];

                if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    damage = damage * soul[ObjectManager.GetEntities<Hero>().FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon).Spellbook.Spell2.Level];
                
                var dmg = v.Health - damage;
                var canKill = dmg <= 0;
                var screenPos = HUDInfo.GetHPbarPosition(v);
                if (!OnScreen(v.Position)) continue;

                var text = canKill ? "Yes:" + Math.Floor(damage) : "No:" + Math.Floor(damage);
                var size = new Vector2(15, 15);
                var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                var position = new Vector2(screenPos.X - textSize.X - 2, screenPos.Y - 3);
                Drawing.DrawText(
                    text,
                    position,
                    size,
                    (canKill ? Color.LawnGreen : Color.Red),
                    FontFlags.AntiAlias);
            }
        }
        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }
        
        public void OnCloseEvent()
        {
            Drawing.OnDraw -= DrawUltiDamage;
        }
    }
}