using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectFenixDown
{
    public enum TileCollision
    {
        //a passable tile which does not hinder player motion at all
        passable = 0,

        //an impassable tile which does not allow player to move through it at all. completely solid
        impassable = 1,

        //a platform tile which behaves like a passable tile except when the player is above it.
        platform = 2,
    }

    struct Tile
    {
        public Texture2D texture;
        public TileCollision collision;

        public const int width = 50;
        public const int height = 50;

        public static readonly Vector2 size = new Vector2(width, height);

        //constructs a new tile
        public Tile(Texture2D textureInput, TileCollision collisionInput)
        {
            texture = textureInput;
            collision = collisionInput;
        }
    }
}
