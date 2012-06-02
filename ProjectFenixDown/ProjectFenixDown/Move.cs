using System;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    /// <summary>
    /// Descibes a sequences of buttons which must be pressed to activate the move.
    /// a real game might add a virtual PerformMove() method to this class
    /// </summary>
    public class Move
    {
        public string name;

        //the sequence of button presses required to activate this move
        public Buttons[] comboSequence;

        //set this to true if the input used to activate this move may be reused as a component of longer moves
        public bool IsSubMove;

        public Move(string nameInput, params Buttons[] comboSequenceInput)
        {
            name = nameInput;
            comboSequence = comboSequenceInput;
        }
    }
}
