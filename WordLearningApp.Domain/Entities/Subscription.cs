using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordLearningApp.Domain.Entities
{
    public class Subscription : BaseEntity
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }

        private string _type;
        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        
        [NotMapped]
        public bool IsActive => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
    }
}