using OpenRSCS.utils;

namespace OpenRSCS.cache.type {
    public interface IType {

        void initialize(ByteBuffer buffer);

        int getID();

    }
}
