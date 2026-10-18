// See https://aka.ms/new-console-template for more information
using Raylib_cs;
using Engine;
using System.Numerics;

using SubsetSharpEngine;

public class TestLevelEditor: LevelEditor {
    public TestLevelEditor () {
        validTiles = new char[]{'#', 'P', 'B'};
        //bp.CreateTile ('P', new Vector2 (0, 0));
        //bp.CreateTile ('#', new Vector2 (64, 64));
        //bp.CreateTile ('#', new Vector2 (64, 64+32));

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
        //Run ();

        RegionSet1D span = new RegionSet1D ();
        Console.WriteLine (span);
        span.SetRegionActivity (10, 20, true);
        span.SetRegionActivity (40, 50, true);
        Console.WriteLine (span);
        span.SetRegionActivity (15, 30, true);
        span.SetRegionActivity (35, 45, true);
        Console.WriteLine (span);
        span.SetRegionActivity (37, 40, false);
        span.SetRegionActivity (15, 25, false);
        Console.WriteLine (span);

        var span2 = (RegionSet1D)span.Clone ();
        Console.WriteLine (span);

        Raylib.InitWindow (600, 400, "?");
        AAMesh ev = new AAMesh ();
        ev.AppendRectangle (new Vector2 (32), new Vector2 (64*1.5f), true);

        ev.AppendRectangle (new Vector2 (32, 32+32*3), new Vector2 (64, 64+32*3.5f), true);
        ev.AppendRectangle (new Vector2 (128, 32+32*1.5f), new Vector2 (128+32, 64+32*2f), true);

        ev.AppendRectangle (new Vector2 (64, 32), new Vector2 (128, 48), true);
        //ev.AppendRectangle (new Vector2 (64, 32+32*2), new Vector2 (64+16, 64+32*2.5f), true);
        ev.AppendRectangle (new Vector2 (32, 128+64), new Vector2 (128+32, 128+128), true);
        ev.AppendRectangle (new Vector2 (48, 8), new Vector2 (128+32, 16), true);

        ev.AppendRectangle (new Vector2 (32+8, 32+8), new Vector2 (32 + 8*2, 256-25), false);

        Texture2D tex = Raylib.LoadTexture ("assets/Brick.png");
        Raylib.SetTextureFilter (tex, TextureFilter.Point);
        Raylib.SetTextureWrap (tex, TextureWrap.Repeat);

        var coll = new BoxCollider (Vector2.Zero, Vector2.One * 24);
        //Raylib.DrawTexturePro (tex, new Rectangle (0, 0, 32, 32), new Rectangle (32, 32, 32, 32), Vector2.Zero, 0, Color.White);
        var wallCollider = new BoxCollider (Vector2.Zero, Vector2.One);
        while (!Raylib.WindowShouldClose ()) {
            Raylib.BeginDrawing ();
            Raylib.ClearBackground (Color.DarkGray);
            ev.DrawTextured (tex, 0.5f, Color.SkyBlue);

            float dt = Raylib.GetFrameTime ();
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
            for (int axis = 0; axis < 2; axis++) {
                coll.position[axis] += dir[axis] * dt * 60;
                Console.WriteLine ("Loop");
                for (int region = ev.GetRegionIdx(coll.position.X - coll.size.X*2); region < ev.RegionCount - 2; region++) {
                    //Console.WriteLine ($"{region} {ev.RegionCount}");
                    Console.WriteLine ("\tRegion: " + region);
                    (float lower, float upperBound, RegionSet1D subRegionObject) = ev.GetRegionByIdx (region);
                    //Console.WriteLine ($"{coll.position.X + coll.size.X*1.2f} {lower}");
                    if (coll.position.X + coll.size.X*1.2f <= lower) break;
                    for (int subRegion = subRegionObject.GetRegionIdx(coll.position.Y - coll.size.Y*2); subRegion < ev.SubRegionCount (region) - 1; subRegion++) {
                        Console.WriteLine ("\t\tSubRegion: " + subRegion);

                        if (!subRegionObject.IsRegionActive (subRegion)) continue;
                        ev.SubRegionBox (wallCollider, region, subRegion);
                        if (coll.position.Y + coll.size.Y*1.2f < wallCollider.position.Y - wallCollider.size.Y) break;
                        if (Collision.CheckIntersection (coll, wallCollider)) {
                            coll.AlignEdge (wallCollider, axis);
                        }
                    }
                }
            }
            Raylib.DrawRectangleV (coll.position - coll.size / 2, coll.size, Color.Red);
            Raylib.EndDrawing ();

        }



        

    }

    

    public static void Run () {


        /*var entitySystem = new EntitySystem ();

        var renderers = new Renderer2DCollection (3);
        // p or e for play or edit

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
        entitySystem.AddEntity (enemy);*/
        char mode = 'p';


        GameStateSharedInfo info = new GameStateSharedInfo ();
        info.bp = new LevelBP ();
        

        var gameStates = new Dictionary<char, GameState> ();
        GameWorld gw = new GameWorld ();
        gameStates.Add ('W', gw);

        var levelEditor = new TestLevelEditor ();
        levelEditor.info = info;
        levelEditor.tileSize = new Vector2 (32);
        levelEditor.SetSelectedTile (0);
        levelEditor.AddTile (new Vector2 (128));
        gameStates.Add ('E', levelEditor);

        GameStateManager gsManager = new GameStateManager (info, gameStates);


        Raylib.InitWindow (600, 400, "Hello");
        Raylib.SetWindowState (ConfigFlags.ResizableWindow | ConfigFlags.MaximizedWindow | ConfigFlags.VSyncHint);
        var windowManager = new WindowManager (640, 480);
        info.display = windowManager;
        //RenderTexture2D screenTexture = Raylib.LoadRenderTexture (640, 480);
        while (!Raylib.WindowShouldClose ()) {
            Raylib.BeginDrawing ();
            /*var screenTextureSize = new Vector2 (screenTexture.Texture.Width, screenTexture.Texture.Height);
            var windowSize = new Vector2 (Raylib.GetRenderWidth (), Raylib.GetRenderHeight ());
            var screenScale = Utils.FitInside (screenTextureSize, windowSize);
            var outputSize = screenTextureSize * screenScale;

            var mousePos = Raylib.GetMousePosition () + (outputSize - windowSize) / 2;
            mousePos /= screenScale;
            player.mousePos = mousePos;*/
            windowManager.Update ();
            if (Raylib.IsKeyPressed (KeyboardKey.O)) {
                if (mode == 'e') {
                    mode = 'p';
                    gsManager.updateOrder = "W";
                    gsManager.drawOrder = "W";
                }
                else {
                    mode = 'e';
                    gsManager.updateOrder = "E";
                    gsManager.drawOrder = "E";


                }
            }
            info.display.Update ();

            Raylib.BeginTextureMode (windowManager.renderTexture);
            Raylib.ClearBackground (Color.DarkGray);
            gsManager.Update ();
            gsManager.Draw ();
            Raylib.EndTextureMode ();


            /*switch (mode) {
                case 'p':
                    Raylib.BeginTextureMode (windowManager.renderTexture);
                    entitySystem.Update (Raylib.GetFrameTime ());
                    Player.mousePos = windowManager.InnerMousePos;
                    Raylib.ClearBackground (Color.DarkGray);
                    renderers.DrawAll ();
                    Raylib.DrawRectangleV (windowManager.InnerMousePos, new Vector2 (8), Color.White);
                    Raylib.EndTextureMode ();
                    break;

                case 'e':
                    Raylib.BeginTextureMode (windowManager.renderTexture);
                    Raylib.ClearBackground (Color.DarkGray);
                    levelEditor.mousePos = windowManager.InnerMousePos;
                    //levelEditor.Update (Raylib.GetFrameTime());
                    //levelEditor.DrawGameWorld ();
                    Raylib.EndTextureMode ();
                    break;
            }*/
            if (Raylib.IsKeyPressed(KeyboardKey.R)) {
                //entitySystem.Clear ();
                //CreateGameWorld (levelEditor.bp, entitySystem);
                gsManager.QueueLevelChange ();
            }

            Raylib.ClearBackground (Color.Black);

            //var outputSizeFliped = new Vector2 (outputSize.X, -outputSize.Y);

            /*Raylib.DrawTexturePro (screenTexture.Texture,
                new Rectangle (0, 0, screenTextureSize.X, -screenTextureSize.Y),
                new Rectangle(windowSize/2, outputSize),
                outputSize / 2, 0, Color.White
            );*/
            windowManager.Draw ();
            Raylib.DrawFPS (6, 6);
            Raylib.EndDrawing ();
        }
        
        Raylib.UnloadRenderTexture (windowManager.renderTexture);
        Raylib.CloseWindow ();
        Console.WriteLine (info.bp.TileCounts ());
        Console.WriteLine (info.bp);
        Console.WriteLine (windowManager);
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

