using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class Rock
    {
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public Texture2D RockSprite { get; set; }
        public int Width => RockSprite.Width;
        public int Height => RockSprite.Height;
        public bool Active = true;

        public void Initialize(Vector2 position)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(RockSprite, Position, Color.White);
        }
    }
}
