import React from "react";
import { Alert, AlertTitle, Box } from "@mui/material";

interface ErrorMessageProps {
  message: string;
  errors?: string[];
  onClose?: () => void;
}

const ErrorMessage: React.FC<ErrorMessageProps> = ({
  message,
  errors,
  onClose,
}) => {
  return (
    <Box sx={{ my: 2 }}>
      <Alert severity="error" onClose={onClose}>
        <AlertTitle>Error</AlertTitle>
        {message}
        {errors && errors.length > 0 && (
          <ul style={{ margin: "8px 0 0 0", paddingLeft: "20px" }}>
            {errors.map((error, index) => (
              <li key={index}>{error}</li>
            ))}
          </ul>
        )}
      </Alert>
    </Box>
  );
};

export default ErrorMessage;
