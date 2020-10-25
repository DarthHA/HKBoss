using System;
using Terraria;
using Terraria.ModLoader;

namespace HKBoss
{

	public class ShakeScreenPlayer : ModPlayer
	{

		public override void ResetEffects()
		{
			shake = false;
			shakeMega = false;
			shakeSubtle = false;
			shakeQuake = false;
		}


		public override void UpdateDead()
		{
			shake = false;
			shakeMega = false;
			shakeSubtle = false;
			shakeQuake = false;
		}

		public override void ModifyScreenPosition()
		{
			if (shake)
			{
				Main.screenPosition.X += Main.rand.Next(-10, 11);
				Main.screenPosition.Y += Main.rand.Next(-10, 11);
			}
			if (shakeMega)
			{
				Main.screenPosition.X += Main.rand.Next(-20, 21);
				Main.screenPosition.Y += Main.rand.Next(-20, 21);
			}
			if (shakeSubtle)
			{
				Main.screenPosition.X += Main.rand.Next(-3, 3);
				Main.screenPosition.Y += Main.rand.Next(-3, 3);
			}
			if (shakeQuake)
			{
				Main.screenPosition.Y += Main.rand.Next(-5, 5);
			}
		}


		public bool shake;

		public bool shakeMega;

		public bool shakeSubtle;

		public bool shakeQuake;
	}
}