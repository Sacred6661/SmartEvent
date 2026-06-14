import { useEffect, useRef } from "react";
import { Routes, Route } from "react-router-dom";
import Layout from "./components/Layout/Layout";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import { AuthCallback } from "./features/auth/components/AuthCallback";
import { ProtectedRoute } from "./components/Auth/ProtectedRoute";
import { PublicOnlyRoute } from "./components/Auth/PublicOnlyRoute";
import { useAuth } from "./hooks/useAuth";
import LoadingOverlay from "./components/Common/LoadingOverlay";

function App() {
  const isInitialized = useAuth((state) => state.isInitialized);
  const checkAuth = useAuth((state) => state.checkAuth);
  const initRef = useRef(false);

  useEffect(() => {
    // Preventing double calls in Strict Mode
    if (!initRef.current) {
      initRef.current = true;
      checkAuth();
    }
  }, [checkAuth]);

  if (!isInitialized) {
    return <LoadingOverlay loading={true} />;
  }

  return (
    <Layout>
      <Routes>
        {/* Public routes – available to everyone */}
        <Route path="/" element={<HomePage />} />
        <Route path="/auth/callback" element={<AuthCallback />} />

        {/* Routes for unauthenticated users only */}
        <Route
          path="/login"
          element={
            <PublicOnlyRoute>
              <LoginPage />
            </PublicOnlyRoute>
          }
        />
        <Route
          path="/register"
          element={
            <PublicOnlyRoute>
              <RegisterPage />
            </PublicOnlyRoute>
          }
        />

        {/* Secure routes */}
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <div>Profile Page (coming soon)</div>
            </ProtectedRoute>
          }
        />
      </Routes>
    </Layout>
  );
}

export default App;
