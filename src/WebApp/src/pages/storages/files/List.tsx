import {
  DocumentIcon,
  EllipsisHorizontalIcon,
  FolderIcon,
} from "@heroicons/react/24/solid";
import { useContext, useEffect, useState } from "react";
import Item from "./Item";
import { Link, useSearchParams } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { AppContext } from "../../apps";

export default function List() {
  const app = useContext(AppContext)!;
  const { getAccessTokenSilently } = useAuth0();

  const [items, setItems] = useState<Item[]>([]);
  const [searchParams] = useSearchParams();
  const folderId = searchParams.get("folderId")
    ? parseInt(searchParams.get("folderId")!)
    : undefined;

  useEffect(() => {
    const fetchItems = async (folderId?: number) => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(
        folderId
          ? `/api/apps/${app.id}/files?folderId=${folderId}`
          : `/api/apps/${app.id}/files`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      );
      const items = (await res.json()) as Item[];
      setItems(items);
    };
    fetchItems(folderId);
  }, []);

  return (
    <div className="p-2">
      <h1>Files</h1>
      <div className="mt-2 flex space-x-1">
        <button className="bg-primary px-2 py-1 text-white">Upload File</button>
        <button className="bg-primary px-2 py-1 text-white">New Folder</button>
      </div>
      <table className="w-full">
        <thead>
          <th></th>
          <th>
            <td>Name</td>
          </th>
          <th>
            <td>Size</td>
          </th>
          <th>
            <td>Created At</td>
          </th>
          <th>
            <td>Actions</td>
          </th>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              <td>
                {item.type === "file" ? (
                  <DocumentIcon className="h-4 w-4 text-blue-500" />
                ) : (
                  <FolderIcon className="h-4 w-4 text-yellow-500" />
                )}
              </td>
              <td>
                {item.type === "file" ? (
                  <>{item.name}</>
                ) : (
                  <Link to={`?folderId=${item.id}`}>{item.name}</Link>
                )}
              </td>
              <td>{item.size}MB</td>
              <td>{new Date(item.createdAt).toDateString()}</td>
              <td>
                <button>
                  <EllipsisHorizontalIcon className="h-4 w-4" />
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
