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
        private GraphicsDevice _device;
        private SpriteFont _font;

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
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 500;
            _graphics.PreferredBackBufferHeight = 500;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Window.Title = "Fishing Game";
            LoadGame();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _device = _graphics.GraphicsDevice;

            // TODO: use this.Content to load your game content here
            _font = Content.Load<SpriteFont>("myFont");

            _fishTypes = new List<Fish>
            {
                new Fish {Name = "Salmon", Points = 10},
                new Fish {Name = "Pike", Points = 8},
                new Fish {Name = "Trout", Points = 5},
                new Fish {Name = "Perch", Points = 3},
                new Fish {Name = "Bream", Points = 1},
                new Fish {Name = "Tin Can", Points = 0}
            };

            // Initialises the randomisers at start
            ResetFish();
        }

        private bool IsKeyPressed (Keys key)
        {
            // Makes sure each key fires only once when pressed
            return _currentKbState.IsKeyDown(key) && !_previousKbState.IsKeyDown(key);
        }

        private void ResetFish()
        {
            _fishBites = false;
            _fishingRodCast = false;
            _biteTimer = 0;
            _biteInterval = _randomiser.Next(2, 6);
            _nextFish = _fishTypes[_randomiser.Next(_fishTypes.Count)];
        }

        private void ResetStats()
        {
            // Resets the save file
            _catchList.Clear();
            _fishPoints = 0;
            SaveGame();
        }

        private void SaveGame()
        {
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
                // Starts the timer to see when the fish will bite
                _biteTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // The fish bites once the timer reaches a randomised number
                // Sets the 2 second timer for catching the fish
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
                    // Starts the 2 second timer during which the fish must be caught
                    _catchTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Chooses a previously randomised fish from the list and
                    // adds it to another list that is displayed to the player
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

                    // If the fish isn't caught in two seconds,
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

            // TODO: Add your update logic here

            // Creates the gameplay loop for catching fish
            CatchFish(gameTime);

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
            // These two are displayed for testing purposes
            _spriteBatch.DrawString(_font, $"Points: {_fishPoints}", new Vector2(20,45), Color.White);
            _spriteBatch.DrawString(_font, $"Time until fish bites: {_biteInterval}", new Vector2(20, 450), Color.White);

            if (_fishEscaped)
            {
                _spriteBatch.DrawString(_font, "The fish got away!", new Vector2(20, 85), Color.White);
            }

            if (!_fishingRodCast)
            {
                _spriteBatch.DrawString(_font, "Press Space to cast your fishing rod.", new Vector2(20, 105), Color.White);
            }
            else if (_fishBites)
            {
                _spriteBatch.DrawString(_font, "Fish hooked! Press Space to catch it.", new Vector2(20, 105), Color.White);
            }

            if (_caughtFish != null)
            {
                _spriteBatch.DrawString(_font, $"You caught a {_caughtFish.Name}!", new Vector2(20, 125), Color.White);
            }

            if (_showCatchList)
            {
                // Shows the list of caught fish when the player presses Tab
                int yOffset = 10;
                _spriteBatch.DrawString(_font, "Fish caught:", new Vector2(300, yOffset), Color.White);
                yOffset += 30;
                foreach (var i in _catchList)
                {
                    _spriteBatch.DrawString(_font, $"{i.Key}: {i.Value}", new Vector2(300, yOffset), Color.White);
                    yOffset += 20;
                }
            }
        }
    }
}
