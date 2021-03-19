using UnityEngine;

namespace ValheimPlus.APIs
{
    public class Player_API
    {
        public Player original;

        public Player_API(ref Player instance) => original = instance;

        private SEMan m_seman => this.original.GetSEMan();

        public bool addStatusEffect(string name, bool resetTime = false) => (bool) (Object) this.m_seman.AddStatusEffect(name, resetTime);

        public bool removeStatusEffect(string name, bool quiet = false) => this.m_seman.RemoveStatusEffect(name, quiet);
    }
}