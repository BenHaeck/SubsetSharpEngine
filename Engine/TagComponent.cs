
namespace Engine {

    public class TagManager<TTagType> where TTagType : Enum, IConvertible{
        public readonly TTagType tag;

        public TagManager (TTagType tag) {
            this.tag = tag;
        }
    }

    public struct EntitiesWithTagManager <TTagType>: ISingleEntityInfo, ITagManagerContainer<TTagType> where TTagType : Enum, IConvertible {
        private Entity entity;
        private TagManager<TTagType> tagManager;

        public bool PopulateFrom (Entity entity) {
            this.entity = entity;
            tagManager = entity.GetComponent<TagManager<TTagType>>()!;
            return tagManager != null;
        }

        public Entity GetEntity () {
            return entity;
        }

        public TagManager<TTagType> GetTagManager () {
            return tagManager;
        }
    }

    public class EntitiesByTagCollection <TEntityInfo, TTagType> : EntityCollection
    where TTagType : Enum, IConvertible
    where TEntityInfo : ISingleEntityInfo, ITagManagerContainer<TTagType>, new(){
        public (TTagType tag, List<TEntityInfo> lists)[] entitiesByTags;

        public EntitiesByTagCollection (TTagType[] acceptedTagSets) {
            acceptedTagSets = (TTagType[])acceptedTagSets.Clone ();
            
            Utils.Sort (acceptedTagSets, static (TTagType tag) => (double)(tag.ToInt32(null)));
            entitiesByTags = new (TTagType tag, List<TEntityInfo> list)[acceptedTagSets.Length];
            for (int i = 0; i < acceptedTagSets.Length; i++) {
                entitiesByTags[i] = (acceptedTagSets[i], new List<TEntityInfo> ());
                Console.WriteLine (acceptedTagSets[i]+ " Initialized");
            }
        }

        public List<TEntityInfo>? GetEntitiesByTags (int tagSet) {
            int? idxNullable = Utils.BinSearch (entitiesByTags, tagSet, static ((TTagType tag, List<TEntityInfo>) tagCollection) => tagCollection.tag.ToInt32(null));
            if (idxNullable.HasValue) {
                int idx = idxNullable.Value;
                return entitiesByTags[idx].lists;
            }
            return null;
        }

        // callbacks
        public override bool TryAddEntity (Entity entity) {
            var entityInfo = new TEntityInfo ();
            var success = entityInfo.PopulateFrom (entity);
            if (!success) {
                
                return false;
            }
            success = false;
            int entityBitMask = entityInfo.GetTagManager ().tag.ToInt32 (null);
            for (int i = 0; i < entitiesByTags.Length; i++) {
                int collectionBitMask = entitiesByTags[i].tag.ToInt32(null);
                if ((entityBitMask & collectionBitMask) == collectionBitMask) {
                    success = true;
                    entitiesByTags[i].lists.Add (entityInfo);
                }
            }

            return success;
        }

        public override void Clear () {
            for (int i = 0; i < entitiesByTags.Length; i++) {
                entitiesByTags[i].lists.Clear ();
            }
        }

        public override void Clean() {
            for (int i = 0; i < entitiesByTags.Length; i++) {
                Entity.CleanList (entitiesByTags[i].lists);
            }
        }
    }

    public interface ITagManagerContainer<TTagType> where TTagType : Enum, IConvertible {
        public TagManager<TTagType> GetTagManager ();
    }
}