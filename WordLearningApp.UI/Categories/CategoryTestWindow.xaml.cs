using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class CategoryTestWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Category? _category;
        private readonly List<int>? _wordIds;
        private List<Word> _testWords = new();
        private int _currentQuestionIndex = 0;
        private int _correctAnswers = 0;
        private Word? _currentWord;

        public CategoryTestWindow(AppDbContext context, Category category)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _category = category ?? throw new ArgumentNullException(nameof(category));

            CategoryTitleText.Text = $"Тест по категории: {category.Name}";
            LoadTestQuestions();
        }

        public CategoryTestWindow(AppDbContext context, List<int> wordIds)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _wordIds = wordIds ?? throw new ArgumentNullException(nameof(wordIds));

            CategoryTitleText.Text = "Тест по изученным словам";
            LoadTestQuestions();
        }

        private void LoadTestQuestions()
        {
            IQueryable<Word> wordsQuery = _context.Words.Include(w => w.Category);

            if (_category != null)
            {
                wordsQuery = wordsQuery.Where(w => w.CategoryId == _category.Id);
            }
            else if (_wordIds != null)
            {
                wordsQuery = wordsQuery.Where(w => _wordIds.Contains(w.Id));
            }
            else
            {
                MessageBox.Show("Нет слов для теста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            var allWords = wordsQuery.ToList();
            _testWords = allWords.OrderBy(w => Guid.NewGuid()).Take(5).ToList();

            if (!_testWords.Any())
            {
                MessageBox.Show("Нет слов для теста.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
                return;
            }

            TotalQuestionsText.Text = _testWords.Count.ToString();
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (_currentQuestionIndex >= _testWords.Count)
            {
                ShowResults();
                return;
            }

            _currentWord = _testWords[_currentQuestionIndex];
            WordToTranslateText.Text = _currentWord?.ForeignWord ?? "";
            CurrentQuestionText.Text = (_currentQuestionIndex + 1).ToString();

            GenerateAnswers();
        }

        private void GenerateAnswers()
        {
            if (_currentWord == null || AnswersPanel == null) return;

            IQueryable<Word> otherWordsQuery = _context.Words;

            if (_category != null)
            {
                otherWordsQuery = otherWordsQuery.Where(w => w.CategoryId == _category.Id && w.Id != _currentWord.Id);
            }
            else if (_wordIds != null)
            {
                otherWordsQuery = otherWordsQuery.Where(w => _wordIds.Contains(w.Id) && w.Id != _currentWord.Id);
            }
            else
            {
                return;
            }

            var otherWords = otherWordsQuery
                .OrderBy(w => Guid.NewGuid())
                .Take(3)
                .Select(w => w.Translation)
                .ToList();

            while (otherWords.Count < 3)
            {
                otherWords.Add($"Вариант {otherWords.Count + 2}");
            }

            var correctIndex = new Random().Next(0, 4);
            otherWords.Insert(correctIndex, _currentWord.Translation);

            var children = AnswersPanel.Children;
            for (int i = 0; i < 4 && i < children.Count; i++)
            {
                if (children[i] is Button button)
                {
                    button.Content = otherWords[i];
                    button.Tag = (i == correctIndex);
                    button.Background = System.Windows.Media.Brushes.LightGray;
                }
            }
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is bool isCorrect)
            {
                if (isCorrect)
                {
                    _correctAnswers++;
                    button.Background = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    button.Background = System.Windows.Media.Brushes.Red;
                    var children = AnswersPanel?.Children;
                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            if (child is Button btn && btn.Tag is bool correct && correct)
                            {
                                btn.Background = System.Windows.Media.Brushes.Green;
                                break;
                            }
                        }
                    }
                }

                var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    _currentQuestionIndex++;
                    ShowQuestion();
                };
                timer.Start();
            }
        }

        private void ShowResults()
        {
            var percentage = (_testWords.Count > 0) ? (_correctAnswers * 100 / _testWords.Count) : 0;

            // ✅ СОХРАНЕНИЕ ДО закрытия окна
            var testResult = new TestResult
            {
                TestDate = DateTime.Now,
                CorrectAnswers = _correctAnswers,
                TotalQuestions = _testWords.Count,
                Percentage = percentage,
                CategoryName = _category?.Name ?? "Изученные слова"
            };

            _context.TestResults.Add(testResult);
            _context.SaveChanges();

            string message = $"Тест завершен!\n\n" +
                           $"Правильных ответов: {_correctAnswers} из {_testWords.Count}\n" +
                           $"Результат: {percentage}%\n\n";

            if (percentage >= 80)
                message += "Отлично!";
            else if (percentage >= 60)
                message += "Хорошо!";
            else
                message += "Попробуйте еще раз.";

            MessageBox.Show(message, "Результаты", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}