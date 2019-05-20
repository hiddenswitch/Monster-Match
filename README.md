# MonsterMatch

This is the code for the online interactive and game Monstermatch located at [https://monstermatch.hiddenswitch.com](https://monstermatch.hiddenswitch.com).

## The Algorithm

This game uses non-negative matrix factorization to impute unknown player ratings, trained with the ratings of 100 simulated users on the 55 monster profiles the player can see in the game.

 - [Data](Assets/Scripts/MonsterMatch/CollaborativeFiltering/MonsterMatchArrayData.cs)
 - [Recommender](Assets/Scripts/MonsterMatch/CollaborativeFiltering/CollaborativeFilteringRecommender.cs)
 - [Algorithm](Assets/Scripts/MonsterMatch/CollaborativeFiltering/NonnegativeMatrixFactorization.cs)
 
Additionally, MonsterMatch values the most recent ratings (like or pass) in the swiping session most highly.

[Read more about dating algorithms here.](https://monstermatch.hiddenswitch.com/algorithms)

## Getting Started

Using this source code requires the Unity editor, version 2018.3.13f1, on a macOS system.

Certain commercial asset store packages were used in creating this project. They were omitted from this repository to respect their license terms. You will need the following asset store packages:

 - [MaterialUI](https://assetstore.unity.com/packages/tools/gui/materialui-51870)
 - [DOTween Pro](https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416)
 - [FlexibleKeyboard](https://assetstore.unity.com/packages/tools/gui/flexible-keyboard-39752)
 - [Optimized ScrollView Adapter](https://assetstore.unity.com/packages/tools/gui/optimized-scrollview-adapter-listview-gridview-68436)
 
 Optionally, you can also install these editor packages:
 
  - [Sprite Font Builder](https://assetstore.unity.com/packages/tools/gui/sprite-font-builder-37343)
  - [Build Report Tool](https://assetstore.unity.com/packages/tools/utilities/build-report-tool-8162)

Some small changes were made to these packages that will not effect the overall running of the game. Please contact me for those patches.

This source only supports the WebGL target platform. Run `updateunityloader.sh` to copy our modified loader into your `PlaybackEngines` directory, which enables WebGL to run on mobile devices.

## Credits

Please visit the [About page](https://monstermatch.hiddenswitch.com/about) for a full list of credits.

## License

Wherever code is authored by **doctorpangloss**, it is licensed under the [Affero GPL version 3 license](https://www.gnu.org/licenses/agpl-3.0.en.html).

Some packages, like UniRx, are licensed under their own terms in a compatible way.

Creative assets (files ending in `.ink`, `.json`, `.png`, `.psd`, and all other text and images) are Copyright 2019 Hidden Switch, All Rights Reserved. Additionally, these assets can be used as distributed in the MonsterMatch binaries, on social media, blogs, news or other user-generated non-game or journalistic content commenting on or promoting MonsterMatch, and with this source code for educational purposes. You may not use the creative assets for any other purpose, commercial or otherwise, including but not limited to your own games, avatars that have nothing to do with MonsterMatch, or other unrelated media.
