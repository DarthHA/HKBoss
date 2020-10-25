
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HKBoss
{
    public class DreamEnter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Challenge Markoth");
            DisplayName.AddTranslation(GameCulture.Chinese,"挑战马科斯");
            Tooltip.SetDefault("Dreamborn god of meditation and isolation.\nRight click to change difficulty.");
            Tooltip.AddTranslation(GameCulture.Chinese,"冥想与孤立之梦中神\n使用右键来切换难度");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 44;
            item.rare = ItemRarityID.Red;
            item.maxStack = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 45;
            item.useTime = 45;
            item.consumable = false;
            item.expert = true;
        }


        public override bool CanUseItem(Player player)
        {
            return !SubworldLibrary.Subworld.IsActive<DreamBattleWorld>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                if (player.altFunctionUse == 2)
                {
                    DreamModWorld.Difficulty = (DreamModWorld.Difficulty + 1) % 3;
                    string mode = "";
                    Color color = Color.White;
                    switch (DreamModWorld.Difficulty)
                    {
                        case 0:
                            mode = Language.GetTextValue("Mods.HKBoss.Mode1");
                            color = Color.DarkOrange;
                            break;
                        case 1:
                            mode = Language.GetTextValue("Mods.HKBoss.Mode2");
                            color = Color.Silver;
                            break;
                        case 2:
                            mode = Language.GetTextValue("Mods.HKBoss.Mode3");
                            color = Color.Goldenrod;
                            break;
                        default:
                            break;
                    }
                    Main.NewText(mode, color);
                }
                else
                {
                    if (!DreamBattleWorld.IsActive<DreamBattleWorld>())
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/DreamEnter").WithVolume(2f).WithPitchVariance(.5f), player.Center);
                        DreamBattleWorld.Enter<DreamBattleWorld>();
                    }
                }
            }
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.SoulofNight, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}