import { configureStore } from '@reduxjs/toolkit';
import { graphqlApi } from './api/graphql';
import authReducer from './slices/authSlice';

export const store = configureStore({
  reducer: {
    [graphqlApi.reducerPath]: graphqlApi.reducer,
    auth: authReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [graphqlApi.util.resetApiState.type],
      },
    }).concat(graphqlApi.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch; 