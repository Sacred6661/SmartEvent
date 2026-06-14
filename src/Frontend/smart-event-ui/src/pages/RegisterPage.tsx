import { Container, Typography, Paper, Box } from "@mui/material";
import { RegisterForm } from "../features/auth/components/RegisterForm";

const RegisterPage = () => {
  return (
    <Container maxWidth="sm">
      <Box sx={{ mt: 4, mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Create Account
        </Typography>
        <Typography
          variant="body1"
          color="text.secondary"
          align="center"
          sx={{ mb: 4 }}
        >
          Join SmartEvent and never miss an event
        </Typography>
        <Paper elevation={3} sx={{ p: 4 }}>
          <RegisterForm />
        </Paper>
      </Box>
    </Container>
  );
};

export default RegisterPage;
