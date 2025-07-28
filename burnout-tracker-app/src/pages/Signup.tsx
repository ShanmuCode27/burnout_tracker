import { useState } from 'react';
import { Button, Container, TextField, Typography, Stack } from '@mui/material';
import api from '../services/api';
import { useNavigate } from 'react-router-dom';

export default function Signup() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const handleSignup = async () => {
    try {
      await api.post('/auth/register', { username, password });
      alert('User registered successfully!');
      navigate('/login');
    } catch (err) {
      alert('Signup failed');
      console.error(err);
    }
  };

  return (
    <Container sx={{ mt: 14, display: 'flex', justifyContent: 'center', alignContent: 'center' }}>
      <Stack spacing={2} sx={{ maxWidth: 400 }}>
        <Typography variant="h5" gutterBottom>Sign Up</Typography>
        <TextField label="Username" value={username} onChange={(e) => setUsername(e.target.value)} />
        <TextField label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
        <Button variant="contained" onClick={handleSignup}>Sign Up</Button>
      </Stack>
    </Container>
  );
}
