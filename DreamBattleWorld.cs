using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Terraria.World.Generation;
using Terraria.ID;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria.GameContent.Events;
using IL.Terraria.Initializers;
using Terraria.ModLoader.IO;

namespace HKBoss
{

	public class DreamBattleWorld : Subworld
	{
		public override int width => 500;
		public override int height => 750;

		public override ModWorld modWorld => ModContent.GetInstance<DreamModWorld>();

		public override bool saveSubworld => false;
		public override bool disablePlayerSaving => false;
		public override bool saveModData => false;
		public override ushort votingDuration => 120;
        public override void OnVotedFor()
        {
			
        }
		
        public override List<GenPass> tasks => new List<GenPass>()
		{
		new SubworldGenPass(progress =>
		{
			progress.Message = "... ... ..."; //Sets the text above the worldgen progress bar
			Main.worldSurface = Main.maxTilesY - 31; //Hides the underground layer just out of bounds 42
			Main.rockLayer = Main.maxTilesY-30; //Hides the cavern layer way out of bounds
			Main.spawnTileX = Main.maxTilesX / 2;
			Main.spawnTileY = Main.maxTilesY - 335;
			for(int i = 1; i < Main.maxTilesX - 1 ; i++)
			{
				for(int j = Main.maxTilesY - 1; j > Main.maxTilesY - 301; j--)
				{
					WorldGen.PlaceTile(i,j,TileID.TinBrick,true,true);
				}
				for(int j = 1; j < 201; j++)
				{
					WorldGen.PlaceTile(i,j,TileID.TinBrick,true,true);
				}
			}
			progress.Value = 0.25f;
			if(DreamModWorld.Difficulty > 0)
			{
				for(int i = 201; i < Main.maxTilesX - 201;i++)
				{
					WorldGen.PlaceTile(i,Main.maxTilesY-301,TileID.Spikes,true,true);
				}
			}
			for(int j = 1; j < Main.maxTilesY - 1; j++)
			{
				for(int i = 1; i < 201; i++)
				{
					WorldGen.PlaceTile(i,j,TileID.TinBrick,true,true);            // 201,201,201,201
				}
				for(int i = Main.maxTilesX - 1; i > Main.maxTilesX - 201; i--)
				{
					WorldGen.PlaceTile(i,j,TileID.TinBrick,true,true);
				}
			}
			progress.Value = 0.5f;
			SetPlatForm(Main.maxTilesX / 2 + 33,Main.maxTilesY - 316);
			SetPlatForm(Main.maxTilesX / 2 - 33,Main.maxTilesY - 316);
			SetPlatForm(Main.maxTilesX / 2 - 3,Main.maxTilesY - 331);
			SetPlatForm(Main.maxTilesX / 2 + 33,Main.maxTilesY - 347);
			SetPlatForm(Main.maxTilesX / 2 - 33,Main.maxTilesY - 347);
			SetPlatForm(Main.maxTilesX / 2 - 3,Main.maxTilesY - 362);
			progress.Value = 0.75f;
			for(int j = Main.maxTilesY - 400; j > 301; j--)
            {
				WorldGen.PlaceTile(201,j,TileID.Spikes);
				WorldGen.PlaceTile(Main.maxTilesX - 201,j,TileID.Spikes);
			}
			progress.Value = 1;
		})
	};

		public void SetPlatForm(int x,int y)
        {
			for(int i = x; i <= x + 6; i++)
            {
				for(int j = y; j <= y + 1; j++)
                {
					WorldGen.PlaceTile(i, j, TileID.TinBrick, true, true);
                }
            }
        }
		public override void Load()
		{
			Main.dayTime = true;
			Main.time = 27000;
			
		}
        public override UIState loadingUIState => base.loadingUIState;
    }

	public class DreamWorldGNPC : GlobalNPC
    {
        public override void AI(NPC npc)
        {
			if (Subworld.IsActive<DreamBattleWorld>())
			{
                if (npc.type != ModContent.NPCType<Marx>())
                {
					npc.active = false;
                }
			}
		}
    }

	public class DreamModWorld : ModWorld
	{
		public static bool DownedMarx1;
		public static bool DownedMarx2;
		public static bool DownedMarx3;
		public static int Difficulty = 0;
		public static float enWhite = 0;
		public int Timer1 = 0;
		public int Timer2 = -1;
		public bool ExistBoss = false;

        public override TagCompound Save()
        {
			return new TagCompound
			{
				{"DownedMarx1" ,DownedMarx1},
				{"DownedMarx2" ,DownedMarx2},
				{"DownedMarx3" ,DownedMarx3},
			};
        }
        public override void Load(TagCompound tag)
        {
			DownedMarx1 = false;
			DownedMarx2 = false;
			DownedMarx3 = false;
			DownedMarx1 = tag.GetBool("DownedMarx1");
			DownedMarx2 = tag.GetBool("DownedMarx2");
			DownedMarx3 = tag.GetBool("DownedMarx3");
		}
        public override void PreUpdate()
        {
			if (DreamBattleWorld.IsActive<DreamBattleWorld>())
			{
				enWhite = 0;
                if (Timer1 < 60)
                {
					enWhite = (float)(60 - Timer1) / 60;
				}
				if (Timer1 < 180)
				{
					Timer1++;
				}
				if (Timer1 == 180)
				{
					Timer1++;
					Vector2 Pos = Main.LocalPlayer.Center - new Vector2(0, 400);
					NPC.NewNPC((int)Pos.X, (int)Pos.Y, ModContent.NPCType<Marx>());
					ExistBoss = true;
				}
				if (ExistBoss && !NPC.AnyNPCs(ModContent.NPCType<Marx>()))
				{
					ExistBoss = false;
					Timer2 = 180;
				}
				if (Timer2 > 0)
				{
					Timer2--;
				}
				if (Timer2 < 60 && Timer2 >= 0) 
				{
					enWhite = (float)(60 - Timer2) / 60;
				}
				if (Timer2 == 0)
				{
					Timer2--;
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/DreamEnter").WithVolume(2f).WithPitchVariance(.5f), Main.LocalPlayer.Center);
					DreamBattleWorld.Exit();
					Timer1 = 0;
					Timer2 = -1;
					ExistBoss = false;
				}
			}
            else
            {
				enWhite = 0;
				Timer1 = 0;
				Timer2 = -1;
				ExistBoss = false;
			}
        }
    }

	public class DreamGProj : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
			if (Subworld.IsActive<DreamBattleWorld>())
			{
				if (projectile.type == ProjectileID.SandBallFalling || projectile.type == ProjectileID.SandBallGun ||
					projectile.type == ProjectileID.PearlSandBallFalling || projectile.type == ProjectileID.PearlSandBallGun ||
					projectile.type == ProjectileID.EbonsandBallFalling || projectile.type == ProjectileID.EbonsandBallGun ||
					projectile.type == ProjectileID.CrimsandBallFalling || projectile.type == ProjectileID.CrimsandBallGun || projectile.type == ProjectileID.IceBlock)
				{
					projectile.active = false;
				}
			}
        }
    }

	public class DreamGTile : GlobalTile
	{
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
			return !Subworld.IsActive<DreamBattleWorld>();
        }
        public override bool CanExplode(int i, int j, int type)
        {
			return !Subworld.IsActive<DreamBattleWorld>();
		}
        public override bool CanPlace(int i, int j, int type)
        {
			return !Subworld.IsActive<DreamBattleWorld>();
		}
    }
}