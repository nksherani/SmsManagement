import React, { useState, useEffect, useRef } from "react";
import { useParams } from "react-router-dom";
import { useSelector } from "react-redux";
import "./css/ChatContainer.css"; // Import CSS file for styling
import SearchComponent from "./SearchComponent";
import Messages from "./Messages";
import ContactHeader from "./ContactHeader";

const ChatContainer = () => {
  const [messages, setMessages] = useState([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const [hasMore, setHasMore] = useState(true);
  const [isLoading, setIsLoading] = useState(false);
  const { phoneNumber } = useParams(); // Get the contact ID from the URL params
  const buttonRef = useRef(null);
  const cleansedNumber = phoneNumber
    .toString()
    .replace("+92", "")
    .replace("+", "");

  useEffect(() => {
    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          console.log("top <p> is visible after scrolling");
          // Add your code to handle the button becoming visible here
          // For example, you can trigger an event or update state
          handleLoadMore();
        }
      });
    });

    if (buttonRef.current) {
      observer.observe(buttonRef.current);
    }

    return () => {
      if (buttonRef.current) {
        observer.unobserve(buttonRef.current);
      }
    };
  }, []);

  const fetchMessages = async () => {
    setIsLoading(true);

    const response = await fetch(
      `http://localhost:4000/api/sms?address=${cleansedNumber}&page=${page}&pageSize=${pageSize}`
    );
    const data = await response.json();
    setMessages((prevMessages) => [...prevMessages, ...data.smsData.reverse()]);
    setHasMore(data.page < data.totalPages);
    setIsLoading(false);
  };

  useEffect(() => {
    fetchMessages();
  }, [page, pageSize]);

  const handleLoadMore = () => {
    if (hasMore && !isLoading) {
      setPage((page) => page + 1);
    }
  };

  const selectedContact = useSelector((state) => state.selectedContact);
  const isSearching = useSelector((state) => state.isSearching);

  return (
    <>
      <SearchComponent phoneNumber={cleansedNumber} />
      {!isSearching && (
        <>
          <ContactHeader selectedContact={selectedContact} />
          <Messages
            messages={messages}
            hasMore={hasMore}
            forwardedRef={buttonRef}
          />
        </>
      )}
    </>
  );
};

export default ChatContainer;
