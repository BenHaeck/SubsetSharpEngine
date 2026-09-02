using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Entity: ISingleEntityContainer {
        private bool queueRemoval = false;
        public bool QueueRemoval => queueRemoval;
        protected object[]? components = null;
        public Entity () {}
        public Entity (object[] components) {
            this.components = components;
        }

        public void Setup () {
            OnSetup ();
        }

        // called when added
        protected virtual void OnSetup () {}

        // called every frame
        public virtual void Update (float dt) {}

        // returns the component of a certain type
        public T? GetComponent<T> (bool allowSubclasses = false) where T: class {
            return Utils.GetDerived<T, object> (components, allowSubclasses);
        }

        // queues the removal of this entity from every list/datas tructure
        public void Destroy () {
            queueRemoval = true;
        }


        // Returns itself. This is mostly useful for writting generic functions
        public Entity GetEntity () {
            return this;
        }

        // removes every entity queued for removal;
        public static bool CleanList<T> (List<T> entityContainerList) where T : ISingleEntityContainer {
            bool entityRemoved = false;
            for (int i = 0; i < entityContainerList.Count; i++) {
                var entity = entityContainerList[i].GetEntity ();
                if (entity.QueueRemoval) {
                    entityContainerList.RemoveAt (i);
                    entityRemoved = true;
                }
            }
            return entityRemoved;
        }

    }

    //  allows a structure that contains an entity to be treated like an entity
    public interface ISingleEntityContainer {
        // Gets the entity object, then returns it.
        Entity GetEntity ();
    }

    // an interface for extracting information from an entity
    // Mainly for Entity Collections.
    public interface ISingleEntityInfo: ISingleEntityContainer {
        // Tries to get information from an entity. Returns true if successful
        bool PopulateFrom (Entity entity);
    }
}
