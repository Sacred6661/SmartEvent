import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
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
import PersonIcon from "@mui/icons-material/Person";
import EmailIcon from "@mui/icons-material/Email";
import LockIcon from "@mui/icons-material/Lock";
import { authService } from "../../../services/authService";
import type {
  RegisterRequest,
  ApiResponse,
  UserInfo,
} from "../../../types/auth";
import { GOOGLE_LOGIN_URL } from "../../../constants/api";

export const RegisterForm = () => {
  const [formData, setFormData] = useState<RegisterRequest>({
    email: "",
    password: "",
    firstName: "",
    lastName: "",
  });
  const [showPassword, setShowPassword] = useState(false);
  const [confirmPassword, setConfirmPassword] = useState("");
  const [passwordError, setPasswordError] = useState("");

  const registerMutation = useMutation({
    mutationFn: (data: RegisterRequest) => authService.register(data),
    onSuccess: (data: ApiResponse<UserInfo>) => {
      if (data.success) {
        console.log("Registered successfully:", data.data);
        // Перенаправляємо на логін або одразу логінимо
        window.location.href = "/";
      }
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // additional password validation on frontend
    if (formData.password !== confirmPassword) {
      setPasswordError("Passwords do not match");
      return;
    }

    setPasswordError("");
    registerMutation.mutate(formData);
  };

  const handleGoogleLogin = () => {
    window.location.href = GOOGLE_LOGIN_URL;
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
        Create Account
      </Typography>

      {/* Google Registration Button */}
      <Button
        fullWidth
        variant="outlined"
        startIcon={<GoogleIcon />}
        onClick={handleGoogleLogin}
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
        Sign up with Google
      </Button>

      <Divider sx={{ width: "100%", mb: 3 }}>
        <Typography variant="body2" color="text.secondary">
          or register with email
        </Typography>
      </Divider>

      {/* Error Messages */}
      {registerMutation.isError && (
        <Alert severity="error" sx={{ width: "100%", mb: 2 }}>
          {registerMutation.error instanceof Error
            ? registerMutation.error.message
            : "Registration failed. Please try again."}
        </Alert>
      )}

      {passwordError && (
        <Alert severity="error" sx={{ width: "100%", mb: 2 }}>
          {passwordError}
        </Alert>
      )}

      {registerMutation.isSuccess && (
        <Alert severity="success" sx={{ width: "100%", mb: 2 }}>
          Registration successful! Redirecting to login...
        </Alert>
      )}

      {/* Registration Form */}
      <Box component="form" onSubmit={handleSubmit} sx={{ width: "100%" }}>
        <Box sx={{ display: "flex", gap: 2 }}>
          <TextField
            fullWidth
            label="First Name"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            margin="normal"
            required
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <PersonIcon color="action" />
                  </InputAdornment>
                ),
              },
            }}
          />
          <TextField
            fullWidth
            label="Last Name"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            margin="normal"
            required
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <PersonIcon color="action" />
                  </InputAdornment>
                ),
              },
            }}
          />
        </Box>

        <TextField
          fullWidth
          label="Email"
          name="email"
          type="email"
          value={formData.email}
          onChange={handleChange}
          margin="normal"
          required
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
          sx={{ mb: 2 }}
        />

        <TextField
          fullWidth
          label="Confirm Password"
          type={showPassword ? "text" : "password"}
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.target.value)}
          margin="normal"
          required
          error={!!passwordError}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <LockIcon color="action" />
                </InputAdornment>
              ),
            },
          }}
          sx={{ mb: 3 }}
        />

        <Button
          type="submit"
          fullWidth
          variant="contained"
          disabled={registerMutation.isPending}
          sx={{
            py: 1.5,
            backgroundColor: "primary.main",
            "&:hover": {
              backgroundColor: "primary.dark",
            },
            position: "relative",
          }}
        >
          {registerMutation.isPending ? (
            <>
              <CircularProgress
                size={24}
                sx={{
                  position: "absolute",
                  color: "white",
                }}
              />
              <span style={{ visibility: "hidden" }}>Create Account</span>
            </>
          ) : (
            "Create Account"
          )}
        </Button>
      </Box>

      <Box sx={{ mt: 2, textAlign: "center" }}>
        <Typography variant="body2" color="text.secondary">
          Already have an account?{" "}
          <Button
            color="primary"
            onClick={() => (window.location.href = "/login")}
            sx={{ textTransform: "none", p: 0, minWidth: "auto" }}
          >
            Sign in
          </Button>
        </Typography>
      </Box>
    </Box>
  );
};
