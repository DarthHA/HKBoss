using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HKBoss
{
	public class HKBoss : Mod
	{
        public override void Load()
        {
            On.Terraria.GameContent.Events.MoonlordDeathDrama.DrawWhite += new On.Terraria.GameContent.Events.MoonlordDeathDrama.hook_DrawWhite(DrawWhiteHook);
            On.Terraria.Player.KillMeForGood += new On.Terraria.Player.hook_KillMeForGood(KillMeForGoodHook);
            On.Terraria.Player.DropCoins += new On.Terraria.Player.hook_DropCoins(DropCoinsHook);
            On.Terraria.Player.DropItems += new On.Terraria.Player.hook_DropItems(DropItemsHook);
            On.Terraria.Player.DropTombstone += new On.Terraria.Player.hook_DropTombstone(DropTombstoneHook);

            ModTranslation CustomText = CreateTranslation("Mode1");
            CustomText.SetDefault("ATTUNED");
            CustomText.AddTranslation(GameCulture.Chinese, "调谐");
            AddTranslation(CustomText);

            CustomText = CreateTranslation("Mode2");
            CustomText.SetDefault("ASCENDED");
            CustomText.AddTranslation(GameCulture.Chinese, "晋升");
            AddTranslation(CustomText);

            CustomText = CreateTranslation("Mode3");
            CustomText.SetDefault("RADIANT");
            CustomText.AddTranslation(GameCulture.Chinese, "辐辉");
            AddTranslation(CustomText);
        }


        public static void DropTombstoneHook(On.Terraria.Player.orig_DropTombstone orig, Player self, int coinowned, NetworkText deathText, int hitDirection)
        {
            if (!Subworld.IsActive<DreamBattleWorld>())
            {
                orig.Invoke(self,coinowned,deathText,hitDirection);
            }
        }

        public static void DropItemsHook(On.Terraria.Player.orig_DropItems orig,Player self)
        {
            if (!Subworld.IsActive<DreamBattleWorld>())
            {
                orig.Invoke(self);
            }
        }

        public static int DropCoinsHook(On.Terraria.Player.orig_DropCoins orig,Player self)
        {
            if (Subworld.IsActive<DreamBattleWorld>())
            {
                self.lostCoins = 0;
                self.lostCoinString = " ";
            }
            else
            {
                orig.Invoke(self);
            }
            return 0;
        }


        public static void KillMeForGoodHook(On.Terraria.Player.orig_KillMeForGood orig,Player self)
        {
            if (!Subworld.IsActive<DreamBattleWorld>())
            {
                orig.Invoke(self);
            }
        }

        public static void DrawWhiteHook(On.Terraria.GameContent.Events.MoonlordDeathDrama.orig_DrawWhite orig,SpriteBatch spritebatch)
        {
            if (SubworldLibrary.Subworld.IsActive<DreamBattleWorld>())
            {
                if (DreamModWorld.enWhite == 0f)
                {
                    return;
                }
                Color color = Color.White * DreamModWorld.enWhite;
                Main.spriteBatch.Draw(Main.magicPixel, new Rectangle(-2, -2, Main.screenWidth + 4, Main.screenHeight + 4), new Rectangle?(new Rectangle(0, 0, 1, 1)), color);
            }
            else
            {
                orig.Invoke(spritebatch);
            }
        }
    }

	public static class SomeUtils
    {
		public static void SummonParticle(Vector2 Pos,float v)
        {
			Projectile.NewProjectile(Pos, (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * v, ModContent.ProjectileType<DreamParticle>(), 1, 0, default, Main.rand.Next(3));
        }
    }

    public class HKPlayer : ModPlayer
    {
        public override void SetControls()
        {
        }
        public override void PostUpdateEquips()
        {
            if (Subworld.IsActive<DreamBattleWorld>())
            {
                player.gravControl = false;
                player.gravControl2 = false;
                player.controlHook = false;
                player.wingTime = -1;
                player.rocketTimeMax = player.rocketTimeMax = 0;
                player.shadowDodge = false;
                player.onHitDodge = false;
            }
        }
        public override void PostUpdateBuffs()
        {
            player.gravControl = false;
            player.gravControl2 = false;
            player.gravDir = 1;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (Subworld.IsActive<DreamBattleWorld>() && DreamModWorld.Difficulty == 2) 
            {
                player.KillMe(PlayerDeathReason.LegacyDefault(), damage, hitDirection, false);
            }
            return true;
        }
        public override void PostUpdateMiscEffects()
        {
            if (Subworld.IsActive<DreamBattleWorld>())
            {
                Vector2 vector3;
                if (!player.mount.Active || !player.mount.Cart)
                {
                    vector3 = Collision.HurtTiles(player.position, player.velocity, player.width, player.height, false);
                }
                else
                {
                    vector3 = Collision.HurtTiles(player.position, player.velocity, player.width, player.height - 16, false);
                }
                if (vector3.Y != 0f)
                {
                    player.Hurt(PlayerDeathReason.ByOther(3),Main.DamageVar(80), 0, false, false, false, 0);
                    if (!player.dead)
                    {
                        player.Spawn();
                    }
                }

                player.spikedBoots = 2;
                player.longInvince = false;
                player.noFallDmg = true;

                int scale = 0;
                bool[] dj = new bool[5];
                if (player.doubleJumpBlizzard) 
                {
                    scale++;
                    dj[0] = true;
                }
                if (player.doubleJumpCloud)
                {
                    scale++;
                    dj[1] = true;
                }
                if (player.doubleJumpFart)
                {
                    scale++;
                    dj[2] = true;
                }
                if (player.doubleJumpSail)
                {
                    scale++;
                    dj[3] = true;
                }
                if (player.doubleJumpSandstorm)
                {
                    scale++;
                    dj[4] = true;
                }
                if (scale > 1)
                {
                    player.doubleJumpSandstorm = false;
                    player.doubleJumpSail = false;
                    player.doubleJumpFart = false;
                    player.doubleJumpCloud = false;
                    player.doubleJumpBlizzard = false;
                    if (dj[2])
                    {
                        player.doubleJumpFart = true;
                    }
                    else if (dj[4])
                    {
                        player.doubleJumpSandstorm = true;
                    }
                    else if (dj[0])
                    {
                        player.doubleJumpBlizzard = true;
                    }
                    else if (dj[3])
                    {
                        player.doubleJumpSail = true;
                    }
                    else if (dj[1])
                    {
                        player.doubleJumpCloud = true;
                    }
                    player.jumpSpeedBoost = 0;
                }
                
                player.ClearBuff(BuffID.MinecartLeft);
                player.ClearBuff(BuffID.MinecartLeftMech);
                player.ClearBuff(BuffID.MinecartLeftWood);
                player.ClearBuff(BuffID.MinecartRight);
                player.ClearBuff(BuffID.MinecartRightMech);
                player.ClearBuff(BuffID.MinecartRightWood);

                player.mount._active = false;
                player.accFlipper = false;
                player.merman = false;
                player.waterWalk = false;
                player.waterWalk2 = false;
                player.adjWater = false;
                player.controlHook = false;

                for (int i = -2; i <= 2; i++)
                {
                    Vector2 pos = player.Center;
                    pos.X += i * 16;
                    pos.Y += player.height / 2;
                    if (player.mount.Active)
                        pos.Y += player.mount.HeightBoost;
                    pos.Y += 8;

                    Tile tile = Framing.GetTileSafely((int)(pos.X / 16), (int)(pos.Y / 16));
                    if (Main.tileSolidTop[tile.type])
                    {
                        tile.inActive(true);
                    }
                }

                for (int i = -4; i <= 4; i++)
                {
                    for (int j = -4; j <= 4; j++)
                    {
                        Vector2 pos = player.Center;
                        pos.X += i * 16;
                        pos.Y += player.height / 2;
                        if (player.mount.Active)
                            pos.Y += player.mount.HeightBoost;
                        pos.Y += j * 8;

                        Tile tile = Framing.GetTileSafely((int)(pos.X / 16), (int)(pos.Y / 16));
                        if (tile.type == TileID.Rope || tile.type == TileID.SilkRope || tile.type == TileID.VineRope || tile.type == TileID.WebRope)
                        {
                            WorldGen.KillTile((int)(pos.X / 16), (int)(pos.Y / 16));
                        }
                    }
                }
            }
        }
    }
}