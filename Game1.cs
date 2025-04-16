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
        private Random _randomiser = new Random();
        private float _biteTimer = 0;
        private float _biteInterval = 0;
        //private float _catchTimer = 0;

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
            _biteInterval = (float)_randomiser.Next(3, 11);

        }

        private void ProcessKeyboard()
        {
            KeyboardState kbState = Keyboard.GetState();

            if (_fishBites == true)
            {
                if (kbState.IsKeyDown(Keys.Space))
                {
                    _fishCounter++;
                    _fishBites = false;
                    _biteTimer = 0;
                    _biteInterval = _randomiser.Next(2, 6);
                }
            }
        }

        private void FishBiteTimer(GameTime gameTime)
        {
            _biteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_biteTimer > _biteInterval)
            {
                _fishBites = true;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            FishBiteTimer(gameTime);


            ProcessKeyboard();

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
            _spriteBatch.DrawString(_font, _biteInterval.ToString(), new Vector2(20, 95), Color.White);
            if (_fishBites == true)
            {
                _spriteBatch.DrawString(_font, "Fish hooked!", new Vector2(20, 70), Color.White);
            }
        }
    }
}
