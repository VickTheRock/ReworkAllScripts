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
    using SharpDX.Direct3D9;

    internal class NyxAssassinController : Variables, IHeroController
    {
        private readonly Menu items = new Menu("Items: ", "Items: ");
        private readonly Menu skills = new Menu("Skills: ", "Skills: ");
        private Ability Q, W, R;
        private Item sheep, vail, soulring, abyssal, mjollnir, arcane, blink, shiva, dagon, ethereal, cheese, halberd, satanic, mom, medall;
        private bool Active;

        public void Combo()
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

            //manta (when silenced)

            target = me.ClosestToMouseTarget(2000);
            if (target == null) return;
            Active = Game.IsKeyDown(menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen;
            Q = me.Spellbook.SpellQ;

            W = me.Spellbook.SpellW;

            R = me.Spellbook.SpellR;

            // item 
            sheep = target.ClassID == ClassID.CDOTA_Unit_Hero_Tidehunter ? null : me.FindItem("item_sheepstick");

            satanic = me.FindItem("item_satanic");

            shiva = me.FindItem("item_shivas_guard");

            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));

            arcane = me.FindItem("item_arcane_boots");

            mom = me.FindItem("item_mask_of_madness");

            vail = me.FindItem("item_veil_of_discord");

            medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

            ethereal = me.FindItem("item_ethereal_blade");

            blink = me.FindItem("item_blink");

            soulring = me.FindItem("item_soul_ring");

            cheese = me.FindItem("item_cheese");

            halberd = me.FindItem("item_heavens_halberd");

            abyssal = me.FindItem("item_abyssal_blade");

            mjollnir = me.FindItem("item_mjollnir");
            var stoneModif = target.HasModifier("modifier_medusa_stone_gaze_stone");
            var linkens = target.IsLinkensProtected();
            var noBlade = target.HasModifier("modifier_item_blade_mail_reflect");
            if (Active && me.IsAlive && target.IsAlive)
            {
                if (target.IsVisible && me.Distance2D(target) <= 2300 && !noBlade)
                {
                    if (
                        me.Distance2D(target) <= 300 && (!me.IsAttackImmune() || !target.IsAttackImmune())
                        && me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
                        )
                    {
                        me.Attack(target);
                        Utils.Sleep(190, "attack");
                    }
                    else if (
                         ((!me.CanAttack() && me.Distance2D(target) >= 0) || me.Distance2D(target) >= 300) && me.NetworkActivity != NetworkActivity.Attack &&
                         me.Distance2D(target) <= 1500 && Utils.SleepCheck("Move")
                         )
                    {
                        me.Move(target.Predict(350));
                        Utils.Sleep(350, "Move");
                    }
                    if (
                        R != null
                        && R.CanBeCasted()
                        && !me.HasModifier("modifier_nyx_assassin_vendetta")
                        && me.Distance2D(target) <= 1400
                        && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                        && Utils.SleepCheck("R")
                        )
                    {
                        R.UseAbility();
                        Utils.Sleep(200, "R");
                    }
                    if (me.HasModifier("modifier_nyx_assassin_vendetta"))
                        return;
                    if (!R.CanBeCasted() ||
                        R == null || !me.HasModifier("modifier_nyx_assassin_vendetta") || !menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name))
                    {
                        if (stoneModif) return;
                        float angle = me.FindAngleBetween(target.Position, true);
                        Vector3 pos = new Vector3((float)(target.Position.X - 100 * Math.Cos(angle)), (float)(target.Position.Y - 100 * Math.Sin(angle)), 0);
                        if (
                            blink != null
                            && Q.CanBeCasted()
                            && me.CanCast()
                            && blink.CanBeCasted()
                            && me.Distance2D(target) >= 300
                            && me.Distance2D(pos) <= 1180
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
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
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                            && !target.IsMagicImmune()
                            && Utils.SleepCheck("vail")
                            && me.Distance2D(target) <= 1500
                            )
                        {
                            vail.UseAbility(target.Position);
                            Utils.Sleep(250, "vail");
                        }


                        if ( // ethereal
                            ethereal != null &&
                            ethereal.CanBeCasted()
                            && (!vail.CanBeCasted()
                                || vail == null)
                            && me.CanCast() &&
                            !linkens &&
                            !target.IsMagicImmune() &&
                            Utils.SleepCheck("ethereal")
                            )
                        {
                            ethereal.UseAbility(target);
                            Utils.Sleep(150, "ethereal");
                        }


                        if ((vail == null || !vail.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(vail.Name)) && (ethereal == null || !ethereal.CanBeCasted() || !menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)))
                        {
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
                            if (Q.CanBeCasted()
                                && me.Position.Distance2D(target.Position) < Q.CastRange - 50
                                && !target.IsMagicImmune()
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                && Utils.SleepCheck("Q"))
                            {
                                Q.CastSkillShot(target);
                                Utils.Sleep(90, "Q");
                            }
                            if (W.CanBeCasted()
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                && me.Position.Distance2D(target.Position) < 800
                                && Utils.SleepCheck("W"))
                            {
                                W.UseAbility(target);
                                Utils.Sleep(60, "W");
                            }

                            if ( // SoulRing Item 
                                soulring != null &&
                                me.Health >= (me.MaximumHealth * 0.3) &&
                                me.Mana <= R.ManaCost &&
                                soulring.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soulring.Name))
                            {
                                soulring.UseAbility();
                            } // SoulRing Item end

                            if ( // Arcane Boots Item
                                arcane != null &&
                                me.Mana <= Q.ManaCost &&
                                arcane.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(arcane.Name))
                            {
                                arcane.UseAbility();
                            } // Arcane Boots Item end

                            if ( // Shiva Item
                                shiva != null &&
                                shiva.CanBeCasted() &&
                                me.CanCast() &&
                                !target.IsMagicImmune() &&
                                Utils.SleepCheck("shiva") &&
                                me.Distance2D(target) <= 600
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                )
                            {
                                shiva.UseAbility();
                                Utils.Sleep(250, "shiva");
                            } // Shiva Item end

                            if ( // Medall
                                medall != null &&
                                medall.CanBeCasted() &&
                                me.CanCast() &&
                                !target.IsMagicImmune() &&
                                Utils.SleepCheck("Medall")
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(medall.Name)
                                && me.Distance2D(target) <= 500
                                )
                            {
                                medall.UseAbility(target);
                                Utils.Sleep(250, "Medall");
                            } // Medall Item end

                            if ( // MOM
                                mom != null &&
                                mom.CanBeCasted() &&
                                me.CanCast() &&
                                Utils.SleepCheck("mom") &&
                                me.Distance2D(target) <= 700
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
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
                                && me.Distance2D(target) <= 700
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(cheese.Name)
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
                                !target.IsMagicImmune() &&
                                Utils.SleepCheck("abyssal") &&
                                me.Distance2D(target) <= 400
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(abyssal.Name)
                                )
                            {
                                abyssal.UseAbility(target);
                                Utils.Sleep(250, "abyssal");
                            } // Abyssal Item end

                            if ( // Hellbard
                                halberd != null &&
                                halberd.CanBeCasted() &&
                                me.CanCast() &&
                                !target.IsMagicImmune() &&
                                Utils.SleepCheck("halberd") &&
                                me.Distance2D(target) <= 700
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(halberd.Name)
                                )
                            {
                                halberd.UseAbility(target);
                                Utils.Sleep(250, "halberd");
                            } // Hellbard Item end

                            if ( // Dagon
                                me.CanCast()
                                && dagon != null
                                && (ethereal == null
                                    || (target.HasModifier("modifier_item_ethereal_blade_slow")
                                        || ethereal.Cooldown < 17))
                                && !target.IsLinkensProtected()
                                && dagon.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && !target.IsMagicImmune()
                                && !stoneModif
                                && Utils.SleepCheck("dagon")
                                )
                            {
                                dagon.UseAbility(target);
                                Utils.Sleep(200, "dagon");
                            } // Dagon Item end


                            if ( // Mjollnir
                                mjollnir != null &&
                                mjollnir.CanBeCasted() &&
                                me.CanCast() &&
                                !target.IsMagicImmune() &&
                                Utils.SleepCheck("mjollnir")
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
                                && me.Distance2D(target) <= 900
                                )
                            {
                                mjollnir.UseAbility(me);
                                Utils.Sleep(250, "mjollnir");
                            } // Mjollnir Item end


                            if ( // Satanic 
                                satanic != null &&
                                me.Health <= (me.MaximumHealth * 0.3) &&
                                satanic.CanBeCasted() &&
                                me.Distance2D(target) <= 700 &&
                                Utils.SleepCheck("Satanic")
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
                                )
                            {
                                satanic.UseAbility();
                                Utils.Sleep(350, "Satanic");
                            } // Satanic Item end
                        }
                    }
                }
            }
        }

        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1b");

            Print.LogMessage.Success("My purpose is clear, my targets doomed.");

            menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            menu.AddSubMenu(items);
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
            menu.AddSubMenu(skills);
            skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"nyx_assassin_impale", true},
                {"nyx_assassin_mana_burn", true},
                {"nyx_assassin_vendetta", true}
            })));
            Console.WriteLine(">Nyx by VickTheRock loaded!");
        }

        public void OnCloseEvent()
        {

        }
    }
}
