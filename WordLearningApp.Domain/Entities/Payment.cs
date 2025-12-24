using System;

namespace WordLearningApp.Domain.Entities
{
    public class Payment : BaseEntity
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        private DateTime _paymentDate;
        public DateTime PaymentDate
        {
            get => _paymentDate;
            set { _paymentDate = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }
    }
}