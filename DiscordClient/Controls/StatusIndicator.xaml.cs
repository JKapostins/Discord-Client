using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Discord.Controls
{
    public enum Status
    {
        Active,
        Away,
        Offline
    }

    public sealed partial class StatusIndicator : UserControl
    {
        public StatusIndicator()
        {
            this.InitializeComponent();
        }

        public Status Status
        {
            get { return (Status)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(Status), typeof(StatusIndicator), null);


    }
}
