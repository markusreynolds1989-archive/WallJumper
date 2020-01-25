using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WallJumper
{
    public class Floor
    {
        private Vector2 Position { get; set; }
        private Texture2D FloorSprite { get; set; }
        public bool Active { get; set; } = true;
        public int Width => FloorSprite.Width;
        public int Height => FloorSprite.Height;
        //Player lives so we can increase them and decrease them as we need.
        
        public void Initialize(Texture2D sprite
            , Vector2 position)
        {
            FloorSprite = sprite;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(FloorSprite,Position,Color.White);
        }
    }
}
