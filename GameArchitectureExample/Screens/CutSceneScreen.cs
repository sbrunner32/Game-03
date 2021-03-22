using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using GameArchitectureExample.StateManagement;
using System.Diagnostics;

namespace GameArchitectureExample.Screens
{
    public class CutSceneScreen :GameScreen
    {
        ContentManager _content;
        Video _video;
        VideoPlayer _player;
        bool _isPlaying = false;
        InputAction _skip;

        public CutSceneScreen()
        {
            _player = new VideoPlayer();
            _skip = new InputAction(new Buttons[] { Buttons.A }, new Keys[] { Keys.Space, Keys.Enter }, true);
        }


        public override void Activate()
        {
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            _video = _content.Load<Video>("Tony_Maqaroni_Intro");
            
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (!_isPlaying)
            {
                _player.Play(_video);
                _isPlaying = true;
            }
            PlayerIndex player;
            if(_skip.Occurred(input, null,out player))
            {
                _player.Stop();
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_player.PlayPosition >= _video.Duration) ExitScreen();
        }

        public override void Deactivate()
        {
            _player.Pause();
            _isPlaying = false;
        }

        public override void Draw(GameTime gameTime)
        {
            /*if(_isPlaying)
            {
                ScreenManager.SpriteBatch.Begin();
                Texture2D tempTexture;
                if(VerifyTexture(_player, out tempTexture))
                {
                    ScreenManager.SpriteBatch.Draw(tempTexture, Vector2.Zero, Color.White);
                }
                //ScreenManager.SpriteBatch.Draw(_player.GetTexture(), Vector2.Zero, Color.White);
                ScreenManager.SpriteBatch.End();
            }*/
            
        }

        public bool VerifyTexture(VideoPlayer vp, out Texture2D? frameTexture)
        {
            Texture2D t1, t2;
            if (_video == null)
                throw new InvalidOperationException("Operation is not valid due to the current state of the object");

            //XNA never returns a null texture
            const int timeOutMs = 500;
            bool textureFound = true;
            Texture2D texture = null;
            var timer = new Stopwatch();
            timer.Start();
            frameTexture = null;
            if(vp.State == MediaState.Playing)
            do
            {
                try
                {
                     texture = vp.GetTexture();
                        frameTexture = texture;
                }
                catch(Exception e)
                {
                        frameTexture = null;
                        textureFound = false;
                        continue;
                }
                if (timer.ElapsedMilliseconds > timeOutMs)
                {
                    frameTexture = null;
                    return false;
                }
            } while (texture == null);
            return textureFound;
        }
    }
    
}
