using Engine;
using System.Numerics;
namespace SubsetSharpEngine {
    public class GameWorld : GameState {
        EntitySystem entitySystem;
        Player player;

        Renderer2DCollection renderers;
        public WindowManager window;

        public GameWorld () {
            entitySystem = new EntitySystem ();

            renderers = new Renderer2DCollection (3);
            // p or e for play or edit

            entitySystem.entityCollections = new EntityCollection[]{
            renderers,
            new EntitiesByTag<EntityWithTagAndBox, Tags> (new Tags[] {
                Tags.Wall,
            }),

            new EntitiesByTag<Combatent, Tags>(new Tags[]{
                Tags.Character | Tags.EnemyAligned,
                Tags.PlayerAligned | Tags.Character,
            })
        };
        }

        public override void LoadLevel (LevelBP bp) {
            entitySystem.Clear ();
            List<Vector2> tilePositions = bp.GetOrCreateTileType ('#');
            for (int i = 0; i < tilePositions.Count; i++) {
                var wallColl = new BoxCollider (tilePositions[i], new Vector2 (32));
                var wall = new Entity (new object[] {
                    wallColl,
                    new RectangleRenderer(wallColl, (int)Layer.Ground),
                    new TagManager<Tags>(Tags.Wall),
                });
                entitySystem.AddEntity (wall);
            }

            player = new Player ();
            player.windowManager = info.display;
            player.collider.position = bp.GetOrCreateTileType ('P')[0];
            entitySystem.AddEntity (player);


            tilePositions = bp.GetOrCreateTileType ('B');
            for (int i = 0; i < tilePositions.Count; i++) {
                Enemy enemy = new Enemy ();
                enemy.collider.position = tilePositions[i];
                entitySystem.AddEntity (enemy);
            }
        }

        public override void Update (GameStateManager manager, float dt) {
            //Console.WriteLine ("A");
            entitySystem.Update (dt);
            
        }

        public override void Draw (GameStateManager manager) {
            renderers.DrawAll ();
        }
    }
}