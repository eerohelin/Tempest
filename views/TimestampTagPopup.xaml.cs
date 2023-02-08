using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Tempest
{
    /// <summary>
    /// Interaction logic for TimestampTagPopup.xaml
    /// </summary>
    public partial class TimestampTagPopup : Popup
    {

        private Timestamp _owner { get; set; }
        private StackPanel tempTag = new();

        public TimestampTagPopup(Timestamp owner)
        {
            InitializeComponent();

            _owner = owner;
        }


        public void CreateTag(string name)
        {
            StackPanel container = new StackPanel { Orientation = Orientation.Horizontal, Tag = name };
            CheckBox checkbox = new CheckBox { Content = name, Width = 75 };
            Button deleteButton = new Button { Content = "X" };

            checkbox.Checked += Checkbox_Checked;
            checkbox.Unchecked += Checkbox_Unchecked;
            deleteButton.Click += (x, y) => DeleteButton_Clicked(x, y, name);

            container.Children.Add(checkbox);
            container.Children.Add(deleteButton);

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

            ReplayView.TagManager.DeleteTag(name);
        }

        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)e.OriginalSource;
            _owner.Tags.Remove(checkbox.Content.ToString());
        }

        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)e.OriginalSource;
            _owner.Tags.Add(checkbox.Content.ToString());
        }

        private void Check_Checkbox(string name)
        {
            foreach (var child in tagContainer.Children)
            {
                if (child is StackPanel)
                {
                    StackPanel childPanel = (StackPanel)child;
                    if ((string)childPanel.Tag == name)
                    {
                        CheckBox checkbox = (CheckBox)childPanel.Children[0];
                        checkbox.IsChecked = true;
                        return;
                    }
                }
            }
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e, string name)
        {
            RemoveTag(name);
        }

        private void NewTagButton_Click(object sender, RoutedEventArgs e)
        {
            tempTag = new StackPanel { Orientation= Orientation.Horizontal };
            TextBox nameInput = new TextBox { Width = 85};
            tempTag.Children.Add(nameInput);


            tagContainer.Children.Add(tempTag);
            nameInput.Focus();
            nameInput.LostFocus += CancelTagCreation;
            nameInput.KeyDown += NameInput_KeyDown;
        }

        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TagSetup();
            }
        }

        private void TagSetup()
        {
            TextBox nameInput = (TextBox)tempTag.Children[0];
            string name = nameInput.Text;
            tagContainer.Children.Remove(tempTag);
            ReplayView.TagManager.CreateTag(name);

            Check_Checkbox(name);
        }

        private void CancelTagCreation(object sender, RoutedEventArgs e)
        {
            tagContainer.Children.Remove(tempTag);
        }
    }
}
