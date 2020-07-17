using System.Collections.Generic;
using System.Linq;
using BotInterface.Game;

namespace RockPaperDynamite
{
    public static class DrawLogic
    {
        
        public static bool WasDrawLastRound(Gamestate state, int roundCount)
        {
            return state.GetRounds()[roundCount - 2].GetP1() == state.GetRounds()[roundCount - 2].GetP2();
        }

        public static Round[] GetAllDraws(Gamestate state)
        {
            return state.GetRounds().Where(x => x.GetP1() == x.GetP2()).ToArray();
        }

        public static Round GetLastDraw(Gamestate state)
        {
            foreach (var round in state.GetRounds().Reverse())
            {
                if (round.GetP1() == round.GetP2())
                {
                    return round;
                }
            }

            return null;
        }

        public static bool IsDraw(Round round)
        {
            return round.GetP1() == round.GetP2();
        }
    }
}