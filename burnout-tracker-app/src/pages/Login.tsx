import { useState } from 'react';
import {
  Container, TextField, Button, Stack, Typography, Alert,
  useTheme
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';

export default function Login() {
  const [username, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { updateAuth } = useAuth();
  const navigate = useNavigate();
  const theme = useTheme();

  const handleLogin = async () => {
    try {
      const res = await api.post('/auth/login', { username, password });
      const token = res.data.token;

      localStorage.setItem('token', token);
      updateAuth(token);
      setError('');
      navigate('/');
    } catch (err) {
      setError('Invalid username or password');
    }
  };

  return (
    <Container sx={{ mt: 14, display: 'flex', justifyContent: 'center', alignContent: 'center' }}>
      <Stack spacing={2} sx={{ maxWidth: 400 }}>
        <Typography variant="h5" gutterBottom>Login</Typography>
        <TextField
          label="Username"
          value={username}
          onChange={(e) => setUserName(e.target.value)}
        />
        <TextField
          label="Password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        {error && <Alert severity="error">{error}</Alert>}
        <Button variant="contained" onClick={handleLogin}>
          Login
        </Button>
        <Button variant="text" onClick={() => navigate('/signup')} sx={{ color: theme.palette.secondary.light }}>
          Don’t have an account? Sign up
        </Button>
      </Stack>
    </Container>
  );
}
