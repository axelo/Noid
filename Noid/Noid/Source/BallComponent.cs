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
using NoidDataTypes;

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
        private BrickData[] _brickDatas;

        private Ball _myBall;

        private ICollection<AABB> _levelBricks;

        private bool _ballPaused = true;
        private bool _collisionPaused = false;
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
            _myBall.VelocityFromAngle(180.0f * (float)Math.PI / 180.0f, 800);
            _myBall.Circle.Position.X = 180;
            _myBall.Circle.Position.Y = 100 + 44;

            _levelBricks = new List<AABB>();

            _levelBricks.Add(new AABB(0, 0, 1, Game.Window.ClientBounds.Height));
            _levelBricks.Add(new AABB(0, Game.Window.ClientBounds.Height, Game.Window.ClientBounds.Width, 1));
            _levelBricks.Add(new AABB(Game.Window.ClientBounds.Width, 0, 1, Game.Window.ClientBounds.Height));
            _levelBricks.Add(new AABB(0, 0, Game.Window.ClientBounds.Width, 1));


            //_levelBricks.Add(new AABB(200, 100, 20, 65));
            //_levelBricks.Add(new AABB(200, 220, 20, 20));

            //_levelBricks.Add(new AABB(300, 300, 20, 20));
            //_levelBricks.Add(new AABB(20, 20, 20, 20));

            //Random r = new Random();

            //for (int i = 0; i < 40; ++i)
            //{
            //    _levelBricks.Add(new AABB(r.Next(0, Game.Window.ClientBounds.Width), r.Next(0, Game.Window.ClientBounds.Height), r.Next(20, 30), r.Next(20, 30)));
            //}


            _prevKeyState = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _ballTexture = Game.Content.Load<Texture2D>("Images/Ball");
            _brickTexture = Game.Content.Load<Texture2D>("Images/BrickTemplate");
            _vectorTexture = Game.Content.Load<Texture2D>("Images/DirectionVector");

            _brickDatas = Game.Content.Load<BrickData[]>("XMLFile1");

            foreach (var brickData in _brickDatas)
            {
                var brick = new AABB(brickData.Position.X, brickData.Position.Y, brickData.Size.X, brickData.Size.Y);
                brick.Color = brickData.Color;
                _levelBricks.Add(brick);
            }

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

            if (keyState.IsKeyDown(Keys.Up) && _prevKeyState.IsKeyUp(Keys.Up))
                _myBall.Circle.Position.Y--;

            if (keyState.IsKeyDown(Keys.Down) && _prevKeyState.IsKeyUp(Keys.Down))
                _myBall.Circle.Position.Y++;

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
                //ball.Velocity = Vector2.Clamp(ball.Velocity, new Vector2(-960.0f, -960.0f), new Vector2(960.0f, 960.0f));
                _incBallSpeed = false;
            }

            if (_decBallSpeed)
            {
                ball.Velocity *= 0.9f;
                //ball.Velocity = Vector2.Clamp(ball.Velocity, new Vector2(-960.0f, -960.0f), new Vector2(960.0f, 960.0f));
                _decBallSpeed = false;
            }


            ball.LastPosition = ball.Circle.Position;

            ball.Circle.Position += ball.Velocity * dt;

            //Console.WriteLine("old: " + ball.LastPosition);
            //Console.WriteLine("current: " + ball.Circle.Position);

            ApplyCollision();

            //Console.WriteLine("new: " + ball.Circle.Position);
        }

        private void ApplyCollision()
        {
            if (!_collisionPaused)
            {
                var collisionVectors = Collision.CollisionVectors(_myBall.LastPosition, _myBall.Circle, _levelBricks);

                if (collisionVectors.Count > 0)
                {
                    IEnumerable<Vector2> vs = from cdata in collisionVectors select cdata.NearestVector;

                    Vector2 avg = Collision.AverageVector(vs);

                    Collision.PushCircleApartFromAABB(_myBall.Circle, avg, 0);

                    Vector2 avgSurfaceNormal = Collision.AverageSurfaceNormal(vs);

                    Console.WriteLine(avgSurfaceNormal);

                    var take2 = Collision.CollisionVectors(_myBall.Circle, from cd in collisionVectors select cd.CollidedWith);

                    if (take2.Count > 0)
                    {
                        avg = Collision.AverageVector(from cd in take2 select cd.NearestVector);
                        avgSurfaceNormal = Collision.AverageSurfaceNormal(from cd in take2 select cd.NearestVector);

                        Collision.PushCircleApartFromAABB(_myBall.Circle, avg, 0.1f);
                    }

                    Console.WriteLine(avgSurfaceNormal);

                    _myBall.Velocity = Vector2.Reflect(_myBall.Velocity, avgSurfaceNormal);
                    
                    _myBall.LastPosition = _myBall.Circle.Position;

                    //_ballPaused = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var brick in _levelBricks)
            {
                DrawBrick(brick);
            }

            DrawBall(_myBall);

            base.Draw(gameTime);
        }

        private void DrawBall(Ball ball)
        {
            Vector2 origin;

            origin.X = ball.Circle.Radius;
            origin.Y = ball.Circle.Radius;

            _spriteBatch.Begin();

            _spriteBatch.Draw(_ballTexture, ball.Circle.Position, null, Color.White, 0, origin, 1, SpriteEffects.None, 0);

            _spriteBatch.Draw(_vectorTexture, ball.Circle.Position, null, Color.White, (float)Math.Atan2(ball.Velocity.Y, ball.Velocity.X), origin, 1, SpriteEffects.None, 0);

            _spriteBatch.End();
        }

        private void DrawBrick(AABB brick)
        {
            Rectangle destRect = new Rectangle(
                (int)(brick.Position.X - brick.HalfExtent.X),
                (int)(brick.Position.Y - brick.HalfExtent.Y),
                (int)brick.HalfExtent.X * 2,
                (int)brick.HalfExtent.Y * 2);

            float opacity = (float)1.0f;

            if (_collisionPaused) opacity = 0.2f;

            _spriteBatch.Begin();

            _spriteBatch.Draw(_brickTexture, destRect, brick.Color * opacity);

            _spriteBatch.End();
        }
    }
}
