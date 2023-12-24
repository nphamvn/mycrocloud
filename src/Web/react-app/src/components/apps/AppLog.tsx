import { useContext, useEffect, useState } from "react";
import { AppContext } from "./AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import ILog from "./Log";

export default function AppLogs() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const [logs, setLogs] = useState<ILog[]>([]);
  useEffect(() => {
    const getLogs = async () => {
      const accessToken = await getAccessTokenSilently();
      const logs = (await (
        await fetch(`/api/apps/${app.id}/logs`, {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        })
      ).json()) as ILog[];
      setLogs(logs);
    };

    getLogs();
  }, []);
  return (
    <>
      <h1>Logs</h1>
      <ul>
        {logs.map((l) => (
          <li key={l.id}>
            <div className="text-sm">
              {new Date(l.timestamp).toUTCString()} {l.method} {l.path}
            </div>
          </li>
        ))}
      </ul>
    </>
  );
}
