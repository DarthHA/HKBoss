using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace HKBoss
{
    public class DreamShield : ModProjectile
    {
        public float R = 150;
        public float R2 = 850;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("DreamShield");
            DisplayName.AddTranslation(GameCulture.Chinese, "梦之盾");
            Main.projFrames[projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 99999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 250;
        }
        public override void AI()
        {
            if (Main.rand.Next(4) == 0) 
            {
                SomeUtils.SummonParticle(projectile.Center, 1);
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 20;
            }
            else
            {
                projectile.alpha = 0;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame = (projectile.frame + 1) % 3;
            }
            projectile.timeLeft = 9999;
            if (!Main.npc[(int)projectile.ai[0]].active) projectile.Kill();
            NPC owner = Main.npc[(int)projectile.ai[0]];
            if (owner.ai[0] == 0)
            {
                projectile.Center = owner.Center + new Vector2(1, 0) * R / 60f * owner.ai[2];
            }
            if (owner.ai[0] == 1)
            {
                if (owner.ai[1] == 0)
                {
                    projectile.Center = owner.Center + (projectile.localAI[1] / 180 * MathHelper.TwoPi).ToRotationVector2() * R;
                    projectile.rotation = (projectile.Center - owner.Center).ToRotation();
                    projectile.localAI[1] = (projectile.localAI[1] + owner.localAI[3]) % 180;
                }
                else if (owner.ai[1] == 1 || owner.ai[1] == 3)
                {
                    projectile.Center = owner.Center + (projectile.localAI[1] / 180 * MathHelper.TwoPi).ToRotationVector2() * R;
                    projectile.rotation = (projectile.Center - owner.Center).ToRotation();
                    projectile.localAI[1] = (projectile.localAI[1] + owner.localAI[3] * Main.rand.Next(2)) % 180;
                }
                else if (owner.ai[1] == 2)
                {
                    float r;
                    if (owner.ai[2] <= 300)
                    {
                        r = R + R2 / 300f * owner.ai[2];
                    }
                    else
                    {
                        r = R + R2 / 300f * (600 - owner.ai[2]);
                    }
                    if (projectile.ai[1] == 1)
                    {
                        r = R;
                    }
                    float v = 1;
                    if (owner.ai[2] > 100 && owner.ai[2] < 500)
                    {
                        v = 4;
                    }
                    else if(owner.ai[2] > 50 || owner.ai[2] < 550)
                    {
                        v = 3;
                    }
                    else if (owner.ai[2] > 25 || owner.ai[2] < 575)
                    {
                        v = 2;
                    }

                    projectile.Center = owner.Center + (projectile.localAI[1] / 180 * MathHelper.TwoPi).ToRotationVector2() * r;
                    projectile.rotation = (projectile.Center - owner.Center).ToRotation();
                    projectile.localAI[1] = (projectile.localAI[1] + owner.localAI[3] * v) % 180;
                }
            }

            if (owner.ai[0] == 2)
            {
                if (owner.ai[1] == 1 || owner.ai[1] == 3)
                {
                    projectile.Center = owner.Center + (projectile.localAI[1] / 180 * MathHelper.TwoPi).ToRotationVector2() * 200;
                    projectile.rotation = (projectile.Center - owner.Center).ToRotation();
                }
                else if (owner.ai[1] == 2)
                {
                    if (projectile.ai[1] == 0)
                    {
                        projectile.Center = owner.Center + (projectile.localAI[1] / 180 * MathHelper.TwoPi).ToRotationVector2() * 200;
                        projectile.rotation = (projectile.Center - owner.Center).ToRotation();
                    }
                    else
                    {
                        float r1 = MathHelper.Pi;
                        foreach(Projectile proj in Main.projectile)
                        {
                            if(proj.active && proj.type == projectile.type)
                            {
                                if (proj.ai[0] == projectile.ai[0] && proj.ai[1] == 0 && proj.whoAmI != projectile.whoAmI)  
                                {
                                    r1 = proj.localAI[1] / 180 * MathHelper.TwoPi;
                                    projectile.localAI[1] = (proj.localAI[1] + 90) % 180;
                                }
                            }
                        }

                        projectile.Center = owner.Center - r1.ToRotationVector2() * 200f / 60f * (owner.ai[2] - 30);
                        projectile.rotation = (projectile.Center - owner.Center).ToRotation();
                    }
                }
            }

            ReflectProj();
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center + (projectile.rotation - MathHelper.Pi / 2).ToRotationVector2() * 40,
                projectile.Center + (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * 40, 30, ref point);
        }
        public void ReflectProj()
        {
            float point = 0f;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && Collision.CheckAABBvLineCollision(proj.position, proj.Hitbox.Size(), projectile.Center + (projectile.rotation - MathHelper.Pi / 2).ToRotationVector2() * 40,
                projectile.Center + (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * 40, 30, ref point))
                {
                    if (proj.friendly && !proj.hostile)
                    {
                        Player player = Main.player[Player.FindClosest(proj.Center, 1, 1)];
                        proj.hostile = true;
                        proj.friendly = false;
                        if (proj.velocity == Vector2.Zero || player.heldProj == proj.whoAmI)
                        {
                            proj.Kill();
                        }
                        else
                        {
                            proj.velocity = Vector2.Normalize(player.Center - proj.Center) * proj.velocity.Length();
                        }
                        Main.PlaySound(SoundID.NPCHit4, proj.Center);
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = SpriteEffects.None;
            Rectangle Frame = new Rectangle(0, projectile.frame * tex.Height / 3, tex.Width, tex.Height / 3);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, Frame, Color.LightGoldenrodYellow * projectile.Opacity, projectile.rotation, Frame.Size() / 2, projectile.scale * 0.7f, SP, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }


    }
}