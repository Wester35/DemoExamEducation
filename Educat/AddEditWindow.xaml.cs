using Educat.Data;
using Educat.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Educat
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private bool IsEditing = false;
        private Good CurrentGood;
        private int CurrentId;
        private bool IsPhotoEdit = false;
        private string FileName;
        private string PhotoPath;

        public AddEditWindow(bool isEditing, Good good)
        {
            InitializeComponent();
            IsEditing = isEditing;
            CurrentGood = good;
            
            if (isEditing)
            {
                InitEditWindow();
            }
            else
            {
                InitAddWindow();
            }

            PhotoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures", $"{CurrentId}.jpg");
        }

        private void InitEditWindow()
        {
            CurrentId = CurrentGood.Id;
            this.Title = "Редактирование товара";
            IdLabel.Content = $"ID: {CurrentId}";
            ArticleBox.Text = CurrentGood.Article;
            UnitOfMeasureBox.Text = CurrentGood.UnitOfMeasure;
            PriceBox.Text = CurrentGood.Price.ToString();
            DiscountBox.Text = CurrentGood.Discount.ToString();
            CountBox.Text = CurrentGood.Count.ToString();
            DescriptionBox.Text = CurrentGood.Description;
            AddEditButton.Content = "Редактировать";

            if (CurrentGood.Photo != null)
            {
                LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pictures", CurrentGood.Photo));
            }

            InitComboBoxes();
            LabelBox.SelectedIndex = CurrentGood.IdLabel;
            CategoryBox.SelectedIndex = CurrentGood.IdCategory;
            FabricBox.SelectedIndex = CurrentGood.IdFabric;
            SupplierBox.SelectedIndex = CurrentGood.IdSupplier;
        }

        private void InitComboBoxes()
        {
            List<ComboBoxItems> labels = DbHelper.GetComboBoxItems("Labels");
            List<ComboBoxItems> fabrics = DbHelper.GetComboBoxItems("Fabrics");
            List<ComboBoxItems> categories = DbHelper.GetComboBoxItems("Categories");
            List<ComboBoxItems> suppliers = DbHelper.GetComboBoxItems("Suppliers");

            ComboBoxItems itemForLists = new ComboBoxItems {Id = 0, Name = "Не выбрано"};
            labels.Insert(0,itemForLists);
            fabrics.Insert(0, itemForLists);
            categories.Insert(0, itemForLists);
            suppliers.Insert(0, itemForLists);

            LabelBox.ItemsSource = labels;
            FabricBox.ItemsSource = fabrics;
            SupplierBox.ItemsSource = suppliers;
            CategoryBox.ItemsSource = categories;

            LabelBox.SelectedValuePath = "Id";
            LabelBox.DisplayMemberPath = "Name";
            FabricBox.SelectedValuePath = "Id";
            FabricBox.DisplayMemberPath = "Name";
            SupplierBox.SelectedValuePath = "Id";
            SupplierBox.DisplayMemberPath = "Name";
            CategoryBox.SelectedValuePath = "Id";
            CategoryBox.DisplayMemberPath = "Name";
        }

        private void LoadImage(string path)
        { 
            if (File.Exists(path))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();

                ImageBox.Source = bitmap;
            }
        }

        private void InitAddWindow()
        {
            CurrentId = DbHelper.GetMaxId() + 1;
            IdLabel.Content = null;
            DeleteButton.Visibility = Visibility.Collapsed;

            InitComboBoxes();
            LabelBox.SelectedIndex = 0;
            CategoryBox.SelectedIndex = 0;
            FabricBox.SelectedIndex = 0;
            SupplierBox.SelectedIndex = 0;
        }
        
        private bool CheckImageSize(string fileName)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fileName);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            if (image.PixelHeight > 200 && image.PixelWidth > 300)
            {
                MessageBox.Show("Размер изображения не может превышать 300х200 пикселей!", 
                    "Ошибка загрузки изображения", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "Изображение (*.jpg;*.png)|*.jpg;*.png";

                if (openFileDialog.ShowDialog() != true) 
                {
                    return;
                }

                if (CheckImageSize(openFileDialog.FileName))
                {
                    FileName = openFileDialog.FileName;
                    IsPhotoEdit = true;
                    LoadImage(FileName);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки фотографии", "Ошибка загрузки файла", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить товар?", 
                "Удаление товара", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes) 
            {
                if (DbHelper.DeleteGood(CurrentId))
                {
                    MessageBox.Show("Товар удален!",
                        "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    File.Delete(PhotoPath);

                    ArticleBox.Text = string.Empty;
                    PriceBox.Text = string.Empty;
                    DiscountBox.Text = string.Empty;
                    CountBox.Text = string.Empty;
                    UnitOfMeasureBox.Text = string.Empty;
                    DescriptionBox.Text = string.Empty;
                    ImageBox.Source = new BitmapImage(new Uri("pack://application:,,,/Resouces/picture.png"));
                    LabelBox.SelectedIndex = 0;
                    CategoryBox.SelectedIndex = 0;
                    FabricBox.SelectedIndex = 0;
                    SupplierBox.SelectedIndex = 0;
                }
            }
        }

        private void AddEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (LabelBox.SelectedIndex == 0 || 
                CategoryBox.SelectedIndex == 0 || 
                FabricBox.SelectedIndex == 0 || 
                SupplierBox.SelectedIndex == 0)
            {
                MessageBox.Show("Не все поля выбраны!", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            string photo = null;

            if (IsPhotoEdit)
            {
                photo = CurrentId.ToString() + ".jpg";
            }

            Good good = new Good
            {
                Id = CurrentId,
                Article = ArticleBox.Text,
                IdLabel = LabelBox.SelectedIndex,
                UnitOfMeasure = UnitOfMeasureBox.Text,
                Price = Convert.ToDouble(PriceBox.Text),
                IdSupplier = SupplierBox.SelectedIndex,
                IdFabric = FabricBox.SelectedIndex,
                IdCategory = CategoryBox.SelectedIndex,
                Discount = Convert.ToInt32(DiscountBox.Text),
                Count = Convert.ToInt32(CountBox.Text),
                Description = DescriptionBox.Text,
                Photo = photo
            };

            if (IsEditing)
            {
                if (DbHelper.EditGoods(good))
                {
                    MessageBox.Show("Товар отредактирован!", "Успех!", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                if (DbHelper.AddGoods(good))
                {
                    MessageBox.Show("Товар добавлен!", "Успех!",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            if (IsPhotoEdit)
            {
                File.Copy(FileName, PhotoPath, true);
                IsPhotoEdit = false;
            }
        }

        private void PriceBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text == ".")
            {
                e.Handled = true;
            }
            TextBox textBox = sender as TextBox;
            if (e.Text == "." && textBox.Text.Contains(".")) 
            {
                e.Handled = true;
            }
        }

        private void IntBoxes_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text == ".")
            {
                e.Handled = true;
            }
        }

        private void ArticleBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text.Length >= 6)
            {
                e.Handled = true;
            }
        }
    }
}
