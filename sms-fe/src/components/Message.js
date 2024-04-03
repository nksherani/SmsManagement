import React from "react";
import "./css/Message.css"; // Import CSS file for styling

const Message = (props) => {
  // You can access props directly inside the functional component
  const message = props.message;
  return (
    <div key={message._id} className="message">
      {message.type === "sent" ? (
        <>
          <div className="sent-message">{message.body}</div>
          <div className="message-timestamp">sent {message.date}</div>
        </>
      ) : (
        <>
          <div className="received-message">{message.body}</div>
          <div className="message-timestamp">{message.date}</div>
        </>
      )}
    </div>
  );
};

export default Message;
