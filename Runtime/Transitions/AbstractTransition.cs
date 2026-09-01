using UnityEngine;

namespace OneM.UISystem
{
    /// <summary>
    /// Abstract component to transit UI components based on <see cref="SelectionState"/>.
    /// </summary>
    public abstract class AbstractTransition : MonoBehaviour
    {
        [Tooltip("The data used to transit the UI state.")]
        [SerializeField] protected SelectableTransitionData data;

        public abstract void Transit(SelectionState state, bool instant);

        public void SetData(SelectableTransitionData data) => this.data = data;
        public void TransitToNormalState() => Transit(SelectionState.Normal, true);
    }
}