const initialState = { selectedContact: null, isSearching: false };

const myReducer = (state = initialState, action) => {
  switch (action.type) {
    case "SELECT_CONTACT":
      return { ...state, selectedContact: action.payload };
    case "IS_SEARCHING":
      return { ...state, isSearching: action.payload };
    default:
      return state;
  }
};
export default myReducer;
