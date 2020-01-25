using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace WallJumper
{
    public class Game1 : Game
    {
        //Declarations
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D wall;
        private Texture2D floor;
        private Texture2D player;
        private SpriteFont font;
        private TimeSpan bulletSpawnTime;
        private TimeSpan previousBulletSpawnTime;
        //private List<Bullet> bullets;
        private bool gameOver = false;
        private bool win = false;
        private int score = 0;
        private const int ScreenWidth = 500;

        private const int ScreenHeight = 800;
        //Debug
        private Point MousePOS; 
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ScreenWidth, PreferredBackBufferHeight = ScreenHeight
            };
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            MousePOS = new Point(0,0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            floor = Content.Load<Texture2D>("Floor");
            wall = Content.Load<Texture2D>("Wall");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var currentMouse = Mouse.GetState();
            MousePOS = currentMouse.Position;
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            //Debug
            _spriteBatch.DrawString(font
            , $"Mouse X:{MousePOS.X.ToString()} Mouse Y:{MousePOS.Y.ToString()}"
            , new Vector2(5, 5)
            , Color.White);
            var scale = 5;
            //Sprites
            _spriteBatch.Draw(floor,new Vector2(0,794),Color.White);
            //Right Wall
            _spriteBatch.Draw(wall,new Vector2(0, 200), Color.White);
            //Left Wall
            _spriteBatch.Draw(wall,new Vector2(490,200),Color.White);
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
