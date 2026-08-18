using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tools
{
    public class SelectableExpansion : Selectable
    {
        public enum SelectableState
        {
            Normal,

            /// <summary>
            /// The UI object is highlighted.
            /// </summary>
            Highlighted,

            /// <summary>
            /// The UI object is pressed.
            /// </summary>
            Pressed,

            /// <summary>
            /// The UI object is selected
            /// </summary>
            Selected,

            /// <summary>
            /// The UI object cannot be selected.
            /// </summary>
            Disabled
        }

        public event Action<SelectableState> ONStateChange;

        public UnityEvent OnStateNormal;
        public UnityEvent OnStateHighlighted;
        public UnityEvent OnStatePressed;
        public UnityEvent OnStateSelected;
        public UnityEvent OnStateDisabled;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            SelectableState selectableState = (SelectableState)state;
            ONStateChange?.Invoke((SelectableState)state);
            switch (selectableState)
            {
                case SelectableState.Normal:
                    OnStateNormal?.Invoke();
                    break;
                case SelectableState.Highlighted:
                    OnStateHighlighted?.Invoke();
                    break;
                case SelectableState.Pressed:
                    OnStatePressed?.Invoke();
                    break;
                case SelectableState.Selected:
                    OnStateSelected?.Invoke();
                    break;
                case SelectableState.Disabled:
                    OnStateDisabled?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}