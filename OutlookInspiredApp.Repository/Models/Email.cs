using OutlookInspiredApp.Repository.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace OutlookInspiredApp.Repository.Service
{
    [DataContract()]
    public partial class Email : ICloneable
    {
        private DateTime date;
        [EntityNotSerializableAttribute]
        public DateTime Date
        {
            get
            {
                return this.Received.Date;
            }
            set
            {
                if (this.date != value)
                {
                    this.Received = value;
                    this.date = value;
                    this.OnPropertyChanged("Date");
                }
            }
        }

        public object Clone()
        {
            var otherEmail = new Email();

            this.SetPropertyValues(otherEmail);

            return otherEmail;
        }

        private void SetPropertyValues(Email otherEmail)
        {
            var propertyInfo = this.GetType().GetProperties().Where(p => p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType.IsEnum || p.PropertyType.Equals(typeof(System.String))));

            foreach (PropertyInfo property in propertyInfo)
            {
                if (property.CanWrite)
                {
                    property.SetValue(otherEmail, property.GetValue(this, null), null);
                }
            }
        }
    }
}
