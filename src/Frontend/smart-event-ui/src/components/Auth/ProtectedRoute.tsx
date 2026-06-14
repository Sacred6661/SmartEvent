import React from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import LoadingOverlay from "../Common/LoadingOverlay";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const isAuthenticated = useAuth((state) => state.isAuthenticated);
  const isInitialized = useAuth((state) => state.isInitialized);
  const location = useLocation();

  // show loader during auth check
  if (!isInitialized) {
    return <LoadingOverlay loading={true} />;
  }

  // if not authenticated - redirect to login
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
};
