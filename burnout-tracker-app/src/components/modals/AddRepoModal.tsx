import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, MenuItem, Stack
} from '@mui/material';

const platforms = [
  { id: 1, name: 'GitHub' },
];

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: { repositoryUrl: string; accessToken?: string; supportedRepositoryId: number }) => void;
}

export default function AddRepoModal({ open, onClose, onSubmit }: Props) {
  const [repositoryUrl, setRepositoryUrl] = useState('');
  const [accessToken, setAccessToken] = useState('');
  const [platformId, setPlatformId] = useState(1);

  const handleSubmit = () => {
    onSubmit({
      repositoryUrl,
      accessToken: accessToken || undefined,
      supportedRepositoryId: platformId,
    });
    onClose();
    setRepositoryUrl('');
    setAccessToken('');
  };

  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Connect Repository</DialogTitle>
      <DialogContent>
        <Stack spacing={2} mt={1}>
          <TextField
            select
            label="Platform"
            value={platformId}
            onChange={(e) => setPlatformId(Number(e.target.value))}
            fullWidth
          >
            {platforms.map(p => (
              <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
            ))}
          </TextField>
          <TextField
            label="Repository URL"
            value={repositoryUrl}
            onChange={(e) => setRepositoryUrl(e.target.value)}
            fullWidth
          />
          <TextField
            label="GitHub Token (optional)"
            value={accessToken}
            onChange={(e) => setAccessToken(e.target.value)}
            fullWidth
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button variant="contained" onClick={handleSubmit}>Connect</Button>
      </DialogActions>
    </Dialog>
  );
}
