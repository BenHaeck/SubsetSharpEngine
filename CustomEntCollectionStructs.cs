using Engine;

namespace SubsetSharpEngine {
    public struct EntityWithTagAndBox: ISingleEntityInfo, ITagManagerContainer<Tags> {
        // values
        private Entity entity;
        private BoxCollider collider;
        private TagManager<Tags> tagManager;

        // initialization
        public bool PopulateFrom (Entity entity) {
            this.entity = entity;
            collider = entity.GetComponent<BoxCollider> ()!;
            tagManager = entity.GetComponent<TagManager<Tags>> ()!;

            return collider != null && tagManager!=null;
        }

        // Getters
        public Entity GetEntity () { return entity; }
        public TagManager<Tags> GetTagManager () { return tagManager; }
        public BoxCollider GetCollider () { return collider; }
    }

    public struct Combatent: ISingleEntityInfo, ITagManagerContainer<Tags> {
        private Entity entity;
        public Entity GetEntity () { return entity; }

        private BoxCollider collider;
        public BoxCollider GetCollider () { return collider; }
        private HealthManager healthManager;
        public HealthManager GetHealthManager () { return healthManager; }
        private TagManager<Tags> tagManager;
        public TagManager<Tags> GetTagManager () { return tagManager; }

        public bool PopulateFrom (Entity entity) {
            this.entity = entity;
            collider = entity.GetComponent<BoxCollider> ()!;
            healthManager = entity.GetComponent<HealthManager> ()!;
            var tagManager = entity.GetComponent<TagManager<Tags>> ();
            
            if (tagManager == null)return false;
            if ((tagManager.tag & Tags.Character) == 0) return false;
            this.tagManager = tagManager;
            Console.WriteLine ($"{entity} added: {collider != null && healthManager != null && tagManager != null}");
            return collider != null && healthManager != null && tagManager != null;
        }

    }
}