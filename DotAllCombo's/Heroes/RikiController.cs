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
    using SharpDX.Direct3D9;

    using Service;
    using Service.Debug;

    internal class RikiController : Variables, IHeroController
    {
        private static Ability Q, W, R;
        private static Item urn, dagon, diff,  mjollnir, orchid, abyssal, mom, Shiva, mail, bkb, satanic, medall, blink;
        private static Font txt;
		private static Font not;
        private static bool Active;

        private static bool loaded;
        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

            Print.LogMessage.Success("Into the shadows.");
            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("keyBind", "Combo key").SetValue(new KeyBind('D', KeyBindType.Press)));
            
            menu.AddItem(
                new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"riki_smoke_screen", true},
                    {"riki_blink_strike", true},
                    {"riki_tricks_of_the_trade", true}
                })));
            menu.AddItem(
                new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
                {
                    {"item_blink", true},
                    {"item_diffusal_blade", true},
                    {"item_diffusal_blade_2", true},
                    {"item_heavens_halberd", true},
                    {"item_orchid", true},
                    {"item_bloodthorn", true},
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
            menu.AddItem(new MenuItem("Heel", "Min targets to BKB").SetValue(new Slider(2, 1, 5)));
            menu.AddItem(new MenuItem("Ult", "Min targets to Ultimate").SetValue(new Slider(3, 1, 5)));
            menu.AddItem(new MenuItem("Heelm", "Min targets to BladeMail").SetValue(new Slider(2, 1, 5)));
			
			txt = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Tahoma",
				   Height = 12,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.Default
			   });

			not = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Tahoma",
				   Height = 170,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.Default
			   });

			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			Drawing.OnEndScene += Drawing_OnEndScene;
			AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
		}

        public void Combo()
        {
            
            me = ObjectManager.LocalHero;
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_Riki)return;

            if (!menu.Item("enabled").IsActive()) return;
            e = me.ClosestToMouseTarget(1800);
            if (e == null) return;
            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key) && !Game.IsChatOpen;

            Q = me.Spellbook.SpellQ;
            W = me.Spellbook.SpellW;
            R = me.Spellbook.SpellR;
            Shiva = me.FindItem("item_shivas_guard");
            mom = me.FindItem("item_mask_of_madness");
            diff = me.FindItem("item_diffusal_blade")?? me.FindItem("item_diffusal_blade_2");
            urn = me.FindItem("item_urn_of_shadows");
            dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
            mjollnir = me.FindItem("item_mjollnir");
            orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
            abyssal = me.FindItem("item_abyssal_blade");
            mail = me.FindItem("item_blade_mail");
            bkb = me.FindItem("item_black_king_bar");
            satanic = me.FindItem("item_satanic");
            blink = me.FindItem("item_blink");
            medall = me.FindItem("item_medallion_of_courage") ?? me.FindItem("item_solar_crest");

            var stoneModif = e.Modifiers.All(y => y.Name == "modifier_medusa_stone_gaze_stone");
            

            var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                    .ToList();

            if (me.HasModifier("modifier_riki_tricks_of_the_trade_phase") || R.IsInAbilityPhase) return;
            if (Active && me.Distance2D(e) <= 1700 && e.IsAlive)
            {
                if (me.HasModifier("modifier_riki_tricks_of_the_trade_phase") || R.IsInAbilityPhase) return;
                if (
                    me.Distance2D(e) <= 300 && (!me.IsAttackImmune() || !e.IsAttackImmune())
                    && me.NetworkActivity != NetworkActivity.Attack && me.CanAttack() && Utils.SleepCheck("attack")
                    )
                {
                    me.Attack(e);
                    Utils.Sleep(190, "attack");
                }
           else if (
                    (!me.CanAttack() || me.Distance2D(e) >= 0) && me.NetworkActivity != NetworkActivity.Attack &&
                    me.Distance2D(e) <= 1500 && Utils.SleepCheck("Move")
                    )
                {
                    me.Move(e.Predict(350));
                    Utils.Sleep(350, "Move");
                }
            }
            if (Active && me.Distance2D(e) <= 1400 && e.IsAlive && (!me.IsInvisible() || me.IsVisibleToEnemies || e.Health <= (me.MaximumHealth * 0.5)))
            {
                if (me.HasModifier("modifier_riki_tricks_of_the_trade_phase") || R.IsInAbilityPhase) return;

                if (stoneModif) return;
                if (e.IsVisible && me.Distance2D(e) <= 1200)
                {

                    if (diff != null
                        && diff.CanBeCasted()
                        && diff.CurrentCharges > 0
                        && me.Distance2D(e) <= 400
                        && !e.HasModifier("modifier_item_diffusal_blade_slow")
                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(diff.Name) &&
                        Utils.SleepCheck("diff"))
                    {
                        diff.UseAbility(e);
                        Utils.Sleep(350, "diff");
                    }
                    else if (
                        Q != null
                        && Q.CanBeCasted()
                        && me.Distance2D(e) <= 300
                        && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                        && Utils.SleepCheck("Q")
                        )
                    {
                        Q.UseAbility(Prediction.InFront(e, 50));
                        Utils.Sleep(200, "Q");
                    }
                    if (
                        W != null && W.CanBeCasted()
                        && me.Distance2D(e) <= W.CastRange
                        && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                        && Utils.SleepCheck("W")
                        )
                    {
                        W.UseAbility(e);
                        Utils.Sleep(200, "W");
                    }
                    if (
                        blink != null
                        && me.CanCast()
                        && blink.CanBeCasted()
                        && me.Distance2D(e) < 1180
                        && me.Distance2D(e) > 300
                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                        && Utils.SleepCheck("blink")
                        )
                        {
                            blink.UseAbility(e.Position);
                            Utils.Sleep(250, "blink");
                        }

                        if ( // Abyssal Blade
                            abyssal != null
                            && abyssal.CanBeCasted()
                            && me.CanCast()
                            && !e.IsStunned()
                            && !e.IsHexed()
                            && Utils.SleepCheck("abyssal")
                            && me.Distance2D(e) <= 400
                            )
                        {
                            abyssal.UseAbility(e);
                            Utils.Sleep(250, "abyssal");
                        } // Abyssal Item end
                        if ( // Mjollnir
                           mjollnir != null
                           && mjollnir.CanBeCasted()
                           && me.CanCast()
                           && !e.IsMagicImmune()
                           && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mjollnir.Name)
                           && Utils.SleepCheck("mjollnir")
                           && me.Distance2D(e) <= 900
                           )
                        {
                            mjollnir.UseAbility(me);
                            Utils.Sleep(250, "mjollnir");
                        } // Mjollnir Item end
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

                        if ( // MOM
                            mom != null
                            && mom.CanBeCasted()
                            && me.CanCast()
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mom.Name)
                            && Utils.SleepCheck("mom")
                            && me.Distance2D(e) <= 700
                            )
                        {
                            mom.UseAbility();
                            Utils.Sleep(250, "mom");
                        }
                        if (orchid != null && orchid.CanBeCasted() && me.Distance2D(e) <= 900 &&
                            menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name) && Utils.SleepCheck("orchid"))
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

                        if ( // Dagon
                            me.CanCast()
                            && dagon != null
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


                        if (urn != null && urn.CanBeCasted() && urn.CurrentCharges > 0 && me.Distance2D(e) <= 400
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(urn.Name) && Utils.SleepCheck("urn"))
                        {
                            urn.UseAbility(e);
                            Utils.Sleep(240, "urn");
                        }
                        if ( // Satanic 
                            satanic != null &&
                            me.Health <= (me.MaximumHealth * 0.3) &&
                            satanic.CanBeCasted() &&
                            me.Distance2D(e) <= me.AttackRange + 50
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(satanic.Name)
                            && Utils.SleepCheck("satanic")
                            )
                        {
                            satanic.UseAbility();
                            Utils.Sleep(240, "satanic");
                        } // Satanic Item end
                        if (mail != null && mail.CanBeCasted() 
                        && ((v.Count(x => x.Distance2D(me) <= 650) >= (menu.Item("Heelm").GetValue<Slider>().Value)) 
                        || me.HasModifier("modifier_skywrath_mystic_flare_aura_effect")) &&
                            menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(mail.Name) && Utils.SleepCheck("mail"))
                        {
                            mail.UseAbility();
                            Utils.Sleep(100, "mail");
                        }
                        if (bkb != null && bkb.CanBeCasted() && ((v.Count(x => x.Distance2D(me) <= 650) >=
                                                                 (menu.Item("Heel").GetValue<Slider>().Value))
                        || me.HasModifier("modifier_skywrath_mystic_flare_aura_effect")) &&
                            menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(bkb.Name) && Utils.SleepCheck("bkb"))
                        {
                            bkb.UseAbility();
                            Utils.Sleep(100, "bkb");
                        }
                    if (R != null && R.CanBeCasted()
                        && (v.Count(x => x.Distance2D(me) <= 500)
                        >= (menu.Item("Ult").GetValue<Slider>().Value))
                        && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                        && Utils.SleepCheck("R"))
                    {
                        R.UseAbility();
                        Utils.Sleep(100, "R");
                    }
                }
			}
		}

        public void OnCloseEvent()
        {
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;

        }


        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
		{
			txt.Dispose();
			not.Dispose();
		}

		static void Drawing_OnEndScene(EventArgs args)
		{
			if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
				return;

			var player = ObjectManager.LocalPlayer;
			var me = ObjectManager.LocalHero;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_Riki)
				return;

			if (Active)
			{
				txt.DrawText(null, "Riki: Comboing!", 4, 150, Color.Coral);
			}
		}

		static void Drawing_OnPostReset(EventArgs args)
		{
			txt.OnResetDevice();
			not.OnResetDevice();
		}

		static void Drawing_OnPreReset(EventArgs args)
		{
			txt.OnLostDevice();
			not.OnLostDevice();
		}
	}
}



