using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRSCS.utils;

namespace OpenRSCS.cache.resources.sprite {
    public class Texture {

        public readonly int[] ids;
        public Texture(ByteBuffer buf) {
            buf.getShort();
            buf.get();
            ids = new int[buf.get()];
            for(int i = 0; i < ids.Length; i++) {
                ids[i] = buf.getUShort();
            }
        }

    }
}
