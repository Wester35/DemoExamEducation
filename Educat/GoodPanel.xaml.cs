using Educat.Data;
using Educat.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Educat
{
    /// <summary>
    /// Interaction logic for GoodPanel.xaml
    /// </summary>
    public partial class GoodPanel : UserControl
    {
        private Good CurrentGood;
        public event Action Edited;
        public GoodPanel(Good good, bool isAdmin)
        {
            InitializeComponent();
            CurrentGood = good;
            LoadItem();
            if (isAdmin)
            {
                MouseDoubleClick += UserControl_MouseDoubleClick;
            }
        }

        private void LoadItem()
        {
            CategoryLabel.Content = $"{CurrentGood.Category} | {CurrentGood.Label}";
            Description.Content = "Описание: " + CurrentGood.Description;
            Count.Content = "Количество на складе: " + CurrentGood.Count.ToString();
            if (CurrentGood.Count == 0) 
            {
                Count.Background = Brushes.LightBlue;            
            }
            Discount.Content = CurrentGood.Discount;
            if (CurrentGood.Discount < 0 || CurrentGood.Discount > 100) 
            {
                Discount.Content = "0";
            }
            if (CurrentGood.Discount > 0) 
            {
                NewPrice.Text = (CurrentGood.Price - CurrentGood.Price * ((double)CurrentGood.Discount / 100)).ToString("F2");
                OldPrice.TextDecorations = TextDecorations.Strikethrough;
                OldPrice.Foreground = Brushes.Red;
            }
            OldPrice.Text = CurrentGood.Price.ToString("F2");
            if (CurrentGood.Discount > 15 && CurrentGood.Discount <= 100)
            {
                DiscountBorder.Background = (Brush)Application.Current.Resources["Discount"];
            }
            UnitOfMeasure.Content = "Единица измерения: " + CurrentGood.UnitOfMeasure;
            Supplier.Content = "Поставщик: " + CurrentGood.Supplier;
            Fabric.Content = "Производитель: " + CurrentGood.Fabric;
            if (CurrentGood.Photo != null) 
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures", CurrentGood.Photo);

                if (File.Exists(path)) 
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(path);
                    bitmap.EndInit();
                    PhotoBox.Source = bitmap;
                }
            }
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddEditWindow wnd = new AddEditWindow(true, CurrentGood)
            {
                Owner = Application.Current.MainWindow
            };
            wnd.Closed += (s, args) =>
            {
                Edited?.Invoke();
            };
            wnd.ShowDialog();
        }
    }
}
