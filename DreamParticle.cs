using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace HKBoss
{
    public class DreamParticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dream Essence");
            DisplayName.AddTranslation(GameCulture.Chinese, "梦境精华（迫真");
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 1;
            projectile.penetrate = -1;
            projectile.alpha = 250;
        }
        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.scale = Main.rand.NextFloat() * 0.7f + 0.3f;
                projectile.localAI[0] = 1;
                //projectile.velocity = (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * Main.rand.NextFloat() * 8;
            }
            projectile.velocity *= 0.93f;
            projectile.ai[1]++;
            if (projectile.ai[1] < 10)
            {
                projectile.alpha -= 30;
                if (projectile.alpha < 0) projectile.alpha = 0;
            }
            else
            {
                projectile.alpha += 7;
                if (projectile.alpha > 250) projectile.Kill();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            Rectangle Frame = new Rectangle(0, (int)projectile.ai[0] * tex.Height / 3, tex.Width, tex.Height / 3);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.Goldenrod * projectile.Opacity, projectile.rotation, Frame.Size() / 2, projectile.scale * 0.3f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }


    }
}