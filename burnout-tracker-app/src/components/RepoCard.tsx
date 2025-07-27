import { Card, CardContent, Typography } from '@mui/material';

interface RepoCardProps {
  repo: {
    repositoryUrl: string;
    platform: string;
  };
}

export default function RepoCard({ repo }: RepoCardProps) {
  return (
    <Card variant="outlined">
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