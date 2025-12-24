using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class CategoryStudyWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Category _selectedCategory;
        private Word? _currentWord;
        private int _sessionCount = 0;
        private int _remainingWordsCount = 0;
        private int _totalInCategory = 0;

        public CategoryStudyWindow(AppDbContext context, Category category)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _selectedCategory = category ?? throw new ArgumentNullException(nameof(category));

            CategoryTitleText.Text = $"Категория: {_selectedCategory.Name}";
            LoadRandomWord();
        }

        private void LoadRandomWord()
        {
            
            _totalInCategory = _context.Words.Count(w => w.CategoryId == _selectedCategory.Id);
            TotalInCategoryText.Text = _totalInCategory.ToString();

            
            _remainingWordsCount = _context.Words
                .Count(w => w.CategoryId == _selectedCategory.Id &&
                           !_context.UserProgresses.Any(up => up.WordId == w.Id && up.IsLearned));

            WordsRemainingText.Text = _remainingWordsCount.ToString();

            
            if (_remainingWordsCount <= 0)
            {
                string message = $"✓ Категория '{_selectedCategory.Name}' завершена!\n" +
                               $"Всего слов: {_totalInCategory}\n" +
                               $"Просмотрено: {_sessionCount}\n\n" +
                               $"Хотите пройти тест по этой категории?";

                var result = MessageBox.Show(message, "Категория завершена",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    
                    var testWindow = new CategoryTestWindow(_context, _selectedCategory);
                    testWindow.ShowDialog();
                }

                Close();
                return;
            }

            
            var wordsInCategory = _context.Words
                .Include(w => w.Category)
                .Where(w => w.CategoryId == _selectedCategory.Id &&
                           !_context.UserProgresses.Any(up => up.WordId == w.Id && up.IsLearned))
                .ToList();

            var random = new Random();
            _currentWord = wordsInCategory.Any()
                ? wordsInCategory[random.Next(wordsInCategory.Count)]
                : null;

            ShowCurrentWord();
        }

        private void ShowCurrentWord()
        {
            if (_currentWord == null) return;

            CurrentWordText.Text = _currentWord.ForeignWord ?? string.Empty;
            TranscriptionText.Text = _currentWord.Transcription ?? string.Empty;
            TranslationText.Text = string.Empty;

            TranslationBorder.Visibility = Visibility.Collapsed;
            ShowTranslationButton.Visibility = Visibility.Visible;
            NextWordButton.Visibility = Visibility.Collapsed;

            UpdateSessionStats();
        }

        private void ShowTranslationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWord == null) return;

            TranslationText.Text = _currentWord.Translation ?? string.Empty;
            TranslationBorder.Visibility = Visibility.Visible;
            ShowTranslationButton.Visibility = Visibility.Collapsed;
            NextWordButton.Visibility = Visibility.Visible;
        }

        private void NextWord_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWord == null) return;

            _sessionCount++;

            var progress = _context.UserProgresses.FirstOrDefault(up => up.WordId == _currentWord.Id);

            if (progress == null)
            {
                progress = new UserProgress
                {
                    WordId = _currentWord.Id,
                    LastReviewed = DateTime.Now,
                    ReviewCount = 1,
                    IsLearned = true
                };
                _context.UserProgresses.Add(progress);
            }
            else
            {
                progress.ReviewCount++;
                progress.LastReviewed = DateTime.Now;
                progress.IsLearned = true;
            }

            _context.SaveChanges();
            LoadRandomWord();
        }

        private void UpdateSessionStats()
        {
            SessionStatsText.Text = $"Просмотрено: {_sessionCount} слов";
        }
    }
}