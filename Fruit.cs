using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class Fruit
    {
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public Texture2D FruitSprite { get; set; }
        public int Width => FruitSprite.Width;
        public int Height => FruitSprite.Height;
        public bool Active = true;

        public void Initialize(Vector2 position,Texture2D texture)
        {
            Position = position;
            FruitSprite = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(FruitSprite, Position, Color.White);
        }
    }
}
