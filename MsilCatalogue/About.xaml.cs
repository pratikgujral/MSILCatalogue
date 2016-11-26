using MsilCatalogue.Common;
using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MsilCatalogue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        string emailIdPratikGujral = @"pratikgujral@gmail.com";

        public About()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ToggleButtonReportBug_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ToggleButtonReportBug.IsChecked)
            {
                // Show the controls, allowing the user to report the bug.
                //MessageDialog msg = new MessageDialog("After entering the details, you'll be prompted to choose your email app. Send the email from there.", "NOTE:");
                //await msg.ShowAsync();
                StackPanelReportBug.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //StackPanelReportBug.Height = Double.NaN;
            }
            else
            {
                // Toggle button is off
                //StackPanelReportBug.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                clearBugForm();
            }
        }
        private void clearBugForm()
        {
            TextBoxReportBugName.Text = String.Empty;
            TextBoxReportBugPhone.Text = String.Empty;
            TextBoxReportBugDescription.Text = String.Empty;
            StackPanelReportBug.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //StackPanelReportBug.Height = 0;
            ToggleButtonReportBug.IsChecked = false;
        }

        private async void ButtonReportBugSendEmail_Click(object sender, RoutedEventArgs e)
        {
            // If Name or description is not provided.
            if (String.IsNullOrWhiteSpace(TextBoxReportBugName.Text) || String.IsNullOrWhiteSpace(TextBoxReportBugDescription.Text))
            {
                MessageDialog msg = new MessageDialog("Name or description cannot be left blank.", "INCOMPLETE INFORMATION");
                await msg.ShowAsync();
            }
            else
            {
                string name = TextBoxReportBugName.Text;
                string phone;
                string message = TextBoxReportBugDescription.Text;
                var version = Package.Current.Id.Version;
                string appVersion = String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

                if (String.IsNullOrWhiteSpace(TextBoxReportBugPhone.Text))
                    phone = "<< User did not enter >>";
                else
                    phone = TextBoxReportBugPhone.Text;

                EmailRecipient emailTo = new EmailRecipient() { Name = "Pratik Gujral", Address = emailIdPratikGujral };


                StringBuilder builderReportBug = new StringBuilder();
                builderReportBug.AppendLine("Hi! I am facing trouble in using the app. I am reporting a bug from the Windows app.").AppendLine("Here are my details");
                builderReportBug.AppendLine("\tName: " + name).AppendLine("\tContact No.: " + phone).AppendLine("\tApp version: " + appVersion).AppendLine("Description: " + message);

                EmailMessage email = new EmailMessage();
                email.To.Add(emailTo);

                email.Subject = "MSIL Catalogue App: Bug found.";
                email.Body = Convert.ToString(builderReportBug);

                await EmailManager.ShowComposeNewEmailAsync(email);
                clearBugForm();
            }
        }

        private async void HyperlinkButtonFacebookPratikGujral_Click(object sender, RoutedEventArgs e)
        {
            var success = await Launcher.LaunchUriAsync(new Uri("fb:profile?id=100005440931315")); //Pratik Gujral's id
        }

       

        private async void HyperlinkButtonFacebookUrmilaYadav_Click(object sender, RoutedEventArgs e)
        {
            var success = await Launcher.LaunchUriAsync(new Uri("fb:profile?id=100002992744820")); //Pratik Gujral's id
        }
    }
}
