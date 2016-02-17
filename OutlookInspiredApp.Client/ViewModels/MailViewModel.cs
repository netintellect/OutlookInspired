using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OutlookInspiredApp.Repository;
using Controls = Telerik.Windows.Controls;
using Telerik.Windows.Data;
using OutlookInspiredApp.Repository.Service;

namespace TelerikOutlookInspiredApp
{
    public partial class MailViewModel : Controls.ViewModelBase
    {
        private bool isInEditMode;
        private bool hasSelectedEmail;
        private Email selectedEmail;
        private string editedEmailContent;
        private string outlookBarHeader;
        private ObservableCollection<Email> emails;
        private EmailClient emailClient;
        private Email editedEmail;
        private Email newEmail;
        private Folder selectedFolder;
        private Controls.RadGridView gridView;
        private Controls.RadRichTextBox richTextBox;
        private FilterDescriptor unreadFilterDescriptor;
        private IEnumerable<Category> categories;

        public EmailClient EmailClient
        {
            get
            {
                if (this.emailClient == null)
                {
                    MailRepository.GetEmailClient(item =>
                        this.emailClient = item);
                }
                return this.emailClient;
            }
            set
            {
                if (this.emailClient != value)
                {
                    this.emailClient = value;
                    this.OnPropertyChanged("EmailClient");
                }
            }
        }

        public Folder SelectedFolder
        {
            get
            {
                if (this.selectedFolder == null)
                {
                    this.selectedFolder = MailRepository.InboxFolder;
                    
                    this.UpdateEmailsCollection();
                }
                return this.selectedFolder;
            }
            set
            {
                if (this.selectedFolder != value)
                {
                    this.selectedFolder = value;
                    this.OnPropertyChanged("SelectedFolder");

                    this.UpdateEmailsCollection();
                }
            }
        }

        public ObservableCollection<Email> Emails
        {
            get
            {
                return this.emails;
            }
            set
            {
                if (this.emails != value)
                {
                    this.emails = value;
                    this.OnPropertyChanged("Emails");
                }
            }
        }

        private void UpdateEmailsCollection()
        {
            if (this.SelectedFolder != null)
            {
                this.Emails = new ObservableCollection<Email>(MailRepository.GetEmailsByFolder(this.SelectedFolder.FolderID));
            }
        }

        public string EditedEmailContent
        {
            get
            {
                return this.editedEmailContent;
            }
            set
            {
                if (this.editedEmailContent != value)
                {
                    this.editedEmailContent = value;
                    this.OnPropertyChanged(() => this.EditedEmailContent);
                }
            }
        }

        public Email NewEmail
        {
            get
            {
                return this.newEmail;
            }
            set
            {
                if (this.newEmail != value)
                {
                    this.newEmail = value;
                    this.OnPropertyChanged("NewEmail");
                }
            }
        }

        public Email EditedEmail
        {
            get
            {
                return this.editedEmail;
            }
            set
            {
                if (this.editedEmail != value)
                {
                    this.editedEmail = value;
                    this.OnPropertyChanged("EditedEmail");
                }
            }
        }

        /// <summary>
        /// Gets or sets HasEmail and notifies for changes
        /// </summary>
        public bool HasSelectedEmail
        {
            get
            {
                return this.hasSelectedEmail;
            }
            set
            {
                if (this.hasSelectedEmail != value)
                {
                    this.hasSelectedEmail = value;
                    this.InvalidateCommands();
                    this.OnPropertyChanged(() => this.HasSelectedEmail);
                }
            }
        }

        public string OutlookBarHeader
        {
            get
            {
                return this.outlookBarHeader;
            }
            set
            {
                if (this.outlookBarHeader != value)
                {
                    this.outlookBarHeader = value;
                    this.OnPropertyChanged("OutlookBarHeader");
                }
            }
        }

        /// <summary>
        /// Gets or sets SelectedEmail and notifies for changes
        /// </summary>
        public Email SelectedEmail
        {
            get
            {
                return this.selectedEmail;
            }
            set
            {
                if (this.selectedEmail != value)
                {
                    this.selectedEmail = value;
                    if (value != null && this.selectedEmail.SenderAddress != null)
                    {
                        this.EditedEmailContent = this.selectedEmail.Body;
                        this.HasSelectedEmail = true;
                    }
                    else
                    {
                        this.HasSelectedEmail = false;
                        this.IsInEditMode = false;
                    }

                    if (this.selectedEmail != null && this.selectedEmail.Status == (int)EmailStatus.Unread)
                    {
                        this.UpdateSelectedEmailStatus();
                        this.UpdateFolderUnreadItemsCount();
                    }

                    this.InvalidateSelectedEmailCommands();
                    this.OnPropertyChanged(() => this.SelectedEmail);
                }
            }
        }

        /// <summary>
        /// Gets or sets IsInEditMode and notifies for changes
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return this.isInEditMode;
            }

            set
            {
                if (this.isInEditMode != value)
                {
                    this.isInEditMode = value;
                    this.OnPropertyChanged(() => this.IsInEditMode);
                }
            }
        }

        public Controls.RadRichTextBox RichTextBox
        {
            get
            {
                return this.richTextBox;
            }
            set
            {
                if (this.richTextBox != value)
                {
                    this.richTextBox = value;
                    this.OnPropertyChanged("RichTextBox");
                }
            }
        }

        /// <summary>
        /// Gets or sets the instance of the RadGridView.
        /// </summary>
        public Controls.RadGridView GridView
        {
            get
            {
                return this.gridView;
            }
            set
            {
                if (this.gridView != value)
                {
                    this.gridView = value;
                    this.OnPropertyChanged("GridView");
                }
            }
        }

        /// <summary>
        /// Gets or sets Categories and notifies for changes
        /// </summary>
        public IEnumerable<Category> Categories
        {
            get
            {
                if (this.categories == null)
                {
                    this.categories = RepositoryBase.Categories;
                }
                return this.categories;
            }
        }

        private void UpdateSelectedEmailStatus()
        {
            if (this.SelectedEmail != null && this.SelectedEmail.Status == (int)EmailStatus.Unread)
            {
                this.SelectedEmail.Status = (int)EmailStatus.Read;

                MailRepository.SaveChangesAsync();
            }
        }

        private void UpdateFolderUnreadItemsCount()
        {
            var folder = this.SelectedFolder;
            if (folder != null)
            {
                folder.UpdateUnreadItemsCount(false);
            }
        }
    }
}
