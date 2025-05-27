using UnityEngine;

namespace Platformer {

    [CreateAssetMenu(fileName = "Collectable", menuName = "ScriptableObject/Collectable")]
    public class CollectableData : EntityData {
        public int score;
    }
}