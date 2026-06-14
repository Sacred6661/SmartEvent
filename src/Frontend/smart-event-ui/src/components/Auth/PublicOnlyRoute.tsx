import React from "react";
import { Navigate } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import LoadingOverlay from "../Common/LoadingOverlay";

interface PublicOnlyRouteProps {
  children: React.ReactNode;
}

export const PublicOnlyRoute: React.FC<PublicOnlyRouteProps> = ({
  children,
}) => {
  const isAuthenticated = useAuth((state) => state.isAuthenticated);
  const isInitialized = useAuth((state) => state.isInitialized);

  // show loader during auth check
  if (!isInitialized) {
    return <LoadingOverlay loading={true} />;
  }

  // if authenticated - редірект на головну
  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};
