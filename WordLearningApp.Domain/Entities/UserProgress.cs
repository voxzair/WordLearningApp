using System;

namespace WordLearningApp.Domain.Entities
{
    public class UserProgress : BaseEntity
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private int _wordId;
        public int WordId
        {
            get => _wordId;
            set { _wordId = value; OnPropertyChanged(); }
        }

        private Word _word;
        public Word Word
        {
            get => _word;
            set { _word = value; OnPropertyChanged(); }
        }

        private bool _isLearned;
        public bool IsLearned
        {
            get => _isLearned;
            set { _isLearned = value; OnPropertyChanged(); }
        }

        private DateTime? _lastReviewed;
        public DateTime? LastReviewed
        {
            get => _lastReviewed;
            set { _lastReviewed = value; OnPropertyChanged(); }
        }

        private int _reviewCount;
        public int ReviewCount
        {
            get => _reviewCount;
            set { _reviewCount = value; OnPropertyChanged(); }
        }
    }
}