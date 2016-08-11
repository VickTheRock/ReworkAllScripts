namespace DotaAllCombo.Heroes
{
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;

    using Service;
    using Service.Debug;

    internal class TuskController : Variables, IHeroController
    {
        
        private Ability Q, W, E, R;

        private Item blink, mjollnir, medall, urn, mail, bkb, abyssal, satanic, halberd, dagon, shiva, soul, arcane, mom;

        //private int[] qDmg = new int[4] {40, 80, 120, 160};

        public void Combo()
        {
            Active = Game.IsKeyDown(Menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            e = me.ClosestToMouseTarget(2000);

            if (!Menu.Item("enabled").IsActive())
                return;
            if (e == null) return;
            if (Active && e.IsAlive && !e.IsInvul() && !e.IsIllusion)
            {
                Q = me.Spellbook.SpellQ;

				W = me.Spellbook.SpellW ?? me.FindSpell("tusk_launch_snowball");

				E = me.Spellbook.SpellE;

                R = me.Spellbook.SpellR;

                urn = me.FindItem("item_urn_of_shadows");

                blink = me.FindItem("item_blink");

                satanic = me.FindItem("item_satanic");

                dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

                halberd = me.FindItem("item_heavens_halberd");

                medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

                abyssal = me.FindItem("item_abyssal_blade");

                mjollnir = me.FindItem("item_mjollnir");

                soul = me.FindItem("item_soul_ring");

                arcane = me.FindItem("item_arcane_boots");

                mom = me.FindItem("item_mask_of_madness");

                shiva = me.FindItem("item_shivas_guard");

                mail = me.FindItem("item_blade_mail");

                bkb = me.FindItem("item_black_king_bar");

                var v =
                    ObjectManager.GetEntities<Hero>()
                        .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                        .ToList();

                var linkens = e.IsLinkensProtected();
                var ModifW = me.HasModifier("modifier_tusk_snowball_movement");
                var medallModiff =
                    e.HasModifier("modifier_item_medallion_of_courage_armor_reduction") ||
                    e.HasModifier("modifier_item_solar_crest_armor_reduction");

				

                if (!me.IsInvisible())
                {
                    if ( // Q Skill
                    Q != null
                    && Q.CanBeCasted()
                    && me.CanCast()
                    && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                    && ModifW
                    && !e.IsMagicImmune()
                    && me.Distance2D(e) <= 300
                    && Utils.SleepCheck("Q")
                    )
                    {
                        Q.UseAbility(e.Predict(400));
                        Utils.Sleep(200, "Q");
                    } // Q Skill end

                    if ( //R Skill
                        R != null
                        && (medallModiff
                        || e.IsMagicImmune()
                        || medall == null)
                        && R.CanBeCasted()
                        && me.CanCast()
                        && !linkens
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                        && me.Distance2D(e) <= 700
                        && Utils.SleepCheck("R")
                        )
                    {
                        R.UseAbility(e);
                        Utils.Sleep(150, "R");
                    } // R Skill end


                    if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 800 &&
                        Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
                    {
                        urn.UseAbility(e);
                        Utils.Sleep(240, "urn");
                    }
                    float angle = me.FindAngleBetween(e.Position, true);
                    Vector3 pos = new Vector3((float)(e.Position.X + 30 * Math.Cos(angle)), (float)(e.Position.Y + 30 * Math.Sin(angle)), 0);
                    if (
                        blink != null
                        && Q.CanBeCasted()
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(pos) >= me.GetAttackRange()+me.HullRadius+24
                        && me.Distance2D(pos) <= 1190
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                    {
                        blink.UseAbility(pos);
                        Utils.Sleep(250, "blink");
                    }
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
                    if ( // E Skill
                        E != null
                        && E.CanBeCasted()
                        && me.CanCast()
                        && ModifW
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
                        && !e.IsMagicImmune()
                        && me.Distance2D(e) <= 500
                        && Utils.SleepCheck("E")
                        )
                    {
                        E.UseAbility();
                        Utils.Sleep(350, "E");
                    } // E Skill end
                    if ( // SoulRing Item 
                        soul != null
                        && me.Mana <= Q.ManaCost
                        && soul.CanBeCasted()
                        && Utils.SleepCheck("soul")
                        )
                    {
                        soul.UseAbility();
                        Utils.Sleep(250, "soul");
                    } // SoulRing Item end

                    if ( // Arcane Boots Item
                        arcane != null
                        && me.Mana <= Q.ManaCost
                        && arcane.CanBeCasted()
                        && Utils.SleepCheck("arcane")
                        )
                    {
                        arcane.UseAbility();
                        Utils.Sleep(250, "arcane");
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

                    if ( // MOM
                        mom != null &&
                        mom.CanBeCasted()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
                        && me.CanCast()
                        && Utils.SleepCheck("mom")
                        && me.Distance2D(e) <= 700
                        )
                    {
                        mom.UseAbility();
                        Utils.Sleep(250, "mom");
                    } // MOM Item end

                    if ( // Medall
                        medall != null &&
                        medall.CanBeCasted() &&
                        !me.IsInvisible()
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
                        && Utils.SleepCheck("Medall")
                        && me.Distance2D(e) <= 500
                        )
                    {
                        medall.UseAbility(e);
                        Utils.Sleep(250, "Medall");
                    } // Medall Item end


                    if ( // Hellbard
                        halberd != null
                        && halberd.CanBeCasted()
                        && me.CanCast()
                        && !e.IsMagicImmune()
                        && Utils.SleepCheck("halberd")
                        && me.Distance2D(e) <= 700
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
                        )
                    {
                        halberd.UseAbility(e);
                        Utils.Sleep(250, "halberd");
                    } // Hellbard Item end

                    if ( // Mjollnir
                        mjollnir != null
                        && mjollnir.CanBeCasted()
                        && me.CanCast()
                        && !e.IsMagicImmune()
                        && Utils.SleepCheck("mjollnir")
                        && me.Distance2D(e) <= 900
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
                        )
                    {
                        mjollnir.UseAbility(me);
                        Utils.Sleep(250, "mjollnir");
                    } // Mjollnir Item end

                    if ( // Dagon
                        dagon != null
                        && dagon.CanBeCasted()
                        && me.CanCast()
                        && !e.IsMagicImmune()
                        && Utils.SleepCheck("dagon")
                        )
                    {
                        dagon.UseAbility(e);
                        Utils.Sleep(150, "dagon");
                    } // Dagon Item end


                    if ( // Satanic 
                        satanic != null
                        && me.Health <= (me.MaximumHealth * 0.3)
                        && satanic.CanBeCasted()
                        && me.Distance2D(e) <= 300
                        && Utils.SleepCheck("satanic")
                        && Menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
                        )
                    {
                        satanic.UseAbility();
                        Utils.Sleep(150, "satanic");
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


                    if ( // W Skill
                        W != null
                        && W.CanBeCasted()
                        && !e.IsMagicImmune()
                        && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                        && Utils.SleepCheck("W")
                        )
                    {
                        W.UseAbility(e);
                        W.UseAbility();
                        Utils.Sleep(120, "W");
                    }

                    var Sigl = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
                                                                        && x.IsAlive && x.IsControllable);

                    if (Menu.Item("SiglControl").IsActive() && Sigl != null)
                    {
                        if (e.Position.Distance2D(Sigl.Position) < 1550 &&
                                    Utils.SleepCheck(Sigl.Handle.ToString()))
                        {
                            Sigl.Move(Prediction.InFront(e, 350));
                            Utils.Sleep(350, Sigl.Handle.ToString());
                        }
                    }
                }
                if (Menu.Item("orbwalk").GetValue<bool>() && me.Distance2D(e) <= 1900)
                {
                    Orbwalking.Orbwalk(e, 0, 1600, true, true);
                }
            }
            if (Active)
            {
                var ModifW = me.HasModifier("modifier_tusk_snowball_movement");
                if (ModifW && Menu.Item("SnowBall").IsActive())
                {

                    var teamarm = ObjectManager.GetEntities<Hero>().Where(ally =>
                        ally.Team == me.Team && ally.IsAlive && me.Distance2D(ally) <= 395
                        && ally.Health >= (ally.MaximumHealth * 0.4)
                        && !ally.HasModifier("modifier_tusk_snowball_movement_friendly")).ToList();

                    var unitToSnow =
                        ObjectManager.GetEntities<Unit>().Where(x =>
                        ((x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                        || x.ClassID == ClassID.CDOTA_Unit_SpiritBear
                        || x.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                        || x.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                        || x.ClassID == ClassID.CDOTA_BaseNPC_Creep)
                        && !x.IsAttackImmune() && !x.IsInvul() && x.IsVisible
                        && x.IsAlive && me.Distance2D(x) <= 395)
                        && x.IsAlive && x.IsControllable
                        && !x.HasModifier("modifier_tusk_snowball_movement_friendly")
                        && !x.HasModifier("modifier_tusk_snowball_movement")).ToList();
                    if (teamarm != null)
                    {
                        foreach (Hero v in teamarm)
                        {
                            if (ModifW && v.Distance2D(me) < 395 &&
                                !v.HasModifier("modifier_tusk_snowball_movement_friendly") && !v.IsInvul() &&
                                !v.IsAttackImmune() && v.IsAlive && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                me.Attack(v);
                                Utils.Sleep(200, v.Handle.ToString());
                            }
                        }
                    }
                    if (unitToSnow != null)
                    {
                        foreach (Unit v in unitToSnow)
                        {
                            if (ModifW && v.Distance2D(me) < 395 &&
                                !v.HasModifier("modifier_tusk_snowball_movement_friendly") && !v.IsInvul() &&
                                !v.IsAttackImmune() && v.IsAlive && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                me.Attack(v);
                                Utils.Sleep(200, v.Handle.ToString());
                            }
                        }
                    }
                }
            }
        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.01");

            Print.LogMessage.Success("Who's ready for a fight? The first hit is free! Anyone? One-Punch Man! xD");

			Menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
			Menu.AddItem(new MenuItem("orbwalk", "orbwalk").SetValue(true));
			Menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("SiglControl", "SiglControl").SetValue(true));
            Menu.AddItem(new MenuItem("SnowBall", "Pick up allies in SnowBall").SetValue(true));
            Menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"tusk_launch_snowball", true},
                    {"tusk_ice_shards", true},
                    {"tusk_frozen_sigil", true},
                    {"tusk_snowball", true},
                    {"tusk_walrus_punch", true}
                })));
            Menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_blink", true},
                    {"item_heavens_halberd", true},
                    {"item_orchid", true},
                    { "item_bloodthorn", true},
                    {"item_urn_of_shadows", true},
                    {"item_veil_of_discord", true},
                    {"item_abyssal_blade", true},
                    {"item_shivas_guard", true},
                    {"item_blade_mail", true},
                    {"item_black_king_bar", true},
                    {"item_satanic", true},
                    {"item_medallion_of_courage", true},
                    {"item_solar_crest", true}
                })));
            Menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            Menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
        }

        public void OnCloseEvent()
        {

        }
    }
}