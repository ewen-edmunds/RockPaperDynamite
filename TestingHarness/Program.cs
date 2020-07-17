using System;
using System.Collections.Generic;
using System.Linq;
using BotInterface.Bot;
using BotInterface.Game;
using RockPaperDynamite;

namespace TestingHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            IBot bot1 = new EwenBot();
            IBot bot2 = new EwenBot();

            int score1 = 0;
            int score2 = 0;

            int roundNumber = 0;
            
            Gamestate gamestate = new Gamestate();

            while (score1 < 1000 && score2 < 1000)
            {
                //play round
                Move m1 = bot1.MakeMove(gamestate);
                Move m2 = bot2.MakeMove(gamestate);
                
                Console.WriteLine($"Round {roundNumber.ToString()}: Bot 1 Played: {m1.ToString()} \tAnd Bot 2 Played: {m2.ToString()}");

                Round newRound = new Round();
                newRound.SetP1(m1);
                newRound.SetP2(m2);

                var Rounds = gamestate.GetRounds();

                Round[] rds;
                if (Rounds != null)
                {
                    rds = Rounds.Append(newRound).ToArray();
                }
                else
                {
                    rds = new[] {newRound};
                }

                gamestate.SetRounds(rds);

                score1 += 1;
                roundNumber += 1;
            }
        }
    }
}