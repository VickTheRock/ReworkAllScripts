

namespace UnitAllPush
{
    using System.Security.Permissions;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Ensage;
    using SharpDX;
    using Ensage.Common.Extensions;
    using Ensage.Heroes;
    using Ensage.Common;
    using SharpDX.Direct3D9;
    using System.Windows.Input;
    using Ensage.Common.Menu;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal class Program
    {

        private static bool activated;
        private static Font txt;
        private static Font not;

        private static readonly Vector3[] mid =
        {
            new Vector3(-5589, -5098, 261),
            new Vector3(-4027, -3532, 137),
            new Vector3(-2470, -1960, 127),
            new Vector3(-891, -425, 55),
            new Vector3(1002, 703, 127),
            new Vector3(2627, 2193, 127),
            new Vector3(4382, 3601, 2562)
        };
        private static readonly Vector3[] bot =
        {
            new Vector3 (-4077, -6160, 268),
            new Vector3 (-1875, -6125, 127),
            new Vector3 (325, -6217, 256),
            new Vector3 (2532, -6215, 256),
            new Vector3(5197, -5968, 384),
            new Vector3 (6090, -4318, 256),
            new Vector3 (6180, -2117, 256),
            new Vector3 (6242, 84, 256),
            new Vector3(6307, 2286, 141),
            new Vector3(6254, 3680, 256)
        };
        private static readonly Vector3[] top =
        {
            new Vector3(-6400, -793, 256),
            new Vector3(-6356, 1141, 256),
            new Vector3(-6320, 3762, 256),
            new Vector3(-5300, 5924, 256),
            new Vector3(-3104, 5929, 256),
            new Vector3(-826, 5942, 256),
            new Vector3(1445, 5807, 256),
            new Vector3(3473, 5949, 256),
            new Vector3(-6506, -4701, 384)
        };

        private static readonly Menu Menu = new Menu("All Unit's Push", "AllUnit's Push", true);
        static void Main(string[] args)
        {


            Menu.AddItem(new MenuItem("Push key", "Togle key Push").SetValue(new KeyBind('K', KeyBindType.Toggle)));
            Menu.AddToMainMenu();


            Game.OnUpdate += Game_OnUpdate;
            Console.WriteLine("> Unit by Vick.Rework!");

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

            var me = ObjectManager.LocalHero;
            if (!Game.IsInGame || me == null || Game.IsWatchingGame)
                return;
            activated = Menu.Item("Push key").GetValue<KeyBind>().Active;

            if (activated && !Game.IsPaused)
            {
                var unit = ObjectManager.GetEntities<Unit>().Where(creep =>
                    (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                     || creep.ClassID == ClassID.CDOTA_BaseNPC_Additive
                     || creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit
                     || creep.ClassID == ClassID.CDOTA_BaseNPC_Warlock_Golem
                     || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep
                     || creep.ClassID == ClassID.CDOTA_Unit_Hero_Beastmaster_Boar
                     || creep.ClassID == ClassID.CDOTA_Unit_SpiritBear
                     || creep.ClassID == ClassID.CDOTA_Unit_Broodmother_Spiderling
                     )
                    && creep.IsAlive
                    && creep.Team == me.Team
                    && creep.IsControllable).ToList();
                if (unit.Count == 0) return;

                Unit fount = ObjectManager.GetEntities<Unit>().FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
                List<Unit> tower = ObjectManager.GetEntities<Unit>().Where(x => x.Team != me.Team && x.ClassID == ClassID.CDOTA_BaseNPC_Tower).ToList();
                var creepsE = ObjectManager.GetEntities<Unit>().Where(creep =>
                       (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                       || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                       || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                       || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
                      creep.IsAlive && creep.Team != me.Team && creep.IsVisible && creep.IsSpawned).ToList();
                var creepsA = ObjectManager.GetEntities<Unit>().Where(creep =>
                       (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane
                       || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege
                       || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral
                       || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
                      creep.IsAlive && creep.Team == me.Team && creep.IsVisible && creep.IsSpawned).ToList();

                for (int i = 0; i < unit.Count(); ++i)
                {
                    if (me.Distance2D(unit[i]) <= 600) return;
                    var v =
                            ObjectManager.GetEntities<Hero>()
                                .Where(x => x.Team != me.Team && x.IsAlive && x.IsVisible && !x.IsIllusion)
                                .ToList();
                    if (v.Count(x => x.Distance2D(unit[i]) <= 900) >= 1
                    && unit[i].CanMove()
                    && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        unit[i].Move(fount.Position);
                        Utils.Sleep(1500, unit[i].Handle.ToString());
                    }
                    if (unit[i].NetworkActivity == NetworkActivity.Move) return;
                    Vector3 Mid = GetClosestToVector(mid, unit[i]);

                    if (Mid.Distance2D(unit[i]) <= 2500 && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        for (int z = 0; z < mid.Count(); ++z)
                        {
                            for (int j = 0; j < mid.Count(); ++j)
                            {
                                var a = mid[j];
                                var b = mid[z];
                                if (a.Distance2D(b) <= 2500 &&
                                    a.Distance2D(fount) < b.Distance2D(fount) && unit[i].Distance2D(b) <= 2500)
                                {
                                    unit[i].Attack(b);
                                }
                            }
                        }
                        Utils.Sleep(1500, unit[i].Handle.ToString());
                    }
                    Vector3 Bot = GetClosestToVector(bot, unit[i]);

                    if (Bot.Distance2D(unit[i]) <= 2500 && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        for (int z = 0; z < bot.Count(); ++z)
                        {
                            for (int j = 0; j < bot.Count(); ++j)
                            {
                                var a = bot[j];
                                var b = bot[z];
                                if (a.Distance2D(b) <= 2500 &&
                                    a.Distance2D(fount) < b.Distance2D(fount) && unit[i].Distance2D(b) <= 2500)
                                {
                                    unit[i].Attack(b);
                                }
                            }
                        }
                        Utils.Sleep(1500, unit[i].Handle.ToString());
                    }
                    Vector3 Top = GetClosestToVector(top, unit[i]);

                    if (Top.Distance2D(unit[i]) <= 2500 && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        for (int z = 0; z < top.Count(); ++z)
                        {
                            for (int j = 0; j < top.Count(); ++j)
                            {
                                var a = top[j];
                                var b = top[z];
                                if (a.Distance2D(b) <= 2500 &&
                                    a.Distance2D(fount) < b.Distance2D(fount) && unit[i].Distance2D(b) <= 2500)
                                {
                                    unit[i].Attack(b);
                                }
                            }
                        }
                        Utils.Sleep(1500, unit[i].Handle.ToString());
                    }
                }
            }
        }


        private static Vector3 GetClosestToVector(Vector3[] coords, Unit z)
        {
            var closestVector = coords.First();
            foreach (var v in coords.Where(v => closestVector.Distance2D(z) > v.Distance2D(z)))
                closestVector = v;
            return closestVector;
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

            var me = ObjectManager.LocalHero;
            if (me == null)
                return;

            if (activated)
            {
                txt.DrawText(null, "Unit Push Active", 1200, 37, Color.Coral);
            }

            if (!activated)
            {
                txt.DrawText(null, "Unit Push UnActive", 1200, 37, Color.DarkRed);
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

