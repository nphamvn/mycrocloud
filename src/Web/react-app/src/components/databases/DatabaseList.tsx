import { useAuth0 } from "@auth0/auth0-react";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";

interface Database {
  id: string;
  name: string;
  //description: string;
  //url: string;
  //username: string;
  //password: string;
  //port: number;
  ///database: string;
  //type: string;
  //status: string;
  createdAt: string;
  updatedAt?: string;
}
export default function DatabaseList() {
  const [dbs, setDbs] = useState<Database[]>([]);
  const id = parseInt(useParams()["id"]!);
  const { getAccessTokenSilently } = useAuth0();
  useEffect(() => {
    const getDbs = async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/databaseServers/${id}/Databases`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const dbs = (await res.json()) as Database[];
      setDbs(dbs);
    };

    getDbs();
  }, []);
  return (
    <div>
      <div className="flex">
        <h1>Databases</h1>
        <Link to={`/dbservers/${id}/dbs/new`} className="ms-auto">
          New
        </Link>
      </div>
      <ul>
        {dbs.map((db) => (
          <li key={db.id}>
            <Link to={`/dbservers/${id}/dbs/${db.id}`}>{db.name}</Link>
          </li>
        ))}
      </ul>
    </div>
  );
}
