namespace DotaAllCombo.Heroes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using System.Threading.Tasks;
	using Service;
	using Service.Debug;

	internal class EarthSpiritController : Variables, IHeroController
	{
		public void Combo()
		{
            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);
            qKey = Game.IsKeyDown(menu.Item("qKey").GetValue<KeyBind>().Key);
            wKey = Game.IsKeyDown(menu.Item("wKey").GetValue<KeyBind>().Key);
            eKey = Game.IsKeyDown(menu.Item("eKey").GetValue<KeyBind>().Key);
            AutoUlt = menu.Item("oneult").IsActive();
            if (!menu.Item("enabled").IsActive())
                return;

            e = me.ClosestToMouseTarget(1300);
            if (e == null) return;

            /*D = me.FindSpell("earth_spirit_stone_caller");
			Q = me.FindSpell("earth_spirit_boulder_smash");
			E = me.FindSpell("earth_spirit_geomagnetic_grip");
			W = me.FindSpell("earth_spirit_rolling_boulder");
			F = me.FindSpell("earth_spirit_petrify");
			R = me.FindSpell("earth_spirit_magnetize");*/
            D = me.Spellbook.SpellD;
            Q = me.Spellbook.SpellQ;
            E = me.Spellbook.SpellE;
            W = me.Spellbook.SpellW;
            F = me.Spellbook.SpellF;
            R = me.Spellbook.SpellR;


            var remnant = ObjectManager.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Earth_Spirit_Stone && x.Team == me.Team && x.IsValid).ToList();
            var remnantCount = remnant.Count;


            if (Active && me.Distance2D(e) <= 1300 && e.IsAlive && !me.IsInvisible() && Utils.SleepCheck("Combo"))
            {
                if (remnantCount <= 0)
                {
                    if (
                    D.CanBeCasted()
                    && Q != null
                    && Q.CanBeCasted()
                    && !Wmod
                    && ((blink == null
                    || !blink.CanBeCasted()
                    || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                    || (blink != null && blink.CanBeCasted() && me.Distance2D(e) <= 450))
                    )
                    {
                        if (me.Distance2D(e) <= E.CastRange - 50
                            && Utils.SleepCheck("Rem"))
                        {
                            D.UseAbility(Prediction.InFront(me, 50));
                            Utils.Sleep(500, "Rem");
                        }
                    }
                    else if (
                        D.CanBeCasted()
                        && Q != null
                        && !Q.CanBeCasted()
                        && E.CanBeCasted()
                        && !Wmod
                        && ((blink == null
                        || !blink.CanBeCasted()
                        || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                        || (blink != null && blink.CanBeCasted() && me.Distance2D(e) <= 450))
                        )
                    {
                        if (me.Distance2D(e) <= E.CastRange - 50
                            && Utils.SleepCheck("Rem"))
                        {
                            D.UseAbility(Prediction.InFront(e, 0));
                            Utils.Sleep(600, "Rem");
                        }
                    }
                }
                for (int i = 0; i < remnantCount; ++i)
                {
                    var r = remnant[i];

                    if (remnantCount >= 1)
                    {
                        if (
                            D != null && D.CanBeCasted()
                            && ((Q != null && Q.CanBeCasted())
                            || (W != null && W.CanBeCasted()))
                            && !Wmod
                            && me.Distance2D(r) >= 350
                            && ((blink == null
                            || !blink.CanBeCasted()
                            || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                            || (blink != null && me.Distance2D(e) <= 350 && blink.CanBeCasted()))
                            )
                        {
                            if (me.Distance2D(e) <= E.CastRange - 50
                                && Utils.SleepCheck("Rem"))
                            {
                                D.UseAbility(Prediction.InFront(me, 50));
                                Utils.Sleep(600, "Rem");
                            }
                        }
                        if (
                            me.Distance2D(r) >= 1500
                            && me.Distance2D(r) <= 350
                            && Q.CanBeCasted()
                            && Utils.SleepCheck("RemMove"))
                        {
                            me.Move(r.Position);
                            Utils.Sleep(250, "RemMove");
                        }
                        if (//Q Skill
                            W != null
                            && (!Q.CanBeCasted()
                            || Q == null)
                            && !E.CanBeCasted()
                            && W.CanBeCasted()
                            && me.Distance2D(e) <= E.CastRange - 50
                            && me.CanCast()
                            && Utils.SleepCheck(me.Handle + "remnantW")
                            )
                        {
                            W.CastSkillShot(e);
                            Utils.Sleep(250, me.Handle + "remnantW");
                        }
                        if (//Q Skill
                           Q != null
                           && Q.CanBeCasted()
                           && me.CanCast()
                           && me.Distance2D(e) <= E.CastRange - 50
                           && me.Distance2D(r) <= 210
                           && Utils.SleepCheck(r.Handle + "remnantQ")
                           )
                        {
                            Q.CastSkillShot(e);
                            Utils.Sleep(250, r.Handle + "remnantQ");
                        }
                        else
                        if (//W Skill
                           W != null
                           && W.CanBeCasted()
                           && !Q.CanBeCasted()
                           && me.Distance2D(e) <= E.CastRange
                           && Utils.SleepCheck(me.Handle + "remnantW")
                           )
                        {
                            W.CastSkillShot(e);
                            Utils.Sleep(250, me.Handle + "remnantW");
                        }
                        if (r != null
                           && E != null
                           && E.CanBeCasted()
                           && me.CanCast()
                           && me.Distance2D(r) < E.CastRange
                           && me.Distance2D(e) <= E.CastRange
                           )
                        {
                            if (//E Skill
                                e.Distance2D(r) <= 200
                                && Utils.SleepCheck(r.Handle + "remnantE")
                               )
                            {
                                E.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                            if (//E Skill
                              me.Distance2D(e) <= 200
                              && e.Distance2D(r) > 0
                              && me.Distance2D(r) >= e.Distance2D(r)
                              && Utils.SleepCheck(r.Handle + "remnantE")
                              )
                            {
                                E.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                        }
                        if (
                           blink != null
                           && r != null
                           && me.CanCast()
                           && blink.CanBeCasted()
                           && me.Distance2D(e) >= 450
                           && me.Distance2D(e) <= 1150
                           && r.Distance2D(me) >= 300
                           && !Wmod
                           && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                           && Utils.SleepCheck("blink")
                           )
                        {
                            blink.UseAbility(e.Position);
                            Utils.Sleep(250, "blink");
                        }
                    }
                }
                Utils.Sleep(50, "Combo");
            }
        }
		private  void Others(EventArgs args)
		{

            qKey = Game.IsKeyDown(menu.Item("qKey").GetValue<KeyBind>().Key);
            wKey = Game.IsKeyDown(menu.Item("wKey").GetValue<KeyBind>().Key);
            eKey = Game.IsKeyDown(menu.Item("eKey").GetValue<KeyBind>().Key);
            AutoUlt = menu.Item("oneult").IsActive();
            if (!menu.Item("enabled").IsActive())
                return;

            if (e == null) return;

            D = me.Spellbook.SpellD;
            Q = me.Spellbook.SpellQ;
            E = me.Spellbook.SpellE;
            W = me.Spellbook.SpellW;
            F = me.Spellbook.SpellF;
            R = me.Spellbook.SpellR;


            var magnetizemod = e.Modifiers.Where(y => y.Name == "modifier_earth_spirit_magnetize").DefaultIfEmpty(null).FirstOrDefault();

            if (AutoUlt && magnetizemod != null && magnetizemod.RemainingTime <= 0.2 + Game.Ping && me.Distance2D(e) <= D.CastRange && Utils.SleepCheck("Rem"))
            {
                D.UseAbility(e.Position);
                Utils.Sleep(1000, "Rem");
            }
            var remnant = ObjectManager.GetEntities<Unit>().Where(x => x.ClassID == ClassID.CDOTA_Unit_Earth_Spirit_Stone && x.Team == me.Team
                                       && x.Distance2D(me) <= 1700 && x.IsAlive && x.IsValid).ToList();
            var remnantCount = remnant.Count;
            if (Active && me.Distance2D(e) <= 1400 && e.IsAlive && !me.IsInvisible() && Utils.SleepCheck("Combo"))
            {
                Wmod = me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");


                ethereal = me.FindItem("item_ethereal_blade");
                urn = me.FindItem("item_urn_of_shadows");
                dagon =
                    me.Inventory.Items.FirstOrDefault(
                        item =>
                            item.Name.Contains("item_dagon"));
                halberd = me.FindItem("item_heavens_halberd");
                orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
                abyssal = me.FindItem("item_abyssal_blade");
                mail = me.FindItem("item_blade_mail");
                bkb = me.FindItem("item_black_king_bar");
                blink = me.FindItem("item_blink");
                medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");
                sheep = e.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");
                vail = me.FindItem("item_veil_of_discord");
                cheese = me.FindItem("item_cheese");
                ghost = me.FindItem("item_ghost");
                atos = me.FindItem("item_rod_of_atos");
                soulring = me.FindItem("item_soul_ring");
                arcane = me.FindItem("item_arcane_boots");
                stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
                Shiva = me.FindItem("item_shivas_guard");
                var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
                var charge = me.Modifiers.FirstOrDefault(y => y.Name == "modifier_earth_spirit_stone_caller_charge_counter");

                if (//W Skill
                       W != null
                       && charge.StackCount == 0
                       && W.CanBeCasted()
                       && me.Distance2D(e) <= 800
                       && me.CanCast()
                       && Utils.SleepCheck(me.Handle + "remnantW")
                       )
                {
                    W.CastSkillShot(e);
                    Utils.Sleep(250, me.Handle + "remnantW");
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
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
                    )
                {
                    halberd.UseAbility(e);
                    Utils.Sleep(250, "halberd");
                }
                if ( //Ghost
                    ghost != null
                    && ghost.CanBeCasted()
                    && me.CanCast()
                    && ((me.Position.Distance2D(e) < 300
                         && me.Health <= (me.MaximumHealth * 0.7))
                        || me.Health <= (me.MaximumHealth * 0.3))
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                    && Utils.SleepCheck("Ghost"))
                {
                    ghost.UseAbility();
                    Utils.Sleep(250, "Ghost");
                }
                if ( // Arcane Boots Item
                    arcane != null
                    && me.Mana <= W.ManaCost
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
                    && arcane.CanBeCasted()
                    && Utils.SleepCheck("arcane")
                    )
                {
                    arcane.UseAbility();
                    Utils.Sleep(250, "arcane");
                } // Arcane Boots Item end
                if (
                    // cheese
                    cheese != null
                    && cheese.CanBeCasted()
                    && me.Health <= (me.MaximumHealth * 0.3)
                    && me.Distance2D(e) <= 700
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
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
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
                    && me.Distance2D(e) <= 700
                    )
                {
                    medall.UseAbility(e);
                    Utils.Sleep(250, "Medall");
                } // Medall Item end

                if ( //R Skill
                    R != null
                    && R.CanBeCasted()
                    && me.CanCast()
                    && me.Distance2D(e) <= 200
                    && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                    && Utils.SleepCheck("R")
                    )
                {
                    R.UseAbility();
                    Utils.Sleep(200, "R");
                } // R Skill end
                if ( // sheep
                    sheep != null
                    && sheep.CanBeCasted()
                    && me.CanCast()
                    && !e.IsLinkensProtected()
                    && !e.IsMagicImmune()
                    && me.Distance2D(e) <= 1400
                    && !stoneModif
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
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
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
                    && Utils.SleepCheck("abyssal")
                    && me.Distance2D(e) <= 400
                    )
                {
                    abyssal.UseAbility(e);
                    Utils.Sleep(250, "abyssal");
                } // Abyssal Item end
                if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) &&
                    Utils.SleepCheck("orchid"))
                {
                    orchid.UseAbility(e);
                    Utils.Sleep(100, "orchid");
                }

                if (Shiva != null && Shiva.CanBeCasted() && me.Distance2D(e) <= 600
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
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
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
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
                    && remnant.Count == 0
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                    && Utils.SleepCheck("blink")
                    )
                {
                    blink.UseAbility(e.Position);
                    Utils.Sleep(250, "blink");
                }

                if ( // SoulRing Item 
                    soulring != null
                    && soulring.CanBeCasted()
                    && me.CanCast()
                    && me.Health >= (me.MaximumHealth * 0.5)
                    && me.Mana <= R.ManaCost
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(soulring.Name)
                    )
                {
                    soulring.UseAbility();
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
                if ( // atos Blade
                    atos != null
                    && atos.CanBeCasted()
                    && me.CanCast()
                    && !e.IsLinkensProtected()
                    && !e.IsMagicImmune()
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(atos.Name)
                    && me.Distance2D(e) <= 2000
                    && Utils.SleepCheck("atos")
                    )
                {
                    atos.UseAbility(e);

                    Utils.Sleep(250, "atos");
                } // atos Item end
                if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
                {
                    urn.UseAbility(e);
                    Utils.Sleep(240, "urn");
                }
                if ( // vail
                    vail != null
                    && vail.CanBeCasted()
                    && me.CanCast()
                    && !e.IsMagicImmune()
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                    && me.Distance2D(e) <= 1500
                    && Utils.SleepCheck("vail")
                    )
                {
                    vail.UseAbility(e.Position);
                    Utils.Sleep(250, "vail");
                } // orchid Item end
                if (
                    stick != null
                    && stick.CanBeCasted()
                    && stick.CurrentCharges != 0
                    && me.Distance2D(e) <= 700
                    && (me.Health <= (me.MaximumHealth * 0.5)
                        || me.Mana <= (me.MaximumMana * 0.5))
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name))
                {
                    stick.UseAbility();
                    Utils.Sleep(200, "mana_items");
                }

                var v =
                    ObjectManager.GetEntities<Hero>()
                        .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && x.Distance2D(me) <= 700)
                        .ToList();
                if (mail != null && mail.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
                                                               (menu.Item("Heelm").GetValue<Slider>().Value)) &&
                        menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
                {
                    mail.UseAbility();
                    Utils.Sleep(100, "mail");
                }
                if (bkb != null && bkb.CanBeCasted() && (v.Count(x => x.Distance2D(me) <= 650) >=
                                                         (menu.Item("Heel").GetValue<Slider>().Value)) &&
                    menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
                {
                    bkb.UseAbility();
                    Utils.Sleep(100, "bkb");
                }

                Utils.Sleep(50, "Combo");
            }
            if (qKey && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !me.IsInvisible())
            {
                Wmod = me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");
                if (remnant.Count == 0)
                {
                    if (
                    D.CanBeCasted()
                    && Q.CanBeCasted()
                    && !Wmod
                    && ((blink == null
                    || !blink.CanBeCasted()
                    || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                    || (blink != null && me.Distance2D(e) <= 450 && blink.CanBeCasted()))
                    )
                    {
                        if (me.Distance2D(e) <= E.CastRange - 50
                            && Utils.SleepCheck("Rem"))
                        {
                            
                            D.UseAbility(Prediction.InFront(me, 50));
                            Utils.Sleep(600, "Rem");
                        }
                    }
                }
                if (remnantCount >= 1)
                {
                    for (int i = 0; i < remnantCount; ++i)
                    {
                        var r = remnant[i];
                        if (
                            D != null && D.CanBeCasted()
                            && ((Q != null && Q.CanBeCasted())
                            || (W != null && W.CanBeCasted()))
                            && !Wmod
                            && me.Distance2D(r) >= 350
                            && ((blink == null
                            || !blink.CanBeCasted()
                            || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name))
                            || (blink != null && me.Distance2D(e) <= 450 && blink.CanBeCasted()))
                            )
                        {
                            if (me.Distance2D(e) <= E.CastRange - 50
                                && Utils.SleepCheck("Rem"))
                            {
                                D.UseAbility(Prediction.InFront(me, 50));
                                Utils.Sleep(600, "Rem");
                            }
                        }
                        if (
                           me.Distance2D(r) >= 200
                           && me.Distance2D(r) <= 350
                           && Q.CanBeCasted()
                           && Utils.SleepCheck("RemMove"))
                        {
                            me.Move(r.Position);
                            Utils.Sleep(300, "RemMove");
                        }
                        if (//Q Skill
                          r != null
                          && Q != null
                          && Q.CanBeCasted()
                          && me.CanCast()
                          && me.Distance2D(e) <= E.CastRange - 50
                          && r.Distance2D(me) <= 210
                          && Utils.SleepCheck(r.Handle + "remnantQ")
                          )
                        {
                            Q.CastSkillShot(e);
                            Utils.Sleep(250, r.Handle + "remnantQ");
                        }
                    }
                }
            }
            if (wKey)
            {
                Wmod = me.HasModifier("modifier_earth_spirit_rolling_boulder_caster");
                Task.Delay(350).ContinueWith(_ =>
                {
                    if (remnant.Count == 0)
                    {
                        if (
                            D.CanBeCasted()
                            && Wmod
                            && me.Distance2D(e) >= 600
                            && Utils.SleepCheck("nextAction")
                            )
                        {
                            D.UseAbility(Prediction.InFront(me, 170));
                            Utils.Sleep(1800 + D.FindCastPoint(), "nextAction");
                        }
                    }
                });
                if (remnantCount >= 1)
                {
                    for (var i = 0; i < remnantCount; ++i)
                    {

                        if (//W Skill
                            W != null
                            && W.CanBeCasted()
                            && Game.MousePosition.Distance2D(e) <= 500
                            && me.Distance2D(e) <= W.CastRange - 200
                            && Utils.SleepCheck(me.Handle + "remnantW")
                            )
                        {
                            W.CastSkillShot(e);
                            Utils.Sleep(250, me.Handle + "remnantW");
                        }
                        else if (//W Skill
                            W != null
                            && W.CanBeCasted()
                            && Game.MousePosition.Distance2D(e) >= 500
                            && Utils.SleepCheck(me.Handle + "remnantW")
                            )
                        {
                            W.UseAbility(Game.MousePosition);
                            Utils.Sleep(250, me.Handle + "remnantW");
                        }

                        Task.Delay(350).ContinueWith(_ =>
                        {

                            var r = remnant[i];
                            if (r != null && me.Distance2D(r) >= 200)
                            {
                                if (
                                    D.CanBeCasted()
                                    && Wmod
                                    && me.Distance2D(e) >= 600

                                    && Utils.SleepCheck("nextAction")
                                    )
                                {
                                    D.UseAbility(Prediction.InFront(me, 170));
                                    Utils.Sleep(1800 + D.FindCastPoint(), "nextAction");
                                }
                            }
                        });
                    }
                }
            }
            if (eKey && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !me.IsInvisible())
            {
                if (remnant.Count == 0)
                {
                    if (
                    D.CanBeCasted()
                    && E.CanBeCasted()
                    )
                    {
                        if (me.Distance2D(e) <= E.CastRange - 50
                            && Utils.SleepCheck("Rem"))
                        {
                            D.UseAbility(e.Position);
                            Utils.Sleep(1000, "Rem");
                        }
                    }
                }
                if (remnantCount >= 1)
                {
                    for (int i = 0; i < remnantCount; ++i)
                    {
                        var r = remnant[i];

                        if (r.Distance2D(e) >= 300)
                        {
                            if (
                                D.CanBeCasted()
                                && (E != null && E.CanBeCasted())
                                   && !r.HasModifier("modifier_earth_spirit_boulder_smash")
                                   && !r.HasModifier("modifier_earth_spirit_geomagnetic_grip")
                                   )
                            {
                                if (me.Distance2D(e) <= E.CastRange - 50
                                    && Utils.SleepCheck("Rem"))
                                {
                                    D.UseAbility(e.Position);
                                    Utils.Sleep(1000, "Rem");
                                }
                            }
                        }
                        if (r != null
                           && E != null
                           && E.CanBeCasted()
                           && me.CanCast()
                           && me.Distance2D(r) < E.CastRange
                           && me.Distance2D(e) <= E.CastRange
                           )
                        {
                            if (//E Skill
                                e.Distance2D(r) <= 200
                                && Utils.SleepCheck(r.Handle + "remnantE")
                               )
                            {
                                E.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                            if (//E Skill
                              me.Distance2D(e) <= 200
                              && e.Distance2D(r) > 0
                              && me.Distance2D(r) >= e.Distance2D(r)
                              && Utils.SleepCheck(r.Handle + "remnantE")
                              )
                            {
                                E.UseAbility(r.Position);
                                Utils.Sleep(220, r.Handle + "remnantE");
                            }
                        }
                    }
                }
            }
        }
	
		public void OnCloseEvent()
		{
			Game.OnUpdate -= Others;
		}
		private static Ability Q, W, E, F, R, D;
	    private static bool Wmod;

        private static Item urn, dagon, ghost, soulring, atos, vail, sheep, cheese, stick, arcane, halberd, ethereal, orchid,
			abyssal, Shiva, mail, bkb, medall, blink;

		private static bool Active, qKey, wKey, eKey;
		private static bool AutoUlt;

		public void OnLoadEvent()
		{
			AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");
			Game.OnUpdate += Others;
			Print.LogMessage.Success("This beginning marks their end!");

			menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
			menu.AddItem(new MenuItem("qKey", "Q Spell").SetValue(new KeyBind('Q', KeyBindType.Press)));
			menu.AddItem(new MenuItem("wKey", "W Spell").SetValue(new KeyBind('W', KeyBindType.Press)));
			menu.AddItem(new MenuItem("eKey", "E Spell").SetValue(new KeyBind('E', KeyBindType.Press)));

		    menu.AddItem(
				new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    { "earth_spirit_magnetize", true}
				})));
			menu.AddItem(
				new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_ethereal_blade", true},
				    {"item_blink", true},
				    {"item_heavens_halberd", true},
				    {"item_orchid", true},
                    {"item_dagon", true},
                    {"item_urn_of_shadows", true},
				    {"item_veil_of_discord", true},
				    {"item_abyssal_blade", true},
				    {"item_bloodthorn", true},
				    {"item_blade_mail", true},
				    {"item_black_king_bar", true},
				    {"item_medallion_of_courage", true},
				    {"item_solar_crest", true}
				})));
			menu.AddItem(
				new MenuItem("Item", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
				{
				    {"item_shivas_guard", true},
				    {"item_sheepstick", true},
				    {"item_cheese", true},
				    {"item_ghost", true},
				    {"item_rod_of_atos", true},
				    {"item_soul_ring", true},
				    {"item_arcane_boots", true},
				    {"item_magic_stick", true},
				    {"item_magic_wand", true}
				})));
			menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			menu.AddItem(new MenuItem("oneult", "Use AutoUpdate Ultimate Remnant").SetValue(true));
		}
	}
}