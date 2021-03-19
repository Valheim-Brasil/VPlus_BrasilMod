using ValheimPlus.APIs;
using UnityEngine;

namespace ValheimPlus.APIs
{
  public class StarvationSystem
  {
    private Player player;
    private bool isStarving = false;
    private bool effectActive = false;
    private float interval = 30f;
    private float damage = ValheimPlusPlugin.starvingsys_damage;
    private float timer;
    private float deltaTime = Time.deltaTime;

    public void setPlayerInstance(ref Player playerInstance) => this.player = playerInstance;

    public bool checkUpdate() => (Object) this.player != (Object) null && !this.player.IsDead();

    public void update()
    {
      if ((double) this.timer >= (double) this.interval)
        this.damageTick();
      else
        this.timer += this.deltaTime;
    }

    private void effectTick()
    {
      if (!this.isStarving && this.effectActive)
      {
        GLOBAL.playerApi.removeStatusEffect(StatusEffectList.Starving);
        this.effectActive = false;
      }
      else
      {
        if (!this.isStarving || this.effectActive)
          return;
        GLOBAL.playerApi.addStatusEffect(StatusEffectList.Starving);
        this.effectActive = true;
      }
    }

    private void damageTick()
    {
      if (this.getPlayerFood() == 0)
      {
        this.isStarving = true;
        this.damagePlayer();
      }
      else
        this.isStarving = false;
      this.timer = 0.0f;
    }

    private int getPlayerFood() => this.player.GetFoods().Count;

    private void damagePlayer() => this.player.ApplyDamage(new HitData()
    {
      m_damage = {
        m_damage = this.damage
      }
    }, true, true);
  }
}