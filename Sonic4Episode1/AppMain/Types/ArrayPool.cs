using System.Collections.Generic;
using System.Drawing;

internal partial class AppMain
{
    internal class ArrayPool<T> where T : new()
    {
#if !NETCOREAPP
        private List<T[]> freeArrays_ = new List<T[]>();
        private List<T[]> usedArrays_ = new List<T[]>();
#endif

        public T[] AllocArray(int size)
        {
#if !NETCOREAPP
            int index1 = -1;
            int num = this.freeArrays_.Count > 0 ? this.freeArrays_[0].Length : -1;
            for (int index2 = 0; index2 < this.freeArrays_.Count; ++index2)
            {
                int length = this.freeArrays_[index2].Length;
                if (length >= size && length <= num)
                    index1 = index2;
            }
            T[] objArray;
            if (index1 >= 0)
            {
                objArray = this.freeArrays_[index1];
                this.freeArrays_.RemoveAt(index1);
            }
            else
                objArray = new T[size];
            this.usedArrays_.Add(objArray);
            return objArray;
#else
            return System.Buffers.ArrayPool<T>.Shared.Rent(size);
#endif
        }

        public void ReleaseArray(T[] array)
        {
#if !NETCOREAPP
            this.freeArrays_.Add(array);
            this.usedArrays_.Remove(array);
#else
            System.Buffers.ArrayPool<T>.Shared.Return(array);
#endif
        }

        public void ReleaseUsedArrays()
        {
#if !NETCOREAPP
            this.freeArrays_.AddRange(usedArrays_);
            this.usedArrays_.Clear();
#endif
        }

        public void Clear()
        {
#if !NETCOREAPP
            this.freeArrays_.Clear();
            this.usedArrays_.Clear();
#endif
        }
    }
}
