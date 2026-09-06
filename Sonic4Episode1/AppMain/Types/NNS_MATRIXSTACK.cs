using System.Collections.Generic;

internal partial class AppMain
{
    internal class NNS_MATRIXSTACK
    {
        private readonly NNS_MATRIX identity = NNS_MATRIX.CreateIdentity();
        private List<NNS_MATRIX> stack;
        private NNS_MATRIX invert;

        public NNS_MATRIXSTACK(uint uiSize)
        {
            this.stack = new List<NNS_MATRIX>((int)uiSize);
        }

        public NNS_MATRIXSTACK()
        {
            this.stack = new List<NNS_MATRIX>(16);
        }

        public void push(NNS_MATRIX matrix)
        {
            this.invert = null;
            this.stack.Add(matrix);
        }

        public NNS_MATRIX pop()
        {
            this.invert = null;
            int index = this.stack.Count - 1;
            NNS_MATRIX nnsMatrix = this.stack[index];
            this.stack.RemoveAt(index);
            return nnsMatrix;
        }

        public NNS_MATRIX get()
        {
            return this.stack.Count == 0 ? this.identity : this.stack[this.stack.Count - 1];
        }

        // copies, the C++ stack holds values. storing the caller's matrix here hands it to the
        // matrix pool when this level is popped, and it then gets aliased with the next push
        public void set(NNS_MATRIX mtx)
        {
            if (this.stack.Count > 0)
            {
                this.stack[this.stack.Count - 1].Assign(mtx);
                return;
            }

            NNS_MATRIX copy = new NNS_MATRIX();
            copy.Assign(mtx);
            this.push(copy);
        }

        public void clear()
        {
            this.stack.Clear();
        }

        public NNS_MATRIX getInvert()
        {
            if (this.invert == null)
            {
                NNS_MATRIX src = this.get();
                this.invert = GlobalPool<NNS_MATRIX>.Alloc();
                nnInvertMatrix(this.invert, src);
            }
            return this.invert;
        }
    }
}
