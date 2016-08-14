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

    internal class ZuusController : Variables, IHeroController
    {
        private Ability Q, W, R;
        private readonly double[] eDmg = { 0, 4, 6, 8, 10 };
        private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ult = new Menu("AutoUlt", "AutoUlt");

        private int[] rDmg;
        private int[] wDmg;
        private int[] qDmg;
        public void Combo()
        {
            e = me.ClosestToMouseTarget(2000);


            //spell
            Q = me.Spellbook.SpellQ;

            W = me.Spellbook.SpellW;

            R = me.Spellbook.SpellR;

            // Item
            ethereal = me.FindItem("item_ethereal_blade");


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

            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            Push = Game.IsKeyDown(Menu.Item("keyQ").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            var modifEther = e.HasModifier("modifier_item_ethereal_blade_slow");
            var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
            if (Push)
            {
                if (Q == null) return;
                var Units = ObjectManager.GetEntities<Unit>().Where(creep =>
                    (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                    || creep.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster_Boar
                    || creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
                    || creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                    )
                    && creep.IsAlive
                    && creep.Distance2D(me) <= Q.GetCastRange() + me.HullRadius
                    && creep.Team != me.Team
                    ).ToList();
                var lens = me.HasModifier("modifier_item_aether_lens");
                var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
                qDmg = new[] { 85, 100, 115, 145 };
                foreach (var v in Units)
                {
                    var damageQ = Math.Floor(qDmg[Q.Level - 1] * (1 - v.MagicDamageResist));
                    if (me.Distance2D(v) < 1150)
                        damageQ = damageQ + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health * (1 - v.MagicDamageResist);

                    if (lens) damageQ = damageQ * 1.08;
                    damageQ = damageQ * spellamplymult;
                    if (Q != null && Q.CanBeCasted() && v.Health <= damageQ && Utils.SleepCheck("qq"))
                    {
                        Q.UseAbility(v);
                        Utils.Sleep(250, "qq");
                    }
                }
            }
            if (e == null) return;

            sheep = e.Name == "npc_dota_hero_tidehunter" ? null : me.FindItem("item_sheepstick");
            if (Active && me.IsAlive && e.IsAlive && Utils.SleepCheck("activated"))
            {
                var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
                if (e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
                {
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

                    if (
                        blink != null
                        && Q.CanBeCasted()
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(e) > 1000
                        && !stoneModif
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        blink.UseAbility(e.Position);
                        Utils.Sleep(250, "blink");
                    }
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
                                    W != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && me.Distance2D(e) < 1300
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility(e.Position);
                                    Utils.Sleep(200, "W");
                                }
                                if (
                                    Q != null
                                    && Q.CanBeCasted()
                                    && (e.IsLinkensProtected()
                                        || !e.IsLinkensProtected())
                                    && me.CanCast()
                                    && me.Distance2D(e) < 1400
                                    && !stoneModif
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                    && Utils.SleepCheck("Q")
                                    )
                                {
                                    Q.UseAbility(e);
                                    Utils.Sleep(330, "Q");
                                }
                                if (
                                    R != null
                                    && R.CanBeCasted()
                                    && !Q.CanBeCasted()
                                    && !W.CanBeCasted()
                                    && me.CanCast()
                                    && me.Position.Distance2D(e) < 1200
                                    && e.Health <= (e.MaximumHealth * 0.5)
                                    && !stoneModif
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                                    && Utils.SleepCheck("R"))
                                {
                                    R.UseAbility();
                                    Utils.Sleep(330, "R");
                                }
                                if ( // SoulRing Item 
                                    soul != null
                                    && soul.CanBeCasted()
                                    && me.CanCast()
                                    && me.Health >= (me.MaximumHealth * 0.6)
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
                                    && dagon != null
                                    && (ethereal == null
                                        || (modifEther
                                            || ethereal.Cooldown < 17))
                                    && !e.IsLinkensProtected()
                                    && dagon.CanBeCasted()
                                    && me.Distance2D(e) <= 1400
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
                    Utils.Sleep(200, "activated");
                }
                if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
                {
                    Orbwalking.Orbwalk(e, 0, 1600, true, true);
                }
            }
            A();
        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

            Print.LogMessage.Success("Ah these mortals and their futile games. Oh wait! I'm one of them!");

            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
            Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            Menu.AddItem(new MenuItem("keyQ", "Farm Creep Key").SetValue(new KeyBind('F', KeyBindType.Press)));

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
            ult.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                { "zuus_arc_lightning", true},
                { "zuus_lightning_bolt", true},
                {"zuus_thundergods_wrath", true}
            })));
            items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_arc_lightning", true}
            })));
            ult.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(skills);
            Menu.AddSubMenu(items);
            Menu.AddSubMenu(ult);
        }

        public void OnCloseEvent()
        {

        }

        public void A()
        {
            if (!me.IsAlive) return;
            var enemies =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsIllusion).ToList();


            if (enemies.Count <= 0) return;
            if (Menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
            {
                double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
                double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
                rDmg = me.AghanimState() ? new[] { 440, 540, 640 } : new[] { 225, 325, 425 };
                qDmg = new[] { 85, 100, 115, 145 };

                wDmg = new[] { 100, 175, 275, 350 };
                foreach (var v in enemies)
                {
                    var lens = me.HasModifier("modifier_item_aether_lens");
                    var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
                    double damageR = rDmg[R.Level - 1];
                    if (me.Distance2D(v) < 1150)
                        damageR = damageR + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health;

                    if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                    {
                        damageR =
                            Math.Floor(rDmg[R.Level - 1] *
                                       (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)));
                        if (me.Distance2D(v) < 1150)
                            damageR = damageR + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health;
                    }
                    if (v.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" &&
                        v.Spellbook.SpellR.CanBeCasted())
                        damageR = 0;
                    if (lens) damageR = damageR * 1.08;
                    if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageR = damageR * 0.5;
                    if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageR = damageR * 1.3;
                    if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                    {
                        var blood =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker);
                        if (blood != null)
                            damageR = damageR * bloodrage[blood.Spellbook.Spell1.Level];
                        else
                            damageR = damageR * 1.4;
                    }


                    if (v.HasModifier("modifier_chen_penitence"))
                    {
                        var chen =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen);
                        if (chen != null)
                            damageR = damageR * penitence[chen.Spellbook.Spell1.Level];
                    }

                    if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    {
                        var demon =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon);
                        if (demon != null)
                            damageR = damageR * souls[demon.Spellbook.Spell2.Level];
                    }

                    double damageW = wDmg[W.Level - 1];
                    if (me.Distance2D(v) < 1150)
                        damageW = damageW + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health;
                    if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                    {
                        damageW =
                            Math.Floor(wDmg[W.Level - 1] *
                                       (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)));
                        if (me.Distance2D(v) < 1150)
                            damageW = damageW + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health;
                    }
                    if (lens) damageW = damageW * 1.08;
                    if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageW = damageW * 0.5;
                    if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageW = damageW * 1.3;

                    if (me.HasModifier("modifier_item_aether_lens")) damageW = damageW * 1.08;
                    if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageW = damageW * 0.5;
                    if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageW = damageW * 1.3;
                    if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                    {
                        var blood =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker);
                        if (blood != null)
                            damageW = damageW * bloodrage[blood.Spellbook.Spell1.Level];
                        else
                            damageW = damageW * 1.4;
                    }


                    if (v.HasModifier("modifier_chen_penitence"))
                    {
                        var chen =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen);
                        if (chen != null)
                            damageW = damageW * penitence[chen.Spellbook.Spell1.Level];
                    }


                    if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    {
                        var demon =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon);
                        if (demon != null)
                            damageW = damageW * souls[demon.Spellbook.Spell2.Level];
                    }

                    double damageQ = qDmg[Q.Level - 1];
                    if (me.Distance2D(v) < 1150)
                        damageQ = damageQ + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health;
                    if (v.NetworkName == "CDOTA_Unit_Hero_Spectre" && v.Spellbook.Spell3.Level > 0)
                    {
                        damageQ =
                            Math.Floor(qDmg[Q.Level - 1] *
                                       (1 - (0.10 + v.Spellbook.Spell3.Level * 0.04)));
                        if (me.Distance2D(v) < 1150)
                            damageQ = damageQ + eDmg[me.Spellbook.Spell3.Level] * 0.01 * v.Health;
                    }
                    if (lens) damageQ = damageQ * 1.08;
                    if (v.HasModifier("modifier_bloodseeker_bloodrage"))
                    {
                        var blood =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker);
                        if (blood != null)
                            damageQ = damageQ * bloodrage[blood.Spellbook.Spell1.Level];
                        else
                            damageQ = damageQ * 1.4;
                    }


                    if (v.HasModifier("modifier_chen_penitence"))
                    {
                        var chen =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen);
                        if (chen != null)
                            damageQ = damageQ * penitence[chen.Spellbook.Spell1.Level];
                    }

                    if (v.HasModifier("modifier_shadow_demon_soul_catcher"))
                    {
                        var demon =
                            ObjectManager.GetEntities<Hero>()
                                .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon);
                        if (demon != null)
                            damageQ = damageQ * souls[demon.Spellbook.Spell2.Level];
                    }
                    if (v.HasModifier("modifier_kunkka_ghost_ship_damage_absorb")) damageQ = damageQ * 0.5;
                    if (v.HasModifier("modifier_item_mask_of_madness_berserk")) damageQ = damageQ * 1.3;
                    damageQ = damageQ * spellamplymult;
                    damageQ = damageQ*(1 - v.MagicDamageResist);

                    damageR = damageR * spellamplymult;
                    damageR = damageR * (1 - v.MagicDamageResist);

                    damageW = damageW * spellamplymult;
                    damageW = damageW * (1 - v.MagicDamageResist);
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
                    int etherealdamage = (int)(((me.TotalIntelligence * 2) + 75));
                    if ( // vail
                      ethereal != null
                      && ethereal.CanBeCasted()
                      && ((W != null
                      && W.CanBeCasted()
                      && v.Health <= etherealdamage + damageW * 1.4
                      && v.Health >= damageW)
                      || (Q != null
                      && Q.CanBeCasted()
                      && v.Health <= etherealdamage + (damageQ * 1.4)
                      && v.Health >= damageQ)
                      || (R != null
                      && R.CanBeCasted()
                      && v.Health <= etherealdamage + (damageR * 1.4)
                      && enemies.Count(x => x.Health <= (damageR - 20)) >= (Menu.Item("Heel").GetValue<Slider>().Value)
                      && v.Health >= damageR))
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
                    if (
                        soul != null
                        && soul.CanBeCasted()
                        && Utils.SleepCheck(v.Handle.ToString())
                        && me.Mana < R.ManaCost
                        && me.Mana + 150 > R.ManaCost
                        )
                    {
                        soul.UseAbility();
                        Utils.Sleep(150, v.Handle.ToString());
                    }
                    if (arcane != null
                        && arcane.CanBeCasted()
                        && Utils.SleepCheck(v.Handle.ToString())
                        && me.Mana < R.ManaCost
                        && me.Mana + 135 > R.ManaCost)
                    {
                        arcane.UseAbility();
                        Utils.Sleep(150, v.Handle.ToString());
                    }
                    if (arcane != null
                        && soul != null
                        && Utils.SleepCheck(v.Handle.ToString())
                        && arcane.CanBeCasted() && soul.CanBeCasted()
                        && me.Mana < R.ManaCost
                        && me.Mana + 285 > R.ManaCost)
                    {
                        arcane.UseAbility();
                        soul.UseAbility();
                        Utils.Sleep(150, v.Handle.ToString());
                    }
                    if (Q != null && v != null && Q.CanBeCasted()
                        && me.Distance2D(v) <= Q.GetCastRange() + 50
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
                        && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                        && v.Health < damageQ
                        && Utils.SleepCheck(v.Handle.ToString()))
                    {
                        Q.UseAbility(v);
                        Utils.Sleep(150, v.Handle.ToString());
                        return;
                    }
                    if (me.Distance2D(v) <= W.GetCastRange() + me.HullRadius + 24)
                    {
                        if (W != null && W.CanBeCasted()
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
                        && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(W.Name)
                        && v.Health < damageW
                        && Utils.SleepCheck(v.Handle.ToString()))
                        {
                            W.UseAbility(v.Position);
                            Utils.Sleep(150, v.Handle.ToString());
                        }
                    }
                    else if (me.Distance2D(v) >= W.GetCastRange() + me.HullRadius + 24 && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius + 324)
                    {
                        float angle = me.FindAngleBetween(v.Position, true);
                        Vector3 pos = new Vector3((float)(v.Position.X - 300 * Math.Cos(angle)), (float)(v.Position.Y - 300 * Math.Sin(angle)), 0);
                        var Units = ObjectManager.GetEntities<Unit>().Where(creep =>
                    (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                    || creep.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster_Boar
                    || creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
                    || creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                    )
                    && creep.IsAlive
                    && creep.Distance2D(v) <= 320
                    && creep.Team != me.Team
                    ).ToList();
                        if (W != null && W.CanBeCasted()
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
                        && Units.Count(x => x.Distance2D(v) <= 300) == 0
                        && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(W.Name)
                        && v.Health < damageW
                        && Utils.SleepCheck(v.Handle.ToString()))
                        {
                            W.UseAbility(pos);
                            Utils.Sleep(150, v.Handle.ToString());
                        }
                    }
                    if (R != null && R.CanBeCasted()
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
                        && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
                        && enemies.Count(x => x.Health <= (damageR - 20)) >= (Menu.Item("Heel").GetValue<Slider>().Value)
                        && Utils.SleepCheck(v.Handle.ToString()))
                    {
                        R.UseAbility();
                        Utils.Sleep(150, v.Handle.ToString());
                        return;
                    }

                    var refresh = me.FindItem("item_refresher");
                    if (refresh != null && refresh.CanBeCasted()
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
                        && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
                        && enemies.Count(x => x.Health <= (R.CanBeCasted() ? (damageR-60) * 2 : damageR-20)) >= 3
                        && Utils.SleepCheck(v.Handle.ToString()))
                    {
                        if (
                            soul != null
                            && soul.CanBeCasted()
                            && Utils.SleepCheck(v.Handle.ToString())
                            && me.Mana < R.ManaCost
                            && me.Mana + 150 > R.ManaCost
                            )
                        {
                            soul.UseAbility();
                            Utils.Sleep(150, v.Handle.ToString());
                        }
                        if (arcane != null
                            && arcane.CanBeCasted()
                            && Utils.SleepCheck(v.Handle.ToString())
                            && me.Mana < R.ManaCost
                            && me.Mana + 135 > R.ManaCost)
                        {
                            arcane.UseAbility();
                            Utils.Sleep(150, v.Handle.ToString());
                        }
                        if (arcane != null
                            && soul != null
                            && Utils.SleepCheck(v.Handle.ToString())
                            && arcane.CanBeCasted() && soul.CanBeCasted()
                            && me.Mana < R.ManaCost
                            && me.Mana + 285 > R.ManaCost)
                        {
                            arcane.UseAbility();
                            soul.UseAbility();
                            Utils.Sleep(150, v.Handle.ToString());
                        }
                        if (R != null && R.CanBeCasted() && me.Mana > R.ManaCost + 375)
                        {
                            R.UseAbility();
                        }
                        else if (R != null && !R.CanBeCasted() && me.Mana >= 375 + R.ManaCost)
                        {
                            refresh.UseAbility();
                        }
                        Utils.Sleep(150, v.Handle.ToString());
                    }
                    if (v.IsLinkensProtected() && (me.IsVisibleToEnemies && !me.IsInvisible() || Active))
                    {
                        if (Q != null && Q.CanBeCasted() && me.Distance2D(v) < Q.CastRange &&
                            Menu.Item("Link").GetValue<AbilityToggler>().IsEnabled(Q.Name) &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            Q.UseAbility(v);
                            Utils.Sleep(500, v.Handle.ToString());
                        }
                    }
                }
            }
        }
    }
}
