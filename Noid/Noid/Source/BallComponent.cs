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

        private Ball _myBall;

        private Texture2D _brickTexture;

        private ICollection<LineSegment> windowWalls;

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
            _myBall.VelocityFromAngle(45.0f * (float)Math.PI / 180.0f, 600);
            _myBall.Position.X = 100;
            _myBall.Position.Y = 200;

            LineSegment leftWall = new LineSegment(0, 0, 0, Game.Window.ClientBounds.Height);
            LineSegment rightWall = new LineSegment(Game.Window.ClientBounds.Width, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            LineSegment topWall = new LineSegment(0, 0, Game.Window.ClientBounds.Width, 0);
            LineSegment bottomWall = new LineSegment(0, Game.Window.ClientBounds.Height, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);

            windowWalls = new List<LineSegment>();

            windowWalls.Add(leftWall);
            windowWalls.Add(rightWall);
            windowWalls.Add(topWall);
            windowWalls.Add(bottomWall);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _ballTexture = Game.Content.Load<Texture2D>("Images/Ball");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            UpdateBall(_myBall, (float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        private void UpdateBall(Ball ball, float dt)
        {
            ball.Position += ball.Velocity * dt;

            foreach (var wall in windowWalls)
            {
                ApplyCollision(ball, wall);
            }
        }

        private void ApplyCollision(Ball ball, LineSegment line)
        {
            if (Collision.Intersects(line, ball))
            {
                Vector2 intersectionPoint = Collision.NearestIntersectionPointToLineSegment(line, ball.Position);

                ball.Position += Collision.DistanceToNotCollide(ball, intersectionPoint);

                Vector2 surfaceNormal = Vector2.Normalize(intersectionPoint - ball.Position);

                ball.Velocity = Vector2.Reflect(ball.Velocity, surfaceNormal);
            }
        }

        public override void Draw(GameTime gameTime)
        {
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

            _spriteBatch.End();
        }

    }
}
