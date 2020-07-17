using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Game;

namespace RockPaperDynamite
{
    public static class Learner
    {
        public static Dictionary<Move, int> Learn(Gamestate state)
        {
            Dictionary<Move, int> newDict = new Dictionary<Move, int>();
            newDict.Add(Move.R, 0);
            newDict.Add(Move.P, 0);
            newDict.Add(Move.S, 0);
            newDict.Add(Move.D, 0);
            newDict.Add(Move.W, 0);
            
            foreach (var round in state.GetRounds())
            {
                newDict[round.GetP2()] = newDict[round.GetP2()] + 1;
            }

            return newDict;
        }
        
        public static Dictionary<Move, int> LearnDraws(Gamestate state)
        {
            Dictionary<Move, int> newDict = new Dictionary<Move, int>();
            newDict.Add(Move.R, 0);
            newDict.Add(Move.P, 0);
            newDict.Add(Move.S, 0);
            newDict.Add(Move.W, 0);
            newDict.Add(Move.D, 0);

            for (int i = 0; i < state.GetRounds().Length-1; i++)
            {
                var round = state.GetRounds()[i];
                if (round.GetP2() == round.GetP1())
                {
                    var learnFromRound = state.GetRounds()[i + 1];
                    newDict[learnFromRound.GetP2()] = newDict[learnFromRound.GetP2()] + 1;
                    
                    //We weight water being played after a draw with increased weight 
                    if (learnFromRound.GetP2() == Move.W)
                    {
                        newDict[learnFromRound.GetP2()] = newDict[learnFromRound.GetP2()] + 1;
                    }
                }
            }
            return newDict;
        }

        //returns a dict of pairs corresponding to the frequency of the opponent's responses to a specific move
            //also requires a condition to narrow down what rounds to consider (e.g. only draws)
        public static Dictionary<Move, int> LearnResponseTo(Round[] rounds, Move myMove, Func<Round, bool> roundCondition)
        {
            Dictionary<Move, int> newDict = new Dictionary<Move, int>();
            newDict.Add(Move.R, 0);
            newDict.Add(Move.P, 0);
            newDict.Add(Move.S, 0);
            newDict.Add(Move.D, 0);
            newDict.Add(Move.W, 0);
            
            for (int i = 0; i < rounds.Length-1; i++)
            {
                var round = rounds[i];
                if (round.GetP1() == myMove && roundCondition(round))
                {
                    var learnFromRound = rounds[i + 1];
                    newDict[learnFromRound.GetP2()] = newDict[learnFromRound.GetP2()] + 1;
                }
            }
            return newDict;
        }

        public static Move? ReturnSignificantValue(Dictionary<Move, int> responses)
        {
            int totalResponses = responses.Sum(x => x.Value);

            foreach (KeyValuePair<Move, int> pair in responses)
            {
                if (pair.Value > (totalResponses *  0.80f))
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}