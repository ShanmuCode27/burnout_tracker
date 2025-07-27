import api from './api';

export async function fetchConnectedRepositories() {
  const res = await api.get('/repos');
  return res.data;
}

export async function connectRepository(data: {
  repositoryUrl: string;
  accessToken?: string;
  supportedRepositoryId: number;
}) {
  const res = await api.post('/repos/connect', data);
  return res.data;
}

export async function deleteRepository(id: number) {
  await api.delete(`/repos/${id}`);
}
