using System.Linq;
using UnityEditor;

namespace MonsterMatch.Editor
{
    public static class BuildScripts
    {
        [MenuItem("Build/WebGL")]
        public static void BuildWebGL()
        {
            var buildPath = GetArg("-buildPath", "obj/webgl");

            PlayerSettings.WebGL.emscriptenArgs = "";
            
            var buildPlayerOptions = new BuildPlayerOptions()
            {
                locationPathName = buildPath,
                scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
                targetGroup = BuildTargetGroup.WebGL,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Both;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Medium);
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }
        
        private static string GetArg(string arg, string defaultValue)
        {
            var args = System.Environment.GetCommandLineArgs();
            var value = defaultValue;
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == arg)
                {
                    value = args[i + 1];
                    break;
                }
            }

            return value;
        }
    }
}