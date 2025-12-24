using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WordLearningApp.Data;
using Microsoft.EntityFrameworkCore;
using WordLearningApp.Domain.Entities;

namespace WordLearningApp.UI
{
    public partial class PaymentWindow : Window
    {
        private readonly AppDbContext _context;
        private bool _isUpdatingCardNumber = false;

        public PaymentWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var selectedTariff = TariffComboBox.SelectedItem as ComboBoxItem;
            if (selectedTariff?.Tag == null)
            {
                ShowError("Выберите тариф подписки!");
                return;
            }

            bool isYearly = selectedTariff.Tag.ToString() == "Year";
            decimal amount = isYearly ? 2990m : 299m;
            int months = isYearly ? 12 : 1;

            try
            {
                using var transaction = _context.Database.BeginTransaction();

                // 1. Создаем платеж
                var payment = new Payment
                {
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    Status = "Success"
                };
                _context.Payments.Add(payment);
                _context.SaveChanges();

                // 2. Проверяем активную подписку СЕГОДНЯ
                var today = DateTime.Today;
                var activeSubscription = _context.Subscriptions
                    .Where(s => today >= s.StartDate.Date && today <= s.EndDate.Date)
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefault();

                Subscription subscriptionToSave;

                if (activeSubscription != null)
                {
                    // Продлеваем активную подписку
                    activeSubscription.EndDate = activeSubscription.EndDate.AddMonths(months);
                    _context.Subscriptions.Update(activeSubscription);
                    subscriptionToSave = activeSubscription;
                }
                else
                {
                    // Создаем новую подписку с сегодняшней даты
                    var newSubscription = new Subscription
                    {
                        StartDate = today,
                        EndDate = today.AddMonths(months),
                        Type = selectedTariff.Tag.ToString()
                    };
                    _context.Subscriptions.Add(newSubscription);
                    subscriptionToSave = newSubscription;
                }

                _context.SaveChanges();
                transaction.Commit();

                System.Diagnostics.Debug.WriteLine($"✓ Подписка {(activeSubscription != null ? "продлена" : "создана")}: ID={subscriptionToSave.Id}, End={subscriptionToSave.EndDate:yyyy-MM-dd}");

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ ОШИБКА ОПЛАТЫ: {ex}");
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            var cardNumber = CardNumberTextBox.Text?.Replace(" ", "") ?? "";
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 16)
            {
                ShowError("Введите корректный номер карты (16 цифр)");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ExpiryDateTextBox.Text) ||
                !ExpiryDateTextBox.Text.Contains("/"))
            {
                ShowError("Введите срок действия карты (MM/YY)");
                return false;
            }

            if (string.IsNullOrWhiteSpace(CvvTextBox.Text) ||
                CvvTextBox.Text.Length != 3)
            {
                ShowError("Введите CVV код (3 цифры)");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CardNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingCardNumber) return;

            var textBox = sender as TextBox;
            if (textBox == null) return;

            string text = textBox.Text.Replace(" ", "");
            text = new string(text.Where(char.IsDigit).ToArray());

            if (text.Length > 16) text = text.Substring(0, 16);

            if (text.Length > 0 && text.Length % 4 == 0 && text.Length < 16)
            {
                _isUpdatingCardNumber = true;
                textBox.Text += " ";
                textBox.CaretIndex = textBox.Text.Length;
                _isUpdatingCardNumber = false;
            }
        }

        private void ExpiryDateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ExpiryDateTextBox.Text.Replace("/", "");
            if (text.Length >= 2 && !ExpiryDateTextBox.Text.Contains("/"))
            {
                ExpiryDateTextBox.Text = text.Substring(0, 2) + "/" + text.Substring(2);
                ExpiryDateTextBox.CaretIndex = ExpiryDateTextBox.Text.Length;
            }
        }
    }
}