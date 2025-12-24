using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class WordsListWindow : Window
    {
        private readonly AppDbContext _context;

        public WordsListWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadWords();
        }

        private void LoadWords()
        {
            var words = _context.Words
                .Include(w => w.Category)
                .OrderBy(w => w.ForeignWord)
                .ToList();
            WordsDataGrid.ItemsSource = words;
        }

        private void BtnAddWord_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new WordEditWindow(_context);
            if (editWindow.ShowDialog() == true)
            {
                LoadWords();
            }
        }

        private void BtnEditWord_Click(object sender, RoutedEventArgs e)
        {
            if (WordsDataGrid.SelectedItem is Word selectedWord)
            {
                var editWindow = new WordEditWindow(_context, selectedWord);
                if (editWindow.ShowDialog() == true)
                {
                    LoadWords();
                }
            }
            else
            {
                MessageBox.Show("Выберите слово!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteWord_Click(object sender, RoutedEventArgs e)
        {
            if (WordsDataGrid.SelectedItem is Word selectedWord)
            {
                var result = MessageBox.Show($"Удалить '{selectedWord.ForeignWord}'?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var progress = _context.UserProgresses.Where(up => up.WordId == selectedWord.Id);
                    _context.UserProgresses.RemoveRange(progress);
                    _context.Words.Remove(selectedWord);
                    _context.SaveChanges();
                    LoadWords();
                }
            }
        }
    }
}