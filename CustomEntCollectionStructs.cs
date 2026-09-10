using Engine;

namespace SubsetSharpEngine {
    public struct EntityWithTagAndBoxCollider: ISingleEntityInfo, ITagManagerContainer<Tags> {
        private Entity entity;
        private BoxCollider collider;
        private TagManager<Tags> tagManager;

        public bool PopulateFrom (Entity entity) {
            this.entity = entity;
            collider = entity.GetComponent<BoxCollider> ()!;
            tagManager = entity.GetComponent<TagManager<Tags>> ()!;

            return collider != null && tagManager!=null;
        }

        public Entity GetEntity () { return entity; }
        public TagManager<Tags> GetTagManager () { return tagManager; }
        public BoxCollider GetCollider () { return collider; }
    }
}