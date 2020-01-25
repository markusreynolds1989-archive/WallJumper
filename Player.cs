using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public Texture2D PlayerSprite { get; set; }
        public int Width => PlayerSprite.Width;
        public int Height => PlayerSprite.Height;
        //Player lives so we can increase them and decrease them as we need.
        public int Lives { get; set; } = 3;
        
        public void Initialize(Vector2 position)
        {
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
