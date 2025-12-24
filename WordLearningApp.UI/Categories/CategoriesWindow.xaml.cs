using System.Linq;
using System.Windows;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class CategoriesWindow : Window
    {
        private readonly AppDbContext _context;

        public CategoriesWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            CategoriesDataGrid.ItemsSource = categories;
        }

        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new CategoryEditWindow(_context);
            if (editWindow.ShowDialog() == true)
            {
                LoadCategories();
            }
        }

        private void BtnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is Category selectedCategory)
            {
                var editWindow = new CategoryEditWindow(_context, selectedCategory);
                if (editWindow.ShowDialog() == true)
                {
                    LoadCategories();
                }
            }
            else
            {
                MessageBox.Show("Выберите категорию!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is Category selectedCategory)
            {
                if (_context.Words.Any(w => w.CategoryId == selectedCategory.Id))
                {
                    MessageBox.Show("Нельзя удалить категорию с словами!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show($"Удалить '{selectedCategory.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Categories.Remove(selectedCategory);
                    _context.SaveChanges();
                    LoadCategories();
                }
            }
        }
    }
}