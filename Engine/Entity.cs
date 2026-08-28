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
        public void Setup () {
            OnSetup ();
        }

        protected virtual void OnSetup () {
            
        }

        public virtual void Update (float dt) {

        }

        public T? GetComponent<T> (bool allowSubclasses = false) where T: class {
            return Utils.GetDerived<T, object> (components, allowSubclasses);
        }

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

    public interface ISingleEntityContainer {
        // Gets the entity object, then returns it.
        Entity GetEntity ();
    }

    public interface ISingleEntityInfo: ISingleEntityContainer {
        bool PopulateFrom (Entity entity);
    }
}
