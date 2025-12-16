using CommunitySharing.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CommunitySharing.ViewModels
{
    public class DiscoverPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SharedItem> Items { get; set; }

        public DiscoverPageViewModel()
        {
            LoadItems();
        }

        private void LoadItems()
        {
            Items = new ObservableCollection<SharedItem>
            {
                new SharedItem
                {
                    Title = "Fresh Apples",
                    ImageUrl = "https://images.pexels.com/photos/1755711/pexels-photo-1755711.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Distance = "2 KM",
                    StockAvailable = 3
                },
                new SharedItem
                {
                    Title = "Novels",
                    ImageUrl = "https://images.pexels.com/photos/1292115/pexels-photo-1292115.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Distance = "10 KM",
                    StockAvailable = 4
                },
                new SharedItem
                {
                    Title = "Clothes",
                    ImageUrl = "https://images.pexels.com/photos/1043148/pexels-photo-1043148.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Distance = "5 KM",
                    StockAvailable = 7
                },
                new SharedItem
                {
                    Title = "Basket of Full Tomatoes",
                    ImageUrl = "https://images.pexels.com/photos/1327838/pexels-photo-1327838.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",
                    Distance = "1 KM",
                    StockAvailable = 2
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}