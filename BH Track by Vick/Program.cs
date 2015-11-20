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
        private static Font txt;
        private static Font not;
        private static Key keyTrack = Key.Home;


        static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> Track by Vick# loaded!");

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
			if (!Game.IsInGame || me.ClassID != ClassID.CDOTA_Unit_Hero_BountyHunter || me == null)
			{
				return;
			}


			var modifINV = me.Modifiers.Any(y => y.Name == "modifier_bounty_hunter_wind_walk");

			var enemies = ObjectMgr.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.Team != me.Team).ToList();
			var track = me.Spellbook.SpellR;

			if (activated && me.IsAlive && track != null)
				if (me.Modifiers.All(y => y.Name != "modifier_bounty_hunter_wind_walk"))
					foreach (var u in enemies)
					{
						if (
							(( u.ClassID == ClassID.CDOTA_Unit_Hero_Riki     || u.ClassID == ClassID.CDOTA_Unit_Hero_Broodmother
							|| u.ClassID == ClassID.CDOTA_Unit_Hero_Clinkz   || u.ClassID == ClassID.CDOTA_Unit_Hero_Invoker
							|| u.ClassID == ClassID.CDOTA_Unit_Hero_SandKing || u.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin
							|| u.ClassID == ClassID.CDOTA_Unit_Hero_Treant   || u.ClassID == ClassID.CDOTA_Unit_Hero_PhantomLancer
							)
							|| u.Health <= (u.MaximumHealth * 0.5)) && !u.Modifiers.Any(y => y.Name == "modifier_bounty_hunter_track")
							&& track.CanBeCasted() && me.Distance2D(u) <= 1200 && Utils.SleepCheck("R"))
						{
							track.UseAbility(u);
							Utils.Sleep(300, "R");
						}
					}
		}



		private static void Game_OnWndProc(WndEventArgs args)
        {
			var me = ObjectMgr.LocalHero;
			var player = ObjectMgr.LocalPlayer;
			if (player == null || player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_BountyHunter)
				return;
			if (Game.IsKeyDown(keyTrack) && !Game.IsChatOpen && Utils.SleepCheck("toggle"))
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
			var me = ObjectMgr.LocalHero;
			var player = ObjectMgr.LocalPlayer;
			if (
				Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame || player == null 
				|| player.Team == Team.Observer || me.ClassID != ClassID.CDOTA_Unit_Hero_BountyHunter
				)
                return;

			if (activated)
            {
                txt.DrawText(null, "Track On Home", 1200, 17, Color.Aqua);
            }

            if (!activated)
            {
                txt.DrawText(null, "Track Off Home", 1200, 17, Color.Aqua);
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
