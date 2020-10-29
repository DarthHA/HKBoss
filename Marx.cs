
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Specialized;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HKBoss
{
    [AutoloadBossHead]
    public class Marx : ModNPC
    {
        public bool HarfLife = false;
        public int SRTimer = 0;
        public bool ProjResist = false;
        public int KBTimer = -1;
        public Vector2 OldVel = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Markoth");
            DisplayName.AddTranslation(GameCulture.Chinese, "马科斯");
            Main.npcFrameCount[npc.type] = 15;
        }
        public override void SetDefaults()
        {
            npc.width = 100;
            npc.height = 200;
            npc.damage = 90;
            npc.defense = 0;
            npc.lifeMax = 15000;
            if (DreamModWorld.Difficulty > 0)
            {
                npc.damage = 180;
                npc.lifeMax = 25000;
            }
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            npc.netAlways = true;
            musicPriority = MusicPriority.BossHigh;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/DreamBattle");
            for(int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage /= 2;
            npc.lifeMax /= 2;
        }
        public override void AI()
        {
            if (Main.rand.Next(3) == 0)
            {
                SomeUtils.SummonParticle(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), Main.rand.Next(6) + 6);
            }

            npc.TargetClosest();
            Player target = Main.player[npc.target];
            if (npc.ai[0] < 3)
            {
                if (target.dead || !target.active)
                {
                    npc.active = false;
                }
            }
            target.thorns = 0;
            if (KBTimer > 0)
            {
                KBTimer--;
                if (KBTimer == 0)
                {
                    KBTimer--;
                    npc.velocity = Vector2.Zero;
                }
            }
            if (npc.ai[0] == 0)
            {
                npc.dontTakeDamage = true;
                npc.localAI[3] = 1;
                if (npc.ai[2] == 0)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/MarkothCastCalm").WithVolume(2f).WithPitchVariance(.5f), npc.Center);
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DreamShield>(), npc.damage / 4, 0, default, npc.whoAmI, 0);
                }
                npc.ai[2]++;
                if (npc.ai[2] < 20)
                {
                    target.GetModPlayer<ShakeScreenPlayer>().shakeSubtle = true;
                }
                if (npc.ai[2] > 60)
                {
                    npc.dontTakeDamage = false;
                    npc.ai[0] = 1;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                }
            }
            if (npc.ai[0] == 1)
            {
                if (npc.ai[1] == 0)           //放骨钉
                {
                    SRTimer = (SRTimer + 1) % 1300;
                    if (SRTimer < 600)
                    {
                        npc.localAI[3] = 1;
                    }
                    if (SRTimer >= 600 && SRTimer <= 700)
                    {
                        npc.localAI[3] = 0;
                    }
                    if (SRTimer > 700)
                    {
                        npc.localAI[3] = -1;
                    }


                    if (npc.velocity == Vector2.Zero)
                    {
                        if (OldVel == Vector2.Zero)
                        {
                            npc.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 4;
                        }
                        else
                        {
                            npc.velocity = OldVel;
                        }
                    }
                    if (npc.Distance(target.Center) > 1000)
                    {
                        npc.velocity = Vector2.Normalize(target.Center - npc.Center) * 4f;
                    }
                    if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {

                        if (Collision.SolidCollision(npc.position + new Vector2(npc.velocity.X, 0), npc.width, npc.height))
                        {
                            npc.velocity.X = -npc.velocity.X;
                        }
                        if (Collision.SolidCollision(npc.position + new Vector2(0, npc.velocity.Y), npc.width, npc.height))
                        {
                            npc.velocity.Y = -npc.velocity.Y;
                        }
                    }
                    else
                    {
                        npc.velocity = Vector2.Normalize(target.Center - npc.Center) * 4f;
                    }
                    npc.ai[2]++;
                    int freq = HarfLife ? 40 : 50;
                    if (npc.ai[2] % freq == freq - 1) 
                    {
                        Vector2 Pos = target.Center + (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 400;
                        Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<Nail>(), npc.damage / 4, 0, default);
                    }
                    CheckLife();
                    if (npc.ai[2] > 445)
                    {
                        if (npc.localAI[3] == 0) npc.localAI[3] = Main.rand.Next(2) * 2 - 1;
                        npc.ai[1] = 1;
                        npc.ai[2] = 0;
                    }
                }
                else if (npc.ai[1] == 1)      //开始转
                {
                    if (npc.ai[2] == 0)
                    {
                        npc.width = 200;
                        npc.position.X -= 50;
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/MarkothCast2").WithVolume(2f).WithPitchVariance(.5f), npc.Center);
                    }
                    npc.velocity *= 0.5f;
                    npc.ai[2]++;
                    if (npc.ai[2] > 15)
                    {
                        npc.ai[1] = 2;
                        npc.ai[2] = 0;
                    }
                }
                else if (npc.ai[1] == 2)   //转盾
                {
                    npc.ai[2]++;
                    if (npc.ai[2] > 600)
                    {
                        npc.ai[1] = 3;
                        npc.ai[2] = 0;
                    }
                }
                else if (npc.ai[1] == 3)         //结束转
                {
                    npc.velocity *= 0.5f;
                    npc.ai[2]++;
                    if (npc.ai[2] > 15)
                    {
                        ProjResist = false;
                        npc.width = 100;
                        npc.position.X += 50;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        CheckLife();
                        npc.velocity = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * 4;
                    }
                }
            }

            if (npc.ai[0] == 2)
            {
                if (npc.ai[1] == 1)
                {
                    npc.dontTakeDamage = true;
                    npc.velocity *= 0.5f;
                    npc.ai[2]++;
                    if (npc.ai[2] > 15)
                    {
                        npc.ai[1] = 2;
                        npc.ai[2] = 0;
                    }
                }
                else if (npc.ai[1] == 2)
                {
                    npc.velocity = Vector2.Zero;
                    npc.ai[2]++;
                    if (npc.ai[2] == 30)
                    {
                        Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DreamShield>(), npc.damage / 4, 0, default, npc.whoAmI, 1);
                    }
                    if (npc.ai[2] > 90)
                    {
                        npc.ai[1] = 3;
                        npc.ai[2] = 0;
                    }
                }
                else if (npc.ai[1] == 3)
                {
                    npc.velocity *= 0.5f;
                    npc.ai[2]++;
                    if (npc.ai[2] > 15)
                    {
                        npc.dontTakeDamage = false;
                        npc.ai[0] = 1;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                }
            }

            if (npc.ai[0] == 3)
            {
                music = -1;
                if (npc.ai[2] == 1)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/MarkothDeath").WithVolume(2f).WithPitchVariance(.5f), npc.Center);
                }
                npc.velocity *= 0.5f;
                npc.ai[2]++;
                if (npc.ai[2] > 60 && npc.ai[2] < 300)
                {
                    target.GetModPlayer<ShakeScreenPlayer>().shake = true;
                    for (int i = 0; i < 4; i++)
                    {
                        SomeUtils.SummonParticle(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), Main.rand.Next(25) + 5);
                    }
                }
                if (npc.ai[2] == 360)
                {
                    for(int i = 0; i < 240; i++)
                    {
                        SomeUtils.SummonParticle(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), Main.rand.Next(25) + 5);
                    }
                    npc.life = 0;
                    npc.checkDead();
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/DreamEssenceCorpseCollect").WithVolume(2f).WithPitchVariance(.5f), npc.Center);
                }
            }
        }


        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] < 3)
            {
                if (npc.ai[1] == 0)
                {
                    if (npc.frameCounter > 5)
                    {
                        npc.frameCounter = 0;
                        npc.frame = new Rectangle(0, (npc.frame.Y + 500) % 3000, 500, 500);
                    }
                    npc.frameCounter++;
                }

                if (npc.ai[1] == 1 || npc.ai[1] == 3)
                {
                    if (npc.frameCounter > 3)
                    {
                        npc.frameCounter = 0;
                        if (npc.frame.Y < 5500 || npc.frame.Y > 7000)
                        {
                            npc.frame.Y = 5500;
                        }
                        npc.frame = new Rectangle(0, npc.frame.Y + 500, 500, 500);
                        if (npc.frame.Y > 7000)
                        {
                            npc.frame.Y = 5500;
                        }
                    }
                    npc.frameCounter++;
                }
                if (npc.ai[1] == 2)
                {
                    if (npc.frameCounter > 5)
                    {
                        npc.frameCounter = 0;
                        if (npc.frame.Y < 3500 || npc.frame.Y > 5000)
                        {
                            npc.frame.Y = 3500;
                        }
                        npc.frame = new Rectangle(0, npc.frame.Y + 500, 500, 500);
                        if (npc.frame.Y > 5000)
                        {
                            npc.frame.Y = 3500;
                        }
                    }
                    npc.frameCounter++;
                }
            }

            if (npc.ai[0] == 3)
            {
                if (npc.frameCounter > 5)
                {
                    npc.frameCounter = 0;
                    if (npc.frame.Y < 3500 || npc.frame.Y > 5000)
                    {
                        npc.frame.Y = 3500;
                    }
                    npc.frame = new Rectangle(0, npc.frame.Y + 500, 500, 500);
                    if (npc.frame.Y > 5000)
                    {
                        npc.frame.Y = 3500;
                    }
                }
                npc.frameCounter++;
            }
        }
        public override bool CheckDead()
        {
            if (npc.ai[0] < 3)
            {
                npc.life = 1;
                npc.ai[0] = 3;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.dontTakeDamage = true;
                foreach(Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<DreamShield>() && proj.ai[0] == npc.whoAmI)
                    {
                        proj.Kill();
                    }
                }
                return false;
            }
            return true;
        }
        public void CheckLife()
        {
            if (npc.life < npc.lifeMax / 2 && !HarfLife) 
            {
                HarfLife = true;
                npc.ai[0] = 2;
                npc.ai[1] = 1;
                npc.ai[2] = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D tex = Main.npcTexture[npc.type];
            Vector2 Center = npc.position + new Vector2(npc.width / 2, 150);
            Color color = Color.White;
            switch (KBTimer)
            {
                case 1:
                case 4:
                    color.A /= 3;
                    break;
                case 2:
                case 3:
                    color.A = 0;
                    break;
                case 0:
                case 5:
                    color.A /= 2;
                    break;
                default:
                    break;
            }
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            //spriteBatch.Draw(tex, Center - Main.screenPosition, npc.frame, Color.Goldenrod, npc.rotation, npc.frame.Size() / 2, npc.scale * 0.71f, SpriteEffects.None, 0);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(tex, Center - Main.screenPosition, npc.frame, color, npc.rotation, npc.frame.Size() / 2, npc.scale * 0.7f, SpriteEffects.None, 0);

            if (npc.ai[0] == 3 && npc.ai[2] < 20)
            {

                Vector2 ScreenPos = npc.Center - Main.screenPosition;
                int R = (int)(64 * npc.scale * Math.Sqrt(npc.ai[2] / 10) * 10);
                if (npc.ai[2] >= 10)
                {
                    R = (int)(64 * npc.scale);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                DrawData value10 = new DrawData(TextureManager.Load("Images/Misc/Perlin"), ScreenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, R * 2, R * 2)), new Color(50, 50, 50, 50), npc.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                GameShaders.Misc["ForceField"].Apply(new DrawData?(value10));
                value10.Draw(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone);
            }

            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ProjResist)
            {
                damage /= 12;
                crit = false;
                if (DreamModWorld.Difficulty > 0)
                {
                    damage = 0;
                }
            }
            else
            {
                damage *= 2;
                crit = true;
                ProjResist = true;

                KBTimer = 5;
                OldVel = npc.velocity;
                npc.velocity = Vector2.Normalize(npc.Center - projectile.Center + new Vector2(0, 0.001f)) * 40;
            }
            if (!HarfLife && npc.life < npc.lifeMax / 2)
            {
                damage = 1;
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (item.type != ItemID.PsychoKnife)
            {
                damage *= 4;
            }
            if (!HarfLife && npc.life < npc.lifeMax / 2)
            {
                damage = 1;
            }
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            KBTimer = 5;
            OldVel = npc.velocity;
            npc.velocity = Vector2.Normalize(npc.Center - player.Center + new Vector2(0, 0.001f)) * 40;
            for (int i = 0; i < 14; i++)
            {
                float r = (npc.Center - player.Center).ToRotation();
                Projectile.NewProjectile(npc.Center, (r + Main.rand.NextFloat() * MathHelper.Pi / 3 - MathHelper.Pi / 6).ToRotationVector2() * (Main.rand.Next(35) + 10), ModContent.ProjectileType<DreamParticle>(), 1, 0, default, Main.rand.Next(3));
            }
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 14; i++)
            {
                float r = (npc.Center - projectile.Center).ToRotation();
                Projectile.NewProjectile(npc.Center, (r + Main.rand.NextFloat() * MathHelper.Pi / 3 - MathHelper.Pi / 6).ToRotationVector2() * (Main.rand.Next(35) + 10), ModContent.ProjectileType<DreamParticle>(), 1, 0, default, Main.rand.Next(3));
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.None;
        }
        public override void NPCLoot()
        {
            if (DreamModWorld.Difficulty == 0)
            {
                DreamModWorld.DownedMarx1 = true;
            }
            if (DreamModWorld.Difficulty == 1)
            {
                DreamModWorld.DownedMarx2 = true;
            }
            if (DreamModWorld.Difficulty == 2)
            {
                DreamModWorld.DownedMarx3 = true;
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return (npc.ai[0] < 3);
        }
    }
}