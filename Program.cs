// See https://aka.ms/new-console-template for more information
using Raylib_cs;
using Engine;
using System.Numerics;

using SubsetSharpEngine;

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

        var physicsEntities = new EntitiesWithComponent<BoxCollider> ();
        var renderers = new Renderer2DCollection ();

        entitySystem.entityCollections = new EntityCollection[]{
            physicsEntities, renderers, new EntitiesByTagCollection<EntityWithTagAndBoxCollider, Tags> (new Tags[] {
                Tags.Wall
            })
        };

        var player = new Player ();
        player.collider.position = new Vector2 (64, 64);

        var wallPositions = new Vector2[] { new Vector2 (64, 64+32), new Vector2 (64, 128) };
        for (int i = 0; i < wallPositions.Length; i++) {
            var wallColl = new BoxCollider (wallPositions[i], new Vector2 (32));
            var wall = new Entity (new object[] {
                wallColl,
                new RectangleRenderer(wallColl, 0),
                new TagManager<Tags>(Tags.Wall),
            });
            entitySystem.AddEntity (wall);
        }

        entitySystem.AddEntity (player);


        Raylib.InitWindow (600, 400, "Hello");
        //Raylib.SetWindowState (ConfigFlags.VSyncHint);
        while (!Raylib.WindowShouldClose ()) {
            Raylib.BeginDrawing ();
            Raylib.ClearBackground (Color.DarkGray);
            
            entitySystem.Update (Raylib.GetFrameTime ());

            renderers.DrawAll ();

            Raylib.DrawFPS (6, 6);
            Raylib.EndDrawing ();
        }

        Raylib.CloseWindow ();
    }
}

