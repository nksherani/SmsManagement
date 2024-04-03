import React from "react";
import Message from "./Message";
import "./css/Messages.css";

const Messages = (props) => {
  const { messages, hasMore, forwardedRef } = props;
  return (
    <div className="start-from-bottom">
      {messages.map((message) => (
        <Message message={message} />
      ))}
      {hasMore && <p ref={forwardedRef}></p>}
    </div>
  );
};

export default Messages;
