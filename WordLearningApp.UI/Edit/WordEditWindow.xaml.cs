using System.Linq;
using System.Windows;
using WordLearningApp.Data;
using System;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class WordEditWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Word _editingWord;
        private bool _isEditMode;

        public WordEditWindow(AppDbContext context, Word word = null)
        {
            InitializeComponent();
            _context = context;
            _editingWord = word;
            _isEditMode = word != null;

            Title = _isEditMode ? "Редактировать слово" : "Добавить слово";
            LoadCategories();

            if (_isEditMode)
            {
                ForeignWordTextBox.Text = _editingWord.ForeignWord;
                TranslationTextBox.Text = _editingWord.Translation;
                TranscriptionTextBox.Text = _editingWord.Transcription;
                CategoryComboBox.SelectedValue = _editingWord.CategoryId;
            }
        }

        private void LoadCategories()
        {
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();
            CategoryComboBox.ItemsSource = categories;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ForeignWordTextBox.Text))
            {
                MessageBox.Show("Введите иностранное слово", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TranslationTextBox.Text))
            {
                MessageBox.Show("Введите перевод", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isEditMode)
            {
                _editingWord.ForeignWord = ForeignWordTextBox.Text.Trim();
                _editingWord.Translation = TranslationTextBox.Text.Trim();
                _editingWord.Transcription = TranscriptionTextBox.Text.Trim();
                _editingWord.CategoryId = ((Category)CategoryComboBox.SelectedItem).Id;
                _editingWord.Category = (Category)CategoryComboBox.SelectedItem;
            }
            else
            {
                var newWord = new Word
                {
                    ForeignWord = ForeignWordTextBox.Text.Trim(),
                    Translation = TranslationTextBox.Text.Trim(),
                    Transcription = TranscriptionTextBox.Text.Trim(),
                    CategoryId = ((Category)CategoryComboBox.SelectedItem).Id,
                    AddedDate = System.DateTime.Now
                };
                _context.Words.Add(newWord);
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