using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SmsManagement
{
    public class Message
    {
        [BsonElement("_id")]
        public BsonObjectId Id { get; set; }

        [BsonElement("protocol")]
        public string Protocol { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("subject")]
        public string Subject { get; set; }

        [BsonElement("body")]
        public string Body { get; set; }

        [BsonElement("toa")]
        public string Toa { get; set; }

        [BsonElement("sc_toa")]
        public string ScToa { get; set; }

        [BsonElement("service_center")]
        public string ServiceCenter { get; set; }

        [BsonElement("read")]
        public string Read { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

        [BsonElement("locked")]
        public string Locked { get; set; }

        [BsonElement("readable_date")]
        public string ReadableDate { get; set; }

        [BsonElement("contact_name")]
        public string ContactName { get; set; }

        [BsonElement("source")]
        public string Source { get; set; }

        [BsonElement("file")]
        public string File { get; set; }
    }

}
