using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using OutlookInspiredApp.Repository;
using Service = OutlookInspiredApp.Repository.Service;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TelerikOutlookInspiredApp
{
    public partial class MailViewModel
    {
        private Window editedEmailWindow;

        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand DiscardCommand { get; set; }
        public DelegateCommand ElementExportingCommand { get; private set; }
        public DelegateCommand ExportToXlsxCommand { get; private set; }
        public DelegateCommand FindGridViewCommand { get; private set; }
        public DelegateCommand FindRichTextBoxCommand { get; private set; }
        public DelegateCommand ForwardCommand { get; private set; }
        public DelegateCommand MarkUnreadCommand { get; private set; }
        public DelegateCommand MarkUnreadReadCommand { get; private set; }
        public DelegateCommand MenuOpenStateChangedCommand { get; private set; }
        public DelegateCommand NewMailCommand { get; private set; }
        public DelegateCommand OpenedCommand { get; private set; }
        public DelegateCommand OpenDialogCommand { get; private set; }
        public DelegateCommand PopOutCommand { get; private set; }
        public DelegateCommand PrintCommand { get; private set; }
        public DelegateCommand ReplyAllCommand { get; private set; }
        public DelegateCommand ReplyCommand { get; private set; }
        public DelegateCommand SendCommand { get; private set; }
        public DelegateCommand SetCategoryCommand { get; private set; }
        public DelegateCommand SetFlagCommand { get; private set; }
        public DelegateCommand ViewUnreadEmailsCommand { get; private set; }
        public DelegateCommand ViewAllEmailsCommand { get; private set; }

        public MailViewModel()
        {
            this.InitializeProperties();
            this.InitializeCommands();
            this.LoadData();
        }

        private void InitializeCommands()
        {
            this.DeleteCommand = new DelegateCommand(this.OnDeleteCommandExecuted, o => this.SelectedEmail != null);
            this.DiscardCommand = new DelegateCommand(this.OnDiscardCommandExecute, o => this.HasSelectedEmail);
            this.ElementExportingCommand = new DelegateCommand(this.OnElementExportingCommandExecuted);
            this.ExportToXlsxCommand = new DelegateCommand(this.OnExportToXlsxCommandExecuted);
            this.FindGridViewCommand = new DelegateCommand(this.OnFindGridViewCommandExecuted);
            this.FindRichTextBoxCommand = new DelegateCommand(this.OnFindRichTextBoxCommandExecuted);
            this.ForwardCommand = new DelegateCommand(this.OnForwardCommandExecuted, o => this.HasSelectedEmail);
            this.MarkUnreadCommand = new DelegateCommand(this.OnMarkUnreadCommandExecuted, this.CanMarkUnreadCommandExecute);
            this.MarkUnreadReadCommand = new DelegateCommand(this.OnMarkUnreadReadCommandExecuted, o => this.SelectedEmail != null);
            this.MenuOpenStateChangedCommand = new DelegateCommand(this.OnMenuOpenStateChangedCommandExecuted);
            this.NewMailCommand = new DelegateCommand(this.OnNewMailCommand);
            this.OpenDialogCommand = new DelegateCommand(this.OnOpenDialogCommandExecuted);
            this.OpenedCommand = new DelegateCommand(this.OnOpenedCommandExecuted);
            this.PopOutCommand = new DelegateCommand(this.OnPopOutCommandExecuted, o => this.SelectedEmail != null);
            this.PrintCommand = new DelegateCommand(this.OnPrintCommandExecuted, o => this.SelectedEmail != null);
            this.ReplyAllCommand = new DelegateCommand(this.OnReplyAllCommandExecuted, o => this.HasSelectedEmail);
            this.ReplyCommand = new DelegateCommand(this.OnReplyCommandExecuted, o => this.HasSelectedEmail);
            this.SendCommand = new DelegateCommand(this.OnSendCommandExecuted);
            this.SetCategoryCommand = new DelegateCommand(this.OnSetCategoryCommandExecuted, o => this.SelectedEmail != null);
            this.SetFlagCommand = new DelegateCommand(this.OnSetFlagCommandExecuted, o => this.SelectedEmail != null);
            this.ViewUnreadEmailsCommand = new DelegateCommand(this.OnViewUnreadEmailsCommandExecuted);
            this.ViewAllEmailsCommand = new DelegateCommand(this.OnViewAllEmailsCommandExecuted);
        }

        private void InitializeProperties()
        {
            this.OutlookBarHeader = "Mail";

            this.unreadFilterDescriptor = new FilterDescriptor
            {
                Member = "Status",
                Operator = FilterOperator.IsEqualTo,
                Value = (int)EmailStatus.Unread,
            };
        }

        private void LoadData()
        {
            Task.Run(()=> MailRepository.LoadData());
        }

        private void OnDiscardCommandExecute(object parameter)
        {
            this.EditedEmailContent = this.SelectedEmail.Body;

            this.ExitEditMode();
        }

        private void ExitEditMode()
        {
            this.IsInEditMode = false;
        }

        private void OnElementExportingCommandExecuted(object parameter)
        {
            var args = parameter as GridViewCellExportingEventArgs;
            if (args != null)
            {
                var column = args.Column as GridViewDataColumn;
                args.Cancel = column == null || (column != null && column.DisplayIndex > 3);
            }
        }

        private void OnExportToXlsxCommandExecuted(object parameter)
        {
            if (this.GridView != null)
            {
                var exportOptions = new GridViewDocumentExportOptions()
                {
                    AutoFitColumnsWidth = true,
                    ExportDefaultStyles = true,
                    ShowColumnHeaders = this.GridView.ShowColumnHeaders
                };

                var dialog = new SaveFileDialog()
                {
                    DefaultExt = "xlsx",
                    Filter = String.Format("(*.{0})|*.{1}", "xlsx", "xlsx")
                };

                if (dialog.ShowDialog() == true)
                {
                    using (var stream = dialog.OpenFile())
                    {
                        this.gridView.ExportToXlsx(stream, exportOptions);
                    }
                }
            }
        }

        public void OnPrintCommandExecuted(object obj)
        {
            if (this.RichTextBox != null)
            {
                this.RichTextBox.Print(this.NewEmail != null ? this.NewEmail.Subject : this.SelectedEmail.Subject, Telerik.Windows.Documents.UI.PrintMode.Native);
            }
        }

        public void OnFindGridViewCommandExecuted(object obj)
        {
            var gridView = obj as RadGridView;
            if (gridView != null)
            {
                this.GridView = gridView;
            }
        }

        private void OnForwardCommandExecuted(object parameter)
        {
            this.EditedEmail = this.SelectedEmail.Clone() as Service.Email;

            this.SetEditedEmailAddresses(null);

            var subject = String.Format("FW: {0}", this.SelectedEmail.Subject);
            this.SetEditedEmailSubject(subject);

            this.IsInEditMode = true;

            this.InvalidatePopOutCommand();
        }

        private void OnReplyCommandExecuted(object obj)
        {
            this.EditedEmail = this.SelectedEmail.Clone() as Service.Email;
            this.SetEditedEmailAddresses(this.EditedEmail.SenderAddress);

            var subject = String.Format("RE: {0}", this.SelectedEmail.Subject);
            this.SetEditedEmailSubject(subject);

            this.IsInEditMode = true;

            this.InvalidatePopOutCommand();
        }

        private void OnReplyAllCommandExecuted(object parameter)
        {
            this.EditedEmail = this.SelectedEmail.Clone() as Service.Email;
            var subject = String.Format("RE: {0}", this.SelectedEmail.Subject);
            this.SetEditedEmailSubject(subject);

            var recipientAddress = String.Format("{0};{1}", this.SelectedEmail.SenderAddress, this.SelectedEmail.CarbonCopy);
            this.SetEditedEmailAddresses(recipientAddress);

            this.IsInEditMode = true;

            this.InvalidatePopOutCommand();
        }

        private void SetEditedEmailAddresses(string recipientAddress)
        {
            this.EditedEmail.SenderAddress = this.EmailClient.EmailAddress;
            this.EditedEmail.RecipientAddress = recipientAddress;
        }

        private void SetEditedEmailSubject(string subject)
        {
            this.EditedEmail.Subject = subject;
        }

        public void OnFindRichTextBoxCommandExecuted(object obj)
        {
            var richTextBox = obj as RadRichTextBox;
            if (richTextBox != null)
            {
                this.RichTextBox = richTextBox;
            }
        }

        private void OnMarkUnreadReadCommandExecuted(object parameter)
        {
            if (this.SelectedEmail != null)
            {
                this.SelectedEmail.Status = this.SelectedEmail.Status == (int)EmailStatus.Unread ? (int)EmailStatus.Read : (int)EmailStatus.Unread;

                this.SaveSelectedEmailChanges();

                this.SelectedEmail.Folder.UpdateUnreadItemsCount(this.SelectedEmail.Status == (int)EmailStatus.Unread);
            }
        }

        private void SaveSelectedEmailChanges()
        {
            MailRepository.Update(this.SelectedEmail);

            MailRepository.SaveChangesAsync();
        }

        private void OnMenuOpenStateChangedCommandExecuted(object parameter)
        {
            var ribbon = parameter as RadRichTextBoxRibbonUI;
            if (ribbon.IsApplicationMenuOpen)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    this.DiscardCommand.Execute(null);
                }));
            }
        }

        private void OnOpenDialogCommandExecuted(object obj)
        {
            RadWindow.Alert(
                new DialogParameters
                {
                    Content = string.Format("{0}'s command executed.", obj.ToString()),
                    Header = "Telerik"
                });
        }

        private void OnPopOutCommandExecuted(object obj)
        {
            this.NewEmail = this.EditedEmail.Clone() as Service.Email;

            this.ShowNewEmailWindow();
            this.IsInEditMode = false;
        }

        private void ShowNewEmailWindow()
        {
            var window = new NewEmailWindow(this);
            window.Owner = Application.Current.MainWindow;
            window.Show();

            this.SetEditedEmailWindow(window);
        }

        private void CloseNewEmailWindow()
        {
            if (this.editedEmailWindow != null)
            {
                this.NewEmail = null;

                this.editedEmailWindow.Close();
                this.editedEmailWindow = null;
            }
        }

        private void SetEditedEmailWindow(NewEmailWindow window)
        {
            this.editedEmailWindow = window;
        }

        private bool CanMarkUnreadCommandExecute(object obj)
        {
            return this.SelectedEmail != null && this.SelectedEmail.Status == (int)EmailStatus.Read;
        }

        private void OnMarkUnreadCommandExecuted(object parameter)
        {
            this.SelectedEmail.Status = (int)EmailStatus.Unread;

            this.SaveSelectedEmailChanges();

            this.SelectedEmail.Folder.UpdateUnreadItemsCount(true);
        }

        private void OnNewMailCommand(object parameter)
        {
            this.NewEmail = new Service.Email();

            this.ShowNewEmailWindow();
        }

        private void OnOpenedCommandExecuted(object parameter)
        {
            var args = parameter as RadRoutedEventArgs;
            if (args != null)
            {
                var menu = args.OriginalSource as RadContextMenu;
                if (menu != null)
                {
                    this.MarkUnreadCommand.InvalidateCanExecute();

                    var row = menu.GetClickedElement<GridViewRow>();
                    if (row != null)
                    {
                        row.IsSelected = row.IsCurrent = true;
                        GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                        if (cell != null)
                        {
                            cell.IsCurrent = true;
                        }
                    }
                    else
                    {
                        menu.IsOpen = false;
                    }
                }
            }
        }

        private void OnDeleteCommandExecuted(object parameter)
        {
            var email = parameter as Service.Email;

            if (email != null)
            {
                var folder = email.Folder;
                if (folder != null)
                {
                    var originalFolderID = folder.FolderID;
                    if (MailRepository.DeletedItemsFolder != null && originalFolderID == MailRepository.DeletedItemsFolder.FolderID)
                    {
                        this.DeleteEmail(email);
                    }
                    else
                    {
                        this.MoveToDeletedItemsFolder(email);
                    }

                    MailRepository.SaveChangesAsync();

                    Task.Run(() => MailRepository.UpdateFolderEmailsCache(MailRepository.DeletedItemsFolder.FolderID));
                }
            }
        }

        private void DeleteEmail(Service.Email email)
        {
            this.Emails.Remove(email);

            MailRepository.Context.DeleteObject(email);
        }

        private void MoveToDeletedItemsFolder(Service.Email email)
        {
            email.Folder = MailRepository.DeletedItemsFolder;
            email.FolderID = MailRepository.DeletedItemsFolder.FolderID;

            MailRepository.Update(email);
            this.Emails.Remove(email);
        }

        private void OnViewAllEmailsCommandExecuted(object parameter)
        {
            this.Emails = new ObservableCollection<Service.Email>(MailRepository.GetEmailsByFolder(this.SelectedFolder.FolderID));
        }

        private void OnViewUnreadEmailsCommandExecuted(object parameter)
        {
            this.Emails = new ObservableCollection<Service.Email>(this.Emails.Where(e => e.Status == (int)EmailStatus.Unread));
        }

        public void OnSendCommandExecuted(object parameter)
        {
            var currentEmail = this.NewEmail != null ? this.NewEmail : this.EditedEmail;
            if (this.IsMessageValid(currentEmail))
            {
                var email = currentEmail;
                email.SenderAddress = this.EmailClient.EmailAddress;
                email.FolderID = MailRepository.SentItemsFolder != null ? (int?)MailRepository.SentItemsFolder.FolderID : null;
                email.Status = (int)EmailStatus.Read;
                email.Received = DateTime.Now;
                email.EmailClientID = this.EmailClient.ID;
                email.Body = this.NewEmail != null ? this.NewEmail.Body : this.EditedEmailContent;

                MailRepository.Context.AddToEmails(email);

                MailRepository.SaveChangesAsync();

                this.CloseNewEmailWindow();
                this.ExitEditMode();

                Task.Run(() => MailRepository.UpdateFolderEmailsCache(MailRepository.SentItemsFolder.FolderID));
            }
        }

        private void OnSetCategoryCommandExecuted(object parameter)
        {
            var selectedCategory = parameter as Telerik.Windows.Controls.Category;
            if (selectedCategory != null && this.SelectedEmail != null)
            {
                var newCategory = this.GetNewCategory(selectedCategory);
                if (newCategory != null)
                {
                    SetCategory(this.SelectedEmail, newCategory);
                }

                MailRepository.Update(this.SelectedEmail);
            }
            else
            {
                SetCategory(this.SelectedEmail, null);
            }

            MailRepository.SaveChangesAsync();
        }


        public Service.Category GetNewCategory(Telerik.Windows.Controls.Category selectedCategory)
        {
            var newCategory = this.Categories.FirstOrDefault(c => c.DisplayName == selectedCategory.DisplayName);
            if (newCategory != null)
            {
                return newCategory;
            }
            return null;
        }
        private static void SetCategory(Service.Email email, Service.Category category)
        {
            email.Category = category;
            email.CategoryID = category != null ? (int?)category.CategoryID : null;
        }

        public void OnSetFlagCommandExecuted(object parameter)
        {
            if (parameter != null)
            {
                var flag = (FollowUpType)parameter;

                this.SelectedEmail.Flag = (int)flag;
            }
            else
            {
                this.SelectedEmail.Flag = null;
            }

            MailRepository.SaveChangesAsync();
        }


        public bool IsMessageValid(Service.Email email)
        {
            var isRecipientAddressValid = IsEmailValid(email.RecipientAddress);
            var isSubjectEmpty = string.IsNullOrEmpty(email.Subject);

            if (!isRecipientAddressValid)
            {
                this.DisplayAlert("Please make sure you enter at least one correct recipient address.");
            }
            else if (isSubjectEmpty)
            {
                this.DisplayAlert("This message must have a subject.");
            }
            return isRecipientAddressValid && !isSubjectEmpty;
        }

        public static bool IsEmailValid(string email)
        {
            if (email != null)
            {
                return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                    && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
            }
            return false;
        }

        private void DisplayAlert(string message)
        {
            RadWindow.Alert(
               new DialogParameters
               {
                   Content = message,
                   Header = "Telerik"
               });
        }

        private void InvalidatePopOutCommand()
        {
            this.PopOutCommand.InvalidateCanExecute();
        }

        private void InvalidateCommands()
        {
            this.ReplyCommand.InvalidateCanExecute();
            this.ReplyAllCommand.InvalidateCanExecute();
            this.ForwardCommand.InvalidateCanExecute();
            this.DiscardCommand.InvalidateCanExecute();
            this.DeleteCommand.InvalidateCanExecute();
        }

        private void InvalidateSelectedEmailCommands()
        {
            this.MarkUnreadReadCommand.InvalidateCanExecute();
            this.SetCategoryCommand.InvalidateCanExecute();
            this.SetFlagCommand.InvalidateCanExecute();
        }
    }
}
