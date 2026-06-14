import { useState } from "react";
import {
  Box,
  Button,
  TextField,
  Typography,
  Divider,
  InputAdornment,
  IconButton,
  Alert,
  CircularProgress,
} from "@mui/material";
import GoogleIcon from "@mui/icons-material/Google";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import EmailIcon from "@mui/icons-material/Email";
import LockIcon from "@mui/icons-material/Lock";
import { useAuth } from "../../../hooks/useAuth";
import type { LoginRequest } from "../../../types/auth";

export const LoginForm = () => {
  const [formData, setFormData] = useState<LoginRequest>({
    email: "",
    password: "",
  });
  const [showPassword, setShowPassword] = useState(false);

  const login = useAuth((state) => state.login);
  const loginWithGoogle = useAuth((state) => state.loginWithGoogle);
  const error = useAuth((state) => state.error);
  const isLoading = useAuth((state) => state.isLoading);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const success = await login(formData.email, formData.password);

    if (success) {
      // Successful login - redirect to the home page
      // TODO: should be changed to registered user home page maybe
      window.location.href = "/";
    }
    // if error it has beed already in error state
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        width: "100%",
      }}
    >
      <Typography variant="h5" gutterBottom sx={{ fontWeight: 600 }}>
        Sign In
      </Typography>

      {/* Google Login Button */}
      <Button
        fullWidth
        variant="outlined"
        startIcon={<GoogleIcon />}
        onClick={loginWithGoogle}
        sx={{
          mb: 3,
          py: 1.5,
          borderColor: "#e0e0e0",
          color: "#424242",
          "&:hover": {
            borderColor: "#bdbdbd",
            backgroundColor: "#fafafa",
          },
        }}
      >
        Continue with Google
      </Button>

      <Divider sx={{ width: "100%", mb: 3 }}>
        <Typography variant="body2" color="text.secondary">
          or sign in with email
        </Typography>
      </Divider>

      {/* Error Message */}
      {error && (
        <Alert severity="error" sx={{ width: "100%", mb: 2 }}>
          {error}
        </Alert>
      )}

      {/* Login Form */}
      <Box component="form" onSubmit={handleSubmit} sx={{ width: "100%" }}>
        <TextField
          fullWidth
          label="Email"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          margin="normal"
          required
          autoFocus
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <EmailIcon color="action" />
                </InputAdornment>
              ),
            },
          }}
          sx={{ mb: 2 }}
        />

        <TextField
          fullWidth
          label="Password"
          name="password"
          type={showPassword ? "text" : "password"}
          value={formData.password}
          onChange={handleChange}
          margin="normal"
          required
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <LockIcon color="action" />
                </InputAdornment>
              ),
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPassword(!showPassword)}
                    edge="end"
                  >
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              handleSubmit(e);
            }
          }}
          sx={{ mb: 3 }}
        />

        <Button
          type="submit"
          fullWidth
          variant="contained"
          disabled={isLoading}
          sx={{
            py: 1.5,
            backgroundColor: "primary.main",
            "&:hover": {
              backgroundColor: "primary.dark",
            },
            position: "relative",
          }}
        >
          {isLoading ? (
            <>
              <CircularProgress
                size={24}
                sx={{
                  position: "absolute",
                  color: "white",
                }}
              />
              <span style={{ visibility: "hidden" }}>Sign In</span>
            </>
          ) : (
            "Sign In"
          )}
        </Button>
      </Box>

      <Box sx={{ mt: 2, textAlign: "center" }}>
        <Typography variant="body2" color="text.secondary">
          Don't have an account?{" "}
          <Button
            color="primary"
            href="/register"
            sx={{ textTransform: "none", p: 0, minWidth: "auto" }}
          >
            Register here
          </Button>
        </Typography>
      </Box>
    </Box>
  );
};
