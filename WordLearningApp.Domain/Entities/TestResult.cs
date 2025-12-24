using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordLearningApp.Domain.Entities
{
    public class TestResult : BaseEntity
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private DateTime _testDate;
        public DateTime TestDate
        {
            get => _testDate;
            set { _testDate = value; OnPropertyChanged(); }
        }

        private int _correctAnswers;
        public int CorrectAnswers
        {
            get => _correctAnswers;
            set { _correctAnswers = value; OnPropertyChanged(); }
        }

        private int _totalQuestions;
        public int TotalQuestions
        {
            get => _totalQuestions;
            set { _totalQuestions = value; OnPropertyChanged(); }
        }

        private double _percentage;
        public double Percentage
        {
            get => _percentage;
            set { _percentage = value; OnPropertyChanged(); }
        }

        private string? _categoryName;
        public string? CategoryName
        {
            get => _categoryName;
            set { _categoryName = value; OnPropertyChanged(); }
        }
    }
}
