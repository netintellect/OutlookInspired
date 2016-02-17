using OutlookInspiredApp.Repository.Core;

namespace OutlookInspiredApp.Repository.Service
{
    public class CalendarType
    {
        [EntityNotSerializableAttribute]
        public string Name { get; set; }

        [EntityNotSerializableAttribute]
        public string CalendarTypeBrush { get; set; }
    }
}
