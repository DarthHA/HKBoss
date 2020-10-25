using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace HKBoss
{
    public class Nail : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dream Nail");
            DisplayName.AddTranslation(GameCulture.Chinese, "梦之钉");
        }
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 250;
        }
        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 40;
            }
            else
            {
                projectile.alpha = 0;
            }
            Player target = Main.player[Player.FindClosest(projectile.Center, 1, 1)];
            projectile.ai[0]++;

            if (projectile.ai[0] < 40)
            {
                projectile.rotation = (target.Center - projectile.Center).ToRotation();
                projectile.Center -= projectile.rotation.ToRotationVector2() * 1f;

                if (projectile.ai[0] == 1)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/DreamGhostAppear").WithVolume(2f).WithPitchVariance(.5f), Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center);
                    for (int i = 0; i < 15; i++)
                    {
                        float r = projectile.rotation - MathHelper.Pi / 4 * 3 + MathHelper.Pi / 2 * Main.rand.NextFloat() + MathHelper.Pi * Main.rand.Next(2);
                        Projectile.NewProjectile(projectile.Center, r.ToRotationVector2() * (Main.rand.NextFloat() * 10 + 4), ModContent.ProjectileType<DreamParticle>(), 1, 0, default, Main.rand.Next(3));
                    }
                }
            }
            else
            {
                if (projectile.ai[0] >= 45)
                {
                    if (projectile.ai[0] == 45)
                    {
                        Main.PlaySound(SoundID.Item1, Main.player[Player.FindClosest(projectile.Center, 1, 1)].Center);
                    }
                    if (Main.rand.Next(4) == 0)
                    {
                        SomeUtils.SummonParticle(projectile.Center, 1);
                    }
                    projectile.velocity = projectile.rotation.ToRotationVector2() * 25;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + projectile.rotation.ToRotationVector2() * 100,
                projectile.Center - projectile.rotation.ToRotationVector2() * 100, 20, ref point);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.LightGoldenrodYellow * projectile.Opacity, projectile.rotation, tex.Size() / 2, projectile.scale * 0.7f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }


    }
}