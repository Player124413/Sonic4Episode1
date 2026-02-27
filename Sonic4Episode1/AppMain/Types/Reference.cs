internal partial class AppMain
{
    internal class Reference<T>
    {
        private T target_;

        public Reference(T target)
        {
            this.target_ = target;
        }

        public T Target
        {
            get => this.target_;
            set => this.target_ = value;
        }
    }
}
