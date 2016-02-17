using OutlookInspiredApp.Repository.Service;
using OutlookInspiredApp.Repository.Core;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OutlookInspiredApp.Repository
{
    public abstract class RepositoryBase
    {
        private static readonly string propertyPrefix = "d";
        private static readonly string metadataPrefix = "m";
        private static readonly string propertiesXNamePrefix = "properties";

        private static IEnumerable<Category> categories;

        public static IEnumerable<Category> Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = Context.Categories.ToList();
                }
                return categories;
            }
        }

        public static OutlookEntities Context
        {
            get
            {
                return ConnectionManager.Context;
            }
        }

        static RepositoryBase()
        {
            Context.WritingEntity += Context_WritingEntity;
        }

        protected static async void ExecuteQueryAsync<T>(DataServiceQuery<T> query, Action<IEnumerable<T>> callback)
        {
            var result = await Task<IEnumerable<T>>.Factory.FromAsync(query.BeginExecute, query.EndExecute, null);

            if (callback != null)
            {
                callback(result.ToList<T>());
            }
        }

        protected static async void ExecuteQueryAsync<T>(Task<IEnumerable<T>> task, Action<IEnumerable<T>> callback)
        {
            var result = await task;
            callback(result);
        }

        protected static async void ExecuteQueryAsync<T>(Task<T> task, Action<T> callback)
        {
            var result = await task;
            callback(result);
        }

        public abstract bool Contains(object entity);

        public static void Update(object entity)
        {
            Context.UpdateObject(entity);
        }

        public async static void SaveChangesAsync()
        {
            await Task.Run(() => SaveChanges());
        }

        public static void SaveChanges(Action callback = null)
        {
            try
            {
                var result = Context.SaveChanges();
                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error occured while saving changes: {0}", ex.Message));
            }
            finally
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        private static void Context_WritingEntity(object sender, ReadingWritingEntityEventArgs e)
        {
            XName xNameEntityProperties = XName.Get(propertiesXNamePrefix, e.Data.GetNamespaceOfPrefix(metadataPrefix).NamespaceName);
            XElement xElementPayload = null;
            foreach (PropertyInfo property in e.Entity.GetType().GetProperties())
            {
                object[] notSerializableAttributes = property.GetCustomAttributes(typeof(EntityNotSerializableAttribute), false);
                if (notSerializableAttributes.Length > 0)
                {
                    if (xElementPayload == null)
                    {
                        xElementPayload = e.Data.Descendants().Where<XElement>(xe => xe.Name == xNameEntityProperties)
                                                              .First<XElement>();
                    }

                    XName xNameProperty = XName.Get(property.Name, e.Data.GetNamespaceOfPrefix(propertyPrefix).NamespaceName);
                    foreach (XElement xElementRemoveThisProperty in xElementPayload.Descendants(xNameProperty).ToList())
                    {
                        xElementRemoveThisProperty.Remove();
                    }
                }
            }
        }
    }
}
