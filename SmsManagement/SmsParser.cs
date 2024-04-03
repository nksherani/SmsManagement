using MongoDB.Bson;
using MongoDB.Driver;

namespace SmsManagement
{
    public class SmsParser
    {
        public static void ProcessFilesInDirectory(string directoryPath)
        {
            IMongoCollection<BsonDocument> smsCollection = MyMongoUtils.GetCollection("sms");
            IMongoCollection<BsonDocument> folderStatsCollection = MyMongoUtils.GetCollection("folderStats");

            // Process XML files
            string[] xmlFiles = Directory.GetFiles(directoryPath, "*.xml");
            var mongoXmlCount = XmlParse.ParseXml(directoryPath, smsCollection);


            // Process VMG files
            string[] vmgFiles = Directory.GetFiles(directoryPath, "*.vmg");
            var mongoVmgCount = VmgParser.ParseVmgFiles(directoryPath, smsCollection);

            // Process CSV files
            string[] csvFiles = Directory.GetFiles(directoryPath, "*.csv");
            var mongoCsvCount = CsvParser.ParseCsv(directoryPath, smsCollection);

            var bson = new BsonDocument
            {
                new BsonElement("folder", directoryPath),
                new BsonElement("xmlFileCount", xmlFiles.Length),
                new BsonElement("vmgFileCount", vmgFiles.Length),
                new BsonElement("csvFileCount", csvFiles.Length),
                new BsonElement("xmlDocCount", mongoXmlCount),
                new BsonElement("vmgDocCount", mongoVmgCount),
                new BsonElement("csvDocCount", mongoCsvCount)
            };
            folderStatsCollection.InsertOne(bson);
            // Recursively process subdirectories
            string[] subdirectories = Directory.GetDirectories(directoryPath);
            foreach (var subdirectory in subdirectories)
            {
                ProcessFilesInDirectory(subdirectory);
            }
        }
    }
}

