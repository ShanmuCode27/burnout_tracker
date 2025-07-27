import { useEffect, useState } from 'react';
import {
  Card, CardContent, Typography, Table, TableHead, TableBody,
  TableRow, TableCell
} from '@mui/material';

import type { BurnoutStatsInfo } from '../types/common';
import api from '../services/api';

export const BurnoutStats = ({ connectionId }: { connectionId: number}) => {
  const [stats, setStats] = useState<BurnoutStatsInfo[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get(`/api/analytics/${connectionId}`)
      .then(res => setStats(res.data))
      .finally(() => setLoading(false));
  }, [connectionId]);

  if (loading) return <Typography>Loading stats...</Typography>;
  if (stats.length === 0) return <Typography>No data found</Typography>;

  return (
    <Card>
      <CardContent>
        <Typography variant="h6">Developer Burnout States</Typography>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Developer</TableCell>
              <TableCell>Total Commits</TableCell>
              <TableCell>Avg Commits/Week</TableCell>
              <TableCell>Latest State</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {stats.map(dev => (
              <TableRow key={dev.developerLogin}>
                <TableCell>{dev.developerLogin}</TableCell>
                <TableCell>{dev.totalCommitCount}</TableCell>
                <TableCell>{dev.avgCommitsPerWeek}</TableCell>
                {/* <TableCell>{dev.states.at(-1)?.state ?? 'Unknown'}</TableCell> */}
                <TableCell>{dev.state}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  );
};
