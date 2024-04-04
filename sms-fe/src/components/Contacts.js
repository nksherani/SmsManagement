import React, { useState, useEffect } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";

import "./css/Contacts.css"; // Import CSS file for styling

const Contacts = () => {
  const [contacts, setContacts] = useState([]);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    fetchContacts();
  }, []);

  const fetchContacts = async () => {
    try {
      const response = await fetch("http://localhost:4000/api/unique-contacts"); // Replace with your API endpoint
      const data = await response.json();
      setContacts(data.uniqueContacts);
    } catch (error) {
      console.error("Error fetching contacts:", error);
    }
  };
  const handleClick = (contact) => {
    dispatch({ type: "SELECT_CONTACT", payload: contact });
    // Handle click to open messages for the selected contact
    console.log("Open messages for:", contact);
    navigate(`/messages/${contact.number}`);
  };

  return (
    <div className="contacts-container">
      <h1>Contact List</h1>
      <ul className="contacts-list">
        {contacts.map((contact, idx) => (
          <li
            key={idx}
            className="contact-item"
            onClick={() => handleClick(contact)}
          >
            <div className="contact-avatar"></div>
            <div className="contact-info">
              <p className="contact-name">{contact.contactName}</p>
              <p className="contact-date">
                {new Date(contact.recentDate).toLocaleDateString()}
              </p>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Contacts;
