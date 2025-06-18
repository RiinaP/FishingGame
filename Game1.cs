using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Text.Json;

namespace FishingGame
{
    public class Fish
    {
        public string Name;
        public int Points;
        public Texture2D Texture;
    }

    public class SaveData
    {
        public Dictionary<string, int> CatchList { get; set; } = new();
        public int Points { get; set; } = 0;
    }

    public class Game1 : Game
    {
        // Properties
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private Texture2D _bgTexture;
        private Texture2D _frog1Texture;
        private Texture2D _frog2Texture;
        private Texture2D _exclamationTexture;
        private Texture2D _boardTexture;

        private Vector2 _frogPosition;

        private int _fishPoints = 0;
        private bool _fishBites = false;
        private bool _fishingRodCast = false;
        private bool _fishEscaped = false;

        private Random _randomiser = new Random();
        private float _biteTimer;
        private float _biteInterval;
        private float _catchTimer;
        private float _catchTimerLimit = 2;

        private Fish _caughtFish;
        private Fish _nextFish;
        private List<Fish> _fishTypes = new List<Fish>();

        private Dictionary<string, int> _catchList = new Dictionary<string, int>();
        private bool _showCatchList = false;

        private const string SaveFilePath = "catch_list.json";

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
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "Fishing Game";
            _frogPosition = new Vector2(427, 363);
            LoadGame();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loads textures
            _bgTexture = Content.Load<Texture2D>("background");
            _frog1Texture = Content.Load<Texture2D>("frog1");
            _frog2Texture = Content.Load<Texture2D>("frog2");
            _exclamationTexture = Content.Load<Texture2D>("exclamation");
            _boardTexture = Content.Load<Texture2D>("board");
            _font = Content.Load<SpriteFont>("myFont");

            // Sets up fish types and loads textures
            _fishTypes = new List<Fish>
            {
                new Fish {Name = "Flounder", Points = 8, Texture = Content.Load<Texture2D>("flounder")},
                new Fish {Name = "Perch", Points = 3, Texture = Content.Load<Texture2D>("perch")},
                new Fish {Name = "Tin Can", Points = 1, Texture = Content.Load<Texture2D>("tincan")}
            };

            // Initialises the randomisers at start
            ResetFish();
        }

        private bool IsKeyPressed(Keys key)
        {
            // Makes sure each key fires only once when pressed
            return _currentKbState.IsKeyDown(key) && !_previousKbState.IsKeyDown(key);
        }

        private void ResetFish()
        {
            // Sets everything up for the next fish to be caught
            _fishBites = false;
            _fishingRodCast = false;
            _biteTimer = 0;
            _biteInterval = _randomiser.Next(2, 6);
            _nextFish = _fishTypes[_randomiser.Next(_fishTypes.Count)];
        }

        private void ResetStats()
        {
            // Resets all progress
            _catchList.Clear();
            _fishPoints = 0;
            SaveGame();
        }

        private void SaveGame()
        {
            // Saves all progress to a json file
            try
            {
                var saveData = new SaveData
                {
                    CatchList = _catchList,
                    Points = _fishPoints
                };
                string json = JsonSerializer.Serialize(saveData);
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
            }
        }

        private void LoadGame()
        {
            // If save data can be found, loads the contents
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    string json = File.ReadAllText(SaveFilePath);
                    var saveData = JsonSerializer.Deserialize<SaveData>(json);
                    if (saveData != null)
                    {
                        _catchList = saveData.CatchList;
                        _fishPoints = saveData.Points;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
                _catchList = new Dictionary<string, int>();
                _fishPoints = 0;
            }
        }

        private void CatchFish(GameTime gameTime)
        {
            _previousKbState = _currentKbState;
            _currentKbState = Keyboard.GetState();

            if (IsKeyPressed(Keys.Tab))
            {
                _showCatchList = !_showCatchList;
            }

            if (IsKeyPressed(Keys.R))
            {
                ResetStats();
            }

            if (!_fishingRodCast && IsKeyPressed(Keys.Space))
            {
                // Casts the fishing rod to start fishing and clears the "fish got away" message
                _fishingRodCast = true;
                _fishEscaped = false;
                _caughtFish = null;
            }

            if (_fishingRodCast)
            {
                // Starts the first timer to see when the fish will bite
                _biteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // The fish bites once the first timer reaches a randomised number
                // Sets the second timer during which the fish must be caught
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
                    // Starts the second timer
                    _catchTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Chooses a previously randomised fish from the _fishTypes list and
                    // adds it to a list that is displayed to the player
                    if (IsKeyPressed(Keys.Space))
                    {
                        _caughtFish = _nextFish;
                        _fishPoints += _caughtFish.Points;

                        string name = _caughtFish.Name;
                        if (_catchList.ContainsKey(name))
                        {
                            _catchList[name]++;
                        }
                        else
                        {
                            _catchList[name] = 1;
                        }

                        SaveGame();
                        ResetFish();
                    }

                    // If the fish isn't caught in time,
                    // displays the "fish got away" message
                    if (_catchTimer <= 0)
                    {
                        ResetFish();
                        _fishEscaped = true;
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Creates the gameplay loop for catching fish
            CatchFish(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draws the graphics to the screen
            _spriteBatch.Begin();
            DrawScenery();
            DrawGraphics();
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, 800, 500);
            _spriteBatch.Draw(_bgTexture, screenRectangle, Color.White);
            _spriteBatch.Draw(_frog1Texture, _frogPosition, Color.White);
        }

        private void DrawGraphics()
        {
            if (_fishEscaped)
            {
                _spriteBatch.DrawString(_font, "The fish got away !", new Vector2(20, 5), Color.SaddleBrown);
            }

            if (!_fishingRodCast)
            {
                //_spriteBatch.DrawString(_font, "Press Space to cast your fishing rod.", new Vector2(20, 5), Color.SaddleBrown);
            }
            else if (_fishBites)
            {
                //_spriteBatch.DrawString(_font, "Fish hooked! Press Space to catch it.", new Vector2(20, 105), Color.White);
                _spriteBatch.Draw(_exclamationTexture, new Vector2(420, 255), Color.White);
                _spriteBatch.Draw(_frog2Texture, _frogPosition, Color.White);
            }

            if (_caughtFish != null)
            {
                _spriteBatch.DrawString(_font, $"You caught a {_caughtFish.Name}!", new Vector2(20, 5), Color.SaddleBrown);
            }

            if (_showCatchList)
            {
                // Shows the list of caught fish when the player presses Tab
                _spriteBatch.Draw(_boardTexture, new Vector2(5, -145), Color.White);

                int yOffset = 100;
                _spriteBatch.DrawString(_font, "Fish caught:", new Vector2(370, yOffset), Color.SaddleBrown);
                yOffset += 30;

                foreach (var i in _catchList)
                {
                    Fish fish = _fishTypes.Find(f => f.Name == i.Key);
                    if (fish != null && fish.Texture != null)
                    {
                        _spriteBatch.Draw(fish.Texture, new Vector2(240, yOffset), Color.White);
                        _spriteBatch.DrawString(_font, $"x{i.Value}", new Vector2(300, yOffset + 8), Color.SaddleBrown);
                        yOffset += 60;
                    }
                }
            }
        }
    }
}
