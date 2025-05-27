using UnityEditor;
using UnityEngine;
using static System.IO.Path;
using static System.IO.Directory;
using static UnityEditor.AssetDatabase;

public static class SetUp {
    [MenuItem("Tools/SetUp/Create Default Folders")]
    public static void CreateDefaultFolders() {
        Folders.CreateDefault("Resources", "Prefabs", "Scenes", "Scripts", "ScriptableObjects", "Animations");
        Refresh();
    }

    [MenuItem("Tools/SetUp/Import My Favourite Assets")]
    public static void ImportFavouriteAssets() {
        Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
    }

    private static class Folders {
        public static void CreateDefault(string root, params string[] folders) {
            string fullPath = Combine(Application.dataPath, root);
            foreach (string folder in folders) {
                string path = Combine(fullPath, folder);
                if (!Exists(path)) {
                    CreateDirectory(path);  
                }
            }
        }
    }
    
    public static class Assets {
        public static void ImportAsset(string asset, string subfolder, string folder = "C:/Users/helmy/AppData/Roaming/Unity/Asset Store-5.x") {
            AssetDatabase.ImportPackage(Combine(folder, subfolder, asset), false);;
        }
    }
}
