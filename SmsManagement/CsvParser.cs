using MongoDB.Bson;
using MongoDB.Driver;
using System.Globalization;

namespace SmsManagement
{
    public class CsvParser
    {
        public static int ParseCsv(string folderPath, IMongoCollection<BsonDocument> collection)
        {
            var messages = new List<Message>();

            foreach (var filePath in Directory.GetFiles(folderPath, "*.csv"))
            {
                string[] lines = File.ReadAllLines(filePath);
                bool finished = true;
                var message = new Message();
                int count = 0;
                foreach (string line in lines) // Skip the header row
                {
                    count++;
                    message = new Message();
                    message.Id = ObjectId.GenerateNewId();

                    string[] fields = line.Split(';'); // Split the line into fields using semicolon (;) as delimiter
                    if (fields.Length >= 5) // Ensure that the line has at least 5 fields
                    {
                        DateTime date;
                        if (DateTime.TryParseExact(fields[0], "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            message.Date = date;
                        }
                        message.Type = fields[1].ToLower() == "from" ? "received" : "sent"; // Replace "to" and "from" with "sent" and "received"
                        message.Address = fields[2];
                        if (string.IsNullOrWhiteSpace(message.Address) && message.Type == "sent")
                        {
                            message.Type = "draft";
                        }
                        message.ContactName = fields[3].Trim('"');
                        message.Body = fields[4].Trim('"');
                        message.Source = "csv";
                        message.File = filePath;
                        messages.Add(message);
                    }
                    else
                    {
                        message = messages.LastOrDefault();
                        message.Body += "\n" + line;
                    }

                }

            }
            var bsonList = new List<BsonDocument>();
            foreach (var message in messages)
            {

                var bsonMessage = message.ToBsonDocument();
                bsonList.Add(bsonMessage);
            }
            if (bsonList.Any())
            {
                collection.InsertMany(bsonList);
                Console.WriteLine($"{bsonList.Count} CSV documents inserted successfully");
            }
            return bsonList.Count;
        }
    }
}
