import React from "react";
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  Box,
  IconButton,
  Avatar,
  Menu,
  MenuItem,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import EventIcon from "@mui/icons-material/Event";
import { useAuth } from "../../hooks/useAuth";

const Navbar = () => {
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

  // use Zustand selectors
  const user = useAuth((state) => state.user);
  const isAuthenticated = useAuth((state) => state.isAuthenticated);
  const isLoading = useAuth((state) => state.isLoading);
  const logout = useAuth((state) => state.logout);

  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  return (
    <AppBar
      position="sticky"
      sx={{
        background: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
        boxShadow: "0 3px 5px 2px rgba(102, 126, 234, .3)",
      }}
    >
      <Toolbar>
        <EventIcon sx={{ mr: 2 }} />
        <Typography
          variant="h6"
          component="div"
          sx={{ flexGrow: 1, cursor: "pointer" }}
          onClick={() => navigate("/")}
        >
          SmartEvent
        </Typography>

        {!isLoading &&
          (isAuthenticated ? (
            <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
              <Button color="inherit" onClick={() => navigate("/events")}>
                Events
              </Button>
              <Button color="inherit" onClick={() => navigate("/my-tickets")}>
                My Tickets
              </Button>
              <IconButton onClick={handleMenu}>
                <Avatar
                  sx={{ width: 32, height: 32, bgcolor: "secondary.main" }}
                >
                  {user?.firstName?.[0]?.toUpperCase() || "U"}
                </Avatar>
              </IconButton>
              <Menu
                anchorEl={anchorEl}
                open={Boolean(anchorEl)}
                onClose={handleClose}
                anchorOrigin={{ vertical: "bottom", horizontal: "right" }}
                transformOrigin={{ vertical: "top", horizontal: "right" }}
              >
                <MenuItem
                  onClick={() => {
                    handleClose();
                    navigate("/profile");
                  }}
                >
                  Profile
                </MenuItem>
                <MenuItem
                  onClick={() => {
                    handleClose();
                    logout();
                  }}
                >
                  Logout
                </MenuItem>
              </Menu>
            </Box>
          ) : (
            <Box sx={{ display: "flex", gap: 1 }}>
              <Button color="inherit" onClick={() => navigate("/login")}>
                Login
              </Button>
              <Button
                variant="outlined"
                color="inherit"
                onClick={() => navigate("/register")}
                sx={{
                  borderColor: "white",
                  "&:hover": {
                    borderColor: "white",
                    bgcolor: "rgba(255,255,255,0.1)",
                  },
                }}
              >
                Register
              </Button>
            </Box>
          ))}
      </Toolbar>
    </AppBar>
  );
};

export default Navbar;
