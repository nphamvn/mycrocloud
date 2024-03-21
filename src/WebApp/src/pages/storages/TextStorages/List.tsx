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
        <table className="mt-2 w-full">
          <thead>
            <tr>
              <th className="text-start">Name</th>
              <th className="text-start">Size (bytes)</th>
              <th className="text-start">Created At</th>
              <th className="text-start">Updated At</th>
              <th className="text-start">Actions</th>
            </tr>
          </thead>
          <tbody>
            {storages.map((s) => (
              <tr key={s.id}>
                <td>{s.name}</td>
                <td>{s.size}</td>
                <td>{new Date(s.createdAt).toLocaleString()}</td>
                <td>
                  {s.updatedAt ? new Date(s.updatedAt).toLocaleString() : "-"}
                </td>
                <td className="flex space-x-2 divide-x">
                  <Link to={`${s.id}`} className=" text-blue-500">
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
