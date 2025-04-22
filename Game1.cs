using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FishingGame
{
    public class Game1 : Game
    {

        // Properties
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;
        private SpriteFont _font;
        private int _fishCounter = 0;
        private bool _fishBites = false;
        private bool _fishingRodCast = false;
        private Random _randomiser = new Random();
        private float _biteTimer;
        private float _biteInterval;
        private float _catchTimer;
        private float _catchTimerLimit = 2;

        private KeyboardState _currentKbState;
        private KeyboardState _previousKbState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "Fishing Game";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _device = _graphics.GraphicsDevice;

            // TODO: use this.Content to load your game content here
            _font = Content.Load<SpriteFont>("myFont");
            _biteInterval = (float)_randomiser.Next(2, 6);
        }

        private bool IsKeyPressed (Keys key)
        {
            return _currentKbState.IsKeyDown(key) && !_previousKbState.IsKeyDown(key);
        }

        private void ResetFish()
        {
            _fishBites = false;
            _fishingRodCast = false;
            _biteTimer = 0;
            _biteInterval = _randomiser.Next(2, 6);
        }

        private void FishBiteTimer(GameTime gameTime)
        {
            _previousKbState = _currentKbState;
            _currentKbState = Keyboard.GetState();

            if (!_fishingRodCast && IsKeyPressed(Keys.Space))
            {
                _fishingRodCast = true;
            }

            if (_fishingRodCast)
            {
                _biteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (!_fishBites)
                {
                    if (_biteTimer > _biteInterval)
                    {
                        _fishBites = true;
                        _catchTimer = _catchTimerLimit;
                    }
                }
                else
                {
                    _catchTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (IsKeyPressed(Keys.Space))
                    {
                        _fishCounter++;
                        ResetFish();
                    }

                    if (_catchTimer <= 0)
                    {
                        ResetFish();
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            FishBiteTimer(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            DrawText();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawText()
        {
            _spriteBatch.DrawString(_font, "Fish caught: " + _fishCounter.ToString(), new Vector2(20,45), Color.White);
            _spriteBatch.DrawString(_font, "Time until fish bites: " + _biteInterval.ToString(), new Vector2(20, 280), Color.White);
            if (!_fishingRodCast)
            {
                _spriteBatch.DrawString(_font, "Press Space to cast your fishing rod.", new Vector2(20, 105), Color.White);
            }
            else if (_fishBites)
            {
                _spriteBatch.DrawString(_font, "Fish hooked! Press Space to catch it.", new Vector2(20, 105), Color.White);
            }
        }
    }
}
