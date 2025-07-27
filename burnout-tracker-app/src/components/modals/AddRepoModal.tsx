import { useState } from 'react';
import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, MenuItem, Stack,
  FormControlLabel,
  Switch
} from '@mui/material';

const platforms = [
  { id: 1, name: 'GitHub' },
];

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: {
    repositoryUrl: string;
    accessToken?: string;
    supportedRepositoryId: number;
    branch: string;
  }) => void;
}

export default function AddRepoModal({ open, onClose, onSubmit }: Props) {
  const [repositoryUrl, setRepositoryUrl] = useState('');
  const [accessToken, setAccessToken] = useState('');
  const [platformId, setPlatformId] = useState(1);
  const [isPrivate, setIsPrivate] = useState(false);
  const [branch, setBranch] = useState('main');

  const handleSubmit = () => {
    onSubmit({
      repositoryUrl,
      supportedRepositoryId: platformId,
      accessToken: isPrivate ? accessToken : undefined,
      branch,
    });

    onClose();
    setRepositoryUrl('');
    setAccessToken('');
    setIsPrivate(false);
    setBranch('main');
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

          <FormControlLabel
            control={
              <Switch
                checked={isPrivate}
                onChange={(e) => setIsPrivate(e.target.checked)}
              />
            }
            label="Private Repository?"
          />

          {isPrivate && (
            <TextField
              label="GitHub Token"
              value={accessToken}
              onChange={(e) => setAccessToken(e.target.value)}
              fullWidth
            />
          )}

          <TextField
            label="Branch Name"
            value={branch}
            onChange={(e) => setBranch(e.target.value)}
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
