import React from "react";
import { Box, CssBaseline, ThemeProvider, createTheme } from "@mui/material";
import { BrowserRouter } from "react-router-dom";
import Navbar from "./Navbar";
import Footer from "./Footer";

const theme = createTheme({
  palette: {
    primary: {
      main: "#667eea",
    },
    secondary: {
      main: "#764ba2",
    },
    background: {
      default: "#f5f5f5",
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
  },
});

interface LayoutProps {
  children: React.ReactNode;
}

const Layout = ({ children }: LayoutProps) => {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <Box
          sx={{
            display: "flex",
            flexDirection: "column",
            minHeight: "100vh",
          }}
        >
          <Navbar />
          <Box
            component="main"
            sx={{
              flexGrow: 1,
              py: 4,
              px: { xs: 2, md: 4 },
              maxWidth: "1200px",
              width: "100%",
              mx: "auto",
            }}
          >
            {children}
          </Box>
          <Footer />
        </Box>
      </BrowserRouter>
      {/* LoadingOverlay will be here when will be needed*/}
    </ThemeProvider>
  );
};

export default Layout;
