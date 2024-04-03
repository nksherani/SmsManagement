import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Contacts from './components/Contacts';
import ChatContainer from './components/ChatContainer';


function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<Contacts/>} />
          <Route path="/messages/:phoneNumber" element={<ChatContainer/>} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
