using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class StatisticsWindow : Window
    {
        private readonly AppDbContext _context;

        public StatisticsWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));

            LoadStatistics();
            LoadCategoryStatistics();
            LoadTestHistory();
        }

        private void LoadStatistics()
        {
            var total = _context.Words.Count();
            var learned = _context.Set<UserProgress>().Count(up => up.IsLearned);
            StatsTotalWords.Text = $"Всего слов: {total}";
            StatsLearnedWords.Text = $"Изучено: {learned}";
            StatsProgress.Text = $"Прогресс: {(total > 0 ? (learned * 100 / total) : 0)}%";
        }

        private void LoadCategoryStatistics()
        {
            var categoryStats = (from cat in _context.Categories
                                 join w in _context.Words on cat.Id equals w.CategoryId into wordsGroup
                                 select new
                                 {
                                     CategoryName = cat.Name,
                                     TotalWords = wordsGroup.Count(),
                                     LearnedWords = wordsGroup.Count(w => _context.Set<UserProgress>().Any(up => up.WordId == w.Id && up.IsLearned))
                                 })
                                .AsEnumerable()
                                .Select(stat => new
                                {
                                    stat.CategoryName,
                                    stat.TotalWords,
                                    stat.LearnedWords,
                                    Progress = stat.TotalWords > 0 ? (stat.LearnedWords * 100 / stat.TotalWords) : 0
                                })
                                .ToList();

            CategoriesStatsGrid.ItemsSource = categoryStats;
        }

        private void LoadTestHistory()
        {
            var testResults = _context.TestResults
                .OrderByDescending(t => t.TestDate)
                .Take(50)
                .ToList();
            TestsHistoryGrid.ItemsSource = testResults;
        }

        private void BtnResetProgress_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сбросить весь прогресс изучения?\n\n⚠️ Это также удалит историю тестов!",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Set<UserProgress>().RemoveRange(_context.Set<UserProgress>());
                _context.TestResults.RemoveRange(_context.TestResults);
                _context.SaveChanges();

                LoadStatistics();
                LoadCategoryStatistics();
                LoadTestHistory();

                MessageBox.Show("✓ Прогресс и история тестов сброшены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}