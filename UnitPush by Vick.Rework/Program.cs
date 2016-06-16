

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

        public static readonly Vector3[] mid =
           {
            new Vector3(-5627, -5111, 384),
            new Vector3(-3973, -3430, 264),
            new Vector3(-2355, -1870, 256),
            new Vector3(-622, -306, 128),
            new Vector3(1277, 842, 256),
            new Vector3(2867, 2490, 256),
            new Vector3(4420, 3985, 384),
            new Vector3(5213, 4717, 384)

        };
            public static readonly Vector3[] bot =
            {
            new Vector3(-2874, -6188, 256),
            new Vector3(-467, -6305, 384),
            new Vector3(1892, -6207, 384),
            new Vector3(4232, -6142, 384),
            new Vector3(5566, -5869, 384),
            new Vector3(6166, -3550, 384),
            new Vector3(6200, -1049, 384),
            new Vector3(6298, 1210, 290),
            new Vector3(6271, 3626, 384)
        };
            public static readonly Vector3[] top =
            {
            new Vector3(4511, 5732, 392),
            new Vector3(2126, 5881, 256),
            new Vector3(-221, 5911, 384),
            new Vector3(-2506, 6001, 384),
            new Vector3(-4903, 5972, 384),
            new Vector3(-6387, 4595, 384),
            new Vector3(-6361, 2223, 384),
            new Vector3(-6352, -97, 384),
            new Vector3(-6568, -2446, 264),
            new Vector3(-6615, -4323, 384),
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

                    if (Mid.Distance2D(unit[i]) <= 2700 && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        for (int z = 0; z < mid.Count(); ++z)
                        {
                            for (int j = 0; j < mid.Count(); ++j)
                            {
                                var a = mid[j];
                                var b = mid[z];
                                if (a.Distance2D(b) <= 2700 &&
                                    a.Distance2D(fount) < b.Distance2D(fount) && unit[i].Distance2D(b) <= 2700)
                                {
                                    unit[i].Attack(b);
                                }
                            }
                        }
                        Utils.Sleep(1500, unit[i].Handle.ToString());
                    }
                    Vector3 Bot = GetClosestToVector(bot, unit[i]);

                    if (Bot.Distance2D(unit[i]) <= 2700 && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        for (int z = 0; z < bot.Count(); ++z)
                        {
                            for (int j = 0; j < bot.Count(); ++j)
                            {
                                var a = bot[j];
                                var b = bot[z];
                                if (a.Distance2D(b) <= 2700 &&
                                    a.Distance2D(fount) < b.Distance2D(fount) && unit[i].Distance2D(b) <= 2700)
                                {
                                    unit[i].Attack(b);
                                }
                            }
                        }
                        Utils.Sleep(1500, unit[i].Handle.ToString());
                    }
                    Vector3 Top = GetClosestToVector(top, unit[i]);

                    if (Top.Distance2D(unit[i]) <= 2700 && Utils.SleepCheck(unit[i].Handle.ToString()))
                    {
                        for (int z = 0; z < top.Count(); ++z)
                        {
                            for (int j = 0; j < top.Count(); ++j)
                            {
                                var a = top[j];
                                var b = top[z];
                                if (a.Distance2D(b) <= 2700 &&
                                    a.Distance2D(fount) < b.Distance2D(fount) && unit[i].Distance2D(b) <= 2700)
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

