using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WallJumper
{
    public class Game1 : Game
    {
        //Declarations
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D wallSprite;
        private Texture2D floorSprite;
        private Texture2D playerSprite;
        private SpriteFont font;
        private TimeSpan bulletSpawnTime;
        private TimeSpan previousBulletSpawnTime;
        //private List<Bullet> bullets;
        private List<Wall> currentWalls;
        private Player player;
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
            player = new Player();
            player.Initialize(new Vector2(ScreenWidth/2
                , ScreenHeight - 32));
            MousePOS = new Point(0,0);
            currentWalls = new List<Wall>(); 
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            floorSprite = Content.Load<Texture2D>("Floor");
            wallSprite = Content.Load<Texture2D>("Wall");
            playerSprite = Content.Load<Texture2D>("Player");
            //Player has to be init after sprite is loaded.
            player.PlayerSprite = playerSprite;

        }

        protected override void Update(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Jump
            if (keyState.IsKeyDown(Keys.Space))
            {
                player.Speed = new Vector2(0,-3);
                score++;
            } 
            player.Position += player.Speed;
            if(player.Position.Y >= 768)player.Speed = new Vector2(0,3);
            var currentMouse = Mouse.GetState();
            MousePOS = currentMouse.Position;
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
            //Score
            _spriteBatch.DrawString(font
            , $"Score: {score.ToString()}"
            , new Vector2(ScreenWidth - 120, 5)
            , Color.White); 
            //Floor only gets drawn once, then dies.
            _spriteBatch.Draw(floorSprite,new Vector2(0,794),Color.White);
            
            //Right Wall placeholder
            _spriteBatch.Draw(wallSprite,new Vector2(0, 0), Color.White);
            //Left Wall
            _spriteBatch.Draw(wallSprite,new Vector2(484,0),Color.White);
            //Player
            player.Draw(_spriteBatch);
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
