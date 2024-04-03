// app.js

const express = require("express");
const mongoose = require("mongoose");
const cors = require("cors");

const app = express();
app.use(cors());

const PORT = process.env.PORT || 4000;

// Connect to MongoDB
mongoose
  .connect("mongodb://localhost:27017/SMSDb", {
    useNewUrlParser: true,
    useUnifiedTopology: true,
  })
  .then(() => {
    console.log("Connected to MongoDB");
  })
  .catch((err) => {
    console.error("Error connecting to MongoDB:", err);
  });

// Define a schema for your SMS collection
const smsSchema = new mongoose.Schema({
  protocol: String,
  address: String,
  date: Date,
  type: String,
  subject: String,
  body: String,
  toa: String,
  sc_toa: String,
  service_center: String,
  read: String,
  status: String,
  locked: String,
  readable_date: String,
  contact_name: String,
});

// Create a model based on the schema
const SMS = mongoose.model("SMS", smsSchema);

// Define a route to get filtered and paginated SMS data
app.get("/api/sms", async (req, res) => {
  try {
    // Pagination parameters
    const page = parseInt(req.query.page) || 1; // Default to page 1 if not provided
    const limit = parseInt(req.query.pageSize) || 10; // Default limit to 10 items per page

    // Build the filter object based on request parameters
    const filter = {};

    if (req.query.contact_name) {
      filter.contact_name = { $regex: req.query.contact_name, $options: "i" };
    }

    if (req.query.address) {
      var address = req.query.address;
      if (address.startsWith("92")) {
        address = address.substring(2);
      }
      filter.address = { $regex: address, $options: "i" };
    }

    if (req.query.startdate && req.query.enddate) {
      filter.date = {
        $gte: new Date(req.query.startdate),
        $lte: new Date(req.query.enddate),
      };
    } else if (req.query.startdate) {
      filter.date = { $gte: new Date(req.query.startdate) };
    } else if (req.query.enddate) {
      filter.date = { $lte: new Date(req.query.enddate) };
    }

    if (req.query.body) {
      filter.body = { $regex: req.query.body, $options: "i" };
    }

    // Calculate skip value for pagination
    const skip = (page - 1) * limit;

    // Fetch SMS data based on the filter and pagination parameters
    const smsData = await SMS.find(filter)
      .sort({ date: 1 })
      .skip(skip)
      .limit(limit);

    const totalCount = await SMS.countDocuments(filter);

    res.json({
      count: smsData.length,
      totalCount,
      page,
      totalPages: Math.ceil(totalCount / limit),
      smsData,
    });
  } catch (err) {
    console.error("Error fetching filtered SMS data:", err);
    res.status(500).send("Internal Server Error");
  }
});

app.get("/api/unique-contacts", async (req, res) => {
  try {
    const uniqueContacts = await SMS.aggregate([
      {
        $group: {
          _id: {
            contactName: "$contact_name",
            number: "$address",
          },
          recentDate: { $max: "$date" },
        },
      },
      { $sort: { recentDate: -1 } },
      {
        $project: {
          _id: 0,
          contactName: "$_id.contactName",
          number: "$_id.number",
          recentDate: 1,
        },
      },
    ]);

    const filteredContacts = uniqueContacts.filter(
      (contact) => !contact.number.includes("?") && contact.contactName !== ""
    );
    res.json({
      count: filteredContacts.length,
      uniqueContacts: filteredContacts,
    });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: "Internal Server Error" });
  }
});

app.get("/api/message-by-id", async (req, res) => {
  try {
    const mongoId = req.query.id;
    const filter = {};

    if (req.query.address) {
      var address = req.query.address;
      if (address.startsWith("92")) {
        address = address.substring(2);
      }
      filter.address = { $regex: address, $options: "i" };
    }

    // Search for the selected message
    const selectedMessage = await SMS.findById(mongoId);

    if (!selectedMessage) {
      return res.status(404).json({ message: "Message not found" });
    }
    let t1 = new Date(selectedMessage.date.getTime() - 60 * 60000);
    let t2 = new Date(selectedMessage.date.getTime() + 60 * 60000);
    console.log(t1, t2);
    filter.date = {
      $gte: t1,
      $lte: t2,
    };
    // Get 10 messages before and after the selected message
    const messagesAroundSelected = await SMS.find(filter)
      .sort({ date: 1 })
      .limit(40);

    const totalCount = await SMS.countDocuments(filter);

    res.json({
      count: messagesAroundSelected.length,
      totalCount,
      page,
      totalPages: Math.ceil(totalCount / limit),
      smsData: messagesAroundSelected,
    });
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: "Server error" });
  }
});

// Start the server
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
