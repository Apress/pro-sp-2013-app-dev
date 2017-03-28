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
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using Microsoft.SharePoint.Phone.Application;

namespace SampleSPWinPhoneApp
{
    /// <summary>
    /// ListItem New Form
    /// </summary>
    public partial class NewForm : PhoneApplicationPage
    {
        NewItemViewModel viewModel;

        CameraCaptureTask camera = new CameraCaptureTask();
        PhotoChooserTask photoChooser = new PhotoChooserTask();


        /// <summary>
        /// Constructor for New Form
        /// </summary>
        public NewForm()
        {
            InitializeComponent();

            viewModel = App.MainViewModel.CreateItemViewModelInstance;

            camera.Completed += new EventHandler<PhotoResult>(viewModel.OnPhotoSelectionCompleted);
            photoChooser.Completed += new EventHandler<PhotoResult>(viewModel.OnPhotoSelectionCompleted);
        }

        /// <summary>
        /// Code to execute when app navigates to New Form
        /// </summary>
        /// <param name="e" />
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            viewModel.InitializationCompleted += new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
            viewModel.ItemCreated += new EventHandler<ItemCreatedEventArgs>(OnItemCreated);

            if (!viewModel.IsInitialized)
                viewModel.Initialize();

            this.DataContext = viewModel;
        }

        /// <summary>
        /// Code to execute when app navigates away from New Form
        /// </summary>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            viewModel.InitializationCompleted -= new EventHandler<InitializationCompletedEventArgs>(OnViewModelInitialization);
            viewModel.ItemCreated -= new EventHandler<ItemCreatedEventArgs>(OnItemCreated);
        }

        /// <summary>
        /// Code to execute when ViewModel initialization completes
        /// </summary>
        void OnViewModelInitialization(object sender, InitializationCompletedEventArgs e)
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
        /// Code to execute when user clicks on cancel button on the Application
        /// </summary>
        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
        }


        /// <summary>
        /// Code to execute when user clicks on ChooseFile button to add an attachment
        /// </summary>
        private void OnChooseFileButtonClick(object sender, RoutedEventArgs e)
        {
            photoChooser.Show();
        }

        /// <summary>
        /// Code to execute when user clicks on TakenPhoto button to add an attachment
        /// </summary>
        private void OnTakePhotoButtonClick(object sender, RoutedEventArgs e)
        {
            camera.Show();
        }

        /// <summary>
        /// Code to execute when user clicks on Submit button on the NewForm to Create SharePoint ListItem
        /// </summary>
        public void OnBtnSubmitClick(object sender, EventArgs e)
        {
            viewModel.CreateItem();
        }

        /// <summary>
        /// Code to execute after item creation is completed.
        /// </summary>
        void OnItemCreated(object sender, ItemCreatedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.Message, e.Error.GetType().Name, MessageBoxButton.OK);
                    return;
                }

                App.DataProvider.ClearCache();
                this.NavigationService.Navigate(new Uri("/Views/List.xaml", UriKind.Relative));
            });
        }
    }
}
