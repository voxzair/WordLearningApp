using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class CategorySelectionWindow : Window
    {
        private readonly AppDbContext _context;

        public Category? SelectedCategory { get; private set; }

        public CategorySelectionWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            CategoryComboBox.ItemsSource = categories;
            if (categories.Any()) CategoryComboBox.SelectedIndex = 0;
        }

        private void StartStudyButton_Click(object sender, RoutedEventArgs e)
        {
            
            SelectedCategory = CategoryComboBox.SelectedItem as Category;
            if (SelectedCategory == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = true;
            Close();
        }
    }
}