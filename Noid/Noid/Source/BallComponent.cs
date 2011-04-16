using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Noid
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BallComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _ballTexture;
        private Texture2D _brickTexture;
        private Texture2D _vectorTexture;

        private Ball _myBall;
        private Brick _testBrick1;

        private Brick _windowBrick;

        private bool _ballPaused = true;
        private bool _collisionPaused = true;
        private bool _incBallSpeed = false;
        private bool _decBallSpeed = false;

        private KeyboardState _prevKeyState;

        public BallComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            _myBall = new Ball();
            _myBall.VelocityFromAngle(-45.0f * (float)Math.PI / 180.0f, 100);
            _myBall.Position.X = 152;
            _myBall.Position.Y = Game.Window.ClientBounds.Height - 494;

            _windowBrick = new Brick(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);

            _testBrick1 = new Brick(200, 100, 120, 64);

            _prevKeyState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _ballTexture = Game.Content.Load<Texture2D>("Images/Ball");
            _brickTexture = Game.Content.Load<Texture2D>("Images/BrickTemplate");
            _vectorTexture = Game.Content.Load<Texture2D>("Images/DirectionVector");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.B) && _prevKeyState.IsKeyUp(Keys.B))
                _ballPaused = !_ballPaused;

            if (keyState.IsKeyDown(Keys.C) && _prevKeyState.IsKeyUp(Keys.C))
                _collisionPaused = !_collisionPaused;

            if (keyState.IsKeyDown(Keys.N) && _prevKeyState.IsKeyUp(Keys.N))
                _decBallSpeed = !_decBallSpeed;

            if (keyState.IsKeyDown(Keys.M) && _prevKeyState.IsKeyUp(Keys.M))
                _incBallSpeed = !_incBallSpeed;


            _prevKeyState = keyState;

            UpdateBall(_myBall, (float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        private void UpdateBall(Ball ball, float dt)
        {
            if (_ballPaused) return;

            if (_incBallSpeed)
            {
                ball.Velocity *= 1.1f;
                _incBallSpeed = false;
            }

            if (_decBallSpeed)
            {
                ball.Velocity *= 0.9f;
                _decBallSpeed = false;
            }

            ball.Position += ball.Velocity * dt;

            foreach (var wall in _windowBrick.Walls)
            {
                Vector2? collPoint = ApplyCollision(ball, wall);

                if (collPoint != null)
                {
                    Vector2 surfaceNormal = Vector2.Normalize((Vector2)collPoint - ball.Position);
                    ball.Velocity = Vector2.Reflect(ball.Velocity, surfaceNormal);
                }

            }

            if (!_collisionPaused)
            {
                bool recheck;
                bool wasCollision = false;
                Vector2? latestCollPoint = null;

                do
                {
                    recheck = false;
                    foreach (var wall in _testBrick1.Walls)
                    {
                        Vector2? collPoint = ApplyCollision(ball, wall);

                        if (collPoint != null)
                        {
                            recheck = true;
                            wasCollision = true;
                            latestCollPoint = collPoint;
                            break;
                        }
                    }
                } while (recheck);

                if (wasCollision)
                {
                    Vector2 surfaceNormal = Vector2.Normalize((Vector2)latestCollPoint - ball.Position);
                    ball.Velocity = Vector2.Reflect(ball.Velocity, surfaceNormal);
                }

            }
        }

        private Vector2? ApplyCollision(Ball ball, LineSegment line)
        {
            if (Collision.Intersects(line, ball))
            {
                Vector2 intersectionPoint = Collision.NearestIntersectionPointToLineSegment(line, ball.Position);

                var delta = Collision.DistanceToNotCollide(ball, intersectionPoint);

                Console.WriteLine(ball.Position + " + " + delta);

                ball.Position += delta;


                return Collision.NearestIntersectionPointToLineSegment(line, ball.Position);
                //Vector2 surfaceNormal = Vector2.Normalize(intersectionPoint - ball.Position);
                //ball.Velocity = Vector2.Reflect(ball.Velocity, surfaceNormal);

                
            }

            return null;
        }

        public override void Draw(GameTime gameTime)
        {
            DrawBrick(_testBrick1);
            DrawBall(_myBall);

            base.Draw(gameTime);
        }

        private void DrawBall(Ball ball)
        {
            Vector2 origin;

            origin.X = ball.Radius;
            origin.Y = ball.Radius;

            _spriteBatch.Begin();

            _spriteBatch.Draw(_ballTexture, ball.Position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);

            _spriteBatch.Draw(_vectorTexture, ball.Position, null, Color.White, (float) Math.Atan2(ball.Velocity.Y, ball.Velocity.X), origin, 1, SpriteEffects.None, 0);

            _spriteBatch.End();
        }

        private void DrawBrick(Brick brick)
        {
            Rectangle destRect = new Rectangle(
                (int) brick.Position.X,
                (int) brick.Position.Y,
                (int) brick.Size.X,
                (int) brick.Size.Y);


            float opacity = 1.0f;

            if (_collisionPaused) opacity = 0.2f;

            _spriteBatch.Begin();

            _spriteBatch.Draw(_brickTexture, destRect, Color.Violet * opacity);

            _spriteBatch.End();
        }
    }
}
