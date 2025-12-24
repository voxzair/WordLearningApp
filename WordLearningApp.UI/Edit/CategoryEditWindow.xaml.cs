using System.Windows;
using WordLearningApp.Data;
using System;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class CategoryEditWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Category _editingCategory;
        private readonly bool _isEditMode;

        public CategoryEditWindow(AppDbContext context, Category category = null)
        {
            InitializeComponent();
            _context = context;
            _editingCategory = category;
            _isEditMode = category != null;

            Title = _isEditMode ? "Редактировать категорию" : "Добавить категорию";

            if (_isEditMode)
            {
                CategoryNameTextBox.Text = _editingCategory.Name;
                CategoryDescriptionTextBox.Text = _editingCategory.Description;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Введите название категории", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isEditMode)
            {
                _editingCategory.Name = CategoryNameTextBox.Text.Trim();
                _editingCategory.Description = CategoryDescriptionTextBox.Text.Trim();
            }
            else
            {
                var newCategory = new Category
                {
                    Name = CategoryNameTextBox.Text.Trim(),
                    Description = CategoryDescriptionTextBox.Text.Trim()
                };
                _context.Categories.Add(newCategory);
            }

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}