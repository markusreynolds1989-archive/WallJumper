using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;

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
        private Texture2D apple;
        private Texture2D cherries;
        private Texture2D rock;
        private Floor floor;
        private SpriteFont font;
        private TimeSpan rockSpawnTime;
        private TimeSpan previousRocktSpawnTime;
        private TimeSpan fruitSpawnTime;
        private TimeSpan previousFruitSpawnTime;

        private double gravity = 0.4;

        private Wall leftWall;
        private Wall rightWall;
        private Player player;
        private bool gameOver = false;
        private bool win = false;
        private int score = 0;
        private int level = 1;
        private List<Fruit> fruits;
        private List<Rock> rocks;
        private const int ScreenWidth = 400;
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
            rockSpawnTime = TimeSpan.FromSeconds(0.5f);
            fruitSpawnTime = TimeSpan.FromSeconds(0.5f);
            fruits = new List<Fruit>();
            rocks = new List<Rock>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            wallSprite = Content.Load<Texture2D>("Wall");
            playerSprite = Content.Load<Texture2D>("Player");
            cherries = Content.Load<Texture2D>("Cherries");
            //Player has to be init after sprite is loaded.
            player.PlayerSprite = playerSprite;
            InitWalls();
        }

        protected override void Update(GameTime gameTime)
        {
            EventHandler(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            _spriteBatch.Draw(wallSprite, leftWall.Position, Color.White);
            _spriteBatch.Draw(wallSprite, rightWall.Position, Color.White);
            //Score
            _spriteBatch.DrawString(font
                , $"Score: {score.ToString()}" +
                  $"\nLevel: {level.ToString()}"
                , new Vector2(ScreenWidth - 120, 5)
                , Color.White);
            //Floor only gets drawn once, then dies.
            if (floor.Active)
            {
                _spriteBatch.Draw(floorSprite, floor.Position, Color.White);
            }

            foreach (var fruit in fruits)
            {
                _spriteBatch.Draw(cherries,fruit.Position,Color.White);
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
            rightWall.Initialize(wallSprite, new Vector2(384
                , 0));
        }

        private void EventHandler(GameTime gameTime)
        {
            //Check if game is still active before updating.
            if (gameOver) return;

            //Current key state
            var keyState = Keyboard.GetState();

            //Collision Checks
            var collisionRightWall = Collision.RectangleCollision(
                player.Position
                , player.Width
                , player.Height
                , rightWall.Position
                , rightWall.Width
                , rightWall.Height);
            var collisionLeftWall = Collision.RectangleCollision(
                player.Position
                , player.Width
                , player.Height
                , leftWall.Position
                , leftWall.Width
                , leftWall.Height);
            var collisionFloor = Collision.RectangleCollision(
                player.Position
                , player.Width
                , player.Height
                , floor.Position
                , floor.Width
                , floor.Height);
            
            //Fruits
            foreach (var fruit in fruits)
            {
                if (Collision.RectangleCollision(
                    player.Position
                    , player.Width
                    , player.Height
                    , fruit.Position
                    , fruit.Width
                    , fruit.Height))
                {
                    fruit.Active = false;
                }
            }
            //Rocks
            
            //Controls 
            //Right
            if (keyState.IsKeyDown(Keys.Right)
                && !collisionRightWall)
            {
                player.Position += new Vector2(7, 0);
            }

            //Left
            if (keyState.IsKeyDown(Keys.Left)
                && !collisionLeftWall)
            {
                player.Position += new Vector2(-7, 0);
            }

            //Left wall Jump
            if (keyState.IsKeyDown(Keys.Right)
                && keyState.IsKeyDown(Keys.Space)
                && collisionLeftWall)
            {
                player.Speed = new Vector2(0, -20);
                player.Position += player.Speed;
            }

            //Right wall Jump
            if (keyState.IsKeyDown(Keys.Left)
                && keyState.IsKeyDown(Keys.Space)
                && collisionRightWall)
            {
                player.Speed = new Vector2(0, -20);
                player.Position += player.Speed;
            }

            //Floor Jump
            if (keyState.IsKeyDown(Keys.Space)
                && floor.Active
                && collisionFloor)
            {
                player.Speed = new Vector2(0, -20);
                player.Position += player.Speed;
            }

            //Update Position 

            //Fall Down
            if (player.Position.Y != 768)
            {
                player.Speed += new Vector2(0, (float) gravity);
                player.Position += player.Speed;
            }

            //When the player gets to the top. reset their position to the bottom of the screen and kill the floor.
            if (player.Position.Y <= 0)
            {
                player.Position = new Vector2(player.Position.X
                    , 700);
                score += 10;
                level++;
                floor.Active = false;
            }

            //When the floor is active the player won't fall down. 
            if (floor.Active
                && player.Position.Y >= floor.Position.Y - floor.Height -
                player.Height / 2 - 2)
            {
                player.Speed = new Vector2(0, 0);
                player.Position = new Vector2(player.Position.X, 768);
            }

            //When the player drops off, game is over.
            if (!floor.Active
                && player.Position.Y >= ScreenHeight)
            {
                gameOver = true;
            }
            
            CreateFruit(gameTime);
            UpdateFruit();
        }

        public void CreateFruit(GameTime gameTime)
        {
            var random = new Random();
            if (gameTime.TotalGameTime - previousFruitSpawnTime >
                fruitSpawnTime)
            {
                previousFruitSpawnTime = gameTime.TotalGameTime;
                var fruit = new Fruit();
                var x = random.Next(10, 360);
                var y = random.Next(20, 600);
                fruit.Initialize(new Vector2(x,y),cherries);
                fruits.Add(fruit);
            }
        }

        public void UpdateFruit()
        {
            for (var i = 0; i < fruits.Count; i++)
            {
                if (!fruits[i].Active)
                {
                    fruits.Remove(fruits[i]);
                }
            }
        }
    }
}