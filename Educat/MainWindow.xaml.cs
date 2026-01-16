using Educat.Data;
using Educat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Educat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User CurrentUser;
        private bool IsAdmin = false;
        private List<Good> Goods;

        public MainWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            InitRoleComponents();
            LoadItems();
        }

        private void LoadItems()
        {
            Goods = DbHelper.GetGoods();

            ItemsPanel.Children.Clear();
            foreach (Good good in Goods)
            {
                GoodPanel item = new GoodPanel(good, IsAdmin);
                item.Edited += LoadItems;
                ItemsPanel.Children.Add(item);
            } 
        }

        private void InitFilterBox()
        {
            List<ComboBoxItems> list = DbHelper.GetComboBoxItems("Suppliers");
            list.Insert(0, new ComboBoxItems { Id = 0, Name = "Без фильтра" });
            FilterBox.ItemsSource = list;
            FilterBox.SelectedValuePath = "Id";
            FilterBox.DisplayMemberPath = "Name";
            FilterBox.SelectedIndex = 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditWindow wnd = new AddEditWindow(false, null)
            {
                Owner = Application.Current.MainWindow
            };
            wnd.Closed += (s, args) =>
            {
                LoadItems();
            };
            wnd.ShowDialog();
        }

        private void InitRoleComponents()
        {
            if (CurrentUser != null) 
            {
                this.Title += $" ({CurrentUser.Role})";
                FullNameLabel.Content = CurrentUser.FullName;
                BackButton.Content = "Выйти";

                if (CurrentUser.Role == "Администратор") 
                {
                    IsAdmin = true;
                    InitFilterBox();
                }
                else if (CurrentUser.Role == "Менеджер")
                {
                    AddButton.Visibility = Visibility.Collapsed;
                    InitFilterBox();
                }
                else if (CurrentUser.Role == "Авторизированный клиент")
                {
                    SortBox.Visibility = Visibility.Collapsed;
                    FilterBox.Visibility = Visibility.Collapsed;
                    SearchBox.Visibility = Visibility.Collapsed;
                    AddButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                this.Title += " (Гость)";
                SortBox.Visibility = Visibility.Collapsed;
                FilterBox.Visibility = Visibility.Collapsed;
                SearchBox.Visibility = Visibility.Collapsed;
                AddButton.Visibility = Visibility.Collapsed;
                FullNameLabel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
