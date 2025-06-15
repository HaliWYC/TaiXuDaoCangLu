using UnityEngine;

namespace TXDCL.XiuLian.MiShu
{
    [CreateAssetMenu(fileName = "MiShuData",menuName = "XiuLian/MiShu/MiShuData")]
    public class MiShuData : ScriptableObject
    {
        [Header("Basic Data")]
        public string Name;
        public Sprite Icon;
        [TextArea]
        public string Description;
    }
}

