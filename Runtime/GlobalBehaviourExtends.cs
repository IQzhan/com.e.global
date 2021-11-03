namespace E
{
    public partial class GlobalBehaviour
    {
        internal void ExecuteDrawGizmos(in bool selected)
        {
            DrawGizmosCallback(selected);
        }

        protected virtual void DrawGizmosCallback(in bool selected) { }
    }
}