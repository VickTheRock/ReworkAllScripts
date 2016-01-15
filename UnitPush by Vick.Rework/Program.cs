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

namespace AllUnitPush
{
	internal class Program
	{

		private static bool activated;
		private static Vector3[] mid;
		private static Vector3[] top;
		private static Vector3[] bot;
		private static Font txt;
		private static Font not;
		private static Key KeyControl = Key.K;

		private static readonly Vector3[] Mid =
		{
			new Vector3(-5589, -5098, 261),
			new Vector3(-4027, -3532, 137),
			new Vector3(-2470, -1960, 127),
			new Vector3(-891, -425, 55),
			new Vector3(1002, 703, 127),
			new Vector3(2627, 2193, 127),
			new Vector3(4382, 3601, 2562)

		};
		private static readonly Vector3[] Bot =
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
		private static readonly Vector3[] Top =
		{	
			new Vector3(-6400, -793, 256),
			new Vector3(-6356, 1141, 256),
			new Vector3(-6320, 3762, 256),
			new Vector3(-5300, 5924, 256),
			new Vector3(-3104, 5929, 256),
			new Vector3(-826, 5942, 256),
			new Vector3(1445, 5807, 256),
			new Vector3(3473, 5949, 256),
            new Vector3(-6506, -4701, 384),
		};









		static void Main(string[] args)
		{
			Game.OnUpdate += Game_OnUpdate;
			Game.OnWndProc += Game_OnWndProc;
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

			var me = ObjectMgr.LocalHero;
			if (!Game.IsInGame || me == null)
				return;

            var unit = ObjectMgr.GetEntities<Unit>().Where(x => x.Team == me.Team && x.IsControllable && x.NetworkActivity != NetworkActivity.Move && x.NetworkActivity != NetworkActivity.Attack && x.IsAlive && !x.Equals(me) && x.ClassID != ClassID.CDOTA_Unit_Hero_Meepo).ToList();



			if (activated)
			{


				mid = Mid;
				bot = Bot;
				top = Top;

				if (me.Team == Team.Radiant)
				{


					foreach (Unit v in unit)
					{
						if (v.Distance2D(me) > 700)
						{
							/******************************************MID******************************************/
							if (v.Distance2D(mid[0]) <= 2300 && v.Distance2D(mid[1]) >= 2190 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[1]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[1]) <= 2400 && v.Distance2D(mid[2]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[2]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[2]) <= 2640 && v.Distance2D(mid[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[3]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[3]) <= 2340 && v.Distance2D(mid[4]) >= 1900 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[4]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[4]) <= 2400 && v.Distance2D(mid[5]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[5]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[5]) <= 2400 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[6]);
								Utils.Sleep(3700, v.Handle.ToString());
							}

							/******************************************MID******************************************/
							/******************************************BOT******************************************/


							if (v.Distance2D(bot[0]) <= 2400 && v.Distance2D(bot[1]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[1]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[1]) <= 2440 && v.Distance2D(bot[2]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[2]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[2]) <= 2640 && v.Distance2D(bot[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[3]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[3]) <= 2340 && v.Distance2D(bot[4]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[4]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[4]) <= 2380 && v.Distance2D(bot[5]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[5]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[5]) <= 2380 && v.Distance2D(bot[6]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[6]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[6]) <= 2630 && v.Distance2D(bot[7]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[7]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[7]) <= 2630 && v.Distance2D(bot[8]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[8]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[8]) <= 2630 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[9]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							/******************************************BOT******************************************/
							/******************************************TOP******************************************/
							if (v.Distance2D(top[0]) <= 1900 && v.Distance2D(top[1]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[1]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[1]) <= 2640 && v.Distance2D(top[2]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[2]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[2]) <= 2640 && v.Distance2D(top[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[3]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[3]) <= 2340 && v.Distance2D(top[4]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[4]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[4]) <= 2380 && v.Distance2D(top[5]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[5]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[5]) <= 2380 && v.Distance2D(top[6]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[6]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[6]) <= 2630 && v.Distance2D(top[7]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[7]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[7]) <= 2630 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[8]);
								Utils.Sleep(3700, v.Handle.ToString());
							}


							/******************************************TOP******************************************/

						}
					}
				}
				if (me.Team == Team.Dire)
				{
					foreach (Unit v in unit)
					{
						if (v.Distance2D(me) > 700)
						{
							/******************************************MID******************************************/
							if (v.Distance2D(mid[6]) <= 2300 && v.Distance2D(mid[5]) >= 2190 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[5]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[5]) <= 2400 && v.Distance2D(mid[4]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[4]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[4]) <= 2640 && v.Distance2D(mid[3]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[3]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[3]) <= 2340 && v.Distance2D(mid[2]) >= 1900 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[2]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[2]) <= 2400 && v.Distance2D(mid[1]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[1]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(mid[1]) <= 2400 && v.Distance2D(mid[2]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(mid[0]);
								Utils.Sleep(3700, v.Handle.ToString());
							}

							/******************************************MID******************************************/
							/******************************************BOT******************************************/


							if (v.Distance2D(bot[9]) <= 2400 && v.Distance2D(bot[8]) >= 2490 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[8]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[8]) <= 2440 && v.Distance2D(bot[7]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[7]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[7]) <= 2640 && v.Distance2D(bot[6]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[6]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[6]) <= 2340 && v.Distance2D(bot[5]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[5]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[5]) <= 2380 && v.Distance2D(bot[4]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[4]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[4]) <= 500 && v.Distance2D(bot[3]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[3]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[3]) <= 2630 && v.Distance2D(bot[2]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[2]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[2]) <= 2630 && v.Distance2D(bot[1]) >= 2000 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[1]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(bot[1]) <= 2630 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(bot[0]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							/******************************************BOT******************************************/
							/******************************************TOP******************************************/
							if (v.Distance2D(top[7]) <= 1900 && v.Distance2D(top[6]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[6]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[6]) <= 2640 && v.Distance2D(top[5]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[5]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[5]) <= 2640 && v.Distance2D(top[4]) >= 2420 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[4]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[4]) <= 2340 && v.Distance2D(top[3]) >= 2280 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[3]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[3]) <= 2380 && v.Distance2D(top[2]) >= 2380 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[2]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[2]) <= 2600 && v.Distance2D(top[1]) >= 2300 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[1]);
								Utils.Sleep(3700, v.Handle.ToString());
							}
							if (v.Distance2D(top[1]) <= 3500 && Utils.SleepCheck(v.Handle.ToString()))
							{
								v.Attack(top[8]);
								Utils.Sleep(3700, v.Handle.ToString());
							}


							/******************************************TOP******************************************/

						}
					}
				}
			}
		}

		private static float GetDistance2D(Vector3 p1, Vector3 p2)
		{
			return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
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
			var me = ObjectMgr.LocalHero;
			if (player == null || player.Team == Team.Observer)
				return;

			if (activated)
			{
				txt.DrawText(null, "Unit Push Active [K]", 1200, 37, Color.Aqua);
			}

			if (!activated)
			{
				txt.DrawText(null, "Unit Push UnActive [K]", 1200, 37, Color.DarkOrchid);
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

