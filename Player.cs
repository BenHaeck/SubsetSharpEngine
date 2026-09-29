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
        public HealthManager healthManager = new HealthManager (30);
        public readonly BoxCollider collider = new BoxCollider (Vector2.Zero, new Vector2(32));
        public readonly RectangleRenderer renderer;
        public List<EntityWithTagAndBox> walls;

        public Vector2 mousePos = Vector2.One;

        private float fireRecov = 0;

        public const float FIRE_INTERVOL = 0.5f;
        public const float MOVEMENT_SPEED = 100;
        public const float BULLET_SPEED = 1200;

        public Player () {
            renderer = new RectangleRenderer (collider, (int)Layer.Characters);
            components = new object[] {
                collider,
                renderer,
                healthManager,
                new TagManager<Tags>(Tags.Character | Tags.PlayerAligned)
            };
        }

        protected override void OnSetup (EntitySystem entitySystem) {
            walls = entitySystem.GetEntityCollection<EntitiesByTag<EntityWithTagAndBox, Tags>> ()!.GetEntitiesByTags(Tags.Wall)!;
        }

        public override void Update (EntitySystem entitySystem, float dt) {
            fireRecov -= dt;
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

            if (Raylib.IsMouseButtonDown (MouseButton.Left) && fireRecov <= 0) {
                fireRecov = FIRE_INTERVOL;
                var bullet = new Bullet ();
                bullet.collider.position = collider.position;
                bullet.velocity = Vector2.Normalize(mousePos - collider.position) * BULLET_SPEED;
                bullet.tagMask = Tags.EnemyAligned | Tags.Character;
                bullet.damage = 10;
                entitySystem.AddEntity (bullet);
            }

            bool collided = false;
            for (int axis = 0; axis < 2; axis++) {
                collider.position[axis] += dirNormalized[axis] * dt * MOVEMENT_SPEED;

                for (int i = 0; i < walls.Count; i++) {
                    if (Collision.CheckIntersection (walls[i].GetCollider (), collider)) {
                        collider.AlignEdge (walls[i].GetCollider (), axis);
                        collided = true;
                    }
                }
            }
            /*for (int i = 0; i < walls.Count; i++) {
                if (collider.CollideAndCorrectAnyAxis (walls[i].GetCollider ())) {
                    collided = true;
                }
            }*/
            renderer.color = collided ? Color.Red : Color.Blue;
        }
    }
}
