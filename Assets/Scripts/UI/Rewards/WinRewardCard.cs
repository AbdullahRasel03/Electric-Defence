using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinRewardCard : RewardCard
{
    [SerializeField] private ParticleSystem sparkleParticle;
    private int finalAmount;
    private ECONOMY_TYPE economyType;

    public int FinalAmount => finalAmount;
    public ECONOMY_TYPE EconomyType => economyType;

    public override void SetProperties(string rewardType, int amount, Sprite icon, ECONOMY_TYPE economyType)
    {
        base.SetProperties(rewardType, amount, icon, economyType);
        this.finalAmount = amount;

        if (economyType == ECONOMY_TYPE.COIN)
        {
            sparkleParticle.Play();
        }
        else
        {
            sparkleParticle.Stop();
        }
    }

}
