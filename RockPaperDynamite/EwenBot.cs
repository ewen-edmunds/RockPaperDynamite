using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BotInterface.Bot;
using BotInterface.Game;

namespace RockPaperDynamite
{
    
    //If anyone is reading this code, turn back now
    
    //you have been warned
    
    //seriously
    
    public class EwenBot : IBot
    {
        //Magic numbers
        public int DynamiteRemaining = 99;
        public int EnemyDynamiteRemaining = 100;
        public int RoundsSinceOpponentDynamite = 30;
        public int RoundCount = 0;
        public int DrawCount = 0;
        public int DrawCountStreak = 0;
        
        public Dictionary<Move, int> learning = new Dictionary<Move, int>(); 
        
        public Move MakeMove(Gamestate gamestate)
        {
            Random rng = new Random();
            RoundCount += 1;
            
            if (RoundCount == 1)
            {
                DynamiteRemaining -= 1;
                return Move.D;
            }
            
            //Tracking how much dynamite the opponent has remaining
            RoundsSinceOpponentDynamite -= 1;
            if (RoundCount > 1)
            {
                if (gamestate.GetRounds()[RoundCount - 2].GetP2() == Move.D)
                {
                    EnemyDynamiteRemaining -= 1;
                    RoundsSinceOpponentDynamite = 30;
                }
            }
            
            //Handling multiple draws in a row
            if (DrawLogic.WasDrawLastRound(gamestate, RoundCount))
            {
                DrawCountStreak += 1;
                if (DrawCountStreak == 2 && DynamiteRemaining > 75)
                {
                    DynamiteRemaining -= 1;
                    return Move.D;
                }
                if (DrawCountStreak == 3)
                {
                    return GetRandomMove();
                }
            }
            else
            {
                DrawCountStreak = 0;
            }

            //Work out what to do on a draw
            if (RoundCount >= 2 && DrawLogic.WasDrawLastRound(gamestate, RoundCount) && rng.Next(10) < 6)
            {
                DrawCount += 1;
                if (DrawCount > 10)
                {
                    //see if a significant response is likely
                    var myPreviousMove = gamestate.GetRounds()[RoundCount - 2].GetP1();
                    Move? significantResponse =
                        Learner.ReturnSignificantValue(Learner.LearnResponseTo(DrawLogic.GetAllDraws(gamestate), myPreviousMove, x => DrawLogic.IsDraw(x)));
                
                    if (significantResponse != null)
                    {
                        return GetOppositeMove((Move) significantResponse);
                    }
                }

                learning = Learner.LearnDraws(gamestate);
                int totalDraws = learning.Sum(x => x.Value);

                //Weighted response 
                int learnedRandom = rng.Next(totalDraws);
                foreach (KeyValuePair<Move, int> pair in learning)
                {
                    learnedRandom -= pair.Value;
                    if (learnedRandom <= 0)
                    {
                        return GetFinisherMove(pair.Key);
                    }
                }
            }

            //Start predicting what opponent does
            if (RoundCount >= 210 && rng.Next(10) > 4)
            {
                var myPreviousMove = gamestate.GetRounds()[RoundCount - 2].GetP1();
                
                //see if a significant response is likely
                Move? significantResponse =
                    Learner.ReturnSignificantValue(Learner.LearnResponseTo(gamestate.GetRounds(), myPreviousMove, x=> true));
                
                if (significantResponse != null)
                {
                    return GetOppositeMove((Move) significantResponse);
                }

                //otherwise perhaps pick a weighted random play
                int learnedRandom = rng.Next(RoundCount);

                learning = Learner.Learn(gamestate);
                foreach (KeyValuePair<Move, int> pair in learning)
                {
                    learnedRandom -= pair.Value;
                    if (learnedRandom <= 0)
                    {
                        return GetOppositeMove(pair.Key);
                    }
                }
            }

            return GetRandomMove();
        }


        public Move GetOppositeMove(Move enemyMove)
        {
            if (enemyMove == Move.S)
            {
                return Move.R;
            }
            else if (enemyMove == Move.R)
            {
                return Move.P;
            }
            else if (enemyMove == Move.P)
            {
                return Move.S;
            }

            return GetRandomMove();
        }
        
        public Move GetFinisherMove(Move enemyMove)
        {
            if (enemyMove == Move.D && EnemyDynamiteRemaining > 0 && RoundsSinceOpponentDynamite > 0)
            {
                return Move.W;
            }
            else if ((enemyMove == Move.S || enemyMove == Move.R || enemyMove == Move.P) && DynamiteRemaining > 0)
            {
                DynamiteRemaining -= 1;
                return Move.D;
            }
            else 

            return GetRandomMove();
        }

        public Move GetRandomMove()
        {
            Random rng = new Random();
            int randomNum = rng.Next(1000);
            
            if (randomNum < 8 && DynamiteRemaining > 0)
            {
                DynamiteRemaining -= 1;
                return Move.D;
            }

            if (RoundCount >= 800 && DynamiteRemaining > 0 && rng.Next(10) <= 5)
            {
                DynamiteRemaining -= 1;
                return Move.D;
            }
            
            randomNum = rng.Next(900);
            if (randomNum < 300)
            {
                return Move.P;
            }
            else if (randomNum < 600)
            {
                return Move.R;
            }
            return Move.S;
        }
    }
}