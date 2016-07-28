namespace OpenRSCS.cache.type {
    public interface ITypeList<T> where T : IType {

        void init(Cache cache);

        T list(int id);

        void print();

    }
}
