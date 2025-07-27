import { useEffect, useState } from 'react';
import { Container, Typography, Button, Grid } from '@mui/material';
import AddRepoModal from '../components/modals/AddRepoModal';
import RepoCard from '../components/RepoCard';
import { connectRepository, fetchConnectedRepositories } from '../services/repoService';

export default function Dashboard() {
  const [repos, setRepos] = useState<any[]>([]);
  const [modalOpen, setModalOpen] = useState(false);

  const loadRepos = async () => {
    const data = await fetchConnectedRepositories();
    setRepos(data);
  };

  const handleConnect = async (data: any) => {
    await connectRepository(data);
    await loadRepos();
  };

  useEffect(() => {
    loadRepos();
  }, []);

  return (
    <Container sx={{ mt: 4 }}>
      <Typography variant="h5" gutterBottom>Connected Repositories</Typography>
      <Grid container spacing={2} mb={2}>
        {repos.map(repo => (
          <Grid size={{ xs: 26 , md: 6}} key={repo.id}>
            <RepoCard repo={repo} />
          </Grid>
        ))}
      </Grid>
      <Button variant="contained" onClick={() => setModalOpen(true)}>
        Add Repository
      </Button>
      <AddRepoModal
        open={modalOpen}
        onClose={() => {
          setModalOpen(false);
          loadRepos();
        }}
        onSubmit={handleConnect}
      />
    </Container>
  );
}
