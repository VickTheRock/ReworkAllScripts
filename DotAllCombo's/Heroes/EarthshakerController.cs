namespace DotaAllCombo.Heroes
{
    //TODO Only Love MazaiPC
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

    internal class EarthshakerController : Variables, IHeroController
    {
        private Ability Q, W, E, R;
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ult = new Menu("AutoUsage", "AutoUsage");

        private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;

        private readonly int[] eDmg = new[] { 0, 50, 75, 100, 125 };
        private readonly int[] rDmg = new[] { 0, 160, 210, 270 };
        private readonly int[] wDmg = new[] { 0, 100, 200, 300, 400 };
        private readonly int[] qDmg = new[] { 0, 110, 160, 210, 260 };
        private readonly int[] dagonDmg = new[] { 0, 400, 500, 600, 700, 800 };
        private readonly int[] creepsDmg = { 0, 40, 55, 70 };

        private readonly double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        private readonly double[] bloodrage = { 0, 1.25, 1.3, 1.35, 1.4 };
        private readonly double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };


        private Dictionary<uint, double> damage = new Dictionary<uint, double>();

        List<Hero> enemies = new List<Hero>();

        public void Combo()
        {
            //spell
            Q = me.Spellbook.SpellQ;
            W = me.FindSpell("earthshaker_enchant_totem");
            E = me.Spellbook.SpellE;
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

            if (Active && me.IsAlive && Utils.SleepCheck("activated"))
            {
                e = Toolset.ClosestToMouse(me);
                if (e == null) return; var modifEther = e.HasModifier("modifier_item_ethereal_blade_slow");
                var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
                var noBlade = e.HasModifier("modifier_item_blade_mail_reflect");
                sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
                if (e.IsAlive && e.IsVisible && me.Distance2D(e) <= 2300 && !noBlade)
                {
                    if (me.HasModifier("modifier_earthshaker_enchant_totem") && !me.IsAttacking() && me.Distance2D(e) <= 300 && Utils.SleepCheck("WMod"))
                    {
                        me.Attack(e);
                        Utils.Sleep(250, "WMod");
                    }
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
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(e) > 400
                        && me.Distance2D(e) <= 1180
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
                                    && !me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && me.Distance2D(e) < 2300
                                    && me.Distance2D(e) >= 1200
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility();
                                    Utils.Sleep(200, "W");
                                }
                                if (
                                    W != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && !me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && me.Distance2D(e) < W.GetCastRange()
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility();
                                    Utils.Sleep(200, "W");
                                }
                                if (me.AghanimState())
                                {
                                    if (
                                    W != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && !me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && me.Distance2D(e) >= 300
                                    && me.Distance2D(e) < 900 + me.HullRadius
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                    {
                                        W.UseAbility(e.Position);
                                        Utils.Sleep(200, "W");
                                    }
                                    if (
                                    W != null
                                    && W.CanBeCasted()
                                    && me.CanCast()
                                    && !me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && me.Distance2D(e) <= 300
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && Utils.SleepCheck("W"))
                                    {
                                        W.UseAbility(me);
                                        Utils.Sleep(200, "W");
                                    }
                                }
                                if (
                                    Q != null
                                    && Q.CanBeCasted()
                                    && (e.IsLinkensProtected()
                                        || !e.IsLinkensProtected())
                                    && me.CanCast()
                                    && me.Distance2D(e) < Q.GetCastRange() + me.HullRadius + 24
                                    && !stoneModif
                                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                    && Utils.SleepCheck("Q")
                                    )
                                {
                                    Q.UseAbility(e.Position);
                                    Utils.Sleep(330, "Q");
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
                                    && !e.IsRooted()
                                    && !e.IsHexed()
                                    && !e.IsStunned()
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
                                            || ethereal.Cooldown < 18))
                                    && !e.IsLinkensProtected()
                                    && dagon.CanBeCasted()
                                    && me.Distance2D(e) <= 1400
                                    && !e.IsMagicImmune()
                                    && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
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
                    if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1600 && !me.HasModifier("modifier_earthshaker_enchant_totem"))
                    {
                        Orbwalking.Orbwalk(e, 0, 1600, true, true);
                    }
                }
                Utils.Sleep(150, "activated");
            }
            AutoSpells();

        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");


            Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
            Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"earthshaker_enchant_totem", true},
                {"earthshaker_fissure", true}
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
                {"item_shivas_guard",true},
                {"item_blink", true},
                {"item_soul_ring", true},
                {"item_ghost", true},
                {"item_cheese", true}
            })));
            ult.AddItem(new MenuItem("autoUlt", "AutoUsage").SetValue(true));
            ult.AddItem(new MenuItem("dmg", "Show Ult Damage Spell(R)").SetValue(true));

            ult.AddItem(new MenuItem("AutoSpells", "AutoSpells").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"earthshaker_fissure", true},
                {"earthshaker_enchant_totem", true},
                {"earthshaker_echo_slam", true}
            })));
            ult.AddItem(new MenuItem("AutoItems", "AutoItems").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon", true},
                {"item_blink", false},
                {"item_shivas_guard", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
            ult.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(3, 1, 5)));
            ult.AddItem(new MenuItem("Heelm", "Max Enemies in Range to solo kill").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(skills);
            Menu.AddSubMenu(items);
            Menu.AddSubMenu(ult);
            Drawing.OnDraw += DrawUltiDamage;
            Print.LogMessage.Success("Time to shake things up and see where they settle!");
        }

        public void OnCloseEvent()
        {
            Drawing.OnDraw -= DrawUltiDamage;
        }
        private bool CanIncreaseMagicDmg(Hero source, Unit target)
        {
            //var orchid = source.FindItem("item_orchid") ?? source.FindItem("item_bloodthorn");
            var veil = source.FindItem("item_veil_of_discord");
            ethereal = source.FindItem("item_ethereal_blade");

            return (//(orchid != null && orchid.CanBeCasted() && !target.HasModifier("modifier_orchid_malevolence_debuff"))||
                  (veil != null && veil.CanBeCasted() && !target.HasModifier("modifier_item_veil_of_discord_debuff"))
                 || (ethereal != null && ethereal.CanBeCasted() && !IsDisembodied(target))
                 )
                 && source.CanUseItems();
        }

        private bool IsDisembodied(Unit target)
        {
            string[] modifs =
            {
                "modifier_item_ethereal_blade_ethereal",
                "modifier_pugna_decrepify"
            };

            return target.HasModifiers(modifs);
        }

        // При передаче по значению метод получает не саму переменную, а ее копию.
        // А при передаче параметра по ссылке (ref) метод получает адрес переменной в памяти, что в свою очередь экономит память.
        private double GetDamageTaken(Hero victim, ref List<Hero> enemies)
        {
            double dmgResult = 0;

            List<Unit> creeps = ObjectManager.GetEntities<Unit>().Where(x =>
                     (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                      || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                      || x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                      || x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                      || x.ClassID == ClassID.CDOTA_Unit_SpiritBear
                      || x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                      || x.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                      || x.ClassID == ClassID.CDOTA_BaseNPC_Creep
                      || x.HasInventory) && !x.IsMagicImmune() &&
                     x.IsAlive && x.Team != me.Team && x.IsVisible && victim.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24 &&
                     x.IsSpawned && x.IsValidTarget()).ToList();
            int creepsECount = creeps.Count;

            dmgResult = R != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name) &&
                R.CanBeCasted() && enemies.Count(
                            x => victim.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value ? ((creepsECount * creepsDmg[R.Level]) + rDmg[R.Level]) + eDmg[E.Level] : 0;

            if (victim.NetworkName == "CDOTA_Unit_Hero_Spectre" && victim.Spellbook.Spell3.Level > 0)
            {
                dmgResult = R != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name) &&
                    R.CanBeCasted() && enemies.Count(
                            x => victim.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value ? ((creepsECount * creepsDmg[R.Level]) + rDmg[R.Level]) + eDmg[E.Level] *
                      (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04)) : 0;

                if (me.Distance2D(victim) < 300 + me.HullRadius)
                    dmgResult += eDmg[E.Level] * (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04));
            }

            if (Q != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name) && Q.CanBeCasted())
            {
                dmgResult += qDmg[Q.Level];

                if (enemies.Count(x => x.Distance2D(victim) <= 300 + me.HullRadius && !Equals(x)) > 1)
                    dmgResult += eDmg[E.Level];
            }

            if (W != null && W.CanBeCasted() && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
            {
                if (enemies.Count(x => x.Distance2D(victim) <= 300 + me.HullRadius && !Equals(x)) > 1)
                    dmgResult += eDmg[E.Level];
            }

            if (victim.NetworkName == "CDOTA_Unit_Hero_SkeletonKing" && victim.Spellbook.SpellR.CanBeCasted())
                dmgResult = 0;

            if (victim.HasModifier("modifier_kunkka_ghost_ship_damage_absorb"))
                dmgResult *= 0.5;

            if (victim.HasModifier("modifier_bloodseeker_bloodrage"))
            {
                var blood = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_Hero_Bloodseeker);
                if (blood != null)
                    dmgResult *= bloodrage[blood.Spellbook.Spell1.Level];
                else
                    dmgResult *= 1.4;
            }

            if (victim.HasModifier("modifier_chen_penitence"))
            {
                var chen =
                    ObjectManager.GetEntities<Hero>()
                        .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Chen);
                if (chen != null)
                    dmgResult *= penitence[chen.Spellbook.Spell1.Level];
            }

            if (victim.HasModifier("modifier_shadow_demon_soul_catcher"))
            {
                var demon = ObjectManager.GetEntities<Hero>()
                    .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Hero_Shadow_Demon);
                if (demon != null)
                    dmgResult *= souls[demon.Spellbook.Spell2.Level];
            }

            if (victim.HasModifier("modifier_item_mask_of_madness_berserk"))
                dmgResult *= 1.3;

            vail = me.FindItem("item_veil_of_discord");
            if (vail != null && vail.CanBeCasted() && !victim.HasModifier("modifier_item_veil_of_discord_debuff")
                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name))
            {
                dmgResult *= 1.25;
            }

           

            var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
            dmgResult = dmgResult * spellamplymult;
            shiva = me.FindItem("item_shivas_guard");
            if (shiva != null && shiva.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
                dmgResult += 200;

            int etherealdamage = (int)((me.TotalStrength * 2) + 75);
            if (ethereal != null && ethereal.CanBeCasted() && victim.Handle == e?.Handle)
                dmgResult += etherealdamage * 1.4;
            if (dagon != null && dagon.CanBeCasted() && victim.Handle == e?.Handle)
                dmgResult += dagonDmg[dagon.Level];


                if (vail != null && vail.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name) && !victim.HasModifier("modifier_item_veil_of_discord_debuff") &&
                ethereal != null && ethereal.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(ethereal.Name) && victim.Handle == e?.Handle)
                dmgResult += etherealdamage * 1.4;

            dmgResult *= 1 - victim.MagicDamageResist;
            if (!me.HasModifier("modifier_earthshaker_enchant_totem") && W != null && W.CanBeCasted()
                && victim.Handle == e?.Handle
                && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
            {
                dmgResult += (((((me.MaximumDamage + me.MinimumDamage) / 2) * (wDmg[W.Level] / 100)) + me.BonusDamage) - victim.DamageResist) + (eDmg[E.Level] * (1 - victim.MagicDamageResist));
            }

            if (me.HasModifier("modifier_earthshaker_enchant_totem")
               && victim.Handle == e?.Handle
               && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
            {
                dmgResult += ((((me.MaximumDamage + me.MinimumDamage) / 2) + me.BonusDamage) - victim.DamageResist) + (eDmg[E.Level] * (1 - victim.MagicDamageResist));
            }

            return dmgResult;
        } // GetDamageTaken::END

        private void AutoSpells()
        {
            enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsMagicImmune() && !x.IsIllusion).ToList();

            if (Menu.Item("autoUlt").IsActive())
            {
                e = Toolset.ClosestToMouse(me, 9000);

                foreach (var v in enemies.Where(x => !x.IsMagicImmune()))
                {
                    damage[v.Handle] = GetDamageTaken(v, ref enemies);

                    if (me.IsInvisible()) return;

                    if (
                        enemies.Count(
                            x => x.Health <= damage[v.Handle] && v.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value)
                    {
                        uint elsecount = 0; elsecount += 1;
                        if (blink != null
                            && me.CanCast()
                            && blink.CanBeCasted()
                            && me.Distance2D(v) > 100
                            && me.Distance2D(v) <= 1180
                            && Utils.SleepCheck("blink")
                            )
                        {
                            blink.UseAbility(v.Position);
                            Utils.Sleep(250, "blink");
                        }
                        else if (W != null && W.CanBeCasted() && me.Distance2D(v) <= 900 + me.HullRadius + 24 && me.AghanimState()
                            && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                            && (blink == null || !blink.CanBeCasted())
                        && Utils.SleepCheck("W"))
                        {
                            W.UseAbility(v.Position);
                            Utils.Sleep(250, "W");
                        }
                        else elsecount += 1;
                        if (enemies.Count(
                        x => me.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24) >=
                        Menu.Item("Heel").GetValue<Slider>().Value)
                        {
                            if (vail != null
                                && vail.CanBeCasted()
                                && me.CanCast()
                                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                                && me.Distance2D(v) <= 1190
                                && Utils.SleepCheck("vail")
                                )
                            {
                                vail.UseAbility(v.Position);
                                Utils.Sleep(250, "vail");
                            } // orchid Item endelse 

                            else elsecount += 1;
                            if (elsecount == 3 &&
                                ethereal != null
                                && ethereal.CanBeCasted()
                                && me.CanCast()
                                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                && (v.Handle == e.Handle || e.Distance2D(v) > 700)
                                && me.Distance2D(v) <= 1190
                                && Utils.SleepCheck("ethereal")
                                )
                            {
                                ethereal.UseAbility(v);
                                Utils.Sleep(250, "ethereal");
                            } // orchid Item end
                            else elsecount += 1;
                            if (!CanIncreaseMagicDmg(me, v))
                            {
                                if (elsecount == 4 && R != null && R.CanBeCasted() && Utils.SleepCheck("R")
                                    && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name))
                                {
                                    R.UseAbility();
                                    Utils.Sleep(250, "R");
                                }
                                if (R == null || !R.CanBeCasted() ||
                                    !Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name))
                                {
                                    if (Q != null && Q.CanBeCasted() && v.Distance2D(me) <= 525 + me.HullRadius + 24
                                        && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                        && Utils.SleepCheck("Q"))
                                    {
                                        Q.UseAbility(v.Position);
                                        Utils.Sleep(250, "Q");
                                    }
                                    if (shiva != null
                                      && shiva.CanBeCasted()
                                      && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                      && v.Distance2D(me) <= shiva.GetCastRange()
                                      && Utils.SleepCheck("shiva"))
                                    {
                                        shiva.UseAbility();
                                        Utils.Sleep(250, "shiva");
                                    }
                                }

                                if (me.AghanimState())
                                {
                                    if (W != null && W.CanBeCasted()
                                        && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                        && v.Distance2D(me) <= 300 + me.HullRadius + 24 &&
                                    !me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                    {
                                        W.UseAbility(me);
                                        Utils.Sleep(250, "W");
                                    }
                                }
                                else if (W != null && W.CanBeCasted()
                                        && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                        && v.Distance2D(me) <= W.GetCastRange() + me.HullRadius + 24 &&
                                        !me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility();
                                    Utils.Sleep(250, "W");
                                }
                            }
                        }
                    }
                    if (enemies.Count(
                        x => x.Distance2D(v) <= 500) <= Menu.Item("Heelm").GetValue<Slider>().Value
                           && blink != null
                           && blink.CanBeCasted()
                           && me.CanCast()
                           && v.Health <= damage[v.Handle]
                           && me.Distance2D(v) <= 1180
                           && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                           && me.Distance2D(v) > 300
                           && Utils.SleepCheck("blink")
                           )
                    {
                        blink.UseAbility(v.Position);
                        Utils.Sleep(250, "blink");
                    }
                    else if (enemies.Count(
                       x => me.Distance2D(x) <= 300 + me.HullRadius + 24 && v.Health <= damage[v.Handle]) >= 1)
                    {

                        if (R == null || !R.CanBeCasted() ||
                            !Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name) || (R.CanBeCasted() && enemies.Count(
                            x => x.Health <= damage[v.Handle] && me.Distance2D(x) <= R.GetCastRange() + me.HullRadius + 24) <=
                        Menu.Item("Heel").GetValue<Slider>().Value))
                        {
                            if (me.Distance2D(v) <= 500 + me.HullRadius + 24 && me.Distance2D(v) >= 300 + me.HullRadius + 24 && Utils.SleepCheck("Move"))
                            {
                                me.Move(v.Position);
                                Utils.Sleep(500, "Move");
                            }
                            if (v.Distance2D(me) <= 300 + me.HullRadius + 24)
                            {
                                if (vail != null
                                 && vail.CanBeCasted()
                                 && me.CanCast()
                                 && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                                 && Utils.SleepCheck("vail")
                                 )
                                {
                                    vail.UseAbility(v.Position);
                                    Utils.Sleep(250, "vail");
                                } // orchid Item endelse 
                                else if (ethereal != null
                                    && ethereal.CanBeCasted()
                                    && me.CanCast()
                                    && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                    && Utils.SleepCheck("ethereal")
                                    )
                                {
                                    ethereal.UseAbility(v);
                                    Utils.Sleep(250, "ethereal");
                                } // orchid Item end
                                else if (Q != null && Q.CanBeCasted()
                                    && Utils.SleepCheck("Q"))
                                {
                                    Q.UseAbility(v.Position);
                                    Utils.Sleep(250, "Q");
                                }
                                else if (shiva != null
                                  && shiva.CanBeCasted()
                                  && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                  && Utils.SleepCheck("shiva"))
                                {
                                    shiva.UseAbility();
                                    Utils.Sleep(250, "shiva");
                                }
                                else if (dagon != null
                                && dagon.CanBeCasted()
                                && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && Utils.SleepCheck("dagon"))
                                {
                                    dagon.UseAbility(v);
                                    Utils.Sleep(250, "dagon");
                                }
                                else if (W != null && W.CanBeCasted() && !me.AghanimState()
                             && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                             && !me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility();
                                    Utils.Sleep(250, "W");
                                }
                                else if (W != null && W.CanBeCasted() && me.AghanimState()
                                    && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name) &&
                                !me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                {
                                    W.UseAbility(me);
                                    Utils.Sleep(250, "W");
                                }
                                else if (!me.HasModifier("modifier_earthshaker_enchant_totem")
                                    && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                    && v.Health <= (((((me.MaximumDamage + me.MinimumDamage) / 2) * (wDmg[W.Level] / 100)) + me.BonusDamage) + eDmg[E.Level]))
                                {
                                    if (me.AghanimState())
                                    {
                                        if (W != null && W.CanBeCasted()
                                         && !me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                        {
                                            W.UseAbility(me);
                                            Utils.Sleep(250, "W");
                                        }
                                    }
                                    else if (W != null && W.CanBeCasted()
                                        && !me.HasModifier("modifier_earthshaker_enchant_totem") && Utils.SleepCheck("W"))
                                    {
                                        W.UseAbility();
                                        Utils.Sleep(250, "W");
                                    }
                                }
                            }

                        }

                    }
                    if (me.HasModifier("modifier_earthshaker_enchant_totem"))
                    {
                        if (v.Health <=
                            (((me.MinimumDamage + me.MaximumDamage) / 2) + me.BonusDamage) - v.DamageAverage
                            && !me.IsAttacking()
                            && me.Distance2D(v) <= 300 + me.HullRadius + 24
                            && Utils.SleepCheck("Attack"))
                        {
                            me.Attack(v);
                            Utils.Sleep(250, "Attack");
                        }
                        else if (me.Distance2D(v) <= 300 + me.HullRadius + 24
                            && !me.IsAttacking() &&
                            Utils.SleepCheck("Attack"))
                        {
                            me.Attack(v);
                            Utils.Sleep(250, "Attack");
                        }
                    }
                } // foreach::END
            }
        } // AutoSpells::END

        private void DrawUltiDamage(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || enemies.Count == 0) return;

            if (Menu.Item("dmg").IsActive())
            {
                foreach (var v in enemies)
                {
                    var screenPos = HUDInfo.GetHPbarPosition(v);
                    if (!OnScreen(v.Position)) continue;
                    var text = (v.Health <= damage[v.Handle] ? "Yes: " + Math.Floor(damage[v.Handle]) : "No: " + Math.Floor(damage[v.Handle]));
                    var size = new Vector2(18, 18);
                    var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                    var position = new Vector2(screenPos.X - textSize.X + 91, screenPos.Y + 62);
                    Drawing.DrawText(
                        text,
                        position,
                        size,
                        (v.Health <= damage[v.Handle] ? Color.LawnGreen : Color.Red),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        text,
                        new Vector2(screenPos.X - textSize.X + 92, screenPos.Y + 63),
                        size,
                        (Color.Black),
                        FontFlags.AntiAlias);
                }
            }
        } // DrawUltiDamage::END

        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width
                  || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }


    }
}