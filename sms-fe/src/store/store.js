import { configureStore } from '@reduxjs/toolkit';
import counterReducer from './reducers/counterReducer'; // Replace with your actual reducer
import myReducer from './reducers/myReducer';

const store = configureStore({
  reducer: myReducer,
});

export default store;
