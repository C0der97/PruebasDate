using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Pruebacamara
{
    public partial class MainPage : ContentPage
    {
        public object PhotoPath { get; private set; }

        private bool GreaterEleven { get; set; } = false;

        public MainPage()
        {
            InitializeComponent();
            Fecha.MaximumDate = DateTime.Today;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                var statusDos = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                if (status == PermissionStatus.Granted &&
                    statusDos == PermissionStatus.Granted)
                {
                    TakePhotoAsync().GetAwaiter().GetResult();
                }
            });
        }

        async Task TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {PhotoPath}");
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        async Task LoadPhotoAsync(FileResult photo)
        {
            // canceled
            if (photo == null)
            {
                PhotoPath = null;
                return;
            }
            // save the file into local storage
            var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(newFile))
                await stream.CopyToAsync(newStream);

            PhotoPath = newFile;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Entry entryDate = sender as Entry;
            entryDate.TextChanged -= Entry_TextChanged;
            string newValue = e.NewTextValue;
            bool erasingData = CheckErasingEntry(e);
            //01/12/2022
            const string patternDate = @"^\d{2}/\d{2}/\d{4}$";
            //0/12/1312
            const string patternAfterReasedOne = @"^\d{1}/\d{2}/\d{4}";
            //0/1/1312
            const string patternAfterReasedTwo = @"^\d{1}/\d{1}/\d{4}";
            //01/1/1312
            const string patternAfterReasedThree = @"^\d{2}/\d{1}/\d{4}";

            const string separator = "/";

            if (erasingData && !GreaterEleven)
            {
               // newValue = newValue.Replace(separator, string.Empty);
               // entryDate.Text = newValue;
                entryDate.TextChanged += Entry_TextChanged;
                return;
            }
            /*
                if (!Regex.IsMatch(newValue, patternDate, RegexOptions.IgnoreCase) && (Regex.IsMatch(newValue, patternAfterReasedOne, RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(newValue, patternAfterReasedTwo, RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(newValue, patternAfterReasedThree, RegexOptions.IgnoreCase)))
                {
                    entryDate.Text = AddSeparatorWriting(newValue);
                }
                entryDate.TextChanged += Entry_TextChanged;
                return;
            }*/

            if (!Regex.IsMatch(newValue, patternDate, RegexOptions.IgnoreCase))
            {
                if (newValue.Length > 10)
                {
                    GreaterEleven = true;
                    newValue = newValue.Remove(10, 1).Substring(0, 10);
                }
                else
                {
                    GreaterEleven = false;
                }
                newValue = AddSeparatorWriting(newValue);
            }
            entryDate.Text = newValue;
            entryDate.TextChanged += Entry_TextChanged;
        }

        private string AddSeparatorWriting(string text)
        {

            const string separator = "/";

            text = text.Replace(separator, string.Empty);

            // 12
            const string patternTwoNumbers = @"^\d{2}";
            // 12/12/
            const string patternFourNumbers = @"^\d{2}/\d{2}";

            int separatorCount = Regex.Matches(text, separator).Count;


            if (Regex.IsMatch(text, patternTwoNumbers, RegexOptions.IgnoreCase) && separatorCount == 0)
            {
                text = text.Insert(2, separator);
            }

            separatorCount = Regex.Matches(text, separator).Count;

            if (Regex.IsMatch(text, patternFourNumbers, RegexOptions.IgnoreCase) && separatorCount == 1)
            {
                text = text.Insert(5, separator);
            }

            return text;
        }

        private string AddSeparatorDeleting(string text)
        {
            const string separator = "/";
            // 12
            const string patternTwoNumbers = @"^\d{2}";
            // 12/12/
            const string patternFourNumbers = @"^\d{2}/\d{2}";

            if (Regex.IsMatch(text, patternTwoNumbers, RegexOptions.IgnoreCase))
            {
                text = text.Insert(2, separator);
            }
            
            if (Regex.IsMatch(text, patternFourNumbers, RegexOptions.IgnoreCase))
            {
                text = text.Insert(5, separator);
            }

            return text;
        }

        private bool CheckErasingEntry(TextChangedEventArgs eventData)
        {
            return eventData.NewTextValue?.Length
                < eventData.OldTextValue?.Length;
        }
    }
}
