import { Container, Typography, Paper, Box } from "@mui/material";
import { LoginForm } from "../features/auth/components/LoginForm";

const LoginPage = () => {
  return (
    <Container maxWidth="sm">
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Welcome Back
        </Typography>
        <Typography
          variant="body1"
          color="text.secondary"
          align="center"
          sx={{ mb: 4 }}
        >
          Sign in to access your events and tickets
        </Typography>
        <Paper elevation={3} sx={{ p: 4 }}>
          <LoginForm />
        </Paper>
      </Box>
    </Container>
  );
};

export default LoginPage;
