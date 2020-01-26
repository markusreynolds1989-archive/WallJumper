using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

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
        private Floor floor;
        private SpriteFont font;
        private TimeSpan bulletSpawnTime;
        private TimeSpan previousBulletSpawnTime;
        //private List<Bullet> bullets;
        private Wall leftWall;
        private Wall rightWall;
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
            floorSprite = Content.Load<Texture2D>("Floor");
            player = new Player();
            player.Initialize(new Vector2(ScreenWidth/2
                , ScreenHeight - 32));
            floor = new Floor();
            floor.Initialize(floorSprite,new Vector2(0,794));
            MousePOS = new Point(0,0);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            wallSprite = Content.Load<Texture2D>("Wall");
            playerSprite = Content.Load<Texture2D>("Player");
            //Player has to be init after sprite is loaded.
            player.PlayerSprite = playerSprite;
            InitWalls();
        }

        protected override void Update(GameTime gameTime)
        {
            //Check if game is still active before updating.
            if (gameOver == true) return;
            
            var keyState = Keyboard.GetState();
            //Quit options 
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
                    
            //Jump
            //Collision is really messed up.
            if (keyState.IsKeyDown(Keys.Space)
                && floor.Active 
                && player.Position.Y == floor.Height - floor.Position.Y - (player.Height/2))         
                
            {
                player.Speed -= new Vector2(0, 20);
                player.Position += player.Speed;
            }

            //Left and Right
            if (keyState.IsKeyDown(Keys.Right)) player.Position += new Vector2(1, 0);
            if (keyState.IsKeyDown(Keys.Left)) player.Position -= new Vector2(1, 0);
            
            //Gravity
            if (player.Speed != new Vector2(0, 0)
                && player.Position != new Vector2(player.Position.X, 0))
            {
                //Decrease the player speed until it's back to 0,0.
                player.Speed -= new Vector2(0, 1);
                player.Position += new Vector2(0,5);
            }

            //When the floor is active the player won't fall down. 
            if (floor.Active == true
                && player.Position.Y >= floor.Position.Y - floor.Height - player.Height/2)
            {
                player.Speed = new Vector2(0, 0);
                player.Position = new Vector2(player.Position.X
                    , floor.Position.Y - floor.Height - player.Height/2);
            }

            //When the player gets to the top. reset their position to the bottom of the screen and kill the floor.
            if (player.Position.Y <= 0)
            {
                player.Position = new Vector2(player.Position.X
                    , 794);
                score += 10;
                floor.Active = false;
            }
            
            //When the player drops off, game is over.
            if (player.Position.Y >= ScreenHeight)
            {
                gameOver = true;
            }

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
            , new Vector2(20, 5)
            , Color.White);
            _spriteBatch.DrawString(font
            , $"PlayerX: {player.Position.X.ToString()} " +
              $"PlayerY: {player.Position.Y.ToString()}"
            , new Vector2(20,30)
            ,Color.White);
            _spriteBatch.Draw(wallSprite,leftWall.Position,Color.White);
            _spriteBatch.Draw(wallSprite, rightWall.Position, Color.White); 
            //Score
            _spriteBatch.DrawString(font
            , $"Score: {score.ToString()}"
            , new Vector2(ScreenWidth - 120, 5)
            , Color.White); 
            //Floor only gets drawn once, then dies.
            if (floor.Active == true)
            {
                _spriteBatch.Draw(floorSprite, floor.Position, Color.White);
            }
            //Player
            player.Draw(_spriteBatch);
            //Game Over
            if (gameOver == true)
            {
                _spriteBatch.DrawString(font
                , $"GAME OVER\nScore {score.ToString()}"
                , new Vector2(ScreenWidth/2, ScreenHeight/2)
                , Color.White);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void InitWalls()
        {
           leftWall = new Wall();
           rightWall = new Wall();
           leftWall.Initialize(wallSprite, new Vector2(0
           ,0));
           rightWall.Initialize(wallSprite,new Vector2(490
           ,0));
        }
    }
}
