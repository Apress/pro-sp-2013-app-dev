using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.SharePoint.Client;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using Microsoft.Phone.Shell;
using Microsoft.SharePoint.Phone.Application;

namespace SampleSPWinPhoneApp
{
    /// <summary>
    /// ListItem Edit Form
    /// </summary>
    public partial class EditForm : PhoneApplicationPage
    {
        EditItemViewModel viewModel;

        CameraCaptureTask camera = new CameraCaptureTask();
        PhotoChooserTask photoChooser = new PhotoChooserTask();

        /// <summary>
        /// Constructor for Edit Form
        /// </summary>
        public EditForm()
        {
            InitializeComponent();

            viewModel = App.MainViewModel.SelectedItemEditViewModelInstance;

            camera.Completed += new EventHandler<PhotoResult>(viewModel.OnPhotoSelectionCompleted);
            photoChooser.Completed += new EventHandler<PhotoResult>(viewModel.OnPhotoSelectionCompleted);
        }

        /// <summary>
        /// Code to execute when app navigates to Edit Form
        /// </summary>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
            viewModel.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(OnItemUpdated);

            if (!viewModel.IsInitialized)
                viewModel.Initialize();

            this.DataContext = viewModel;
        }

        /// <summary>
        /// Code to execute when app navigates away from Edit Form
        /// </summary>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            viewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
            viewModel.ItemUpdated -= new EventHandler<ItemUpdatedEventArgs>(OnItemUpdated);
        }

        /// <summary>
        /// Code to execute when ViewModel initialization completes
        /// </summary>
        private void OnViewModelInitialization(object sender, InitializationCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                //If initialization has failed show error message and return
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                }
            });
        }

        /// <summary>
        /// Code to execute when user clicks on cancel button on the page
        /// </summary>
        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
        }


        /// <summary>
        /// Code to execute when user clicks on ChooseFile button to add an attachment to the list item
        /// </summary>
        private void OnChooseFileButtonClick(object sender, RoutedEventArgs e)
        {
            photoChooser.Show();
        }

        /// <summary>
        /// Code to execute when user clicks on TakePhoto button to add an attachment to the list item
        /// </summary>
        private void OnTakePhotoButtonClick(object sender, RoutedEventArgs e)
        {
            camera.Show();
        }

        /// <summary>
        /// Code to execute when user clicks on Submit button on the ListEdit Form to update SharePoint ListItem
        /// </summary>
        private void OnBtnSubmitClick(object sender, EventArgs e)
        {
            viewModel.UpdateItem();
        }

        /// <summary>
        /// Code to execute upon completion of item updating operations
        /// </summary>
        private void OnItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                    return;
                }

                this.NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
            });
        }
    }
}
