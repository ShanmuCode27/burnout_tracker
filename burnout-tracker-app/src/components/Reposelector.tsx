import { Button, TextField, Stack } from '@mui/material';
import { useState } from 'react';

type Props = {
  onRepoSelected: (owner: string, repo: string, token?: string) => void;
};

export default function RepoSelector({ onRepoSelected }: Props) {
  const [owner, setOwner] = useState('');
  const [repo, setRepo] = useState('');
  const [token, setToken] = useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (owner && repo) {
      onRepoSelected(owner, repo, token);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Stack spacing={2} sx={{ maxWidth: 400, margin: 'auto', paddingTop: 4 }}>
        <TextField label="Repo Owner" value={owner} onChange={(e) => setOwner(e.target.value)} fullWidth />
        <TextField label="Repo Name" value={repo} onChange={(e) => setRepo(e.target.value)} fullWidth />
        <TextField label="GitHub Token (optional)" value={token} onChange={(e) => setToken(e.target.value)} fullWidth />
        <Button variant="contained" type="submit">Load Contributors</Button>
      </Stack>
    </form>
  );
}
