import React from "react";
import "./css/SearchResults.css";
import Messages from "./Messages";

const SearchResults = (props) => {
  const { searchResults } = props;
  return <Messages messages={searchResults} />;
};

export default SearchResults;
