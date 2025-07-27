import { Card, CardContent, Avatar, Typography } from '@mui/material';

type Props = {
  login: string;
  avatarUrl: string;
  contributions: number;
};

export default function DeveloperCard({ login, avatarUrl, contributions }: Props) {
  return (
    <Card sx={{ textAlign: 'center', padding: 2 }}>
      <Avatar src={avatarUrl} alt={login} sx={{ width: 64, height: 64, margin: 'auto' }} />
      <CardContent>
        <Typography variant="h6">{login}</Typography>
        <Typography variant="body2" color="text.secondary">
          {contributions} contributions
        </Typography>
      </CardContent>
    </Card>
  );
}
