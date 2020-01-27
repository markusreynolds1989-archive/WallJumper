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
        private TimeSpan jumpTime;
        private TimeSpan previousJumpTime;

        private float gravity = 3;

        //private List<Bullet> bullets;
        private Wall leftWall;
        private Wall rightWall;
        private Player player;
        private bool gameOver = false;
        private bool win = false;
        private int score = 0;
        private const int ScreenWidth = 500;
        private const int ScreenHeight = 800;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ScreenWidth,
                PreferredBackBufferHeight = ScreenHeight
            };
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            floorSprite = Content.Load<Texture2D>("Floor");
            player = new Player();
            player.Initialize(new Vector2(ScreenWidth / 2
                , ScreenHeight - 32));
            floor = new Floor();
            floor.Initialize(floorSprite, new Vector2(0, 794));
            jumpTime = TimeSpan.FromSeconds(0.1f);
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
            EventHandler(gameTime);
            UpdateGravity();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.DrawString(font
                , $"PlayerX: {player.Position.X.ToString()} " +
                  $"PlayerY: {player.Position.Y.ToString()}"
                , new Vector2(20, 30)
                , Color.White);
            _spriteBatch.Draw(wallSprite, leftWall.Position, Color.White);
            _spriteBatch.Draw(wallSprite, rightWall.Position, Color.White);
            //Score
            _spriteBatch.DrawString(font
                , $"Score: {score.ToString()}"
                , new Vector2(ScreenWidth - 120, 5)
                , Color.White);
            //Floor only gets drawn once, then dies.
            if (floor.Active)
            {
                _spriteBatch.Draw(floorSprite, floor.Position, Color.White);
            }

            //Player
            player.Draw(_spriteBatch);
            //Game Over
            if (gameOver)
            {
                _spriteBatch.DrawString(font
                    , $"GAME OVER\nScore {score.ToString()}"
                    , new Vector2(ScreenWidth / 2, ScreenHeight / 2)
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
                , 0));
            rightWall.Initialize(wallSprite, new Vector2(485
                , 0));
        }

        private void EventHandler(GameTime gameTime)
        {
            //Check if game is still active before updating.
            if (gameOver) return;

            //Current key state
            var keyState = Keyboard.GetState();

            //Jump from floor
            if (keyState.IsKeyDown(Keys.Space)
                && floor.Active
                && player.Position.Y == 768)
            {
                player.Speed -= new Vector2(0, 10);
                player.Position += player.Speed;
            }

            //Right
            if (keyState.IsKeyDown(Keys.Right)
                && player.Position.X + player.Width / 2 <=
                rightWall.Position.X -
                rightWall.Width + 5)
            {
                player.Position += new Vector2(5, 0);
            }

            //Left 
            if (keyState.IsKeyDown(Keys.Left)
                && player.Position.X + player.Width / 2 >=
                leftWall.Position.X +
                leftWall.Width + 15)
            {
                player.Position -= new Vector2(5, 0);
            }

            //right slide 
            if (keyState.IsKeyDown(Keys.Right)
                && player.Position.X + player.Width / 2 ==
                rightWall.Position.X -
                rightWall.Width + 7)
            {
                //TODO: Reset gravity when away from wall.
                gravity = 0.5f;
            }

            //left slide
            if (keyState.IsKeyDown(Keys.Left)
                && player.Position.X + player.Width / 2 ==
                leftWall.Position.X +
                leftWall.Width + 15)
            {
                //TODO: Reset gravity when away from wall.
                gravity = 0.5f;
            }

            //Right jump
            if (keyState.IsKeyDown(Keys.Left)
                && (keyState.IsKeyDown(Keys.Space))
                && player.Position.X + player.Width / 2 ==
                rightWall.Position.X -
                rightWall.Width + 7) 
            {
                player.Speed -= new Vector2(0,10);
            }

            //Left jump 
            if (keyState.IsKeyDown(Keys.Right)
                && (keyState.IsKeyDown(Keys.Space))
                && player.Position.X + player.Width / 2 ==
                leftWall.Position.X +
                leftWall.Width + 15) 
            {
                player.Speed -= new Vector2(0,10);
                player.Position -= player.Speed;
            }

        }

        private void UpdateGravity()
        {
            //Gravity, the player falls down until they reach screen size.
            if (player.Speed != new Vector2(0, 0)
                && player.Position.Y != 768)
            {
                player.Position += new Vector2(0, gravity);
            }

            //When the floor is active the player won't fall down. 
            if (floor.Active
                && player.Position.Y >= floor.Position.Y - floor.Height -
                player.Height / 2 - 2)
            {
                player.Speed = new Vector2(0, 0);
                player.Position = new Vector2(player.Position.X, 768);
            }

            //When the player gets to the top. reset their position to the bottom of the screen and kill the floor.
            if (player.Position.Y <= 0)
            {
                player.Position = new Vector2(player.Position.X
                    , 700);
                score += 10;
                floor.Active = false;
            }

            //When the player drops off, game is over.
            if (!floor.Active
                && player.Position.Y >= ScreenHeight)
            {
                gameOver = true;
            }
        }
    }
}