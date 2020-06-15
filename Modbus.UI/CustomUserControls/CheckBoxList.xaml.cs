using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Stormbus.UI.CustomUserControls
{
    /// <summary>
    ///     Interaction logic for CheckBoxList.xaml
    /// </summary>
    public partial class CheckBoxList : UserControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(List<bool>), typeof(CheckBoxList), new PropertyMetadata(default(List<bool>)));

        public CheckBoxList()
        {
            InitializeComponent();
        }

        public List<bool> Items
        {
            get => (List<bool>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
    }
}