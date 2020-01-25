using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class Player
    {
        private Vector2 Position { get; set; }
        private Vector2 Speed { get; set; }
        private Texture2D PlayerSprite { get; set; }
        public int Width => PlayerSprite.Width;
        public int Height => PlayerSprite.Height;
        //Player lives so we can increase them and decrease them as we need.
        public int Lives { get; set; } = 3;
        
        protected void Initialize(Texture2D sprite
            , Vector2 position)
        {
            PlayerSprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime
            , Vector2 position
            , Vector2 speed)
        {
            Position = position;
            Speed = speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerSprite,Position,Color.White);
        }
    }
}
