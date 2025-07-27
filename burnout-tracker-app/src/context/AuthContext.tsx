import { createContext, useContext, useEffect, useState, type Dispatch, type ReactNode, type SetStateAction } from 'react';
import { jwtDecode } from 'jwt-decode';

type AuthContextType = {
  username: string | null;
  token: string | null;
  login: (token: string) => void;
  logout: () => void;
  updateAuth: (token: string) => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [username, setUsername] = useState<string | null>(null);

  useEffect(() => {
    storeAuth(token);
  }, [token]);

  const updateAuth = (token: string) => {
    storeAuth(token);
  }

  const storeAuth = (token: string | null) => {
    if (token) {
      localStorage.setItem('token', token);
      const decoded: any = jwtDecode(token);
      console.log("username token ", decoded)
      setUsername(decoded?.username || null);
      login(token);
    } else {
      localStorage.removeItem('token');
      setUsername(null);
    }
  }

  const login = (newToken: string) => {
    setToken(newToken);
  };

  const logout = () => {
    setToken(null);
  };

  return (
    <AuthContext.Provider value={{ username, token, login, logout, updateAuth }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
};
