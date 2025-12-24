using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using WordLearningApp.Data;
using WordLearningApp.Domain.Entities;
namespace WordLearningApp.UI
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context; 

        public MainWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _context.Database.Migrate();
            CheckSubscription();
            UpdateButtonsState();
        }

        private bool HasActiveSubscription()
        {
            var today = DateTime.Today;
            return _context.Subscriptions.AsNoTracking().Any(s =>
                today >= s.StartDate.Date && today <= s.EndDate.Date);
        }

        private void CheckSubscription()
        {
            var today = DateTime.Today;

            var subscription = _context.Subscriptions
                .AsNoTracking()
                .Where(s => today >= s.StartDate.Date && today <= s.EndDate.Date)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefault();

            if (subscription != null)
            {
                SubscriptionStatus.Text = $"✓ Подписка активна до {subscription.EndDate:dd.MM.yyyy}";
                SubscriptionStatus.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                SubscriptionStatus.Text = "✗ Подписка не активна";
                SubscriptionStatus.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void UpdateButtonsState()
        {
            bool hasSubscription = HasActiveSubscription();
            BtnWordsList.IsEnabled = hasSubscription;
            BtnCategories.IsEnabled = hasSubscription;
        }

        private void ShowSubscriptionRequiredMessage()
        {
            MessageBox.Show("Эта функция доступна только с активной подпиской!\n\n" +
                          "Нажмите 'Оплатить подписку', чтобы получить доступ.",
                          "Требуется подписка", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();
        private void MenuStudy_Click(object sender, RoutedEventArgs e) => OpenStudy();
        private void MenuStudyByCategory_Click(object sender, RoutedEventArgs e) => OpenStudyByCategory();
        private void MenuStatistics_Click(object sender, RoutedEventArgs e) => OpenStatistics();
        private void MenuPayment_Click(object sender, RoutedEventArgs e) => OpenPayment();

        private void MenuWordsList_Click(object sender, RoutedEventArgs e)
        {
            if (!HasActiveSubscription()) ShowSubscriptionRequiredMessage();
            else OpenWordsList();
        }

        private void MenuCategories_Click(object sender, RoutedEventArgs e)
        {
            if (!HasActiveSubscription()) ShowSubscriptionRequiredMessage();
            else OpenCategories();
        }

        private void BtnStudy_Click(object sender, RoutedEventArgs e) => OpenStudy();
        private void BtnStudyByCategory_Click(object sender, RoutedEventArgs e) => OpenStudyByCategory();
        private void BtnStatistics_Click(object sender, RoutedEventArgs e) => OpenStatistics();

        private void BtnWordsList_Click(object sender, RoutedEventArgs e)
        {
            if (!HasActiveSubscription()) ShowSubscriptionRequiredMessage();
            else OpenWordsList();
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            if (!HasActiveSubscription()) ShowSubscriptionRequiredMessage();
            else OpenCategories();
        }

        private void BtnUnsubscribe_Click(object sender, RoutedEventArgs e)
        {
            if (!HasActiveSubscription())
            {
                MessageBox.Show("Активная подписка не найдена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите отменить подписку?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var active = _context.Subscriptions
                .Where(s => DateTime.Today >= s.StartDate.Date && DateTime.Today <= s.EndDate.Date)
                .ToList();
            _context.Subscriptions.RemoveRange(active);
            _context.SaveChanges();

            CheckSubscription();
            UpdateButtonsState();
        }

        private void OpenWordsList()
        {
            var window = new WordsListWindow(_context);
            window.Show();
        }

        private void OpenCategories()
        {
            var window = new CategoriesWindow(_context);
            window.Show();
        }

        private void OpenStudy()
        {
            var window = new CategoryStudyWindow(_context, null);
            window.ShowDialog();
        }

        private void OpenStudyByCategory()
        {
            var selectionWindow = new CategorySelectionWindow(_context);
            if (selectionWindow.ShowDialog() == true && selectionWindow.SelectedCategory != null)
            {
                var studyWindow = new CategoryStudyWindow(_context, selectionWindow.SelectedCategory);
                studyWindow.ShowDialog();
            }
        }

        private void OpenStatistics()
        {
            
            var window = new StatisticsWindow(_context);
            window.Show();
        }

        private void OpenPayment()
        {
            var window = new PaymentWindow(_context);
            if (window.ShowDialog() == true)
            {
                
                _context.Dispose();
                _context = new AppDbContext();
                CheckSubscription();
                UpdateButtonsState();
                MessageBox.Show("✓ Подписка успешно оплачена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnTestLearnedWords_Click(object sender, RoutedEventArgs e)
        {
            var learnedWordIds = _context.Set<UserProgress>()
                .Where(up => up.ReviewCount > 0 || up.IsLearned)
                .Select(up => up.WordId)
                .Distinct()
                .ToList();

            if (!learnedWordIds.Any())
            {
                MessageBox.Show("Вы ещё не изучили ни одного слова!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var window = new CategoryTestWindow(_context, learnedWordIds);
            window.Title = "Тест по изученным словам";
            window.ShowDialog();
        }
    }
}
