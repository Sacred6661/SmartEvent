import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Box, CircularProgress, Typography, Alert } from "@mui/material";
import { useAuth } from "../../../hooks/useAuth";

export const AuthCallback = () => {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [error, setError] = useState<string | null>(null);
  const checkAuth = useAuth((state) => state.checkAuth);

  useEffect(() => {
    const handleCallback = async () => {
      try {
        // Check if there is error in the URL
        const errorParam = searchParams.get("error");
        if (errorParam) {
          setError("Authentication failed. Please try again.");
          setTimeout(() => navigate("/login"), 3000);
          return;
        }

        // Give time for cookies to be set
        await new Promise((resolve) => setTimeout(resolve, 500));

        // check auth
        await checkAuth();

        // If authenticated, redirect to the home page
        if (useAuth.getState().isAuthenticated) {
          navigate("/", { replace: true });
        } else {
          // If not logged in try again in a second
          await new Promise((resolve) => setTimeout(resolve, 1000));
          await checkAuth();

          if (useAuth.getState().isAuthenticated) {
            navigate("/", { replace: true });
          } else {
            setError("Failed to complete authentication. Please try again.");
            setTimeout(() => navigate("/login"), 3000);
          }
        }
      } catch (err) {
        console.error("Auth callback error:", err);
        setError("An error occurred during authentication.");
        setTimeout(() => navigate("/login"), 3000);
      }
    };

    handleCallback();
  }, [navigate, searchParams, checkAuth]);

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "60vh",
        gap: 3,
      }}
    >
      {error ? (
        <>
          <Alert severity="error" sx={{ maxWidth: 400, width: "100%" }}>
            {error}
          </Alert>
          <Typography variant="body1" color="text.secondary">
            Redirecting to login page...
          </Typography>
        </>
      ) : (
        <>
          <CircularProgress size={48} />
          <Typography variant="h6" color="text.secondary">
            Completing authentication...
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Please wait while we verify your credentials
          </Typography>
        </>
      )}
    </Box>
  );
};
