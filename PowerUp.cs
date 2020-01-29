using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class PowerUp
    {
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public Texture2D Sprite { get; set; }
        public int Width => Sprite.Width;
        public int Height => Sprite.Height;
        public bool Active = true;

        public void Initialize(Vector2 position,Texture2D sprite)
        {
            Position = position;
            Sprite = sprite;
        }

        public void Update(float gravity)
        {
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, Color.White);
        }
    }
}
