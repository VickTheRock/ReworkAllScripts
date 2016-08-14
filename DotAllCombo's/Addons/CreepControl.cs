namespace DotaAllCombo.Addons
{
    using System.Security.Permissions;
    using Ensage.Common.Menu;
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using SharpDX;
    using System;
    using System.Linq;
    using SharpDX.Direct3D9;
    using Service;

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class CreepControl : IAddon
    {
        private Item midas, abyssal, mjollnir, boots, medall, mom;
        private Hero e = null;
        private Hero TargetLock = null;

        private string test_meDistant;
        private string test_allyDistant;
        private static bool IsTeamHaveDisable = false;
        private Font txt;
        private Font not;
        private Hero me;

        public void Load()
        {
            txt = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Tahoma",
                   Height = 11,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.Default
               });

            not = new Font(
               Drawing.Direct3DDevice9,
               new FontDescription
               {
                   FaceName = "Tahoma",
                   Height = 12,
                   OutputPrecision = FontPrecision.Default,
                   Quality = FontQuality.Default
               });

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;

            OnLoadMessage();
        }

        public void Unload()
        {
            AppDomain.CurrentDomain.DomainUnload -= CurrentDomain_DomainUnload;
            Drawing.OnEndScene -= Drawing_OnEndScene;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnPreReset -= Drawing_OnPreReset;
        }

        public void RunScript()
        {
            me = ObjectManager.LocalHero;
            if (!MainMenu.CCMenu.Item("controll").IsActive() || !Game.IsInGame || me == null || Game.IsPaused ||
                Game.IsChatOpen) return;


            var holdKey = MainMenu.CCMenu.Item("Press Key").GetValue<KeyBind>().Active;
            var toggleKey = MainMenu.CCMenu.Item("Toogle Key").GetValue<KeyBind>().Active;
            var lockTargetKey = MainMenu.CCMenu.Item("Lock target Key").GetValue<KeyBind>().Active;

            var TargetMode = MainMenu.CCMenu.Item("Target mode").GetValue<StringList>().SelectedIndex;
            
            var TargetFindSource = MainMenu.CCMenu.Item("Target find source").GetValue<StringList>().SelectedIndex;
            var TargetFindRange = MainMenu.CCMenu.Item("Target find range").GetValue<Slider>().Value;
            var units = ObjectManager.GetEntities<Unit>().Where(creep =>
                (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                || creep.ClassID == ClassID.CDOTA_BaseNPC_Additive
                || creep.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil
                || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                || creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
                || creep.ClassID == ClassID.CDOTA_Unit_VisageFamiliar
                || creep.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth
                || creep.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm
                || creep.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalFire
                || creep.ClassID == ClassID.CDOTA_NPC_WitchDoctor_Ward
                || creep.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster_Boar
                || creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
                || creep.ClassID == ClassID.CDOTA_BaseNPC_Venomancer_PlagueWard
                || creep.ClassID == ClassID.CDOTA_BaseNPC_ShadowShaman_SerpentWard
                || creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                || creep.ClassID == ClassID.CDOTA_Unit_Elder_Titan_AncestralSpirit
                || creep.IsIllusion
                )
                && creep.IsAlive
                && creep.Team == me.Team
                && creep.IsControllable).ToList();
            if (lockTargetKey)
            {
                TargetLock = TargetSelector.ClosestToMouse(me, TargetFindRange);
            }

            if (TargetLock != null)
            {
                if (TargetLock.IsAlive)
                {
                    e = TargetLock;
                }
                else
                {
                    switch (TargetMode)
                    {
                        case 0:
                            switch (TargetFindSource)
                            {
                                case 0:
                                    var EnemyHero0 = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(Game.MousePosition) <= TargetFindRange).ToList();
                                    e = EnemyHero0.MinOrDefault(x => x.Distance2D(me.Position));
                                    break;
                                case 1:
                                    e = TargetSelector.ClosestToMouse(me, 5000);
                                    break;
                            }
                            break;
                        case 1:
                            switch (TargetFindSource)
                            {
                                case 0:
                                    var EnemyHero0 = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(Game.MousePosition) <= TargetFindRange).ToList();
                                    e = EnemyHero0.MinOrDefault(x => x.Health);
                                    break;
                                case 1:
                                    var EnemyHero1 = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(me.Position) <= TargetFindRange).ToList();
                                    e = EnemyHero1.MinOrDefault(x => x.Health);
                                    break;
                            }
                            break;
                    }
                }
            }
            else
            {
                switch (TargetMode)
                {
                    case 0:
                        e = TargetSelector.ClosestToMouse(me, 5000);
                        break;
                    case 1:
                        var EnemyHero = ObjectManager.GetEntities<Hero>().Where(enemy => enemy.Team == me.GetEnemyTeam() && enemy.IsAlive && !enemy.IsIllusion && enemy.Distance2D(Game.MousePosition) <= TargetFindRange).ToList();
                        e = EnemyHero.MinOrDefault(x => x.Health);
                        break;
                }
            }
            if (Utils.SleepCheck("delay"))
            {
                if (me.IsAlive)
                {
                    var count = units.Count();
                    if (count <= 0) return;
                    for (int i = 0; i < count; ++i)
                    {
                        var v = ObjectManager.GetEntities<Hero>()
                                          .Where(x => x.Team == me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion).ToList();
                       if (units[i].Name == "npc_dota_juggernaut_healing_ward")

                            {
                                if (me.Position.Distance2D(units[i].Position) > 5 && Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Move(me.Position);
                                    Utils.Sleep(50, units[i].Handle.ToString());
                                }
                        }
                        else if (units[i].Name == "npc_dota_neutral_ogre_magi")
                        {
                            for (int z = 0; z < v.Count(); ++z)
                            {
                                var armor = units[i].Spellbook.SpellQ;

                                if ((!v[z].HasModifier("modifier_ogre_magi_frost_armor") || !me.HasModifier("modifier_ogre_magi_frost_armor")) && armor.CanBeCasted() && units[i].Position.Distance2D(v[z]) <= 900
                                    && Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    armor.UseAbility(v[z]);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                            }
                        }
                        else if (units[i].Name == "npc_dota_neutral_forest_troll_high_priest")
                        {
                            if (units[i].Spellbook.SpellQ.CanBeCasted())
                            {
                                for (int z = 0; z < v.Count(); ++z)
                                {
                                    if (units[i].Position.Distance2D(v[z]) <= 900
                                    && Utils.SleepCheck(units[i].Handle + "high_priest"))
                                    {
                                        units[i].Spellbook.SpellQ.UseAbility(v[z]);
                                        Utils.Sleep(350, units[i].Handle + "high_priest");
                                    }
                                }
                            }
                        }


                        if (e == null) return;
                        
                        if (e.IsAlive && !e.IsInvul() && (holdKey || toggleKey))
                        {

                            //spell
                            var CheckStun = e.HasModifier("modifier_centaur_hoof_stomp");
                            var CheckSetka = e.HasModifier("modifier_dark_troll_warlord_ensnare");
                            if (units[i].Name == "npc_dota_neutral_dark_troll_warlord")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 550 && (!CheckSetka || !CheckStun || !e.IsHexed() || !e.IsStunned()) && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                         Utils.SleepCheck(units[i].Handle + "warlord"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(450, units[i].Handle + "warlord");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_big_thunder_lizard")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 250 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "lizard"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(450, units[i].Handle + "lizard");
                                }
                                if (e.Position.Distance2D(units[i].Position) < 550 && units[i].Spellbook.SpellW.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "lizard"))
                                {
                                    units[i].Spellbook.SpellW.UseAbility();
                                    Utils.Sleep(450, units[i].Handle + "lizard");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_centaur_khan")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 200 && (!CheckSetka || !CheckStun || !e.IsHexed() || !e.IsStunned()) && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "centaur"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(450, units[i].Handle + "centaur");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_satyr_hellcaller")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 850 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "satyr"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(350, units[i].Handle + "satyr");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_satyr_trickster")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 850 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "satyr_trickster"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(350, units[i].Handle + "satyr_trickster");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_satyr_soulstealer")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 850 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "satyrsoulstealer"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(350, units[i].Handle + "satyrsoulstealer");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_black_dragon")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 700 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "dragonspawn"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e.Predict(600));
                                    Utils.Sleep(350, units[i].Handle + "dragonspawn");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_big_thunder_lizard")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 200 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "lizard"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(350, units[i].Handle + "lizard");
                                }

                                for (int z = 0; z < v.Count(); z++)
                                {
                                    if (units[i].Spellbook.SpellW.CanBeCasted() && units[i].Position.Distance2D(v[z]) <= 900)
                                    {
                                        if (e.Position.Distance2D(v[z]) < v[z].AttackRange + 150 &&
                                        Utils.SleepCheck(units[i].Handle + "lizard"))
                                        {
                                            units[i].Spellbook.SpellW.UseAbility(v[z]);
                                            Utils.Sleep(350, units[i].Handle + "lizard");
                                        }
                                    }
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_mud_golem")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 850 && (!CheckSetka || !CheckStun || !e.IsHexed() || !e.IsStunned())
                                    && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "golem"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(350, units[i].Handle + "golem");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_polar_furbolg_ursa_warrior")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 240 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle + "ursa"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility();
                                    Utils.Sleep(350, units[i].Handle + "ursa");
                                }
                            }
                            else if (units[i].Name == "npc_dota_neutral_harpy_storm")
                            {
                                if (e.Position.Distance2D(units[i].Position) < 900 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle + "harpy"))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(350, units[i].Handle + "harpy");
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
                            {
                                if (e.Position.Distance2D(units[i].Position) < 1550 &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Move(e.Predict(1500));
                                    Utils.Sleep(700, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_BaseNPC_Creep)
                            {
                                if (units[i].Name == "npc_dota_necronomicon_archer")
                                {
                                    if (e.Position.Distance2D(units[i].Position) <= 700 && units[i].Spellbook.SpellQ.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))

                                    {
                                        units[i].Spellbook.SpellQ.UseAbility(e);
                                        Utils.Sleep(300, units[i].Handle.ToString());
                                    }
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_Unit_VisageFamiliar)
                            {
                                var damageModif = units[i].Modifiers.FirstOrDefault(x => x.Name == "modifier_visage_summon_familiars_damage_charge");

                                if (e.Position.Distance2D(units[i].Position) < 1550 && units[i].Health < 6 && units[i].Spellbook.Spell1.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.Spell1.UseAbility();
                                    Utils.Sleep(200, units[i].Handle.ToString());
                                }

                                if (e.Position.Distance2D(units[i].Position) < 340 && ((damageModif.StackCount < 1) && !e.IsStunned()) && units[i].Spellbook.Spell1.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.Spell1.UseAbility();
                                    Utils.Sleep(350, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth)
                            {
                                if (e.Position.Distance2D(units[i].Position) < 1300 &&
                                    units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                                if (e.Position.Distance2D(units[i].Position) < 340 &&
                                    units[i].Spellbook.SpellR.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellR.UseAbility();
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm)
                            {
                                if (e.Position.Distance2D(units[i].Position) < 700 &&
                                    units[i].Spellbook.SpellQ.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellQ.UseAbility(e.Position);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                                if (e.Position.Distance2D(units[i].Position) < 900 &&
                                    units[i].Spellbook.SpellE.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellE.UseAbility();
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                                if (e.Position.Distance2D(units[i].Position) < 850 &&
                                    units[i].Spellbook.SpellR.CanBeCasted() &&
                                    Utils.SleepCheck(units[i].Handle.ToString()))
                                {
                                    units[i].Spellbook.SpellR.UseAbility(e);
                                    Utils.Sleep(400, units[i].Handle.ToString());
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_Unit_SpiritBear)
                            {
                                if ((!me.AghanimState() && me.Position.Distance2D(units[i]) <= 1200) || me.AghanimState())
                                {
                                    abyssal = units[i].FindItem("item_abyssal_blade");

                                    mjollnir = units[i].FindItem("item_mjollnir");

                                    boots = units[i].FindItem("item_phase_boots");

                                    midas = units[i].FindItem("item_hand_of_midas");

                                    mom = units[i].FindItem("item_mask_of_madness");

                                    medall = units[i].FindItem("item_medallion_of_courage") ?? units[i].FindItem("item_solar_crest");


                                    if (boots != null && e.Position.Distance2D(units[i].Position) < 1550 && boots.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        boots.UseAbility();
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (mjollnir != null && e.Position.Distance2D(units[i].Position) < 525 && mjollnir.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        mjollnir.UseAbility(units[i]);
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (medall != null && e.Position.Distance2D(units[i].Position) < 525 && medall.CanBeCasted() &&
                                       Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        medall.UseAbility(e);
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }

                                    if (mom != null && e.Position.Distance2D(units[i].Position) < 525 && mom.CanBeCasted() &&
                                       Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        mom.UseAbility();
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (abyssal != null && e.Position.Distance2D(units[i].Position) < 500 && abyssal.CanBeCasted() &&
                                        Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        abyssal.UseAbility(e);
                                        Utils.Sleep(350, units[i].Handle.ToString());
                                    }
                                    if (midas != null)
                                    {
                                        var neutrals = ObjectManager.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
                                            creep.IsAlive && creep.IsVisible && creep.IsSpawned && creep.Distance2D(units[i])<= 700 && creep.Team != me.Team).OrderBy(x => x.Health).LastOrDefault();

                                       if (midas.CanBeCasted() && Utils.SleepCheck(neutrals.Handle.ToString()))
                                        {
                                            midas.UseAbility(neutrals);
                                            Utils.Sleep(350, neutrals.Handle.ToString());
                                        }
                                    }
                                }
                            }
                            else if (units[i].ClassID == ClassID.CDOTA_BaseNPC_Additive)
                            {
                                if (units[i].Name == "npc_dota_templar_assassin_psionic_trap")
                                {

                                    if (e.Position.Distance2D(units[i].Position) < 250
                                        && units[i].Spellbook.Spell1.CanBeCasted()
                                        && e.Distance2D(Game.MousePosition) <= 1000
                                        && Utils.SleepCheck(units[i].Handle.ToString()))
                                    {
                                        units[i].Spellbook.Spell1.UseAbility();
                                        Utils.Sleep(250, units[i].Handle.ToString());
                                    }
                                }
                            }
                            if (units[i].Distance2D(e) <= units[i].AttackRange + 100 && !e.IsAttackImmune()
                            && !units[i].IsAttacking() && units[i].CanAttack() && Utils.SleepCheck(units[i].Handle + "Attack")
                            )
                            {
                                units[i].Attack(e);
                                Utils.Sleep(250, units[i].Handle + "Attack");
                            }
                            else if (!units[i].CanAttack() && !units[i].IsAttacking()
                                && units[i].CanMove() && units[i].Distance2D(e) <= 4000 && Utils.SleepCheck(units[i].Handle + "Move")
                                )
                            {
                                units[i].Move(e.Predict(300));
                                Utils.Sleep(350, units[i].Handle + "Move");
                            }
                            else if (units[i].Distance2D(e) >= units[i].GetAttackRange() && !units[i].IsAttacking()
                               && units[i].CanMove() && units[i].Distance2D(e) <= 4000 && Utils.SleepCheck(units[i].Handle + "MoveAttack")
                               )
                            {
                                units[i].Move(e.Predict(300));
                                Utils.Sleep(350, units[i].Handle + "MoveAttack");
                            }
                        }
                        Utils.Sleep(500, "delay");
                    }
                }
            }
        }


        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            txt.Dispose();
            not.Dispose();
        }

        void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            if (MainMenu.CCMenu.Item("controll").IsActive())
            {
                txt.DrawText(null, "Creep Control On", 1200, 27, Color.Coral);
            }

            if (!MainMenu.CCMenu.Item("controll").IsActive())
            {
                txt.DrawText(null, "Creep Control Off", 1200, 27, Color.Coral);
            }
        }



        void Drawing_OnPostReset(EventArgs args)
        {
            txt.OnResetDevice();
            not.OnResetDevice();
        }

        void Drawing_OnPreReset(EventArgs args)
        {
            txt.OnLostDevice();
            not.OnLostDevice();
        }

        private void OnLoadMessage()
        {
            Game.PrintMessage("<font face='verdana' color='#ffa420'>@addon CreepControl is Loaded!</font>", MessageType.LogMessage);
            Service.Debug.Print.ConsoleMessage.Encolored("@addon CreepControl is Loaded!", ConsoleColor.Yellow);
        }
    }
}
