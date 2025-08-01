export interface ConnectedRepo {
  id: number;
  repositoryUrl: string;
  platform: string;
}

export interface StateEntry {
  recordedAt: string;
  state: string;
}

export interface BurnoutStatsInfo {
  developerLogin: string;
  totalCommitCount: number;
  avgCommitsPerWeek: number;
  state: string;
  states: StateEntry[];
}
