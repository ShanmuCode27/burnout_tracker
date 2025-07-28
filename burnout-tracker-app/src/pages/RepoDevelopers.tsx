import { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation, Link } from 'react-router-dom';
import {
  Grid,
  Card,
  CardContent,
  Typography,
  Chip,
  CircularProgress,
  Box,
  Button,
  Stack
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts';

import api from '../services/api';

const COLORS: Record<string, string> = {
  Active: '#4caf50',
  Warning: '#ff9800',
  BurnedOut: '#f44336',
};

export default function RepoDevelopersPage() {
  const { repoId } = useParams();
  const navigate = useNavigate();
  const [developers, setDevelopers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const location = useLocation();

  const state = location.state || {};

  useEffect(() => {
    async function fetchData() {
      try {
        const res = await api.get(`/analytics/${repoId}/developers`);
        const data = res.data;
        setDevelopers(data);
      } finally {
        setLoading(false);
      }
    }
    fetchData();
  }, [repoId]);

  const statusCounts = developers.reduce<Record<string, number>>((acc, dev) => {
    acc[dev.burnoutStatus] = (acc[dev.burnoutStatus] || 0) + 1;
    return acc;
  }, {});

  const pieData = Object.entries(statusCounts).map(([status, count]) => ({
    name: status,
    value: count,
  }));

  if (loading) {
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
      <Stack direction="row" justifyContent="space-between">
        <Typography variant="h4" gutterBottom>
          Contributors
        </Typography>
        <Typography variant='subtitle1'>
          ** This data covers the last 30 days
        </Typography>
      </Stack>
      <Button>
        <Link to={state.repositoryUrl} target="_blank">
          {state.repositoryUrl}
        </Link>
      </Button>

      <Box height={300} mb={4}>
        <ResponsiveContainer>
          <PieChart>
            <Pie
              dataKey="value"
              data={pieData}
              cx="50%"
              cy="50%"
              outerRadius={100}
              label={({ name }) => name}
            >
              {pieData.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={COLORS[entry.name] || '#8884d8'} />
              ))}
            </Pie>
            <Tooltip />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </Box>

      <Grid container spacing={2}>
        {developers.map((dev) => (
          <Grid size={{ xs: 12, md: 6, lg: 4 }} key={dev.developerLogin}>
            <Card
              sx={{ cursor: 'pointer' }}
              onClick={() => navigate(`/repos/${repoId}/devs/${dev.developerLogin}`)}
            >
              <CardContent>
                <Typography variant="h6">{dev.developerLogin}</Typography>
                <Chip
                  label={dev.burnoutStatus}
                  color={
                    dev.burnoutStatus === 'Active'
                      ? 'success'
                      : dev.burnoutStatus === 'Warning'
                      ? 'warning'
                      : 'error'
                  }
                  size="small"
                />
                <Typography variant="body2">Commits: {dev.weeklyCommitCount}</Typography>
                <Typography variant="body2">PRs: {dev.pullRequestCount}</Typography>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
