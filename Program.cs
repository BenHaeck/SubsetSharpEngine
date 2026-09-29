// See https://aka.ms/new-console-template for more information
using Raylib_cs;
using Engine;
using System.Numerics;

using SubsetSharpEngine;

public class TestLevelEditor: LevelEditor {
    public TestLevelEditor () {
        validTiles = new char[]{'#', 'P', 'B'};
        bp.CreateTile ('P', new Vector2 (0, 0));
        bp.CreateTile ('#', new Vector2 (64, 64));
        bp.CreateTile ('#', new Vector2 (64, 64+32));

    }

    public override void DrawTile (char tileType, Vector2 pos) {
        switch (tileType) {
            case '#':
                Raylib.DrawRectangleV (pos - tileSize * 0.5f, Vector2.One * 32, Color.White);
                break;
            case 'P':
                Raylib.DrawRectangleV (pos - tileSize * 0.5f, Vector2.One * 32, Color.Blue);
                break;
            case 'B':
                Raylib.DrawRectangleV (pos - tileSize * 0.5f, Vector2.One * 32, Color.Red);
                break;
        }
    }
}

public static class Program {

    public static int ToInt<T> (T v) where T : Enum, IConvertible {
        return v.ToInt32 (null);
    }
    public static void Main () {
        Run ();
        
        /*var entities = new EntitiesByTagCollection<EntitiesWithTagManager<Tags>, Tags> (new Tags[]{
            Tags.Wall,
            Tags.Wall | Tags.TransparentWall,
            Tags.Enemy
        });

        var testEntities = new Entity[] {
            new Entity(new object[]{new TagManager<Tags>((Tags.Wall))}),
            new Entity(new object[]{new TagManager<Tags>((Tags.Wall | Tags.TransparentWall))}),
            new Entity(new object[]{new TagManager<Tags>((Tags.TransparentWall))}),
            new Entity(new object[]{new TagManager<Tags>((Tags.Enemy | Tags.TransparentWall))}),
            new Entity(new object[]{new TagManager<Tags>((Tags.None))}),
            new Entity(new object[]{new TagManager<Tags>((Tags.all))}),
        };
        for (int i = 0; i < testEntities.Length; i++) {
            entities.TryAddEntity (testEntities[i]);
        }

        for (int i = 0; i < entities.entitiesByTags.Length; i++) {
            Console.WriteLine ((Tags)entities.entitiesByTags[i].tag + " " + entities.entitiesByTags[i].lists.Count);
        }
        
        Console.WriteLine (ToInt(Tags.Item | Tags.Wall));*/
        
    }

    public static void Run () {
        var entitySystem = new EntitySystem ();

        var levelEditor = new TestLevelEditor ();
        levelEditor.tileSize = new Vector2 (32);
        levelEditor.SetSelectedTile (0);
        levelEditor.AddTile (new Vector2(128));
        var renderers = new Renderer2DCollection (3);

        // p or e for play or edit
        char mode = 'p';

        entitySystem.entityCollections = new EntityCollection[]{
            renderers,
            new EntitiesByTag<EntityWithTagAndBox, Tags> (new Tags[] {
                Tags.Wall,
                Tags.Character | Tags.EnemyAligned,
            }),

            new EntitiesByTag<Combatent, Tags>(new Tags[]{
                Tags.Character | Tags.EnemyAligned,
                Tags.PlayerAligned | Tags.Character,
            })
        };


        var wallPositions = new Vector2[] { new Vector2 (64, 64+32), new Vector2 (64, 128) };
        for (int i = 0; i < wallPositions.Length; i++) {
            var wallColl = new BoxCollider (wallPositions[i], new Vector2 (32));
            var wall = new Entity (new object[] {
                wallColl,
                new RectangleRenderer(wallColl, (int)Layer.Ground),
                new TagManager<Tags>(Tags.Wall),
            });
            entitySystem.AddEntity (wall);
        }

        var player = new Player ();
        player.collider.position = new Vector2 (64, 64);
        entitySystem.AddEntity (player);

        var enemy = new Enemy ();
        enemy.collider.position = new Vector2 (128);
        entitySystem.AddEntity (enemy);
        

        Raylib.InitWindow (600, 400, "Hello");
        Raylib.SetWindowState (ConfigFlags.ResizableWindow | ConfigFlags.MaximizedWindow | ConfigFlags.VSyncHint);
        RenderTexture2D screenTexture = Raylib.LoadRenderTexture (640, 480);
        while (!Raylib.WindowShouldClose ()) {
            Raylib.BeginDrawing ();
            var screenTextureSize = new Vector2 (screenTexture.Texture.Width, screenTexture.Texture.Height);
            var windowSize = new Vector2 (Raylib.GetRenderWidth (), Raylib.GetRenderHeight ());
            var outputSize = screenTextureSize * Utils.FitInside (screenTextureSize, windowSize);

            var mousePos = Raylib.GetMousePosition () + (outputSize - windowSize) / 2;
            mousePos /= Utils.FitInside (screenTextureSize, windowSize);
            player.mousePos = mousePos;
            if (Raylib.IsKeyPressed (KeyboardKey.O)) {
                if (mode == 'e') {
                    mode = 'p';
                }
                else {
                    mode = 'e';
                }
            }
            switch (mode) {
                case 'p':
                    Raylib.BeginTextureMode (screenTexture);
                    entitySystem.Update (Raylib.GetFrameTime ());

                    Raylib.ClearBackground (Color.DarkGray);
                    renderers.DrawAll ();
                    Raylib.DrawRectangleV (mousePos, new Vector2 (8), Color.White);
                    Raylib.EndTextureMode ();
                    break;

                case 'e':
                    Raylib.BeginTextureMode (screenTexture);
                    Raylib.ClearBackground (Color.DarkGray);
                    levelEditor.mousePos = mousePos;
                    levelEditor.Update (Raylib.GetFrameTime());
                    levelEditor.DrawGameWorld ();
                    Raylib.EndTextureMode ();
                    break;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.R)) {
                entitySystem.Clear ();
                CreateGameWorld (levelEditor.bp, entitySystem);
            }

            Raylib.ClearBackground (Color.Black);

            //var outputSizeFliped = new Vector2 (outputSize.X, -outputSize.Y);

            Raylib.DrawTexturePro (screenTexture.Texture,
                new Rectangle (0, 0, screenTextureSize.X, -screenTextureSize.Y),
                new Rectangle(windowSize/2, outputSize),
                outputSize / 2, 0, Color.White
            );
            Raylib.DrawFPS (6, 6);
            Raylib.EndDrawing ();
        }
        
        Raylib.UnloadRenderTexture (screenTexture);
        Raylib.CloseWindow ();
        Console.WriteLine (levelEditor.bp.TileCounts ());
        Console.WriteLine (levelEditor.bp);
    }


    public static void CreateGameWorld (LevelBP bp, EntitySystem es) {
        List<Vector2> tilePositions = bp.GetTilesByType ('#');
        for (int i = 0; i < tilePositions.Count; i++) {
            var wallColl = new BoxCollider (tilePositions[i], new Vector2 (32));
            var wall = new Entity (new object[] {
                wallColl,
                new RectangleRenderer(wallColl, (int)Layer.Ground),
                new TagManager<Tags>(Tags.Wall),
            });
            es.AddEntity (wall);
        }
        Player player = new Player ();
        player.collider.position = bp.GetTilesByType ('P')[0];
        es.AddEntity (player);
        tilePositions = bp.GetTilesByType ('B');
        for (int i = 0; i < tilePositions.Count; i++) {
            Enemy enemy = new Enemy ();
            enemy.collider.position = tilePositions[i];
            es.AddEntity (enemy);
        }
        
    }
}

