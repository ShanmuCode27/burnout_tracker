import {
  Box,
  Button,
  Typography,
  Grid,
  Stack,
  CircularProgress,
} from '@mui/material';
import { useEffect, useState } from 'react';
import AddRepoModal from '../components/modals/AddRepoModal';
import { fetchConnectedRepositories, connectRepository } from '../services/repoService';
import type { ConnectedRepo } from '../types/common'
import RepoCard from '../components/RepoCard';
import { useNavigate } from 'react-router-dom';

export default function Dashboard() {
  const [repos, setRepos] = useState<ConnectedRepo[]>([]);
  const [loading, setLoading] = useState(false);
  const [addModalOpen, setAddModalOpen] = useState(false);
  const navigate = useNavigate();

  const fetchRepos = async () => {
    setLoading(true);
    try {
      const data = await fetchConnectedRepositories();
      setRepos(data);
    } catch (err) {
      console.error('Failed to fetch repos:', err);
    }
    setLoading(false);
  };

  const handleRepoConnect = async (data: {
    repositoryUrl: string;
    accessToken?: string;
    supportedRepositoryId: number;
    branch?: string;
  }) => {
    try {
      await connectRepository(data);
      fetchRepos();
    } catch (err) {
      console.error('Failed to connect repo:', err);
    }
  };

  useEffect(() => {
    fetchRepos();
  }, []);

  return (
    <Box p={3}>
      <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5">Connected Repositories</Typography>
        <Button variant="contained" onClick={() => setAddModalOpen(true)}>
          Connect Repository
        </Button>
      </Stack>

      {loading ? (
        <CircularProgress />
      ) : (
        <Grid container spacing={2}>
          {repos.map((repo) => (
            <Grid size={{ xs: 12, md: 6 }} key={repo.id}>
             <RepoCard repo={repo} onClick={() => navigate(`repos/${repo.id}`)} />
            </Grid>
          ))}
        </Grid>
      )}

      <AddRepoModal
        open={addModalOpen}
        onClose={() => setAddModalOpen(false)}
        onSubmit={handleRepoConnect}
      />
    </Box>
  );
}
