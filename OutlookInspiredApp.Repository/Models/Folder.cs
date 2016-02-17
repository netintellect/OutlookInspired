using OutlookInspiredApp.Repository.Core;
using System;
using System.Linq;

namespace OutlookInspiredApp.Repository.Service
{
    public partial class Folder
    {
        private int unreadEmailsCount = -1;

        /// <summary>
        /// Gets the number of unread Email objects of the Folder.
        /// </summary>
        [EntityNotSerializableAttribute]
        public int UnreadEmailsCount
        {
            get
            {
                if (this.unreadEmailsCount == -1)
                {
                    this.CalculateUnreadItemsCount();
                }
                return this.unreadEmailsCount;
            }
            set
            {
                if (this.unreadEmailsCount != value)
                {
                    this.unreadEmailsCount = value;
                    this.OnPropertyChanged("UnreadEmailsCount");
                }
            }
        }

        public void CalculateUnreadItemsCount()
        {
            if (this.Emails != null)
            {

                this.UnreadEmailsCount = MailRepository.GetEmailsByFolder(this.FolderID).Count(i => i.Status == 0);
            }
        }

        public void UpdateUnreadItemsCount(bool incrementUnreadItemsCount)
        {
            if (incrementUnreadItemsCount)
            {
                this.UnreadEmailsCount++;
            }
            else
            {
                this.UnreadEmailsCount--;
            }
        }
    }
}
