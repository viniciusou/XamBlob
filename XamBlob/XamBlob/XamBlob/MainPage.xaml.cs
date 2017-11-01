using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamBlob
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            rootLayout.Children.Insert(0, new ScrollView
            {
                Content = _wrapLayout,
                VerticalOptions = LayoutOptions.FillAndExpand
            });

        }

        void ShowStatus(string msg, bool active = false)
        {
            txtStatus.Text = msg;
            activity.IsVisible = activity.IsRunning = active;
        }

        WrapLayout _wrapLayout = new WrapLayout();
        bool _isRefreshing;

        async void HandleAddClicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
            });

            if (file == null)
                return;

            Console.WriteLine("File: " + file.Path);

            ShowStatus("Uploading...", true);
            await BlobMan.Instance.UploadFileAsync(file.Path);
            ShowStatus("Upload done.");
        }

        async void HandleRefreshClicked(object sender, System.EventArgs e)
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;

            ShowStatus("Refreshing...", true);

            try
            {
                await UpdateAllImagesAsync();
            }
            finally
            {
                _isRefreshing = false;
                ShowStatus("Done refreshing");
            }
        }

        async Task UpdateAllImagesAsync()
        {
            _wrapLayout.Children.Clear();
            var uris = await BlobMan.Instance.GetAllBlobUrisAsync();
            foreach (var uri in uris)
            {
                var img = new Image
                {
                    Source = ImageSource.FromUri(uri),
                    WidthRequest = 70
                };

                _wrapLayout.Children.Add(img);
            }
        }
    }
}
