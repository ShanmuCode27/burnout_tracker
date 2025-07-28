import { Button, Card, CardContent, CardHeader, Stack, Typography } from '@mui/material';

import type { ConnectedRepo } from '../types/common';
import { deleteRepository } from '../services/repoService';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import DeleteConfirmationDialog from './modals/DeleteConfirmation';

interface RepoCardProps {
  repo: ConnectedRepo;
  onClick?: () => void;
}

export default function RepoCard({ repo, onClick }: RepoCardProps) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [deleteRepo, setDeleteRepo] = useState<ConnectedRepo | undefined>();
  const navigate = useNavigate();

  const handleDelete = async (id: number) => {
    try {
      await deleteRepository(id);
      navigate('/');
    } catch (err) {
      console.error('Failed to delete repo:', err);
    }
    setDeleteRepo(undefined);
    setDialogOpen(false);
  };


  return (
    <>
      <Card
        variant="outlined"
        onClick={onClick}
        sx={{
          cursor: 'pointer',
          borderRadius: 5,
          boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)',
          transition: 'box-shadow 0.3s ease-in-out',
          '&:hover': {
            boxShadow: '0 6px 12px rgba(0, 0, 0, 0.15)',
          }
        }}>
        <CardHeader
          action={
            <Button
              variant='contained'
              size="small"
              color="error"
              onClick={(e: React.MouseEvent) => {
                e.stopPropagation();
                setDeleteRepo(repo);
                setDialogOpen(true);
              }}
            >
              Delete
            </Button>
          }
          subheader={
            <Typography variant="subtitle2" color="text.secondary">
              {repo.platform}
            </Typography>
          }
        />
        <CardContent>
          <Stack direction="row" alignContent="center">
            <Typography variant='body1'>
              Repo Link
            </Typography>
            <Typography variant="body2" alignSelf="center">
              {" : " + repo.repositoryUrl}
            </Typography>
          </Stack>
        </CardContent>
      </Card>
      <DeleteConfirmationDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onConfirm={() => handleDelete(deleteRepo?.id ?? 0)}
        itemName="this repository"
      />
    </>

  );
}
