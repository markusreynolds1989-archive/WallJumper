using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class Wall
    {
        private Vector2 Position { get; set; }
        private Texture2D WallSprite { get; set; }
        public bool Active { get; set; } = true;
        public int Width => WallSprite.Width;
        public int Height => WallSprite.Height;
        //Player lives so we can increase them and decrease them as we need.
        
        public void Initialize(Texture2D sprite
            , Vector2 position)
        {
            WallSprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(WallSprite,Position,Color.White);
        }
    }
}
