using System;
using TXDCL.Character;
using UnityEngine;

namespace TXDCL.Effect
{
    public abstract class EffectData : ScriptableObject
    {
        public Sprite effectIcon;
        protected float currentValue => value == 0 ? currentValue : value;
        public float value;
        public int round;
        [TextArea]
        public string description;
        public EffectDuration effectDuration;
        public EffectTarget effectTarget;
        protected CharacterBase from;
        protected CharacterBase target;

        private void OnEnable()
        {
            EventHandler.OnEffectCreateEvent += OnEffectCreate;
            EventHandler.OnEffectExecuteEvent += OnEffectExecute;
            EventHandler.OnEffectEndEvent += OnEffectEnd;
            EventHandler.CharacterTurnBeginEvent += OnCharacterTurnBegin;
        }
        private void OnDisable()
        {
            EventHandler.OnEffectCreateEvent -= OnEffectCreate;
            EventHandler.OnEffectExecuteEvent -= OnEffectExecute;
            EventHandler.OnEffectEndEvent -= OnEffectEnd;
            EventHandler.CharacterTurnBeginEvent -= OnCharacterTurnBegin;
        }
        public abstract void OnEffectCreate(CharacterBase from, CharacterBase target);
        public abstract void OnEffectExecute();
        public abstract void OnEffectEnd(CharacterBase currentTarget);
        protected abstract void OnCharacterTurnBegin(CharacterBase currentTarget);
        
        protected bool IsOnceEffect(EffectData effect)
        {
            return effect.effectDuration == EffectDuration.Once;
        }
    }
}
