using OutlookInspiredApp.Repository.Service;
using System;
using System.Configuration;
using System.Data.Services.Client;
using System.Linq;

namespace OutlookInspiredApp.Repository
{
    public static class ConnectionManager
    {
        private static OutlookEntities context;

        public static OutlookEntities Context
        {
            get
            {
                return context;
            }
        }

        static ConnectionManager()
        {
            var serviceUrl = GetServiceUrl();
            context = new OutlookEntities(new Uri(serviceUrl));

            context.MergeOption = MergeOption.PreserveChanges;
            context.SaveChangesDefaultOptions = SaveChangesOptions.ContinueOnError;
            context.IgnoreMissingProperties = true;
        }

        private static string GetServiceUrl()
        {
            Configuration configurationManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection appSettings = configurationManager.AppSettings.Settings;

            var serviceUrl = appSettings["WCFServiceURL"].Value;

            return serviceUrl;
        }
    }
}
