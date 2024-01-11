import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

interface Server {
  id: string;
  name: string;
  description: string;
  //url: string;
  username: string;
  //password: string;
  //port: number;
  ///database: string;
  //type: string;
  //status: string;
  createdAt: string;
  updatedAt?: string;
}
export default function ServerList() {
  const [servers, setServers] = useState<Server[]>([]);
  const { getAccessTokenSilently } = useAuth0();
  useEffect(() => {
    const getServers = async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch("/api/databaseServers", {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const servers = (await res.json()) as Server[];
      setServers(servers);
    };

    getServers();
  }, []);
  return (
    <div>
      <div className="flex">
        <h1>Servers</h1>
        <Link to="/dbservers/new" className="ms-auto">
          New
        </Link>
      </div>
      <ul>
        {servers.map((server) => (
          <li key={server.id}>
            <Link to={`/dbservers/${server.id}`}>{server.name}</Link>
          </li>
        ))}
      </ul>
    </div>
  );
}
