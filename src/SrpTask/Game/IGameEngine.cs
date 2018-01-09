using System.Collections.Generic;

namespace SrpTask.Game
{
    public interface IGameEngine
    {
        List<IEnemy> GetEnemiesNear(RpgPlayer player);

        void PlaySpecialEffect(string effectName);
    }
}