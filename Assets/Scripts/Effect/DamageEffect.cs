using System;
using TXDCL.Character;
using UnityEngine;

namespace TXDCL.Effect
{
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "Effects/DamageEffect")]
    public class DamageEffect : EffectData
    {
        public float DamageModifier;
        public override void OnEffectCreate(CharacterBase f, CharacterBase t)
        {
            from = f;
            target = t;
            switch (effectDuration)
            {
                case EffectDuration.Once:
                    OnEffectExecute();
                    break;
                case EffectDuration.Sustainable:
                case EffectDuration.Permanent:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnEffectExecute()
        {
            if(target == null) return;
            target.TakeDamage(from.CharacterData, target.CharacterData,
                (int)(currentValue + from.CharacterData.Attack * DamageModifier));
        }

        public override void OnEffectEnd(CharacterBase currentTarget)
        {
            
        }

        protected override void OnCharacterTurnBegin(CharacterBase currentTarget)
        {
            
        }
    }
}
