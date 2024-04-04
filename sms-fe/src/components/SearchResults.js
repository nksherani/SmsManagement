import React from "react";
import { useSelector } from "react-redux";
import ContactHeader from "./ContactHeader";

import "./css/SearchResults.css";
import Messages from "./Messages";

const SearchResults = (props) => {
  const selectedContact = useSelector((state) => state.selectedContact);

  const { searchResults } = props;

  return (
    <>
      <ContactHeader selectedContact={selectedContact} />
      <Messages messages={searchResults} />
    </>
  );
};

export default SearchResults;
