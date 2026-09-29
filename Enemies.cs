using Engine;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsetSharpEngine {
    public class HealthManager {
        public int health;
        public int maxHealth = 20;

        public HealthManager (int health = 30) {
            this.health = health;
            this.maxHealth = health;
        }

        public void Hit (int damage) {
            health = Math.Max (0, health - damage);
        }
    }

    public class Enemy: Entity {
        HealthManager healthManager = new HealthManager ();
        public BoxCollider collider = new BoxCollider(Vector2.Zero, new Vector2(32));
        RectangleRenderer renderer;

        Vector2 velocity = Vector2.Zero;
        float movementTimer = 0;
        float movementIntervol = 2;
        List<EntityWithTagAndBox> walls;
        public Enemy () {
            renderer = new RectangleRenderer (collider, (int)Layer.Characters);
            
            components = new object[]{
                collider,
                healthManager,
                renderer,
                new TagManager<Tags> (Tags.EnemyAligned | Tags.Character)
            };
        }

        protected override void OnSetup (EntitySystem es) {
            walls = es.GetEntityCollection<EntitiesByTag<EntityWithTagAndBox, Tags>> ()!.GetEntitiesByTags(Tags.Wall)!;
        }

        public override void Update (EntitySystem es, float dt) {
            movementTimer -= dt;
            if (movementTimer < 0) {
                var direction = new Vector2 (Random.Shared.NextSingle (), Random.Shared.NextSingle ());
                direction = (direction * 2) - Vector2.One;
                direction = Vector2.Multiply (direction, Vector2.Abs (direction));

                direction = Vector2.Normalize (direction);
                velocity = 20 * direction;
                movementTimer = movementIntervol;
            }

            for (int axis = 0; axis < 2; axis++) {
                collider.position[axis] += velocity[axis] * dt;
                for (int i = 0; i < walls.Count; i++) {
                    if (Collision.CheckIntersection (collider, walls[i].GetCollider ())) {
                        collider.AlignEdge (walls[i].GetCollider (), axis);
                    }
                }
            }

            if (healthManager.health <= 0) {
                Destroy ();
            }
        }

    }
    
}
