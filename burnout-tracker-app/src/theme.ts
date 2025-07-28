import { createTheme } from '@mui/material/styles';

const customTheme = createTheme({
  palette: {
    primary: {
      main: '#292929ff',
    },
    secondary: {
      main: '#dc004e',
      light: '#3a89e9ff'
    },
  },
  typography: {
    fontFamily: '"Quicksand", sans-serif',
    h1: {
      fontSize: '2.5rem',
    },
  },
  spacing: 8,
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 8,
        },
      },
    },
  },
});

export default customTheme;