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
        private Ability Q, W, E, R;
        private Item orchid, sheep, vail, soul, arcane, blink, shiva, dagon, atos, ethereal, cheese, ghost;
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");
        private readonly Menu ultR = new Menu("AutoUsage Ult(R) to kill Enemies ", "idR");
        private readonly Menu ult = new Menu("AutoUsage Abilities and Items to Solo enemy kill", "id");
        private float eDmg;
        private float rDmg;
        private double[] wDmg = { 0, 100, 175, 275, 350 };
        private double[] qDmg = { 0, 85, 100, 115, 145 };
        private double[] penitence = { 0, 1.15, 1.2, 1.25, 1.3 };
        private double[] bloodrage = { 0, 1.15, 1.2, 1.25, 1.3 };
        private double[] souls = { 0, 1.2, 1.3, 1.4, 1.5 };
        private readonly int[] dagonDmg = { 0, 400, 500, 600, 700, 800 };

        private Dictionary<uint, double> damage = new Dictionary<uint, double>();

        List<Hero> enemies = new List<Hero>();

        public void Combo()
        {
            // Target initialization

            // Spells initialization
            Q = me.Spellbook.SpellQ;
            W = me.Spellbook.SpellW;
            E = me.Spellbook.SpellE;
            R = me.Spellbook.SpellR;
            // Items initialization
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

            // State of keys initialization
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            Push = Game.IsKeyDown(Menu.Item("keyQ").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsFullMagicResist() && !x.IsIllusion).ToList();
            // OnUpdateEvent::END

            // [VickTheRock]

            if (Push)
            {
                if (Q == null) return;

                var unitsList = ObjectManager.GetEntities<Unit>().Where(creep =>
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
                    && creep.IsSpawned
                    && creep.Team != me.Team
                    ).ToList();


                foreach (var v in unitsList)
                {
                    var damageQ = qDmg[Q.Level];
                    if (me.Distance2D(v) < 1200)
                        damageQ += E.GetAbilityData("damage_health_pct") * 0.01 * v.Health;

                    var lens = me.HasModifier("modifier_item_aether_lens");
                    var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
                    if (lens) damageQ *= 1.08;
                    damageQ *= spellamplymult;
                    damageQ *= (1 - v.MagicDamageResist);
                    if (Q.CanBeCasted() && v.Distance2D(me) <= Q.GetCastRange() + me.HullRadius && v.Health <= damageQ && Utils.SleepCheck("qq"))
                    {
                        Q.UseAbility(v);
                        Utils.Sleep(250, "qq");
                    }
                }
            } // if(Push)::END

            e = Toolset.ClosestToMouse(me);
            if (e == null) return;

            var modifEther = e.HasModifier("modifier_item_ethereal_blade_slow");
            var stoneModif = e.HasModifier("modifier_medusa_stone_gaze_stone");
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
                                    && (!W.CanBeCasted() || e.Health <= (e.MaximumHealth * 0.5))
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
                } // if(e.IsVisible)::END

                if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
                {
                    Orbwalking.Orbwalk(e, 0, 1600, true, true);
                }
            } // if(Active)::END

            // Run real-time modules
            AutoSpells();
        } // Combo::END

        // [MaZaiPC]
        private bool IsDisembodied(Unit target)
        {
            string[] modifs =
            {
                "modifier_item_ethereal_blade_ethereal",
                "modifier_pugna_decrepify"
            };

            return target.HasModifiers(modifs);
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

        private void AutoSpells()
        {
            enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsMagicImmune() && !x.IsMagicImmune() && !x.IsIllusion && !x.IsFullMagicSpellResist()).ToList();

            if (Menu.Item("AutoUsage").IsActive())
            {
                e = Toolset.ClosestToMouse(me,8000);

                foreach (var v in enemies)
                {
                    var Units = ObjectManager.GetEntities<Unit>().Where(creep =>
                    (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
                    || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                    || creep.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster_Boar
                    || creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
                    || creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                    || creep.HasInventory
                    )
                    && !creep.Equals(v)
                    && creep.IsAlive
                    && creep.Distance2D(v) <= 320
                    && creep.Team != me.Team
                    ).ToList();
                    if (me.IsInvisible()) return;
                    if(v.IsFullMagiclResistZuus()) return;
                    damage[v.Handle] = CalculateDamage(v);

                    var Range = me.HullRadius + (dagon == null ? W?.GetCastRange() : dagon.GetCastRange());

                    float angle = me.FindAngleBetween(v.Position, true);
                    Vector3 pos = new Vector3((float)(v.Position.X - 300 * Math.Cos(angle)), (float)(v.Position.Y - 300 * Math.Sin(angle)), 0);
                    Vector3 posBlink = new Vector3((float)(v.Position.X - Range * Math.Cos(angle)), (float)(v.Position.Y - Range * Math.Sin(angle)), 0);
                    if (enemies.Count(
                        x => x.Distance2D(v) <= 500) <= Menu.Item("Heelm").GetValue<Slider>().Value
                           && blink != null
                           && blink.CanBeCasted()
                           && me.CanCast()
                           && me.Health >= (me.MaximumHealth / 100 * Menu.Item("minHealth").GetValue<Slider>().Value)
                           && v.Health <= damage[v.Handle]
                           && me.Distance2D(posBlink) <= 1180
                           && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                           && me.Distance2D(v) > 500
                           && Utils.SleepCheck("blink")
                           )
                    {
                        blink.UseAbility(posBlink);
                        Utils.Sleep(250, "blink");
                    }
                    if (v.Health <= damage[v.Handle] && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius + 300)
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
                        if (!CanIncreaseMagicDmg(me, v))
                        {
                             if (dagon != null
                            && dagon.CanBeCasted()
                            && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                            && Utils.SleepCheck("dagon"))
                            {
                                dagon.UseAbility(v);
                                Utils.Sleep(250, "dagon");
                            }
                            else if(Q != null && Q.CanBeCasted() && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                && Utils.SleepCheck("Q"))
                            {
                                Q.UseAbility(v);
                                Utils.Sleep(250, "Q");
                            }
                            else if (W != null && W.CanBeCasted() && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                               && Utils.SleepCheck("W"))
                            {
                                W.UseAbility(v.Position);
                                Utils.Sleep(250, "W");
                            }
                            else if (W != null && W.CanBeCasted() && Units.Count(x => x.Distance2D(pos) <= 300) == 0 && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius + 300 && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name)
                              && Utils.SleepCheck("W"))
                            {
                                W.UseAbility(pos);
                                Utils.Sleep(250, "W");
                            }
                            else if (R != null
                            && R.CanBeCasted()
                            && (W == null || !W.CanBeCasted() || !Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
                            && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name)
                            && Utils.SleepCheck("R"))
                            {
                                R.UseAbility();
                                Utils.Sleep(250, "R");
                            }
                            else if (shiva != null
                                  && shiva.CanBeCasted()
                                  && me.Distance2D(v) <= 600 + me.HullRadius
                                  && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                  && Utils.SleepCheck("shiva"))
                            {
                                shiva.UseAbility();
                                Utils.Sleep(250, "shiva");
                            }
                             if(W!=null && W.CanBeCasted() && me.Distance2D(v)>= W.GetCastRange() + me.HullRadius && me.Distance2D(v) <= W.GetCastRange() + me.HullRadius + 325 && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name) && Utils.SleepCheck("Move"))
                            {
                                me.Move(v.Position);
                                Utils.Sleep(250, "Move");
                            }
                        }

                    }
                    damage[v.Handle] = CalculateDamageR(v);
                    if (R != null && R.CanBeCasted() &&
                        Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name))
                    {

                        if (
                            enemies.Count(
                                x =>
                                    x.Health <= damage[v.Handle]) >=
                            Menu.Item("Heel").GetValue<Slider>().Value)
                        {
                            if ( // SoulRing Item 
                                soul != null
                                && soul.CanBeCasted()
                                && me.CanCast()
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
                            if (ethereal != null
                                  && ethereal.CanBeCasted()
                                  && me.CanCast()
                                  && me.Distance2D(v) <= ethereal.GetCastRange()
                                  && Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                  && Utils.SleepCheck("ethereal")
                                  )
                            {
                                ethereal.UseAbility(v);
                                Utils.Sleep(250, "ethereal");
                            } // orchid Item end
                            else if (R != null
                            && R.CanBeCasted()
                            && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name)
                            && Utils.SleepCheck("R"))
                            {
                                R.UseAbility();
                                Utils.Sleep(250, "R");
                            }
                            else if (dagon != null
                            && dagon.CanBeCasted() && me.Distance2D(v) <= dagon.GetCastRange()
                            && Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                            && Utils.SleepCheck("dagon"))
                            {
                                dagon.UseAbility(v);
                                Utils.Sleep(250, "dagon");
                            }
                        }
                    }
                } // foreach::END
            }
        } // AutoSpells::END

        private double CalculateDamageR(Hero victim)
        {
            double dmgResult = 0;
            eDmg = E.GetAbilityData("damage_health_pct") * 0.01f * victim.Health;
            rDmg = me.AghanimState() ? R.GetAbilityData("damage_scepter") : R.GetAbilityData("damage");

            if (R != null &&
                R.CanBeCasted() && Menu.Item("AutoUlt").GetValue<AbilityToggler>().IsEnabled(R.Name))
            {
                if (victim.NetworkName == "CDOTA_Unit_Hero_Spectre" && victim.Spellbook.Spell3.Level > 0)
                {
                    dmgResult += rDmg * (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04));
                }
                else
                    dmgResult += rDmg;

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

                if (victim.Distance2D(me) <= 1200 + me.HullRadius)
                {
                    dmgResult += eDmg;

                    vail = me.FindItem("item_veil_of_discord");
                    if (vail != null && vail.CanBeCasted() &&
                        !victim.HasModifier("modifier_item_veil_of_discord_debuff")
                        && me.Distance2D(victim) <= vail.GetCastRange()
                        && Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled(vail.Name))
                    {
                        dmgResult *= 1.25;
                    }
                    int etherealdamage = (int)((me.TotalStrength * 2) + 75);
                    if (ethereal != null && ethereal.CanBeCasted() && me.Distance2D(victim) <= ethereal.GetCastRange() &&
                        victim.Handle == e?.Handle)
                    {
                        dmgResult += etherealdamage * 1.4;
                    }
                    if (dagon != null && dagon.CanBeCasted() && me.Distance2D(victim) <= dagon.GetCastRange() &&
                        victim.Handle == e?.Handle &&
                        Menu.Item("AutoUltItems").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                        dmgResult += dagonDmg[dagon.Level];
                }

                dmgResult *= 1 - victim.MagicDamageResist;
            }
            return dmgResult;
        }

        private double CalculateDamage(Hero victim)
        {
            double dmgResult = 0;
            eDmg = E.GetAbilityData("damage_health_pct") * 0.01f * victim.Health;
            rDmg = me.AghanimState() ? R.GetAbilityData("damage_scepter") : R.GetAbilityData("damage");

            if (R != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(R.Name) &&
            R.CanBeCasted())
            {
                if (victim.NetworkName == "CDOTA_Unit_Hero_Spectre" && victim.Spellbook.Spell3.Level > 0)
                {
                    dmgResult += rDmg * (1 - (0.10 + victim.Spellbook.Spell3.Level * 0.04));
                }
                else
                    dmgResult += rDmg;
            }
            if (Q != null && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(Q.Name) && Q.CanBeCasted())
                dmgResult += qDmg[Q.Level];

            if (W != null && W.CanBeCasted() && Menu.Item("AutoSpells").GetValue<AbilityToggler>().IsEnabled(W.Name))
                dmgResult += wDmg[W.Level];

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

            if (victim.Distance2D(me) <= 1200 + me.HullRadius)
                dmgResult += eDmg;


            var spellamplymult = 1 + (me.TotalIntelligence / 16 / 100);
            dmgResult = dmgResult * spellamplymult;
            dmgResult *= 1 - victim.MagicDamageResist;
            int etherealdamage = (int)((me.TotalIntelligence * 2) + 75);
            if (ethereal != null && ethereal.CanBeCasted() && victim.Handle == e?.Handle && (vail == null || victim.HasModifier("modifier_item_veil_of_discord_debuff") || vail.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name) || !Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(vail.Name)))
                dmgResult = dmgResult * 1.4 + etherealdamage;

            if (dagon != null && dagon.CanBeCasted() && victim.Handle == e?.Handle && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                dmgResult += dagonDmg[dagon.Level];
            shiva = me.FindItem("item_shivas_guard");
            if (shiva != null && shiva.CanBeCasted() && Menu.Item("AutoItems").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
                dmgResult += 200;



            return dmgResult;
        } // GetDamageTaken::END

        private void DrawUltiDamage(EventArgs args)
        {
            enemies = ObjectManager.GetEntities<Hero>()
                 .Where(x => x.IsVisible && x.IsAlive && x.Team != me.Team && !x.IsFullMagicResist() && !x.IsIllusion).ToList();
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame || enemies.Count == 0) return;

            if (Menu.Item("dmg").IsActive())
            {
                foreach (var v in enemies)
                {

                    damage[v.Handle] = CalculateDamage(v);
                    var screenPos = HUDInfo.GetHPbarPosition(v);
                    if (!OnScreen(v.Position)) continue;
                    var text = v.Health <= damage[v.Handle] ? "Yes: " + Math.Floor(damage[v.Handle]) : "No: " + Math.Floor(damage[v.Handle]);
                    var size = new Vector2(18, 18);
                    var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                    var position = new Vector2(screenPos.X - textSize.X + 85, screenPos.Y + 62);

                    Drawing.DrawText(
                        text,
                        new Vector2(screenPos.X - textSize.X + 84, screenPos.Y + 63),
                        size,
                        (Color.White),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        text,
                        position,
                        size,
                        (v.Health <= damage[v.Handle] ? Color.LawnGreen : Color.Red),
                        FontFlags.AntiAlias);




                    damage[v.Handle] = CalculateDamageR(v);
                    var textR = v.Health <= damage[v.Handle] ? "ThundergodS" : "";
                    var positionR = new Vector2(screenPos.X - textSize.X + 60, screenPos.Y - 20);

                    Drawing.DrawText(
                        textR,
                        new Vector2(screenPos.X - textSize.X + 59, screenPos.Y - 19),
                        size,
                        (Color.White),
                        FontFlags.AntiAlias);
                    Drawing.DrawText(
                        textR,
                        positionR,
                        size,
                        (Color.LawnGreen),
                        FontFlags.AntiAlias);

                }
            }
        } // DrawUltiDamage::END
        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width
                  || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
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
            ultR.AddItem(new MenuItem("AutoUsage", "AutoUsage").SetValue(true));

            ult.AddItem(new MenuItem("dmg", "Show Draw Damage").SetValue(true));

            items.AddItem(new MenuItem("Link", "Auto triggre Linken").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_arc_lightning", true}
            })));
            ult.AddItem(new MenuItem("AutoSpells", "AutoSpells").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_arc_lightning", true},
                {"zuus_lightning_bolt", true},
                {"zuus_thundergods_wrath", true}
            })));
            ult.AddItem(new MenuItem("AutoItems", "AutoItems").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_blink", true},
                {"item_dagon", true},
                {"item_shivas_guard", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
            ultR.AddItem(new MenuItem("Heel", "Min targets to ult").SetValue(new Slider(2, 1, 5)));
            ultR.AddItem(new MenuItem("AutoUlt", "AutoUlt").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"zuus_thundergods_wrath", true}
            })));
            ultR.AddItem(new MenuItem("AutoUltItems", "AutoUltItems").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon", true},
                {"item_veil_of_discord", true},
                {"item_ethereal_blade", true}
            })));
            ult.AddItem(new MenuItem("minHealth", "Min me healh % to blink in killsteal").SetValue(new Slider(25, 05))); // x/ 10%
            ult.AddItem(new MenuItem("Heelm", "Max Enemies in Range to solo kill").SetValue(new Slider(2, 1, 5)));
            Menu.AddSubMenu(skills);
            Menu.AddSubMenu(items);
            Menu.AddSubMenu(ultR);
            Menu.AddSubMenu(ult);
            Drawing.OnDraw += DrawUltiDamage;
        }

        public void OnCloseEvent()
        {

            Drawing.OnDraw -= DrawUltiDamage;
        }
    }

    internal static class ToolsetZuus
    {
        public static bool IsFullMagiclResistZuus(this Unit source)
        {
            return source.HasModifier("modifier_medusa_stone_gaze_stone")
                   || source.HasModifier("modifier_huskar_life_break_charge")
                   || source.HasModifier("modifier_oracle_fates_edict")
                   || source.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                   || source.HasModifier("modifier_puck_phase_shift")
                   || source.HasModifier("modifier_eul_cyclone")
                   || source.HasModifier("modifier_invoker_tornado")
                   || source.HasModifier("modifier_dazzle_shallow_grave")
                   || source.HasModifier("modifier_winter_wyvern_winters_curse")
                   || (source.HasModifier("modifier_legion_commander_duel") && source.ClassID== ClassID.CDOTA_Unit_Hero_Legion_Commander && source.AghanimState())
                   || source.HasModifier("modifier_brewmaster_storm_cyclone")
                   || source.HasModifier("modifier_shadow_demon_disruption")
                   || source.HasModifier("modifier_tusk_snowball_movement")
                   || source.HasModifier("modifier_abaddon_borrowed_time")
                   || source.HasModifier("modifier_faceless_void_time_walk")
                   || source.HasModifier("modifier_huskar_life_break_charge");
        }
    }
}
