import { useContext, useEffect, useState } from "react";
import ITextStorage from "./ITextStorage";
import { useAuth0 } from "@auth0/auth0-react";
import { AppContext } from "../../apps";
import { Link } from "react-router-dom";

export default function List() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();
  const [storages, setStorages] = useState<ITextStorage[]>([]);
  useEffect(() => {
    const getStorages = async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/textstorages`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const storages = (await res.json()) as ITextStorage[];
      setStorages(storages);
    };
    getStorages();
  }, []);
  return (
    <div className="p-2">
      <h1 className="font-bold">Text Storages</h1>
      <div className="mt-2">
        <Link to={"new"} className="bg-primary px-3 py-1 text-white">
          Create Storage
        </Link>
        <table className="mt-2 table-auto">
          <thead>
            <tr>
              <th>Name</th>
              <th>Size (bytes)</th>
              <th>Created At</th>
              <th>Updated At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {storages.map((s) => (
              <tr key={s.id}>
                <td>{s.name}</td>
                <td>{s.size}</td>
                <td>{new Date(s.createdAt).toISOString()}</td>
                <td>
                  {s.updatedAt ? new Date(s.updatedAt).toISOString() : "-"}
                </td>
                <td>
                  <Link to={`${s.id}`} className="text-blue-500">
                    Edit
                  </Link>
                  <Link to={`${s.id}/logon`} className="text-blue-500">
                    Logon
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
