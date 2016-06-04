using System.Threading.Tasks;

namespace DotaAllCombo.Heroes
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using SharpDX;
    using System.Threading;

    using Service;
    using Service.Debug;

    internal class TinkerController : Variables, IHeroController
    {
        private const int HIDE_AWAY_RANGE = 100;
        private Ability Q, W, R, E;
        private bool Active;
        private bool CastW;
        private bool CastE;
        private Item Blink, dagon, sheep, soul, ethereal, shiva, ghost, eul, blink, force, glimmer, vail, orchid, guardian, travel;
        private readonly List<ParticleEffect> Effects = new List<ParticleEffect>();
        private const string EffectPath = @"particles\range_display_blue.vpcf";
        private const string EffectPanicPath = @"particles\range_display_red.vpcf";
        private readonly Menu skills = new Menu("Skills", "Skills");
        private readonly Menu items = new Menu("Items", "Items");

        public Vector3 GetClosestToTarget(Vector3[] coords, Hero target)
        {
            var closestVector = coords.First();
            foreach (var v in coords.Where(v => closestVector.Distance2D(target) > v.Distance2D(target)))
                closestVector = v;
            return closestVector;
        }
        private readonly Vector3[] SafePos =
                {
            new Vector3(-6752, 4325, 384),
            new Vector3(-5017, 5099, 384),
            new Vector3(-4046, 5282, 384),
            new Vector3(-2531, 5419, 384),
            new Vector3(-1561, 5498, 384),
            new Vector3(-1000, 5508, 384),
            new Vector3(-749, 6791, 384),
            new Vector3(359, 6668, 384),
            new Vector3(1624, 6780, 256),
            new Vector3(-6877, 3757, 384),
            new Vector3(-5662, 2268, 384),
            new Vector3(-6941, 1579, 384),
            new Vector3(-6819, 608, 384),
            new Vector3(-6848, 68, 384),
            new Vector3(-7005, -681, 384),
            new Vector3(-7082, -1160, 384),
            new Vector3(-2605, -2657, 256),
            new Vector3(-2207, -2394, 256),
            new Vector3(-1446, -1871, 256),
            new Vector3(-2068, -1151, 256),
            new Vector3(659, 929, 256),
            new Vector3(1065, 1241, 256),
            new Vector3(2259, 667, 256),
            new Vector3(2426, 812, 256),
            new Vector3(2647, 1009, 256),
            new Vector3(2959, 1283, 256),
            new Vector3(2110, 2431, 256),
            new Vector3(6869, 613, 384),
            new Vector3(6832, -206, 384),
            new Vector3(6773, -431, 384),
            new Vector3(6742, -1549, 384),
            new Vector3(6813, -3591, 384),
            new Vector3(6745, -4689, 384),
            new Vector3(6360, -5215, 384),
            new Vector3(4637, -5579, 384),
            new Vector3(4756, -6491, 384),
            new Vector3(4249, -6553, 384),
            new Vector3(2876, -5666, 384),
            new Vector3(3180, -6627, 384),
            new Vector3(2013, -6684, 384),
            new Vector3(-560, -6810, 384),
            new Vector3(-922, -6797, 384),
            new Vector3(-1130, -6860, 384),
            new Vector3(1000, -6928, 384),
            new Vector3(814, 981, 256),
            new Vector3(-6690, 5024, 384),
            new Vector3(-5553, 1961, 384),
        };
        private readonly Vector3[] PanicPos =
        {
            new Vector3(-752, -6961, 383),
            new Vector3(93, -6959, 383),
            new Vector3(758, -7021, 384),
            new Vector3(1816, -6797, 384),
            new Vector3(2705, -6839, 384),
            new Vector3(2506, -5658, 384),
            new Vector3(4658, -5404, 384),
            new Vector3(5558, -6553, 384),
            new Vector3(6261, -6097, 384),
            new Vector3(6744, -5342, 384),
            new Vector3(5271, -4309, 384),
            new Vector3(6994, -3484, 384),
            new Vector3(2654, -4520, 384),
            new Vector3(-492, -5489, 384),
            new Vector3(-1103, -4761, 384),
            new Vector3(-2219, -4762, 255),
            new Vector3(-2458, -4182, 256),
            new Vector3(-2280, -3277, 256),
            new Vector3(-2080, -2578, 256),
            new Vector3(-3698, -1303, 384),
            new Vector3(-2296, -1253, 256),
            new Vector3(-3238, -601, 384),
            new Vector3(-3595, 242, 640),
            new Vector3(-4003, 1318, 384),
            new Vector3(-4530, 1976, 384),
            new Vector3(-5466, 2152, 384),
            new Vector3(-5121, 2041, 640),
            new Vector3(-5417, 1188, 384),
            new Vector3(-5052, 791, 383),
            new Vector3(-4627, -530, 640),
            new Vector3(-7242, -288, 384),
            new Vector3(-6951, 358, 384),
            new Vector3(-7139, 693, 384),
            new Vector3(-7385, 1317, 384),
            new Vector3(-7352, -1285, 384),
            new Vector3(-7464, -3214, 384),
            new Vector3(-7461, -3619, 383),
            new Vector3(-7572, -3872, 384),
            new Vector3(-7407, -4303, 384),
            new Vector3(-7330, -5013, 384),
            new Vector3(-5562, -6994, 384),
            new Vector3(-4830, -7086, 384),
            new Vector3(-4015, -7129, 384),
            new Vector3(-3066, -6947, 256),
            new Vector3(3975, -5508, 384),
            new Vector3(7217, -2250, 384),
            new Vector3(6938, -973, 384),
            new Vector3(6943, -334, 384),
            new Vector3(7000, 316, 384),
            new Vector3(7198, 771, 384),
            new Vector3(7590, 1568, 256),
            new Vector3(7591, 2070, 256),
            new Vector3(7477, 2790, 384),
            new Vector3(7522, 2949, 384),
            new Vector3(7464, 5083, 384),
            new Vector3(5350, 1011, 640),
            new Vector3(4648, -652, 384),
            new Vector3(2395, -251, 384),
            new Vector3(2602, 805, 256),
            new Vector3(2822, 1096, 255),
            new Vector3(844, 1474, 255),
            new Vector3(602, 1064, 256),
            new Vector3(-932, 1953, 384),
            new Vector3(-3115, 931, 384),
            new Vector3(-4439, 3226, 384),
            new Vector3(-2662, 4506, 384),
            new Vector3(-1389, 5406, 384),
            new Vector3(-1258, 5480, 384),
            new Vector3(1012, 4587, 640),
            new Vector3(1652, 7235, 384),
            new Vector3(2020, 6933, 256),
            new Vector3(2493, 7178, 384),
            new Vector3(3864, 6726, 384),
            new Vector3(4612, 6880, 384),
            new Vector3(5654, 6752, 384),
            new Vector3(5469, 6752, 384),
            new Vector3(4505, 2020, 256),
            new Vector3(5210, -643, 384),
            new Vector3(5467, -1202, 384),
            new Vector3(6894, -4705, 384),
            new Vector3(-735, -3173, 256),
            new Vector3(-24, -3628, 384),
            new Vector3(1438, -3047, 384),
            new Vector3(3000, -3094, 384),
            new Vector3(4345, -2316, 383),
            new Vector3(3625, -1479, 384),
            new Vector3(2350, 2732, 256),
            new Vector3(-367, 3230, 384),
            new Vector3(-1816, 4064, 640),
            new Vector3(-4493, 4960, 384),
            new Vector3(-6864, 5850, 384),
            new Vector3(-6308, 6498, 384),
            new Vector3(-5156, 6642, 383),
            new Vector3(-4521, 6699, 384),
            new Vector3(-3690, 6627, 384),
            new Vector3(-2719, 6764, 384),
            new Vector3(-2286, 6778, 384),
            new Vector3(-1637, 6881, 384),
            new Vector3(-676, 6944, 384),
            new Vector3(338, 6875, 384),
            new Vector3(8, 3375, 384),
            new Vector3(1727, 3367, 384),
            new Vector3(-3565, -4903, 256),
            new Vector3(610, -4438, 384),
            new Vector3(4271, -4513, 384),
            new Vector3(3930, -6864, 384),
            new Vector3(-6972, 3864, 384),
            new Vector3(-5545, 4132, 383),
            new Vector3(-7008, 5027, 384),

        };

        public void Combo()
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;

            if (me == null)
                return;
            Active = Game.IsKeyDown(menu.Item("keyBind").GetValue<KeyBind>().Key);
            CastW = Game.IsKeyDown(menu.Item("keySpamW").GetValue<KeyBind>().Key);
            CastE = Game.IsKeyDown(menu.Item("keySpamE").GetValue<KeyBind>().Key);
            List<Unit> fount = ObjectManager.GetEntities<Unit>().Where(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain).ToList();
            var creeps = ObjectManager.GetEntities<Creep>().Where(creep =>
                   (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                   || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                   || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                   || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
                  creep.IsAlive && creep.Team != me.Team && creep.IsVisible && creep.IsSpawned).ToList();
            if (menu.Item("panicMod").IsActive())
            {
                var panic = GetClosestToTarget(PanicPos, me);
                var safe = GetClosestToTarget(SafePos, me);
                blink = me.FindItem("item_blink");
                E = me.Spellbook.SpellE;
                R = me.Spellbook.SpellR;
                travel = me.FindItem("item_travel_boots") ?? me.FindItem("item_travel_boots_2");
                soul = me.FindItem("item_soul_ring");
                var v =
                ObjectManager.GetEntities<Hero>()
                    .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion && !x.IsMagicImmune())
                    .ToList();
                if (v.Count(x => x.Distance2D(me) <= 2000) >= 1
                    && me.Health <= (me.MaximumHealth / 100 * menu.Item("Healh").GetValue<Slider>().Value))
                {
                    if (
                        blink != null
                        && me.CanCast()
                        && me.Distance2D(panic) <= 1190
                        && me.Distance2D(panic) >= 100
                        && !R.IsChanneling
                        && Utils.SleepCheck("blink")
                        && blink.CanBeCasted()
                        )
                    {
                        blink.UseAbility(panic);
                        Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                        Utils.Sleep(250, "blink");
                    }
                    else if (
                        travel != null
                        && travel.CanBeCasted()
                        && menu.Item("pushTravel").IsActive()
                        && me.Distance2D(panic) <= HIDE_AWAY_RANGE
                        && Utils.SleepCheck("travel")
                        )
                    {
                        travel.UseAbility(fount.First().Position);
                        Utils.Sleep(300, "travel");
                    }
                    else if (
                           R != null
                           && R.CanBeCasted()
                           && me.Distance2D(panic) <= 100
                           && travel != null
                           && !travel.CanBeCasted()
                           && !R.IsChanneling
                           && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                           && Utils.SleepCheck("R")
                           )
                    {
                        R.UseAbility();
                        Utils.Sleep(3000, "R");
                    }
                    
                }
            }
            if (Active && !Game.IsChatOpen)
            {
                target = me.ClosestToMouseTarget(2500);
                //Skils
                Q = me.Spellbook.SpellQ;
                W = me.Spellbook.SpellW;
                R = me.Spellbook.SpellR;
                //Items
                blink = me.FindItem("item_blink");
                dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
                sheep = me.FindItem("item_sheepstick");
                soul = me.FindItem("item_soul_ring");
                ethereal = me.FindItem("item_ethereal_blade");
                shiva = me.FindItem("item_shivas_guard");
                ghost = me.FindItem("item_ghost");
                eul = me.FindItem("item_cyclone");
                force = me.FindItem("item_force_staff");
                glimmer = me.FindItem("item_glimmer_cape");
                vail = me.FindItem("item_veil_of_discord");
                orchid = me.FindItem("item_orchid") ?? me.FindItem("item_bloodthorn");
                guardian = me.FindItem("item_guardian_greaves");
                if (target == null) return;
                if (target.IsAlive && !target.IsIllusion && !me.IsChanneling())
                {

                    if (target.IsLinkensProtected())
                    {
                        if (eul != null
                            && eul.CanBeCasted()
                            && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(eul.Name))
                        {
                            if (Utils.SleepCheck("Linkens"))
                            {
                                eul.UseAbility(target);
                                Utils.Sleep(200, "Linkens");
                            }
                        }
                        else if (force != null
                            && force.CanBeCasted()
                            && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(force.Name))
                        {
                            if (Utils.SleepCheck("Linkens"))
                            {
                                force.UseAbility(target);
                                Utils.Sleep(200, "Linkens");
                            }
                        }
                        else if (dagon != null
                            && dagon.CanBeCasted()
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                        {
                            if (Utils.SleepCheck("Linkens"))
                            {
                                dagon.UseAbility(target);
                                Utils.Sleep(200, "Linkens");
                            }
                        }
                        else if (ethereal != null
                            && ethereal.CanBeCasted()
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                        {
                            if (Utils.SleepCheck("Linkens"))
                            {
                                ethereal.UseAbility(target);
                                Utils.Sleep(200, "Linkens");
                                Utils.Sleep((me.NetworkPosition.Distance2D(target.NetworkPosition) / 650) * 1000, "Linkens");
                            }
                        }
                        else if (Q != null
                            && Q.CanBeCasted()
                            && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
                        {
                            if (Utils.SleepCheck("Linkens"))
                            {
                                Q.UseAbility(target);
                                Utils.Sleep(200, "Linkens");
                            }
                        }
                        else if (sheep != null
                            && sheep.CanBeCasted()
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                        {
                            if (Utils.SleepCheck("Linkens"))
                            {
                                sheep.UseAbility(target);
                                Utils.Sleep(200, "Linkens");
                            }
                        }
                    }
                    else
                    {
                        float angle = me.FindAngleBetween(target.Position, true);
                        Vector3 pos = new Vector3((float)(target.Position.X - 280 * Math.Cos(angle)), (float)(target.Position.Y - 280 * Math.Sin(angle)), 0);
                        uint elsecount = 0;


                        bool magicimune = (!target.IsMagicImmune() && !target.HasModifier("modifier_eul_cyclone"));
                        if (Utils.SleepCheck("combo"))
                        {

                            if (blink != null
                                && blink.CanBeCasted()
                                && !me.IsChanneling()
                                && me.Distance2D(pos) <= 1200
                                && me.Mana > Q.ManaCost
                                && me.Distance2D(target) >= 390
                                && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(blink.Name)
                                && Utils.SleepCheck("Rearm"))
                                blink.UseAbility(pos);
                            else elsecount += 1;
                            if (orchid != null
                                && orchid.CanBeCasted()
                                && !me.IsChanneling()
                                && !target.IsSilenced()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name)
                                && Utils.SleepCheck("Rearm"))
                                orchid.UseAbility(target);
                            else elsecount += 1;
                            if (vail != null
                                && vail.CanBeCasted()
                                && !me.IsChanneling()
                                && me.Distance2D(pos) <= 1000
                                && me.Mana > R.ManaCost
                                && !target.HasModifier("modifier_item_veil_of_discord_debuff")
                                && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(vail.Name)
                                && Utils.SleepCheck("Rearm"))
                                vail.UseAbility(target.Position);
                            else elsecount += 1;
                            if (glimmer != null
                                && glimmer.CanBeCasted()
                                && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)
                                && Utils.SleepCheck("Rearm"))
                                glimmer.UseAbility(me);
                            else
                                elsecount += 1;
                            if (soul != null
                                && soul.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                                && Utils.SleepCheck("Rearm"))
                                soul.UseAbility();
                            else
                                elsecount += 1;
                            if (ethereal != null
                                && ethereal.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                            {
                                ethereal.UseAbility(target);
                                if (Utils.SleepCheck("TimeEther") && me.Distance2D(target) <= ethereal.CastRange)
                                    Utils.Sleep((me.NetworkPosition.Distance2D(target.NetworkPosition) / 620) * 1000, "TimeEther");
                            }
                            else
                                elsecount += 1;
                            if (ghost != null
                                && ghost.CanBeCasted()
                                && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ghost.Name)
                                && Utils.SleepCheck("Rearm"))
                                ghost.UseAbility();

                            else
                                elsecount += 1;
                            if (sheep != null
                                && sheep.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                                sheep.UseAbility(target);
                            else
                                elsecount += 1;
                            if (Q != null
                                && Q.CanBeCasted()
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                                Q.UseAbility(target);

                            else
                                elsecount += 1;
                            if (dagon != null
                                && dagon.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon")
                                && magicimune && Utils.SleepCheck("Rearm")
                                && Utils.SleepCheck("TimeEther"))
                                dagon.UseAbility(target);
                            else
                                elsecount += 1;
                            if (W != null
                                && W.CanBeCasted()
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                            {
                                W.UseAbility();
                                if (Utils.SleepCheck("TimeW")
                                    && me.Distance2D(target) <= W.CastRange)
                                    Utils.Sleep((me.NetworkPosition.Distance2D(target.NetworkPosition) / 600) * 1000, "TimeW");
                            }
                            else
                                elsecount += 1;
                            if (shiva != null
                                && shiva.CanBeCasted()
                                && me.Distance2D(target) <= 600
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name)
                                && magicimune && Utils.SleepCheck("Rearm"))
                                shiva.UseAbility();
                            else elsecount += 1;
                            if (guardian != null
                                && guardian.CanBeCasted()
                                && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(guardian.Name)
                                && Utils.SleepCheck("Rearm"))
                                guardian.UseAbility();
                            else
                                elsecount += 1;
                            if (elsecount == 13
                                && eul != null
                                && eul.CanBeCasted()
                                && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(eul.Name)
                                && magicimune && Utils.SleepCheck("Rearm") && Utils.SleepCheck("TimeEther")
                                && Utils.SleepCheck("TimeW"))
                                eul.UseAbility(target);
                            else
                                elsecount += 1;
                            if (elsecount == 14
                                && R != null && R.CanBeCasted()
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                                && !R.IsChanneling
                                && Utils.SleepCheck("Rearm")
                                && CheckRefresh())
                            {
                                R.UseAbility();
                                Utils.Sleep(800, "Rearm");
                            }
                            else
                            {
                                if (!me.IsChanneling()
                                    && !target.IsAttackImmune()
                                    && !me.IsAttackImmune()
                                    && Utils.SleepCheck("Rearm") &&
                                    me.Distance2D(target) <= me.AttackRange
                                    && me.NetworkActivity != NetworkActivity.Attack)
                                {
                                    me.Attack(target);
                                    Game.ExecuteCommand("dota_player_units_auto_attack_mode 1");
                                }
                                else
                                {
                                    if (!me.IsChanneling()
                                        && (target.IsAttackImmune() || me.IsAttackImmune())
                                        && Utils.SleepCheck("Rearm") &&
                                        me.Distance2D(target) <= me.AttackRange - 100
                                        && me.NetworkActivity != NetworkActivity.Attack)
                                    {
                                        me.Move(target.Position);
                                    }
                                }
                            }
                            Utils.Sleep(150, "combo");
                        }
                    }
                }
            }
            if (CastW && !Game.IsChatOpen)
            {
                target = me.ClosestToMouseTarget(2500);
                //Skils
                W = me.Spellbook.SpellW;
                R = me.Spellbook.SpellR;
                //Items
                soul = me.FindItem("item_soul_ring");
                guardian = me.FindItem("item_guardian_greaves");
                if (target == null || R == null || W == null) return;
                if (target.IsAlive && !target.IsIllusion && !me.IsChanneling() && me.Distance2D(target) <= 2500)
                {
                    if (R.IsChanneling || me.HasModifier("modifier_tinker_rearm") || me.IsChanneling()) return;
                    uint elsecount = 0;
                    bool magicimune = (!target.IsMagicImmune() && !target.HasModifier("modifier_eul_cyclone"));
                    if (Utils.SleepCheck("combo"))
                    {

                        if (soul != null
                            && soul.CanBeCasted()
                            && me.Health >= (me.MaximumHealth * 0.5)
                            && Utils.SleepCheck("Rearm"))
                            soul.UseAbility();
                        else
                            elsecount += 1;
                        if (W != null
                            && W.CanBeCasted()
                            && magicimune && Utils.SleepCheck("Rearm"))
                        {
                            W.UseAbility();
                            if (Utils.SleepCheck("TimeW")
                                && me.Distance2D(target) <= W.CastRange)
                                Utils.Sleep((me.NetworkPosition.Distance2D(target.NetworkPosition) / 600) * 1000, "TimeW");
                        }
                        else
                            elsecount += 1;
                        if (guardian != null
                            && guardian.CanBeCasted()
                            && Utils.SleepCheck("Rearm"))
                            guardian.UseAbility();
                        else
                            elsecount += 1;
                        if (elsecount == 3
                            && R != null && R.CanBeCasted()
                            && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                            && !R.IsChanneling
                            && ((((soul != null && !soul.CanBeCasted())) || soul == null)
                            || (((W != null && !W.CanBeCasted())) || W == null)
                            || (((guardian != null && !guardian.CanBeCasted())) || guardian == null)
                               )
                            && Utils.SleepCheck("Rearm")
                            )
                        {
                            R.UseAbility();
                            Utils.Sleep(800, "Rearm");
                        }
                        Utils.Sleep(150, "combo");
                    }
                }
            }
            if (CastE && !Game.IsChatOpen)
            {
                //Skils
                E = me.Spellbook.SpellE;
                R = me.Spellbook.SpellR;
                //Items
                soul = me.FindItem("item_soul_ring");
                guardian = me.FindItem("item_guardian_greaves");
                if (R == null || E == null) return;
                if (R.IsChanneling || me.HasModifier("modifier_tinker_rearm") || me.IsChanneling()) return;
                if (!me.IsChanneling())
                {

                    uint elsecount = 0;
                    if (Utils.SleepCheck("combo"))
                    {

                        if (soul != null
                            && soul.CanBeCasted()
                            && me.Health >= (me.MaximumHealth * 0.5)
                            && Utils.SleepCheck("Rearm"))
                            soul.UseAbility();
                        else
                            elsecount += 1;
                        if (E != null
                            && E.CanBeCasted()
                            && Utils.SleepCheck("Rearm"))
                            E.UseAbility(Prediction.InFront(me, 290));
                        else
                            elsecount += 1;
                        if (guardian != null
                            && guardian.CanBeCasted()
                            && Utils.SleepCheck("Rearm"))
                            guardian.UseAbility();
                        else
                            elsecount += 1;
                        if (elsecount == 3
                            && R != null && R.CanBeCasted()
                            && !R.IsChanneling
                            && ((((soul != null && !soul.CanBeCasted())) || soul == null)
                                || (((E != null && !E.CanBeCasted())) || E == null)
                                || (((guardian != null && !guardian.CanBeCasted())) || guardian == null)
                                )
                            && Utils.SleepCheck("Rearm")
                            )
                        {
                            R.UseAbility();
                            Utils.Sleep(800, "Rearm");
                        }
                        Utils.Sleep(150, "combo");
                    }
                }

            }
           
            if (menu.Item("pushMod").IsActive() && !Active && !CastW && !CastE)
            {
                var panic = GetClosestToTarget(PanicPos, me);
                var safe = GetClosestToTarget(SafePos, me);
                blink = me.FindItem("item_blink");
                E = me.Spellbook.SpellE;
                R = me.Spellbook.SpellR;
                travel = me.FindItem("item_travel_boots") ?? me.FindItem("item_travel_boots_2");
                soul = me.FindItem("item_soul_ring");
                if (me.HasModifier("modifier_fountain_aura_buff"))
                {

                    if (R.IsChanneling || me.HasModifier("modifier_tinker_rearm") || me.IsChanneling()) return;

                    if (creeps.Count >= 1)
                    {
                        if (soul != null
                            && soul.CanBeCasted()
                            && !R.IsChanneling
                            && me.Health >= (me.MaximumHealth * 0.5)
                            && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                            && Utils.SleepCheck("soul"))
                        {
                            soul.UseAbility();
                            Utils.Sleep(250, "soul");
                        }
                        else if (
                                E != null && E.CanBeCasted()
                                && !R.IsChanneling
                                && creeps.Count(x => x.Distance2D(me) <= 1100) >= 2
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(E.Name)
                                && Utils.SleepCheck("E")
                                )
                        {
                            E.UseAbility(Prediction.InFront(me, 290));
                            Utils.Sleep(250, "E");
                        }
                        else if (
                                blink != null
                                && !E.CanBeCasted()
                                && me.CanCast()
                                && !R.IsChanneling
                                && blink.CanBeCasted()
                                )
                        {
                            if (me.Distance2D(safe) <= 1190
                                && me.Distance2D(safe) >= 100
                                && Utils.SleepCheck("blink"))
                            {
                                blink.UseAbility(safe);
                                Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                                Utils.Sleep(250, "blink");
                            }
                        }
                        else if (
                               blink != null
                               && !E.CanBeCasted()
                               && me.CanCast()
                               && menu.Item("panicMod").IsActive()
                               && !R.IsChanneling
                               && blink.CanBeCasted()
                               )
                        {
                            if (me.Distance2D(safe) >= 1190
                                && me.Distance2D(panic) <= 1190
                                && Utils.SleepCheck("blink"))
                            {
                                blink.UseAbility(panic);
                                Game.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                                Utils.Sleep(250, "blink");
                            }
                        }
                        else if (
                                R != null
                                && R.CanBeCasted()
                                && travel != null
                                && !travel.CanBeCasted()
                                && me.Distance2D(fount.First().Position) <= 1200
                                && !R.IsChanneling
                                && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                                && Utils.SleepCheck("R")
                                )
                        {
                            R.UseAbility();
                            Utils.Sleep(3000, "R");
                        }
                    }
                }
                if (R.IsChanneling || me.HasModifier("modifier_tinker_rearm") || me.IsChanneling()) return;


                if (me.Distance2D(safe) >= 150) return;
                if (soul != null
                        && soul.CanBeCasted()
                        && !R.IsChanneling
                        && me.Health >= (me.MaximumHealth * 0.5)
                        && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                        && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name)
                        && Utils.SleepCheck("soul"))
                {
                    soul.UseAbility();
                    Utils.Sleep(500, "soul");
                }
                else
                    if (
                           R != null
                           && R.CanBeCasted()
                           && !E.CanBeCasted()
                           && !R.IsChanneling
                           && me.Mana >= R.ManaCost + 75
                           && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                           && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(R.Name)
                           && Utils.SleepCheck("R")
                       )
                {
                    R.UseAbility();
                    Utils.Sleep(3000, "R");
                }
                else
                    if (
                           travel != null
                           && travel.CanBeCasted()
                           && !R.IsChanneling
                           && menu.Item("pushTravel").IsActive()
                           && me.Mana <= R.ManaCost + 75
                           && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                           && Utils.SleepCheck("travel")
                       )
                {
                    travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }
                else
                    if (
                           travel != null
                           && travel.CanBeCasted()
                           && creeps.Count(x => x.Distance2D(me) <= 1100) <= 1
                           && !R.IsChanneling
                           && menu.Item("pushTravel").IsActive()
                           && me.Distance2D(safe) <= HIDE_AWAY_RANGE
                           && Utils.SleepCheck("travel")
                       )
                {
                    travel.UseAbility(fount.First().Position);
                    Utils.Sleep(300, "travel");
                }
            }
            

        }
        bool CheckRefresh()
        {
            if ((ghost != null && ghost.CanBeCasted() && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(ghost.Name))
                || (soul != null && soul.CanBeCasted() && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(soul.Name))
                || (sheep != null && sheep.CanBeCasted() && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                || (Q != null && Q.CanBeCasted() && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(Q.Name))
                || (ethereal != null && ethereal.CanBeCasted() && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                || (dagon != null && dagon.CanBeCasted() && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                || (W != null && W.CanBeCasted() && menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(W.Name))
                || (guardian != null && guardian.CanBeCasted() && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(guardian.Name))
                || (shiva != null && shiva.CanBeCasted() && menu.Item("Items").GetValue<AbilityToggler>().IsEnabled(shiva.Name))
                || (eul != null && eul.CanBeCasted() && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(eul.Name))
                || (glimmer != null && glimmer.CanBeCasted() && menu.Item("Item").GetValue<AbilityToggler>().IsEnabled(glimmer.Name)))
                return false;
            return true;
        }
        public void OnLoadEvent()
        {
            AssemblyExtensions.InitAssembly("VickTheRock", "0.1");

            Print.LogMessage.Success(" I have several theories I'd like to put into practice.");
            menu.AddItem(new MenuItem("keyBind", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            menu.AddItem(new MenuItem("keySpamW", "Use Missile and Mana items(DefMode)").SetValue(new KeyBind('F', KeyBindType.Press)));
            menu.AddItem(new MenuItem("keySpamE", "Use March and Mana items(DefMode)").SetValue(new KeyBind('G', KeyBindType.Press)));
            menu.AddItem(new MenuItem("pushMod", "AutoPushHalper").SetValue(true));
            menu.AddItem(new MenuItem("panicMod", "Auto Blink and Travel base position if Healt <=| and have enemy").SetValue(true));
            menu.AddItem(new MenuItem("Healh", "Min healh % to ult").SetValue(new Slider(35, 0, 100))); // x/ 10%
            menu.AddItem(new MenuItem("drawPart", "Draw the position for use Blink").SetValue(true));
            menu.AddItem(new MenuItem("pushTravel", "Use Boots Of Travel").SetValue(true));
            skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"tinker_march_of_the_machines",true},
                {"tinker_laser",true},
                {"tinker_heat_seeking_missile",true},
                {"tinker_rearm",true}
            })));
            items.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_dagon",true},
                {"item_sheepstick",true},
                {"item_soul_ring",true},
                {"item_orchid",true},
                {"item_guardian_greaves",true},
                {"item_ethereal_blade",true},
                {"item_shivas_guard",true}
            })));
            items.AddItem(new MenuItem("Item", "Items: ").SetValue(new AbilityToggler(new Dictionary<string, bool>
            {
                {"item_ghost",true},
                {"item_cyclone",true},
                {"item_force_staff",true},
                {"item_glimmer_cape",true},
                { "item_veil_of_discord",true},
                { "item_blink",true},
            })));
            menu.AddSubMenu(skills);
            menu.AddSubMenu(items);
            Drawing.OnDraw += ParticleDraw;
        }

        public void OnCloseEvent()
        {
            Drawing.OnDraw -= ParticleDraw;
        }

        bool iscreated;
        void ParticleDraw(EventArgs args)
        {
            //
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;

            if (me == null) return;
            if (menu.Item("drawPart").IsActive())
            {
                for (int i = 0; i < SafePos.Count(); ++i)
                {
                    if (!iscreated)
                    {
                        ParticleEffect effect = new ParticleEffect(EffectPath, SafePos[i]);
                        effect.SetControlPoint(1, new Vector3(HIDE_AWAY_RANGE, 0, 0));
                        Effects.Add(effect);
                    }
                }
                if (menu.Item("panicMod").IsActive())
                {
                    for (int i = 0; i < PanicPos.Count(); ++i)
                    {
                        if (!iscreated)
                        {
                            ParticleEffect effect = new ParticleEffect(EffectPanicPath, PanicPos[i]);
                            effect.SetControlPoint(1, new Vector3(HIDE_AWAY_RANGE, 0, 0));
                            Effects.Add(effect);
                        }
                    }
                }
                iscreated = true;
            }
        }
    }
}
