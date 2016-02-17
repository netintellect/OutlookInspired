using OutlookInspiredApp.Repository.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OutlookInspiredApp.Repository
{
    public class MailRepository : RepositoryBase
    {
        private static readonly Dictionary<int, IEnumerable<Email>> emailsCache = new Dictionary<int, IEnumerable<Email>>();

        private static IEnumerable<Folder> folders;
        private static ObservableCollection<Folder> parentFolders;

        public static Folder DeletedItemsFolder  {get; private set;}
        public static Folder SentItemsFolder  {get; private set;}
        public static Folder InboxFolder  {get; private set;}

        public static IEnumerable<Folder> Folders
        {
            get
            {
                return folders;
            }
        }

        public static ObservableCollection<Folder> ParentFolders
        {
            get
            {
                if(parentFolders == null)
                {
                    parentFolders = new ObservableCollection<Folder>();
                }
                return parentFolders;
            }
            set
            {
                parentFolders = value;
            }
        }

        public static IEnumerable<Folder> GetFolders()
        {
            return Context.Folders.Expand("Folders1");
        }

        public static void GetEmailClient(Action<EmailClient> callback)
        {
            ExecuteQueryAsync<EmailClient>(GetEmailClientAsync(), callback);
        }

        private static async Task<EmailClient> GetEmailClientAsync()
        {
            return await Task.Run(() => Context.EmailClients.FirstOrDefault());
        }

        public static IEnumerable<Folder> GetParentFolders()
        {
            return Folders.Where(f => f.ParentFolderID == null);
        }

        public static Folder GetInboxFolder()
        {
            return Folders.Where(f => f.Name == "Inbox").Single();
        }

        public static Folder GetDeletedItemsFolder()
        {
            return Folders.Where(f => f.Name == "Deleted Items").Single();
        }

        public static Folder GetSentItemsFolder()
        {
            return Folders.Where(f => f.Name == "Sent Items").Single();
        }

        public static IEnumerable<Email> GetEmailsByFolder(int folderID)
        {
            if (!emailsCache.ContainsKey(folderID))
            {
                var emails = Context.Emails.Expand(e => e.Folder).Expand(e => e.Category).Where(e => e.FolderID != null && e.FolderID == folderID).ToList();
                emailsCache.Add(folderID, emails);
            }
            return emailsCache[folderID];
        }

        public static async Task UpdateFolderEmailsCache(int folderID)
        {
            if (emailsCache.ContainsKey(folderID))
            {
                var emails = Context.Emails.Expand(e => e.Folder).Expand(e => e.Category).Where(e => e.FolderID != null && e.FolderID == folderID).ToList();
                emailsCache[folderID] = emails;
            }
        }

        private static async Task<IEnumerable<Email>> GetEmailsAsync(int folderID)
        {
            return await Task.Run(() => Context.Emails.Expand(e => e.Folder).Expand(e => e.Category).Where(e => e.FolderID != null && e.FolderID == folderID));
        }


        private async static Task<IEnumerable<Folder>> GetFoldersAsync()
        {
            return await Task.Run(() => Context.Folders.Expand("Folders1"));
        }

        public static void InsertEmail(object entity)
        {
            Context.AddToEmails((Email)entity);
        }

        public override bool Contains(object entity)
        {
            return Context.Emails.Where(e => e.EmailID == ((Email)entity).EmailID).Count() >= 1;
        }

        public static async Task LoadData()
        {
            folders = await GetFoldersAsync();

            DeletedItemsFolder = GetDeletedItemsFolder();
            InboxFolder = GetInboxFolder();
            SentItemsFolder = GetSentItemsFolder();

            var mainFolders = GetParentFolders().ToList();
            mainFolders.ForEach(f => ParentFolders.Add(f));
        }
    }
}
