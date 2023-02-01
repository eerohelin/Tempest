using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for ComboBoxTag.xaml
    /// </summary>
    public partial class ComboBoxTag : ComboBoxItem
    {

        private string _TagName { get; set; }

        public ComboBoxTag(string TagName)
        {
            InitializeComponent();

            _TagName = TagName;

            Loaded += ComboBoxTag_Loaded;
        }

        private void ComboBoxTag_Loaded(object sender, RoutedEventArgs e)
        {
            TagTitle.Text = _TagName;
        }

        private void TagCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ReplayView.CurrentTags.Add(_TagName);
        }

        private void TagCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ReplayView.CurrentTags.Remove(_TagName);
        }
    }
}
