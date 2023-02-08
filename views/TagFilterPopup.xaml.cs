using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Tempest
{
    /// <summary>
    /// Interaction logic for TagFilterPopup.xaml
    /// </summary>
    public partial class TagFilterPopup : Popup
    {
        public TagFilterPopup()
        {
            InitializeComponent();
        }

        public void CreateTag(string name)
        {
            StackPanel container = new StackPanel { Orientation = Orientation.Horizontal, Tag = name };
            CheckBox checkbox = new CheckBox { Content = name, Width = 75 };

            checkbox.Checked += Checkbox_Checked;
            checkbox.Unchecked += Checkbox_Unchecked;

            container.Children.Add(checkbox);

            tagContainer.Children.Add(container);
        }

        public void RemoveTag(string name)
        {
            List<StackPanel> toRemove = new();
            foreach (var child in tagContainer.Children)
            {
                if (child is StackPanel)
                {
                    StackPanel childPanel = (StackPanel)child;
                    if ((string)childPanel.Tag == name)
                    {
                        toRemove.Add(childPanel);
                    }
                }
            }
            foreach (var child in toRemove)
            {
                tagContainer.Children.Remove(child);
            }
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)e.OriginalSource;
            ReplayView.CurrentTags.Remove(checkbox.Content.ToString());
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)e.OriginalSource;
            ReplayView.CurrentTags.Add(checkbox.Content.ToString());
        }

        private void ClearButton_Clicked(object sender, RoutedEventArgs e)
        {
            foreach(var child in tagContainer.Children)
            {
                if (child is StackPanel)
                {
                    StackPanel childPanel = (StackPanel)child;
                    CheckBox checkbox = (CheckBox)childPanel.Children[0];
                    checkbox.IsChecked = false;
                }
            }
        }
    }
}
