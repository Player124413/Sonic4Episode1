using System.Collections.Generic;

internal partial class AppMain
{
    internal class ArrayPoolFast<T> where T : new()
    {
#if !NETCOREAPP
        private List<T[]> freeArrays_ = new List<T[]>();
        private List<T[]> usedArrays_ = new List<T[]>();
#endif

        public T[] AllocArray(int size)
        {
#if !NETCOREAPP
            int index1 = -1;
            for (int index2 = 0; index2 < this.freeArrays_.Count; ++index2)
            {
                if (size <= this.freeArrays_[index2].Length)
                {
                    index1 = index2;
                    break;
                }
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

        public int GetFreeCount()
        {
#if !NETCOREAPP
            return this.freeArrays_.Count;
#endif
            // always something free
            return 1;
        }
    }
}
