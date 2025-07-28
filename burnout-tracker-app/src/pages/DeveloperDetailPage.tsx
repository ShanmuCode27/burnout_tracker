import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  Paper,
  CircularProgress,
  Divider,
  Chip,
  Grid,
  Button
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

import api from '../services/api';

export default function DeveloperDetailPage() {
  const { repoId, login } = useParams();
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    async function fetchDetails() {
      const res = await api.get(`/analytics/${repoId}/developers/${login}`);
      const data = res.data;
      setData(data);
      setLoading(false);
    }
    fetchDetails();
  }, [repoId, login]);

  if (loading || !data) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box p={3}>
      <Button
        startIcon={<ArrowBackIcon />}
        onClick={() => navigate(-1)}
      >
        Back
      </Button>
      <Typography variant="h4" gutterBottom>
        Developer: {data.developerLogin}
      </Typography>

      <Chip
        label={data.burnoutStatus}
        color={
          data.burnoutStatus === 'Active'
            ? 'success'
            : data.burnoutStatus === 'Warning'
              ? 'warning'
              : 'error'
        }
        sx={{ mb: 2 }}
      />

      <Typography variant='subtitle1' textAlign='end' my='15px'>
        ** This data covers the last 30 days
      </Typography>

      <Grid container spacing={2}>
        <Grid size={{ xs: 12, md: 6 }}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Activity Summary</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography>Weekly Commits: {data.weeklyCommitCount}</Typography>
            <Typography>Total Commits: {data.totalCommitCount}</Typography>
            <Typography>Pull Requests: {data.pullRequestCount}</Typography>
            <Typography>Review Changes: {data.reviewChangesCount}</Typography>
          </Paper>
        </Grid>

        <Grid size={{ xs: 12, md: 6 }}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Burnout Signals</Typography>
            <Divider sx={{ my: 1 }} />
            <Typography>Night Work Count: {data.nightWorkCount}</Typography>
            <Typography>Latest Work Time: {data.latestWorkTimeUtc || 'N/A'}</Typography>
            <Typography>Revert Count: {data.revertCount}</Typography>
            <Typography>Burnout Score: {data.burnoutScore}</Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}