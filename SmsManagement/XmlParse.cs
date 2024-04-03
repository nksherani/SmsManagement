using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmsManagement
{
    public class XmlParse
    {
        static bool ContainsSmsesElement(string xmlFilePath)
        {
            try
            {
                XDocument xmlDocument = XDocument.Load(xmlFilePath);
                return xmlDocument.Root?.Name == "smses";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking XML file: {ex.Message}");
                return false;
            }
        }
        static int InsertXmlDataIntoMongoDB(string xmlFilePath, IMongoCollection<BsonDocument> collection)
        {
            // Load XML file into XDocument
            XDocument xmlDocument = XDocument.Load(xmlFilePath);

            // Extract SMS elements from XML
            var smsElements = xmlDocument.Descendants("sms");

            List<BsonDocument> smsDocuments = new List<BsonDocument>();

            // Iterate through SMS elements and insert into MongoDB
            foreach (var smsElement in smsElements)
            {
                BsonDocument bsonDocument = new BsonDocument();
                bsonDocument.Add("source", "xml");
                bsonDocument.Add("file", xmlFilePath);
                foreach (var attribute in smsElement.Attributes())
                {
                    if (attribute.Name.LocalName == "date")
                    {
                        // Convert date attribute to DateTime format
                        long unixTimestamp = long.Parse(attribute.Value);
                        DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime;
                        bsonDocument.Add("date", dateTime);
                    }
                    else if (attribute.Name.LocalName == "type")
                    {
                        // Convert type attribute to "received" or "sent"
                        string messageType = attribute.Value == "1" ? "received" : "sent";
                        bsonDocument.Add("type", messageType);
                    }
                    else
                    {
                        bsonDocument.Add(attribute.Name.LocalName, BsonValue.Create(attribute.Value));
                    }
                }
                smsDocuments.Add(bsonDocument);
            }
            if (smsDocuments.Any())
            {
                collection.InsertMany(smsDocuments);
                Console.WriteLine($"{smsDocuments.Count} XML Documents inserted successfully.");
            }
            return smsDocuments.Count;
        }


        public static int ParseXml(string currentDirectory, IMongoCollection<BsonDocument> collection)
        {
            string[] xmlFiles = Directory.GetFiles(currentDirectory, "*.xml");

            int count = 0;
            int mongoDocumentCount = 0;
            // Iterate through each XML file and insert data into MongoDB
            foreach (string xmlFile in xmlFiles)
            {
                if (ContainsSmsesElement(xmlFile))
                {
                    mongoDocumentCount += InsertXmlDataIntoMongoDB(xmlFile, collection);
                    count++;
                }
                else
                {
                    Console.WriteLine($"Skipping {xmlFile} as it does not contain <smses> element.");
                }
            }
            return mongoDocumentCount;
        }

    }
}
