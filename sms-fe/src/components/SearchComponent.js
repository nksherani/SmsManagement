import React, { useState } from "react";
import SearchResults from "./SearchResults";

import "./css/SearchComponent.css"; // Make sure you have this CSS file

const SearchComponent = (props) => {
  const [expanded, setExpanded] = useState(false);
  const [searchText, setSearchText] = useState("");
  const [searchResults, setSearchResults] = useState([]);
  const { phoneNumber } = props;
  const handleSearchIconClick = () => {
    setExpanded(true);
  };

  const handleInputChange = (e) => {
    const inputText = e.target.value;
    setSearchText(inputText);
    if (inputText === "") {
      setExpanded(false);
    }

    // Mocking search results using setTimeout
    setTimeout(() => {
      handleSearchChange(inputText);
    }, 300); // Adjust the timeout as needed
  };
  const handleSearchChange = async (text) => {
    // Add your logic for handling search term changes here

    const response = await fetch(
      `http://localhost:4000/api/sms?address=${phoneNumber}&body=${text}`
    );
    const data = await response.json();
    // setMessages(data.smsData);
    // setHasMore(data.count > messages.length);
    // setIsLoading(false);
    setSearchResults(data.smsData);
  };

  return (
    <div className={`search-container ${expanded ? "expanded" : ""}`}>
      <div className="search-bar">
        {expanded ? (
          <input
            type="text"
            placeholder="Search..."
            value={searchText}
            onChange={handleInputChange}
          />
        ) : (
          <div className="search-icon" onClick={handleSearchIconClick}>
            🔍
          </div>
        )}
      </div>
      {expanded && searchText !== "" && (
        <SearchResults searchResults={searchResults} />
      )}
    </div>
  );
};

export default SearchComponent;
