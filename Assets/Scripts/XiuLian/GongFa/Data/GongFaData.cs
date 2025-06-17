using System.Collections.Generic;
using UnityEngine;

namespace TXDCL.XiuLian.GongFa
{
    [CreateAssetMenu(fileName = "GongFaData", menuName = "XiuLian/GongFa/GongFaData")]
    public class GongFaData : ScriptableObject
    {
        public string Name;
        public Sprite GongFaIcon;
        public MiniQuality MiniQuality;
        public QualityType QualityType;
        [TextArea]
        public string Description;
        
        [Header("Effects")] 
        public GongFaType GongFaType;
        public int BasicXiuLianSpeed;//基础道藏获取速度
        public float AdditionalXiuLianSpeed;//道藏获取比例
        public MiniJingjieLevel MiniJingjie;//小境界上限
        public JingjieLevel Jingjie;//大境界上限
        public ShuXing ShuXing;//功法属性
        public List<Property> PropertyList = new();
        //TODO：触发退化和进化条件需要写成泛型
        public GongFaData LowerGongFaData;//退化功法，如受到致命伤或神通影响导致功法降级
        public GongFaData UpperGongFaData;//进化功法，如集齐残页或获得大机缘导致功法进化
    }
}