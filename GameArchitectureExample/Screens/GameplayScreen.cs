using System;
using System.Timers;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameArchitectureExample.StateManagement;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GameArchitectureExample.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private static System.Timers.Timer waitTimer;

        private ContentManager _content;
        private SpriteFont _gameFont;

        private SpriteBatch _spriteBatch;


        private Vector2 _playerPosition = new Vector2(400, 400);
        private Vector2 _coinPosition = new Vector2(200, 200);

        private readonly Random _random = new Random();

        public List<CoinSprite> coinsList = new List<CoinSprite>();

        private SlimeSprite slimeSprite;
        private int coinsCount;

        private SoundEffect coinPickup;
        private string coinSoundName = "ICanTellWav";

        private Song gameMusic;
        private string gameMusicName = "bensound-dreams";

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _spriteBatch = ScreenManager.SpriteBatch;

            if(coinsList.Count == 0 )
            {
                CoinSprite temp = new CoinSprite(_coinPosition);
                coinsList.Add(temp);
            }         

            coinsCount = 0;
            slimeSprite = new SlimeSprite();

            // Loads content for gameplay elements
            foreach (var coin in coinsList) coin.LoadContent(_content);
            slimeSprite.LoadContent(_content);
            coinPickup = _content.Load<SoundEffect>(coinSoundName);
            gameMusic = _content.Load<Song>(gameMusicName);
            MediaPlayer.IsRepeating = true;
            float roundedMusicVolume = (float)(MediaPlayer.Volume / 4.0);
            MediaPlayer.Volume = roundedMusicVolume;
            MediaPlayer.Play(gameMusic);
            _gameFont = _content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            //ScreenManager.Game.ResetElapsedTime();
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            System.Random rand = new System.Random();


            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Vector2 acceleration = new Vector2(0, 30);
                bool addCoin = false;
                List<CoinSprite> tempCoinsList = new List<CoinSprite>();

                foreach (var coin in coinsList)
                {
                    ///If the player touches a coin
                    if (!coin.Collected && coin.Bounds.CollidesWith(slimeSprite.Bounds))
                    {
                        addCoin = true;
                        coinsCount++;
                        coin.Collected = true;
                        coin.Warp();
                        coinPickup.Play();
                    }
                    /// If the coin goes beneath the game window
                    if (coin.Position.Y > Constants.GAME_HEIGHT)
                    {
                        Vector2 temp = new Vector2(coin.Position.X, 0);
                        coin.Position = temp;
                    }
                    /// If the coin goes past the right boundary
                    if (coin.Position.X > Constants.GAME_WIDTH)
                    {
                        Vector2 temp = new Vector2(0, coin.Position.Y);
                        coin.Position = temp;
                    }
                    coin.Velocity += acceleration * t;
                    coin.Position += t * coin.Velocity;
                    /// Check to see if the player has gotten enough coins to win 
                    if (coinsCount > 9)
                    {
                        //System.Windows.Forms.MessageBoxButtons buttons = System.Windows.Forms.MessageBoxButtons.YesNo;
                        //System.Windows.Forms.DialogResult result;
                        //result = MessageBox.Show("Congrats", "You Won!", (IEnumerable<string>)buttons);               
                    }

                    ///Adds a coin randomly in the game window
                    if (addCoin)
                    {
                        CoinSprite tempCoin = new CoinSprite(
                            new Vector2((float)rand.NextDouble() * Constants.GAME_WIDTH,
                            (float)rand.NextDouble() * Constants.GAME_HEIGHT));
                        tempCoin.LoadContent(_content);
                        if (!(tempCoinsList == null))
                            tempCoinsList.Add(tempCoin);
                        addCoin = false;
                    }
                }
                foreach (CoinSprite c in tempCoinsList)
                {
                    coinsList.Add(c);
                }
                tempCoinsList = new List<CoinSprite>();

            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            //int playerIndex = (int)ControllingPlayer.Value;
            int playerIndex = 1;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Otherwise move the player position.
                var movement = Vector2.Zero;


                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) movement += slimeSprite.ApplyMovement(MovementDirection.Up);
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) movement += slimeSprite.ApplyMovement(MovementDirection.Down);
                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)) movement += slimeSprite.ApplyMovement(MovementDirection.Left);
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)) movement += slimeSprite.ApplyMovement(MovementDirection.Right);


               
                _playerPosition += movement;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            foreach (var coin in coinsList)
            {
                coin.Draw(gameTime, spriteBatch);

            }

            slimeSprite.Draw(gameTime, spriteBatch);
            //spriteBatch.DrawString(_gameFont, gameTime, new Vector2() )
            spriteBatch.DrawString(_gameFont, $"Coin Count: {coinsCount}", new Vector2(2, 2), Color.Gold, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(_gameFont,  gameTime.ElapsedGameTime.TotalSeconds.ToString(), new Vector2(300, 2), Color.Gold, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            if (coinsCount < 4)
            {
                spriteBatch.DrawString(_gameFont, $"Collect 10 Coins to Win", new Vector2(500, 2), Color.Gold, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
            if(coinsCount > 9)
            {
                spriteBatch.DrawString(_gameFont, $"Congrats you won", new Vector2(300, 2), Color.Gold, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

       
    }
}
