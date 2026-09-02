using Engine;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace SubsetSharpEngine {
    public class Player: Entity {
        public readonly BoxCollider collider = new BoxCollider (Vector2.Zero, new Vector2(32));
        public readonly RectangleRenderer renderer;

        public Player () {
            renderer = new RectangleRenderer (collider, 0);
            components = new object[] { collider, renderer };
        }

        protected override void OnSetup () {}

        public override void Update (float dt) {
            var dir = Vector2.Zero;
            if (Raylib.IsKeyDown (KeyboardKey.D)) {
                dir.X += 1;
            }
            if (Raylib.IsKeyDown (KeyboardKey.A)) {
                dir.X -= 1;
            }
            if (Raylib.IsKeyDown (KeyboardKey.W)) {
                dir.Y -= 1;
            }
            if (Raylib.IsKeyDown (KeyboardKey.S)) {
                dir.Y += 1;
            }
            var dirNormalized = Vector2.Zero;
            if (dir != Vector2.Zero) dirNormalized = Vector2.Normalize (dir);
            collider.position += dirNormalized * dt * 200;
            
            base.Update (dt);
        }
    }
}
