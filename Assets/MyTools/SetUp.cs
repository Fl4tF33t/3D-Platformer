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
}
