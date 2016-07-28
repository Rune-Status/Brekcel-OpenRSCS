using System.Collections.Generic;

namespace OpenRSCS.cache.utils {
    //TODO: XTEAKey Stuff.
    public class XTEAManager {

        private static readonly Dictionary<int, int[]> maps = new Dictionary<int, int[]>();
        private static readonly Dictionary<int, int[]> tables = new Dictionary<int, int[]>();
        public static readonly int[] NULL_KEYS = new int[4];

        static XTEAManager() { }

        public static int[] lookupTable(int id) {
            int[] keys = tables[id];
            if(keys == null)
                return NULL_KEYS;

            return keys;
        }

        public static int[] lookupMap(int id) {
            int[] keys = maps[id];
            if(keys == null)
                return NULL_KEYS;

            return keys;
        }

    }
}
