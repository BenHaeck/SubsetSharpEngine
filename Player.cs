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
        public List<EntityWithTagAndBoxCollider> walls;
        public Player () {
            renderer = new RectangleRenderer (collider, 0);
            components = new object[] { collider, renderer };
        }

        protected override void OnSetup (EntitySystem entitySystem) {
            walls = entitySystem.GetEntityCollection<EntitiesByTagCollection<EntityWithTagAndBoxCollider, Tags>> ()!.GetEntitiesByTags(Tags.Wall)!;
        }

        public override void Update (EntitySystem entitySystem, float dt) {
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
            //collider.position += dirNormalized * dt * 200;

            bool collided = false;
            for (int axis = 0; axis < 2; axis++) {
                collider.position[axis] += dirNormalized[axis] * dt * 200;

                for (int i = 0; i < walls.Count; i++) {
                    if (Collision.CheckIntersection (walls[i].GetCollider (), collider)) {
                        collided = true;
                        collider.AlignEdge (walls[i].GetCollider (), axis);
                    }
                }
            }
            /*for (int i = 0; i < walls.Count; i++) {
                if (collider.CollideAndCorrectAnyAxis (walls[i].GetCollider ())) {
                    collided = true;
                }
            }*/
            renderer.color = collided ? Color.Red : Color.White;
        }
    }
}
