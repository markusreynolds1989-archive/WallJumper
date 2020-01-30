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
        private Texture2D rockSprite;
        private Texture2D gravityUpSprite;
        private Texture2D gravityDownSprite;
        private Floor floor;
        private SpriteFont font;
        private TimeSpan rockSpawnTime;
        private TimeSpan previousRockSpawnTime;
        private TimeSpan fruitSpawnTime;
        private TimeSpan previousFruitSpawnTime;
        private TimeSpan powerUpSpawnTime;
        private TimeSpan previousPowerUpSpawnTime;
        private bool tutorial = true;
        private int rockMod = 1;
        private float gravity = 0.1f;
        private Wall leftWall;
        private Wall rightWall;
        private Player player;
        private bool gameOver = false;
        private bool win = false;
        private int score = 0;
        private int level = 1;
        private List<Fruit> fruits;
        private List<Rock> rocks;
        private List<PowerUp> powerUps;
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
            rockSpawnTime = TimeSpan.FromSeconds(1f);
            fruitSpawnTime = TimeSpan.FromSeconds(3f);
            powerUpSpawnTime = TimeSpan.FromSeconds(10f);
            fruits = new List<Fruit>();
            rocks = new List<Rock>();
            powerUps = new List<PowerUp>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            wallSprite = Content.Load<Texture2D>("Wall");
            playerSprite = Content.Load<Texture2D>("Player");
            cherries = Content.Load<Texture2D>("Cherries");
            rockSprite = Content.Load<Texture2D>("Rock");
            gravityUpSprite = Content.Load<Texture2D>("GravityUp");
            gravityDownSprite = Content.Load<Texture2D>("GravityDown");
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
            if (tutorial)
            {
                _spriteBatch.DrawString(font
                , "Hold space bar to jump.\nKeep it held and then jump off\n" +
                  "walls by pressing away from \nthem with the arrow keys.\nCollect" +
                  "fruit watch out for falling rocks.\n" +
                  "Press R to reset."
                , new Vector2(15,600)
                ,Color.White );
            }
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
                _spriteBatch.Draw(cherries, fruit.Position, Color.White);
            }

            foreach (var rock in rocks)
            {
                _spriteBatch.Draw(rockSprite,rock.Position,Color.White);
            }

            foreach (var powerUp in powerUps)
            {
                powerUp.Draw(_spriteBatch);
            }
            
            //Player
            player.Draw(_spriteBatch);
            //Game Over
            if (gameOver)
            {
                _spriteBatch.DrawString(font
                    , $"GAME OVER\nScore {score.ToString()}"
                    , new Vector2((ScreenWidth / 3), ScreenHeight / 2)
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
           
            //Current key state
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.R))
            {
                gameOver = false;
                Initialize();
                level = 1;
                score = 0;
                gravity = 0.1f;
                tutorial = true;
            }

            if (gameOver) return;
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
                    score += 50;
                }
            }
            //Rocks
            foreach (var rock in rocks)
            {
                if (Collision.RectangleCollision(
                    player.Position
                    , player.Width
                    , player.Height
                    , rock.Position
                    , rock.Width
                    , rock.Height))
                {
                    gameOver = true;
                }
            }
            //PowerUps
            foreach (var powerUp in powerUps)
            {
                if (Collision.RectangleCollision(
                    player.Position
                    , player.Width
                    , player.Height
                    , powerUp.Position
                    , powerUp.Width
                    , powerUp.Height))
                {
                    gravity -= 0.2f;
                    powerUp.Active = false;
                }
            }
            
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
                player.Speed += new Vector2(0, gravity);
                player.Position += player.Speed;
            }

            //When the player gets to the top. reset their position to the bottom of the screen and kill the floor.
            if (player.Position.Y <= 0)
            {
                player.Position = new Vector2(player.Position.X
                    , 700);
                score += 10;
                level++;
                if (level % 3 == 0)
                {
                    gravity += 0.1f;
                    rockMod++; 
                    rockSpawnTime -= TimeSpan.FromSeconds(0.1f);
                }
                floor.Active = false;
                tutorial = false;
                //reset fruit
                foreach (var fruit in fruits)
                {
                    fruit.Active = false;
                }

                //reset rocks
                foreach (var rock in rocks)
                {
                    rock.Active = false;
                }

                foreach (var powerup in powerUps)
                {
                    powerup.Active = false;
                }
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
            CreateRock(gameTime);
            CreatePowerUp(gameTime);
            UpdateRocks();
            UpdateFruit();
            UpdatePowerUps();
        }

        private void CreateFruit(GameTime gameTime)
        {
            var random = new Random();
            if (gameTime.TotalGameTime - previousFruitSpawnTime >
                fruitSpawnTime
                && fruits.Count < 3
                && !floor.Active)
            {
                previousFruitSpawnTime = gameTime.TotalGameTime;
                var fruit = new Fruit();
                var x = random.Next(10, 360);
                var y = random.Next(20, 600);
                fruit.Initialize(new Vector2(x, y), cherries);
                fruits.Add(fruit);
            }
        }

        private void UpdateFruit()
        {
            for (var i = 0;
                i < fruits.Count; i++)
            {
                if (!fruits[i].Active)
                {
                    fruits.Remove(fruits[i]);
                }
            }
        }

        private void CreateRock(GameTime gameTime)
        {
            var random = new Random();
            if (gameTime.TotalGameTime - previousRockSpawnTime >
                rockSpawnTime
                && rocks.Count < 3 * rockMod
                && !floor.Active)
            {
                previousRockSpawnTime = gameTime.TotalGameTime;
                var rock = new Rock();
                var x = random.Next(20, 340);
                rock.Initialize(new Vector2(x, 50), rockSprite);
                rocks.Add(rock);
            }
        }

        private void UpdateRocks()
        {
            for (var i = 0; i < rocks.Count; i++)
            {
                rocks[i].Update(gravity + 1);
                if (rocks[i].Position.Y > ScreenHeight)
                {
                    rocks[i].Active = false;
                }
                if (!rocks[i].Active)
                {
                    rocks.Remove(rocks[i]);
                }
            }
        }
        private void CreatePowerUp(GameTime gameTime)
        {
            var random = new Random();
            if (gameTime.TotalGameTime - previousPowerUpSpawnTime >
                powerUpSpawnTime
                && powerUps.Count < 1
                && !floor.Active
                && gravity > 0.1f)
            {
                previousFruitSpawnTime = gameTime.TotalGameTime;
                var powerUp = new PowerUp();
                var x = random.Next(10, 360);
                var y = random.Next(20, 600);
                powerUp.Initialize(new Vector2(x, y), gravityDownSprite);
                powerUps.Add(powerUp);
            }
        } 
        private void UpdatePowerUps()
        {
            for (var i = 0;
                i < powerUps.Count; i++)
            {
                if (!powerUps[i].Active)
                {
                    powerUps.Remove(powerUps[i]);
                }
            }
        } 
    }
}