using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine {
    public class EntitySystem {
        
        private List<Entity> allEntities = new List<Entity> ();

        public EntityCollection[] entityCollections = new EntityCollection[0];

        // adds an entity to the entity system
        public void AddEntity (Entity entity) {
            allEntities.Add (entity);
            entity.Setup ();
            for (int i = 0; i < entityCollections.Length; i++) {
                entityCollections[i].TryAddEntity (entity);
            }
        }

        // updates the entity system
        public void Update (float dt) {
            // updates all entities.
            for (int i = 0; i < allEntities.Count; i++) {
                allEntities[i].Update (dt);
            }

            // cleans all lists of entities queued for removal
            bool entityRemoved = Entity.CleanList (allEntities);
            if (entityRemoved) {
                for (int i = 0; i < entityCollections.Length; i++) {
                    entityCollections[i].Clean ();
                }
            }
        }

    }

    

    public abstract class EntityCollection {
        // if an entity follows the rules of the collection, adds it
        public abstract bool TryAddEntity (Entity entity);

        // removes every entity from the collection
        public abstract void Clear ();

        // removes entities that were queued for removal
        public abstract void Clean ();
    }

    public struct EntityComponentPair <T> : ISingleEntityInfo where T : class {
        public Entity entity;
        public T component;

        // gets the entity. This exists so certain generic functions can
        // treat it like an entity. And perform any actions on it that could be performed on a structure of entities
        public Entity GetEntity () {
            return entity;
        }

        // Gets the component, then Caches it with the entity
        public bool PopulateFrom (Entity entity) {
            this.entity = entity;
            var component = entity.GetComponent<T> ();
            if (component == null) return false;
            this.component = component;
            return true;
        }
    }
    public class EntitiesWithComponent<T>: EntitiesWithInfo <EntityComponentPair<T>> where T : class {}

    public class EntitiesWithInfo<T> : EntityCollection where T : ISingleEntityInfo, new() {
        private List<T> entityAndInfo = new List<T> ();

        public int Count => entityAndInfo.Count;

        public T GetInfo (int index) {
            return entityAndInfo[index];
        }

        // if an entity possesses the right information, add it to the collection
        public override bool TryAddEntity (Entity entity) {
            T info = new T();
            bool success = info.PopulateFrom (entity);

            if (success) {
                entityAndInfo.Add (info);
                return true;
            }
            return false;
        }

        // removes every entity
        public override void Clear () {
            entityAndInfo.Clear ();
        }

        // removes any entity queued for removal
        public override void Clean () {
            Entity.CleanList (entityAndInfo);
        }
    }
}
