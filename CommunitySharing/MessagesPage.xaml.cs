using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using CommunitySharing.Services;

namespace CommunitySharing
{
    public partial class MessagesPage : ContentPage
    {
        private readonly ApiService _api;
        public MessagesPage(ApiService api)
        {
            InitializeComponent();
            _api = api; // Resolve from DI
            HighlightTab("Message");
            LoadAllMessages();
        }

        private void LoadAllMessages()
        {
            var messages = new List<MessageItem>
            {
                new MessageItem { Name = "Apip the Nonchalant", Message = "Hi! The book available....", Time = "5 minutes ago" },
                new MessageItem { Name = "Arencheng", Message = "Thanks for the apples....", Time = "5 minutes ago" },
                new MessageItem { Name = "Brian Griffin", Message = "Is the tomatoes still available?....", Time = "5 hours ago" },
                new MessageItem { Name = "Jackie Chan", Message = "Is the clothes still available....", Time = "10 hours ago" },
                new MessageItem { Name = "IShowSpeed", Message = "Thanks for the book....", Time = "12 hours ago" }
            };

            MessagesList.ItemsSource = messages;
        }

        private void OnAllMessagesTapped(object sender, EventArgs e)
        {
            HighlightMessageTab("All");
            LoadAllMessages();
        }

        private void OnRequestTapped(object sender, EventArgs e)
        {
            HighlightMessageTab("Request");
            MessagesList.ItemsSource = new List<MessageItem>
            {
                new MessageItem { Name = "Request Example", Message = "Can I request more tomatoes?", Time = "1 hour ago" }
            };
        }

        private void HighlightMessageTab(string selected)
        {
            // FIX CS1061: Replacing BorderColor with Stroke for the Border control
            if (selected == "All")
            {
                AllMessagesTab.Stroke = Color.FromArgb("#34C759");
                RequestTab.Stroke = Colors.Black;
            }
            else
            {
                AllMessagesTab.Stroke = Colors.Black;
                RequestTab.Stroke = Color.FromArgb("#34C759");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnNavDiscover(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///DiscoverPage");
        }

        private async void OnNavUpload(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///UploadPage");
        }

        private async void OnNavMessage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///MessagesPage");
        }

        private async void OnNavProfile(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///ProfilePage");
        }


        private void HighlightTab(string selected)
        {
            ResetTabColors(DiscoverTab);
            ResetTabColors(UploadTab);
            ResetTabColors(MessageTab);
            ResetTabColors(ProfileTab);

            switch (selected)
            {
                case "Discover": SetActiveTab(DiscoverTab); break;
                case "Upload": SetActiveTab(UploadTab); break;
                case "Message": SetActiveTab(MessageTab); break;
                case "Profile": SetActiveTab(ProfileTab); break;
            }
        }

        private void ResetTabColors(VerticalStackLayout tab)
        {
            if (tab.Children[0] is Image icon)
                icon.Opacity = 0.6;
            if (tab.Children[1] is Label label)
                label.TextColor = Colors.Black;
        }

        private void SetActiveTab(VerticalStackLayout tab)
        {
            if (tab.Children[0] is Image icon)
                icon.Opacity = 1.0;
            if (tab.Children[1] is Label label)
                label.TextColor = Color.FromArgb("#34C759");
        }
    }

    public class MessageItem
    {
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }
}