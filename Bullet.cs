using System;
using System.Numerics;
using Engine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsetSharpEngine {
    public class Bullet : Entity {
        public BoxCollider collider;
        public RectangleRenderer renderer;
        public int damage;

        public Vector2 velocity = Vector2.Zero;

        public float lifetime = 2f;

        private List<Combatent> targets;
        private List<EntityWithTagAndBox> walls;
        public Tags tagMask = Tags.PlayerAligned | Tags.Character;

        


        public Bullet () {
            collider = new BoxCollider (Vector2.Zero, Vector2.One * 16);
            renderer = new RectangleRenderer (collider, (int)Layer.Bullets);
            components = new object[]{collider, renderer};
            damage = 1;
        }

        protected override void OnSetup (EntitySystem es) {
            var targetHolder = es.GetEntityCollection<EntitiesByTag<EntityWithTagAndBox, Tags>> ()!;
            walls = targetHolder.GetEntitiesByTags (Tags.Wall)!;
            
            targets = es.GetEntityCollection<EntitiesByTag<Combatent, Tags>> ()!.GetEntitiesByTags(tagMask)!;
        }

        public override void Update (EntitySystem es, float dt) {
            lifetime -= dt;
            if (lifetime < 0) Destroy ();
            collider.position += velocity * dt;

            for (int i = 0; i < targets.Count; i++) {
                //Console.WriteLine (i);
                if (Collision.CheckIntersection (targets[i].GetCollider(), collider)) {
                    targets[i].GetHealthManager ().Hit(damage);
                    Destroy ();
                }
            }

            for (int i = 0; i < walls.Count; i++) {
                if (Collision.CheckIntersection (walls[i].GetCollider(), collider)) {
                    Destroy ();
                    break;
                }
            }
        }
    }
}
