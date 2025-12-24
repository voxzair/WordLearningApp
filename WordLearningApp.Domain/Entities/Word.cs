using System;

namespace WordLearningApp.Domain.Entities
{
    public class Word : BaseEntity
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _foreignWord;
        public string ForeignWord
        {
            get => _foreignWord;
            set { _foreignWord = value; OnPropertyChanged(); }
        }

        private string _translation;
        public string Translation
        {
            get => _translation;
            set { _translation = value; OnPropertyChanged(); }
        }

        private string _transcription;
        public string Transcription
        {
            get => _transcription;
            set { _transcription = value; OnPropertyChanged(); }
        }

        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set { _categoryId = value; OnPropertyChanged(); }
        }

        private Category _category;
        public Category Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        private int _difficulty;
        public int Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(); }
        }

        private DateTime _addedDate;
        public DateTime AddedDate
        {
            get => _addedDate;
            set { _addedDate = value; OnPropertyChanged(); }
        }
    }
}