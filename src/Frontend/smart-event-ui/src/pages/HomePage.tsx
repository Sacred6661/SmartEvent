import {
  Typography,
  Box,
  Button,
  Grid,
  Card,
  CardContent,
} from "@mui/material";
import { useNavigate } from "react-router-dom";

const HomePage = () => {
  const navigate = useNavigate();

  return (
    <Box>
      <Box sx={{ textAlign: "center", mb: 6 }}>
        <Typography variant="h2" component="h1" gutterBottom>
          Welcome to SmartEvent
        </Typography>
        <Typography
          variant="h5"
          color="text.secondary"
          sx={{
            mb: 2,
            whiteSpace: "pre-line",
          }}
        >
          Discover and organize amazing events
        </Typography>
        <Button
          variant="contained"
          size="large"
          onClick={() => navigate("/register")}
          sx={{ mt: 2 }}
        >
          Get Started
        </Button>
      </Box>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography variant="h5" gutterBottom>
                Find Events
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Browse through hundreds of events happening near you
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography variant="h5" gutterBottom>
                Create Events
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Organize and manage your own events with ease
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography variant="h5" gutterBottom>
                AI Recommendations
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Get personalized event recommendations powered by AI
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default HomePage;
