import { Box, Container, Typography, Link, Grid } from "@mui/material";
import EventIcon from "@mui/icons-material/Event";

const Footer = () => {
  return (
    <Box
      component="footer"
      sx={{
        py: 3,
        px: 2,
        mt: "auto",
        backgroundColor: (theme) =>
          theme.palette.mode === "light"
            ? theme.palette.grey[200]
            : theme.palette.grey[800],
      }}
    >
      <Container maxWidth="lg">
        <Grid container spacing={4}>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Box sx={{ display: "flex", alignItems: "center", mb: 1 }}>
              <EventIcon sx={{ mr: 1 }} />
              <Typography variant="h6">SmartEvent</Typography>
            </Box>
            <Typography variant="body2" color="text.secondary">
              Your platform for amazing events
            </Typography>
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Typography variant="h6" gutterBottom>
              Quick Links
            </Typography>
            <Link
              href="/events"
              color="inherit"
              underline="hover"
              sx={{ display: "block" }}
            >
              Browse Events
            </Link>
            <Link
              href="/about"
              color="inherit"
              underline="hover"
              sx={{ display: "block" }}
            >
              About Us
            </Link>
            <Link
              href="/contact"
              color="inherit"
              underline="hover"
              sx={{ display: "block" }}
            >
              Contact
            </Link>
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Typography variant="h6" gutterBottom>
              Contact
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Email: info@smartevent.com
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Phone: (555) 123-4567
            </Typography>
          </Grid>
        </Grid>
        <Typography
          variant="body2"
          color="text.secondary"
          align="center"
          sx={{ mt: 3 }}
        >
          © {new Date().getFullYear()} SmartEvent. All rights reserved.
        </Typography>
      </Container>
    </Box>
  );
};

export default Footer;
