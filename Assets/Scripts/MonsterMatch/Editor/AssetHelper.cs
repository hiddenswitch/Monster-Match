using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonsterMatch.UI;
using UnityEditor;
using UnityEngine;

namespace MonsterMatch.Assets
{
    public static class AssetHelper
    {
        [MenuItem("Assets/Create/Catalogue Sprites")]
        public static void CreateCatalogueSpritesAsset()
        {
            ScriptableObjectUtility.CreateAsset<CatalogueSpritesAsset>();
        }

        [MenuItem("Assets/Create/Profile Creator")]
        public static void CreateProfileCreatorAsset()
        {
            var textures = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
            var sprites = textures.Select(t => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(t)))
                .ToArray();
            var asset = ScriptableObjectUtility.CreateAsset<ProfileCreatorAsset>();
            if (sprites.Length == 0)
            {
                return;
            }

            var groupedByNameAndColor = sprites.GroupBy(sprite =>
            {
                if (sprite.name.Contains("-") || sprite.name.Contains("_"))
                {
                    var strings = sprite.name.Split('-', '_');
                    var nameString = strings[0];
                    var colorString = strings[1];
                    if (Enum.TryParse<CatalogueColorEnum>(colorString, true, out var color))
                    {
                        return new Tuple<string, CatalogueColorEnum>(nameString, color);
                    }

                    Debug.LogWarning(
                        $"Sprite {sprite.name} had a name with a dash / underscore in it but the color for it could not be found.");
                    return new Tuple<string, CatalogueColorEnum>(sprite.name, CatalogueColorEnum.None);
                }

                return new Tuple<string, CatalogueColorEnum>(sprite.name, CatalogueColorEnum.None);
            });

            var items = new List<CatalogueItemAsset>();
            foreach (var grouping in groupedByNameAndColor)
            {
                foreach (var sprite in grouping)
                {
                    var path = AssetDatabase.GetAssetPath((Sprite) sprite);
                    var categoryFolder = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar).LastOrDefault();
                    if (!Enum.TryParse<CategoryEnum>(categoryFolder.Replace(" ", ""), true, out var category))
                    {
                        throw new UnityException(
                            $"Category folder {categoryFolder} has no corresponding category enum.");
                    }

                    var flips = category == CategoryEnum.Ears || category == CategoryEnum.Eyes;

                    items.Add(new CatalogueItemAsset()
                    {
                        category = category,
                        color = grouping.Key.Item2,
                        sprite = sprite,
                        flipsAfterStamp = flips,
                        name = grouping.Key.Item1
                    });
                }
            }

            asset.catalogueItemAssets = items.ToArray();
            Debug.Log($"AssetHelper.CreateProfileCreatorAsset: Wrote {items.Count} assets.");
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/Chat Bubble Style")]
        public static void CreateChatBubbleStyle()
        {
            ScriptableObjectUtility.CreateAsset<ChatBubbleStyle>();
        }

        [MenuItem("Assets/Create/Profiles")]
        public static void CreateProfilesAsset()
        {
            ScriptableObjectUtility.CreateAsset<MonsterMatchProfilesAsset>();
        }
    }
}