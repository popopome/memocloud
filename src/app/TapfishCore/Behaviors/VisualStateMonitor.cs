using System;
using System.Collections;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TapfishCore.Behaviors
{
    public class VisualStateMonitor : Behavior<FrameworkElement>
    {
        #region State DependencyProperty

        /// <summary>
        /// The <see cref="State" /> dependency property's name.
        /// </summary>
        public const string StatePropertyName = "State";

        /// <summary>
        /// Gets or sets the value of the <see cref="State" />
        /// property. This is a dependency property.
        /// </summary>
        public string State
        {
            get
            {
                return (string)GetValue(StateProperty);
            }
            set
            {
                SetValue(StateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="State" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            StatePropertyName,
            typeof(string),
            typeof(VisualStateMonitor),
            new PropertyMetadata(""));

        #endregion State DependencyProperty

        VisualStateGroup Group { get; set; }

        /// <summary>
        /// Object attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            IList groups =
                VisualStateManager.GetVisualStateGroups(AssociatedObject);
            if (null == groups
                || 0 == groups.Count)
                return;

            Group = groups[0] as VisualStateGroup;
            if (null == Group)
                return;

            Group.CurrentStateChanged +=
                new EventHandler<VisualStateChangedEventArgs>(OnCurrentStateChanged);
        }

        /// <summary>
        /// Object detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (Group != null)
            {
                Group.CurrentStateChanged -=
                    new EventHandler<VisualStateChangedEventArgs>(OnCurrentStateChanged);
                Group = null;
            }
        }

        /// <summary>
        /// Current state changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCurrentStateChanged(
                    object sender,
                    VisualStateChangedEventArgs e)
        {
            State = e.NewState.Name;
        }
    }
}