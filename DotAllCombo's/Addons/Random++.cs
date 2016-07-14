

namespace DotaAllCombo.Addons
{
	using System;
	using Ensage;
	using Ensage.Common;
	using SharpDX;

	
	public class GreaterBash
	{
		private const double C = 0.04069;
		private Hero me;
		private readonly Ability ability;

		private readonly DotaTexture abilityIcon;

		private bool attacked;
		private Vector2 iconSize;

		private uint unsuccessfulAttackCount;
		public static Hero Hero { get; set; }
		public GreaterBash(Ability ability)
		{
			this.ability = me.Spellbook.Spell3;
			this.unsuccessfulAttackCount = 1;
			this.abilityIcon = Drawing.GetTexture("materials/ensage_ui/spellicons/spirit_breaker_greater_bash");
			this.iconSize = new Vector2(HUDInfo.GetHpBarSizeY() * 2);
		}

		public double GetChance
		{
			get
			{
				return C * this.unsuccessfulAttackCount;
			}
		}
		public void CheckProc()
		{
			if (this.ability.Level <= 0)
			{
				return;
			}

			var onCooldown = Orbwalking.AttackOnCooldown();
			if (this.attacked && !onCooldown)
			{
				this.attacked = false;
			}

			var canCancel = Orbwalking.CanCancelAnimation();
			if (!canCancel || !onCooldown || this.attacked)
			{
				return;
			}

			DelayAction.Add(
				Game.Ping,
				() =>
				{
					if (this.ability.Cooldown > 0)
					{
						this.unsuccessfulAttackCount = 1;
					}
					else
					{
						this.unsuccessfulAttackCount += 1;
					}
				});
			this.attacked = true;
		}

		public void DrawChance()
		{
			if (this.ability.Level <= 0)
			{
				return;
			}

			Vector2 screenPosition;
			if (
				!Drawing.WorldToScreen(
					Hero.Position + new Vector3(0, 0, Hero.HealthBarOffset / 3),
					out screenPosition))
			{
				return;
			}

			screenPosition += new Vector2((float)(-this.iconSize.X * 0.2), this.iconSize.Y * 2);
			Drawing.DrawRect(screenPosition, this.iconSize, this.abilityIcon);
			var chance = Math.Floor(this.GetChance * 100) + " %";
			Drawing.DrawText(
				chance,
				screenPosition + new Vector2(this.iconSize.X + 2, 2),
				new Vector2((float)(this.iconSize.X * 0.85)),
				Color.White,
				FontFlags.AntiAlias | FontFlags.Additive);
		}

	}
}
