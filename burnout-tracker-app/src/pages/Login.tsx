import { useState } from 'react';
import {
  Container, TextField, Button, Stack, Typography, Alert
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

export default function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleLogin = async () => {
    try {
      const res = await api.post('/auth/login', { username, password });
      const token = res.data.token;

      localStorage.setItem('token', token);
      setError('');
      navigate('/');
    } catch (err) {
      setError('Invalid username or password');
    }
  };

  return (
    <Container sx={{ mt: 6 }}>
      <Typography variant="h5" gutterBottom>Login</Typography>
      <Stack spacing={2} sx={{ maxWidth: 400 }}>
        <TextField
          label="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
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
        <Button variant="text" onClick={() => navigate('/signup')}>
          Don’t have an account? Sign up
        </Button>
      </Stack>
    </Container>
  );
}
