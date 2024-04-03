using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsManagement
{
    public class VmgParser
    {
        // Define a regular expression pattern to match the name
        public static string namePatternfromFile = @"^\d+\s+(.*?)\.\w+$";
        public static int ParseVmgFiles(string folderPath, MongoDB.Driver.IMongoCollection<MongoDB.Bson.BsonDocument> collection)
        {
            var messages = new List<Message>();

            foreach (var filePath in Directory.GetFiles(folderPath, "*.vmg"))
            {
                //string content = File.ReadAllText(filePath, Encoding.Unicode);
                var message = new Message();
                message.Id = ObjectId.GenerateNewId();
                using (StreamReader reader = new StreamReader(filePath, Encoding.Unicode))
                {
                    string line;
                    bool isVbody = false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("BEGIN:VBODY"))
                        {
                            isVbody = true;
                        }
                        else if (line.StartsWith("END:VBODY"))
                        {
                            isVbody = false;
                        }
                        else if (line.StartsWith("N:"))
                        {
                            message.ContactName = line.Substring(2);
                        }
                        else if (line.StartsWith("TEL:"))
                        {
                            message.Address = line.Substring(4);
                            if (!string.IsNullOrEmpty(message.Address) && message.Type == "draft")
                            {
                                message.Type = "sent";
                            }
                        }
                        else if (line.StartsWith("Date:"))
                        {
                            DateTime date;

                            // Try parsing with the first format
                            if (DateTime.TryParseExact(line.Substring(5), "d.M.yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out date))
                            {
                                // Parsing succeeded using the first format
                                message.Date = date;
                            }
                            else if (DateTime.TryParseExact(line.Substring(5), "dd-MMM-yy h:mm:ss tt", null, System.Globalization.DateTimeStyles.None, out date))
                            {
                                // Parsing succeeded using the second format
                                message.Date = date;
                            }
                            else if (DateTime.TryParseExact(line.Substring(5), "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out date))
                            {
                                // Parsing succeeded using the second format
                                message.Date = date;
                            }
                            else
                            {
                                // Both parsing attempts failed, handle the error or set a default value
                                Console.WriteLine("Failed to parse the date string.");
                                // You can set a default value like:
                                // message.Date = DateTime.MinValue;
                            }

                        }
                        else if (line.StartsWith("X-IRMC-BOX:"))
                        {
                            if (line.EndsWith("INBOX"))
                            {
                                message.Type = "received";
                            }
                            else if (line.EndsWith("DRAFT") && !string.IsNullOrEmpty(message.Address))
                            {
                                message.Type = "sent";
                            }
                            else if (line.EndsWith("DRAFT") && string.IsNullOrEmpty(message.Address))
                            {
                                message.Type = "draft";
                            }
                        }
                        else if (isVbody)
                        {
                            message.Body += line.Replace("\u2029", "\n");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(message.ContactName))
                {
                    var fileName = Path.GetFileName(filePath);

                    // Use Regex.Match to find the first match in the file name
                    Match match = Regex.Match(fileName, namePatternfromFile);

                    if (match.Success)
                    {
                        // Extract the name from the match
                        string name = match.Groups[1].Value;
                        message.ContactName = name;
                    }
                }
                message.Source = "vmg";
                message.File = filePath;
                messages.Add(message);
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
                Console.WriteLine($"{bsonList.Count} VMG documents inserted successfully");
            }
            return bsonList.Count;
        }

        private static string GetMatchValue(string input, Regex regex)
        {
            var match = regex.Match(input);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        private static string GetMatchValue(string input, string regex)
        {
            var match = Regex.Match(input, regex);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }
    }
}
