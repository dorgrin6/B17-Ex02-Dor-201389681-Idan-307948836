﻿using System;

namespace Program
{
    class GameLogic
    {
        internal const short k_ValueNotExists = -1; // signals value does'nt exist

        internal const ushort k_GuessArraySize = 4; // guesses array size
        
        private ushort m_UserGuessesAmount; // holds amount of guesses wanted by user
 
        private short[] m_GameGoal; //GameGoal: index i holds the offset of the letter 'A'+i in current raffle if it exists, and ValueNotExists otherwise

        private BoardLine[] m_Board; // game board      

        internal enum eGuessAmountBounds : ushort
        {
            MinGuessNum = 4,
            MaxGuessNum = 10
        }

        internal enum eGuessLetterBounds : ushort
        {
            MinGuessLetter = 'A',
            MaxGuessLetter = 'H'
        }

        internal enum eBoardPadding : ushort
        {
            SecretCoding = '#',
            EmptySpace = ' '
        }

        public ushort UserGuessesAmount
        {
            get
            {
                return m_UserGuessesAmount;
            }
            set
            {
                this.m_UserGuessesAmount = value;
            }
        }

        public ushort GuessArraySize
        {
            get
            {
                return k_GuessArraySize;
            }
        }

        public BoardLine[] Board
        {
            get
            {
                return m_Board;
            }
        }

        public BoardLine this[int i_index]
        {
            get
            {
                return m_Board[i_index];
            }
        }

        public void InitiateGame()
        {
            m_Board = new BoardLine[m_UserGuessesAmount + 1];
            m_Board[0] = new BoardLine(k_GuessArraySize, (char)eBoardPadding.SecretCoding);

            for (int i = 1; i < m_Board.Length; i++)
            {
                m_Board[i] = new BoardLine(k_GuessArraySize, (char)eBoardPadding.EmptySpace);
            }

            createGameGoalValues();
        }

        public void SetExistingValuesInGuess(int i_BoardIndex)
        {
            ushort rightPlaceCount;
            ushort wrongPlaceCount;

            // count right place and wrong place guesses
            this.countExiststingValuesInGuess(this.Board[i_BoardIndex].UserGuess, out rightPlaceCount, out wrongPlaceCount);

            // set results
            Board[i_BoardIndex].ExistRightPlaceResult = rightPlaceCount;
            Board[i_BoardIndex].ExistWrongPlaceResult = wrongPlaceCount;
        }

        public bool IsLetterLegal(int i_LetterIndex, string i_Letters)
        {
            return isLetterInBounds(i_Letters[i_LetterIndex]);
        }

        public bool HasDuplicateLetters(string i_Input)
        {
            bool result = false;
            char[] splittedString = i_Input.Replace(" ", string.Empty).ToCharArray();
            short valuesAmount = eGuessLetterBounds.MaxGuessLetter - eGuessLetterBounds.MinGuessLetter + 1;
            short[] currentGuess = new short[valuesAmount]; // similar to GameGoal

            // init currentGuess
            for (int i = 0; i < currentGuess.Length; i++)
            {
                currentGuess[i] = k_ValueNotExists;
            }

            for (int i = 0; i < splittedString.Length; i++)
            {
                short currentOffset = (short)(splittedString[i] - GameLogic.eGuessLetterBounds.MinGuessLetter);

                // found an element that already existed
                if (currentGuess[currentOffset] != k_ValueNotExists)
                {
                    result = true;
                    break;
                }
                else
                {
                    currentGuess[currentOffset] = (short)i;
                }
            }

            return result;
        }

        public bool IsWinningGuess(int i_BoardIndex)
        {
            return (Board[i_BoardIndex].ExistRightPlaceResult == (ushort)k_GuessArraySize);
        }

        private void initGameGoalValues()
        {
            // amount of values in game
            ushort valuesAmount =
                eGuessLetterBounds.MaxGuessLetter - eGuessLetterBounds.MinGuessLetter + 1;
            m_GameGoal = new short[valuesAmount];

            for (int i = 0; i < valuesAmount; i++)
            {
                m_GameGoal[i] = k_ValueNotExists;
            }
        }

        private void countExiststingValuesInGuess(char[] i_UserGuess, out ushort i_CountRightPlace, out ushort i_CountWrongPlace)
        {
            i_CountRightPlace = 0;
            i_CountWrongPlace = 0;

            for (int i = 0; i < i_UserGuess.Length; i++)
            {
                char currentLetter = i_UserGuess[i];
                ushort currentOffset = 
                    (ushort)(currentLetter - eGuessLetterBounds.MinGuessLetter); // offset from borad start

                if (m_GameGoal[currentOffset] == i)
                {
                    ++i_CountRightPlace;
                }
                else if (this.m_GameGoal[currentOffset] != k_ValueNotExists)
                {
                    ++i_CountWrongPlace;
                }
            }
        }

        private void createGameGoalValues()
        {
            Random randInt = new Random();
            int hashIndex; // hash index to insert to
            int minBound = 0;
            int maxBound = (int)(eGuessLetterBounds.MaxGuessLetter - eGuessLetterBounds.MinGuessLetter + 1);

            this.initGameGoalValues();
            for (int i = 0; i < k_GuessArraySize; i++)
            {
                do
                {
                    hashIndex = randInt.Next(minBound, maxBound);
                }
                while (m_GameGoal[hashIndex] != k_ValueNotExists);
                m_GameGoal[hashIndex] = (short)i;
            }
        }

        private bool isLetterInBounds(char i_Letter)
        {
            return (int)i_Letter >= (int)eGuessLetterBounds.MinGuessLetter && 
                (int)i_Letter <= (int)eGuessLetterBounds.MaxGuessLetter;
        }
    }
}
