using System;
using System.Linq;

using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using System.Windows.Input;

namespace ControlCreep_By_Vick
{
    internal class Program
    {

        private static bool activated;
        private static Item midas;
        private static Font txt;
        private static Font not;
        private static Key KeyControl = Key.Z;


        static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> ControlCreep By Vick# loaded!");

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
        }




        public static void Game_OnUpdate(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;

            var target = me.ClosestToMouseTarget(1200);

            if (target == null)
            {
                return;
            }
            if (activated && target.IsAlive && !target.IsInvul() && Game.MousePosition.Distance2D(target) <= 1000 || target.Distance2D(me) <= 600)
            {

                //spell
                var CheckStun = target.Modifiers.Any(y => y.Name == "modifier_centaur_hoof_stomp");
                var CheckSetka = target.Modifiers.Any(y => y.Name == "modifier_dark_troll_warlord_ensnare");
                var Neutrals = ObjectMgr.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep &&
                 creep.IsAlive && creep.IsVisible && creep.IsSpawned) && creep.Team == me.GetEnemyTeam()).ToList();
                var Neutral = ObjectMgr.GetEntities<Creep>().Where(creep => ( creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral &&
                 creep.IsAlive ) && creep.Team == me.GetEnemyTeam()).ToList();

                var troll = ObjectMgr.GetEntities<Creep>().Where(unit => unit.Name == "npc_dota_neutral_dark_troll_warlord").ToList();
                if (troll == null)
                {
                    return;
                }
                    foreach (var v in troll)
                    {


                        if (target.Position.Distance2D(v.Position) < 550 && (!CheckSetka || !CheckStun || target.IsHexed() || target.IsStunned()) && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                


                var lizard = ObjectMgr.GetEntities<Creep>().Where(unit => unit.Name == "npc_dota_neutral_big_thunder_lizard").ToList();
                if (lizard == null)
                {
                    return;
                }
                
                    foreach (var v in lizard)
                    {


                        if (target.Position.Distance2D(v.Position) < 250 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility();
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 550 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellW.UseAbility();
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 1550 &&
                          Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                


                var centaur = ObjectMgr.GetEntities<Creep>().Where(unit => unit.Name == "npc_dota_neutral_centaur_khan").ToList();
                if (centaur == null)
                {
                    return;
                }
                    foreach (var v in centaur)
                    {

                        if (target.Position.Distance2D(v.Position) < 200 && (!CheckSetka || !CheckStun || target.IsHexed() || target.IsStunned()) && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility();
                            Utils.Sleep(300, v.Handle.ToString());
                        }

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var satyr = ObjectMgr.GetEntities<Creep>().Where(unit => unit.Name == "npc_dota_neutral_satyr_hellcaller").ToList();
                if (satyr == null)
                {
                    return;
                }
               
                    foreach (var v in satyr)
                    {

                        if (target.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility(target.Position);
                            Utils.Sleep(300, v.Handle.ToString());
                        }

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var ursa = ObjectMgr.GetEntities<Creep>().Where(unit => unit.Name == "npc_dota_neutral_polar_furbolg_ursa_warrior").ToList();
                if (ursa == null)
                {
                    return;
                }
                    foreach (var v in ursa)
                    {

                        if (target.Position.Distance2D(v.Position) < 240 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility();
                            Utils.Sleep(300, v.Handle.ToString());
                        }

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var Sigl = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Tusk_Sigil)
                        && x.IsAlive && x.IsControllable);
                if (Sigl == null)
                {
                    return;
                }
                    foreach (var v in Sigl)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Follow(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var InvForgeds = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit)
                        && x.IsAlive && x.IsControllable);
                if (InvForgeds == null)
                {
                    return;
                }
               
                    foreach (var v in InvForgeds)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var WarlockGolem = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem)
                        && x.IsAlive && x.IsControllable);
                if (WarlockGolem == null)
                {
                    return;
                }
                    foreach (var v in WarlockGolem)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                
                var Necronomicons = ObjectMgr.GetEntities<Creep>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep)
                        && x.IsAlive && x.IsControllable);
                if (Necronomicons == null)
                {
                    return;
                }
                    foreach (var v in Necronomicons)
                    {


                        var archer = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_necronomicon_archer").ToList();
                        if (archer != null && target.Position.Distance2D(v.Position) <= 650 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))

                        {
                            v.Spellbook.SpellQ.UseAbility(target);
                            Utils.Sleep(300, v.Handle.ToString());
                        }

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                           Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var spiritbear = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_SpiritBear)
                       && x.IsAlive && x.IsControllable);
                if (spiritbear == null)
                {
                    return;
                }
                 var aghanim = me.FindItem("item_phase_boots");
                foreach (var v in spiritbear)
                    if ((!me.AghanimState() && me.Position.Distance2D(v) <= 1200) || me.AghanimState())
                    {

                        
                        var abyssal = v.FindItem("item_abyssal_blade");
                        var mjollnir = v.FindItem("item_mjollnir");
                        var boots = v.FindItem("item_phase_boots");
                        midas = v.FindItem("item_hand_of_midas");


                        if (target.Position.Distance2D(v.Position) < 1550 && boots.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.FindItem("item_phase_boots").UseAbility();
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 525 && mjollnir.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.FindItem("item_mjollnir").UseAbility(v);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 170 && abyssal.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.FindItem("item_abyssal_blade").UseAbility(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 525 && mjollnir.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.FindItem("item_mjollnir").UseAbility(v);
                            Utils.Sleep(700, v.Handle.ToString());
                        }

                        foreach (var f in Neutrals)
                        {
                            if (target.Position.Distance2D(f.Position) < 650 && midas.CanBeCasted() &&
                                Utils.SleepCheck(f.Handle.ToString()))
                            {
                                v.FindItem("item_hand_of_midas").UseAbility(f);
                                Utils.Sleep(700, f.Handle.ToString());
                            }
                        }
                        if (target.Position.Distance2D(v.Position) < 1550 &&
                        Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var Familliar = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_VisageFamiliar)
                      && x.IsAlive && x.IsControllable);
                if (Familliar == null)
                {
                    return;
                }
                    foreach (var v in Familliar)
                    {
                        var damageModif = v.Modifiers.FirstOrDefault(x => x.Name == "modifier_visage_summon_familiars_damage_charge");


                        if (target.Position.Distance2D(v.Position) < 1550 && v.Health < 5 && v.Spellbook.Spell1.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.Spell1.UseAbility();
                            Utils.Sleep(700, v.Handle.ToString());
                        }

                        if (target.Position.Distance2D(v.Position) < 340 && ((damageModif.StackCount < 1)  && !target.IsStunned()) && v.Spellbook.Spell1.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.Spell1.UseAbility();
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 1550 &&
                           Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                
                var primalearth = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalEarth)
                    && x.IsAlive && x.IsControllable);
                if (primalearth == null)
                {
                    return;
                }
                    foreach (var v in primalearth)
                    {

                        if (target.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility(target);
                            Utils.Sleep(400, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 340 && v.Spellbook.SpellR.CanBeCasted() &&
                           Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellR.UseAbility();
                            Utils.Sleep(400, v.Handle.ToString());
                        }

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                
                var primalstorm = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalStorm)
                       && x.IsAlive && x.IsControllable);
                if (primalstorm == null)
                {
                    return;
                }

                    foreach (var v in primalstorm)
                    {


                        if (target.Position.Distance2D(v.Position) < 500 && v.Spellbook.SpellQ.CanBeCasted() &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellQ.UseAbility(target.Position);
                            Utils.Sleep(400, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 850 && v.Spellbook.SpellR.CanBeCasted() &&
                           Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Spellbook.SpellR.UseAbility(target);
                            Utils.Sleep(400, v.Handle.ToString());
                        }
                        if (target.Position.Distance2D(v.Position) < 1550 &&
                           Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var primalfire = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_Unit_Brewmaster_PrimalFire)
                       && x.IsAlive && x.IsControllable);
                if (primalfire == null)
                {
                    return;
                }
                    foreach (var v in primalfire)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                






                var boar = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_beastmaster_boar_1").ToList();
                if (boar == null)
                {
                    return;
                }
                    foreach (var v in boar)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                

                var eidolon = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_eidolon").ToList();
                if (eidolon == null)
                {
                    return;
                }
                    foreach (var v in eidolon)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                


                var Ward = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_NPC_WitchDoctor_Ward)
                       && x.IsAlive && x.IsControllable);
                if (Ward == null)
                {
                    return;
                }

                    foreach (var v in Ward)
                    {

                        if (target.Position.Distance2D(v.Position) < 900 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                


                var Wolf = ObjectMgr.GetEntities<Creep>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral)
                     && x.IsAlive && x.IsControllable);
                if (Wolf == null)
                {
                    return;
                }
                    foreach (var v in Wolf)
                    {

                        if (target.Position.Distance2D(v.Position) < 1550 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                




                var SerpentWard = ObjectMgr.GetEntities<Unit>().Where(x => (x.ClassID == ClassID.CDOTA_BaseNPC_ShadowShaman_SerpentWard)
                    && x.IsAlive && x.IsControllable);
                if (SerpentWard == null)
                {
                    return;
                }

                    foreach (var v in SerpentWard)
                    {

                        if (target.Position.Distance2D(v.Position) < 650 &&
                            Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(target);
                            Utils.Sleep(700, v.Handle.ToString());
                        }
                    }
                
            }
        }



        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (Game.IsKeyDown(KeyControl) && !Game.IsChatOpen && Utils.SleepCheck("toggle"))
            {
                activated = !activated;
                Utils.Sleep(250, "toggle");
            }
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

            var player = ObjectMgr.LocalPlayer;


            if (activated)
            {
                txt.DrawText(null, "Creep Control On BackSpace", 1200, 27, Color.Aqua);
            }

            if (!activated)
            {
                txt.DrawText(null, "Creep Control Off BackSpace", 1200, 27, Color.Aqua);
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



