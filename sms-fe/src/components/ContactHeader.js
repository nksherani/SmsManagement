import React from "react";

const ContactHeader = (props) => {
  const { selectedContact } = props;
  return (
    <div className="sticky-header">
      {selectedContact ? (
        <p>
          {selectedContact.contactName}
          <br />
          {selectedContact.number}
        </p>
      ) : (
        <p>Object not yet available</p>
      )}
    </div>
  );
};

export default ContactHeader;
