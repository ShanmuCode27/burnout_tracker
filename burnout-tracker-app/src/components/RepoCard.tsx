import { Button, Card, CardContent, Typography } from '@mui/material';

import type { ConnectedRepo } from '../types/common';
import { deleteRepository } from '../services/repoService';
import { useNavigate } from 'react-router-dom';

interface RepoCardProps {
  repo: ConnectedRepo;
  onClick?: () => void;
}

export default function RepoCard({ repo, onClick }: RepoCardProps) {
  const navigate = useNavigate();

  const handleDeleteRepo = async (id: number) => {
  if (!window.confirm('Are you sure you want to delete this repository?')) return;
      try {
        await deleteRepository(id);
        navigate('/');
      } catch (err) {
        console.error('Failed to delete repo:', err);
      }
    };


  return (
    <Card variant="outlined" onClick={onClick} sx={{ cursor: 'pointer' }}>
        <Button
          variant='outlined'
          size="small"
          color="error"
          onClick={() => handleDeleteRepo(repo.id)}
        >
          Delete
        </Button>
      <CardContent>
        <Typography variant="subtitle2" color="text.secondary">
          {repo.platform}
        </Typography>
        <Typography variant="body1">
          {repo.repositoryUrl}
        </Typography>
      </CardContent>
    </Card>
  );
}
