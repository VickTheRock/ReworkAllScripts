using System.Security.Permissions;

namespace DotaAllCombo.Addons
{
	using System.Reflection;
	using Ensage;
	using Ensage.Common;
	using Ensage.Common.Extensions;
	using Ensage.Common.Menu;
	using SharpDX;
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using SharpDX.Direct3D9;
	using System.Windows.Input;
	using Service;
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public class OthersAddons : IAddon
	{
		private static Hero me;
		private static bool _load;
		private static int _seconds;
		private static Font _text;
		public static Line _line = new Line(Drawing.Direct3DDevice9);

		public void Load()
		{
			_text = new Font(
			   Drawing.Direct3DDevice9,
			   new FontDescription
			   {
				   FaceName = "Monospace",
				   Height = 21,
				   OutputPrecision = FontPrecision.Default,
				   Quality = FontQuality.ClearType
			   });
			_load = false;
			Drawing.OnDraw += Drawing_OnDraw;
			Drawing.OnPreReset += Drawing_OnPreReset;
			Drawing.OnPostReset += Drawing_OnPostReset;
			
			OnLoadMessage();
			me = ObjectManager.LocalHero;
		}

		public void Unload()
		{
			
			Drawing.OnPreReset -= Drawing_OnPreReset;
			Drawing.OnPostReset -= Drawing_OnPostReset;
			Drawing.OnDraw -= Drawing_OnDraw;
			_load = false;
		}
		private float _lastRange, AttackRange;
		private ParticleEffect rangeDisplay;
		private static double _aPoint;
        public void RunScript()
		{
			
		}
		void Drawing_OnDraw(EventArgs args)
		{
			if (!Game.IsInGame || Game.IsWatchingGame)
				return;

			if (me == null)
				return;
			if (!MainMenu.OthersMenu.Item("others").IsActive() || !Game.IsInGame || me == null || Game.IsPaused ||
				Game.IsChatOpen) return;

				
            
            if (MainMenu.OthersMenu.Item("ShowAttakRange").GetValue<bool>())
			{
                Item item = me.Inventory.Items.FirstOrDefault(x => x != null && x.IsValid && (x.Name.Contains("item_dragon_lance") || x.Name.Contains("item_hurricane_pike")));
                
                

                if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord && me.HasModifier("modifier_troll_warlord_berserkers_rage"))
			        AttackRange = 150 + me.HullRadius+24;
                   else if (me.ClassID == ClassID.CDOTA_Unit_Hero_TrollWarlord && !me.HasModifier("modifier_troll_warlord_berserkers_rage"))
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_TemplarAssassin)
                    AttackRange = me.GetAttackRange() + me.HullRadius;
                else 
                if (me.ClassID == ClassID.CDOTA_Unit_Hero_DragonKnight && me.HasModifier("modifier_dragon_knight_dragon_form"))
                    AttackRange = me.GetAttackRange() + me.HullRadius+24;
                else
                if(item == null && me.IsRanged)
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
               if (item !=null && me.IsRanged)
                    AttackRange = me.GetAttackRange() + me.HullRadius + 24;
                else
                    AttackRange = me.GetAttackRange() + me.HullRadius;
                if (rangeDisplay == null)
					{
						if (me.IsAlive)
						{
							rangeDisplay = me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
							rangeDisplay.SetControlPoint(1, new Vector3(255, 0, 222));
							rangeDisplay.SetControlPoint(3, new Vector3(5, 0, 0));
							rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
						}
					}
					else
					{
						if (!me.IsAlive)
						{
							rangeDisplay.Dispose();
							rangeDisplay = null;
						}
						else if (_lastRange != AttackRange)
						{
							rangeDisplay.Dispose();
							_lastRange = AttackRange;
							rangeDisplay = me.AddParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf");
							rangeDisplay.SetControlPoint(1, new Vector3(255, 0, 222));
							rangeDisplay.SetControlPoint(3, new Vector3(5, 0, 0));
							rangeDisplay.SetControlPoint(2, new Vector3(_lastRange, 255, 0));
						}
					}
				}
				else
				{
					if (rangeDisplay != null) rangeDisplay.Dispose();
					rangeDisplay = null;
				}

            if (MainMenu.OthersMenu.Item("Auto Un Aggro").GetValue<bool>())
            {
                Toolset.UnAggro(me);
            }

            var target = me.ClosestToMouseTarget(10000);
			if (target == null) return;
			if (!target.IsIllusion && target.IsAlive)
			{
				Vector2 target_health_bar = HeroPositionOnScreen(target);
				Drawing.DrawText("Target to Death", target_health_bar, new Vector2(16, 20), me.Distance2D(target) < 1200 ? Color.Coral : Color.WhiteSmoke, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
			}

		}
		

		private static void Drawing_OnPostReset(EventArgs args)
		{
			_text.OnResetDevice();
		}

		private static void Drawing_OnPreReset(EventArgs args)
		{
			_text.OnLostDevice();
		}
		

		Vector2 HeroPositionOnScreen(Hero x)
		{
			float scaleX = HUDInfo.ScreenSizeX();
			float scaleY = HUDInfo.ScreenSizeY();
			Vector2 PicPosition;
			Drawing.WorldToScreen(x.Position, out PicPosition);
			PicPosition = new Vector2((float)(PicPosition.X + (scaleX * -0.035)), (float)((PicPosition.Y) + (scaleY * -0.10)));
			return PicPosition;
		}
		private void OnLoadMessage()
		{
			Game.PrintMessage("<font face='verdana' color='#ffa420'>@addon OtherAddons is Loaded!</font>", MessageType.LogMessage);
			Service.Debug.Print.ConsoleMessage.Encolored("@addon OtherAddons is Loaded!", ConsoleColor.Yellow);
		}
	}
}
