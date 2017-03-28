using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using Microsoft.SharePoint.Phone.Application;
using Microsoft.SharePoint.Client;
using System.Collections.ObjectModel;

namespace SampleSPWinPhoneApp
{
    [DataContract]
    public class DesignNewItemViewModel : NewItemViewModelBase
    {
        /// <summary>
        /// Provides new item values for fields of the List, given its name. Also used for data binding to New form UI
        /// </summary>
        public DesignNewItemViewModel()
        {

            //Title
            this["Title"] = "Sample Text";

            //Attachments
            this["Attachments"] = new ObservableCollection<AttachmentFieldViewModel>
                                                            { 
                                                                new AttachmentFieldViewModel { Name = "FileName1.txt", IsChecked = true },
                                                                new AttachmentFieldViewModel { Name = "Picture1.jpg", IsChecked = true }, 
                                                                new AttachmentFieldViewModel { Name = "Test.docx", IsChecked = true }
                                                            };

            //Location
            this["Location"] = "Sample Text";

            //EventDate
            this["EventDate"] = "1/21/2012";

            //EndDate
            this["EndDate"] = "1/21/2012";

            //Description
            this["Description"] = "This is a multiline text field";

            //ParticipantsPicker
            this["ParticipantsPicker"] = new UserFieldViewModel { TitleInternal = "John Doe" };

            //Category
            this["Category"] = new ObservableCollection<ChoiceFieldViewModel>
                                                            { 
                                                                new ChoiceFieldViewModel { Name = "Option 1", IsChecked = true }, 
                                                                new ChoiceFieldViewModel { Name = "Option 1", IsChecked = false }, 
                                                                new ChoiceFieldViewModel { Name = "Option 1", IsChecked = false }
                                                            };

        }


        /// <summary>
        /// Provides display values for fields of the List, given its name. Also used for data binding to New form UI
        /// </summary>
        /// <param name="fieldName" />
        /// <returns />
        public override object this[string fieldName]
        {
            get
            {
                return base[fieldName];
            }
            set
            {
                base[fieldName] = value;
            }
        }
    }
}
