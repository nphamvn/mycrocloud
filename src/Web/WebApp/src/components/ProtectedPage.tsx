import { useAuth0 } from "@auth0/auth0-react";
import { useEffect } from "react";

export default function ProtectedPage({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isAuthenticated, isLoading, loginWithRedirect } = useAuth0();
  useEffect(() => {
    if (!isLoading) {
      if (!isAuthenticated) {
        loginWithRedirect();
      }
    }
  }, [isLoading]);

  if (isLoading) {
    return null;
  }
  return <>{children}</>;
}
