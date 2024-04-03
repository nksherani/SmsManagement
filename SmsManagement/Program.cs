using System.Text.Json.Nodes;
using System.Text.Json;
using System.Xml.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using SmsManagement;


//MyMongoUtils.CreateUniquenessIndex(collection);

// Get all XML files from the current directory
string currentDirectory = Directory.GetCurrentDirectory();
currentDirectory = @"G:\My Drive\Backup\old data\old data\1.nokia backups old";

SmsParser.ProcessFilesInDirectory(currentDirectory);



