using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Fishing_game
{
    public class Game1 : Game
    {

        // Properties
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GraphicsDevice _device;
        private SpriteFont _font;
        private int _fishCounter = 0;
        private bool _fishHooked = false;

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
        }

        private void ProcessKeyboard()
        {
            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.Space))
            {
                _fishCounter++;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
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
        }
    }
}
