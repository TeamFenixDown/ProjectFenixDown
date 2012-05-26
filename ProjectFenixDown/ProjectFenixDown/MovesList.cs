using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectFenixDown
{
    /// <summary>
    /// Represents a set of available moves for matching.
    /// This internal storage of this class is optimized for efficient match searches.
    /// </summary>
    class MovesList
    {
        private Move[] movesList;

        public MovesList(IEnumerable<Move> movesList)
        {
            //store the list of moves in order of decreasing sequence length.
            //this greatly simplifies the logic of the DetectMove method
            this.movesList = movesList.OrderByDescending(m => m.comboSequence.Length).ToArray();
        }

        ///<summary>
        /// Finds the longest Move which matches the given input, if any.
        /// </summary>
        public Move DetectMove(InputManager input)
        {
            //perform a linear search for a move which matches the input. 
            //this relies on the move array being in order of decreasing sequence length
            foreach (Move move in movesList)
            {
                if (input.SequenceMatchesMove(move))
                {
                    return move;
                }
            }
            return null;
        }

        public int LongestMoveLength
        {
            get
            {
                //since they are in decreasing order,
                //the first move is the longest
                return movesList[0].comboSequence.Length;
            }
        }
    }
}
