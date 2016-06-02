namespace DotaAllCombo.Heroes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;
    using SharpDX;
    using Service;
    using Service.Debug;

    internal class AntimageController : Variables, IHeroController
    {
        private Ability W, R;
        private Item urn, dagon, soulring, phase, cheese, halberd, ethereal, arcane,
            mjollnir, orchid, abyssal, stick, mom, Shiva, mail, bkb, satanic, medall, blink, sheep;
        private bool Active;
        private readonly double[] ult = { 0, 0.6, 0.85, 1.1 };
        private float scaleX, scaleY;
        public void Combo()
        {
            if (!menu.Item("enabled").IsActive())
                return;

            e = me.ClosestToMouseTarget(1800);
            if (e == null)
                return;
            W = me.Spellbook.SpellW;
            R = me.Spellbook.SpellR;
            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);

            Shiva = me.FindItem("item_shivas_guard");
            ethereal = me.FindItem("item_ethereal_blade");
            mom = me.FindItem("item_mask_of_madness");
            urn = me.FindItem("item_urn_of_shadows");
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
            soulring = me.FindItem("item_soul_ring");
            arcane = me.FindItem("item_arcane_boots");
            stick = me.FindItem("item_magic_stick") ?? me.FindItem("item_magic_wand");
            phase = me.FindItem("item_phase_boots");
            var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                    .ToList();

            var ModifEther = e.Modifiers.Any(y => y.Name == "modifier_item_ethereal_blade_slow");
            var stoneModif = e.Modifiers.Any(y => y.Name == "modifier_medusa_stone_gaze_stone");
            if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive)
            {
                if (
                    me.Distance2D(e) <= me.AttackRange + 100 && (!me.IsAttackImmune() || !e.IsAttackImmune())
                    && me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
                    )
                {
                    me.Attack(e);
                    Utils.Sleep(150, "attack");
                }
                else if (
                    (!me.CanAttack() || me.Distance2D(e) >= 0) && me.NetworkActivity != NetworkActivity.Attack &&
                    me.Distance2D(e) <= 600 && Utils.SleepCheck("Move")
                    )
                {
                    me.Move(e.Predict(400));
                    Utils.Sleep(390, "Move");
                }
            }
            if (Active && me.Distance2D(e) <= 1400 && e != null && e.IsAlive && !me.IsInvisible())
            {
                if (
                       W != null 
                       && W.CanBeCasted() 
                       && me.Distance2D(e) <= 700
                       && me.Distance2D(e) >= me.AttackRange+25
                       && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                       && Utils.SleepCheck("W")
                       )
                {
                    W.UseAbility(Prediction.InFront(e, me.AttackRange+40));
                    Utils.Sleep(200, "W");
                }

                if ( // MOM
                    mom != null
                    && mom.CanBeCasted()
                    && me.CanCast()
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mom.Name)
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
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
                    )
                {
                    halberd.UseAbility(e);
                    Utils.Sleep(250, "halberd");
                }
                if ( // Arcane Boots Item
                    arcane != null
                    && me.Mana <= R.ManaCost
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(arcane.Name)
                    && arcane.CanBeCasted()
                    && Utils.SleepCheck("arcane")
                    )
                {
                    arcane.UseAbility();
                    Utils.Sleep(250, "arcane");
                } // Arcane Boots Item end
                if ( // Mjollnir
                    mjollnir != null
                    && mjollnir.CanBeCasted()
                    && me.CanCast()
                    && !e.IsMagicImmune()
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
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
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(Shiva.Name)
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
                        || (ModifEther
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
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(phase.Name)
                    && !blink.CanBeCasted()
                    && me.Distance2D(e) >= me.AttackRange + 20)
                {
                    phase.UseAbility();
                    Utils.Sleep(200, "phase");
                }
                if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
                    && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
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
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(stick.Name))
                {
                    stick.UseAbility();
                    Utils.Sleep(200, "mana_items");
                }
                if ( // Satanic 
                    satanic != null &&
                    me.Health <= (me.MaximumHealth * 0.3) &&
                    satanic.CanBeCasted() &&
                    me.Distance2D(e) <= me.AttackRange + 50
                    && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
                    && Utils.SleepCheck("satanic")
                    )
                {
                    satanic.UseAbility();
                    Utils.Sleep(240, "satanic");
                } // Satanic Item end
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
            }
        } // Combo

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

            Print.LogMessage.Success("Blood!");

            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));

            var ul = new Menu("AutoUlt", "ult");
            menu.AddItem(new MenuItem("hpdraw", "Draw HP Bar").SetValue(true)
                                             .SetTooltip("Will show ulti damage on HP Bar"));
            menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"antimage_blink", true},
                    {"antimage_mana_void", true}
                })));
            menu.AddItem(
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
            menu.AddItem(
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
                   {"item_phase_boots", true}
               })));
            menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
            menu.AddSubMenu(ul);
            ul.AddItem(new MenuItem("autoUlt", "EnableAutoUlt").SetValue(true));
            ul.AddItem(new MenuItem("ulti", "Min targets to ult").SetValue(new Slider(2, 1, 5)));

            Drawing.OnDraw += DrawUltiDamage;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        private void DrawUltiDamage(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
            {
                return;
            }
            

            R = me.Spellbook.SpellR;
            var ultLvl = R.Level;
            var enemy =
                ObjectManager.GetEntities<Hero>()
                    .Where(y => y.Team != me.Team && y.IsAlive && y.IsVisible && !y.IsIllusion)
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
                var rum = v.Modifiers.Any(x => x.Name == "modifier_kunkka_ghost_ship_damage_absorb");
                if (rum) damage = damage * 0.5;
                var mom = v.Modifiers.Any(x => x.Name == "modifier_item_mask_of_madness_berserk");
                if (mom) damage = damage * 1.3;
                var dmg = v.Health - damage;
                var canKill = dmg <= 0;
                var screenPos = HUDInfo.GetHPbarPosition(v);
                if (!OnScreen(v.Position)) continue;

                var text = canKill ? "Yes" : "No, damage:" + Math.Floor(damage);
                var size = new Vector2(15, 15);
                var textSize = Drawing.MeasureText(text, "Arial", size, FontFlags.AntiAlias);
                var position = new Vector2(screenPos.X - textSize.X - 2, screenPos.Y - 3);
                Drawing.DrawText(
                    text,
                    position,
                    size,
                    (canKill ? Color.LawnGreen : Color.Red),
                    FontFlags.AntiAlias);
                if (menu.Item("autoUlt").GetValue<bool>() && me.IsAlive)
                {
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
                        && me.Distance2D(v) <= R.GetCastRange() + 50
                        && !v.IsMagicImmune()
                        && enemy.Count(x => (x.Health - damage) <= 0 && x.Distance2D(v) <= 500)
                        >= (menu.Item("ulti").GetValue<Slider>().Value)
                        && enemy.Count(x => x.Distance2D(v) <= 500)
                        >= (menu.Item("ulti").GetValue<Slider>().Value)
                        && Utils.SleepCheck(v.Handle.ToString()))
                    {
                        R.UseAbility(v);
                        Utils.Sleep(150, v.Handle.ToString());
                        return;
                    }
                }
            }

        }
        private bool OnScreen(Vector3 v)
        {
            return !(Drawing.WorldToScreen(v).X < 0 || Drawing.WorldToScreen(v).X > Drawing.Width || Drawing.WorldToScreen(v).Y < 0 || Drawing.WorldToScreen(v).Y > Drawing.Height);
        }
        private void Drawing_OnDraw(EventArgs args)
        {
            if (!Game.IsInGame)
                return;
            if (menu.Item("hpdraw").GetValue<bool>())
            {
                if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                {
                    return;
                }
            
                var ultLvl = me.Spellbook.SpellR.Level;
                var enemy =
                    ObjectManager.GetEntities<Hero>()
                        .Where(y => y.Team != me.Team && y.IsAlive && y.IsVisible && !y.IsIllusion)
                        .ToList();

                foreach (var x in enemy)
                {
                    //Console.WriteLine(1);
                    var health = x.Health;
                    var maxHealth = x.MaximumHealth;
                
                    var manna = (x.MaximumMana - x.Mana);
                    var damage = Math.Floor((manna * ult[ultLvl]) * (1 - x.MagicDamageResist));
                    var hpleft = health;
                    var hpperc = hpleft / maxHealth;

                    var dmgperc = Math.Min(damage, health) / maxHealth;
                    Vector2 hbarpos;
                    hbarpos = HUDInfo.GetHPbarPosition(x);

                    Vector2 screenPos;
                    var enemyPos = x.Position + new Vector3(0, 0, x.HealthBarOffset);
                    if (!Drawing.WorldToScreen(enemyPos, out screenPos)) continue;

                    var start = screenPos;


                    hbarpos.X = start.X + (HUDInfo.GetHPBarSizeX(x) / 2);
                    hbarpos.Y = start.Y;
                    var hpvarx = hbarpos.X;
                    var hpbary = hbarpos.Y;
                    float a = (float)Math.Round((damage * HUDInfo.GetHPBarSizeX(x)) / (x.MaximumHealth));
                    var position = hbarpos - new Vector2(a, 32 * scaleY);

                    //Console.WriteLine("damage" + damage.ToString());

                    try
                    {
                        float left = (float)Math.Round(damage / 7);
                        Drawing.DrawRect(
                            position,
                            new Vector2(a, (float)(HUDInfo.GetHpBarSizeY(x))),
                            (x.Health > 0) ? new Color(150, 225, 150, 80) : new Color(70, 225, 150, 225));
                        Drawing.DrawRect(position, new Vector2(a,(HUDInfo.GetHpBarSizeY(x))), Color.Black, true);
                    }
                    catch (Exception v)
                    {
                        Console.WriteLine(v.Message);
                    }
                }
            }

        }
        public void OnCloseEvent()
        {
            Drawing.OnDraw -= DrawUltiDamage;
            Drawing.OnDraw -= Drawing_OnDraw;
        }
    }
}