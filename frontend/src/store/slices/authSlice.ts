import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { UserDto, DevAuthData } from '../../types/api';

interface AuthState {
  isAuthenticated: boolean;
  user: UserDto | null;
  token: string | null;
  devAuth: DevAuthData | null;
  loading: boolean;
  error: string | null;
}

const initialState: AuthState = {
  isAuthenticated: false,
  user: null,
  token: localStorage.getItem('lauf_token'),
  devAuth: JSON.parse(localStorage.getItem('lauf_dev_auth') || 'null'),
  loading: false,
  error: null,
};

// Check if we have dev auth data on startup
if (initialState.devAuth) {
  initialState.isAuthenticated = true;
}

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },

    loginSuccess: (state, action: PayloadAction<{ user: UserDto; token: string }>) => {
      state.isAuthenticated = true;
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.loading = false;
      state.error = null;
      localStorage.setItem('lauf_token', action.payload.token);
    },

    loginError: (state, action: PayloadAction<string>) => {
      state.loading = false;
      state.error = action.payload;
      state.isAuthenticated = false;
      state.user = null;
      state.token = null;
      state.devAuth = null;
      localStorage.removeItem('lauf_token');
      localStorage.removeItem('lauf_dev_auth');
    },

    logout: (state) => {
      state.isAuthenticated = false;
      state.user = null;
      state.token = null;
      state.devAuth = null;
      state.loading = false;
      state.error = null;
      localStorage.removeItem('lauf_token');
      localStorage.removeItem('lauf_dev_auth');
    },

    // Dev Authentication
    setDevAuth: (state, action: PayloadAction<DevAuthData>) => {
      state.devAuth = action.payload;
      state.isAuthenticated = true;
      state.loading = false;
      state.error = null;
      localStorage.setItem('lauf_dev_auth', JSON.stringify(action.payload));
    },

    clearDevAuth: (state) => {
      state.devAuth = null;
      state.isAuthenticated = false;
      state.user = null;
      localStorage.removeItem('lauf_dev_auth');
    },

    updateUser: (state, action: PayloadAction<UserDto>) => {
      state.user = action.payload;
    },

    clearError: (state) => {
      state.error = null;
    },
  },
});

export const {
  loginStart,
  loginSuccess,
  loginError,
  logout,
  setDevAuth,
  clearDevAuth,
  updateUser,
  clearError,
} = authSlice.actions;

export default authSlice.reducer;

// Selectors
export const selectAuth = (state: { auth: AuthState }) => state.auth;
export const selectIsAuthenticated = (state: { auth: AuthState }) => state.auth.isAuthenticated;
export const selectCurrentUser = (state: { auth: AuthState }) => state.auth.user || state.auth.devAuth;
export const selectAuthLoading = (state: { auth: AuthState }) => state.auth.loading;
export const selectAuthError = (state: { auth: AuthState }) => state.auth.error;
export const selectDevAuth = (state: { auth: AuthState }) => state.auth.devAuth; 