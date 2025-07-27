import { useEffect, useState } from 'react';
import { Card, CardContent, Typography, Chip, Grid } from '@mui/material';
import api from '../services/api';

export default function DeveloperList({ repoId }: { repoId: number }) {
  const [developers, setDevelopers] = useState<any[]>([]);

  useEffect(() => {
    async function fetchData() {
      const res = await api.get(`/repos/${repoId}/developers`);
      const devs = res.data;
      setDevelopers(devs);
    }
    fetchData();
  }, [repoId]);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Active': return 'success';
      case 'Warning': return 'warning';
      case 'BurnedOut': return 'error';
      default: return 'default';
    }
  };

  return (
    <Grid container spacing={2}>
      {developers.map((dev) => (
        <Grid size={{ xs: 12, md: 6, lg: 4 }} key={dev.developerLogin}>
          <Card>
            <CardContent>
              <Typography variant="h6">{dev.developerLogin}</Typography>
              <Chip
                label={dev.burnoutStatus}
                color={getStatusColor(dev.burnoutStatus)}
                size="small"
              />
              <Typography variant="body2">Commits: {dev.weeklyCommitCount}</Typography>
              <Typography variant="body2">PRs: {dev.pullRequestCount}</Typography>
            </CardContent>
          </Card>
        </Grid>
      ))}
    </Grid>
  );
}
