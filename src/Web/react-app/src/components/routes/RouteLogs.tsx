import { useContext, useEffect, useState } from "react";
import { AppContext } from "../apps/AppContext";
import { useAuth0 } from "@auth0/auth0-react";
import ILog from "../apps/Log";
import { useParams } from "react-router-dom";

export default function RouteLogs() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const [logs, setLogs] = useState<ILog[]>([]);
  const routeId = parseInt(useParams()["routeId"]!);
  useEffect(() => {
    const getLogs = async () => {
      const accessToken = await getAccessTokenSilently();
      const logs = (await (
        await fetch(`/api/apps/${app.id}/logs?routeId=${routeId}`, {
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
    <div className="p-2">
      <ul>
        {logs.map((l) => (
          <li key={l.id}>
            <div className="text-sm">
              {new Date(l.timestamp).toUTCString()} {l.method} {l.path}
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
