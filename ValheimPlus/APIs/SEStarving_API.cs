using UnityEngine;

namespace ValheimPlus.APIs
{
    public class SE_Starving : StatusEffect
    {
        [Header("SE_Starving")]
        public HitData.DamageTypes m_damage;
        public float m_damageInterval = ValheimPlusPlugin.starvingsys_damage;
        private float m_timer;

        public override bool CanAdd(Character character) => !character.m_tolerateSmoke && base.CanAdd(character);

        public override void UpdateStatusEffect(float dt)
        {
            base.UpdateStatusEffect(dt);
            this.m_damage.m_poison = 5f;
            this.m_timer += dt;
            if ((double) this.m_timer <= (double) this.m_damageInterval)
                return;
            this.m_timer = 0.0f;
            this.m_character.ApplyDamage(new HitData()
            {
                m_point = this.m_character.GetCenterPoint(),
                m_damage = this.m_damage
            }, true, false);
        }
    }
}