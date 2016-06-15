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


    internal class SkywrathMageController : Variables, IHeroController
    {
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");
        private readonly Menu healh = new Menu("Healh", "Min % Enemy Healh to Ult");
        private bool Active;

        private Ability Q, W, E, R;

        private Item orchid, sheep, vail, soulring, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost, core, force, cyclone;
        public void OnLoadEvent()
        {

            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

            Print.LogMessage.Success("I am sworn to turn the tide where ere I can.");

            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));


            skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"skywrath_mage_arcane_bolt", true},
                {"skywrath_mage_concussive_shot", true},
                {"skywrath_mage_mystic_flare", true}
            })));
            items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon",true},
                {"item_orchid", true},
                {"item_bloodthorn", true},
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
            ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"skywrath_mage_mystic_flare", true}
            })));
            items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_force_staff", true},
                {"item_cyclone", true},
                {"item_orchid", true}, {"item_bloodthorn", true},
                {"item_rod_of_atos", true},
                {"item_dagon", true}
            })));
            healh.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 10, 70))); // x/ 10%
            menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(false));
            menu.AddSubMenu(skills);
            menu.AddSubMenu(items);
            menu.AddSubMenu(ult);
            menu.AddSubMenu(healh);
        } // OnLoadEvent

        public void OnCloseEvent()
        {
            target = null;
        }

        /* Доп. функции скрипта
		-----------------------------------------------------------------------------*/


        public void Combo()
        {
            target = me.ClosestToMouseTarget(2000);
            if (target.HasModifier("modifier_abaddon_borrowed_time")
            || target.HasModifier("modifier_item_blade_mail_reflect")
            || target.IsMagicImmune())
            {
                var enemies = ObjectManager.GetEntities<Hero>()
                        .Where(x => x.IsAlive && x.Team != me.Team && !x.IsIllusion && !x.IsMagicImmune()
                        && !x.Modifiers.Any(y => y.Name == "modifier_abaddon_borrowed_time"
                        || y.Name == "modifier_item_blade_mail_reflect")
                        && x.Distance2D(target) > 200).ToList();
                target = GetClosestToTarget(enemies, target) ?? null;
                if (Utils.SleepCheck("spam"))
                {
                    Utils.Sleep(5000, "spam");
                }
            }
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

            orchid = me.FindItem("item_orchid");

            atos = me.FindItem("item_rod_of_atos");

            soulring = me.FindItem("item_soul_ring");

            arcane = me.FindItem("item_arcane_boots");

            blink = me.FindItem("item_blink");

            shiva = me.FindItem("item_shivas_guard");

            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));


            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);


            var stoneModif = target.HasModifier("modifier_medusa_stone_gaze_stone");

            if (Game.IsKeyDown(keyCode: 70) && Q.CanBeCasted() && target != null)
            {
                if (
                    Q != null
                    && Q.CanBeCasted()
                    && (target.IsLinkensProtected()
                    || !target.IsLinkensProtected())
                    && me.CanCast()
                    && me.Distance2D(target) < 900
                    && Utils.SleepCheck("Q")
                    )
                {
                    Q.UseAbility(target);
                    Utils.Sleep(200, "Q");
                }
            }
            if (Active && me.IsAlive && target.IsAlive && Utils.SleepCheck("activated"))
            {
                if (stoneModif) return;
                //var noBlade = target.HasModifier("modifier_item_blade_mail_reflect");
                if (target.IsVisible && me.Distance2D(target) <= 2300)
                {
                    if (
                        Q != null
                        && Q.CanBeCasted()
                        && me.CanCast()
                        && !target.IsMagicImmune()
                        && me.Distance2D(target) < 1400
                        && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                        && Utils.SleepCheck("Q")
                        )
                    {
                        Q.UseAbility(target);
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
                        W != null
                        && target.IsVisible
                        && W.CanBeCasted()
                        && me.CanCast()
                        && !target.IsMagicImmune()
                        && me.Distance2D(target) < 900
                        && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                        && Utils.SleepCheck("W"))
                    {
                        W.UseAbility();
                        Utils.Sleep(300, "W");
                    }
                    float angle = me.FindAngleBetween(target.Position, true);
                    Vector3 pos = new Vector3((float)(target.Position.X - 500 * Math.Cos(angle)), (float)(target.Position.Y - 500 * Math.Sin(angle)), 0);
                    if (
                        blink != null
                        && Q.CanBeCasted()
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(target) >= 490
                        && me.Distance2D(pos) <= 1180
                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        blink.UseAbility(pos);
                        Utils.Sleep(250, "blink");
                    }
                    if (
                           E != null
                           && E.CanBeCasted()
                           && me.CanCast()
                           && !target.IsLinkensProtected()
                           && me.Position.Distance2D(target) < 1400
                           && Utils.SleepCheck("E"))
                    {
                        E.UseAbility(target);
                        Utils.Sleep(200, "E");
                    }
                    if (!E.CanBeCasted() || E == null || me.IsSilenced())
                    {
                        if ( // orchid
                            orchid != null
                            && orchid.CanBeCasted()
                            && me.CanCast()
                            && !target.IsLinkensProtected()
                            && !target.IsMagicImmune()
                            && me.Distance2D(target) <= 1400
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
                            && Utils.SleepCheck("orchid")
                            )
                        {
                            orchid.UseAbility(target);
                            Utils.Sleep(250, "orchid");
                        } // orchid Item end
                        if (!orchid.CanBeCasted() || orchid == null || me.IsSilenced() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
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
                            if (!vail.CanBeCasted() || vail == null || me.IsSilenced() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name))
                            {
                                if (// ethereal
                                       ethereal != null
                                       && ethereal.CanBeCasted()
                                       && me.CanCast()
                                       && !target.IsLinkensProtected()
                                       && !target.IsMagicImmune()
                                       && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                       && Utils.SleepCheck("ethereal")
                                      )
                                {
                                    ethereal.UseAbility(target);
                                    Utils.Sleep(200, "ethereal");
                                } // ethereal Item end
                                if (!ethereal.CanBeCasted() || ethereal == null || me.IsSilenced() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                                {
                                    if (
                                         Q != null
                                        && Q.CanBeCasted()
                                        && me.CanCast()
                                        && me.Distance2D(target) < 1400
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
                                       && me.CanCast()
                                       && !target.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                                       && target.MovementSpeed <= 240
                                       && me.Position.Distance2D(target) < 1200
                                       && target.Health >= (target.MaximumHealth / 100 * menu.Item("Healh").GetValue<Slider>().Value)

                                       && !target.HasModifier("modifier_item_blade_mail_reflect")
                                       && !target.HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                                       && !target.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                                       && !target.HasModifier("modifier_puck_phase_shift")
                                       && !target.HasModifier("modifier_eul_cyclone")
                                       && !target.HasModifier("modifier_dazzle_shallow_grave")
                                       && !target.HasModifier("modifier_brewmaster_storm_cyclone")
                                       && !target.HasModifier("modifier_spirit_breaker_charge_of_darkness")
                                       && !target.HasModifier("modifier_shadow_demon_disruption")
                                       && !target.HasModifier("modifier_tusk_snowball_movement")
                                       && !target.IsMagicImmune()
                                       && (!target.FindSpell("abaddon_borrowed_time").CanBeCasted() && !target.HasModifier("modifier_abaddon_borrowed_time_damage_redirect"))

                                       && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                                       && Utils.SleepCheck("R"))
                                    {
                                        R.UseAbility(Prediction.InFront(target, 100));
                                        Utils.Sleep(330, "R");
                                    }

                                    if (// SoulRing Item 
                                        soulring != null
                                        && soulring.CanBeCasted()
                                        && me.CanCast()
                                        && me.Health >= (me.MaximumHealth * 0.5)
                                        && me.Mana <= R.ManaCost
                                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
                                        )
                                    {
                                        soulring.UseAbility();
                                    } // SoulRing Item end

                                    if (// Arcane Boots Item
                                        arcane != null
                                        && arcane.CanBeCasted()
                                        && me.CanCast()
                                        && me.Mana <= R.ManaCost
                                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
                                        )
                                    {
                                        arcane.UseAbility();
                                    } // Arcane Boots Item end

                                    if (//Ghost
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


                                    if (// Shiva Item
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
                                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
                                        && Utils.SleepCheck("sheep")
                                        )
                                    {
                                        sheep.UseAbility(target);
                                        Utils.Sleep(250, "sheep");
                                    } // sheep Item end

                                    if (// Dagon
                                        me.CanCast()
                                        && dagon != null
                                        && (ethereal == null
                                        || (target.HasModifier("modifier_item_ethereal_blade_slow")
                                        || ethereal.Cooldown < 17))
                                        && !target.IsLinkensProtected()
                                        && dagon.CanBeCasted()
                                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                        && !target.IsMagicImmune()
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
                        if (menu.Item("orbwalk").GetValue<bool>())
                        {
                            if (me.Distance2D(target) <= me.AttackRange + 5 &&
                            (!me.IsAttackImmune()
                            || !target.IsAttackImmune()
                            )
                            && me.NetworkActivity != NetworkActivity.Attack
                            && me.CanAttack()
                            && !me.IsAttacking()
                            && Utils.SleepCheck("attack")
                            )
                            {
                                me.Attack(target);
                                Utils.Sleep(180, "attack");
                            }
                            else if (
                                (!me.CanAttack()
                                || me.Distance2D(target) >= 450)
                                && me.NetworkActivity != NetworkActivity.Attack
                                && me.Distance2D(target) <= 2500
                                && !me.IsAttacking()
                                && Utils.SleepCheck("Move")
                                )
                            {
                                me.Move(Prediction.InFront(target, 100));
                                Utils.Sleep(400, "Move");
                            }
                        }
                    }
                }
                Utils.Sleep(100, "activated");
            }
            A();
        } // Combo


        private Hero GetClosestToTarget(List<Hero> units, Hero z)
        {
            Hero closestHero = null;
            foreach (var v in units.Where(v => closestHero == null || closestHero.Distance2D(z) > v.Distance2D(z)))
            {
                closestHero = v;
            }
            return closestHero;
        }

        public void A()
        {
            me = ObjectManager.LocalHero;
            var e =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsAlive && x.IsValid && x.Team != me.Team && !x.IsIllusion).ToList();

            var ecount = e.Count();
            if (ecount == 0) return;
            if (menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
            {
                if (!me.IsAlive) return;
                force = me.FindItem("item_force_staff");
                cyclone = me.FindItem("item_cyclone");
                orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
                atos = me.FindItem("item_rod_of_atos");

                for (int i = 0; i < ecount; ++i)
                {
                    var reflect = e[i].HasModifier("modifier_item_blade_mail_reflect");

                   if (me.HasModifier("modifier_spirit_breaker_charge_of_darkness_vision"))
                    {
                        if (Utils.SleepCheck(e[i].Handle.ToString()))
                        {

                            if (e[i].ClassID == ClassID.CDOTA_Unit_Hero_SpiritBreaker)
                            {
                                float angle = me.FindAngleBetween(e[i].Position, true);
                                Vector3 pos = new Vector3((float) (me.Position.X + 100 *Math.Cos(angle)),
                                    (float) (me.Position.Y + 100 *Math.Sin(angle)), 0);
                                
                                if (W != null && W.CanBeCasted() && me.Distance2D(e[i]) <= 900 + Game.Ping
                                         && !e[i].IsMagicImmune())
                                {
                                    W.UseAbility();
                                }
                                if (atos != null && R != null && R.CanBeCasted() && atos.CanBeCasted()
                                         && me.Distance2D(e[i]) <= 900 + Game.Ping
                                         && !e[i].IsMagicImmune())
                                {
                                    atos.UseAbility(e[i]);
                                }
                                if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= 700 + Game.Ping
                                   && !e[i].HasModifier("modifier_item_blade_mail_reflect")
                                   && !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                                   && !e[i].HasModifier("modifier_dazzle_shallow_grave")
                                   && !e[i].IsMagicImmune()
                                   && menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
                                   )
                                {
                                    R.UseAbility(pos);
                                    //me.Stop();
                                }
                                if (cyclone != null && !R.CanBeCasted() 
                                    && cyclone.CanBeCasted() 
                                    && me.Distance2D(e[i]) <= 500 + Game.Ping)
                                {
                                    cyclone.UseAbility(me);
                                }
                            }
                            Utils.Sleep(150, e[i].Handle.ToString());
                        }
                    }
                    if (Utils.SleepCheck(e[i].Handle.ToString()))
                    {
                         
                        if (cyclone != null && reflect && cyclone.CanBeCasted() &&
                            e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect") &&
                            me.Distance2D(e[i]) < cyclone.CastRange
                            )
                        {
                            cyclone.UseAbility(me);
                        }
                        E = me.Spellbook.SpellE;
                        if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= R.CastRange + 100
                            && (e[i].MovementSpeed <= 200 && !E.CanBeCasted())
                            && !e[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
                            && !e[i].HasModifier("modifier_item_blade_mail_reflect")
                            && !e[i].HasModifier("modifier_sniper_headshot")
                            && !e[i].HasModifier("modifier_leshrac_lightning_storm_slow")
                            && !e[i].HasModifier("modifier_razor_unstablecurrent_slow")
                            && !e[i].HasModifier("modifier_pudge_meat_hook")
                            && !e[i].HasModifier("modifier_tusk_snowball_movement")
                            && !e[i].HasModifier("modifier_faceless_void_time_walk")
                            && !e[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                            && !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                            && !e[i].HasModifier("modifier_puck_phase_shift")
                            && !e[i].HasModifier("modifier_abaddon_borrowed_time")
                            && !e[i].HasModifier("modifier_winter_wyvern_winters_curse")
                            && !e[i].HasModifier("modifier_eul_cyclone")
                            && !e[i].HasModifier("modifier_dazzle_shallow_grave")
                            && !e[i].HasModifier("modifier_brewmaster_storm_cyclone")
                            && !e[i].HasModifier("modifier_mirana_leap")
                            && !e[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
                            && !e[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
                            && !e[i].HasModifier("modifier_shadow_demon_disruption")
                            && ((e[i].FindSpell("abaddon_borrowed_time") != null
                                 && e[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
                                || e[i].FindSpell("abaddon_borrowed_time") == null)
                            && e[i].Health >= (e[i].MaximumHealth / 100 * (menu.Item("Healh").GetValue<Slider>().Value))
                            && !e[i].IsMagicImmune()
                            && menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name))
                        {
                            R.UseAbility(Prediction.InFront(e[i], 90));
                        }
                        if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= R.CastRange + 100
                            &&
                            (e[i].HasModifier("modifier_meepo_earthbind")
                             || e[i].HasModifier("modifier_pudge_dismember")
                             || e[i].HasModifier("modifier_naga_siren_ensnare")
                             || e[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
                             || (e[i].HasModifier("modifier_legion_commander_duel")
                                 && !e[i].AghanimState())
                             || e[i].HasModifier("modifier_kunkka_torrent")
                             || e[i].HasModifier("modifier_ice_blast")
                             || e[i].HasModifier("modifier_crystal_maiden_crystal_nova")
                             || e[i].HasModifier("modifier_enigma_black_hole_pull")
                             || e[i].HasModifier("modifier_ember_spirit_searing_chains")
                             || e[i].HasModifier("modifier_dark_troll_warlord_ensnare")
                             || e[i].HasModifier("modifier_crystal_maiden_frostbite")
                             ||
                             (e[i].FindSpell("rattletrap_power_cogs") != null &&
                              e[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
                             || e[i].HasModifier("modifier_axe_berserkers_call")
                             || e[i].HasModifier("modifier_bane_fiends_grip")
                             || (e[i].HasModifier("modifier_faceless_void_chronosphere_freeze")
                                 && e[i].ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid)
                             || e[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
                             || (e[i].FindSpell("witch_doctor_death_ward") != null
                                 && e[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
                             || (e[i].FindSpell("crystal_maiden_crystal_nova") != null
                                 && e[i].FindSpell("crystal_maiden_crystal_nova").IsInAbilityPhase)
                             || e[i].IsStunned())
                            && (!e[i].HasModifier("modifier_medusa_stone_gaze_stone")
                                && !e[i].HasModifier("modifier_faceless_void_time_walk")
                                && !e[i].HasModifier("modifier_item_monkey_king_bar")
                                && !e[i].HasModifier("modifier_rattletrap_battery_assault")
                                && !e[i].HasModifier("modifier_item_blade_mail_reflect")
                                && !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                                && !e[i].HasModifier("modifier_pudge_meat_hook")
                                && !e[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
                                && !e[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                                && !e[i].HasModifier("modifier_puck_phase_shift")
                                && !e[i].HasModifier("modifier_eul_cyclone")
                                && !e[i].HasModifier("modifier_invoker_tornado")
                                && !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                                && !e[i].HasModifier("modifier_dazzle_shallow_grave")
                                && !e[i].HasModifier("modifier_mirana_leap")
                                && !e[i].HasModifier("modifier_abaddon_borrowed_time")
                                && !e[i].HasModifier("modifier_winter_wyvern_winters_curse")
                                && !e[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
                                && !e[i].HasModifier("modifier_brewmaster_storm_cyclone")
                                && !e[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
                                && !e[i].HasModifier("modifier_shadow_demon_disruption")
                                && !e[i].HasModifier("modifier_tusk_snowball_movement")
                                && !e[i].HasModifier("modifier_invoker_tornado")
                                && ((e[i].FindSpell("abaddon_borrowed_time") != null
                                     && e[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
                                    || e[i].FindSpell("abaddon_borrowed_time") == null)
                                && e[i].Health >= (e[i].MaximumHealth / 100 * (menu.Item("Healh").GetValue<Slider>().Value))
                                && !e[i].IsMagicImmune())
                            && menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name))
                        {
                            R.UseAbility(Prediction.InFront(e[i], 70));
                        }

                        if (R != null && R.CanBeCasted() && me.Distance2D(e[i]) <= R.CastRange + 100
                            && (e[i].MovementSpeed <= 200 && !E.CanBeCasted())
                            && e[i].MagicDamageResist <= 0.06
                            && e[i].Health >= (e[i].MaximumHealth / 100 * (menu.Item("Healh").GetValue<Slider>().Value))
                            && !e[i].HasModifier("modifier_item_blade_mail_reflect")
                            && !e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                            && !e[i].HasModifier("modifier_zuus_lightningbolt_vision_thinker")
                            && !e[i].HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                            && !e[i].HasModifier("modifier_puck_phase_shift")
                            && !e[i].HasModifier("modifier_eul_cyclone")
                            && !e[i].HasModifier("modifier_invoker_tornado")
                            && !e[i].HasModifier("modifier_dazzle_shallow_grave")
                            && !e[i].HasModifier("modifier_brewmaster_storm_cyclone")
                            && !e[i].HasModifier("modifier_spirit_breaker_charge_of_darkness")
                            && !e[i].HasModifier("modifier_shadow_demon_disruption")
                            && !e[i].HasModifier("modifier_faceless_void_time_walk")
                            && !e[i].HasModifier("modifier_winter_wyvern_winters_curse")
                            && !e[i].HasModifier("modifier_mirana_leap")
                            && !e[i].HasModifier("modifier_earth_spirit_rolling_boulder_caster")
                            && !e[i].HasModifier("modifier_tusk_snowball_movement")
                            && !e[i].IsMagicImmune()
                            && !e[i].HasModifier("modifier_abaddon_borrowed_time")
                            && ((e[i].FindSpell("abaddon_borrowed_time") != null
                                 && e[i].FindSpell("abaddon_borrowed_time").Cooldown > 0)
                                || e[i].FindSpell("abaddon_borrowed_time") == null)
                            && menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name))
                        {
                            R.UseAbility(Prediction.InFront(e[i], 90));
                        }
                        if (W != null && W.CanBeCasted() && me.Distance2D(e[i]) <= 1400
                            && (e[i].MovementSpeed <= 255
                                || (e[i].Distance2D(me) <= me.HullRadius + 10
                                    && e[i].NetworkActivity == NetworkActivity.Attack)
                                || e[i].MagicDamageResist <= 0.07)
                            && !e[i].IsMagicImmune())
                        {
                            W.UseAbility();
                        }
                        if (atos != null && R != null && R.CanBeCasted() && atos.CanBeCasted()
                            && !e[i].IsLinkensProtected()
                            && me.Distance2D(e[i]) <= 1200
                            && e[i].MagicDamageResist <= 0.07
                            && !e[i].IsMagicImmune())
                        {
                            atos.UseAbility(e[i]);
                        }
                        if (vail != null && e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                            && vail.CanBeCasted()
                            && me.Distance2D(e[i]) <= 1200
                            )
                        {
                            vail.UseAbility(e[i].Position);
                        }
                        if (E != null && e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                            && E.CanBeCasted()
                            && me.Distance2D(e[i]) <= 900)
                        {
                            E.UseAbility(e[i]);
                        }
                        if (ethereal != null &&
                            e[i].HasModifier("modifier_skywrath_mystic_flare_aura_effect")
                            && !e[i].HasModifier("modifier_legion_commander_duel")
                            && ethereal.CanBeCasted()
                            && E.CanBeCasted()
                            && me.Distance2D(e[i]) <= 1000)
                        {
                            ethereal.UseAbility(e[i]);
                        }
                        if (E != null && E.CanBeCasted() && me.Distance2D(e[i]) <= E.CastRange
                            && !e[i].IsLinkensProtected()
                            &&
                            (e[i].HasModifier("modifier_meepo_earthbind")
                             || e[i].HasModifier("modifier_pudge_dismember")
                             || e[i].HasModifier("modifier_naga_siren_ensnare")
                             || e[i].HasModifier("modifier_lone_druid_spirit_bear_entangle_effect")
                             || e[i].HasModifier("modifier_legion_commander_duel")
                             || e[i].HasModifier("modifier_kunkka_torrent")
                             || e[i].HasModifier("modifier_ice_blast")
                             || e[i].HasModifier("modifier_enigma_black_hole_pull")
                             || e[i].HasModifier("modifier_ember_spirit_searing_chains")
                             || e[i].HasModifier("modifier_dark_troll_warlord_ensnare")
                             || e[i].HasModifier("modifier_crystal_maiden_crystal_nova")
                             || e[i].HasModifier("modifier_axe_berserkers_call")
                             || e[i].HasModifier("modifier_bane_fiends_grip")
                             || e[i].HasModifier("modifier_rubick_telekinesis")
                             || e[i].HasModifier("modifier_storm_spirit_electric_vortex_pull")
                             || e[i].HasModifier("modifier_winter_wyvern_cold_embrace")
                             || e[i].HasModifier("modifier_shadow_shaman_shackles")
                             ||
                             (e[i].FindSpell("magnataur_reverse_polarity") != null &&
                              e[i].FindSpell("magnataur_reverse_polarity").IsInAbilityPhase)
                             || (e[i].FindItem("item_blink") != null && e[i].FindItem("item_blink").Cooldown > 11)
                             ||
                             (e[i].FindSpell("queenofpain_blink") != null &&
                              e[i].FindSpell("queenofpain_blink").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("antimage_blink") != null && e[i].FindSpell("antimage_blink").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("antimage_mana_void") != null &&
                              e[i].FindSpell("antimage_mana_void").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("legion_commander_duel") != null &&
                              e[i].FindSpell("legion_commander_duel").Cooldown <= 0)
                             ||
                             (e[i].FindSpell("doom_bringer_doom") != null &&
                              e[i].FindSpell("doom_bringer_doom").IsInAbilityPhase)
                             ||
                             (e[i].HasModifier("modifier_faceless_void_chronosphere_freeze") &&
                              e[i].ClassID != ClassID.CDOTA_Unit_Hero_FacelessVoid)
                             ||
                             (e[i].FindSpell("witch_doctor_death_ward") != null &&
                              e[i].FindSpell("witch_doctor_death_ward").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("rattletrap_power_cogs") != null &&
                              e[i].FindSpell("rattletrap_power_cogs").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("tidehunter_ravage") != null &&
                              e[i].FindSpell("tidehunter_ravage").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("axe_berserkers_call") != null &&
                              e[i].FindSpell("axe_berserkers_call").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("brewmaster_primal_split") != null &&
                              e[i].FindSpell("brewmaster_primal_split").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("omniknight_guardian_angel") != null &&
                              e[i].FindSpell("omniknight_guardian_angel").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("queenofpain_sonic_wave") != null &&
                              e[i].FindSpell("queenofpain_sonic_wave").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("sandking_epicenter") != null &&
                              e[i].FindSpell("sandking_epicenter").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("slardar_slithereen_crush") != null &&
                              e[i].FindSpell("slardar_slithereen_crush").IsInAbilityPhase)
                             || (e[i].FindSpell("tiny_toss") != null && e[i].FindSpell("tiny_toss").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("tusk_walrus_punch") != null &&
                              e[i].FindSpell("tusk_walrus_punch").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("faceless_void_time_walk") != null &&
                              e[i].FindSpell("faceless_void_time_walk").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("faceless_void_chronosphere") != null &&
                              e[i].FindSpell("faceless_void_chronosphere").IsInAbilityPhase)
                             ||
                             (e[i].FindSpell("disruptor_static_storm") != null &&
                              e[i].FindSpell("disruptor_static_storm").Cooldown <= 0)
                             ||
                             (e[i].FindSpell("lion_finger_of_death") != null &&
                              e[i].FindSpell("lion_finger_of_death").Cooldown <= 0)
                             || (e[i].FindSpell("luna_eclipse") != null && e[i].FindSpell("luna_eclipse").Cooldown <= 0)
                             ||
                             (e[i].FindSpell("lina_laguna_blade") != null && e[i].FindSpell("lina_laguna_blade").Cooldown <= 0)
                             ||
                             (e[i].FindSpell("doom_bringer_doom") != null && e[i].FindSpell("doom_bringer_doom").Cooldown <= 0)
                             ||
                             (e[i].FindSpell("life_stealer_rage") != null && e[i].FindSpell("life_stealer_rage").Cooldown <= 0 &&
                              me.Level >= 7)
                             || e[i].IsStunned()
                                )
                            && !e[i].IsMagicImmune()
                            && !e[i].HasModifier("modifier_medusa_stone_gaze_stone"))
                        {
                            E.UseAbility(e[i]);
                        }
                        Utils.Sleep(250, e[i].Handle.ToString());
                    }

                    if (e[i].IsLinkensProtected() && (me.IsVisibleToEnemies || Active) && Utils.SleepCheck(e[i].Handle.ToString()))
                    {
                        if (force != null && force.CanBeCasted() && me.Distance2D(e[i]) < force.CastRange &&
                            menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(force.Name))
                        {
                            force.UseAbility(e[i]);
                        }
                        else if (cyclone != null && cyclone.CanBeCasted() && me.Distance2D(e[i]) < cyclone.CastRange &&
                                 menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
                        {
                            cyclone.UseAbility(e[i]);
                        }
                        else if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e[i]) < orchid.CastRange &&
                                 menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                        {
                            orchid.UseAbility(e[i]);
                        }
                        else if (atos != null && atos.CanBeCasted() && me.Distance2D(e[i]) < atos.CastRange - 400 &&
                                 menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(atos.Name))
                        {
                            atos.UseAbility(e[i]);
                        }
                        else if (dagon != null && dagon.CanBeCasted() && me.Distance2D(e[i]) < dagon.CastRange &&
                                 menu.Item("Link").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                 )
                        {
                            dagon.UseAbility(e[i]);

                        }
                        Utils.Sleep(350, e[i].Handle.ToString());
                    }
                }
            }
        } // SkywrathMage class
    }
}
