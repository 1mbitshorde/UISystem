using System;
using UnityEngine.EventSystems;

namespace OneM.UISystem
{
    /// <summary>
    /// Interface used on objects able to be selected. 
    /// </summary>
    public interface ISelectable : IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        /// <summary>
        /// Event fired when this object is selected.
        /// </summary>
        event Action OnSelected;
    }
}