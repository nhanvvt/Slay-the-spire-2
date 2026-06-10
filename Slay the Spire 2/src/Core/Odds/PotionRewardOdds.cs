using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Odds;

public class PotionRewardOdds : AbstractOdds
{
	public const float targetOdds = 0.5f;

	public const float eliteBonus = 0.25f;

	private const float _basePotionRewardOdds = 0.4f;

	public PotionRewardOdds(Rng rng)
		: base(0.4f, rng)
	{
	}

	public PotionRewardOdds(float initialValue, Rng rng)
		: base(initialValue, rng)
	{
	}

	public bool Roll(Player player, AscensionManager ascensionManager, RoomType roomType)
	{
		float currentValue = base.CurrentValue;
		bool flag = Hook.ShouldForcePotionReward(player.RunState, player, roomType);
		float num = ((roomType != RoomType.Elite) ? 0f : 0.25f);
		float num2 = num;
		float num3 = currentValue + num2 * 0.5f;
		float num4 = _rng.NextFloat();
		if (flag || num4 < num3)
		{
			base.CurrentValue -= 0.1f;
			return true;
		}
		base.CurrentValue += 0.1f;
		return false;
	}
}
