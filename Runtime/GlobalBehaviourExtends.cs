namespace E
{
    public partial class GlobalBehaviour
    {
        internal void ExecuteDrawGizmos(bool selected)
        {
            OnDrawGizmos(selected);
        }

        protected virtual void OnDrawGizmos(bool selected) { }
    }
}